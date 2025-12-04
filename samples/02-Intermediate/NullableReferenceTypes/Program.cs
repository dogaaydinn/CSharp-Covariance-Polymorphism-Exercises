#nullable enable
class User {
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public void Display() => Console.WriteLine($"{Name}: {Email ?? "No email"}");
}
class Program {
    static void Main() {
        Console.WriteLine("=== Nullable Reference Types ===\n");
        new User { Name = "Ali", Email = "ali@test.com" }.Display();
        new User { Name = "Ayşe", Email = null }.Display();
        Console.WriteLine("\n💡 string? enables nullable annotations");
    }
}
