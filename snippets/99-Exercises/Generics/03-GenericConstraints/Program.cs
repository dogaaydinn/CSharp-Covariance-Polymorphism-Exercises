namespace GenericConstraints;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Generic Constraints Exercise");
        Console.WriteLine("Run 'dotnet test' to check your solutions\n");
    }

    // TODO 1: Repository with class + new() constraints
    // where T : class - T must be a reference type
    // where T : new() - T must have a parameterless constructor
    public class Repository<T> where T : class, new()
    {
        private readonly List<T> _items = new();

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public T Create()
        {
            // Use new() constraint to create instance
            throw new NotImplementedException();
        }

        public List<T> GetAll()
        {
            throw new NotImplementedException();
        }
    }

    // TODO 2: Processor with struct constraint
    // where T : struct - T must be a value type
    public class ValueTypeProcessor<T> where T : struct
    {
        public T GetDefault()
        {
            // For value types, default(T) returns zero/false/etc
            throw new NotImplementedException();
        }

        public bool IsDefault(T value)
        {
            throw new NotImplementedException();
        }
    }

    // TODO 3: Repository with base class constraint
    // where T : Entity - T must inherit from Entity
    public class EntityRepository<T> where T : Entity, new()
    {
        private readonly List<T> _entities = new();

        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public T? FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<T> GetRecent(int count)
        {
            throw new NotImplementedException();
        }
    }

    // TODO 4: Sorted list with interface constraint
    // where T : IComparable<T> - T must implement IComparable<T>
    public class SortedList<T> where T : IComparable<T>
    {
        private readonly List<T> _items = new();

        public void Add(T item)
        {
            // Use IComparable<T> to maintain sorted order
            throw new NotImplementedException();
        }

        public T? GetMin()
        {
            throw new NotImplementedException();
        }

        public T? GetMax()
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll() => _items;
    }

    // TODO 5: Multiple constraints with IDisposable
    // where T : class, IDisposable, new()
    public class DisposableManager<T> where T : class, IDisposable, new()
    {
        private readonly List<T> _resources = new();

        public T CreateAndTrack()
        {
            throw new NotImplementedException();
        }

        public void DisposeAll()
        {
            throw new NotImplementedException();
        }

        public int Count => _resources.Count;
    }

    // TODO 6: Generic factory with new() constraint
    public static T CreateInstance<T>() where T : new()
    {
        // Create and return new instance using new() constraint
        throw new NotImplementedException();
    }
}
