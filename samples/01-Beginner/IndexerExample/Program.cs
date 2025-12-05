// SCENARIO: Özel Koleksiyon Sınıfları - Indexer Patterns
// BAD PRACTICE: GetItem/SetItem metodları ile erişim (verbose)
// GOOD PRACTICE: Indexer ile array/dictionary syntax
// ADVANCED: Multi-dimensional, Range, DateTime indexers

using IndexerExample;

class Program
{
    static void Main()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   📚  INDEXER PATTERNS - ÖZEL KOLEKSİYON SIN IFLARI    ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // 1. BAD PRACTICE
        Console.WriteLine("═══ 1. ❌ BAD PRACTICE: GetItem/SetItem ═══\n");
        DemonstrateBadPractice();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 2. Integer Indexer
        Console.WriteLine("═══ 2. ✅ INTEGER INDEXER (Array-like) ═══\n");
        DemonstrateIntegerIndexer();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 3. String Indexer
        Console.WriteLine("═══ 3. ✅ STRING INDEXER (Dictionary-like) ═══\n");
        DemonstrateStringIndexer();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 4. Dual Indexer (int + string)
        Console.WriteLine("═══ 4. ✅ DUAL INDEXER (int + string) ═══\n");
        DemonstrateDualIndexer();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 5. Multi-Dimensional Indexer
        Console.WriteLine("═══ 5. ✅ MULTI-DIMENSIONAL INDEXER (2D, 3D) ═══\n");
        DemonstrateMultiDimensionalIndexer();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 6. Range Indexer (C# 8+)
        Console.WriteLine("═══ 6. ✅ RANGE INDEXER (C# 8+) ═══\n");
        DemonstrateRangeIndexer();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 7. Sparse Matrix (Memory Efficient)
        Console.WriteLine("═══ 7. ✅ SPARSE MATRIX - Memory Efficient ═══\n");
        DemonstrateSparseMatrix();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 8. DateTime Indexer (Time Series)
        Console.WriteLine("═══ 8. ✅ DATETIME INDEXER - Time Series ═══\n");
        DemonstrateTimeSeriesIndexer();

        // Final Summary
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    📊 ÖZET                                ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("✅ ÖĞRENİLENLER:");
        Console.WriteLine("   • Indexer: this[index] syntax ile custom access");
        Console.WriteLine("   • Integer indexer: Array-like access (this[int])");
        Console.WriteLine("   • String indexer: Dictionary-like access (this[string])");
        Console.WriteLine("   • Multi-param indexer: Matrix access (this[int, int])");
        Console.WriteLine("   • Range indexer: Slice access (this[Range])");
        Console.WriteLine("   • Index indexer: From end access (this[Index])");
        Console.WriteLine("   • DateTime indexer: Time-based access (this[DateTime])");
        Console.WriteLine();
        Console.WriteLine("💡 BEST PRACTICES:");
        Console.WriteLine("   • Validation her zaman indexer içinde");
        Console.WriteLine("   • Clear exception messages (IndexOutOfRangeException)");
        Console.WriteLine("   • Multiple indexer types aynı class'ta olabilir");
        Console.WriteLine("   • Sparse structures ile memory efficiency");
    }

    /// <summary>
    /// ❌ BAD PRACTICE: GetItem/SetItem metodları (verbose)
    /// </summary>
    static void DemonstrateBadPractice()
    {
        Console.WriteLine("💀 Bad Practice: GetItem/SetItem metodları\n");

        BadCollection<string> bad = new BadCollection<string>();
        bad.Add("Apple");
        bad.Add("Banana");
        bad.Add("Cherry");

        // ❌ BAD: Verbose method calls
        Console.WriteLine("GetItem/SetItem kullanımı:");
        Console.WriteLine($"  bad.GetItem(0): {bad.GetItem(0)}");
        Console.WriteLine($"  bad.GetItem(1): {bad.GetItem(1)}");

        bad.SetItem(1, "Blueberry");
        Console.WriteLine($"  bad.GetItem(1) after change: {bad.GetItem(1)}");

        Console.WriteLine("\n⚠️  SORUNLAR:");
        Console.WriteLine("   ❌ Verbose syntax: GetItem(0) yerine [0]");
        Console.WriteLine("   ❌ Not intuitive: SetItem(1, value) yerine [1] = value");
        Console.WriteLine("   ❌ No array-like syntax");
        Console.WriteLine("   ❌ More code to write");
        Console.WriteLine("   ❌ Harder to read");

        Console.WriteLine("\n✅ SOLUTION: Use indexer!");
        SmartArray<string> good = new SmartArray<string>();
        good.Add("Apple");
        good.Add("Banana");
        good.Add("Cherry");

        Console.WriteLine($"  good[0]: {good[0]}");  // ✅ Array-like!
        good[1] = "Blueberry";                       // ✅ Assignment syntax!
        Console.WriteLine($"  good[1] after change: {good[1]}");
    }

    /// <summary>
    /// ✅ GOOD: Integer indexer ile array-like erişim
    /// </summary>
    static void DemonstrateIntegerIndexer()
    {
        Console.WriteLine("🎯 Integer indexer: this[int index]\n");

        SmartArray<string> fruits = new SmartArray<string>();
        fruits.Add("Apple");
        fruits.Add("Banana");
        fruits.Add("Cherry");
        fruits.Add("Date");
        fruits.Add("Elderberry");

        Console.WriteLine("Initial array:");
        fruits.Display();

        // Array-like access
        Console.WriteLine("\n📋 Reading elements:");
        Console.WriteLine($"  fruits[0] = {fruits[0]}");
        Console.WriteLine($"  fruits[2] = {fruits[2]}");
        Console.WriteLine($"  fruits[4] = {fruits[4]}");

        // Assignment
        Console.WriteLine("\n✏️  Modifying elements:");
        fruits[1] = "Blueberry";
        fruits[3] = "Dragon Fruit";

        Console.WriteLine("\nModified array:");
        fruits.Display();

        // Bounds checking
        Console.WriteLine("\n🚫 Bounds checking:");
        try
        {
            Console.WriteLine($"  fruits[10]: {fruits[10]}");
        }
        catch (IndexOutOfRangeException ex)
        {
            Console.WriteLine($"  ❌ EXCEPTION: {ex.Message}");
        }

        try
        {
            Console.WriteLine($"  fruits[-1]: {fruits[-1]}");
        }
        catch (IndexOutOfRangeException ex)
        {
            Console.WriteLine($"  ❌ EXCEPTION: {ex.Message}");
        }

        Console.WriteLine("\n💡 INTEGER INDEXER BENEFITS:");
        Console.WriteLine("   ✅ Array-like syntax: array[index]");
        Console.WriteLine("   ✅ Intuitive read/write operations");
        Console.WriteLine("   ✅ Automatic bounds checking");
        Console.WriteLine("   ✅ Validation in getter/setter");
    }

    /// <summary>
    /// ✅ GOOD: String indexer ile dictionary-like erişim
    /// </summary>
    static void DemonstrateStringIndexer()
    {
        Console.WriteLine("🎯 String indexer: this[string key]\n");

        StudentGrades grades = new StudentGrades();

        // Dictionary-like assignment
        Console.WriteLine("Adding students:");
        grades["Ali"] = 85;
        grades["Ayşe"] = 92;
        grades["Mehmet"] = 78;
        grades["Zeynep"] = 88;
        grades["Can"] = 95;

        grades.DisplayAll();

        // Reading values
        Console.WriteLine("\n📋 Reading grades:");
        Console.WriteLine($"  Ali's grade: {grades["Ali"]}");
        Console.WriteLine($"  Ayşe's grade: {grades["Ayşe"]}");

        // Updating values
        Console.WriteLine("\n✏️  Updating grades:");
        grades["Mehmet"] = 82;  // Update existing

        // Default value for missing key
        Console.WriteLine("\n🔍 Missing key behavior:");
        Console.WriteLine($"  grades[\"Ahmet\"] (not exists): {grades["Ahmet"]}");

        // Validation
        Console.WriteLine("\n🚫 Validation:");
        try
        {
            Console.WriteLine("  Trying to set grade = 150:");
            grades["Ali"] = 150;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"  ❌ EXCEPTION: {ex.Message}");
        }

        try
        {
            Console.WriteLine("  Trying to set grade = -10:");
            grades["Ayşe"] = -10;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"  ❌ EXCEPTION: {ex.Message}");
        }

        Console.WriteLine("\n💡 STRING INDEXER BENEFITS:");
        Console.WriteLine("   ✅ Dictionary-like syntax: dict[\"key\"]");
        Console.WriteLine("   ✅ Named access instead of numeric indices");
        Console.WriteLine("   ✅ Validation in setter");
        Console.WriteLine("   ✅ Default value for missing keys");
    }

    /// <summary>
    /// ✅ ADVANCED: Dual indexer (hem int hem string)
    /// </summary>
    static void DemonstrateDualIndexer()
    {
        Console.WriteLine("🎯 Dual indexer: this[int] + this[string]\n");
        Console.WriteLine("Same collection, two access patterns!\n");

        ProductCatalog catalog = new ProductCatalog();

        // Adding products via string indexer
        Console.WriteLine("Adding products via string indexer:");
        catalog["PROD001"] = new Product { Code = "PROD001", Name = "Laptop", Price = 45000m };
        catalog["PROD002"] = new Product { Code = "PROD002", Name = "Mouse", Price = 250m };
        catalog["PROD003"] = new Product { Code = "PROD003", Name = "Keyboard", Price = 800m };

        catalog.DisplayAll();

        // Access by integer index
        Console.WriteLine("\n📋 Access by INTEGER index:");
        Console.WriteLine($"  catalog[0]: {catalog[0].Name} - {catalog[0].Price:C}");
        Console.WriteLine($"  catalog[1]: {catalog[1].Name} - {catalog[1].Price:C}");
        Console.WriteLine($"  catalog[2]: {catalog[2].Name} - {catalog[2].Price:C}");

        // Access by string code
        Console.WriteLine("\n📋 Access by STRING code:");
        Console.WriteLine($"  catalog[\"PROD001\"]: {catalog["PROD001"].Name} - {catalog["PROD001"].Price:C}");
        Console.WriteLine($"  catalog[\"PROD002\"]: {catalog["PROD002"].Name} - {catalog["PROD002"].Price:C}");

        // Update via string indexer
        Console.WriteLine("\n✏️  Updating via string indexer:");
        catalog["PROD002"] = new Product { Code = "PROD002", Name = "Wireless Mouse", Price = 350m };

        // Read updated value via int indexer
        Console.WriteLine($"  catalog[1] (updated): {catalog[1].Name} - {catalog[1].Price:C}");

        Console.WriteLine("\n💡 DUAL INDEXER BENEFITS:");
        Console.WriteLine("   ✅ Flexible access: by position OR by key");
        Console.WriteLine("   ✅ Different use cases: iteration (int) vs lookup (string)");
        Console.WriteLine("   ✅ Same collection, multiple access patterns");
        Console.WriteLine("   ✅ Type-safe overloads (compiler distinguishes)");
    }

    /// <summary>
    /// ✅ GOOD: Multi-dimensional indexers (2D, 3D)
    /// </summary>
    static void DemonstrateMultiDimensionalIndexer()
    {
        Console.WriteLine("🎯 Multi-dimensional indexers\n");

        // 2D indexer (Matrix)
        Console.WriteLine("1️⃣  2D INDEXER: Matrix[row, col]\n");

        Matrix matrix = new Matrix(4, 4);

        Console.WriteLine("Filling matrix with pattern (i * 10 + j):");
        matrix.Fill((i, j) => i * 10 + j);
        matrix.Display();

        Console.WriteLine("\n📋 Reading elements:");
        Console.WriteLine($"  matrix[0, 0] = {matrix[0, 0]}");
        Console.WriteLine($"  matrix[1, 2] = {matrix[1, 2]}");
        Console.WriteLine($"  matrix[3, 3] = {matrix[3, 3]}");

        Console.WriteLine("\n✏️  Modifying elements:");
        matrix[1, 1] = 99;
        matrix[2, 2] = 88;

        matrix.Display();

        // 3D indexer (Cube)
        Console.WriteLine("\n2️⃣  3D INDEXER: Cube[x, y, z]\n");

        Cube<int> cube = new Cube<int>(3, 3, 3);

        Console.WriteLine("Setting values in 3D space:");
        cube[0, 0, 0] = 1;
        cube[1, 1, 1] = 2;
        cube[2, 2, 2] = 3;
        cube[0, 1, 2] = 4;

        Console.WriteLine("\n📋 Reading 3D elements:");
        Console.WriteLine($"  cube[0, 0, 0] = {cube[0, 0, 0]}");
        Console.WriteLine($"  cube[1, 1, 1] = {cube[1, 1, 1]}");
        Console.WriteLine($"  cube[2, 2, 2] = {cube[2, 2, 2]}");
        Console.WriteLine($"  cube[0, 1, 2] = {cube[0, 1, 2]}");

        Console.WriteLine("\n💡 MULTI-DIMENSIONAL INDEXER BENEFITS:");
        Console.WriteLine("   ✅ Natural matrix/grid/cube access");
        Console.WriteLine("   ✅ Multiple parameters: this[int, int] or this[int, int, int]");
        Console.WriteLine("   ✅ Readable syntax: matrix[row, col]");
        Console.WriteLine("   ✅ Bounds checking per dimension");
    }

    /// <summary>
    /// ✅ MODERN: Range ve Index indexers (C# 8+)
    /// </summary>
    static void DemonstrateRangeIndexer()
    {
        Console.WriteLine("🎯 Range ve Index indexers (C# 8+)\n");

        SmartArray<int> numbers = new SmartArray<int>();
        for (int i = 0; i < 10; i++)
            numbers.Add(i * 10);

        Console.WriteLine("Initial array:");
        numbers.Display();

        // Range indexer - Slicing
        Console.WriteLine("\n📋 RANGE INDEXER - Slicing:");

        var slice1 = numbers[2..5];    // [2, 3, 4]
        Console.WriteLine($"  numbers[2..5]: [{string.Join(", ", slice1)}]");

        var slice2 = numbers[..3];     // [0, 1, 2]
        Console.WriteLine($"  numbers[..3]: [{string.Join(", ", slice2)}]");

        var slice3 = numbers[7..];     // [7, 8, 9]
        Console.WriteLine($"  numbers[7..]: [{string.Join(", ", slice3)}]");

        var slice4 = numbers[^3..];    // Last 3 elements
        Console.WriteLine($"  numbers[^3..]: [{string.Join(", ", slice4)}]");

        var slice5 = numbers[^5..^2];  // From 5th from end to 2nd from end
        Console.WriteLine($"  numbers[^5..^2]: [{string.Join(", ", slice5)}]");

        // Index indexer - From end access
        Console.WriteLine("\n📋 INDEX INDEXER - From end:");

        Console.WriteLine($"  numbers[^1] (last): {numbers[^1]}");
        Console.WriteLine($"  numbers[^2] (second last): {numbers[^2]}");
        Console.WriteLine($"  numbers[^5]: {numbers[^5]}");

        Console.WriteLine("\n✏️  Modifying with Index:");
        numbers[^1] = 999;  // Set last element

        Console.WriteLine("\nModified array:");
        numbers.Display();

        Console.WriteLine("\n💡 RANGE/INDEX INDEXER BENEFITS:");
        Console.WriteLine("   ✅ Range syntax: [start..end]");
        Console.WriteLine("   ✅ From end: [^n] means n-th from end");
        Console.WriteLine("   ✅ Open ranges: [..n], [n..], [..]");
        Console.WriteLine("   ✅ Negative indexing without manual calculation");
    }

    /// <summary>
    /// ✅ PRODUCTION: Sparse matrix (memory efficient)
    /// </summary>
    static void DemonstrateSparseMatrix()
    {
        Console.WriteLine("🎯 Sparse Matrix - Memory Efficient Storage\n");
        Console.WriteLine("Use case: Large matrices with many zero values\n");

        // Create a large sparse matrix (1000×1000)
        SparseMatrix sparse = new SparseMatrix(1000, 1000);

        Console.WriteLine("Setting only a few non-zero values in 1000×1000 matrix:");

        // Set only a few values (most remain 0)
        sparse[0, 0] = 100;
        sparse[10, 20] = 50;
        sparse[100, 200] = 75;
        sparse[500, 500] = 42;
        sparse[999, 999] = 1;

        sparse.Display();

        Console.WriteLine("\n📋 Reading values:");
        Console.WriteLine($"  sparse[0, 0] = {sparse[0, 0]}");
        Console.WriteLine($"  sparse[10, 20] = {sparse[10, 20]}");
        Console.WriteLine($"  sparse[5, 5] (not set) = {sparse[5, 5]}");  // Returns 0

        Console.WriteLine("\n💾 Memory Analysis:");
        int totalElements = 1000 * 1000;
        int nonZeroElements = sparse.NonZeroCount;
        double memoryUsed = ((double)nonZeroElements / totalElements) * 100;

        Console.WriteLine($"  Total possible elements: {totalElements:N0}");
        Console.WriteLine($"  Non-zero elements: {nonZeroElements}");
        Console.WriteLine($"  Memory used: {memoryUsed:F3}% (vs 100% for dense matrix)");
        Console.WriteLine($"  Memory saved: ~{(1.0 - memoryUsed / 100) * 100:F1}%");

        Console.WriteLine("\n✏️  Setting value to 0 (removes from storage):");
        sparse[10, 20] = 0;  // Remove from storage

        Console.WriteLine($"  Non-zero elements after removal: {sparse.NonZeroCount}");

        Console.WriteLine("\n💡 SPARSE MATRIX BENEFITS:");
        Console.WriteLine("   ✅ Huge memory savings for sparse data");
        Console.WriteLine("   ✅ Dictionary-backed storage (only non-zero values)");
        Console.WriteLine("   ✅ Same indexer syntax as regular matrix");
        Console.WriteLine("   ✅ Default value (0) for unset elements");
        Console.WriteLine("   ✅ Perfect for graphs, scientific computing");
    }

    /// <summary>
    /// ✅ PRODUCTION: DateTime indexer for time series
    /// </summary>
    static void DemonstrateTimeSeriesIndexer()
    {
        Console.WriteLine("🎯 DateTime Indexer - Time Series Data\n");
        Console.WriteLine("Use case: Stock prices, sensor data, logs\n");

        TimeSeries stockPrices = new TimeSeries();

        // Add stock prices for different timestamps
        Console.WriteLine("Adding stock prices:");
        DateTime baseDate = new DateTime(2024, 1, 1, 9, 0, 0);

        for (int i = 0; i < 10; i++)
        {
            DateTime timestamp = baseDate.AddHours(i);
            decimal price = 100m + (decimal)(i * 2.5);
            stockPrices[timestamp] = price;
        }

        // Read specific timestamps
        Console.WriteLine("\n📋 Reading specific timestamps:");
        Console.WriteLine($"  Price at 09:00: {stockPrices[baseDate]:C}");
        Console.WriteLine($"  Price at 12:00: {stockPrices[baseDate.AddHours(3)]:C}");
        Console.WriteLine($"  Price at 18:00: {stockPrices[baseDate.AddHours(9)]:C}");

        // Range query (between two dates)
        Console.WriteLine("\n📊 Range query (10:00 to 14:00):");
        DateTime start = baseDate.AddHours(1);
        DateTime end = baseDate.AddHours(5);
        stockPrices.DisplayRange(start, end);

        // Update existing value
        Console.WriteLine("\n✏️  Updating price:");
        stockPrices[baseDate.AddHours(5)] = 125.75m;

        // Missing timestamp (returns 0)
        Console.WriteLine("\n🔍 Missing timestamp:");
        DateTime missingTime = baseDate.AddDays(1);
        Console.WriteLine($"  Price at {missingTime:yyyy-MM-dd HH:mm}: {stockPrices[missingTime]:C} (not exists)");

        Console.WriteLine($"\n📊 Total data points: {stockPrices.Count}");

        Console.WriteLine("\n💡 DATETIME INDEXER BENEFITS:");
        Console.WriteLine("   ✅ Natural time-based access: series[timestamp]");
        Console.WriteLine("   ✅ Range queries: series[start, end]");
        Console.WriteLine("   ✅ Sorted storage (SortedDictionary)");
        Console.WriteLine("   ✅ Perfect for time-series analysis");
        Console.WriteLine("   ✅ Stock prices, sensor data, logs");
    }
}
