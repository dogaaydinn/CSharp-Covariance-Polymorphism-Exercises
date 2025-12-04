// SCENARIO: Veritabanı sağlayıcıları - Interface implementation patterns
// BAD PRACTICE: Çakışan metod adlarında explicit implementation kullanmamak
// GOOD PRACTICE: Explicit interface implementation ile metod çakışmalarını çöz

using InterfaceBasics;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Interface Implementation Patterns ===\n");

        Console.WriteLine("=== 1. Implicit Interface Implementation (SqlDatabase) ===\n");
        DemonstrateImplicitImplementation();

        Console.WriteLine("\n=== 2. Explicit Interface Implementation (MongoDatabase) ===\n");
        DemonstrateExplicitImplementation();

        Console.WriteLine("\n=== 3. Çoklu Interface Implementation ===\n");
        DemonstrateMultipleInterfaces();

        Console.WriteLine("\n=== 4. Interface Referans vs Class Referans ===\n");
        DemonstrateInterfaceVsClassReference();

        Console.WriteLine("\n=== 5. Gerçek Dünya: Repository Pattern ===\n");
        DemonstrateRepositoryPattern();

        // Analiz
        Console.WriteLine("\n=== Output Analysis ===");
        Console.WriteLine("1. Implicit implementation: Metod hem interface hem class üzerinden çağrılabilir");
        Console.WriteLine("2. Explicit implementation: Metod SADECE interface referansı üzerinden çağrılabilir");
        Console.WriteLine("3. Çoklu interface: Bir class birden fazla interface implement edebilir");
        Console.WriteLine("4. Metod çakışması: Explicit implementation ile çözülür");

        Console.WriteLine("\n💡 Best Practice:");
        Console.WriteLine("   ✅ Kullan: Implicit implementation (varsayılan)");
        Console.WriteLine("   ✅ Kullan: Explicit implementation (metod çakışması varsa)");
        Console.WriteLine("   ✅ Kullan: Interface referansı (abstraction için)");
    }

    /// <summary>
    /// Implicit implementation - Public metodlar
    /// </summary>
    static void DemonstrateImplicitImplementation()
    {
        SqlDatabase sqlDb = new() { ConnectionString = "Server=localhost;Database=MyDb" };

        // ✅ Class referansı üzerinden çağrılabilir
        Console.WriteLine("Class referansı üzerinden:");
        sqlDb.Connect();
        sqlDb.ExecuteQuery("SELECT * FROM Users");
        sqlDb.ExecuteStoredProcedure("sp_GetUsers");  // Class'a özgü metod
        sqlDb.Disconnect();

        Console.WriteLine("\nInterface referansı üzerinden:");
        IDatabase db = sqlDb;
        db.Connect();
        db.ExecuteQuery("SELECT * FROM Products");
        // db.ExecuteStoredProcedure();  // ❌ Hata! Interface'de yok
        db.Disconnect();
    }

    /// <summary>
    /// Explicit implementation - Sadece interface referansı üzerinden
    /// </summary>
    static void DemonstrateExplicitImplementation()
    {
        MongoDatabase mongo = new() { ConnectionString = "mongodb://localhost:27017" };

        Console.WriteLine("⚠️  EXPLICIT IMPLEMENTATION - Class referansı üzerinden çağrılamaz:\n");

        // ❌ mongo.Connect();  // Derleme hatası! Explicit implementation

        // ✅ IDatabase referansı üzerinden
        IDatabase dbInterface = mongo;
        Console.WriteLine("IDatabase referansı:");
        dbInterface.Connect();  // SQL-like bağlantı
        dbInterface.ExecuteQuery("db.users.find()");
        Console.WriteLine();

        // ✅ INoSqlDatabase referansı üzerinden
        INoSqlDatabase nosqlInterface = mongo;
        Console.WriteLine("INoSqlDatabase referansı:");
        nosqlInterface.Connect();  // NoSQL bağlantı
        nosqlInterface.InsertDocument("{name: 'Ahmet', age: 30}");
        Console.WriteLine();

        // ✅ Public metod - Her referanstan çağrılabilir
        Console.WriteLine("Public metod (implicit):");
        mongo.Disconnect();
        mongo.CreateIndex("name");  // Class'a özgü public metod
    }

    /// <summary>
    /// Bir class birden fazla interface implement edebilir
    /// </summary>
    static void DemonstrateMultipleInterfaces()
    {
        MongoDatabase mongo = new() { ConnectionString = "mongodb://localhost:27017" };

        // ICacheProvider interface'i
        ICacheProvider cache = mongo;
        cache.Set("user:1", new { Name = "Ali", Age = 25 });
        cache.Set("user:2", new { Name = "Ayşe", Age = 28 });

        var user1 = cache.Get("user:1");
        var user3 = cache.Get("user:3");  // Bulunamaz

        cache.Remove("user:1");

        Console.WriteLine("\n✅ Aynı instance 3 farklı interface olarak kullanıldı:");
        Console.WriteLine("   - IDatabase");
        Console.WriteLine("   - INoSqlDatabase");
        Console.WriteLine("   - ICacheProvider");
    }

    /// <summary>
    /// Interface vs Class referansı farkı
    /// </summary>
    static void DemonstrateInterfaceVsClassReference()
    {
        MongoDatabase mongo = new() { ConnectionString = "mongodb://localhost:27017" };

        Console.WriteLine("Class Referansı (MongoDatabase):");
        Console.WriteLine($"   Tip: {mongo.GetType().Name}");
        Console.WriteLine("   Çağrılabilir metodlar:");
        Console.WriteLine("     ✅ InsertDocument() - public");
        Console.WriteLine("     ✅ Disconnect() - public");
        Console.WriteLine("     ✅ CreateIndex() - public (class'a özgü)");
        Console.WriteLine("     ❌ Connect() - explicit, çağrılamaz!");

        Console.WriteLine("\nInterface Referansı (IDatabase):");
        IDatabase db = mongo;
        Console.WriteLine($"   Static Tip: IDatabase");
        Console.WriteLine($"   Runtime Tip: {db.GetType().Name}");
        Console.WriteLine("   Çağrılabilir metodlar:");
        Console.WriteLine("     ✅ Connect() - explicit implementation");
        Console.WriteLine("     ✅ ExecuteQuery() - explicit implementation");
        Console.WriteLine("     ✅ Disconnect() - public (implicit)");
        Console.WriteLine("     ❌ CreateIndex() - interface'de yok!");
    }

    /// <summary>
    /// Gerçek dünya örneği: Repository Pattern
    /// </summary>
    static void DemonstrateRepositoryPattern()
    {
        Console.WriteLine("Repository Pattern - Interface segregation:\n");

        // Farklı implementasyonlar aynı interface'i kullanır
        List<IDatabase> databases = new()
        {
            new SqlDatabase { ConnectionString = "SQL Server" },
            // MongoDatabase için IDatabase cast gerekir
        };

        Console.WriteLine("Polymorphic kullanım:");
        foreach (var db in databases)
        {
            Console.WriteLine($"\n{db.GetType().Name}:");
            db.Connect();
            db.ExecuteQuery("SELECT * FROM Data");
            db.Disconnect();
        }

        Console.WriteLine("\n💡 Interface sayesinde:");
        Console.WriteLine("   - Loose coupling (gevşek bağlılık)");
        Console.WriteLine("   - Dependency Injection kullanılabilir");
        Console.WriteLine("   - Test edilebilir kod (mock objects)");
        Console.WriteLine("   - Implementasyon değiştirilebilir");
    }
}
