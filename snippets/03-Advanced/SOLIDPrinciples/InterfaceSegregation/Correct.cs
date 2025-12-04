namespace SOLIDPrinciples.InterfaceSegregation;

/// <summary>
/// CORRECT: Segregated interfaces - clients depend only on what they use.
/// Benefits:
/// - No forced implementations
/// - Clear capability contracts
/// - Low coupling
/// - Better composability
/// </summary>

#region Segregated Worker Interfaces

/// <summary>
/// Core interface - all workers can work
/// </summary>
public interface IWorkable
{
    void Work();
}

/// <summary>
/// Separate interface for biological needs
/// </summary>
public interface IFeedable
{
    void Eat();
}

/// <summary>
/// Separate interface for rest
/// </summary>
public interface IRestable
{
    void Sleep();
    void TakeBreak();
}

/// <summary>
/// Separate interface for payment
/// </summary>
public interface IPayable
{
    void GetPaid(decimal amount);
}

/// <summary>
/// Human implements all interfaces
/// </summary>
public class HumanWorkerCorrect : IWorkable, IFeedable, IRestable, IPayable
{
    public string Name { get; set; } = string.Empty;

    public void Work()
    {
        Console.WriteLine($"[CORRECT] {Name} (Human) is working efficiently");
    }

    public void Eat()
    {
        Console.WriteLine($"[CORRECT] {Name} is eating a healthy lunch");
    }

    public void Sleep()
    {
        Console.WriteLine($"[CORRECT] {Name} is getting rest");
    }

    public void TakeBreak()
    {
        Console.WriteLine($"[CORRECT] {Name} is taking a refreshing break");
    }

    public void GetPaid(decimal amount)
    {
        Console.WriteLine($"[CORRECT] {Name} received salary: ${amount}");
    }
}

/// <summary>
/// Robot only implements what it can do
/// </summary>
public class RobotWorkerCorrect : IWorkable
{
    public string SerialNumber { get; set; } = string.Empty;

    public void Work()
    {
        Console.WriteLine($"[CORRECT] Robot {SerialNumber} is working 24/7");
    }

    // No Eat(), Sleep(), TakeBreak(), or GetPaid() - robots don't need these!
}

/// <summary>
/// Contractor implements work and payment only
/// </summary>
public class Contractor : IWorkable, IPayable
{
    public string Name { get; set; } = string.Empty;

    public void Work()
    {
        Console.WriteLine($"[CORRECT] Contractor {Name} is working on project");
    }

    public void GetPaid(decimal amount)
    {
        Console.WriteLine($"[CORRECT] Contractor {Name} invoiced: ${amount}");
    }

    // No biological needs - manages their own schedule
}

/// <summary>
/// Manager that respects segregated interfaces
/// </summary>
public class WorkManagerCorrect
{
    public static void AssignWork(IWorkable worker)
    {
        Console.WriteLine("\n[CORRECT] Assigning work:");
        worker.Work();
    }

    public static void ProcessPayroll(IPayable employee, decimal amount)
    {
        Console.WriteLine("\n[CORRECT] Processing payroll:");
        employee.GetPaid(amount);
    }

    public static void ScheduleBreak(IRestable employee)
    {
        Console.WriteLine("\n[CORRECT] Scheduling break:");
        employee.TakeBreak();
    }

    public static void ProvideLunch(IFeedable employee)
    {
        Console.WriteLine("\n[CORRECT] Providing lunch:");
        employee.Eat();
    }
}

#endregion

#region Segregated Printer Interfaces

/// <summary>
/// Basic printing capability
/// </summary>
public interface IPrinter
{
    void Print(string document);
}

/// <summary>
/// Scanning capability
/// </summary>
public interface IScanner
{
    void Scan(string document);
}

/// <summary>
/// Fax capability
/// </summary>
public interface IFax
{
    void Fax(string document);
}

/// <summary>
/// Photocopy capability
/// </summary>
public interface IPhotocopier
{
    void Photocopy(string document);
}

/// <summary>
/// Email capability
/// </summary>
public interface IEmailSender
{
    void Email(string document, string address);
}

/// <summary>
/// Cloud upload capability
/// </summary>
public interface ICloudConnected
{
    void CloudUpload(string document, string cloudService);
}

/// <summary>
/// Basic printer - only printing
/// </summary>
public class SimplePrinter : IPrinter
{
    public void Print(string document)
    {
        Console.WriteLine($"[CORRECT] Simple printer: Printing {document}");
    }
}

/// <summary>
/// Mid-range printer - printing and scanning
/// </summary>
public class PrinterScanner : IPrinter, IScanner
{
    public void Print(string document)
    {
        Console.WriteLine($"[CORRECT] Printer-Scanner: Printing {document}");
    }

    public void Scan(string document)
    {
        Console.WriteLine($"[CORRECT] Printer-Scanner: Scanning {document}");
    }
}

/// <summary>
/// High-end multi-function device
/// </summary>
public class MultiFunctionDevice : IPrinter, IScanner, IFax, IPhotocopier, IEmailSender, ICloudConnected
{
    public void Print(string document)
    {
        Console.WriteLine($"[CORRECT] MFD: Printing {document}");
    }

    public void Scan(string document)
    {
        Console.WriteLine($"[CORRECT] MFD: Scanning {document}");
    }

    public void Fax(string document)
    {
        Console.WriteLine($"[CORRECT] MFD: Faxing {document}");
    }

    public void Photocopy(string document)
    {
        Console.WriteLine($"[CORRECT] MFD: Photocopying {document}");
    }

    public void Email(string document, string address)
    {
        Console.WriteLine($"[CORRECT] MFD: Emailing {document} to {address}");
    }

    public void CloudUpload(string document, string cloudService)
    {
        Console.WriteLine($"[CORRECT] MFD: Uploading {document} to {cloudService}");
    }
}

/// <summary>
/// Virtual printer - prints to file and cloud
/// </summary>
public class VirtualPrinter : IPrinter, ICloudConnected
{
    public void Print(string document)
    {
        Console.WriteLine($"[CORRECT] Virtual printer: Creating PDF of {document}");
    }

    public void CloudUpload(string document, string cloudService)
    {
        Console.WriteLine($"[CORRECT] Virtual printer: Uploading PDF to {cloudService}");
    }
}

#endregion

#region Segregated Vehicle Interfaces

/// <summary>
/// Road travel capability
/// </summary>
public interface IDrivable
{
    void Drive();
    int GetMaxSpeed();
}

/// <summary>
/// Air travel capability
/// </summary>
public interface IFlyable
{
    void Fly();
    int GetMaxAltitude();
}

/// <summary>
/// Water surface travel capability
/// </summary>
public interface ISailable
{
    void Sail();
}

/// <summary>
/// Underwater travel capability
/// </summary>
public interface ISubmersible
{
    void Submerge();
    int GetMaxDepth();
}

/// <summary>
/// Combat capability
/// </summary>
public interface IWeaponized
{
    void LaunchMissiles();
    int GetMissileCount();
}

/// <summary>
/// Regular car - only drives
/// </summary>
public class CarCorrect : IDrivable
{
    public void Drive()
    {
        Console.WriteLine($"[CORRECT] Car: Driving on the highway");
    }

    public int GetMaxSpeed()
    {
        return 120; // mph
    }
}

/// <summary>
/// Airplane - flies and drives (taxi)
/// </summary>
public class Airplane : IFlyable, IDrivable
{
    public void Fly()
    {
        Console.WriteLine($"[CORRECT] Airplane: Flying at cruising altitude");
    }

    public void Drive()
    {
        Console.WriteLine($"[CORRECT] Airplane: Taxiing on runway");
    }

    public int GetMaxAltitude()
    {
        return 40000; // feet
    }

    public int GetMaxSpeed()
    {
        return 30; // mph on ground
    }
}

/// <summary>
/// Boat - sails on water
/// </summary>
public class Boat : ISailable
{
    public void Sail()
    {
        Console.WriteLine($"[CORRECT] Boat: Sailing on the ocean");
    }
}

/// <summary>
/// Submarine - sails and submerges, has weapons
/// </summary>
public class SubmarineCorrect : ISailable, ISubmersible, IWeaponized
{
    public void Sail()
    {
        Console.WriteLine($"[CORRECT] Submarine: Sailing on the surface");
    }

    public void Submerge()
    {
        Console.WriteLine($"[CORRECT] Submarine: Diving underwater");
    }

    public int GetMaxDepth()
    {
        return 2000; // feet
    }

    public void LaunchMissiles()
    {
        Console.WriteLine($"[CORRECT] Submarine: Launching torpedoes");
    }

    public int GetMissileCount()
    {
        return 20;
    }
}

/// <summary>
/// Amphibious vehicle - drives and sails
/// </summary>
public class AmphibiousVehicle : IDrivable, ISailable
{
    public void Drive()
    {
        Console.WriteLine($"[CORRECT] Amphibious vehicle: Driving on land");
    }

    public void Sail()
    {
        Console.WriteLine($"[CORRECT] Amphibious vehicle: Floating on water");
    }

    public int GetMaxSpeed()
    {
        return 60; // mph on land
    }
}

#endregion

#region Segregated Database Interfaces

/// <summary>
/// Basic CRUD operations - all databases support
/// </summary>
public interface ICrudOperations
{
    void Create(string table, object data);
    void Read(string table, int id);
    void Update(string table, int id, object data);
    void Delete(string table, int id);
}

/// <summary>
/// Transaction support
/// </summary>
public interface ITransactional
{
    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
}

/// <summary>
/// SQL-specific features
/// </summary>
public interface ISqlDatabase
{
    void ExecuteStoredProcedure(string procedureName);
    void CreateView(string viewName, string query);
}

/// <summary>
/// Index management
/// </summary>
public interface IIndexable
{
    void CreateIndex(string table, string column);
    void DropIndex(string table, string indexName);
}

/// <summary>
/// Full-text search capability
/// </summary>
public interface ISearchable
{
    void FullTextSearch(string query);
}

/// <summary>
/// SQL Server - supports all SQL features
/// </summary>
public class SqlServerDatabase : ICrudOperations, ITransactional, ISqlDatabase, IIndexable
{
    public void Create(string table, object data)
    {
        Console.WriteLine($"[CORRECT] SQL Server: INSERT INTO {table}");
    }

    public void Read(string table, int id)
    {
        Console.WriteLine($"[CORRECT] SQL Server: SELECT FROM {table} WHERE id={id}");
    }

    public void Update(string table, int id, object data)
    {
        Console.WriteLine($"[CORRECT] SQL Server: UPDATE {table} WHERE id={id}");
    }

    public void Delete(string table, int id)
    {
        Console.WriteLine($"[CORRECT] SQL Server: DELETE FROM {table} WHERE id={id}");
    }

    public void BeginTransaction()
    {
        Console.WriteLine($"[CORRECT] SQL Server: BEGIN TRANSACTION");
    }

    public void CommitTransaction()
    {
        Console.WriteLine($"[CORRECT] SQL Server: COMMIT");
    }

    public void RollbackTransaction()
    {
        Console.WriteLine($"[CORRECT] SQL Server: ROLLBACK");
    }

    public void ExecuteStoredProcedure(string procedureName)
    {
        Console.WriteLine($"[CORRECT] SQL Server: EXEC {procedureName}");
    }

    public void CreateView(string viewName, string query)
    {
        Console.WriteLine($"[CORRECT] SQL Server: CREATE VIEW {viewName}");
    }

    public void CreateIndex(string table, string column)
    {
        Console.WriteLine($"[CORRECT] SQL Server: CREATE INDEX ON {table}({column})");
    }

    public void DropIndex(string table, string indexName)
    {
        Console.WriteLine($"[CORRECT] SQL Server: DROP INDEX {indexName} ON {table}");
    }
}

/// <summary>
/// MongoDB - supports CRUD, transactions, and indexing (no SQL-specific features)
/// </summary>
public class MongoDatabaseCorrect : ICrudOperations, ITransactional, IIndexable, ISearchable
{
    public void Create(string collection, object data)
    {
        Console.WriteLine($"[CORRECT] MongoDB: db.{collection}.insertOne()");
    }

    public void Read(string collection, int id)
    {
        Console.WriteLine($"[CORRECT] MongoDB: db.{collection}.findOne({{_id: {id}}})");
    }

    public void Update(string collection, int id, object data)
    {
        Console.WriteLine($"[CORRECT] MongoDB: db.{collection}.updateOne({{_id: {id}}})");
    }

    public void Delete(string collection, int id)
    {
        Console.WriteLine($"[CORRECT] MongoDB: db.{collection}.deleteOne({{_id: {id}}})");
    }

    public void BeginTransaction()
    {
        Console.WriteLine($"[CORRECT] MongoDB: session.startTransaction()");
    }

    public void CommitTransaction()
    {
        Console.WriteLine($"[CORRECT] MongoDB: session.commitTransaction()");
    }

    public void RollbackTransaction()
    {
        Console.WriteLine($"[CORRECT] MongoDB: session.abortTransaction()");
    }

    public void CreateIndex(string collection, string field)
    {
        Console.WriteLine($"[CORRECT] MongoDB: db.{collection}.createIndex({{{field}: 1}})");
    }

    public void DropIndex(string collection, string indexName)
    {
        Console.WriteLine($"[CORRECT] MongoDB: db.{collection}.dropIndex('{indexName}')");
    }

    public void FullTextSearch(string query)
    {
        Console.WriteLine($"[CORRECT] MongoDB: Text search for '{query}'");
    }
}

/// <summary>
/// Redis - simple key-value store (only basic CRUD)
/// </summary>
public class RedisDatabase : ICrudOperations
{
    public void Create(string key, object value)
    {
        Console.WriteLine($"[CORRECT] Redis: SET {key}");
    }

    public void Read(string key, int id)
    {
        Console.WriteLine($"[CORRECT] Redis: GET {key}");
    }

    public void Update(string key, int id, object value)
    {
        Console.WriteLine($"[CORRECT] Redis: SET {key} (overwrite)");
    }

    public void Delete(string key, int id)
    {
        Console.WriteLine($"[CORRECT] Redis: DEL {key}");
    }
}

#endregion

/// <summary>
/// Demonstrates the benefits of Interface Segregation Principle
/// </summary>
public class InterfaceSegregationCorrectDemo
{
    public static void DemonstrateBenefits()
    {
        Console.WriteLine("\n=== BENEFITS OF INTERFACE SEGREGATION PRINCIPLE ===");

        Console.WriteLine("\nBenefit 1: No forced implementations");
        Console.WriteLine("  Classes only implement what they can do");
        Console.WriteLine("  No dummy or exception-throwing methods");

        Console.WriteLine("\nBenefit 2: Clear capability contracts");
        Console.WriteLine("  Interface names clearly indicate capabilities");
        Console.WriteLine("  Easy to understand what an object can do");

        Console.WriteLine("\nBenefit 3: Low coupling");
        Console.WriteLine("  Clients depend only on methods they use");
        Console.WriteLine("  Changes to unused interfaces don't affect clients");

        Console.WriteLine("\nBenefit 4: Better composability");
        Console.WriteLine("  Combine interfaces to create exactly what you need");
        Console.WriteLine("  Mix and match capabilities");

        DemonstrateWorkerInterfaces();
        DemonstratePrinterInterfaces();
        DemonstrateVehicleInterfaces();
        DemonstrateDatabaseInterfaces();
    }

    private static void DemonstrateWorkerInterfaces()
    {
        Console.WriteLine("\n--- Segregated Worker Interfaces ---");

        var human = new HumanWorkerCorrect { Name = "Alice" };
        var robot = new RobotWorkerCorrect { SerialNumber = "R2D2" };
        var contractor = new Contractor { Name = "Bob" };

        // Each worker type only exposes relevant capabilities
        WorkManagerCorrect.AssignWork(human);
        WorkManagerCorrect.AssignWork(robot);
        WorkManagerCorrect.AssignWork(contractor);

        WorkManagerCorrect.ProvideLunch(human);
        // Can't call ProvideLunch on robot - compiler prevents it!

        WorkManagerCorrect.ProcessPayroll(human, 5000);
        WorkManagerCorrect.ProcessPayroll(contractor, 3000);
        // Can't call ProcessPayroll on robot - compiler prevents it!
    }

    private static void DemonstratePrinterInterfaces()
    {
        Console.WriteLine("\n--- Segregated Printer Interfaces ---");

        IPrinter simple = new SimplePrinter();
        simple.Print("Document1.pdf");

        var scanner = new PrinterScanner();
        scanner.Print("Document2.pdf");
        scanner.Scan("Photo.jpg");

        var mfd = new MultiFunctionDevice();
        mfd.Print("Report.pdf");
        mfd.Scan("Contract.pdf");
        if (mfd is IEmailSender emailer)
        {
            emailer.Email("Report.pdf", "boss@company.com");
        }

        Console.WriteLine("\n[CORRECT] Each printer exposes only its capabilities");
        Console.WriteLine("  Compiler prevents calling unsupported operations!");
    }

    private static void DemonstrateVehicleInterfaces()
    {
        Console.WriteLine("\n--- Segregated Vehicle Interfaces ---");

        IDrivable car = new CarCorrect();
        car.Drive();
        Console.WriteLine($"  Max speed: {car.GetMaxSpeed()} mph");

        var plane = new Airplane();
        plane.Drive(); // Taxi
        plane.Fly();
        Console.WriteLine($"  Max altitude: {plane.GetMaxAltitude()} feet");

        var sub = new SubmarineCorrect();
        sub.Sail();
        sub.Submerge();
        if (sub is IWeaponized weaponized)
        {
            weaponized.LaunchMissiles();
        }

        Console.WriteLine("\n[CORRECT] Each vehicle exposes only its capabilities");
    }

    private static void DemonstrateDatabaseInterfaces()
    {
        Console.WriteLine("\n--- Segregated Database Interfaces ---");

        var sqlServer = new SqlServerDatabase();
        sqlServer.Create("Users", new { Name = "Alice" });
        if (sqlServer is ISqlDatabase sqlDb)
        {
            sqlDb.ExecuteStoredProcedure("sp_GetUsers");
        }

        var mongo = new MongoDatabaseCorrect();
        mongo.Create("users", new { Name = "Bob" });
        if (mongo is ISearchable searchable)
        {
            searchable.FullTextSearch("Bob");
        }

        var redis = new RedisDatabase();
        redis.Create("user:1", new { Name = "Charlie" });

        Console.WriteLine("\n[CORRECT] Each database exposes only supported operations");
        Console.WriteLine("  MongoDB doesn't have stored procedures - compiler prevents calling them!");
    }
}
