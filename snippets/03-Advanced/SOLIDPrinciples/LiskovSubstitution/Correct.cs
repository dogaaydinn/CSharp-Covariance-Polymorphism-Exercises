namespace SOLIDPrinciples.LiskovSubstitution;

/// <summary>
/// CORRECT: Subtypes are fully substitutable for their base types.
/// Benefits:
/// - Polymorphism works as expected
/// - No surprises when using base class references
/// - Derived classes honor base class contracts
/// - Safe to substitute child for parent
/// </summary>

#region Correct Shape Hierarchy

/// <summary>
/// Base abstraction: All shapes have area
/// No mutable state that could cause LSP violations
/// </summary>
public abstract class Shape
{
    public abstract int CalculateArea();
    public abstract string GetDescription();
}

/// <summary>
/// Rectangle with immutable dimensions
/// </summary>
public class ImmutableRectangle : Shape
{
    public int Width { get; }
    public int Height { get; }

    public ImmutableRectangle(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public override int CalculateArea()
    {
        return Width * Height;
    }

    public override string GetDescription()
    {
        return $"Rectangle({Width}x{Height})";
    }
}

/// <summary>
/// Square with immutable dimensions
/// Doesn't violate any parent contracts because it doesn't inherit from Rectangle!
/// </summary>
public class ImmutableSquare : Shape
{
    public int Side { get; }

    public ImmutableSquare(int side)
    {
        Side = side;
    }

    public override int CalculateArea()
    {
        return Side * Side;
    }

    public override string GetDescription()
    {
        return $"Square({Side}x{Side})";
    }
}

/// <summary>
/// Client code that works with any shape
/// </summary>
public class ShapeCalculator
{
    public static void DisplayShapeInfo(Shape shape)
    {
        Console.WriteLine($"\n[CORRECT] Analyzing {shape.GetDescription()}");
        Console.WriteLine($"  Area: {shape.CalculateArea()}");
        Console.WriteLine("  SUCCESS: All shapes work correctly!");
    }
}

#endregion

#region Correct Bird Hierarchy

/// <summary>
/// Base abstraction: Common bird behaviors
/// Only includes operations ALL birds can perform
/// </summary>
public abstract class BirdBase
{
    public string Name { get; set; } = string.Empty;

    public void Eat()
    {
        Console.WriteLine($"  {Name} is eating");
    }

    public abstract void Move();
}

/// <summary>
/// Interface for birds that can fly
/// Only flying birds implement this
/// </summary>
public interface IFlyable
{
    void Fly();
    int GetMaxAltitude();
}

/// <summary>
/// Interface for birds that can swim
/// Only swimming birds implement this
/// </summary>
public interface ISwimmable
{
    void Swim();
    int GetMaxDepth();
}

/// <summary>
/// Flying bird implementation
/// </summary>
public class Eagle : BirdBase, IFlyable
{
    public override void Move()
    {
        Console.WriteLine($"[CORRECT] {Name} (Eagle) moves by flying");
        Fly();
    }

    public void Fly()
    {
        Console.WriteLine($"  {Name} soars through the sky at high altitude");
    }

    public int GetMaxAltitude()
    {
        return 10000; // feet
    }
}

/// <summary>
/// Swimming bird implementation
/// Penguins don't fly, so they don't implement IFlyable!
/// </summary>
public class PenguinCorrect : BirdBase, ISwimmable
{
    public override void Move()
    {
        Console.WriteLine($"[CORRECT] {Name} (Penguin) moves by swimming and walking");
        Swim();
    }

    public void Swim()
    {
        Console.WriteLine($"  {Name} swims gracefully underwater");
    }

    public int GetMaxDepth()
    {
        return 500; // feet
    }
}

/// <summary>
/// Bird that both flies and swims
/// </summary>
public class Duck : BirdBase, IFlyable, ISwimmable
{
    public override void Move()
    {
        Console.WriteLine($"[CORRECT] {Name} (Duck) can fly and swim");
    }

    public void Fly()
    {
        Console.WriteLine($"  {Name} flies to the next pond");
    }

    public void Swim()
    {
        Console.WriteLine($"  {Name} paddles in the water");
    }

    public int GetMaxAltitude()
    {
        return 4000; // feet
    }

    public int GetMaxDepth()
    {
        return 20; // feet
    }
}

/// <summary>
/// Client code that respects each bird's capabilities
/// </summary>
public class BirdSanctuaryCorrect
{
    public static void MakeBirdsMove(List<BirdBase> birds)
    {
        Console.WriteLine("\n[CORRECT] Making birds move:");

        foreach (var bird in birds)
        {
            bird.Move();

            // Only call Fly() on birds that can fly
            if (bird is IFlyable flyableBird)
            {
                Console.WriteLine($"  Max altitude: {flyableBird.GetMaxAltitude()} feet");
            }

            // Only call Swim() on birds that can swim
            if (bird is ISwimmable swimmableBird)
            {
                Console.WriteLine($"  Max depth: {swimmableBird.GetMaxDepth()} feet");
            }

            Console.WriteLine();
        }
    }
}

#endregion

#region Correct Account Hierarchy

/// <summary>
/// Base interface: Common account operations
/// </summary>
public interface IAccount
{
    string AccountNumber { get; }
    decimal Balance { get; }
    void Deposit(decimal amount);
}

/// <summary>
/// Interface for accounts that allow withdrawals
/// Only accounts with withdrawal capability implement this
/// </summary>
public interface IWithdrawable
{
    void Withdraw(decimal amount);
    bool CanWithdraw(decimal amount);
}

/// <summary>
/// Regular checking account - supports withdrawals
/// </summary>
public class CheckingAccount : IAccount, IWithdrawable
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; private set; }

    public void Deposit(decimal amount)
    {
        Balance += amount;
        Console.WriteLine($"  [CORRECT] Deposited ${amount}. New balance: ${Balance}");
    }

    public bool CanWithdraw(decimal amount)
    {
        return Balance >= amount;
    }

    public void Withdraw(decimal amount)
    {
        if (CanWithdraw(amount))
        {
            Balance -= amount;
            Console.WriteLine($"  [CORRECT] Withdrew ${amount}. New balance: ${Balance}");
        }
        else
        {
            Console.WriteLine($"  [CORRECT] Insufficient funds for withdrawal");
        }
    }
}

/// <summary>
/// Savings account with withdrawal limits
/// </summary>
public class SavingsAccount : IAccount, IWithdrawable
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; private set; }
    private int _withdrawalsThisMonth = 0;
    private const int MaxMonthlyWithdrawals = 6;

    public void Deposit(decimal amount)
    {
        Balance += amount;
        Console.WriteLine($"  [CORRECT] Deposited ${amount}. New balance: ${Balance}");
    }

    public bool CanWithdraw(decimal amount)
    {
        return Balance >= amount && _withdrawalsThisMonth < MaxMonthlyWithdrawals;
    }

    public void Withdraw(decimal amount)
    {
        if (!CanWithdraw(amount))
        {
            if (_withdrawalsThisMonth >= MaxMonthlyWithdrawals)
            {
                Console.WriteLine($"  [CORRECT] Monthly withdrawal limit reached");
            }
            else
            {
                Console.WriteLine($"  [CORRECT] Insufficient funds");
            }
            return;
        }

        Balance -= amount;
        _withdrawalsThisMonth++;
        Console.WriteLine($"  [CORRECT] Withdrew ${amount}. New balance: ${Balance}");
        Console.WriteLine($"  Withdrawals this month: {_withdrawalsThisMonth}/{MaxMonthlyWithdrawals}");
    }
}

/// <summary>
/// Fixed deposit - doesn't allow withdrawals until maturity
/// Doesn't implement IWithdrawable!
/// </summary>
public class FixedDepositAccountCorrect : IAccount
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; private set; }
    public DateTime MaturityDate { get; set; }

    public void Deposit(decimal amount)
    {
        Balance += amount;
        Console.WriteLine($"  [CORRECT] Deposited ${amount}. Maturity: {MaturityDate:yyyy-MM-dd}");
    }

    public bool CanWithdrawOnMaturity()
    {
        return DateTime.Now >= MaturityDate;
    }

    public void WithdrawOnMaturity()
    {
        if (CanWithdrawOnMaturity())
        {
            Console.WriteLine($"  [CORRECT] Withdrawing ${Balance} on maturity");
            Balance = 0;
        }
        else
        {
            Console.WriteLine($"  [CORRECT] Cannot withdraw before {MaturityDate:yyyy-MM-dd}");
        }
    }
}

/// <summary>
/// Banking system that respects account capabilities
/// </summary>
public class BankingSystemCorrect
{
    public static void ProcessDeposit(IAccount account, decimal amount)
    {
        Console.WriteLine($"\n[CORRECT] Processing deposit to {account.AccountNumber}");
        account.Deposit(amount);
    }

    public static void ProcessWithdrawal(IWithdrawable account, decimal amount)
    {
        Console.WriteLine($"\n[CORRECT] Processing withdrawal");

        if (account.CanWithdraw(amount))
        {
            account.Withdraw(amount);
        }
        else
        {
            Console.WriteLine("  [CORRECT] Withdrawal not allowed");
        }
    }
}

#endregion

#region Correct Collection Hierarchy

/// <summary>
/// Base interface: Read-only collection operations
/// </summary>
public interface IReadOnlyCollection<T>
{
    int Count { get; }
    IEnumerable<T> GetItems();
    bool Contains(T item);
}

/// <summary>
/// Interface for collections that can be modified
/// </summary>
public interface IModifiableCollection<T> : IReadOnlyCollection<T>
{
    void Add(T item);
    void Remove(T item);
    void Clear();
}

/// <summary>
/// Immutable collection - only implements read-only operations
/// </summary>
public class ImmutableCollection<T> : IReadOnlyCollection<T>
{
    private readonly List<T> _items;

    public ImmutableCollection(IEnumerable<T> items)
    {
        _items = new List<T>(items);
    }

    public int Count => _items.Count;

    public IEnumerable<T> GetItems() => _items.AsReadOnly();

    public bool Contains(T item) => _items.Contains(item);
}

/// <summary>
/// Mutable collection - implements all operations
/// </summary>
public class MutableCollection<T> : IModifiableCollection<T>
{
    private readonly List<T> _items = new();

    public int Count => _items.Count;

    public IEnumerable<T> GetItems() => _items;

    public bool Contains(T item) => _items.Contains(item);

    public void Add(T item)
    {
        _items.Add(item);
        Console.WriteLine($"  [CORRECT] Added: {item}");
    }

    public void Remove(T item)
    {
        _items.Remove(item);
        Console.WriteLine($"  [CORRECT] Removed: {item}");
    }

    public void Clear()
    {
        _items.Clear();
        Console.WriteLine($"  [CORRECT] Cleared all items");
    }
}

#endregion

/// <summary>
/// Demonstrates the benefits of Liskov Substitution Principle
/// </summary>
public class LiskovCorrectDemo
{
    public static void DemonstrateBenefits()
    {
        Console.WriteLine("\n=== BENEFITS OF LISKOV SUBSTITUTION PRINCIPLE ===");

        Console.WriteLine("\nBenefit 1: Polymorphism works reliably");
        Console.WriteLine("  Any subtype can be used where base type is expected");
        Console.WriteLine("  No unexpected exceptions or behaviors");

        Console.WriteLine("\nBenefit 2: Interface segregation helps LSP");
        Console.WriteLine("  Objects only expose capabilities they truly have");
        Console.WriteLine("  No forced implementations");

        Console.WriteLine("\nBenefit 3: Stronger type safety");
        Console.WriteLine("  Compiler helps prevent LSP violations");
        Console.WriteLine("  Fewer runtime errors");

        Console.WriteLine("\nBenefit 4: Easier to reason about code");
        Console.WriteLine("  Clear contracts and expectations");
        Console.WriteLine("  Predictable behavior");

        DemonstrateShapeHierarchy();
        DemonstrateBirdHierarchy();
        DemonstrateAccountHierarchy();
        DemonstrateCollectionHierarchy();
    }

    private static void DemonstrateShapeHierarchy()
    {
        Console.WriteLine("\n--- Shape Hierarchy (Correct) ---");

        var shapes = new List<Shape>
        {
            new ImmutableRectangle(5, 10),
            new ImmutableSquare(7)
        };

        foreach (var shape in shapes)
        {
            ShapeCalculator.DisplayShapeInfo(shape);
        }
    }

    private static void DemonstrateBirdHierarchy()
    {
        Console.WriteLine("\n--- Bird Hierarchy (Correct) ---");

        var birds = new List<BirdBase>
        {
            new Eagle { Name = "Eddie" },
            new PenguinCorrect { Name = "Penny" },
            new Duck { Name = "Donald" }
        };

        BirdSanctuaryCorrect.MakeBirdsMove(birds);
    }

    private static void DemonstrateAccountHierarchy()
    {
        Console.WriteLine("\n--- Account Hierarchy (Correct) ---");

        var checking = new CheckingAccount { AccountNumber = "CHK-001" };
        BankingSystemCorrect.ProcessDeposit(checking, 1000);
        BankingSystemCorrect.ProcessWithdrawal(checking, 500);

        var savings = new SavingsAccount { AccountNumber = "SAV-001" };
        BankingSystemCorrect.ProcessDeposit(savings, 5000);
        BankingSystemCorrect.ProcessWithdrawal(savings, 200);

        var fixedDeposit = new FixedDepositAccountCorrect
        {
            AccountNumber = "FD-001",
            MaturityDate = DateTime.Now.AddDays(30)
        };
        BankingSystemCorrect.ProcessDeposit(fixedDeposit, 10000);
        Console.WriteLine("\n[CORRECT] Fixed deposit doesn't implement IWithdrawable");
        Console.WriteLine("  Compiler prevents calling ProcessWithdrawal!");
    }

    private static void DemonstrateCollectionHierarchy()
    {
        Console.WriteLine("\n--- Collection Hierarchy (Correct) ---");

        // Read-only collection
        var immutable = new ImmutableCollection<string>(new[] { "Item1", "Item2", "Item3" });
        Console.WriteLine($"\n[CORRECT] Immutable collection count: {immutable.Count}");
        Console.WriteLine($"  Contains 'Item1': {immutable.Contains("Item1")}");

        // Modifiable collection
        var mutable = new MutableCollection<string>();
        mutable.Add("Item1");
        mutable.Add("Item2");
        Console.WriteLine($"\n[CORRECT] Mutable collection count: {mutable.Count}");

        Console.WriteLine("\n[CORRECT] Compiler prevents calling Add() on immutable collection!");
        Console.WriteLine("  Type safety enforces LSP at compile time");
    }
}
