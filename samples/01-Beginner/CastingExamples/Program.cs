// SCENARIO: Şirket çalışan yönetim sistemi - Tip dönüşümleri
// BAD PRACTICE: Güvensiz casting, null kontrolü yapmadan cast
// GOOD PRACTICE: 'as' operatörü, 'is' operatörü, pattern matching kullanımı

using CastingExamples;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== C# Casting Örnekleri: as, is, Pattern Matching ===\n");

        // Çalışanlar oluştur
        Employee emp1 = new Manager(1, "Ayşe Yılmaz", "Yönetim", 15000m, 5000m);
        Employee emp2 = new Developer(2, "Mehmet Kaya", "IT", 12000m, "C#", 5);
        Employee emp3 = new HRSpecialist(3, "Zeynep Demir", "İK", 10000m, 50);
        Employee emp4 = new Employee(4, "Ali Çelik", "Operasyon", 8000m);

        Console.WriteLine("=== 1. Upcasting (Implicit - Güvenli) ===\n");
        DemonstrateUpcasting();

        Console.WriteLine("\n=== 2. Downcasting: as Operatörü (Güvenli) ===\n");
        DemonstrateAsOperator(emp1, emp2, emp3, emp4);

        Console.WriteLine("\n=== 3. Downcasting: Explicit Cast (Riskli) ===\n");
        DemonstrateExplicitCast(emp1, emp4);

        Console.WriteLine("\n=== 4. is Operatörü (Type Checking) ===\n");
        DemonstrateIsOperator(emp1, emp2, emp3, emp4);

        Console.WriteLine("\n=== 5. Pattern Matching (Modern C#) ===\n");
        DemonstratePatternMatching(emp1, emp2, emp3, emp4);

        Console.WriteLine("\n=== 6. Switch Expression Pattern Matching ===\n");
        DemonstrateSwitchPattern(emp1, emp2, emp3, emp4);

        // Analiz
        Console.WriteLine("\n=== Output Analysis ===");
        Console.WriteLine("1. as operator: Null döner, exception atmaz (güvenli)");
        Console.WriteLine("2. explicit cast: InvalidCastException atabilir (riskli)");
        Console.WriteLine("3. is operator: Boolean döner, casting yapmaz");
        Console.WriteLine("4. pattern matching: is ile cast'i birleştirir (en iyi)");
        Console.WriteLine("5. switch expression: Çoklu tip kontrolü için elegant");

        Console.WriteLine("\n💡 Best Practice:");
        Console.WriteLine("   ✅ Kullan: 'as' veya pattern matching (emp is Manager mgr)");
        Console.WriteLine("   ❌ Kullanma: Explicit cast (Manager)emp - hata riski yüksek");
    }

    static void DemonstrateUpcasting()
    {
        // ✅ Upcasting (derived → base) her zaman güvenlidir
        Manager manager = new Manager(100, "Can Öz", "Yönetim", 20000m, 8000m);
        Employee employee = manager;  // Implicit conversion

        Console.WriteLine("✅ Manager → Employee (upcasting):");
        Console.WriteLine($"   Türetilmiş sınıf: {manager.GetType().Name}");
        Console.WriteLine($"   Base referans: {employee.GetType().Name}");
        Console.WriteLine($"   Runtime type korunur: {employee is Manager}");
    }

    static void DemonstrateAsOperator(params Employee[] employees)
    {
        Console.WriteLine("as operatörü - Güvenli downcasting:\n");

        foreach (var emp in employees)
        {
            // ✅ 'as' operatörü - başarısız olursa null döner
            Manager? manager = emp as Manager;

            if (manager != null)
            {
                Console.WriteLine($"✅ {emp.Name} bir Manager");
                manager.HoldMeeting();
            }
            else
            {
                Console.WriteLine($"❌ {emp.Name} Manager değil (null döndü)");
            }
            Console.WriteLine();
        }
    }

    static void DemonstrateExplicitCast(Employee managerEmp, Employee regularEmp)
    {
        try
        {
            // ✅ Başarılı explicit cast
            Manager manager = (Manager)managerEmp;
            Console.WriteLine($"✅ Başarılı cast: {manager.Name}");
            manager.HoldMeeting();
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine($"❌ Cast başarısız: {ex.Message}");
        }

        Console.WriteLine();

        try
        {
            // ❌ Başarısız explicit cast - Exception atar!
            Manager impossibleCast = (Manager)regularEmp;
            Console.WriteLine($"Bu satır çalışmaz: {impossibleCast.Name}");
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine($"❌ Exception yakalandı: InvalidCastException");
            Console.WriteLine($"   Mesaj: {ex.Message}");
            Console.WriteLine($"   Çözüm: 'as' operatörü veya pattern matching kullan!");
        }
    }

    static void DemonstrateIsOperator(params Employee[] employees)
    {
        Console.WriteLine("is operatörü - Type checking:\n");

        foreach (var emp in employees)
        {
            Console.Write($"{emp.Name}: ");

            // 'is' operatörü sadece kontrol eder, cast etmez
            if (emp is Manager)
                Console.Write("Manager ✓ ");
            if (emp is Developer)
                Console.Write("Developer ✓ ");
            if (emp is HRSpecialist)
                Console.Write("HRSpecialist ✓ ");
            if (emp is Employee)  // Hepsi Employee
                Console.Write("Employee ✓");

            Console.WriteLine();
        }
    }

    static void DemonstratePatternMatching(params Employee[] employees)
    {
        Console.WriteLine("Pattern Matching - is ile cast birleşimi:\n");

        foreach (var emp in employees)
        {
            // ✅ BEST PRACTICE: Pattern matching ile kontrol ve cast tek satırda
            if (emp is Manager mgr)
            {
                Console.WriteLine($"✅ {emp.Name} bir Manager");
                Console.WriteLine($"   Ekip: {mgr.Team.Count} kişi, Bonus: {mgr.Bonus:C}");
                mgr.HoldMeeting();
            }
            else if (emp is Developer dev)
            {
                Console.WriteLine($"✅ {emp.Name} bir Developer");
                Console.WriteLine($"   Dil: {dev.ProgrammingLanguage}, Deneyim: {dev.YearsOfExperience} yıl");
                dev.WriteCode();
            }
            else if (emp is HRSpecialist hr)
            {
                Console.WriteLine($"✅ {emp.Name} bir HRSpecialist");
                Console.WriteLine($"   Yönetilen: {hr.EmployeesManaged} çalışan");
                hr.ProcessPayroll();
            }
            else
            {
                Console.WriteLine($"ℹ️  {emp.Name} - Base Employee");
                emp.Work();
            }
            Console.WriteLine();
        }
    }

    static void DemonstrateSwitchPattern(params Employee[] employees)
    {
        Console.WriteLine("Switch Expression - Elegant pattern matching:\n");

        foreach (var emp in employees)
        {
            // ✅ Modern C# 12 switch expression
            string role = emp switch
            {
                Manager m => $"Yönetici (Ekip: {m.Team.Count}, Bonus: {m.Bonus:C})",
                Developer d => $"Geliştirici ({d.ProgrammingLanguage}, {d.YearsOfExperience} yıl)",
                HRSpecialist hr => $"İK Uzmanı ({hr.EmployeesManaged} çalışan)",
                Employee e => $"Genel Çalışan (Departman: {e.Department})",
                _ => "Bilinmeyen"
            };

            Console.WriteLine($"{emp.Name}: {role}");
        }
    }
}
