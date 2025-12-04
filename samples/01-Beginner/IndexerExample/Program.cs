// SCENARIO: Custom indexers - Array-like, Dictionary-like, Multi-dimensional
// BAD PRACTICE: GetItem(index), SetItem(index, value) metodları
// GOOD PRACTICE: Indexer ile array/dictionary syntax

using IndexerExample;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Indexer Patterns ===\n");

        Console.WriteLine("=== 1. Integer Indexer (Array-like) ===\n");
        DemonstrateIntIndexer();

        Console.WriteLine("\n=== 2. String Indexer (Dictionary-like) ===\n");
        DemonstrateStringIndexer();

        Console.WriteLine("\n=== 3. Multi-Parameter Indexer (2D) ===\n");
        DemonstrateMultiIndexer();

        Console.WriteLine("\n=== 4. Range Indexer (Slice) ===\n");
        DemonstrateRangeIndexer();

        Console.WriteLine("\n=== Analysis ===");
        Console.WriteLine("• Indexer: this[index] syntax");
        Console.WriteLine("• Multiple parameters: this[row, col]");
        Console.WriteLine("• Different types: int, string, Range");
        Console.WriteLine("• Enables array/dictionary-like access");
    }

    static void DemonstrateIntIndexer()
    {
        var array = new SmartArray<string>();
        array.Add("Apple");
        array.Add("Banana");
        array.Add("Cherry");

        // Array-like access
        Console.WriteLine($"array[0]: {array[0]}");
        Console.WriteLine($"array[1]: {array[1]}");

        // Modification
        array[1] = "Blueberry";
        Console.WriteLine($"array[1] after change: {array[1]}");

        // Bounds checking
        try
        {
            Console.WriteLine($"\narray[10]: {array[10]}");
        }
        catch (IndexOutOfRangeException ex)
        {
            Console.WriteLine($"❌ {ex.Message}");
        }
    }

    static void DemonstrateStringIndexer()
    {
        var grades = new StudentGrades();
        grades["Ali"] = 85;
        grades["Ayşe"] = 92;
        grades["Mehmet"] = 78;

        Console.WriteLine("Notlar:");
        foreach (var student in grades.Students)
        {
            Console.WriteLine($"  {student}: {grades[student]}");
        }

        // Default value for missing key
        Console.WriteLine($"\nZeynep (yok): {grades["Zeynep"]}");

        // Validation
        try
        {
            Console.WriteLine("\nAli'nin notunu 150 yapılıyor...");
            grades["Ali"] = 150;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"❌ {ex.Message}");
        }
    }

    static void DemonstrateMultiIndexer()
    {
        var matrix = new Matrix(3, 3);

        // Fill matrix
        Console.WriteLine("Matrix oluşturuluyor:");
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                matrix[i, j] = i * 3 + j + 1;
            }
        }

        // Display matrix
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Console.Write($"{matrix[i, j],3} ");
            }
            Console.WriteLine();
        }

        Console.WriteLine($"\nmatrix[1, 1]: {matrix[1, 1]}");
    }

    static void DemonstrateRangeIndexer()
    {
        var array = new SmartArray<int>();
        for (int i = 0; i < 10; i++)
            array.Add(i);

        // Range access (slice)
        var slice1 = array[2..5];   // [2, 3, 4]
        var slice2 = array[^3..];   // Son 3 eleman

        Console.WriteLine($"array[2..5]: {string.Join(", ", slice1)}");
        Console.WriteLine($"array[^3..]: {string.Join(", ", slice2)}");
    }
}
