namespace InterfaceBasics;

/// <summary>
/// PostgreSQL database implementation - Implicit interface implementation
/// </summary>
public class PostgreSqlDatabase : IDatabase
{
    public string ConnectionString { get; set; } = string.Empty;
    private bool _isConnected = false;

    public void Connect()
    {
        Console.WriteLine($"[PostgreSQL] Connecting to: {ConnectionString}");
        Console.WriteLine("[PostgreSQL] Using Npgsql driver...");
        _isConnected = true;
        Console.WriteLine("[PostgreSQL] ✅ Connected successfully!");
    }

    public void ExecuteQuery(string sql)
    {
        if (!_isConnected)
        {
            Console.WriteLine("[PostgreSQL] ❌ Error: Not connected!");
            return;
        }

        Console.WriteLine($"[PostgreSQL] Executing query: {sql}");
        Console.WriteLine("[PostgreSQL] Query executed successfully.");
        Console.WriteLine("[PostgreSQL] Rows affected: 5");
    }

    public void Disconnect()
    {
        if (_isConnected)
        {
            Console.WriteLine("[PostgreSQL] Closing connection...");
            _isConnected = false;
            Console.WriteLine("[PostgreSQL] ✅ Disconnected!");
        }
    }

    // PostgreSQL-specific method (not in interface)
    public void CreateSchema(string schemaName)
    {
        Console.WriteLine($"[PostgreSQL] Creating schema: {schemaName}");
        Console.WriteLine($"[PostgreSQL] CREATE SCHEMA {schemaName};");
    }
}
