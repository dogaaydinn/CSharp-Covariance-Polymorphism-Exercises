// SCENARIO: Hesap makinesi sınıfı - Method Overloading
// BAD PRACTICE: object parametre ile runtime type checking
// GOOD PRACTICE: Method overloading ile compile-time type safety
// BEST PRACTICE: Generic methods ile reusable code

using MethodOverloading;

class Program
{
    static void Main()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   🧮  METHOD OVERLOADING - HESAP MAKİNESİ ÖRNEĞİ       ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // 1. BAD PRACTICE DEMO
        Console.WriteLine("═══ 1. ❌ BAD PRACTICE: object Parametre ═══\n");
        DemonstrateBadPractice();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 2. GOOD PRACTICE DEMO - Parameter count overloads
        Console.WriteLine("═══ 2. ✅ PARAMETRE SAYISINA GÖRE OVERLOAD ═══\n");
        DemonstrateParameterCountOverloads();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 3. GOOD PRACTICE DEMO - Parameter type overloads
        Console.WriteLine("═══ 3. ✅ PARAMETRE TİPİNE GÖRE OVERLOAD ═══\n");
        DemonstrateParameterTypeOverloads();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 4. Array vs params overloads
        Console.WriteLine("═══ 4. ✅ ARRAY vs PARAMS OVERLOAD ═══\n");
        DemonstrateArrayVsParamsOverloads();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 5. Optional parameters
        Console.WriteLine("═══ 5. ✅ OPTIONAL PARAMETERS ═══\n");
        DemonstrateOptionalParameters();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 6. Named arguments
        Console.WriteLine("═══ 6. ✅ NAMED ARGUMENTS ═══\n");
        DemonstrateNamedArguments();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 7. ref/out/in overloads
        Console.WriteLine("═══ 7. ✅ REF vs OUT vs IN OVERLOADS ═══\n");
        DemonstrateRefOutInOverloads();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 8. Tuple return overloads
        Console.WriteLine("═══ 8. ✅ TUPLE RETURN OVERLOADS ═══\n");
        DemonstrateTupleReturnOverloads();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 9. BEST PRACTICE DEMO - Generic methods
        Console.WriteLine("═══ 9. ✨ BEST PRACTICE: GENERIC METHODS ═══\n");
        DemonstrateGenericMethods();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 10. Overload resolution
        Console.WriteLine("═══ 10. 🔍 OVERLOAD RESOLUTION ═══\n");
        DemonstrateOverloadResolution();

        // Final Summary
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    📊 ÖZET                                ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("✅ ÖĞRENİLENLER:");
        Console.WriteLine("   • Method overloading: Aynı isim, farklı imza");
        Console.WriteLine("   • Compile-time resolution: Type-safe, hızlı");
        Console.WriteLine("   • 7 farklı overload tipi: count, type, array, params, ref/out/in, tuple, optional");
        Console.WriteLine("   • Bad practice: object parametre (runtime errors)");
        Console.WriteLine("   • Good practice: Overloading (compile-time safety)");
        Console.WriteLine("   • Best practice: Generics (reusable, type-safe)");
        Console.WriteLine();
        Console.WriteLine("💡 BEST PRACTICES:");
        Console.WriteLine("   • object parametre kullanma (runtime type checking gerekir)");
        Console.WriteLine("   • Overload kullan (compile-time type safety)");
        Console.WriteLine("   • Generic methods kullan (reusable code)");
        Console.WriteLine("   • Optional parameters kullan (flexibility)");
        Console.WriteLine("   • Named arguments kullan (clarity)");
    }

    /// <summary>
    /// ❌ BAD PRACTICE: object parametre ile runtime type checking
    /// </summary>
    static void DemonstrateBadPractice()
    {
        Console.WriteLine("💀 Bad Practice: object parametre kullanımı\n");

        BadCalculator badCalc = new();

        try
        {
            // ❌ Runtime type checking gerekir
            object result1 = badCalc.Add(5, 10);
            Console.WriteLine($"Result 1: {result1} (runtime checked)");

            object result2 = badCalc.Add(3.5, 2.5);
            Console.WriteLine($"Result 2: {result2} (runtime checked)");

            // ❌ Magic strings + runtime checking
            object result3 = badCalc.Calculate(5, 10, "add");
            Console.WriteLine($"Result 3: {result3}");

            // ❌ Runtime error - compile-time'da yakalanamaz!
            // object result4 = badCalc.Add("Hello", 10);  // ArgumentException!
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ HATA: {ex.Message}");
        }

        Console.WriteLine("\n⚠️  SORUNLAR:");
        Console.WriteLine("   • Runtime type checking (YAVAŞ)");
        Console.WriteLine("   • No IntelliSense support");
        Console.WriteLine("   • Runtime errors (compile-time'da yakalanamaz)");
        Console.WriteLine("   • Boxing/unboxing overhead (performance)");
        Console.WriteLine("   • Magic strings (typo riski)");
    }

    /// <summary>
    /// ✅ GOOD PRACTICE: Parametre sayısına göre overload
    /// </summary>
    static void DemonstrateParameterCountOverloads()
    {
        Console.WriteLine("🎯 Parametre sayısına göre farklı overload'lar:\n");

        GoodCalculator calc = new();

        // 2 parametre
        int result1 = calc.Add(5, 10);

        // 3 parametre
        int result2 = calc.Add(5, 10, 15);

        // 4 parametre
        int result3 = calc.Add(5, 10, 15, 20);

        Console.WriteLine($"\nToplam Sonuçlar: {result1}, {result2}, {result3}");
        Console.WriteLine("\n✅ AVANTAJLAR:");
        Console.WriteLine("   • Aynı isim (Add) - mantıksal tutarlılık");
        Console.WriteLine("   • Farklı parametre sayıları - esneklik");
        Console.WriteLine("   • Compile-time resolution - type-safe");
        Console.WriteLine("   • IntelliSense support - developer experience");
    }

    /// <summary>
    /// ✅ GOOD PRACTICE: Parametre tipine göre overload
    /// </summary>
    static void DemonstrateParameterTypeOverloads()
    {
        Console.WriteLine("🎯 Parametre tipine göre farklı overload'lar:\n");

        GoodCalculator calc = new();

        // int overload
        int intResult = calc.Add(5, 10);

        // double overload
        double doubleResult = calc.Add(3.5, 2.5);

        // decimal overload
        decimal decimalResult = calc.Add(10.5m, 5.25m);

        // string overload
        string stringResult = calc.Add("Hello", "World");

        Console.WriteLine($"\nSonuçlar: int={intResult}, double={doubleResult}, decimal={decimalResult}, string=\"{stringResult}\"");
        Console.WriteLine("\n✅ AVANTAJLAR:");
        Console.WriteLine("   • Farklı tipler için özelleştirilmiş davranış");
        Console.WriteLine("   • Compile-time type checking");
        Console.WriteLine("   • No boxing/unboxing overhead");
        Console.WriteLine("   • Type-specific optimizations");
    }

    /// <summary>
    /// ✅ GOOD PRACTICE: Array vs params overload
    /// </summary>
    static void DemonstrateArrayVsParamsOverloads()
    {
        Console.WriteLine("🎯 Array vs params overload:\n");

        GoodCalculator calc = new();

        // Explicit array
        int[] numbers = { 1, 2, 3, 4, 5 };
        int result1 = calc.Add(numbers);

        // params - variable arguments
        int result2 = calc.AddMany(1, 2, 3, 4, 5);
        int result3 = calc.AddMany(10, 20, 30);
        int result4 = calc.AddMany(100);

        Console.WriteLine($"\nSonuçlar: array={result1}, params5={result2}, params3={result3}, params1={result4}");
        Console.WriteLine("\n✅ FARKLAR:");
        Console.WriteLine("   • Add(int[]): Explicit array gerekir - int[] arr = {1,2,3}; Add(arr);");
        Console.WriteLine("   • AddMany(params int[]): Variable arguments - AddMany(1, 2, 3, 4, 5);");
        Console.WriteLine("   • params daha esnek - any number of arguments");
        Console.WriteLine("   • params compile-time'da array'e dönüştürülür");
    }

    /// <summary>
    /// ✅ GOOD PRACTICE: Optional parameters (default values)
    /// </summary>
    static void DemonstrateOptionalParameters()
    {
        Console.WriteLine("🎯 Optional parameters ile esneklik:\n");

        GoodCalculator calc = new();

        // 1 parametre (b=1, c=1)
        int result1 = calc.Multiply(5);

        // 2 parametre (c=1)
        int result2 = calc.Multiply(5, 2);

        // 3 parametre
        int result3 = calc.Multiply(5, 2, 3);

        // Power with default exponent
        double power1 = calc.Power(2);      // 2^2 = 4
        double power2 = calc.Power(2, 3);   // 2^3 = 8

        Console.WriteLine($"\nSonuçlar: mult1={result1}, mult2={result2}, mult3={result3}");
        Console.WriteLine($"Powers: 2^2={power1}, 2^3={power2}");
        Console.WriteLine("\n✅ AVANTAJLAR:");
        Console.WriteLine("   • Default values - commonly used values");
        Console.WriteLine("   • Flexibility - override when needed");
        Console.WriteLine("   • Backward compatibility - existing code still works");
        Console.WriteLine("   • Less overloads - single method with defaults");
    }

    /// <summary>
    /// ✅ GOOD PRACTICE: Named arguments (clarity)
    /// </summary>
    static void DemonstrateNamedArguments()
    {
        Console.WriteLine("🎯 Named arguments ile okunabilir kod:\n");

        GoodCalculator calc = new();

        // Named arguments - order doesn't matter!
        double result1 = calc.CalculateCompoundInterest(
            principal: 1000,
            annualRate: 0.05,
            years: 10
        );

        // Different order - same result
        double result2 = calc.CalculateCompoundInterest(
            years: 10,
            principal: 1000,
            annualRate: 0.05
        );

        // Override optional parameter
        double result3 = calc.CalculateCompoundInterest(
            principal: 1000,
            annualRate: 0.05,
            years: 10,
            compoundingsPerYear: 4  // Quarterly instead of monthly
        );

        Console.WriteLine($"\nSonuçlar: monthly={result1:C}, monthly2={result2:C}, quarterly={result3:C}");
        Console.WriteLine("\n✅ AVANTAJLAR:");
        Console.WriteLine("   • Self-documenting code - açık ve net");
        Console.WriteLine("   • Order independence - sıra önemli değil");
        Console.WriteLine("   • Skip optional params - sadece ihtiyacını belirt");
        Console.WriteLine("   • Reduce errors - parametre karışıklığı olmaz");
    }

    /// <summary>
    /// ✅ GOOD PRACTICE: ref/out/in parameter modifiers
    /// Note: C# doesn't allow overloading ONLY by ref/out/in modifiers
    /// </summary>
    static void DemonstrateRefOutInOverloads()
    {
        Console.WriteLine("🎯 ref/out/in parameter modifiers:\n");

        GoodCalculator calc = new();

        // By value - no change to original
        int value1 = 10;
        calc.Process(value1);
        Console.WriteLine($"  After Process(int): value1={value1} (unchanged)\n");

        // By ref - changes reflected (different method name)
        int value2 = 10;
        calc.ProcessByRef(ref value2);
        Console.WriteLine($"  After ProcessByRef(ref int): value2={value2} (changed!)\n");

        // Out - must initialize (different method name)
        calc.ProcessWithOut(out int value3);
        Console.WriteLine($"  After ProcessWithOut(out int): value3={value3} (initialized)\n");

        // In - readonly reference (C# 7.2+, different method name)
        int value4 = 10;
        calc.ProcessReadonly(in value4);
        Console.WriteLine($"  After ProcessReadonly(in int): value4={value4} (readonly ref)\n");

        Console.WriteLine("✅ FARKLAR:");
        Console.WriteLine("   • Process(int): By value - copy, no change to original");
        Console.WriteLine("   • ProcessByRef(ref int): By reference - changes reflected, must initialize before");
        Console.WriteLine("   • ProcessWithOut(out int): Out - must initialize in method, no need to initialize before");
        Console.WriteLine("   • ProcessReadonly(in int): Readonly reference - performance benefit for large structs");
        Console.WriteLine("\n⚠️  NOT: C# ref/out/in'i SADECE modifier farkıyla overload etmeye izin vermez!");
    }

    /// <summary>
    /// ✅ GOOD PRACTICE: Tuple return overloads (C# 7+)
    /// </summary>
    static void DemonstrateTupleReturnOverloads()
    {
        Console.WriteLine("🎯 Tuple return types ile multiple values:\n");

        GoodCalculator calc = new();

        // int overload - returns (sum, product)
        var (sum1, product1) = calc.Calculate(5, 10);
        Console.WriteLine($"  Results: sum={sum1}, product={product1}\n");

        // double overload - returns (sum, product, quotient)
        var (sum2, product2, quotient2) = calc.Calculate(10.0, 2.5);
        Console.WriteLine($"  Results: sum={sum2}, product={product2}, quotient={quotient2}\n");

        Console.WriteLine("✅ AVANTAJLAR:");
        Console.WriteLine("   • Multiple return values - no out parameters");
        Console.WriteLine("   • Named tuple elements - self-documenting");
        Console.WriteLine("   • Deconstruction support - clean syntax");
        Console.WriteLine("   • Type-safe - compile-time checking");
    }

    /// <summary>
    /// ✨ BEST PRACTICE: Generic methods alternatifi
    /// </summary>
    static void DemonstrateGenericMethods()
    {
        Console.WriteLine("🌟 Generic methods ile reusable code:\n");

        GenericCalculator genericCalc = new();

        // int with generics
        int intResult = genericCalc.Add<int>(5, 10);

        // double with generics
        double doubleResult = genericCalc.Add<double>(3.5, 2.5);

        // decimal with generics
        decimal decimalResult = genericCalc.Add<decimal>(10.5m, 5.25m);

        Console.WriteLine($"\nGeneric Results: int={intResult}, double={doubleResult}, decimal={decimalResult}");

        // Custom operation with delegate
        Console.WriteLine("\nCustom Operations:");
        int multiplyResult = genericCalc.Calculate<int>(5, 10, (a, b) => a * b);
        double divideResult = genericCalc.Calculate<double>(10.0, 2.5, (a, b) => a / b);

        // Type conversion
        Console.WriteLine("\nType Conversion:");
        double convertedResult = genericCalc.Convert<int, double>(42, x => x * 1.5);

        Console.WriteLine("\n✨ AVANTAJLAR:");
        Console.WriteLine("   • Single method - multiple types");
        Console.WriteLine("   • Type-safe - compile-time checking");
        Console.WriteLine("   • Reusable - DRY principle");
        Console.WriteLine("   • Constraints - restrict to specific types");
        Console.WriteLine("   • No code duplication - maintenance benefit");
    }

    /// <summary>
    /// 🔍 Overload Resolution Rules
    /// </summary>
    static void DemonstrateOverloadResolution()
    {
        Console.WriteLine("🔍 Compiler nasıl overload seçer?\n");

        GoodCalculator calc = new();

        // 1. Exact match (en yüksek öncelik)
        Console.WriteLine("1. Exact Match:");
        int exactMatch = calc.Add(5, 10);  // Add(int, int) - exact match

        // 2. Implicit conversion
        Console.WriteLine("\n2. Implicit Conversion:");
        double implicitConv = calc.Add(5.5, 10.5);  // Add(double, double)

        // 3. params keyword (en düşük öncelik)
        Console.WriteLine("\n3. params Keyword (lowest priority):");
        int paramsMatch = calc.AddMany(1, 2, 3, 4, 5);  // AddMany(params int[])

        // 4. Ambiguous call prevention
        Console.WriteLine("\n4. Ambiguity Resolution:");
        Console.WriteLine("  Compiler en spesifik overload'u seçer");
        Console.WriteLine("  Add(int, int) > AddMany(params int[]) for Add(1, 2)");

        Console.WriteLine("\n📋 OVERLOAD RESOLUTION SIRALAMA:");
        Console.WriteLine("   1. Exact match (highest priority)");
        Console.WriteLine("   2. Implicit conversion");
        Console.WriteLine("   3. Optional parameters");
        Console.WriteLine("   4. params keyword (lowest priority)");
        Console.WriteLine("\n💡 İPUCU:");
        Console.WriteLine("   Compiler ambiguous calls'ı reddeder");
        Console.WriteLine("   En spesifik overload her zaman kazanır");
    }
}
