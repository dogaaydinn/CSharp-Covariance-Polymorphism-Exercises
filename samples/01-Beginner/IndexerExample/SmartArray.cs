namespace IndexerExample;

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
        }
    }

    // Range indexer - Slice access
    public List<T> this[Range range]
    {
        get
        {
            var (start, length) = range.GetOffsetAndLength(_items.Count);
            return _items.GetRange(start, length);
        }
    }

    public void Add(T item) => _items.Add(item);
    public int Count => _items.Count;
}

public class Dictionary2D<TValue>
{
    private readonly Dictionary<string, TValue> _data = new();

    // Multi-parameter indexer - 2D access
    public TValue this[int row, int col]
    {
        get
        {
            string key = $"{row},{col}";
            return _data.TryGetValue(key, out var value)
                ? value
                : throw new KeyNotFoundException($"Key ({row},{col}) not found");
        }
        set
        {
            string key = $"{row},{col}";
            _data[key] = value;
        }
    }

    public int Count => _data.Count;
}

public class StudentGrades
{
    private readonly Dictionary<string, int> _grades = new();

    // String indexer - Dictionary-like access
    public int this[string studentName]
    {
        get => _grades.TryGetValue(studentName, out var grade) ? grade : 0;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentException("Not 0-100 arasında olmalı!");
            _grades[studentName] = value;
        }
    }

    public IEnumerable<string> Students => _grades.Keys;
}

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

    // 2D indexer with bounds checking
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
