namespace MethodOverloading;

/// <summary>
/// ❌ BAD PRACTICE: object parametre ile type checking
/// Problem: Runtime errors, no type safety, poor performance
/// </summary>
public class BadCalculator
{
    // ❌ BAD: object parametre - runtime type checking gerekir
    public object Add(object a, object b)
    {
        // Runtime type checking - YAVAŞ ve HATA EĞİLİMLİ!
        if (a is int intA && b is int intB)
        {
            return intA + intB;
        }
        else if (a is double doubleA && b is double doubleB)
        {
            return doubleA + doubleB;
        }
        else if (a is string strA && b is string strB)
        {
            return strA + strB;
        }
        else
        {
            throw new ArgumentException("Desteklenmeyen tipler!");
        }
    }

    // ❌ BAD: Her operasyon için object - type safety yok!
    public object Calculate(object a, object b, string operation)
    {
        // Magic strings + runtime type checking = NIGHTMARE!
        if (operation == "add")
        {
            return Add(a, b);
        }
        else if (operation == "multiply")
        {
            if (a is int intA && b is int intB)
            {
                return intA * intB;
            }
            return Convert.ToDouble(a) * Convert.ToDouble(b);
        }

        throw new ArgumentException("Geçersiz operasyon!");
    }
}

/// <summary>
/// ✅ GOOD PRACTICE: Method overloading ile type-safe operations
/// Compile-time type checking, better performance, intellisense support
/// </summary>
public class GoodCalculator
{
    // ═══════════════════════════════════════════════════════
    // 1. PARAMETRE SAYISINA GÖRE OVERLOAD (Number of Parameters)
    // ═══════════════════════════════════════════════════════

    public int Add(int a, int b)
    {
        Console.WriteLine($"  ➜ Add(int, int): {a} + {b} = {a + b}");
        return a + b;
    }

    public int Add(int a, int b, int c)
    {
        Console.WriteLine($"  ➜ Add(int, int, int): {a} + {b} + {c} = {a + b + c}");
        return a + b + c;
    }

    public int Add(int a, int b, int c, int d)
    {
        Console.WriteLine($"  ➜ Add(int, int, int, int): {a} + {b} + {c} + {d} = {a + b + c + d}");
        return a + b + c + d;
    }

    // ═══════════════════════════════════════════════════════
    // 2. PARAMETRE TİPİNE GÖRE OVERLOAD (Parameter Types)
    // ═══════════════════════════════════════════════════════

    public double Add(double a, double b)
    {
        Console.WriteLine($"  ➜ Add(double, double): {a} + {b} = {a + b}");
        return a + b;
    }

    public decimal Add(decimal a, decimal b)
    {
        Console.WriteLine($"  ➜ Add(decimal, decimal): {a} + {b} = {a + b}");
        return a + b;
    }

    public string Add(string a, string b)
    {
        Console.WriteLine($"  ➜ Add(string, string): \"{a}\" + \"{b}\" = \"{a + b}\"");
        return a + b;
    }

    // ═══════════════════════════════════════════════════════
    // 3. ARRAY vs PARAMS OVERLOAD
    // ═══════════════════════════════════════════════════════

    // Array overload - explicit array required
    public int Add(int[] numbers)
    {
        int sum = numbers.Sum();
        Console.WriteLine($"  ➜ Add(int[]): Array of {numbers.Length} numbers = {sum}");
        return sum;
    }

    // params overload - variable arguments (more flexible)
    public int AddMany(params int[] numbers)
    {
        int sum = numbers.Sum();
        Console.WriteLine($"  ➜ AddMany(params int[]): {numbers.Length} numbers = {sum}");
        return sum;
    }

    // ═══════════════════════════════════════════════════════
    // 4. OPTIONAL PARAMETERS (Default Values)
    // ═══════════════════════════════════════════════════════

    public int Multiply(int a, int b = 1, int c = 1)
    {
        int result = a * b * c;
        Console.WriteLine($"  ➜ Multiply({a}, {b}, {c}) = {result}");
        return result;
    }

    public double Power(double baseValue, int exponent = 2)
    {
        double result = Math.Pow(baseValue, exponent);
        Console.WriteLine($"  ➜ Power({baseValue}, {exponent}) = {result}");
        return result;
    }

    // ═══════════════════════════════════════════════════════
    // 5. NAMED ARGUMENTS İÇİN (Clarity and Flexibility)
    // ═══════════════════════════════════════════════════════

    public double CalculateCompoundInterest(
        double principal,
        double annualRate,
        int years,
        int compoundingsPerYear = 12)
    {
        // A = P(1 + r/n)^(nt)
        double result = principal * Math.Pow(1 + annualRate / compoundingsPerYear, compoundingsPerYear * years);
        Console.WriteLine($"  ➜ Compound Interest: Principal={principal:C}, Rate={annualRate:P}, Years={years}, Compounds={compoundingsPerYear} => {result:C}");
        return result;
    }

    // ═══════════════════════════════════════════════════════
    // 6. REF vs OUT OVERLOADS (Different method names for 'in')
    // ═══════════════════════════════════════════════════════

    public void Process(int value)
    {
        Console.WriteLine($"  ➜ Process(int): value={value} (by value - no change)");
    }

    public void ProcessByRef(ref int value)
    {
        Console.WriteLine($"  ➜ ProcessByRef(ref int): value={value} (before)");
        value *= 2;
        Console.WriteLine($"  ➜ ProcessByRef(ref int): value={value} (after doubling)");
    }

    public void ProcessWithOut(out int value)
    {
        Console.WriteLine($"  ➜ ProcessWithOut(out int): initializing value");
        value = 100;
        Console.WriteLine($"  ➜ ProcessWithOut(out int): value={value} (initialized)");
    }

    // 'in' modifier - separate method name (C# 7.2+)
    public void ProcessReadonly(in int value)
    {
        Console.WriteLine($"  ➜ ProcessReadonly(in int): value={value} (readonly reference)");
        // value = 10; // ❌ COMPILE ERROR - readonly!
    }

    // ═══════════════════════════════════════════════════════
    // 7. TUPLE RETURN OVERLOADS (C# 7+)
    // ═══════════════════════════════════════════════════════

    public (int sum, int product) Calculate(int a, int b)
    {
        Console.WriteLine($"  ➜ Calculate(int, int): sum={a + b}, product={a * b}");
        return (a + b, a * b);
    }

    public (double sum, double product, double quotient) Calculate(double a, double b)
    {
        Console.WriteLine($"  ➜ Calculate(double, double): sum={a + b}, product={a * b}, quotient={a / b}");
        return (a + b, a * b, a / b);
    }
}

/// <summary>
/// ✅ BEST PRACTICE: Generic method alternatifi
/// Type-safe, reusable, compile-time checking
/// </summary>
public class GenericCalculator
{
    // Generic method with constraint
    public T Add<T>(T a, T b) where T : struct, IComparable, IFormattable, IConvertible
    {
        // Dynamic kullanımı (runtime overhead var ama esnek)
        dynamic dynA = a;
        dynamic dynB = b;
        T result = dynA + dynB;

        Console.WriteLine($"  ➜ Add<{typeof(T).Name}>({a}, {b}) = {result}");
        return result;
    }

    // Generic method with custom delegate
    public T Calculate<T>(T a, T b, Func<T, T, T> operation)
    {
        T result = operation(a, b);
        Console.WriteLine($"  ➜ Calculate<{typeof(T).Name}>({a}, {b}) with custom operation = {result}");
        return result;
    }

    // Generic method with multiple type parameters
    public TResult Convert<TInput, TResult>(TInput value, Func<TInput, TResult> converter)
    {
        TResult result = converter(value);
        Console.WriteLine($"  ➜ Convert<{typeof(TInput).Name}, {typeof(TResult).Name}>({value}) = {result}");
        return result;
    }
}

/// <summary>
/// Extension methods demonstrating overload-like behavior
/// </summary>
public static class CalculatorExtensions
{
    public static int Square(this int value)
    {
        return value * value;
    }

    public static double Square(this double value)
    {
        return value * value;
    }

    public static int Cube(this int value)
    {
        return value * value * value;
    }
}
