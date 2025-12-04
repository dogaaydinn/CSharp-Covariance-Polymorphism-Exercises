using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace AdvancedConcepts.SourceGenerators.Tests.Helpers;

/// <summary>
/// Helper class for testing source generators with Roslyn testing framework.
/// </summary>
public static class GeneratorTestHelper
{
    /// <summary>
    /// Verifies that the generator produces output containing the expected strings.
    /// </summary>
    public static async Task VerifyGeneratorContainsAsync<TSourceGenerator>(
        string sourceCode,
        params string[] expectedContents)
        where TSourceGenerator : IIncrementalGenerator, new()
    {
        await Task.CompletedTask; // Make method async for consistency

        var compilation = CreateCompilation(sourceCode);
        var generator = new TSourceGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var diagnostics);

        diagnostics.Should().BeEmpty("generator should not produce diagnostics");

        var runResult = driver.GetRunResult();
        var generatorResult = runResult.Results.Should().ContainSingle().Subject;

        var generatedSource = string.Join("\n",
            generatorResult.GeneratedSources.Select(s => s.SourceText.ToString()));

        foreach (var expected in expectedContents)
        {
            generatedSource.Should().Contain(expected,
                $"generated source should contain '{expected}'");
        }
    }

    /// <summary>
    /// Creates a CSharp compilation from source code.
    /// </summary>
    public static CSharpCompilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Cast<MetadataReference>()
            .ToList();

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return compilation;
    }

    /// <summary>
    /// Runs a generator and returns the generated sources.
    /// </summary>
    public static GeneratorDriverRunResult RunGenerator<TSourceGenerator>(string sourceCode)
        where TSourceGenerator : IIncrementalGenerator, new()
    {
        var compilation = CreateCompilation(sourceCode);
        var generator = new TSourceGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out _,
            out _);

        return driver.GetRunResult();
    }

    /// <summary>
    /// Asserts that the generator produces no diagnostics.
    /// </summary>
    public static void AssertNoDiagnostics(GeneratorDriverRunResult runResult)
    {
        runResult.Diagnostics.Should().BeEmpty("generator should not produce diagnostics");
    }

    /// <summary>
    /// Gets all generated source texts from a generator run.
    /// </summary>
    public static IEnumerable<string> GetGeneratedSources(GeneratorDriverRunResult runResult)
    {
        return runResult.GeneratedTrees.Select(tree => tree.ToString());
    }
}
