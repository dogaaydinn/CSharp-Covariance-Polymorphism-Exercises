namespace CastingExamples;

/// <summary>
/// Temel çalışan sınıfı - Tüm çalışanlar için ortak özellikler.
/// </summary>
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public decimal Salary { get; set; }

    public Employee(int id, string name, string department, decimal salary)
    {
        Id = id;
        Name = name;
        Department = department;
        Salary = salary;
    }

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"[{GetType().Name}] ID: {Id}, Ad: {Name}, Departman: {Department}, Maaş: {Salary:C}");
    }

    public virtual void Work()
    {
        Console.WriteLine($"{Name} çalışıyor...");
    }
}

/// <summary>
/// Yönetici sınıfı - Employee'dan türetilmiş ek yetkilerle.
/// </summary>
public class Manager : Employee
{
    public List<Employee> Team { get; set; } = new();
    public decimal Bonus { get; set; }

    public Manager(int id, string name, string department, decimal salary, decimal bonus)
        : base(id, name, department, salary)
    {
        Bonus = bonus;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"   └─ Bonus: {Bonus:C}, Ekip Sayısı: {Team.Count}");
    }

    public override void Work()
    {
        Console.WriteLine($"{Name} (Yönetici) ekibini yönetiyor ve stratejik kararlar alıyor.");
    }

    // Manager'a özgü metodlar
    public void HoldMeeting()
    {
        Console.WriteLine($"{Name} bir toplantı düzenliyor 📊");
    }

    public void ReviewPerformance(Employee employee)
    {
        Console.WriteLine($"{Name}, {employee.Name}'in performansını değerlendiriyor");
    }

    public void AddTeamMember(Employee employee)
    {
        Team.Add(employee);
        Console.WriteLine($"✅ {employee.Name}, {Name}'in ekibine eklendi.");
    }
}

/// <summary>
/// Geliştirici sınıfı - Teknik çalışan.
/// </summary>
public class Developer : Employee
{
    public string ProgrammingLanguage { get; set; }
    public int YearsOfExperience { get; set; }

    public Developer(int id, string name, string department, decimal salary,
                     string programmingLanguage, int experience)
        : base(id, name, department, salary)
    {
        ProgrammingLanguage = programmingLanguage;
        YearsOfExperience = experience;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"   └─ Dil: {ProgrammingLanguage}, Deneyim: {YearsOfExperience} yıl");
    }

    public override void Work()
    {
        Console.WriteLine($"{Name} (Developer) {ProgrammingLanguage} ile kod yazıyor 💻");
    }

    public void WriteCode()
    {
        Console.WriteLine($"{Name} yeni özellik geliştiriyor...");
    }

    public void FixBug()
    {
        Console.WriteLine($"{Name} bir bug düzeltiyor 🐛");
    }
}

/// <summary>
/// İK uzmanı sınıfı.
/// </summary>
public class HRSpecialist : Employee
{
    public int EmployeesManaged { get; set; }

    public HRSpecialist(int id, string name, string department, decimal salary, int employeesManaged)
        : base(id, name, department, salary)
    {
        EmployeesManaged = employeesManaged;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"   └─ Yönetilen Çalışan Sayısı: {EmployeesManaged}");
    }

    public override void Work()
    {
        Console.WriteLine($"{Name} (İK) çalışan işleriyle ilgileniyor 👔");
    }

    public void ConductInterview()
    {
        Console.WriteLine($"{Name} bir mülakat yapıyor");
    }

    public void ProcessPayroll()
    {
        Console.WriteLine($"{Name} maaş bordrosunu hazırlıyor");
    }
}
