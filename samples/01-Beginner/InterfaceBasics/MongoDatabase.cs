namespace InterfaceBasics;

/// <summary>
/// MongoDB veritabanı - Explicit interface implementation
/// İki interface'de aynı metod adı var (Connect), çakışmayı önlemek için explicit kullan
/// </summary>
public class MongoDatabase : IDatabase, INoSqlDatabase, ICacheProvider
{
    private bool _isConnected;
    private readonly Dictionary<string, object> _cache = new();
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// ❌ EXPLICIT IMPLEMENTATION - Sadece IDatabase referansı üzerinden çağrılabilir
    /// MongoDatabase instance üzerinden çağrılamaz!
    /// </summary>
    void IDatabase.Connect()
    {
        Console.WriteLine($"📡 MongoDB'ye (SQL modunda) bağlanılıyor: {ConnectionString}");
        _isConnected = true;
        Console.WriteLine("✅ SQL-like bağlantı başarılı");
    }

    /// <summary>
    /// ❌ EXPLICIT IMPLEMENTATION - Sadece INoSqlDatabase referansı üzerinden çağrılabilir
    /// </summary>
    void INoSqlDatabase.Connect()
    {
        Console.WriteLine($"📡 MongoDB'ye (NoSQL modunda) bağlanılıyor: {ConnectionString}");
        _isConnected = true;
        Console.WriteLine("✅ NoSQL bağlantı başarılı");
    }

    // ✅ Implicit implementation - Her referanstan çağrılabilir
    public void Disconnect()
    {
        Console.WriteLine("🔌 MongoDB bağlantısı kapatılıyor...");
        _isConnected = false;
        Console.WriteLine("✅ Bağlantı kapatıldı");
    }

    // IDatabase.ExecuteQuery - Explicit
    void IDatabase.ExecuteQuery(string sql)
    {
        if (!_isConnected)
        {
            Console.WriteLine("❌ Hata: Önce bağlantı kurmalısınız!");
            return;
        }

        Console.WriteLine($"🔍 MongoDB Query (SQL syntax): {sql}");
        Console.WriteLine("✅ Query başarılı");
    }

    // INoSqlDatabase.InsertDocument - Implicit
    public void InsertDocument(string json)
    {
        if (!_isConnected)
        {
            Console.WriteLine("❌ Hata: Önce bağlantı kurmalısınız!");
            return;
        }

        Console.WriteLine($"📝 Document ekleniyor: {json.Substring(0, Math.Min(50, json.Length))}...");
        Console.WriteLine("✅ Document eklendi");
    }

    // ICacheProvider implementation - Explicit
    void ICacheProvider.Set(string key, object value)
    {
        _cache[key] = value;
        Console.WriteLine($"💾 Cache'e eklendi: {key}");
    }

    object? ICacheProvider.Get(string key)
    {
        if (_cache.TryGetValue(key, out var value))
        {
            Console.WriteLine($"✅ Cache'den okundu: {key}");
            return value;
        }

        Console.WriteLine($"❌ Cache'de bulunamadı: {key}");
        return null;
    }

    void ICacheProvider.Remove(string key)
    {
        if (_cache.Remove(key))
        {
            Console.WriteLine($"🗑️  Cache'den silindi: {key}");
        }
    }

    // MongoDB'ye özgü public metod (interface'lerde yok)
    public void CreateIndex(string fieldName)
    {
        Console.WriteLine($"📇 Index oluşturuluyor: {fieldName}");
    }
}
