using System.Reflection;
using System.Reflection.Emit;

namespace DynamicCodeGeneration;

/// <summary>
/// Creates dynamic types at runtime using TypeBuilder.
///
/// USE CASES:
/// - ORM frameworks (Entity Framework, Dapper)
/// - Serialization libraries (Newtonsoft.Json, System.Text.Json)
/// - Proxy generation (Castle DynamicProxy)
/// - Mock frameworks (Moq, NSubstitute)
///
/// PERFORMANCE:
/// - Type creation: One-time overhead (100-500ms)
/// - Method invocation: Same as compiled code after JIT
/// </summary>
public static class DynamicTypeBuilder
{
    /// <summary>
    /// Create a Calculator type with an Add method at runtime.
    ///
    /// Generated IL:
    ///   public class Calculator {
    ///       public int Add(int a, int b) {
    ///           return a + b;
    ///       }
    ///   }
    /// </summary>
    public static Type CreateCalculatorType()
    {
        var assemblyName = new AssemblyName("DynamicAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            assemblyName,
            AssemblyBuilderAccess.Run);

        var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
        var typeBuilder = moduleBuilder.DefineType(
            "Calculator",
            TypeAttributes.Public);

        // Define Add method: public int Add(int a, int b) { return a + b; }
        var methodBuilder = typeBuilder.DefineMethod(
            "Add",
            MethodAttributes.Public,
            typeof(int),
            new[] { typeof(int), typeof(int) });

        var il = methodBuilder.GetILGenerator();

        // IL Instructions:
        il.Emit(OpCodes.Ldarg_1);  // Load first argument (a)
        il.Emit(OpCodes.Ldarg_2);  // Load second argument (b)
        il.Emit(OpCodes.Add);      // Add them together
        il.Emit(OpCodes.Ret);      // Return result

        return typeBuilder.CreateType()!;
    }

    /// <summary>
    /// Create a more complex type with multiple methods.
    /// </summary>
    public static Type CreateAdvancedCalculatorType()
    {
        var assemblyName = new AssemblyName("AdvancedCalculatorAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            assemblyName,
            AssemblyBuilderAccess.Run);

        var moduleBuilder = assemblyBuilder.DefineDynamicModule("AdvancedModule");
        var typeBuilder = moduleBuilder.DefineType(
            "AdvancedCalculator",
            TypeAttributes.Public);

        // Add Subtract method
        DefineSubtractMethod(typeBuilder);

        // Add Multiply method
        DefineMultiplyMethod(typeBuilder);

        // Add Divide method
        DefineDivideMethod(typeBuilder);

        return typeBuilder.CreateType()!;
    }

    private static void DefineSubtractMethod(TypeBuilder typeBuilder)
    {
        var methodBuilder = typeBuilder.DefineMethod(
            "Subtract",
            MethodAttributes.Public,
            typeof(int),
            new[] { typeof(int), typeof(int) });

        var il = methodBuilder.GetILGenerator();
        il.Emit(OpCodes.Ldarg_1);  // Load a
        il.Emit(OpCodes.Ldarg_2);  // Load b
        il.Emit(OpCodes.Sub);      // a - b
        il.Emit(OpCodes.Ret);
    }

    private static void DefineMultiplyMethod(TypeBuilder typeBuilder)
    {
        var methodBuilder = typeBuilder.DefineMethod(
            "Multiply",
            MethodAttributes.Public,
            typeof(int),
            new[] { typeof(int), typeof(int) });

        var il = methodBuilder.GetILGenerator();
        il.Emit(OpCodes.Ldarg_1);  // Load a
        il.Emit(OpCodes.Ldarg_2);  // Load b
        il.Emit(OpCodes.Mul);      // a * b
        il.Emit(OpCodes.Ret);
    }

    private static void DefineDivideMethod(TypeBuilder typeBuilder)
    {
        var methodBuilder = typeBuilder.DefineMethod(
            "Divide",
            MethodAttributes.Public,
            typeof(int),
            new[] { typeof(int), typeof(int) });

        var il = methodBuilder.GetILGenerator();
        il.Emit(OpCodes.Ldarg_1);  // Load a
        il.Emit(OpCodes.Ldarg_2);  // Load b
        il.Emit(OpCodes.Div);      // a / b
        il.Emit(OpCodes.Ret);
    }
}
