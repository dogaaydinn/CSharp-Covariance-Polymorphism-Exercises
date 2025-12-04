using System;
using System.Collections.Generic;
using System.Threading;

namespace DesignPatterns.Structural;

/// <summary>
/// PROXY PATTERN - Provides a surrogate or placeholder to control access to an object
///
/// Problem:
/// - Need to control access to an object
/// - Want to add functionality before/after accessing object
/// - Object creation is expensive (lazy loading)
/// - Need to add logging, caching, or access control
///
/// UML Structure:
/// ┌──────────────┐
/// │   Subject    │ (interface)
/// └──────────────┘
///        △
///        │ implements
///        │
/// ┌──────┴──────────────────────┐
/// │                              │
/// ┌──────────────┐        ┌─────┴──────┐
/// │  RealSubject │◄───────│   Proxy    │
/// └──────────────┘        └────────────┘
///
/// Types of Proxies:
/// 1. Virtual Proxy - Lazy loading, creates object on demand
/// 2. Protection Proxy - Controls access based on permissions
/// 3. Remote Proxy - Represents object in different address space
/// 4. Caching Proxy - Caches results to improve performance
///
/// When to Use:
/// - Lazy initialization (virtual proxy)
/// - Access control (protection proxy)
/// - Local representative of remote object (remote proxy)
/// - Logging, caching, or monitoring access
///
/// Benefits:
/// - Control object access
/// - Add functionality without changing object
/// - Lazy initialization
/// - Open/Closed Principle
/// </summary>

#region Virtual Proxy (Lazy Loading)

/// <summary>
/// Subject interface
/// </summary>
public interface IImage
{
    void Display();
    void Resize(int width, int height);
    string GetInfo();
}

/// <summary>
/// Real subject - Expensive object to create
/// </summary>
public class HighResolutionImage : IImage
{
    private readonly string _filename;
    private readonly int _width;
    private readonly int _height;
    private byte[] _imageData;

    public HighResolutionImage(string filename)
    {
        _filename = filename;
        Console.WriteLine($"  [Proxy] Loading high-resolution image: {filename}");
        Thread.Sleep(1000); // Simulate expensive loading operation
        _width = 4096;
        _height = 2160;
        _imageData = new byte[_width * _height * 4]; // Simulate large image data
        Console.WriteLine($"  [Proxy] Loaded {_imageData.Length / 1024 / 1024}MB image data");
    }

    public void Display()
    {
        Console.WriteLine($"  [Proxy] Displaying {_filename} ({_width}x{_height})");
    }

    public void Resize(int width, int height)
    {
        Console.WriteLine($"  [Proxy] Resizing {_filename} to {width}x{height}");
        _imageData = new byte[width * height * 4];
    }

    public string GetInfo() => $"{_filename} - {_width}x{_height}";
}

/// <summary>
/// Virtual Proxy - Delays loading until needed
/// </summary>
public class ImageProxy : IImage
{
    private readonly string _filename;
    private HighResolutionImage? _realImage;

    public ImageProxy(string filename)
    {
        _filename = filename;
        Console.WriteLine($"  [Proxy] Created proxy for: {filename} (not loaded yet)");
    }

    public void Display()
    {
        EnsureImageLoaded();
        _realImage!.Display();
    }

    public void Resize(int width, int height)
    {
        EnsureImageLoaded();
        _realImage!.Resize(width, height);
    }

    public string GetInfo()
    {
        // Can return info without loading the full image
        if (_realImage == null)
        {
            return $"{_filename} - Not loaded yet";
        }
        return _realImage.GetInfo();
    }

    private void EnsureImageLoaded()
    {
        if (_realImage == null)
        {
            Console.WriteLine($"  [Proxy] Lazy loading triggered for: {_filename}");
            _realImage = new HighResolutionImage(_filename);
        }
    }
}

#endregion

#region Protection Proxy (Access Control)

/// <summary>
/// Subject interface for documents
/// </summary>
public interface IDocument
{
    string Read();
    void Write(string content);
    void Delete();
}

/// <summary>
/// Real subject
/// </summary>
public class Document : IDocument
{
    private readonly string _name;
    private string _content;

    public Document(string name, string content)
    {
        _name = name;
        _content = content;
    }

    public string Read()
    {
        Console.WriteLine($"  [Proxy] Reading document: {_name}");
        return _content;
    }

    public void Write(string content)
    {
        Console.WriteLine($"  [Proxy] Writing to document: {_name}");
        _content = content;
    }

    public void Delete()
    {
        Console.WriteLine($"  [Proxy] Deleting document: {_name}");
        _content = string.Empty;
    }
}

/// <summary>
/// User roles for access control
/// </summary>
public enum UserRole
{
    Guest,
    User,
    Admin
}

/// <summary>
/// Protection Proxy - Controls access based on user role
/// </summary>
public class ProtectedDocument : IDocument
{
    private readonly Document _document;
    private readonly UserRole _userRole;
    private readonly string _documentName;

    public ProtectedDocument(Document document, UserRole userRole, string documentName)
    {
        _document = document ?? throw new ArgumentNullException(nameof(document));
        _userRole = userRole;
        _documentName = documentName;
    }

    public string Read()
    {
        Console.WriteLine($"  [Proxy] Access check for READ (User: {_userRole})");

        if (_userRole == UserRole.Guest)
        {
            Console.WriteLine($"  [Proxy] Access DENIED - Guests cannot read {_documentName}");
            return "[ACCESS DENIED]";
        }

        Console.WriteLine("  [Proxy] Access GRANTED");
        return _document.Read();
    }

    public void Write(string content)
    {
        Console.WriteLine($"  [Proxy] Access check for WRITE (User: {_userRole})");

        if (_userRole == UserRole.Guest || _userRole == UserRole.User)
        {
            Console.WriteLine($"  [Proxy] Access DENIED - Only admins can write to {_documentName}");
            throw new UnauthorizedAccessException("Insufficient permissions to write");
        }

        Console.WriteLine("  [Proxy] Access GRANTED");
        _document.Write(content);
    }

    public void Delete()
    {
        Console.WriteLine($"  [Proxy] Access check for DELETE (User: {_userRole})");

        if (_userRole != UserRole.Admin)
        {
            Console.WriteLine($"  [Proxy] Access DENIED - Only admins can delete {_documentName}");
            throw new UnauthorizedAccessException("Insufficient permissions to delete");
        }

        Console.WriteLine("  [Proxy] Access GRANTED");
        _document.Delete();
    }
}

#endregion

#region Caching Proxy

/// <summary>
/// Subject interface for expensive computations
/// </summary>
public interface IDataService
{
    string GetData(string key);
    List<string> SearchData(string query);
}

/// <summary>
/// Real subject - Expensive data service
/// </summary>
public class DatabaseService : IDataService
{
    public string GetData(string key)
    {
        Console.WriteLine($"  [Proxy] Database query for key: {key}");
        Thread.Sleep(500); // Simulate slow database query
        return $"Data for {key} from database";
    }

    public List<string> SearchData(string query)
    {
        Console.WriteLine($"  [Proxy] Database search for: {query}");
        Thread.Sleep(1000); // Simulate slow search
        return new List<string>
        {
            $"Result 1 for {query}",
            $"Result 2 for {query}",
            $"Result 3 for {query}"
        };
    }
}

/// <summary>
/// Caching Proxy - Improves performance with caching
/// </summary>
public class CachingDataServiceProxy : IDataService
{
    private readonly DatabaseService _databaseService;
    private readonly Dictionary<string, string> _cache = new();
    private readonly Dictionary<string, List<string>> _searchCache = new();
    private readonly Dictionary<string, DateTime> _cacheTimestamps = new();
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromSeconds(30);

    public CachingDataServiceProxy(DatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    public string GetData(string key)
    {
        // Check if data is in cache and not expired
        if (_cache.ContainsKey(key) &&
            _cacheTimestamps.ContainsKey(key) &&
            DateTime.Now - _cacheTimestamps[key] < _cacheExpiration)
        {
            Console.WriteLine($"  [Proxy] Cache HIT for key: {key}");
            return _cache[key];
        }

        Console.WriteLine($"  [Proxy] Cache MISS for key: {key}");

        // Get from database and cache it
        string data = _databaseService.GetData(key);
        _cache[key] = data;
        _cacheTimestamps[key] = DateTime.Now;

        return data;
    }

    public List<string> SearchData(string query)
    {
        // Check cache
        if (_searchCache.ContainsKey(query) &&
            _cacheTimestamps.ContainsKey($"search_{query}") &&
            DateTime.Now - _cacheTimestamps[$"search_{query}"] < _cacheExpiration)
        {
            Console.WriteLine($"  [Proxy] Search cache HIT for: {query}");
            return new List<string>(_searchCache[query]);
        }

        Console.WriteLine($"  [Proxy] Search cache MISS for: {query}");

        // Get from database and cache it
        var results = _databaseService.SearchData(query);
        _searchCache[query] = results;
        _cacheTimestamps[$"search_{query}"] = DateTime.Now;

        return results;
    }

    public void ClearCache()
    {
        Console.WriteLine("  [Proxy] Clearing all caches");
        _cache.Clear();
        _searchCache.Clear();
        _cacheTimestamps.Clear();
    }

    public int GetCacheSize() => _cache.Count + _searchCache.Count;
}

#endregion

#region Logging Proxy

/// <summary>
/// Subject interface for business operations
/// </summary>
public interface IUserService
{
    void CreateUser(string username, string email);
    void UpdateUser(string username, string email);
    void DeleteUser(string username);
}

/// <summary>
/// Real subject
/// </summary>
public class UserService : IUserService
{
    public void CreateUser(string username, string email)
    {
        Console.WriteLine($"  [Proxy] Creating user: {username}");
    }

    public void UpdateUser(string username, string email)
    {
        Console.WriteLine($"  [Proxy] Updating user: {username}");
    }

    public void DeleteUser(string username)
    {
        Console.WriteLine($"  [Proxy] Deleting user: {username}");
    }
}

/// <summary>
/// Logging Proxy - Adds logging to all operations
/// </summary>
public class LoggingUserServiceProxy : IUserService
{
    private readonly UserService _userService;
    private readonly List<string> _auditLog = new();

    public LoggingUserServiceProxy(UserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public void CreateUser(string username, string email)
    {
        LogOperation("CREATE", $"User: {username}, Email: {email}");
        _userService.CreateUser(username, email);
    }

    public void UpdateUser(string username, string email)
    {
        LogOperation("UPDATE", $"User: {username}, Email: {email}");
        _userService.UpdateUser(username, email);
    }

    public void DeleteUser(string username)
    {
        LogOperation("DELETE", $"User: {username}");
        _userService.DeleteUser(username);
    }

    private void LogOperation(string operation, string details)
    {
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {operation}: {details}";
        _auditLog.Add(logEntry);
        Console.WriteLine($"  [Proxy] Logged: {logEntry}");
    }

    public List<string> GetAuditLog() => new List<string>(_auditLog);
}

#endregion

/// <summary>
/// Example demonstrating Proxy pattern
/// </summary>
public static class ProxyExample
{
    public static void Run()
    {
        Console.WriteLine();
        Console.WriteLine("6. PROXY PATTERN - Controls access to objects");
        Console.WriteLine("-".PadRight(70, '-'));
        Console.WriteLine();

        // Example 1: Virtual Proxy (Lazy Loading)
        Console.WriteLine("Example 1: Virtual Proxy - Lazy Loading Images");
        Console.WriteLine();

        var images = new List<IImage>
        {
            new ImageProxy("photo1.jpg"),
            new ImageProxy("photo2.jpg"),
            new ImageProxy("photo3.jpg")
        };

        Console.WriteLine("  Created 3 image proxies (images not loaded yet)");
        Console.WriteLine();

        // Get info without loading
        Console.WriteLine("  Getting info (no loading required):");
        foreach (var image in images)
        {
            Console.WriteLine($"    {image.GetInfo()}");
        }

        Console.WriteLine();
        Console.WriteLine("  Displaying first image (triggers loading):");
        images[0].Display();

        Console.WriteLine();

        // Example 2: Protection Proxy (Access Control)
        Console.WriteLine("Example 2: Protection Proxy - Access Control");
        Console.WriteLine();

        var document = new Document("confidential.txt", "Top secret information");

        // Guest user
        Console.WriteLine("  Guest user attempting access:");
        var guestDoc = new ProtectedDocument(document, UserRole.Guest, "confidential.txt");
        var guestContent = guestDoc.Read();
        Console.WriteLine($"  Result: {guestContent}");

        Console.WriteLine();

        // Regular user
        Console.WriteLine("  Regular user attempting access:");
        var userDoc = new ProtectedDocument(document, UserRole.User, "confidential.txt");
        userDoc.Read();
        try
        {
            userDoc.Write("Trying to modify");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"  Exception: {ex.Message}");
        }

        Console.WriteLine();

        // Admin user
        Console.WriteLine("  Admin user attempting access:");
        var adminDoc = new ProtectedDocument(document, UserRole.Admin, "confidential.txt");
        adminDoc.Read();
        adminDoc.Write("Modified by admin");
        adminDoc.Delete();

        Console.WriteLine();

        // Example 3: Caching Proxy
        Console.WriteLine("Example 3: Caching Proxy - Performance Optimization");
        Console.WriteLine();

        var dbService = new DatabaseService();
        var cachedService = new CachingDataServiceProxy(dbService);

        Console.WriteLine("  First request (cache miss):");
        cachedService.GetData("user_123");

        Console.WriteLine();
        Console.WriteLine("  Second request (cache hit):");
        cachedService.GetData("user_123");

        Console.WriteLine();
        Console.WriteLine("  Search query (cache miss):");
        var results = cachedService.SearchData("design patterns");

        Console.WriteLine();
        Console.WriteLine("  Same search again (cache hit):");
        cachedService.SearchData("design patterns");

        Console.WriteLine($"  Cache size: {cachedService.GetCacheSize()} items");

        Console.WriteLine();

        // Example 4: Logging Proxy
        Console.WriteLine("Example 4: Logging Proxy - Audit Trail");
        Console.WriteLine();

        var userService = new UserService();
        var loggedService = new LoggingUserServiceProxy(userService);

        loggedService.CreateUser("john_doe", "john@example.com");
        loggedService.UpdateUser("john_doe", "john.doe@example.com");
        loggedService.DeleteUser("john_doe");

        Console.WriteLine();
        Console.WriteLine("  Audit log entries:");
        foreach (var entry in loggedService.GetAuditLog())
        {
            Console.WriteLine($"    {entry}");
        }

        Console.WriteLine();
        Console.WriteLine("  Key Benefit: Control access and add functionality transparently!");
    }
}
