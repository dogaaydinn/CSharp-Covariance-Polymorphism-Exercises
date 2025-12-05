namespace IndexerExample;

// ═══════════════════════════════════════════════════════════
// ❌ BAD PRACTICE: GetItem/SetItem Methods
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ❌ BAD PRACTICE: Indexer yerine method kullanımı
/// Problem: Verbose syntax, array-like syntax yok
/// </summary>
public class BadCollection<T>
{
    private readonly List<T> _items = new();

    // ❌ BAD: Method ile erişim - verbose!
    public T GetItem(int index)
    {
        if (index < 0 || index >= _items.Count)
            throw new IndexOutOfRangeException();
        return _items[index];
    }

    // ❌ BAD: Method ile set - verbose!
    public void SetItem(int index, T value)
    {
        if (index < 0 || index >= _items.Count)
            throw new IndexOutOfRangeException();
        _items[index] = value;
    }

    public void Add(T item) => _items.Add(item);
    public int Count => _items.Count;
}

// ═══════════════════════════════════════════════════════════
// ✅ GOOD PRACTICE: Integer Indexer (Array-like)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ GOOD: Integer indexer ile array-like erişim
/// Use case: Custom collection, array wrapper
/// </summary>
public class SmartArray<T>
{
    private readonly List<T> _items = new();

    // Integer indexer - Array-like access
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _items.Count)
                throw new IndexOutOfRangeException($"Index {index} out of range [0-{_items.Count - 1}]");
            return _items[index];
        }
        set
        {
            if (index < 0 || index >= _items.Count)
                throw new IndexOutOfRangeException($"Index {index} out of range [0-{_items.Count - 1}]");
            _items[index] = value;
            Console.WriteLine($"  ✅ SmartArray[{index}] = {value}");
        }
    }

    // Range indexer (C# 8+) - Slice access
    public List<T> this[Range range]
    {
        get
        {
            var (start, length) = range.GetOffsetAndLength(_items.Count);
            Console.WriteLine($"  📋 Slice [{range}]: start={start}, length={length}");
            return _items.GetRange(start, length);
        }
    }

    // Index indexer (C# 8+) - From end access
    public T this[Index index]
    {
        get
        {
            int actualIndex = index.GetOffset(_items.Count);
            Console.WriteLine($"  🔍 Index [{index}]: actualIndex={actualIndex}");
            return _items[actualIndex];
        }
        set
        {
            int actualIndex = index.GetOffset(_items.Count);
            _items[actualIndex] = value;
            Console.WriteLine($"  ✅ SmartArray[{index}] = {value} (actualIndex={actualIndex})");
        }
    }

    public void Add(T item) => _items.Add(item);
    public int Count => _items.Count;

    public void Display()
    {
        Console.WriteLine($"  SmartArray[{Count} items]: [{string.Join(", ", _items)}]");
    }
}

// ═══════════════════════════════════════════════════════════
// ✅ GOOD PRACTICE: String Indexer (Dictionary-like)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ GOOD: String indexer ile dictionary-like erişim
/// Use case: Key-value collections, named access
/// </summary>
public class StudentGrades
{
    private readonly Dictionary<string, int> _grades = new();

    // String indexer - Dictionary-like access
    public int this[string studentName]
    {
        get
        {
            if (_grades.TryGetValue(studentName, out var grade))
            {
                return grade;
            }

            Console.WriteLine($"  ⚠️  Student '{studentName}' not found, returning 0");
            return 0;  // Default value
        }
        set
        {
            // Validation
            if (value < 0 || value > 100)
                throw new ArgumentException("Grade must be between 0-100!", nameof(value));

            bool isUpdate = _grades.ContainsKey(studentName);
            _grades[studentName] = value;

            if (isUpdate)
                Console.WriteLine($"  ✏️  Updated: {studentName} = {value}");
            else
                Console.WriteLine($"  ✅ Added: {studentName} = {value}");
        }
    }

    public IEnumerable<string> Students => _grades.Keys;
    public int Count => _grades.Count;

    public void DisplayAll()
    {
        Console.WriteLine($"\n  📊 Student Grades ({Count} students):");
        foreach (var student in Students.OrderBy(s => s))
        {
            Console.WriteLine($"     • {student}: {_grades[student]}");
        }
    }
}

// ═══════════════════════════════════════════════════════════
// ✅ ADVANCED: Dual Indexer (int + string)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ ADVANCED: Hem int hem string indexer ile erişim
/// Use case: Flexible access patterns (by index or by name)
/// </summary>
public class ProductCatalog
{
    private readonly List<Product> _products = new();
    private readonly Dictionary<string, Product> _productsByCode = new();

    // Integer indexer - Position-based access
    public Product this[int index]
    {
        get
        {
            if (index < 0 || index >= _products.Count)
                throw new IndexOutOfRangeException($"Index {index} out of range [0-{_products.Count - 1}]");
            return _products[index];
        }
    }

    // String indexer - Code-based access
    public Product this[string code]
    {
        get
        {
            if (_productsByCode.TryGetValue(code, out var product))
                return product;

            throw new KeyNotFoundException($"Product with code '{code}' not found!");
        }
        set
        {
            if (_productsByCode.ContainsKey(code))
            {
                // Update existing product
                var existingProduct = _productsByCode[code];
                int index = _products.IndexOf(existingProduct);
                _products[index] = value;
                _productsByCode[code] = value;
                Console.WriteLine($"  ✏️  Updated product: {code}");
            }
            else
            {
                // Add new product
                _products.Add(value);
                _productsByCode[code] = value;
                Console.WriteLine($"  ✅ Added product: {code}");
            }
        }
    }

    public int Count => _products.Count;

    public void DisplayAll()
    {
        Console.WriteLine($"\n  📦 Product Catalog ({Count} products):");
        for (int i = 0; i < Count; i++)
        {
            var p = _products[i];
            Console.WriteLine($"     [{i}] {p.Code}: {p.Name} - {p.Price:C}");
        }
    }
}

public class Product
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public decimal Price { get; init; }
}

// ═══════════════════════════════════════════════════════════
// ✅ ADVANCED: Multi-Dimensional Indexer (2D, 3D)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ GOOD: 2D indexer ile matrix-like erişim
/// Use case: Matrix, grid, table data
/// </summary>
public class Matrix
{
    private readonly int[,] _data;
    public int Rows { get; }
    public int Cols { get; }

    public Matrix(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        _data = new int[rows, cols];
    }

    // 2D indexer - Multi-parameter
    public int this[int row, int col]
    {
        get
        {
            ValidateIndices(row, col);
            return _data[row, col];
        }
        set
        {
            ValidateIndices(row, col);
            _data[row, col] = value;
            Console.WriteLine($"  ✅ Matrix[{row},{col}] = {value}");
        }
    }

    private void ValidateIndices(int row, int col)
    {
        if (row < 0 || row >= Rows)
            throw new IndexOutOfRangeException($"Row {row} out of range [0-{Rows - 1}]");
        if (col < 0 || col >= Cols)
            throw new IndexOutOfRangeException($"Col {col} out of range [0-{Cols - 1}]");
    }

    public void Display()
    {
        Console.WriteLine($"\n  📐 Matrix ({Rows}×{Cols}):");
        for (int i = 0; i < Rows; i++)
        {
            Console.Write("     ");
            for (int j = 0; j < Cols; j++)
            {
                Console.Write($"{_data[i, j],4}");
            }
            Console.WriteLine();
        }
    }

    // Fill with pattern
    public void Fill(Func<int, int, int> pattern)
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                _data[i, j] = pattern(i, j);
            }
        }
    }
}

/// <summary>
/// ✅ ADVANCED: 3D indexer
/// Use case: 3D data structures, voxel grids, cubes
/// </summary>
public class Cube<T>
{
    private readonly T[,,] _data;
    public int Width { get; }
    public int Height { get; }
    public int Depth { get; }

    public Cube(int width, int height, int depth)
    {
        Width = width;
        Height = height;
        Depth = depth;
        _data = new T[width, height, depth];
    }

    // 3D indexer - Three parameters
    public T this[int x, int y, int z]
    {
        get
        {
            ValidateIndices(x, y, z);
            return _data[x, y, z];
        }
        set
        {
            ValidateIndices(x, y, z);
            _data[x, y, z] = value;
            Console.WriteLine($"  ✅ Cube[{x},{y},{z}] = {value}");
        }
    }

    private void ValidateIndices(int x, int y, int z)
    {
        if (x < 0 || x >= Width)
            throw new IndexOutOfRangeException($"X {x} out of range [0-{Width - 1}]");
        if (y < 0 || y >= Height)
            throw new IndexOutOfRangeException($"Y {y} out of range [0-{Height - 1}]");
        if (z < 0 || z >= Depth)
            throw new IndexOutOfRangeException($"Z {z} out of range [0-{Depth - 1}]");
    }
}

// ═══════════════════════════════════════════════════════════
// ✅ REAL-WORLD: Sparse Matrix (Memory Efficient)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ PRODUCTION: Sparse matrix with dictionary backing (memory efficient)
/// Use case: Large matrices with many zero/default values
/// </summary>
public class SparseMatrix
{
    private readonly Dictionary<string, int> _data = new();
    public int Rows { get; }
    public int Cols { get; }

    public SparseMatrix(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
    }

    // 2D indexer with sparse storage
    public int this[int row, int col]
    {
        get
        {
            ValidateIndices(row, col);
            string key = GetKey(row, col);
            return _data.TryGetValue(key, out var value) ? value : 0;  // Default to 0
        }
        set
        {
            ValidateIndices(row, col);
            string key = GetKey(row, col);

            if (value == 0)
            {
                // Remove zero values to save memory
                _data.Remove(key);
                Console.WriteLine($"  🗑️  Removed: SparseMatrix[{row},{col}] (was 0)");
            }
            else
            {
                _data[key] = value;
                Console.WriteLine($"  ✅ SparseMatrix[{row},{col}] = {value}");
            }
        }
    }

    private string GetKey(int row, int col) => $"{row},{col}";

    private void ValidateIndices(int row, int col)
    {
        if (row < 0 || row >= Rows)
            throw new IndexOutOfRangeException($"Row {row} out of range [0-{Rows - 1}]");
        if (col < 0 || col >= Cols)
            throw new IndexOutOfRangeException($"Col {col} out of range [0-{Cols - 1}]");
    }

    public int NonZeroCount => _data.Count;

    public void Display()
    {
        Console.WriteLine($"\n  💾 SparseMatrix ({Rows}×{Cols}), Non-zero: {NonZeroCount}:");
        Console.WriteLine($"     Memory efficiency: {((double)NonZeroCount / (Rows * Cols)) * 100:F1}% used");

        for (int i = 0; i < Math.Min(Rows, 5); i++)  // Show first 5 rows
        {
            Console.Write("     ");
            for (int j = 0; j < Math.Min(Cols, 10); j++)  // Show first 10 cols
            {
                int value = this[i, j];
                Console.Write(value == 0 ? "   ." : $"{value,4}");
            }
            if (Cols > 10) Console.Write(" ...");
            Console.WriteLine();
        }
        if (Rows > 5) Console.WriteLine("     ...");
    }
}

// ═══════════════════════════════════════════════════════════
// ✅ REAL-WORLD: Time Series with DateTime Indexer
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ PRODUCTION: Time series data with DateTime indexer
/// Use case: Stock prices, sensor data, logs
/// </summary>
public class TimeSeries
{
    private readonly SortedDictionary<DateTime, decimal> _data = new();

    // DateTime indexer
    public decimal this[DateTime timestamp]
    {
        get => _data.TryGetValue(timestamp, out var value) ? value : 0m;
        set
        {
            _data[timestamp] = value;
            Console.WriteLine($"  📅 TimeSeries[{timestamp:yyyy-MM-dd HH:mm}] = {value:F2}");
        }
    }

    // Range indexer - Get data between dates
    public Dictionary<DateTime, decimal> this[DateTime start, DateTime end]
    {
        get
        {
            return _data
                .Where(kvp => kvp.Key >= start && kvp.Key <= end)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }

    public int Count => _data.Count;

    public void DisplayRange(DateTime start, DateTime end)
    {
        var range = this[start, end];
        Console.WriteLine($"\n  📊 Time Series Data ({range.Count} entries):");
        foreach (var kvp in range)
        {
            Console.WriteLine($"     {kvp.Key:yyyy-MM-dd HH:mm}: {kvp.Value:F2}");
        }
    }
}
