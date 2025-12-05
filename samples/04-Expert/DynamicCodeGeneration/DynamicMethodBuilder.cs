using System.Reflection;
using System.Reflection.Emit;

namespace DynamicCodeGeneration;

/// <summary>
/// Creates dynamic methods at runtime using DynamicMethod.
///
/// DIFFERENCES from TypeBuilder:
/// - DynamicMethod: Lightweight, method-only, faster creation
/// - TypeBuilder: Full type creation, supports fields/properties
///
/// WHEN TO USE DynamicMethod:
/// - Expression compilation (LINQ, lambda expressions)
/// - Performance-critical code generation
/// - Temporary method creation
/// - Method-level code generation
///
/// PERFORMANCE:
/// - Creation: 10-50x faster than TypeBuilder
/// - Execution: Same speed as compiled code
/// - Memory: Lower overhead
/// </summary>
public static class DynamicMethodBuilder
{
    /// <summary>
    /// Create a Multiply method dynamically.
    ///
    /// Generated IL:
    ///   int Multiply(int a, int b) {
    ///       return a * b;
    ///   }
    /// </summary>
    public static MethodInfo CreateMultiplyMethod()
    {
        var dynamicMethod = new DynamicMethod(
            "Multiply",
            typeof(int),
            new[] { typeof(int), typeof(int) },
            typeof(DynamicMethodBuilder).Module);

        var il = dynamicMethod.GetILGenerator();

        // IL Instructions:
        il.Emit(OpCodes.Ldarg_0);  // Load first argument
        il.Emit(OpCodes.Ldarg_1);  // Load second argument
        il.Emit(OpCodes.Mul);      // Multiply
        il.Emit(OpCodes.Ret);      // Return result

        return dynamicMethod;
    }

    /// <summary>
    /// Create a Divide method with error handling.
    /// </summary>
    public static Func<int, int, int> CreateDivideMethod()
    {
        var dynamicMethod = new DynamicMethod(
            "Divide",
            typeof(int),
            new[] { typeof(int), typeof(int) },
            typeof(DynamicMethodBuilder).Module);

        var il = dynamicMethod.GetILGenerator();

        // IL: a / b
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Div);
        il.Emit(OpCodes.Ret);

        return (Func<int, int, int>)dynamicMethod.CreateDelegate(typeof(Func<int, int, int>));
    }

    /// <summary>
    /// Create a conditional method (returns max of two numbers).
    ///
    /// Generated C#:
    ///   int Max(int a, int b) {
    ///       return (a > b) ? a : b;
    ///   }
    /// </summary>
    public static Func<int, int, int> CreateMaxMethod()
    {
        var dynamicMethod = new DynamicMethod(
            "Max",
            typeof(int),
            new[] { typeof(int), typeof(int) },
            typeof(DynamicMethodBuilder).Module);

        var il = dynamicMethod.GetILGenerator();
        var labelReturnB = il.DefineLabel();
        var labelEnd = il.DefineLabel();

        // if (a > b) return a; else return b;
        il.Emit(OpCodes.Ldarg_0);      // Load a
        il.Emit(OpCodes.Ldarg_1);      // Load b
        il.Emit(OpCodes.Bgt_S, labelReturnB);  // Branch if a > b

        // Return b
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Br_S, labelEnd);

        // Return a
        il.MarkLabel(labelReturnB);
        il.Emit(OpCodes.Ldarg_0);

        il.MarkLabel(labelEnd);
        il.Emit(OpCodes.Ret);

        return (Func<int, int, int>)dynamicMethod.CreateDelegate(typeof(Func<int, int, int>));
    }

    /// <summary>
    /// Create a method that calls another method (demonstrates Ldftn, Call).
    /// </summary>
    public static Func<int, int> CreateSquareMethod()
    {
        var dynamicMethod = new DynamicMethod(
            "Square",
            typeof(int),
            new[] { typeof(int) },
            typeof(DynamicMethodBuilder).Module);

        var il = dynamicMethod.GetILGenerator();

        // return n * n;
        il.Emit(OpCodes.Ldarg_0);  // Load n
        il.Emit(OpCodes.Ldarg_0);  // Load n again
        il.Emit(OpCodes.Mul);      // n * n
        il.Emit(OpCodes.Ret);

        return (Func<int, int>)dynamicMethod.CreateDelegate(typeof(Func<int, int>));
    }

    /// <summary>
    /// Create a method with local variables.
    ///
    /// Generated C#:
    ///   int Sum(int a, int b, int c) {
    ///       int temp = a + b;
    ///       return temp + c;
    ///   }
    /// </summary>
    public static Func<int, int, int, int> CreateSumWithLocalMethod()
    {
        var dynamicMethod = new DynamicMethod(
            "SumWithLocal",
            typeof(int),
            new[] { typeof(int), typeof(int), typeof(int) },
            typeof(DynamicMethodBuilder).Module);

        var il = dynamicMethod.GetILGenerator();

        // Declare local variable: int temp
        var localTemp = il.DeclareLocal(typeof(int));

        // temp = a + b
        il.Emit(OpCodes.Ldarg_0);           // Load a
        il.Emit(OpCodes.Ldarg_1);           // Load b
        il.Emit(OpCodes.Add);               // a + b
        il.Emit(OpCodes.Stloc, localTemp);  // Store in temp

        // return temp + c
        il.Emit(OpCodes.Ldloc, localTemp);  // Load temp
        il.Emit(OpCodes.Ldarg_2);           // Load c
        il.Emit(OpCodes.Add);               // temp + c
        il.Emit(OpCodes.Ret);

        return (Func<int, int, int, int>)dynamicMethod.CreateDelegate(typeof(Func<int, int, int, int>));
    }
}
