namespace SOLIDPrinciples.InterfaceSegregation;

/// <summary>
/// VIOLATION: Fat interfaces force clients to implement methods they don't need.
/// Problem: One-size-fits-all interfaces with too many methods.
/// Consequences:
/// - Forced to implement unused methods
/// - Empty or exception-throwing implementations
/// - Tight coupling to unnecessary dependencies
/// - Violates "clients shouldn't depend on methods they don't use"
/// </summary>

#region Worker Interface Problem

/// <summary>
/// FAT INTERFACE: Not all workers can eat, sleep, and work
/// </summary>
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
    void TakeBreak();
    void GetPaid(decimal amount);
}

/// <summary>
/// Human worker - can do everything
/// </summary>
public class HumanWorker : IWorker
{
    public string Name { get; set; } = string.Empty;

    public void Work()
    {
        Console.WriteLine($"[CORRECT] {Name} (Human) is working");
    }

    public void Eat()
    {
        Console.WriteLine($"[CORRECT] {Name} is eating lunch");
    }

    public void Sleep()
    {
        Console.WriteLine($"[CORRECT] {Name} is sleeping");
    }

    public void TakeBreak()
    {
        Console.WriteLine($"[CORRECT] {Name} is taking a coffee break");
    }

    public void GetPaid(decimal amount)
    {
        Console.WriteLine($"[CORRECT] {Name} received payment: ${amount}");
    }
}

/// <summary>
/// VIOLATION: Robot forced to implement Eat() and Sleep()
/// </summary>
public class RobotWorker : IWorker
{
    public string SerialNumber { get; set; } = string.Empty;

    public void Work()
    {
        Console.WriteLine($"[CORRECT] Robot {SerialNumber} is working");
    }

    public void Eat()
    {
        // VIOLATION: Robots don't eat!
        Console.WriteLine($"[VIOLATION] Robot {SerialNumber} cannot eat!");
        throw new NotSupportedException("Robots don't eat!");
    }

    public void Sleep()
    {
        // VIOLATION: Robots don't sleep!
        Console.WriteLine($"[VIOLATION] Robot {SerialNumber} cannot sleep!");
        throw new NotSupportedException("Robots don't sleep!");
    }

    public void TakeBreak()
    {
        // VIOLATION: Robots don't take breaks!
        Console.WriteLine($"[VIOLATION] Robot {SerialNumber} cannot take breaks!");
        throw new NotSupportedException("Robots don't take breaks!");
    }

    public void GetPaid(decimal amount)
    {
        // VIOLATION: Robots don't get paid!
        Console.WriteLine($"[VIOLATION] Robot {SerialNumber} doesn't get paid!");
        throw new NotSupportedException("Robots don't get paid!");
    }
}

/// <summary>
/// VIOLATION: Manager that tries to use any IWorker
/// </summary>
public class WorkerManager
{
    public static void ManageWorker(IWorker worker)
    {
        Console.WriteLine("\n[VIOLATION] Managing worker with fat interface:");

        try
        {
            worker.Work();
            worker.Eat();
            worker.TakeBreak();
            worker.Sleep();
            worker.GetPaid(1000);
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"  ERROR: {ex.Message}");
            Console.WriteLine("  ISP VIOLATED: Interface too fat for this implementation!");
        }
    }
}

#endregion

#region Printer Interface Problem

/// <summary>
/// FAT INTERFACE: Not all printers can do everything
/// </summary>
public interface IMultiFunctionPrinter
{
    void Print(string document);
    void Scan(string document);
    void Fax(string document);
    void Photocopy(string document);
    void Email(string document, string address);
    void CloudUpload(string document, string cloudService);
}

/// <summary>
/// High-end printer - can do everything
/// </summary>
public class AdvancedPrinter : IMultiFunctionPrinter
{
    public void Print(string document)
    {
        Console.WriteLine($"[CORRECT] Advanced printer: Printing {document}");
    }

    public void Scan(string document)
    {
        Console.WriteLine($"[CORRECT] Advanced printer: Scanning {document}");
    }

    public void Fax(string document)
    {
        Console.WriteLine($"[CORRECT] Advanced printer: Faxing {document}");
    }

    public void Photocopy(string document)
    {
        Console.WriteLine($"[CORRECT] Advanced printer: Photocopying {document}");
    }

    public void Email(string document, string address)
    {
        Console.WriteLine($"[CORRECT] Advanced printer: Emailing {document} to {address}");
    }

    public void CloudUpload(string document, string cloudService)
    {
        Console.WriteLine($"[CORRECT] Advanced printer: Uploading {document} to {cloudService}");
    }
}

/// <summary>
/// VIOLATION: Basic printer forced to implement advanced features
/// </summary>
public class BasicPrinter : IMultiFunctionPrinter
{
    public void Print(string document)
    {
        Console.WriteLine($"[CORRECT] Basic printer: Printing {document}");
    }

    public void Scan(string document)
    {
        // VIOLATION: Basic printer can't scan!
        Console.WriteLine($"[VIOLATION] Basic printer: Cannot scan!");
        throw new NotSupportedException("This printer doesn't support scanning!");
    }

    public void Fax(string document)
    {
        // VIOLATION: Basic printer can't fax!
        Console.WriteLine($"[VIOLATION] Basic printer: Cannot fax!");
        throw new NotSupportedException("This printer doesn't support faxing!");
    }

    public void Photocopy(string document)
    {
        // VIOLATION: Basic printer can't photocopy!
        Console.WriteLine($"[VIOLATION] Basic printer: Cannot photocopy!");
        throw new NotSupportedException("This printer doesn't support photocopying!");
    }

    public void Email(string document, string address)
    {
        // VIOLATION: Basic printer can't email!
        Console.WriteLine($"[VIOLATION] Basic printer: Cannot email!");
        throw new NotSupportedException("This printer doesn't support emailing!");
    }

    public void CloudUpload(string document, string cloudService)
    {
        // VIOLATION: Basic printer can't upload to cloud!
        Console.WriteLine($"[VIOLATION] Basic printer: Cannot upload to cloud!");
        throw new NotSupportedException("This printer doesn't support cloud upload!");
    }
}

#endregion

#region Vehicle Interface Problem

/// <summary>
/// FAT INTERFACE: Not all vehicles have all features
/// </summary>
public interface IVehicle
{
    void Drive();
    void Fly();
    void Sail();
    void Submerge();
    void LaunchMissiles();
    void TransformToRobot();
}

/// <summary>
/// VIOLATION: Regular car forced to implement flying, sailing, etc.
/// </summary>
public class Car : IVehicle
{
    public void Drive()
    {
        Console.WriteLine($"[CORRECT] Car: Driving on the road");
    }

    public void Fly()
    {
        Console.WriteLine($"[VIOLATION] Car: Cannot fly!");
        throw new NotSupportedException("Cars can't fly!");
    }

    public void Sail()
    {
        Console.WriteLine($"[VIOLATION] Car: Cannot sail!");
        throw new NotSupportedException("Cars can't sail!");
    }

    public void Submerge()
    {
        Console.WriteLine($"[VIOLATION] Car: Cannot submerge!");
        throw new NotSupportedException("Cars can't submerge!");
    }

    public void LaunchMissiles()
    {
        Console.WriteLine($"[VIOLATION] Car: Cannot launch missiles!");
        throw new NotSupportedException("Cars don't have missiles!");
    }

    public void TransformToRobot()
    {
        Console.WriteLine($"[VIOLATION] Car: Cannot transform!");
        throw new NotSupportedException("Cars can't transform to robots!");
    }
}

/// <summary>
/// VIOLATION: Submarine forced to implement flying and transforming
/// </summary>
public class Submarine : IVehicle
{
    public void Drive()
    {
        Console.WriteLine($"[VIOLATION] Submarine: Cannot drive on roads!");
        throw new NotSupportedException("Submarines don't drive on roads!");
    }

    public void Fly()
    {
        Console.WriteLine($"[VIOLATION] Submarine: Cannot fly!");
        throw new NotSupportedException("Submarines can't fly!");
    }

    public void Sail()
    {
        Console.WriteLine($"[CORRECT] Submarine: Sailing on the surface");
    }

    public void Submerge()
    {
        Console.WriteLine($"[CORRECT] Submarine: Submerging underwater");
    }

    public void LaunchMissiles()
    {
        Console.WriteLine($"[CORRECT] Submarine: Launching torpedoes");
    }

    public void TransformToRobot()
    {
        Console.WriteLine($"[VIOLATION] Submarine: Cannot transform!");
        throw new NotSupportedException("Submarines can't transform!");
    }
}

#endregion

#region Database Interface Problem

/// <summary>
/// FAT INTERFACE: Forces implementation of operations not all databases support
/// </summary>
public interface IDatabase
{
    // CRUD operations - all databases should support these
    void Create(string table, object data);
    void Read(string table, int id);
    void Update(string table, int id, object data);
    void Delete(string table, int id);

    // Advanced features - not all databases support these!
    void ExecuteStoredProcedure(string procedureName);
    void CreateIndex(string table, string column);
    void CreateView(string viewName, string query);
    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
}

/// <summary>
/// VIOLATION: NoSQL database forced to implement SQL-specific features
/// </summary>
public class MongoDatabase : IDatabase
{
    public void Create(string collection, object data)
    {
        Console.WriteLine($"[CORRECT] MongoDB: Inserting document into {collection}");
    }

    public void Read(string collection, int id)
    {
        Console.WriteLine($"[CORRECT] MongoDB: Reading document from {collection}");
    }

    public void Update(string collection, int id, object data)
    {
        Console.WriteLine($"[CORRECT] MongoDB: Updating document in {collection}");
    }

    public void Delete(string collection, int id)
    {
        Console.WriteLine($"[CORRECT] MongoDB: Deleting document from {collection}");
    }

    public void ExecuteStoredProcedure(string procedureName)
    {
        Console.WriteLine($"[VIOLATION] MongoDB: No stored procedures!");
        throw new NotSupportedException("MongoDB doesn't support stored procedures!");
    }

    public void CreateIndex(string collection, string field)
    {
        Console.WriteLine($"[CORRECT] MongoDB: Creating index on {collection}.{field}");
    }

    public void CreateView(string viewName, string query)
    {
        Console.WriteLine($"[VIOLATION] MongoDB: Limited view support!");
        throw new NotSupportedException("MongoDB has different view semantics!");
    }

    public void BeginTransaction()
    {
        Console.WriteLine($"[CORRECT] MongoDB: Starting transaction (4.0+)");
    }

    public void CommitTransaction()
    {
        Console.WriteLine($"[CORRECT] MongoDB: Committing transaction");
    }

    public void RollbackTransaction()
    {
        Console.WriteLine($"[CORRECT] MongoDB: Rolling back transaction");
    }
}

#endregion

/// <summary>
/// Demonstrates the problems with Interface Segregation Principle violations
/// </summary>
public class InterfaceSegregationViolationDemo
{
    public static void DemonstrateProblems()
    {
        Console.WriteLine("\n=== PROBLEMS WITH VIOLATING INTERFACE SEGREGATION PRINCIPLE ===");

        Console.WriteLine("\nProblem 1: Forced implementations");
        Console.WriteLine("  Classes must implement methods they don't support");
        Console.WriteLine("  Leads to exception-throwing dummy implementations");

        Console.WriteLine("\nProblem 2: Tight coupling");
        Console.WriteLine("  Clients depend on methods they don't use");
        Console.WriteLine("  Changes to unused methods still affect clients");

        Console.WriteLine("\nProblem 3: Confusion");
        Console.WriteLine("  Interface suggests capabilities that don't exist");
        Console.WriteLine("  Developers must read implementation to know what works");

        Console.WriteLine("\nProblem 4: Poor testability");
        Console.WriteLine("  Must mock methods that should never be called");
        Console.WriteLine("  Tests become more complex");

        DemonstrateWorkerProblem();
        DemonstratePrinterProblem();
        DemonstrateVehicleProblem();
    }

    private static void DemonstrateWorkerProblem()
    {
        Console.WriteLine("\n--- Worker Interface Problem ---");

        var human = new HumanWorker { Name = "John" };
        WorkerManager.ManageWorker(human);

        var robot = new RobotWorker { SerialNumber = "R2D2" };
        WorkerManager.ManageWorker(robot);  // BREAKS!
    }

    private static void DemonstratePrinterProblem()
    {
        Console.WriteLine("\n--- Printer Interface Problem ---");

        IMultiFunctionPrinter basic = new BasicPrinter();
        Console.WriteLine("\n[VIOLATION] Trying to use basic printer:");

        try
        {
            basic.Print("Document.pdf");
            basic.Scan("Document.pdf");  // BREAKS!
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"  ERROR: {ex.Message}");
        }
    }

    private static void DemonstrateVehicleProblem()
    {
        Console.WriteLine("\n--- Vehicle Interface Problem ---");

        IVehicle car = new Car();
        Console.WriteLine("\n[VIOLATION] Trying to use car:");

        try
        {
            car.Drive();
            car.Fly();  // BREAKS!
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"  ERROR: {ex.Message}");
        }
    }
}
