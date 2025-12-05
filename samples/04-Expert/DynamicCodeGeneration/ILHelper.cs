using System.Reflection.Emit;

namespace DynamicCodeGeneration;

/// <summary>
/// Helper class explaining IL opcodes used in dynamic code generation.
///
/// IL (Intermediate Language) is the low-level bytecode that .NET uses.
/// Understanding IL is crucial for dynamic code generation.
/// </summary>
public static class ILHelper
{
    /// <summary>
    /// Print explanation of common IL opcodes.
    /// </summary>
    public static void PrintOpcodeReference()
    {
        Console.WriteLine("\n=== IL Opcode Reference ===\n");

        Console.WriteLine("LOADING ARGUMENTS:");
        Console.WriteLine("  Ldarg_0  - Load argument 0 (this, or first argument if static)");
        Console.WriteLine("  Ldarg_1  - Load argument 1 (first parameter)");
        Console.WriteLine("  Ldarg_2  - Load argument 2 (second parameter)");
        Console.WriteLine("  Ldarg_S  - Load argument by index (0-255)");

        Console.WriteLine("\nLOADING LOCALS:");
        Console.WriteLine("  Ldloc_0  - Load local variable 0");
        Console.WriteLine("  Ldloc_1  - Load local variable 1");
        Console.WriteLine("  Ldloc_S  - Load local variable by index");
        Console.WriteLine("  Stloc_0  - Store to local variable 0");
        Console.WriteLine("  Stloc_S  - Store to local variable by index");

        Console.WriteLine("\nARITHMETIC:");
        Console.WriteLine("  Add      - Add two values");
        Console.WriteLine("  Sub      - Subtract");
        Console.WriteLine("  Mul      - Multiply");
        Console.WriteLine("  Div      - Divide");
        Console.WriteLine("  Rem      - Remainder (modulo)");

        Console.WriteLine("\nCOMPARISON:");
        Console.WriteLine("  Ceq      - Compare equal (push 1 if equal, 0 otherwise)");
        Console.WriteLine("  Cgt      - Compare greater than");
        Console.WriteLine("  Clt      - Compare less than");

        Console.WriteLine("\nBRANCHING:");
        Console.WriteLine("  Br       - Unconditional branch");
        Console.WriteLine("  Brtrue   - Branch if true");
        Console.WriteLine("  Brfalse  - Branch if false");
        Console.WriteLine("  Beq      - Branch if equal");
        Console.WriteLine("  Bgt      - Branch if greater than");
        Console.WriteLine("  Blt      - Branch if less than");

        Console.WriteLine("\nMETHOD CALLS:");
        Console.WriteLine("  Call     - Call a method");
        Console.WriteLine("  Callvirt - Call a virtual method");
        Console.WriteLine("  Ret      - Return from method");

        Console.WriteLine("\nOBJECTS:");
        Console.WriteLine("  Newobj   - Create new object");
        Console.WriteLine("  Ldfld    - Load field value");
        Console.WriteLine("  Stfld    - Store field value");
        Console.WriteLine("  Ldstr    - Load string literal");

        Console.WriteLine("\nSTACK OPERATIONS:");
        Console.WriteLine("  Pop      - Remove top of stack");
        Console.WriteLine("  Dup      - Duplicate top of stack");
        Console.WriteLine("  Nop      - No operation (debugging)");
        Console.WriteLine();
    }

    /// <summary>
    /// Example: Print IL for a simple Add method.
    /// </summary>
    public static void PrintAddMethodIL()
    {
        Console.WriteLine("\n=== IL for Add(int a, int b) ===\n");
        Console.WriteLine("  .method public hidebysig static");
        Console.WriteLine("      int32 Add(int32 a, int32 b) cil managed");
        Console.WriteLine("  {");
        Console.WriteLine("      .maxstack 2");
        Console.WriteLine("      IL_0000: ldarg.0      // Load a onto stack");
        Console.WriteLine("      IL_0001: ldarg.1      // Load b onto stack");
        Console.WriteLine("      IL_0002: add          // Add top two stack values");
        Console.WriteLine("      IL_0003: ret          // Return (pop stack as return value)");
        Console.WriteLine("  }");
        Console.WriteLine();
    }

    /// <summary>
    /// Example: Print IL for a conditional method.
    /// </summary>
    public static void PrintMaxMethodIL()
    {
        Console.WriteLine("\n=== IL for Max(int a, int b) ===\n");
        Console.WriteLine("  .method public hidebysig static");
        Console.WriteLine("      int32 Max(int32 a, int32 b) cil managed");
        Console.WriteLine("  {");
        Console.WriteLine("      .maxstack 2");
        Console.WriteLine("      IL_0000: ldarg.0      // Load a");
        Console.WriteLine("      IL_0001: ldarg.1      // Load b");
        Console.WriteLine("      IL_0002: bgt.s IL_0006  // Branch if a > b");
        Console.WriteLine("      IL_0004: ldarg.1      // Load b");
        Console.WriteLine("      IL_0005: ret          // Return b");
        Console.WriteLine("      IL_0006: ldarg.0      // Load a");
        Console.WriteLine("      IL_0007: ret          // Return a");
        Console.WriteLine("  }");
        Console.WriteLine();
    }
}
