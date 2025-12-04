using CastingExamples.Examples;

namespace CastingExamples;

/// <summary>
/// Casting Examples - Understanding Type Conversion in C#
///
/// This sample demonstrates:
/// 1. Upcasting (implicit casting to base type)
/// 2. Downcasting (explicit casting to derived type)
/// 3. 'is' operator (type checking)
/// 4. 'as' operator (safe casting)
/// 5. Pattern matching with 'is'
/// 6. Common casting pitfalls
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        PrintHeader();

        while (true)
        {
            PrintMenu();

            Console.Write("\nEnter your choice (1-7, or 0 to exit): ");
            var choice = Console.ReadLine();

            Console.Clear();

            switch (choice)
            {
                case "1":
                    UpcastingExample.Run();
                    break;
                case "2":
                    DowncastingExample.Run();
                    break;
                case "3":
                    IsOperatorExample.Run();
                    break;
                case "4":
                    AsOperatorExample.Run();
                    break;
                case "5":
                    PatternMatchingExample.Run();
                    break;
                case "6":
                    CastingPitfallsExample.Run();
                    break;
                case "7":
                    RealWorldExample.Run();
                    break;
                case "0":
                    Console.WriteLine("\n👋 Thanks for learning about casting!\n");
                    return;
                default:
                    Console.WriteLine("\n❌ Invalid choice. Please try again.\n");
                    continue;
            }

            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey(true);
            Console.Clear();
        }
    }

    static void PrintHeader()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                                                                ║");
        Console.WriteLine("║              CASTING EXAMPLES - TYPE CONVERSION                ║");
        Console.WriteLine("║                                                                ║");
        Console.WriteLine("║        Master upcasting, downcasting, and safe casting        ║");
        Console.WriteLine("║                                                                ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void PrintMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n📚 EXAMPLES:");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("  1️⃣   Upcasting (Implicit)");
        Console.WriteLine("      └─ Converting derived type to base type automatically");
        Console.WriteLine();

        Console.WriteLine("  2️⃣   Downcasting (Explicit)");
        Console.WriteLine("      └─ Converting base type to derived type with cast operator");
        Console.WriteLine();

        Console.WriteLine("  3️⃣   'is' Operator");
        Console.WriteLine("      └─ Type checking before casting");
        Console.WriteLine();

        Console.WriteLine("  4️⃣   'as' Operator");
        Console.WriteLine("      └─ Safe casting that returns null on failure");
        Console.WriteLine();

        Console.WriteLine("  5️⃣   Pattern Matching");
        Console.WriteLine("      └─ Modern C# pattern matching with 'is'");
        Console.WriteLine();

        Console.WriteLine("  6️⃣   Casting Pitfalls");
        Console.WriteLine("      └─ Common mistakes and InvalidCastException");
        Console.WriteLine();

        Console.WriteLine("  7️⃣   Real-World Example");
        Console.WriteLine("      └─ Shape processing system with safe casting");
        Console.WriteLine();

        Console.WriteLine("  0️⃣   Exit");
        Console.WriteLine();
    }
}
