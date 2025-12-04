namespace InterfaceBasics;

/// <summary>
/// İlişkisel veritabanı interface'i (SQL)
/// </summary>
public interface IDatabase
{
    void Connect();
    void ExecuteQuery(string sql);
    void Disconnect();

    // C# 8+ Default interface implementation
    void LogOperation(string operation)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {operation}");
    }
}

/// <summary>
/// NoSQL veritabanı interface'i
/// </summary>
public interface INoSqlDatabase
{
    void Connect();  // Aynı metod adı - Explicit implementation gerekir!
    void InsertDocument(string json);
    void Disconnect();
}

/// <summary>
/// Cache interface'i - Çoklu interface implementation örneği
/// </summary>
public interface ICacheProvider
{
    void Set(string key, object value);
    object? Get(string key);
    void Remove(string key);
}
