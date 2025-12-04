// Boxing/Unboxing performance
using System.Diagnostics;

class Program {
    static void Main() {
        Console.WriteLine("=== Boxing/Unboxing Performance ===\n");

        const int iterations = 10_000_000;

        var sw1 = Stopwatch.StartNew();
        int sum1 = 0;
        for (int i = 0; i < iterations; i++) sum1 += i;
        sw1.Stop();
        Console.WriteLine($"Value type: {sw1.ElapsedMilliseconds}ms");

        var sw2 = Stopwatch.StartNew();
        object sum2 = 0;
        for (int i = 0; i < iterations; i++) {
            sum2 = (int)sum2 + i;
        }
        sw2.Stop();
        Console.WriteLine($"Boxing: {sw2.ElapsedMilliseconds}ms (

{sw2.ElapsedMilliseconds / (double)sw1.ElapsedMilliseconds:F1}x slower)");
    }
}
