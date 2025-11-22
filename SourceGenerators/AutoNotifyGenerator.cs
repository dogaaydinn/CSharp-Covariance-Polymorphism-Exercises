using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceGenerators;

/// <summary>
/// Source generator that automatically implements INotifyPropertyChanged pattern
/// for properties marked with [AutoNotify] attribute.
///
/// Example usage:
/// <code>
/// [AutoNotify]
/// private string _name;
///
/// // Generates:
/// public string Name
/// {
///     get => _name;
///     set
///     {
///         if (_name != value)
///         {
///             _name = value;
///             OnPropertyChanged(nameof(Name));
///         }
///     }
/// }
/// </code>
/// </summary>
[Generator]
public class AutoNotifyGenerator : ISourceGenerator
{
    private const string AttributeText = @"
using System;

namespace SourceGenerators.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class AutoNotifyAttribute : Attribute
    {
        public AutoNotifyAttribute()
        {
        }

        public string? PropertyName { get; set; }
    }
}";

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // Add the attribute to the compilation
        context.AddSource("AutoNotifyAttribute.g.cs", SourceText.From(AttributeText, Encoding.UTF8));

        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            return;

        // Group fields by containing class
        var groupedFields = receiver.CandidateFields
            .GroupBy(f => f.Parent as TypeDeclarationSyntax)
            .Where(g => g.Key != null);

        foreach (var group in groupedFields)
        {
            var classDeclaration = group.Key!;
            var semanticModel = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

            if (classSymbol == null)
                continue;

            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
            var className = classSymbol.Name;

            var source = GenerateClass(namespaceName, className, group.ToList(), semanticModel);
            context.AddSource($"{className}_AutoNotify.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private string GenerateClass(
        string namespaceName,
        string className,
        List<FieldDeclarationSyntax> fields,
        SemanticModel semanticModel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System.ComponentModel;");
        sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine();
        sb.AppendLine($"namespace {namespaceName}");
        sb.AppendLine("{");
        sb.AppendLine($"    partial class {className} : INotifyPropertyChanged");
        sb.AppendLine("    {");
        sb.AppendLine("        public event PropertyChangedEventHandler? PropertyChanged;");
        sb.AppendLine();
        sb.AppendLine("        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)");
        sb.AppendLine("        {");
        sb.AppendLine("            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));");
        sb.AppendLine("        }");
        sb.AppendLine();

        foreach (var fieldDeclaration in fields)
        {
            var variable = fieldDeclaration.Declaration.Variables.First();
            var fieldName = variable.Identifier.Text;
            var fieldSymbol = semanticModel.GetDeclaredSymbol(variable);

            if (fieldSymbol == null)
                continue;

            var fieldType = fieldSymbol.Type.ToDisplayString();
            var propertyName = GetPropertyName(fieldName);

            sb.AppendLine($"        public {fieldType} {propertyName}");
            sb.AppendLine("        {");
            sb.AppendLine($"            get => {fieldName};");
            sb.AppendLine("            set");
            sb.AppendLine("            {");
            sb.AppendLine($"                if (!System.Collections.Generic.EqualityComparer<{fieldType}>.Default.Equals({fieldName}, value))");
            sb.AppendLine("                {");
            sb.AppendLine($"                    {fieldName} = value;");
            sb.AppendLine($"                    OnPropertyChanged();");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private string GetPropertyName(string fieldName)
    {
        // Remove leading underscore and capitalize first letter
        if (fieldName.StartsWith("_"))
            fieldName = fieldName.Substring(1);

        if (fieldName.Length == 0)
            return string.Empty;

        return char.ToUpper(fieldName[0]) + fieldName.Substring(1);
    }

    class SyntaxReceiver : ISyntaxReceiver
    {
        public List<FieldDeclarationSyntax> CandidateFields { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Find fields with [AutoNotify] attribute
            if (syntaxNode is FieldDeclarationSyntax fieldDeclaration &&
                fieldDeclaration.AttributeLists.Count > 0)
            {
                foreach (var attributeList in fieldDeclaration.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        var name = attribute.Name.ToString();
                        if (name == "AutoNotify" || name == "AutoNotifyAttribute")
                        {
                            CandidateFields.Add(fieldDeclaration);
                            break;
                        }
                    }
                }
            }
        }
    }
}
