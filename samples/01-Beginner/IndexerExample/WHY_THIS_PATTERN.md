# Why Indexers? - Detailed Explanation

## 📚 Table of Contents

1. [What is an Indexer?](#what-is-an-indexer)
2. [Why Indexers Over Methods?](#why-indexers-over-methods)
3. [When to Use Each Pattern](#when-to-use-each-pattern)
4. [Real-World Scenarios](#real-world-scenarios)
5. [Performance Considerations](#performance-considerations)
6. [Common Mistakes](#common-mistakes)
7. [Best Practices](#best-practices)
8. [Comparison Table](#comparison-table)

---

## What is an Indexer?

An **indexer** is a special property in C# that allows instances of a class to be indexed like arrays or dictionaries. It provides array-like syntax for accessing elements in a collection-like class.

### Syntax

```csharp
public T this[TIndex index]
{
    get { /* return value */ }
    set { /* set value */ }
}
```

**Key Points**:
- Uses `this` keyword
- Can have different parameter types (int, string, DateTime, etc.)
- Can have multiple overloads (dual indexer)
- Can be multi-dimensional (`this[int row, int col]`)
- Can be read-only (only getter) or read-write (getter + setter)

---

## Why Indexers Over Methods?

### ❌ BAD: GetItem/SetItem Methods

```csharp
public class BadCollection<T>
{
    private List<T> _items = new();

    public T GetItem(int index) => _items[index];
    public void SetItem(int index, T value) => _items[index] = value;
}

// Usage - Verbose and unclear
var collection = new BadCollection<string>();
collection.Add("Apple");

string item = collection.GetItem(0);      // ❌ Verbose
collection.SetItem(0, "Banana");          // ❌ Not intuitive
```

**Problems**:
1. **Verbose**: Requires method calls instead of natural syntax
2. **Not Intuitive**: Doesn't follow array/dictionary conventions
3. **Harder to Read**: More code to understand the intent
4. **Not Natural**: Breaks expected patterns for collections

### ✅ GOOD: Indexer

```csharp
public class GoodCollection<T>
{
    private List<T> _items = new();

    public T this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }
}

// Usage - Clean and intuitive
var collection = new GoodCollection<string>();
collection.Add("Apple");

string item = collection[0];              // ✅ Clean syntax
collection[0] = "Banana";                 // ✅ Intuitive
```

**Benefits**:
1. **Natural Syntax**: Follows array/dictionary conventions
2. **Cleaner Code**: Less verbose, more expressive
3. **Better Readability**: Intent is immediately clear
4. **Industry Standard**: Expected pattern for collections

### Comparison

| Aspect | Methods (GetItem/SetItem) | Indexer (this[]) |
|--------|---------------------------|------------------|
| Syntax | `obj.GetItem(0)` | `obj[0]` |
| Readability | ❌ Verbose | ✅ Clean |
| Intuitive | ❌ No | ✅ Yes |
| Standard | ❌ Non-standard | ✅ Standard |
| Lines of code | More | Less |

---

## When to Use Each Pattern

### 1️⃣ Integer Indexer: `this[int index]`

**Use When**:
- You have a collection-like class (list, array wrapper)
- Items are accessed by position
- Sequential access patterns

**Don't Use When**:
- Items are better accessed by name/key
- No natural ordering exists
- Performance overhead for index lookup

**Example Scenarios**:
```csharp
// ✅ GOOD - Sequential collection
var playlist = new Playlist();
Song firstSong = playlist[0];
Song lastSong = playlist[^1];  // C# 8+ Index

// ✅ GOOD - Array wrapper
var buffer = new CircularBuffer<int>(10);
buffer[0] = 42;
buffer[5] = 100;

// ❌ BAD - No natural ordering
var employees = new EmployeeManager();
Employee emp = employees[0];  // Which employee? Order not meaningful
```

### 2️⃣ String Indexer: `this[string key]`

**Use When**:
- Items are identified by name/key
- Dictionary-like access patterns
- Named configuration/settings

**Don't Use When**:
- Numeric indices make more sense
- Sequential iteration is primary use case
- Keys are complex objects (use proper key type)

**Example Scenarios**:
```csharp
// ✅ GOOD - Named access
var config = new AppConfig();
string dbConnection = config["DatabaseConnection"];
int timeout = int.Parse(config["Timeout"]);

// ✅ GOOD - Student grades
var grades = new StudentGrades();
grades["Alice"] = 95;
grades["Bob"] = 87;

// ❌ BAD - Should use proper type
var users = new UserManager();
User user = users["John"];  // What if multiple Johns? Use UserId instead
```

### 3️⃣ Dual Indexer: `this[int]` + `this[string]`

**Use When**:
- Multiple access patterns are needed
- Both position and name access make sense
- Flexibility is required for API consumers

**Don't Use When**:
- Only one access pattern is natural
- Confusion between overloads is likely
- Synchronization overhead is significant

**Example Scenarios**:
```csharp
// ✅ GOOD - Product catalog
var catalog = new ProductCatalog();

// Access by position (iterating all)
for (int i = 0; i < catalog.Count; i++)
    Console.WriteLine(catalog[i].Name);

// Access by product code (specific lookup)
Product laptop = catalog["PROD-123"];

// ✅ GOOD - Column collection
var columns = new DataColumns();
string firstColumnData = columns[0];        // By index
string nameColumnData = columns["Name"];    // By name

// ❌ BAD - Ambiguous access patterns
var items = new ItemList();
var item1 = items[5];      // Position 5 or ID 5?
var item2 = items["5"];    // String "5" or position?  // CONFUSING!
```

### 4️⃣ Multi-Dimensional: `this[int row, int col]`

**Use When**:
- Data is naturally 2D/3D (matrix, grid, table)
- Multi-dimensional access is primary pattern
- Coordinates/positions are meaningful

**Don't Use When**:
- Data is actually a flat collection
- Only single-dimensional access is needed
- Performance overhead is unacceptable

**Example Scenarios**:
```csharp
// ✅ GOOD - Matrix operations
var matrix = new Matrix(100, 100);
matrix[0, 0] = 1;
matrix[50, 50] = 100;

// ✅ GOOD - Game board
var board = new ChessBoard();
Piece piece = board[4, 4];  // e4 square
board[4, 4] = new Queen(Color.White);

// ✅ GOOD - Image pixels
var image = new Bitmap(1920, 1080);
Color pixel = image[100, 200];
image[100, 200] = Color.Red;

// ❌ BAD - Flattened is better
var list = new LinearData();
int value = list[row, col];  // Just use: list[row * width + col]
```

### 5️⃣ Range & Index: `this[Range]`, `this[Index]`

**Use When**:
- Slicing operations are common
- From-end access is needed
- Collection is sequential and ordered

**Don't Use When**:
- Collection is unordered
- Random access is not efficient
- C# version < 8.0

**Example Scenarios**:
```csharp
// ✅ GOOD - Array slicing
var numbers = new SmartArray<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
var first3 = numbers[..3];           // [1, 2, 3]
var last3 = numbers[^3..];           // [8, 9, 10]
var middle = numbers[3..7];          // [4, 5, 6, 7]
int lastItem = numbers[^1];          // 10

// ✅ GOOD - String operations
var text = new SmartString("Hello World");
string hello = text[..5];            // "Hello"
string world = text[6..];            // "World"

// ❌ BAD - Sparse collection
var sparseData = new SparseMatrix(1000, 1000);
var slice = sparseData[0..100];      // Inefficient! Most values are zero
```

### 6️⃣ Custom Type Indexer: `this[DateTime]`, `this[TKey]`

**Use When**:
- Custom key types provide meaningful access
- Type safety is important
- Domain-specific indexing makes sense

**Don't Use When**:
- Standard types (int, string) are sufficient
- Complexity is not warranted
- Performance overhead is significant

**Example Scenarios**:
```csharp
// ✅ GOOD - Time-series data
var stockPrices = new TimeSeries();
decimal price = stockPrices[DateTime.Now];
stockPrices[DateTime.Today] = 150.25m;

// ✅ GOOD - Configuration with enum
var settings = new AppSettings();
string theme = settings[SettingKey.Theme];
int fontSize = int.Parse(settings[SettingKey.FontSize]);

// ✅ GOOD - User lookup by strongly-typed ID
var userDb = new UserDatabase();
User user = userDb[new UserId(12345)];

// ❌ BAD - Overly complex key
var cache = new Cache();
var data = cache[new ComplexKey(id: 1, type: "user", region: "us-east")];
// Just use: cache.Get(id, type, region) method instead
```

### 7️⃣ Sparse Matrix: Dictionary-Backed Indexer

**Use When**:
- Most values are default/zero
- Memory efficiency is critical
- Large sparse data structures

**Don't Use When**:
- Data is dense (many non-default values)
- Sequential access is primary pattern
- Dictionary overhead is significant

**Example Scenarios**:
```csharp
// ✅ GOOD - Large sparse matrix
var adjacencyMatrix = new SparseMatrix(10000, 10000);
adjacencyMatrix[0, 1] = 1;     // Edge exists
adjacencyMatrix[5, 100] = 1;   // Edge exists
// Only 2 values stored, not 100 million!

// ✅ GOOD - Sparse grid
var gameWorld = new SparseGrid(1000000, 1000000);  // Huge world
gameWorld[100, 200] = new Tree();   // Only stores non-empty cells

// ❌ BAD - Dense matrix
var imagePixels = new SparseMatrix(1920, 1080);
// Every pixel has a color → 100% populated → Use regular array!
```

---

## Real-World Scenarios

### Scenario 1: Configuration Management

```csharp
// String indexer for named configuration
public class AppConfig
{
    private readonly Dictionary<string, string> _settings = new();

    public string this[string key]
    {
        get => _settings.TryGetValue(key, out var value) ? value : "";
        set => _settings[key] = value;
    }
}

// Usage
var config = new AppConfig();
config["DatabaseConnection"] = "Server=localhost;Database=MyDb";
config["LogLevel"] = "Debug";
string dbConn = config["DatabaseConnection"];
```

**Why this pattern?**
- Named access is intuitive for settings
- Follows convention (like `appsettings.json`)
- Easy to add/remove settings dynamically

### Scenario 2: Game Board

```csharp
// Multi-dimensional indexer for 2D board
public class ChessBoard
{
    private Piece[,] _board = new Piece[8, 8];

    public Piece this[int row, int col]
    {
        get
        {
            ValidatePosition(row, col);
            return _board[row, col];
        }
        set
        {
            ValidatePosition(row, col);
            _board[row, col] = value;
        }
    }

    // Also support chess notation
    public Piece this[string notation]
    {
        get
        {
            var (row, col) = ParseNotation(notation);  // "e4" → (4, 4)
            return this[row, col];
        }
        set
        {
            var (row, col) = ParseNotation(notation);
            this[row, col] = value;
        }
    }
}

// Usage
var board = new ChessBoard();
board[4, 4] = new Queen(Color.White);   // By position
board["e4"] = new Queen(Color.White);   // By notation (dual indexer!)
```

**Why this pattern?**
- 2D indexer matches natural board structure
- Dual indexer (int + string) supports both coordinate and notation access
- Type-safe, bounds-checked

### Scenario 3: Time-Series Data

```csharp
// DateTime indexer for time-series stock prices
public class StockPriceHistory
{
    private readonly SortedDictionary<DateTime, decimal> _prices = new();

    public decimal this[DateTime date]
    {
        get => _prices.TryGetValue(date.Date, out var price) ? price : 0m;
        set => _prices[date.Date] = value;
    }

    // Range query
    public Dictionary<DateTime, decimal> this[DateTime start, DateTime end]
    {
        get => _prices
            .Where(kvp => kvp.Key >= start && kvp.Key <= end)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}

// Usage
var prices = new StockPriceHistory();
prices[new DateTime(2024, 1, 15)] = 150.25m;
prices[new DateTime(2024, 1, 16)] = 152.50m;

// Single date access
decimal price = prices[new DateTime(2024, 1, 15)];

// Range query
var janPrices = prices[
    new DateTime(2024, 1, 1),
    new DateTime(2024, 1, 31)
];
```

**Why this pattern?**
- DateTime as key is natural for time-series data
- SortedDictionary maintains chronological order
- Range query indexer provides convenient date range access

### Scenario 4: Data Table Columns

```csharp
// Dual indexer for column access (by index or name)
public class DataRow
{
    private readonly List<object> _values = new();
    private readonly Dictionary<string, int> _columnIndexes = new();

    // By column index (position)
    public object this[int index]
    {
        get => _values[index];
        set => _values[index] = value;
    }

    // By column name
    public object this[string columnName]
    {
        get
        {
            if (_columnIndexes.TryGetValue(columnName, out int index))
                return _values[index];
            throw new KeyNotFoundException($"Column '{columnName}' not found");
        }
        set
        {
            if (_columnIndexes.TryGetValue(columnName, out int index))
                _values[index] = value;
            else
                throw new KeyNotFoundException($"Column '{columnName}' not found");
        }
    }
}

// Usage - Flexible access
var row = new DataRow();
row.AddColumn("Id", 1);
row.AddColumn("Name", "Alice");
row.AddColumn("Age", 30);

// Access by index (iteration)
for (int i = 0; i < row.ColumnCount; i++)
    Console.WriteLine(row[i]);

// Access by name (specific column)
string name = (string)row["Name"];
int age = (int)row["Age"];
```

**Why this pattern?**
- Dual indexer provides flexibility
- By-index for iteration, by-name for specific lookups
- Matches SQL result set patterns

---

## Performance Considerations

### Integer Indexer

**Performance**: ⭐⭐⭐⭐⭐ (Excellent)
- Direct array/list access: O(1)
- No lookup overhead
- Cache-friendly (sequential access)

```csharp
// Fast - Direct array access
public T this[int index] => _items[index];  // O(1)
```

### String Indexer

**Performance**: ⭐⭐⭐⭐ (Good)
- Dictionary lookup: O(1) average, O(n) worst case
- Hash computation overhead
- Less cache-friendly than array

```csharp
// Good - Dictionary lookup
public T this[string key] => _dict[key];  // O(1) average
```

### Multi-Dimensional Indexer

**Performance**: ⭐⭐⭐⭐⭐ (Excellent for dense) / ⭐⭐⭐ (Good for sparse)
- Dense (array-backed): O(1)
- Sparse (dictionary-backed): O(1) average

```csharp
// Dense - Fast array access
public T this[int row, int col] => _data[row, col];  // O(1)

// Sparse - Dictionary lookup
public T this[int row, int col] => _dict[$"{row},{col}"];  // O(1) average
```

### Range Indexer

**Performance**: ⭐⭐⭐ (Good)
- List.GetRange: O(n) where n is slice length
- Memory allocation for new list
- Good for small slices

```csharp
// Good - But creates new list
public List<T> this[Range range]  // O(n)
{
    get
    {
        var (start, length) = range.GetOffsetAndLength(_items.Count);
        return _items.GetRange(start, length);  // Allocates new list
    }
}
```

### Performance Comparison Table

| Indexer Type | Time Complexity | Space Overhead | Cache Friendly | Use Case |
|--------------|-----------------|----------------|----------------|----------|
| Integer | O(1) | None | ⭐⭐⭐⭐⭐ | Sequential collections |
| String | O(1) avg | Dictionary | ⭐⭐⭐ | Named access |
| Multi-dim (dense) | O(1) | None | ⭐⭐⭐⭐ | Dense matrices |
| Multi-dim (sparse) | O(1) avg | Dictionary | ⭐⭐ | Sparse matrices |
| Range | O(n) | New list | ⭐⭐⭐ | Slicing |
| DateTime | O(log n) | SortedDict | ⭐⭐ | Time-series |

---

## Common Mistakes

### ❌ Mistake 1: No Bounds Checking

```csharp
// ❌ BAD - No validation
public T this[int index]
{
    get => _items[index];  // Throws IndexOutOfRangeException if invalid
}

// ✅ GOOD - With validation
public T this[int index]
{
    get
    {
        if (index < 0 || index >= _items.Count)
            throw new IndexOutOfRangeException(
                $"Index {index} out of range [0-{_items.Count - 1}]");
        return _items[index];
    }
}
```

### ❌ Mistake 2: Returning Null for Missing Keys

```csharp
// ❌ BAD - Returns null (NullReferenceException later)
public string this[string key]
{
    get => _dict.ContainsKey(key) ? _dict[key] : null;  // ❌ null!
}

// ✅ GOOD - Default value or throw
public string this[string key]
{
    get => _dict.TryGetValue(key, out var value) ? value : "";  // ✅ Default
}

// ✅ GOOD - Throw exception for critical cases
public User this[int userId]
{
    get
    {
        if (_users.TryGetValue(userId, out var user))
            return user;
        throw new KeyNotFoundException($"User {userId} not found");
    }
}
```

### ❌ Mistake 3: Ignoring Validation in Setter

```csharp
// ❌ BAD - No validation
public int this[string studentName]
{
    set => _grades[studentName] = value;  // ❌ Allows grade = 150!
}

// ✅ GOOD - With validation
public int this[string studentName]
{
    set
    {
        if (value < 0 || value > 100)
            throw new ArgumentException("Grade must be 0-100");
        _grades[studentName] = value;
    }
}
```

### ❌ Mistake 4: Mixing Indexer Types Confusingly

```csharp
// ❌ BAD - Ambiguous dual indexer
public Item this[int id]        // Is this position or ID?
{
    get => _itemsById[id];     // Actually ID lookup!
}

public Item this[string code]   // String code lookup
{
    get => _itemsByCode[code];
}

// ✅ GOOD - Clear separation
public Item GetById(int id) => _itemsById[id];        // Method for ID
public Item this[int index] => _itemsList[index];     // Indexer for position
public Item this[string code] => _itemsByCode[code];  // Indexer for code
```

### ❌ Mistake 5: Forgetting Read-Only Indexers

```csharp
// ❌ BAD - Setter allows mutation of computed value
public decimal this[string productCode]
{
    get
    {
        var product = _products[productCode];
        return product.Price * (1 - product.Discount);  // Computed
    }
    set
    {
        // ❌ BAD: Can't set a computed value!
        // What does it mean to set final price? Change price? Change discount?
    }
}

// ✅ GOOD - Read-only indexer (no setter)
public decimal this[string productCode]
{
    get
    {
        var product = _products[productCode];
        return product.Price * (1 - product.Discount);
    }
    // No setter - read-only!
}
```

---

## Best Practices

### ✅ 1. Always Validate Indices

```csharp
public T this[int index]
{
    get
    {
        // Validate range
        if (index < 0 || index >= _items.Count)
            throw new IndexOutOfRangeException(
                $"Index {index} out of range [0-{_items.Count - 1}]");
        return _items[index];
    }
}
```

### ✅ 2. Use Meaningful Error Messages

```csharp
// ❌ BAD - Generic error
throw new Exception("Invalid index");

// ✅ GOOD - Specific error with context
throw new IndexOutOfRangeException(
    $"Index {index} out of range [0-{_items.Count - 1}]. " +
    $"Collection has {_items.Count} items.");
```

### ✅ 3. Document Indexer Behavior

```csharp
/// <summary>
/// Gets or sets the student's grade by name.
/// </summary>
/// <param name="studentName">The student's name (case-sensitive)</param>
/// <returns>The grade (0-100), or 0 if student not found</returns>
/// <exception cref="ArgumentException">Thrown when grade is not 0-100</exception>
public int this[string studentName]
{
    get => _grades.TryGetValue(studentName, out var grade) ? grade : 0;
    set
    {
        if (value < 0 || value > 100)
            throw new ArgumentException("Grade must be between 0-100");
        _grades[studentName] = value;
    }
}
```

### ✅ 4. Use Generic Indexers for Type Safety

```csharp
// ✅ GOOD - Generic collection
public class SmartArray<T>
{
    private readonly List<T> _items = new();

    public T this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }
}

// Usage - Type-safe!
var strings = new SmartArray<string>();
strings[0] = "Hello";
string value = strings[0];  // No cast needed!
```

### ✅ 5. Consider Read-Only Indexers

```csharp
// Read-only indexer (only get, no set)
public T this[int index]
{
    get
    {
        ValidateIndex(index);
        return _items[index];
    }
    // No setter - immutable from outside!
}
```

### ✅ 6. Use TryGet Pattern for Safe Access

```csharp
// Indexer throws exception
public User this[int userId]
{
    get => _users[userId];  // Throws KeyNotFoundException
}

// TryGet method for safe access
public bool TryGetUser(int userId, out User user)
{
    return _users.TryGetValue(userId, out user);
}

// Usage
if (userDb.TryGetUser(123, out var user))
    Console.WriteLine(user.Name);
else
    Console.WriteLine("User not found");
```

### ✅ 7. Synchronize Dual Indexers

```csharp
public class ProductCatalog
{
    private readonly List<Product> _products = new();
    private readonly Dictionary<string, Product> _productsByCode = new();

    public Product this[string code]
    {
        set
        {
            if (_productsByCode.ContainsKey(code))
            {
                // ✅ GOOD - Update both collections
                var existingProduct = _productsByCode[code];
                int index = _products.IndexOf(existingProduct);
                _products[index] = value;              // Update list
                _productsByCode[code] = value;         // Update dict
            }
            else
            {
                // ✅ GOOD - Add to both collections
                _products.Add(value);
                _productsByCode[code] = value;
            }
        }
    }
}
```

### ✅ 8. Leverage C# 8+ Range/Index

```csharp
public class SmartArray<T>
{
    // Integer indexer
    public T this[int index] { get; set; }

    // Range indexer - Slicing
    public List<T> this[Range range]
    {
        get
        {
            var (start, length) = range.GetOffsetAndLength(_items.Count);
            return _items.GetRange(start, length);
        }
    }

    // Index indexer - From-end
    public T this[Index index]
    {
        get
        {
            int actualIndex = index.GetOffset(_items.Count);
            return _items[actualIndex];
        }
    }
}

// Usage - Modern C# syntax
var arr = new SmartArray<int> { 1, 2, 3, 4, 5 };
var slice = arr[1..4];      // [2, 3, 4]
int last = arr[^1];         // 5
```

---

## Comparison Table

| Pattern | Syntax | Use Case | Performance | Complexity |
|---------|--------|----------|-------------|------------|
| **Integer Indexer** | `obj[0]` | Sequential collections | ⭐⭐⭐⭐⭐ | ⭐ Low |
| **String Indexer** | `obj["key"]` | Named access, configs | ⭐⭐⭐⭐ | ⭐⭐ Medium |
| **Dual Indexer** | `obj[0]` + `obj["key"]` | Flexible access | ⭐⭐⭐⭐ | ⭐⭐⭐ High |
| **Multi-Dimensional** | `obj[row, col]` | Matrices, grids | ⭐⭐⭐⭐⭐ | ⭐⭐ Medium |
| **Range Indexer** | `obj[1..5]` | Slicing | ⭐⭐⭐ | ⭐⭐ Medium |
| **Index Indexer** | `obj[^1]` | From-end access | ⭐⭐⭐⭐⭐ | ⭐ Low |
| **DateTime Indexer** | `obj[date]` | Time-series | ⭐⭐⭐ | ⭐⭐ Medium |
| **Sparse Matrix** | `obj[row, col]` (dict) | Sparse data | ⭐⭐⭐ | ⭐⭐⭐ High |

---

## Summary

### When to Use Indexers

✅ **DO use indexers when**:
1. You have a collection-like class
2. Array/dictionary syntax makes sense
3. Access by index/key is primary pattern
4. You want natural, intuitive syntax

❌ **DON'T use indexers when**:
1. Simple methods are clearer
2. Multiple operations are needed (use methods)
3. Complexity outweighs benefits
4. Access pattern is not collection-like

### Key Takeaways

1. **Indexers provide natural array/dictionary syntax** for custom collections
2. **Multiple indexer types** (int, string, multi-dim, Range, custom) serve different use cases
3. **Always validate** indices and provide meaningful error messages
4. **Performance varies** by indexer type (array-backed ⭐⭐⭐⭐⭐, dict-backed ⭐⭐⭐⭐)
5. **Use read-only indexers** for computed/derived values
6. **Document behavior** clearly, especially for dual indexers
7. **Leverage modern C#** (Range, Index) for expressive code

---

## Further Reading

- [C# Indexers (Microsoft Docs)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/indexers/)
- [Range and Index (C# 8)](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#indices-and-ranges)
- [Collection Indexers Best Practices](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/indexers/using-indexers)
- [Generic Collections (MSDN)](https://docs.microsoft.com/en-us/dotnet/standard/collections/commonly-used-collection-types)

---

**Back to [README.md](README.md)**
