using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicCodeGeneration;

/// <summary>
/// Demonstrates runtime code generation using Reflection.Emit and IL opcodes.
/// Create types and methods dynamically at runtime.
/// </summary>
class Program
{
    static void Main()
    {
        Console.WriteLine("=== Dynamic Code Generation Example ===\n");

        // 1. Create a dynamic type
        var calc = CreateCalculatorType();
        Console.WriteLine($"✅ Created dynamic type: {calc.Name}");

        // 2. Create instance and invoke method
        var instance = Activator.CreateInstance(calc)!;
        var addMethod = calc.GetMethod("Add")!;
        var result = (int)addMethod.Invoke(instance, new object[] { 10, 32 })!;
        Console.WriteLine($"✅ Dynamic Add(10, 32) = {result}");

        // 3. Create a dynamic method (lighter weight)
        var dynamicMultiply = CreateMultiplyMethod();
        var product = (int)dynamicMultiply.Invoke(null, new object[] { 6, 7 })!;
        Console.WriteLine($"✅ Dynamic Multiply(6, 7) = {product}");

        Console.WriteLine("\n✅ Dynamic code generation complete!");
    }

    static Type CreateCalculatorType()
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
        il.Emit(OpCodes.Ldarg_1);  // Load first argument (a)
        il.Emit(OpCodes.Ldarg_2);  // Load second argument (b)
        il.Emit(OpCodes.Add);      // Add them
        il.Emit(OpCodes.Ret);      // Return result

        return typeBuilder.CreateType()!;
    }

    static MethodInfo CreateMultiplyMethod()
    {
        var dynamicMethod = new DynamicMethod(
            "Multiply",
            typeof(int),
            new[] { typeof(int), typeof(int) },
            typeof(Program).Module);

        var il = dynamicMethod.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);  // Load first argument
        il.Emit(OpCodes.Ldarg_1);  // Load second argument
        il.Emit(OpCodes.Mul);      // Multiply
        il.Emit(OpCodes.Ret);      // Return

        return dynamicMethod;
    }
}
