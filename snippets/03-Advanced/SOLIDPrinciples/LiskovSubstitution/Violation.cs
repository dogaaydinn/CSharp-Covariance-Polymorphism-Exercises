namespace SOLIDPrinciples.LiskovSubstitution;

/// <summary>
/// VIOLATION: Subtype cannot be substituted for base type.
/// Problem: Square violates Rectangle's contract.
/// Consequences:
/// - Unexpected behavior when using base class reference
/// - Breaks client code expectations
/// - Can't use polymorphism safely
/// - Violates "is-a" relationship
/// </summary>

#region Classic Rectangle/Square Problem

/// <summary>
/// Base class: Mutable Rectangle
/// Contract: Width and Height can be set independently
/// </summary>
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }

    public int CalculateArea()
    {
        return Width * Height;
    }

    public override string ToString()
    {
        return $"Rectangle({Width}x{Height})";
    }
}

/// <summary>
/// VIOLATION: Square breaks Rectangle's contract
/// Problem: Setting Width affects Height (and vice versa)
/// </summary>
public class Square : Rectangle
{
    private int _side;

    public override int Width
    {
        get => _side;
        set => _side = value;  // VIOLATION: Also changes Height!
    }

    public override int Height
    {
        get => _side;
        set => _side = value;  // VIOLATION: Also changes Width!
    }

    public override string ToString()
    {
        return $"Square({_side}x{_side})";
    }
}

/// <summary>
/// Client code that expects Rectangle contract to work
/// </summary>
public class RectangleTester
{
    public static void TestRectangle(Rectangle rect)
    {
        Console.WriteLine($"\n[VIOLATION] Testing {rect}");

        // Client expects: Setting width and height independently
        rect.Width = 5;
        rect.Height = 10;

        Console.WriteLine($"  After setting Width=5, Height=10:");
        Console.WriteLine($"  Width: {rect.Width}, Height: {rect.Height}");
        Console.WriteLine($"  Expected Area: 50");
        Console.WriteLine($"  Actual Area: {rect.CalculateArea()}");

        if (rect.CalculateArea() != 50)
        {
            Console.WriteLine("  ERROR: Area is wrong! LSP VIOLATED!");
        }
        else
        {
            Console.WriteLine("  SUCCESS: Area is correct");
        }
    }
}

#endregion

#region Bird Hierarchy Problem

/// <summary>
/// Base class: All birds can fly (wrong assumption!)
/// </summary>
public abstract class Bird
{
    public string Name { get; set; } = string.Empty;

    public abstract void Fly();
}

/// <summary>
/// Normal flying bird - works fine
/// </summary>
public class Sparrow : Bird
{
    public override void Fly()
    {
        Console.WriteLine($"[CORRECT] {Name} (Sparrow) is flying gracefully");
    }
}

/// <summary>
/// VIOLATION: Penguin can't fly but is forced to implement Fly()
/// </summary>
public class Penguin : Bird
{
    public override void Fly()
    {
        // Penguins can't fly!
        Console.WriteLine($"[VIOLATION] {Name} (Penguin) cannot fly!");
        throw new NotSupportedException("Penguins cannot fly!");
    }
}

/// <summary>
/// VIOLATION: Ostrich can't fly but is forced to implement Fly()
/// </summary>
public class Ostrich : Bird
{
    public override void Fly()
    {
        // Ostriches can't fly!
        Console.WriteLine($"[VIOLATION] {Name} (Ostrich) cannot fly!");
        throw new NotSupportedException("Ostriches cannot fly!");
    }
}

/// <summary>
/// Client code that expects all birds to fly
/// </summary>
public class BirdSanctuary
{
    public static void MakeBirdsFly(List<Bird> birds)
    {
        Console.WriteLine("\n[VIOLATION] Making all birds fly:");

        foreach (var bird in birds)
        {
            try
            {
                bird.Fly();
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"  ERROR: {ex.Message}");
                Console.WriteLine("  LSP VIOLATED: Cannot substitute Penguin/Ostrich for Bird!");
            }
        }
    }
}

#endregion

#region Account Hierarchy Problem

/// <summary>
/// Base class: Bank account with withdrawal capability
/// </summary>
public class BankAccount
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; protected set; }

    public virtual void Deposit(decimal amount)
    {
        Balance += amount;
        Console.WriteLine($"  Deposited ${amount}. New balance: ${Balance}");
    }

    public virtual void Withdraw(decimal amount)
    {
        if (Balance >= amount)
        {
            Balance -= amount;
            Console.WriteLine($"  Withdrew ${amount}. New balance: ${Balance}");
        }
        else
        {
            Console.WriteLine($"  ERROR: Insufficient funds!");
        }
    }
}

/// <summary>
/// VIOLATION: Fixed deposit account breaks withdrawal contract
/// </summary>
public class FixedDepositAccount : BankAccount
{
    public DateTime MaturityDate { get; set; }

    public override void Withdraw(decimal amount)
    {
        // VIOLATION: Cannot withdraw before maturity
        if (DateTime.Now < MaturityDate)
        {
            Console.WriteLine($"  [VIOLATION] Cannot withdraw before maturity date!");
            throw new InvalidOperationException("Cannot withdraw from fixed deposit before maturity");
        }

        base.Withdraw(amount);
    }
}

/// <summary>
/// Client code that expects to withdraw from any account
/// </summary>
public class BankingSystem
{
    public static void ProcessWithdrawal(BankAccount account, decimal amount)
    {
        Console.WriteLine($"\n[VIOLATION] Processing withdrawal from {account.AccountNumber}");

        try
        {
            account.Withdraw(amount);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  ERROR: {ex.Message}");
            Console.WriteLine("  LSP VIOLATED: Cannot substitute FixedDepositAccount for BankAccount!");
        }
    }
}

#endregion

#region Collection Hierarchy Problem

/// <summary>
/// Base class: Mutable collection
/// </summary>
public class MutableList<T>
{
    protected List<T> Items { get; } = new();

    public virtual void Add(T item)
    {
        Items.Add(item);
        Console.WriteLine($"  Added: {item}");
    }

    public virtual void Remove(T item)
    {
        Items.Remove(item);
        Console.WriteLine($"  Removed: {item}");
    }

    public int Count => Items.Count;

    public IEnumerable<T> GetItems() => Items;
}

/// <summary>
/// VIOLATION: Immutable list breaks mutable contract
/// </summary>
public class ImmutableList<T> : MutableList<T>
{
    public override void Add(T item)
    {
        Console.WriteLine($"  [VIOLATION] Cannot add to immutable list!");
        throw new NotSupportedException("Cannot modify immutable list");
    }

    public override void Remove(T item)
    {
        Console.WriteLine($"  [VIOLATION] Cannot remove from immutable list!");
        throw new NotSupportedException("Cannot modify immutable list");
    }
}

#endregion

/// <summary>
/// Demonstrates the problems with Liskov Substitution Principle violations
/// </summary>
public class LiskovViolationDemo
{
    public static void DemonstrateProblems()
    {
        Console.WriteLine("\n=== PROBLEMS WITH VIOLATING LISKOV SUBSTITUTION PRINCIPLE ===");

        Console.WriteLine("\nProblem 1: Unexpected behavior with polymorphism");
        Console.WriteLine("  Client expects Rectangle behavior");
        Console.WriteLine("  Square breaks that expectation");

        Console.WriteLine("\nProblem 2: Exceptions in derived classes");
        Console.WriteLine("  Base class defines an operation");
        Console.WriteLine("  Derived class throws exception instead");

        Console.WriteLine("\nProblem 3: Strengthened preconditions");
        Console.WriteLine("  Base class accepts any condition");
        Console.WriteLine("  Derived class requires more restrictive conditions");

        Console.WriteLine("\nProblem 4: Weakened postconditions");
        Console.WriteLine("  Base class guarantees a result");
        Console.WriteLine("  Derived class doesn't guarantee the same result");

        DemonstrateRectangleSquareProblem();
        DemonstrateBirdProblem();
        DemonstrateBankAccountProblem();
        DemonstrateCollectionProblem();
    }

    private static void DemonstrateRectangleSquareProblem()
    {
        Console.WriteLine("\n--- Rectangle/Square Problem ---");

        var rect = new Rectangle();
        RectangleTester.TestRectangle(rect);

        var square = new Square();
        RectangleTester.TestRectangle(square);  // BREAKS!
    }

    private static void DemonstrateBirdProblem()
    {
        Console.WriteLine("\n--- Bird Hierarchy Problem ---");

        var birds = new List<Bird>
        {
            new Sparrow { Name = "Tweety" },
            new Penguin { Name = "Pingu" },
            new Ostrich { Name = "Oscar" }
        };

        BirdSanctuary.MakeBirdsFly(birds);  // BREAKS!
    }

    private static void DemonstrateBankAccountProblem()
    {
        Console.WriteLine("\n--- Bank Account Problem ---");

        var regularAccount = new BankAccount { AccountNumber = "REG-001" };
        regularAccount.Deposit(1000);
        BankingSystem.ProcessWithdrawal(regularAccount, 500);

        var fixedAccount = new FixedDepositAccount
        {
            AccountNumber = "FD-001",
            MaturityDate = DateTime.Now.AddDays(30)
        };
        fixedAccount.Deposit(5000);
        BankingSystem.ProcessWithdrawal(fixedAccount, 500);  // BREAKS!
    }

    private static void DemonstrateCollectionProblem()
    {
        Console.WriteLine("\n--- Collection Problem ---");

        MutableList<string> mutable = new MutableList<string>();
        mutable.Add("Item 1");
        mutable.Add("Item 2");
        Console.WriteLine($"  Mutable list count: {mutable.Count}");

        Console.WriteLine("\nTrying with immutable list:");
        MutableList<string> immutable = new ImmutableList<string>();
        try
        {
            immutable.Add("Item 1");  // BREAKS!
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"  ERROR: {ex.Message}");
            Console.WriteLine("  LSP VIOLATED: Cannot substitute ImmutableList for MutableList!");
        }
    }
}
