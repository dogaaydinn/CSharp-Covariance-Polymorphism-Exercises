// SCENARIO: Şirket çalışan hiyerarşisi - Constructor Chaining
// BAD PRACTICE: Her constructor'da kod tekrarı
// GOOD PRACTICE: this() ve base() ile constructor chaining
// ADVANCED: Readonly fields, multi-level inheritance chains

using ConstructorChaining;

class Program
{
    static void Main()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   🏢  CONSTRUCTOR CHAINING - ŞİRKET HİYERARŞİSİ        ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // 1. BAD PRACTICE
        Console.WriteLine("═══ 1. ❌ BAD PRACTICE: Kod Tekrarı ═══\n");
        DemonstrateBadPractice();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 2. this() chaining - same class
        Console.WriteLine("═══ 2. ✅ this() CHAINING - Aynı Class İçinde ═══\n");
        DemonstrateThisChaining();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 3. base() chaining - inheritance
        Console.WriteLine("═══ 3. ✅ base() CHAINING - Inheritance ═══\n");
        DemonstrateBaseChaining();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 4. Multi-level chaining
        Console.WriteLine("═══ 4. ✅ MULTI-LEVEL CHAINING - 3 Seviye ═══\n");
        DemonstrateMultiLevelChaining();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 5. Readonly fields
        Console.WriteLine("═══ 5. ✅ READONLY FIELDS - Constructor'da Set ═══\n");
        DemonstrateReadonlyFields();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 6. Factory methods
        Console.WriteLine("═══ 6. ✅ FACTORY METHODS - Constructor Chaining ═══\n");
        DemonstrateFactoryMethods();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 7. Execution order
        Console.WriteLine("═══ 7. 🔍 EXECUTION ORDER - Zincir Sırası ═══\n");
        DemonstrateExecutionOrder();

        // Final Summary
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    📊 ÖZET                                ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("✅ ÖĞRENİLENLER:");
        Console.WriteLine("   • this(): Aynı class'taki başka constructor'ı çağırır");
        Console.WriteLine("   • base(): Base class constructor'ı çağırır");
        Console.WriteLine("   • Execution order: Zincirin başından sona doğru (base → derived)");
        Console.WriteLine("   • Readonly fields: Sadece constructor'da initialize edilebilir");
        Console.WriteLine("   • Kod tekrarını önler → DRY (Don't Repeat Yourself)");
        Console.WriteLine("   • Maintainability artırır → Tek yerden yönetim");
        Console.WriteLine();
        Console.WriteLine("💡 BEST PRACTICES:");
        Console.WriteLine("   • Ortak initialization logic'i en basit constructor'da tut");
        Console.WriteLine("   • Overload'lar this() ile zincirle");
        Console.WriteLine("   • Derived class'lar base() ile zincirle");
        Console.WriteLine("   • Readonly fields için constructor chaining kullan");
    }

    /// <summary>
    /// ❌ BAD PRACTICE: Her constructor'da aynı kod tekrarı
    /// </summary>
    static void DemonstrateBadPractice()
    {
        Console.WriteLine("💀 Bad Practice: Kod tekrarı örneği\n");

        Console.WriteLine("BadEmployee() oluşturuluyor:");
        BadEmployee bad1 = new();

        Console.WriteLine("\nBadEmployee(name, age) oluşturuluyor:");
        BadEmployee bad2 = new("Ali", 30);

        Console.WriteLine("\nBadEmployee(full) oluşturuluyor:");
        BadEmployee bad3 = new("Ayşe", 25, "IT", 60000m);

        Console.WriteLine("\n⚠️  SORUNLAR:");
        Console.WriteLine("   • Her constructor'da aynı validation logic");
        Console.WriteLine("   • Kod tekrarı → Maintenance nightmare");
        Console.WriteLine("   • Logic değişirse 3 yerde güncelleme gerekir");
        Console.WriteLine("   • DRY prensibini ihlal ediyor");
    }

    /// <summary>
    /// ✅ GOOD PRACTICE: this() ile aynı class içinde zincirleme
    /// </summary>
    static void DemonstrateThisChaining()
    {
        Console.WriteLine("🎯 this() ile constructor zincirleme:\n");

        Console.WriteLine("Person(name, age, address) oluşturuluyor:");
        Console.WriteLine("Constructor chain: () → (name) → (name,age) → (name,age,address)\n");

        Person person = new("Mehmet", 35, "Istanbul");

        Console.WriteLine("\n✅ Sonuç:");
        person.DisplayInfo();

        Console.WriteLine("\n💡 this() CHAINING FAYDAları:");
        Console.WriteLine("   • Ortak initialization tek yerde (default constructor)");
        Console.WriteLine("   • Readonly fields (Id, CreatedAt) sadece bir kere set edilir");
        Console.WriteLine("   • Kod tekrarı YOK → DRY principle");
        Console.WriteLine("   • Logic değişirse tek yerden güncelleme");
    }

    /// <summary>
    /// ✅ GOOD PRACTICE: base() ile base class constructor'a zincirleme
    /// </summary>
    static void DemonstrateBaseChaining()
    {
        Console.WriteLine("🎯 base() ile base class'a zincirleme:\n");

        Console.WriteLine("Employee(full) oluşturuluyor:");
        Console.WriteLine("Constructor chain: Person(name,age,address) → Employee(full)\n");

        Employee emp = new("Fatma", 28, "Ankara", "Engineering", 75000m);

        Console.WriteLine("\n✅ Sonuç:");
        emp.DisplayInfo();

        Console.WriteLine("\n💡 base() CHAINING FAYDAları:");
        Console.WriteLine("   • Base class initialization otomatik");
        Console.WriteLine("   • Person'daki readonly fields (Id) otomatik set edilir");
        Console.WriteLine("   • Base class logic'i tekrar yazmaya gerek yok");
        Console.WriteLine("   • Inheritance hierarchy doğru şekilde kurulur");
    }

    /// <summary>
    /// ✅ GOOD PRACTICE: Multi-level constructor chaining (3 seviye)
    /// </summary>
    static void DemonstrateMultiLevelChaining()
    {
        Console.WriteLine("🎯 3-Level constructor chain:\n");

        Console.WriteLine("Manager(full) oluşturuluyor:");
        Console.WriteLine("Constructor chain:");
        Console.WriteLine("  Person(name,age,address) → Employee(full) → Manager(full)\n");

        Manager mgr = new(
            name: "Zeynep",
            age: 40,
            address: "Izmir",
            department: "Management",
            salary: 120000m,
            bonus: 30000m,
            managementLevel: 2,  // Manager
            officeLocation: "2nd Floor, Room 201"
        );

        Console.WriteLine("\n✅ Sonuç:");
        mgr.DisplayInfo();

        Console.WriteLine("\n💡 MULTI-LEVEL CHAINING:");
        Console.WriteLine("   • Person → Employee → Manager zincirleme");
        Console.WriteLine("   • Her seviye kendi initialization'ını yapar");
        Console.WriteLine("   • Readonly fields her seviyede set edilir");
        Console.WriteLine("   • Execution order: Person → Employee → Manager");
    }

    /// <summary>
    /// ✅ Readonly fields sadece constructor'da set edilebilir
    /// </summary>
    static void DemonstrateReadonlyFields()
    {
        Console.WriteLine("🎯 Readonly fields demonstration:\n");

        Console.WriteLine("3 farklı Person oluşturuluyor (farklı readonly ID'ler):\n");

        Person p1 = new("Ali", 25, "Istanbul");
        Thread.Sleep(10);  // Farklı CreatedAt için bekleme

        Person p2 = new("Veli", 30, "Ankara");
        Thread.Sleep(10);

        Person p3 = new("Ayşe", 28, "Izmir");

        Console.WriteLine("\n✅ Readonly Fields:");
        Console.WriteLine($"Person 1 - ID: {p1.Id}, Created: {p1.CreatedAt:HH:mm:ss.fff}");
        Console.WriteLine($"Person 2 - ID: {p2.Id}, Created: {p2.CreatedAt:HH:mm:ss.fff}");
        Console.WriteLine($"Person 3 - ID: {p3.Id}, Created: {p3.CreatedAt:HH:mm:ss.fff}");

        // ❌ Readonly fields değiştirilemez!
        // p1.Id = Guid.NewGuid();  // COMPILE ERROR!
        // p1.CreatedAt = DateTime.Now;  // COMPILE ERROR!

        Console.WriteLine("\n💡 READONLY FIELDS:");
        Console.WriteLine("   • Sadece constructor'da set edilebilir");
        Console.WriteLine("   • Constructor chaining ile tek yerden yönetim");
        Console.WriteLine("   • Immutability → Thread-safe");
        Console.WriteLine("   • Assignment sonrası değiştirilemez");
    }

    /// <summary>
    /// ✅ Factory methods constructor chaining kullanır
    /// </summary>
    static void DemonstrateFactoryMethods()
    {
        Console.WriteLine("🎯 Factory methods ile constructor chaining:\n");

        Console.WriteLine("Factory methods çalıştırılıyor...\n");

        Employee intern = EmployeeFactory.CreateIntern("Can", 22);
        Console.WriteLine("✅ Intern created\n");

        Employee junior = EmployeeFactory.CreateJunior("Deniz", 25, "Istanbul");
        Console.WriteLine("✅ Junior created\n");

        Employee senior = EmployeeFactory.CreateSenior("Ece", 32, "Ankara");
        Console.WriteLine("✅ Senior created\n");

        Manager director = EmployeeFactory.CreateDirector("Furkan", 45, "Izmir");
        Console.WriteLine("✅ Director created\n");

        Console.WriteLine("📊 Created Employees:");
        Console.WriteLine($"  Intern:   {intern.Name} - {intern.Department} - {intern.Salary:C}");
        Console.WriteLine($"  Junior:   {junior.Name} - {junior.Department} - {junior.Salary:C}");
        Console.WriteLine($"  Senior:   {senior.Name} - {senior.Department} - {senior.Salary:C}");
        Console.WriteLine($"  Director: {director.Name} - {director.Department} - {director.Salary:C} + {director.Bonus:C} bonus");

        Console.WriteLine("\n💡 FACTORY METHODS:");
        Console.WriteLine("   • Constructor chaining'i encapsulate eder");
        Console.WriteLine("   • Predefined configurations ile kolay nesne yaratma");
        Console.WriteLine("   • Named methods → Daha okunabilir kod");
        Console.WriteLine("   • Constructor complexity'i gizler");
    }

    /// <summary>
    /// 🔍 Constructor execution order demonstration
    /// </summary>
    static void DemonstrateExecutionOrder()
    {
        Console.WriteLine("🔍 Constructor execution order:\n");

        Console.WriteLine("═══ Scenario 1: Person with full chain ═══");
        Console.WriteLine("new Person(\"Test\", 30, \"Address\");\n");

        Console.WriteLine("Expected execution order:");
        Console.WriteLine("  [1] Person()");
        Console.WriteLine("  [2] Person(name)");
        Console.WriteLine("  [3] Person(name, age)");
        Console.WriteLine("  [4] Person(name, age, address)");

        Console.WriteLine("\nActual execution:\n");
        Person testPerson = new("Test", 30, "Address");

        Console.WriteLine("\n" + new string('─', 60));

        Console.WriteLine("\n═══ Scenario 2: Manager with full chain ═══");
        Console.WriteLine("new Manager(full params);\n");

        Console.WriteLine("Expected execution order:");
        Console.WriteLine("  [1] Person()");
        Console.WriteLine("  [2] Person(name)");
        Console.WriteLine("  [3] Person(name, age)");
        Console.WriteLine("  [4] Person(name, age, address)");
        Console.WriteLine("  [7] Employee(full)");
        Console.WriteLine("  [10] Manager(full)");

        Console.WriteLine("\nActual execution:\n");
        Manager testManager = new("Manager", 40, "City", "Dept", 100000m, 25000m, 2, "Office");

        Console.WriteLine("\n💡 EXECUTION ORDER RULES:");
        Console.WriteLine("   1. Base class constructor ÖNCE çalışır");
        Console.WriteLine("   2. this() chain soldan sağa işlenir");
        Console.WriteLine("   3. base() chain her seviyede tekrar eder");
        Console.WriteLine("   4. Constructor body SON çalışır");
        Console.WriteLine("   5. Zincirin başından sonuna: Base → Derived");
    }
}
