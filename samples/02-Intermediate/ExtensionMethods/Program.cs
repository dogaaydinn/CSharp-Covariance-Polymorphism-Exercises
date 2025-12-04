static class StringExt {
    public static bool IsEmail(this string s) => s.Contains('@');
    public static string Truncate(this string s, int len) =>
        s.Length <= len ? s : s[..len] + "...";
}

class Program {
    static void Main() {
        Console.WriteLine("=== Extension Methods ===\n");
        string email = "user@test.com";
        Console.WriteLine($"{email} is email: {email.IsEmail()}");
        Console.WriteLine($"Truncated: {"Long text here".Truncate(10)}");
    }
}
