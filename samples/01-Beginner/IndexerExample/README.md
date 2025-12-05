# 🔢 Indexer Example - Custom Collection Indexers

## 📚 Overview

**IndexerExample** demonstrates all C# indexer patterns from basic to advanced, including integer indexers (array-like), string indexers (dictionary-like), multi-dimensional indexers (matrix/cube), and modern C# 8+ features like Range and Index.

**Scenario**: Custom collection classes with intuitive syntax
**Project Type**: .NET 8.0 Console Application
**Lines of Code**: 936 lines (488 Program.cs + 448 Collections.cs)
**Build Status**: ✅ 0 errors, 121 style warnings

---

## 🎯 What You'll Learn

### ✅ Core Indexer Patterns

1. **Integer Indexer** - Array-like access with `this[int index]`
2. **String Indexer** - Dictionary-like access with `this[string key]`
3. **Dual Indexer** - Both int and string on same collection
4. **Multi-Dimensional Indexer** - 2D and 3D access with `this[int row, int col]`
5. **Range Indexer** (C# 8+) - Slicing with `this[Range range]`
6. **Index Indexer** (C# 8+) - From-end access with `this[Index index]`
7. **DateTime Indexer** - Time-series data access
8. **Sparse Matrix** - Memory-efficient dictionary-backed indexer

### ✅ Key Concepts

- **Why indexers over GetItem/SetItem methods?** - Cleaner, more intuitive syntax
- **Validation in indexers** - Bounds checking, range validation
- **Multiple indexer overloads** - Type-based overloading (int vs string)
- **Read-only indexers** - Only getter, no setter
- **Computed indexers** - Range queries, slicing
- **Memory efficiency** - Sparse storage patterns

---

## 📂 Project Structure

```
IndexerExample/
├── IndexerExample.csproj          # .NET 8.0 project
├── Collections.cs                 # 9 collection classes (448 lines)
│   ├── BadCollection<T>           # ❌ Anti-pattern: GetItem/SetItem
│   ├── SmartArray<T>              # ✅ Integer + Range + Index indexers
│   ├── StudentGrades              # ✅ String indexer with validation
│   ├── ProductCatalog + Product   # ✅ Dual indexer (int + string)
│   ├── Matrix                     # ✅ 2D indexer
│   ├── Cube<T>                    # ✅ 3D indexer
│   ├── SparseMatrix               # ✅ Memory-efficient 2D indexer
│   └── TimeSeries                 # ✅ DateTime indexer
├── Program.cs                     # 8 demonstrations (488 lines)
├── README.md                      # This file
└── WHY_THIS_PATTERN.md            # Detailed explanation
```

---

## 🚀 Quick Start

### Build and Run

```bash
# Navigate to project directory
cd samples/01-Beginner/IndexerExample

# Build the project
dotnet build

# Run all demonstrations
dotnet run
```

### Expected Output

The program demonstrates 8 indexer patterns with clear output:

```
╔═══════════════════════════════════════════════════════════╗
║   📚  INDEXER PATTERNS - ÖZEL KOLEKSİYON SINIFLARI    ║
╚═══════════════════════════════════════════════════════════╝

═══ 1. ❌ BAD PRACTICE: GetItem/SetItem ═══
═══ 2. ✅ INTEGER INDEXER (Array-like) ═══
═══ 3. ✅ STRING INDEXER (Dictionary-like) ═══
═══ 4. ✅ DUAL INDEXER (Both int + string) ═══
═══ 5. ✅ MULTI-DIMENSIONAL INDEXER (2D + 3D) ═══
═══ 6. ✅ RANGE & INDEX INDEXER (C# 8+ slicing) ═══
═══ 7. ✅ SPARSE MATRIX (Memory Efficient) ═══
═══ 8. ✅ DATETIME INDEXER (Time Series) ═══
```

---

## 💻 Code Examples

### 1️⃣ BAD PRACTICE: GetItem/SetItem Methods

**Problem**: Verbose syntax, not intuitive

```csharp
public class BadCollection<T>
{
    private readonly List<T> _items = new();

    // ❌ BAD: Method-based access - verbose!
    public T GetItem(int index) => _items[index];
    public void SetItem(int index, T value) => _items[index] = value;
}

// Usage - verbose and unclear
var bad = new BadCollection<string>();
bad.Add("Apple");
string item = bad.GetItem(0);      // ❌ Verbose
bad.SetItem(0, "Banana");           // ❌ Not intuitive
```

**Problems**:
- ❌ Verbose: `GetItem(0)` instead of `[0]`
- ❌ Not intuitive: `SetItem(1, value)` instead of `[1] = value`
- ❌ No array-like syntax
- ❌ More code to write
- ❌ Harder to read

---

### 2️⃣ GOOD: Integer Indexer (Array-like)

**Solution**: Array-like syntax with `this[int index]`

```csharp
public class SmartArray<T>
{
    private readonly List<T> _items = new();

    // ✅ Integer indexer - Array-like access
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _items.Count)
                throw new IndexOutOfRangeException(
                    $"Index {index} out of range [0-{_items.Count - 1}]");
            return _items[index];
        }
        set
        {
            if (index < 0 || index >= _items.Count)
                throw new IndexOutOfRangeException(
                    $"Index {index} out of range [0-{_items.Count - 1}]");
            _items[index] = value;
            Console.WriteLine($"  ✅ SmartArray[{index}] = {value}");
        }
    }

    public void Add(T item) => _items.Add(item);
    public int Count => _items.Count;
}

// Usage - clean and intuitive
var fruits = new SmartArray<string>();
fruits.Add("Apple");
fruits.Add("Banana");

string first = fruits[0];           // ✅ Clean syntax
fruits[1] = "Blueberry";            // ✅ Intuitive

// Bounds checking
fruits[10];  // ❌ Throws IndexOutOfRangeException
```

**Benefits**:
- ✅ Array-like syntax: `array[index]`
- ✅ Intuitive read/write operations
- ✅ Automatic bounds checking
- ✅ Validation in getter/setter

---

### 3️⃣ GOOD: String Indexer (Dictionary-like)

**Use Case**: Named access to collection items

```csharp
public class StudentGrades
{
    private readonly Dictionary<string, int> _grades = new();

    // ✅ String indexer - Dictionary-like access
    public int this[string studentName]
    {
        get
        {
            if (_grades.TryGetValue(studentName, out var grade))
                return grade;

            Console.WriteLine($"  ⚠️  Student '{studentName}' not found, returning 0");
            return 0;  // Default value
        }
        set
        {
            // Validation
            if (value < 0 || value > 100)
                throw new ArgumentException("Grade must be between 0-100!",
                    nameof(value));

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
}

// Usage
var grades = new StudentGrades();
grades["Ali"] = 85;              // ✅ Added: Ali = 85
grades["Ayşe"] = 92;             // ✅ Added: Ayşe = 92

int aliGrade = grades["Ali"];    // 85
grades["Ali"] = 88;              // ✏️  Updated: Ali = 88

int missing = grades["Unknown"]; // ⚠️  Student 'Unknown' not found, returning 0
grades["Test"] = 150;            // ❌ Throws ArgumentException
```

**Benefits**:
- ✅ Dictionary-like syntax: `dict["key"]`
- ✅ Named access instead of numeric indices
- ✅ Validation in setter
- ✅ Default value for missing keys

---

### 4️⃣ ADVANCED: Dual Indexer (int + string)

**Use Case**: Flexible access patterns - by index or by name

```csharp
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
                throw new IndexOutOfRangeException(
                    $"Index {index} out of range [0-{_products.Count - 1}]");
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
            }
            else
            {
                // Add new product
                _products.Add(value);
                _productsByCode[code] = value;
            }
        }
    }

    public int Count => _products.Count;
}

public class Product
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public decimal Price { get; init; }
}

// Usage - Dual access
var catalog = new ProductCatalog();
catalog["PROD001"] = new Product
{
    Code = "PROD001",
    Name = "Laptop",
    Price = 1200m
};

// Access by index (position)
Product firstProduct = catalog[0];

// Access by code (name)
Product laptop = catalog["PROD001"];
```

**Benefits**:
- ✅ Flexible access: both index and key
- ✅ Type-based overloading (int vs string)
- ✅ Synchronized storage (List + Dictionary)
- ✅ Update or add automatically

---

### 5️⃣ ADVANCED: Multi-Dimensional Indexer (2D + 3D)

**Use Case**: Matrix, grid, table data

```csharp
// 2D Matrix
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
        }
    }

    private void ValidateIndices(int row, int col)
    {
        if (row < 0 || row >= Rows)
            throw new IndexOutOfRangeException($"Row {row} out of range [0-{Rows - 1}]");
        if (col < 0 || col >= Cols)
            throw new IndexOutOfRangeException($"Col {col} out of range [0-{Cols - 1}]");
    }
}

// 3D Cube
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

// Usage
var matrix = new Matrix(3, 3);
matrix[0, 0] = 1;
matrix[1, 1] = 5;
matrix[2, 2] = 9;

var cube = new Cube<string>(2, 2, 2);
cube[0, 0, 0] = "Corner";
cube[1, 1, 1] = "Opposite Corner";
```

**Benefits**:
- ✅ Multi-parameter indexers
- ✅ Natural syntax for 2D/3D data
- ✅ Bounds checking for all dimensions
- ✅ Generic support (Cube<T>)

---

### 6️⃣ MODERN: Range & Index Indexer (C# 8+)

**Use Case**: Slicing, from-end access

```csharp
public class SmartArray<T>
{
    private readonly List<T> _items = new();

    // Range indexer (C# 8+) - Slice access
    public List<T> this[Range range]
    {
        get
        {
            var (start, length) = range.GetOffsetAndLength(_items.Count);
            return _items.GetRange(start, length);
        }
    }

    // Index indexer (C# 8+) - From end access
    public T this[Index index]
    {
        get
        {
            int actualIndex = index.GetOffset(_items.Count);
            return _items[actualIndex];
        }
        set
        {
            int actualIndex = index.GetOffset(_items.Count);
            _items[actualIndex] = value;
        }
    }
}

// Usage - Slicing with Range
var numbers = new SmartArray<int>();
for (int i = 0; i < 10; i++)
    numbers.Add(i * 10);

var slice1 = numbers[2..5];      // [20, 30, 40]
var slice2 = numbers[..3];       // [0, 10, 20] (first 3)
var slice3 = numbers[7..];       // [70, 80, 90] (from 7 to end)
var slice4 = numbers[^3..];      // Last 3 elements
var slice5 = numbers[^5..^2];    // From 5th from end to 2nd from end

// Usage - From-end with Index
int last = numbers[^1];          // Last element (90)
int secondLast = numbers[^2];    // Second last (80)
numbers[^1] = 999;               // Set last element
```

**Benefits**:
- ✅ Modern C# 8+ syntax
- ✅ Slicing with Range operator (`..`)
- ✅ From-end access with Index operator (`^`)
- ✅ Less code, more expressive

---

### 7️⃣ PRODUCTION: Sparse Matrix (Memory Efficient)

**Use Case**: Large matrices with many zero/default values

```csharp
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
            }
            else
            {
                _data[key] = value;
            }
        }
    }

    private string GetKey(int row, int col) => $"{row},{col}";
    public int NonZeroCount => _data.Count;

    private void ValidateIndices(int row, int col)
    {
        if (row < 0 || row >= Rows)
            throw new IndexOutOfRangeException($"Row {row} out of range [0-{Rows - 1}]");
        if (col < 0 || col >= Cols)
            throw new IndexOutOfRangeException($"Col {col} out of range [0-{Cols - 1}]");
    }
}

// Usage - Memory-efficient
var sparse = new SparseMatrix(1000, 1000); // 1 million cells!

// Only store non-zero values
sparse[0, 0] = 5;
sparse[500, 500] = 10;
sparse[999, 999] = 15;

Console.WriteLine($"Non-zero cells: {sparse.NonZeroCount}");  // 3
Console.WriteLine($"Memory efficiency: {(sparse.NonZeroCount / 1_000_000.0) * 100:F1}%");  // 0.0003%

// Reading zero cells (not stored)
int zero = sparse[1, 1];  // 0 (default, not stored)
```

**Benefits**:
- ✅ Dictionary-backed storage
- ✅ Only stores non-default values
- ✅ Memory efficient for sparse data
- ✅ 99.9% memory savings possible

---

### 8️⃣ PRODUCTION: TimeSeries (DateTime Indexer)

**Use Case**: Stock prices, sensor data, logs

```csharp
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
}

// Usage - Time-series data
var stockPrices = new TimeSeries();
var today = DateTime.Today;

stockPrices[today.AddHours(9)] = 100.50m;   // 9:00 AM
stockPrices[today.AddHours(10)] = 102.30m;  // 10:00 AM
stockPrices[today.AddHours(11)] = 101.80m;  // 11:00 AM
stockPrices[today.AddHours(12)] = 103.20m;  // 12:00 PM

// Range query
var morningPrices = stockPrices[
    today.AddHours(9),
    today.AddHours(12)
];  // All prices from 9 AM to 12 PM
```

**Benefits**:
- ✅ DateTime as index key
- ✅ Sorted storage (SortedDictionary)
- ✅ Range queries between dates
- ✅ Real-world time-series patterns

---

## 📊 What's Covered

### 8 Complete Demonstrations

1. **DemonstrateBadPractice()** - GetItem/SetItem vs indexer comparison
2. **DemonstrateIntegerIndexer()** - Array-like access with bounds checking
3. **DemonstrateStringIndexer()** - Dictionary-like access with validation
4. **DemonstrateDualIndexer()** - Both int and string on same collection
5. **DemonstrateMultiDimensionalIndexer()** - 2D Matrix and 3D Cube
6. **DemonstrateRangeIndexer()** - Range and Index (C# 8+ slicing)
7. **DemonstrateSparseMatrix()** - Memory-efficient dictionary-backed storage
8. **DemonstrateTimeSeriesIndexer()** - DateTime-based access and range queries

### Key Features Demonstrated

✅ **Indexer Types**:
- Integer indexer (`this[int index]`)
- String indexer (`this[string key]`)
- Multi-dimensional indexer (`this[int row, int col]`)
- 3D indexer (`this[int x, int y, int z]`)
- Range indexer (`this[Range range]`) - C# 8+
- Index indexer (`this[Index index]`) - C# 8+
- DateTime indexer (`this[DateTime timestamp]`)
- Dual indexer (multiple parameter types)

✅ **Validation Patterns**:
- Bounds checking (index out of range)
- Range validation (min/max values)
- Null/missing key handling
- Exception throwing vs default values

✅ **Memory Efficiency**:
- Sparse storage (Dictionary-backed)
- Only non-default values stored
- Memory usage reporting

✅ **Modern C# Features**:
- Range operator (`..`)
- Index operator (`^`)
- SortedDictionary for ordered data
- Generic types (`SmartArray<T>`, `Cube<T>`)

---

## 💡 Best Practices

### ✅ DO

1. **Always validate indices** - Throw `IndexOutOfRangeException` for invalid indices
2. **Provide meaningful error messages** - Include actual index and valid range
3. **Use read-only indexers** - Only getter when modification not allowed
4. **Consider default values** - Return default for missing keys (e.g., 0 for grades)
5. **Use generic indexers** - Make collection type-safe with `<T>`
6. **Validate values in setter** - Check business rules (e.g., grade 0-100)
7. **Use Range/Index** - Leverage C# 8+ slicing features for modern code
8. **Document behavior** - XML comments for indexer getters/setters

### ❌ DON'T

1. **Don't use GetItem/SetItem methods** - Use indexers instead
2. **Don't skip bounds checking** - Always validate indices
3. **Don't return null** - Return default value or throw exception
4. **Don't ignore validation** - Check parameter validity
5. **Don't mix concerns** - Keep indexer logic focused
6. **Don't forget about exceptions** - Handle `IndexOutOfRangeException`, `KeyNotFoundException`

---

## 🔗 Related Patterns

- **Properties** - Similar syntax but for named members, not indexed access
- **Iterator Pattern** - `foreach` support with `IEnumerable<T>`
- **Collection Initializers** - `{ [key] = value }` syntax
- **Dictionary<TKey, TValue>** - Built-in indexed collection
- **Array/List<T>** - Built-in integer-indexed collections

---

## 📖 References

- [C# Indexers Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/indexers/)
- [Range and Index (C# 8)](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#indices-and-ranges)
- [Sparse Matrix (Wikipedia)](https://en.wikipedia.org/wiki/Sparse_matrix)
- [Collection Indexers (C# Programming Guide)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/indexers/using-indexers)

---

## 📝 Summary

This project demonstrates **8 comprehensive indexer patterns** in C#:

- ✅ **Integer indexer** - Array-like syntax
- ✅ **String indexer** - Dictionary-like syntax
- ✅ **Dual indexer** - Type-based overloading
- ✅ **Multi-dimensional** - 2D/3D data structures
- ✅ **Range & Index** - Modern C# 8+ slicing
- ✅ **Sparse matrix** - Memory-efficient storage
- ✅ **TimeSeries** - DateTime-based access
- ✅ **Validation** - Bounds checking, business rules

**Total**: 936 lines of production-ready code with comprehensive demonstrations.

---

**See also**: [WHY_THIS_PATTERN.md](WHY_THIS_PATTERN.md) for in-depth explanation of when and why to use each indexer pattern.
