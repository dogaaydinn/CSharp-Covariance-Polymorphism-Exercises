namespace ConstructorChaining;

/// <summary>
/// ❌ BAD PRACTICE: Kod tekrarı - her constructor'da aynı logic
/// </summary>
public class BadEmployee
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Department { get; set; }
    public decimal Salary { get; set; }

    // ❌ BAD: Kod tekrarı!
    public BadEmployee()
    {
        Name = "Unknown";
        Age = 0;
        Department = "Unknown";
        Salary = 0;
        Console.WriteLine("[BAD] Default validation executed");
    }

    // ❌ BAD: Aynı validation logic tekrar!
    public BadEmployee(string name, int age)
    {
        Name = name;
        Age = age;
        Department = "Unknown";
        Salary = 0;
        Console.WriteLine("[BAD] Validation executed again"); // Kod tekrarı!
    }

    // ❌ BAD: Yine aynı logic!
    public BadEmployee(string name, int age, string department, decimal salary)
    {
        Name = name;
        Age = age;
        Department = department;
        Salary = salary;
        Console.WriteLine("[BAD] Validation executed third time!"); // Kod tekrarı!
    }
}

/// <summary>
/// ✅ GOOD PRACTICE: Person base class
/// Readonly fields, this() constructor chaining
/// </summary>
public class Person
{
    // Readonly fields - sadece constructor'da set edilebilir
    public readonly Guid Id;
    public readonly DateTime CreatedAt;

    // Regular properties
    public string Name { get; set; }
    public int Age { get; set; }
    public string Address { get; set; }

    // ═══════════════════════════════════════════════════════
    // this() CHAINING - Aynı class içinde constructor zincirleme
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// Default constructor - tüm diğer constructor'lar buraya zincirlenecek
    /// </summary>
    public Person()
    {
        // Readonly fields burada initialize edilmeli
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;

        // Default values
        Name = "Unknown";
        Age = 0;
        Address = "Unknown";

        Console.WriteLine("  [1] Person() → Default constructor executed");
        Console.WriteLine($"      Generated ID: {Id}");
    }

    /// <summary>
    /// Name overload - this() ile default constructor'a zincirlenmiş
    /// </summary>
    public Person(string name) : this()  // ← this() çağrısı önce çalışır
    {
        Name = name;
        Console.WriteLine($"  [2] Person(name) → Name set to: {name}");
    }

    /// <summary>
    /// Name + Age overload - this(name) ile zincirleme
    /// </summary>
    public Person(string name, int age) : this(name)  // ← this(name) önce çalışır
    {
        Age = age;
        Console.WriteLine($"  [3] Person(name, age) → Age set to: {age}");
    }

    /// <summary>
    /// Full overload - this(name, age) ile zincirleme
    /// </summary>
    public Person(string name, int age, string address) : this(name, age)
    {
        Address = address;
        Console.WriteLine($"  [4] Person(name, age, address) → Address set to: {address}");
    }

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"  Person: {Name}, {Age} years old, {Address}");
        Console.WriteLine($"  ID: {Id}, Created: {CreatedAt:yyyy-MM-dd HH:mm:ss}");
    }
}

/// <summary>
/// ✅ GOOD PRACTICE: Employee derived class
/// base() constructor chaining ile Person'a zincirlenmiş
/// </summary>
public class Employee : Person
{
    // Readonly field for Employee
    public readonly string EmployeeCode;

    // Employee-specific properties
    public string Department { get; set; }
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }

    // ═══════════════════════════════════════════════════════
    // base() CHAINING - Base class constructor'a zincirleme
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// Default constructor - base() ile Person() çağrılır
    /// </summary>
    public Employee() : base()  // ← base() önce çalışır (Person default constructor)
    {
        EmployeeCode = GenerateEmployeeCode();
        Department = "Unknown";
        Salary = 0;
        HireDate = DateTime.Now;

        Console.WriteLine("  [5] Employee() → Default employee constructor");
        Console.WriteLine($"      Employee Code: {EmployeeCode}");
    }

    /// <summary>
    /// Name + Age overload - base(name, age) ile Person constructor'a zincirlenmiş
    /// </summary>
    public Employee(string name, int age) : base(name, age)
    {
        EmployeeCode = GenerateEmployeeCode();
        Department = "General";
        Salary = 50000m;
        HireDate = DateTime.Now;

        Console.WriteLine("  [6] Employee(name, age) → Basic employee created");
        Console.WriteLine($"      Employee Code: {EmployeeCode}");
    }

    /// <summary>
    /// Full overload - base(name, age, address) ile Person constructor'a zincirlenmiş
    /// </summary>
    public Employee(string name, int age, string address, string department, decimal salary)
        : base(name, age, address)  // ← Person(name, age, address) çağrılır
    {
        EmployeeCode = GenerateEmployeeCode();
        Department = department;
        Salary = salary;
        HireDate = DateTime.Now;

        Console.WriteLine($"  [7] Employee(full) → Department: {department}, Salary: {salary:C}");
        Console.WriteLine($"      Employee Code: {EmployeeCode}");
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"  Employee Code: {EmployeeCode}");
        Console.WriteLine($"  Department: {Department}, Salary: {Salary:C}");
        Console.WriteLine($"  Hire Date: {HireDate:yyyy-MM-dd}");
    }

    private static string GenerateEmployeeCode()
    {
        return $"EMP{DateTime.Now.Ticks % 100000:D5}";
    }
}

/// <summary>
/// ✅ GOOD PRACTICE: Manager derived class
/// Multi-level inheritance ile Person -> Employee -> Manager zincirleme
/// </summary>
public class Manager : Employee
{
    // Readonly field for Manager
    public readonly int ManagementLevel;  // 1=Team Lead, 2=Manager, 3=Director

    // Manager-specific properties
    public List<Employee> Team { get; set; }
    public decimal Bonus { get; set; }
    public string OfficeLocation { get; set; }

    // ═══════════════════════════════════════════════════════
    // MULTI-LEVEL CHAINING - 3 seviye inheritance
    // Manager → Employee → Person
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// Default constructor - base() ile Employee() çağrılır
    /// Employee() → Person() zincirleme otomatik olur
    /// </summary>
    public Manager() : base()  // ← Employee() → Person() chain
    {
        ManagementLevel = 1;  // Team Lead
        Team = new List<Employee>();
        Bonus = 0;
        OfficeLocation = "Main Office";

        Console.WriteLine("  [8] Manager() → Default manager (Team Lead)");
        Console.WriteLine($"      Management Level: {ManagementLevel}");
    }

    /// <summary>
    /// Basic overload - base(name, age) ile Employee constructor'a zincirlenmiş
    /// </summary>
    public Manager(string name, int age, int managementLevel)
        : base(name, age)  // ← Employee(name, age) → Person(name, age) chain
    {
        ManagementLevel = managementLevel;
        Team = new List<Employee>();
        Bonus = CalculateDefaultBonus(managementLevel);
        OfficeLocation = "Main Office";

        Console.WriteLine($"  [9] Manager(basic) → Management Level: {managementLevel}");
    }

    /// <summary>
    /// Full overload - base() ile Employee full constructor'a zincirlenmiş
    /// En uzun zincir: Manager → Employee → Person
    /// </summary>
    public Manager(string name, int age, string address, string department,
                   decimal salary, decimal bonus, int managementLevel, string officeLocation)
        : base(name, age, address, department, salary)  // ← Employee full → Person full chain
    {
        ManagementLevel = managementLevel;
        Team = new List<Employee>();
        Bonus = bonus;
        OfficeLocation = officeLocation;

        Console.WriteLine($"  [10] Manager(full) → Bonus: {bonus:C}, Level: {managementLevel}, Office: {officeLocation}");
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"  Management Level: {ManagementLevel} ({GetLevelName()})");
        Console.WriteLine($"  Bonus: {Bonus:C}");
        Console.WriteLine($"  Office: {OfficeLocation}");
        Console.WriteLine($"  Team Size: {Team.Count} employees");
    }

    private string GetLevelName()
    {
        return ManagementLevel switch
        {
            1 => "Team Lead",
            2 => "Manager",
            3 => "Director",
            _ => "Unknown"
        };
    }

    private static decimal CalculateDefaultBonus(int level)
    {
        return level switch
        {
            1 => 10000m,  // Team Lead
            2 => 25000m,  // Manager
            3 => 50000m,  // Director
            _ => 0m
        };
    }

    public void AddTeamMember(Employee employee)
    {
        Team.Add(employee);
        Console.WriteLine($"  ✅ {employee.Name} added to {Name}'s team");
    }
}

/// <summary>
/// Demonstration: Static factory methods with constructor chaining
/// </summary>
public class EmployeeFactory
{
    public static Employee CreateIntern(string name, int age)
    {
        // Intern: low salary, internship department
        return new Employee(name, age, "Unknown", "Internship", 20000m);
    }

    public static Employee CreateJunior(string name, int age, string address)
    {
        return new Employee(name, age, address, "Development", 40000m);
    }

    public static Employee CreateSenior(string name, int age, string address)
    {
        return new Employee(name, age, address, "Development", 80000m);
    }

    public static Manager CreateDirector(string name, int age, string address)
    {
        return new Manager(name, age, address, "Management", 150000m, 75000m, 3, "Executive Floor");
    }
}
