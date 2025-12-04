namespace InterfaceBasics;

/// <summary>
/// SQL Server veritabanı - Implicit interface implementation
/// </summary>
public class SqlDatabase : IDatabase
{
    private bool _isConnected;
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// ✅ Implicit implementation - public metod
    /// Hem interface hem de class referansı üzerinden çağrılabilir
    /// </summary>
    public void Connect()
    {
        Console.WriteLine($"📡 SQL Server'a bağlanılıyor: {ConnectionString}");
        _isConnected = true;
        Console.WriteLine("✅ SQL bağlantısı başarılı");
    }

    public void ExecuteQuery(string sql)
    {
        if (!_isConnected)
        {
            Console.WriteLine("❌ Hata: Önce bağlantı kurmalısınız!");
            return;
        }

        Console.WriteLine($"🔍 SQL Query çalıştırılıyor: {sql}");
        Console.WriteLine("✅ Query başarılı");
    }

    public void Disconnect()
    {
        Console.WriteLine("🔌 SQL bağlantısı kapatılıyor...");
        _isConnected = false;
        Console.WriteLine("✅ Bağlantı kapatıldı");
    }

    // Database'e özgü ek metod (interface'de yok)
    public void ExecuteStoredProcedure(string procedureName)
    {
        Console.WriteLine($"⚙️  Stored Procedure çağrılıyor: {procedureName}");
    }
}
