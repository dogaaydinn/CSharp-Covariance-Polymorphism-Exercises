// SCENARIO: Constructor chaining - this() ve base() kullanımı
// BAD PRACTICE: Her constructor'da kod tekrarı
// GOOD PRACTICE: Constructor chaining ile kod tekrarını önle

using ConstructorChaining;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Constructor Chaining: this() ve base() ===\n");

        Console.WriteLine("=== 1. this() Chaining (Aynı Class) ===\n");
        DemonstrateThisChaining();

        Console.WriteLine("\n=== 2. base() Chaining (Inheritance) ===\n");
        DemonstrateBaseChaining();

        Console.WriteLine("\n=== 3. Çoklu Seviye Chaining ===\n");
        DemonstrateMultiLevelChaining();

        Console.WriteLine("\n=== Analysis ===");
        Console.WriteLine("• this(): Aynı class'taki başka constructor çağırır");
        Console.WriteLine("• base(): Base class constructor çağırır");
        Console.WriteLine("• Execution order: Base → Derived (yukarıdan aşağıya)");
        Console.WriteLine("• Kod tekrarını önler, maintainability artırır");
    }

    static void DemonstrateThisChaining()
    {
        Console.WriteLine("Person(name, age, address) oluşturuluyor:\n");
        Person person = new("Ali", 30, "Istanbul");

        Console.WriteLine($"\n✅ Sonuç: {person.Name}, {person.Age}, {person.Address}");
        Console.WriteLine("💡 Constructor chain: () → (name) → (name,age) → (name,age,address)");
    }

    static void DemonstrateBaseChaining()
    {
        Console.WriteLine("Employee(name, age, dept, salary) oluşturuluyor:\n");
        Employee emp = new("Ayşe", 28, "IT", 75000m);

        Console.WriteLine($"\n✅ Sonuç: {emp.Name}, {emp.Age}, {emp.Department}, {emp.Salary:C}");
        Console.WriteLine("💡 Constructor chain: Person(name,age) → Employee(full)");
    }

    static void DemonstrateMultiLevelChaining()
    {
        Console.WriteLine("Manager oluşturuluyor (3 seviye inheritance):\n");
        Manager mgr = new("Mehmet", 35, "Management", 100000m, 20000m);

        Console.WriteLine($"\n✅ Sonuç:");
        Console.WriteLine($"  Name: {mgr.Name}");
        Console.WriteLine($"  Age: {mgr.Age}");
        Console.WriteLine($"  Department: {mgr.Department}");
        Console.WriteLine($"  Salary: {mgr.Salary:C}");
        Console.WriteLine($"  Bonus: {mgr.Bonus:C}");

        Console.WriteLine("\n💡 3-Level chain:");
        Console.WriteLine("  Person(name,age) → Employee(full) → Manager(full)");
    }
}
