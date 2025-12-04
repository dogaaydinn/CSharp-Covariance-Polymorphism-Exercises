class Program {
    static void Main() {
        Console.WriteLine("=== Delegates ===\n");
        Func<int, int, int> add = (a, b) => a + b;
        Console.WriteLine($"add(5, 3) = {add(5, 3)}");

        Action<string> greet = name => Console.WriteLine($"Hello, {name}!");
        greet("Ali");

        Predicate<int> isEven = x => x % 2 == 0;
        Console.WriteLine($"isEven(4) = {isEven(4)}");

        var nums = new List<int> { 1, 2, 3, 4, 5 };
        Console.WriteLine($"Evens: {string.Join(", ", nums.Where(x => x % 2 == 0))}");
    }
}
