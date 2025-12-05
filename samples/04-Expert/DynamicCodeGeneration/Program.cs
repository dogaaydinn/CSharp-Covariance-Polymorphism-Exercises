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

        // 1. Create a dynamic type using TypeBuilder
        var calc = DynamicTypeBuilder.CreateCalculatorType();
        Console.WriteLine($"✅ Created dynamic type: {calc.Name}");

        // 2. Create instance and invoke method
        var instance = Activator.CreateInstance(calc)!;
        var addMethod = calc.GetMethod("Add")!;
        var result = (int)addMethod.Invoke(instance, new object[] { 10, 32 })!;
        Console.WriteLine($"✅ Dynamic Add(10, 32) = {result}");

        // 3. Create a dynamic method (lighter weight) using DynamicMethod
        var dynamicMultiply = DynamicMethodBuilder.CreateMultiplyMethod();
        var product = (int)dynamicMultiply.Invoke(null, new object[] { 6, 7 })!;
        Console.WriteLine($"✅ Dynamic Multiply(6, 7) = {product}");

        // 4. Show IL opcode reference
        ILHelper.PrintOpcodeReference();
        ILHelper.PrintAddMethodIL();

        Console.WriteLine("✅ Dynamic code generation complete!");
    }
}
