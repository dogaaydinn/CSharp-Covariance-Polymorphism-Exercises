namespace MethodOverloading;

public class Calculator
{
    // 1. Parametre sayısına göre overload
    public int Add(int a, int b)
    {
        Console.WriteLine($"Add(int, int): {a} + {b}");
        return a + b;
    }

    public int Add(int a, int b, int c)
    {
        Console.WriteLine($"Add(int, int, int): {a} + {b} + {c}");
        return a + b + c;
    }

    // 2. Parametre türüne göre overload
    public double Add(double a, double b)
    {
        Console.WriteLine($"Add(double, double): {a} + {b}");
        return a + b;
    }

    public string Add(string a, string b)
    {
        Console.WriteLine($"Add(string, string): '{a}' + '{b}'");
        return a + b;
    }

    // 3. Params keyword - Variable arguments
    public int Add(params int[] numbers)
    {
        Console.WriteLine($"Add(params int[]): {numbers.Length} sayı");
        return numbers.Sum();
    }

    // 4. Optional parameters
    public int Multiply(int a, int b = 1, int c = 1)
    {
        Console.WriteLine($"Multiply({a}, {b}, {c})");
        return a * b * c;
    }

    // 5. Named arguments için
    public double Calculate(double value, double rate, int years)
    {
        Console.WriteLine($"Calculate(value: {value}, rate: {rate}, years: {years})");
        return value * Math.Pow(1 + rate, years);
    }

    // 6. Ref vs out overload (farklı imzalar)
    public void Process(int value)
    {
        Console.WriteLine($"Process(int): {value}");
    }

    public void Process(ref int value)
    {
        Console.WriteLine($"Process(ref int): {value}");
        value *= 2;
    }

    public void Process(out int value)
    {
        Console.WriteLine("Process(out int)");
        value = 100;
    }
}

public class StringHelper
{
    // Extension method style overloads
    public string Format(string text)
    {
        return text.Trim();
    }

    public string Format(string text, bool uppercase)
    {
        text = text.Trim();
        return uppercase ? text.ToUpper() : text.ToLower();
    }

    public string Format(string text, int maxLength, string suffix = "...")
    {
        text = text.Trim();
        if (text.Length > maxLength)
            return text.Substring(0, maxLength) + suffix;
        return text;
    }
}
