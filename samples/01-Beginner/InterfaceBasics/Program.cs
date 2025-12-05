// SCENARIO: Çoklu veritabanı desteği - Interface patterns
// BAD PRACTICE: switch-case ile veritabanı tipini kontrol etmek
// GOOD PRACTICE: Interface kullanarak Dependency Injection ve polymorphism

using InterfaceBasics;

class Program
{
    static void Main()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║   INTERFACE BASICS - Çoklu Veritabanı Yönetimi      ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // 1. BAD PRACTICE: Switch-case ile database tipi kontrolü
        Console.WriteLine("═══ 1. ❌ BAD PRACTICE: Switch-Case Yaklaşımı ═══\n");
        DemonstrateBadPractice();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 2. GOOD PRACTICE: Interface kullanımı
        Console.WriteLine("═══ 2. ✅ GOOD PRACTICE: Interface Kullanımı ═══\n");
        DemonstrateGoodPractice();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 3. Dependency Injection simulation
        Console.WriteLine("═══ 3. ✅ DEPENDENCY INJECTION SİMÜLASYONU ═══\n");
        DemonstrateDependencyInjection();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 4. Explicit Interface Implementation
        Console.WriteLine("═══ 4. EXPLICIT INTERFACE IMPLEMENTATION ═══\n");
        DemonstrateExplicitImplementation();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 5. Interface Segregation
        Console.WriteLine("═══ 5. INTERFACE SEGREGATION ═══\n");
        DemonstrateInterfaceSegregation();

        // Final Analysis
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    📊 ANALYSIS                        ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("❌ BAD PRACTICE (Switch-Case):");
        Console.WriteLine("   • Tight coupling (sıkı bağlılık)");
        Console.WriteLine("   • Yeni database eklemek için kod değişikliği gerekir");
        Console.WriteLine("   • Test edilmesi zor");
        Console.WriteLine("   • Open/Closed prensibini ihlal eder");
        Console.WriteLine();
        Console.WriteLine("✅ GOOD PRACTICE (Interface):");
        Console.WriteLine("   • Loose coupling (gevşek bağlılık)");
        Console.WriteLine("   • Yeni database eklemek için sadece yeni class");
        Console.WriteLine("   • Kolayca test edilebilir (mock objects)");
        Console.WriteLine("   • SOLID prensiplerini takip eder");
        Console.WriteLine("   • Dependency Injection kullanılabilir");
    }

    /// <summary>
    /// BAD PRACTICE: Switch-case ile veritabanı yönetimi
    /// Problem: Her yeni database için switch-case'e yeni durum eklemek gerekir.
    /// </summary>
    static void DemonstrateBadPractice()
    {
        Console.WriteLine("💀 Kötü Tasarım: DatabaseManager class'ı ile switch-case kullanımı\n");

        DatabaseManager manager = new DatabaseManager();

        // SQL Server
        manager.SetDatabaseType("SqlServer");
        manager.Connect("Server=localhost;Database=MyDb");
        manager.RunQuery("SELECT * FROM Users");
        manager.Close();

        Console.WriteLine();

        // MongoDB
        manager.SetDatabaseType("MongoDB");
        manager.Connect("mongodb://localhost:27017");
        manager.RunQuery("db.users.find()");
        manager.Close();

        Console.WriteLine();

        // PostgreSQL
        manager.SetDatabaseType("PostgreSQL");
        manager.Connect("Host=localhost;Database=mydb");
        manager.RunQuery("SELECT * FROM products");
        manager.Close();

        Console.WriteLine("\n⚠️  SORUNLAR:");
        Console.WriteLine("   1. Her yeni database için switch-case'e ekleme yapılmalı");
        Console.WriteLine("   2. DatabaseManager class'ı çok fazla sorumluluk alıyor");
        Console.WriteLine("   3. Kod değişiklikleri mevcut kodu bozma riski taşıyor");
        Console.WriteLine("   4. Unit test yazmak çok zor");
    }

    /// <summary>
    /// GOOD PRACTICE: Interface kullanarak polymorphism
    /// Avantaj: Yeni database eklemek için sadece yeni class oluşturmak yeterli.
    /// </summary>
    static void DemonstrateGoodPractice()
    {
        Console.WriteLine("🎯 İyi Tasarım: IDatabase interface ile polymorphism\n");

        // Liste oluştur - Farklı implementasyonlar
        List<IDatabase> databases = new List<IDatabase>
        {
            new SqlDatabase { ConnectionString = "Server=localhost;Database=MyDb" },
            new MongoDatabase { ConnectionString = "mongodb://localhost:27017" },
            new PostgreSqlDatabase { ConnectionString = "Host=localhost;Database=mydb" }
        };

        // Polymorphic kullanım - Hepsi aynı interface'i implement ediyor
        foreach (var db in databases)
        {
            Console.WriteLine($"📦 Database Type: {db.GetType().Name}");
            db.Connect();
            db.ExecuteQuery("SELECT/FIND data FROM collection/table");
            db.Disconnect();
            Console.WriteLine();
        }

        Console.WriteLine("✅ AVANTAJLAR:");
        Console.WriteLine("   1. Yeni database eklemek için sadece yeni class gerekir");
        Console.WriteLine("   2. Her class kendi sorumluluğunu alır (SRP)");
        Console.WriteLine("   3. Mevcut kodu değiştirmeden genişletilebilir (OCP)");
        Console.WriteLine("   4. Kolayca test edilebilir (mock interface)");
    }

    /// <summary>
    /// DEPENDENCY INJECTION simulation
    /// Interface sayesinde DataService belli bir implementasyona bağımlı değil.
    /// </summary>
    static void DemonstrateDependencyInjection()
    {
        Console.WriteLine("💉 Constructor Injection ile loose coupling\n");

        // Farklı implementasyonlar inject edilebilir
        Console.WriteLine("--- SQL Server ile DataService ---");
        IDatabase sqlDb = new SqlDatabase { ConnectionString = "SQL Connection" };
        DataService sqlService = new DataService(sqlDb);
        sqlService.ProcessData();

        Console.WriteLine("\n--- MongoDB ile DataService ---");
        IDatabase mongoDb = new MongoDatabase { ConnectionString = "MongoDB Connection" };
        DataService mongoService = new DataService(mongoDb);
        mongoService.ProcessData();

        Console.WriteLine("\n--- PostgreSQL ile DataService ---");
        IDatabase pgDb = new PostgreSqlDatabase { ConnectionString = "PostgreSQL Connection" };
        DataService pgService = new DataService(pgDb);
        pgService.ProcessData();

        Console.WriteLine("\n💡 Dependency Injection Benefits:");
        Console.WriteLine("   • DataService implementasyondan bağımsız");
        Console.WriteLine("   • Test için mock database inject edilebilir");
        Console.WriteLine("   • Runtime'da farklı implementasyon kullanılabilir");
    }

    /// <summary>
    /// EXPLICIT INTERFACE IMPLEMENTATION
    /// MongoDatabase hem IDatabase hem INoSqlDatabase implement ediyor.
    /// Connect() metodu iki interface'de de var - explicit implementation gerekir.
    /// </summary>
    static void DemonstrateExplicitImplementation()
    {
        MongoDatabase mongo = new MongoDatabase { ConnectionString = "mongodb://localhost:27017" };

        Console.WriteLine("📌 MongoDatabase 2 interface implement ediyor:\n");

        // IDatabase referansı
        IDatabase dbInterface = mongo;
        Console.WriteLine("IDatabase referansı üzerinden:");
        dbInterface.Connect();  // SQL-like connection
        dbInterface.ExecuteQuery("SELECT equivalent query");
        Console.WriteLine();

        // INoSqlDatabase referansı
        INoSqlDatabase nosqlInterface = mongo;
        Console.WriteLine("INoSqlDatabase referansı üzerinden:");
        nosqlInterface.Connect();  // NoSQL connection
        nosqlInterface.InsertDocument("{user: 'John', age: 30}");
        Console.WriteLine();

        // Public metod
        Console.WriteLine("Public metod (class referansı):");
        mongo.Disconnect();

        Console.WriteLine("\n🔍 Explicit Implementation:");
        Console.WriteLine("   • Connect() metodu her iki interface'de de var");
        Console.WriteLine("   • Explicit implementation ile çakışma çözülür");
        Console.WriteLine("   • Interface referansı üzerinden farklı davranış");
    }

    /// <summary>
    /// INTERFACE SEGREGATION
    /// MongoDatabase ayrıca ICacheProvider da implement ediyor.
    /// </summary>
    static void DemonstrateInterfaceSegregation()
    {
        MongoDatabase mongo = new MongoDatabase { ConnectionString = "mongodb://localhost:27017" };

        Console.WriteLine("🧩 Aynı instance, farklı interface'ler:\n");

        // Cache olarak kullan
        ICacheProvider cache = mongo;
        cache.Set("user:1", new { Name = "Alice", Age = 25 });
        cache.Set("user:2", new { Name = "Bob", Age = 30 });

        var user1 = cache.Get("user:1");
        Console.WriteLine($"Cache'den okunan: {user1}");

        cache.Remove("user:1");

        Console.WriteLine("\n✅ Interface Segregation Principle:");
        Console.WriteLine("   • Tek class, birden fazla interface implement edebilir");
        Console.WriteLine("   • Her interface farklı bir yetenek sağlar");
        Console.WriteLine("   • Client sadece ihtiyacı olan interface'i kullanır");
    }
}

/// <summary>
/// BAD PRACTICE: Switch-case ile database yönetimi
/// Her yeni database için switch-case'e yeni durum eklemek gerekir.
/// </summary>
class DatabaseManager
{
    private string _dbType = "";
    private string _connectionString = "";

    public void SetDatabaseType(string type)
    {
        _dbType = type;
        Console.WriteLine($"Database type set to: {type}");
    }

    public void Connect(string connectionString)
    {
        _connectionString = connectionString;

        // ❌ BAD: Switch-case her database için
        switch (_dbType)
        {
            case "SqlServer":
                Console.WriteLine($"[SQL Server] Connecting to: {connectionString}");
                Console.WriteLine("[SQL Server] ✅ Connected!");
                break;
            case "MongoDB":
                Console.WriteLine($"[MongoDB] Connecting to: {connectionString}");
                Console.WriteLine("[MongoDB] ✅ Connected!");
                break;
            case "PostgreSQL":
                Console.WriteLine($"[PostgreSQL] Connecting to: {connectionString}");
                Console.WriteLine("[PostgreSQL] ✅ Connected!");
                break;
            default:
                Console.WriteLine("❌ Unknown database type!");
                break;
        }
    }

    public void RunQuery(string query)
    {
        // ❌ BAD: Her query için switch-case
        switch (_dbType)
        {
            case "SqlServer":
                Console.WriteLine($"[SQL Server] Executing: {query}");
                break;
            case "MongoDB":
                Console.WriteLine($"[MongoDB] Executing: {query}");
                break;
            case "PostgreSQL":
                Console.WriteLine($"[PostgreSQL] Executing: {query}");
                break;
        }
    }

    public void Close()
    {
        switch (_dbType)
        {
            case "SqlServer":
                Console.WriteLine("[SQL Server] Disconnected.");
                break;
            case "MongoDB":
                Console.WriteLine("[MongoDB] Disconnected.");
                break;
            case "PostgreSQL":
                Console.WriteLine("[PostgreSQL] Disconnected.");
                break;
        }
    }
}

/// <summary>
/// GOOD PRACTICE: Constructor Injection ile DataService
/// Interface'e bağımlı, implementasyona değil.
/// </summary>
class DataService
{
    private readonly IDatabase _database;

    // Constructor Injection
    public DataService(IDatabase database)
    {
        _database = database;
    }

    public void ProcessData()
    {
        _database.Connect();
        _database.ExecuteQuery("SELECT * FROM Data");
        _database.Disconnect();
    }
}
