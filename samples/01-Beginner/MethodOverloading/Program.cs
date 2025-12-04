// SCENARIO: Method overloading - Farklı parametre imzaları
// BAD PRACTICE: Her varyant için farklı isim (AddInt, AddDouble, AddThree)
// GOOD PRACTICE: Aynı isim, farklı parametreler - Overload resolution

using MethodOverloading;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Method Overloading Patterns ===\n");

        Calculator calc = new();

        Console.WriteLine("=== 1. Parametre Sayısına Göre Overload ===\n");
        calc.Add(5, 3);           // int Add(int, int)
        calc.Add(5, 3, 2);        // int Add(int, int, int)

        Console.WriteLine("\n=== 2. Parametre Türüne Göre Overload ===\n");
        calc.Add(1, 2);           // int
        calc.Add(1.5, 2.5);       // double
        calc.Add("Hello", "World"); // string

        Console.WriteLine("\n=== 3. Params Keyword ===\n");
        calc.Add(1, 2, 3, 4, 5);  // params int[]

        Console.WriteLine("\n=== 4. Optional Parameters ===\n");
        calc.Multiply(5);         // 5 * 1 * 1 = 5
        calc.Multiply(5, 2);      // 5 * 2 * 1 = 10
        calc.Multiply(5, 2, 3);   // 5 * 2 * 3 = 30

        Console.WriteLine("\n=== 5. Named Arguments ===\n");
        calc.Calculate(value: 1000, rate: 0.05, years: 10);
        calc.Calculate(years: 5, value: 2000, rate: 0.03); // Sıra önemsiz

        Console.WriteLine("\n=== 6. Ref/Out Overloads ===\n");
        int x = 10;
        calc.Process(x);          // Process(int)
        calc.Process(ref x);      // Process(ref int)
        Console.WriteLine($"x after ref: {x}");

        calc.Process(out int y);  // Process(out int)
        Console.WriteLine($"y after out: {y}");

        Console.WriteLine("\n=== 7. Overload Resolution ===\n");
        DemonstrateOverloadResolution();

        Console.WriteLine("\n=== Analysis ===");
        Console.WriteLine("• Overload resolution: Compile-time");
        Console.WriteLine("• Parametre sayı/tür farklı olmalı");
        Console.WriteLine("• Return type overload için yeterli DEĞİL");
        Console.WriteLine("• Optional params > params keyword > normal params");
    }

    static void DemonstrateOverloadResolution()
    {
        Calculator calc = new();

        // Exact match
        int result1 = calc.Add(1, 2);           // Add(int, int)

        // Implicit conversion
        double result2 = calc.Add(1.5, 2.5);    // Add(double, double)

        // params kullanımı
        int result3 = calc.Add(1, 2, 3, 4);     // Add(params int[])

        // Ambiguous call - Dikkat!
        // calc.Add(1, 2);  // İki overload da match eder: Add(int,int) ve Add(params int[])
                            // Compiler en spesifik olanı seçer: Add(int,int)

        Console.WriteLine("✅ Overload resolution başarılı");
    }
}
