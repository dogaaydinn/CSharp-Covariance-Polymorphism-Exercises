using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

// ═══════════════════════════════════════════════════════════
// EXTENSION METHODS - UTILITY EXTENSION METHODS
// ═══════════════════════════════════════════════════════════
// This example demonstrates various extension methods for:
// - String manipulation (email validation, title case, slug)
// - Collection operations (batching, shuffling, chunking)
// - Enum utilities (descriptions, display names)
// - DateTime helpers (weekends, date ranges)
// - Numeric utilities (even/odd, clamping, percentages)
// - LINQ-style extensions (conditional filtering, indexing)
// ═══════════════════════════════════════════════════════════

#region Models and Enums

/// <summary>
/// Priority levels with description attributes
/// </summary>
public enum Priority
{
    [Description("Low priority - can wait")]
    Low = 1,

    [Description("Normal priority - standard processing")]
    Normal = 2,

    [Description("High priority - urgent")]
    High = 3,

    [Description("Critical priority - immediate action required")]
    Critical = 4
}

/// <summary>
/// Order status for demonstrations
/// </summary>
public enum OrderStatus
{
    [Description("Order is pending")]
    Pending,

    [Description("Order is being processed")]
    Processing,

    [Description("Order has been shipped")]
    Shipped,

    [Description("Order has been delivered")]
    Delivered,

    [Description("Order was cancelled")]
    Cancelled
}

/// <summary>
/// Sample product class for demonstrations
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool InStock { get; set; }
}

#endregion

#region 1. STRING EXTENSIONS

/// <summary>
/// Extension methods for string manipulation
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Validates if a string is a valid email address
    /// </summary>
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    /// <summary>
    /// Converts string to Title Case (first letter of each word capitalized)
    /// </summary>
    public static string ToTitleCase(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(text.ToLower());
    }

    /// <summary>
    /// Truncates string to specified length and adds ellipsis
    /// </summary>
    public static string Truncate(this string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            return text;

        return text[..maxLength] + "...";
    }

    /// <summary>
    /// Converts string to URL-friendly slug
    /// </summary>
    public static string ToSlug(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Convert to lowercase
        text = text.ToLowerInvariant();

        // Remove special characters
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

        // Replace spaces with hyphens
        text = Regex.Replace(text, @"\s+", "-");

        // Remove consecutive hyphens
        text = Regex.Replace(text, @"-+", "-");

        return text.Trim('-');
    }

    /// <summary>
    /// Removes all whitespace from string
    /// </summary>
    public static string RemoveWhitespace(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return Regex.Replace(text, @"\s+", "");
    }

    /// <summary>
    /// Reverses the string
    /// </summary>
    public static string Reverse(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var charArray = text.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    /// <summary>
    /// Checks if string contains any of the specified values (case-insensitive)
    /// </summary>
    public static bool ContainsAny(this string text, params string[] values)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        return values.Any(value => text.Contains(value, StringComparison.OrdinalIgnoreCase));
    }
}

#endregion

#region 2. COLLECTION EXTENSIONS

/// <summary>
/// Extension methods for collections (IEnumerable<T>)
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Splits collection into batches of specified size
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (batchSize <= 0)
            throw new ArgumentException("Batch size must be positive", nameof(batchSize));

        var batch = new List<T>(batchSize);

        foreach (var item in source)
        {
            batch.Add(item);

            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }

        if (batch.Count > 0)
            yield return batch;
    }

    /// <summary>
    /// Shuffles collection randomly
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var random = new Random();
        var list = source.ToList();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list;
    }

    /// <summary>
    /// Executes action for each element
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in source)
            action(item);
    }

    /// <summary>
    /// Checks if collection is null or empty
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }

    /// <summary>
    /// Returns distinct elements using a key selector
    /// </summary>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var seenKeys = new HashSet<TKey>();

        foreach (var item in source)
        {
            if (seenKeys.Add(keySelector(item)))
                yield return item;
        }
    }

    /// <summary>
    /// Chunks collection into specified size groups
    /// </summary>
    public static IEnumerable<List<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (chunkSize <= 0)
            throw new ArgumentException("Chunk size must be positive", nameof(chunkSize));

        var chunk = new List<T>(chunkSize);

        foreach (var item in source)
        {
            chunk.Add(item);

            if (chunk.Count == chunkSize)
            {
                yield return chunk;
                chunk = new List<T>(chunkSize);
            }
        }

        if (chunk.Count > 0)
            yield return chunk;
    }
}

#endregion

#region 3. ENUM EXTENSIONS

/// <summary>
/// Extension methods for enums
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the Description attribute value from enum
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());

        if (field == null)
            return value.ToString();

        var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// Gets the display name (friendly name) from enum
    /// </summary>
    public static string GetDisplayName(this Enum value)
    {
        return value.ToString().Replace("_", " ");
    }

    /// <summary>
    /// Converts enum to list of all values
    /// </summary>
    public static List<T> ToList<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }

    /// <summary>
    /// Checks if enum has specific flag set
    /// </summary>
    public static bool HasFlag<T>(this T value, T flag) where T : Enum
    {
        var valueInt = Convert.ToInt32(value);
        var flagInt = Convert.ToInt32(flag);
        return (valueInt & flagInt) == flagInt;
    }
}

#endregion

#region 4. DATETIME EXTENSIONS

/// <summary>
/// Extension methods for DateTime
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Checks if date is weekend (Saturday or Sunday)
    /// </summary>
    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    /// <summary>
    /// Gets the start of the week (Monday)
    /// </summary>
    public static DateTime StartOfWeek(this DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Gets the end of the month
    /// </summary>
    public static DateTime EndOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
    }

    /// <summary>
    /// Calculates age from birth date
    /// </summary>
    public static int Age(this DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age))
            age--;

        return age;
    }

    /// <summary>
    /// Gets the start of day (00:00:00)
    /// </summary>
    public static DateTime StartOfDay(this DateTime date)
    {
        return date.Date;
    }

    /// <summary>
    /// Gets the end of day (23:59:59)
    /// </summary>
    public static DateTime EndOfDay(this DateTime date)
    {
        return date.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Checks if date is today
    /// </summary>
    public static bool IsToday(this DateTime date)
    {
        return date.Date == DateTime.Today;
    }
}

#endregion

#region 5. NUMERIC EXTENSIONS

/// <summary>
/// Extension methods for numeric types
/// </summary>
public static class NumericExtensions
{
    /// <summary>
    /// Checks if number is even
    /// </summary>
    public static bool IsEven(this int number)
    {
        return number % 2 == 0;
    }

    /// <summary>
    /// Checks if number is odd
    /// </summary>
    public static bool IsOdd(this int number)
    {
        return number % 2 != 0;
    }

    /// <summary>
    /// Clamps number between min and max values
    /// </summary>
    public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0)
            return min;
        if (value.CompareTo(max) > 0)
            return max;
        return value;
    }

    /// <summary>
    /// Converts decimal to percentage string
    /// </summary>
    public static string ToPercentage(this decimal value, int decimals = 2)
    {
        var format = $"F{decimals}";
        return $"{(value * 100).ToString(format)}%";
    }

    /// <summary>
    /// Checks if number is in range
    /// </summary>
    public static bool InRange<T>(this T value, T min, T max) where T : IComparable<T>
    {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }

    /// <summary>
    /// Converts number to ordinal string (1st, 2nd, 3rd, etc.)
    /// </summary>
    public static string ToOrdinal(this int number)
    {
        if (number <= 0) return number.ToString();

        switch (number % 100)
        {
            case 11:
            case 12:
            case 13:
                return $"{number}th";
        }

        return (number % 10) switch
        {
            1 => $"{number}st",
            2 => $"{number}nd",
            3 => $"{number}rd",
            _ => $"{number}th"
        };
    }
}

#endregion

#region 6. LINQ EXTENSIONS

/// <summary>
/// LINQ-style extension methods
/// </summary>
public static class LinqExtensions
{
    /// <summary>
    /// Conditionally filters sequence
    /// </summary>
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        return condition ? source.Where(predicate) : source;
    }

    /// <summary>
    /// Conditionally orders sequence
    /// </summary>
    public static IEnumerable<T> OrderByIf<T, TKey>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        return condition ? source.OrderBy(keySelector) : source;
    }

    /// <summary>
    /// Takes elements until condition is met
    /// </summary>
    public static IEnumerable<T> TakeUntil<T>(
        this IEnumerable<T> source,
        Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        foreach (var item in source)
        {
            yield return item;
            if (predicate(item))
                yield break;
        }
    }

    /// <summary>
    /// Iterates with index
    /// </summary>
    public static void ForEachWithIndex<T>(
        this IEnumerable<T> source,
        Action<T, int> action)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);

        int index = 0;
        foreach (var item in source)
        {
            action(item, index++);
        }
    }

    /// <summary>
    /// Returns default value if sequence is empty
    /// </summary>
    public static IEnumerable<T> DefaultIfEmpty<T>(
        this IEnumerable<T> source,
        IEnumerable<T> defaultValue)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(defaultValue);

        return source.Any() ? source : defaultValue;
    }
}

#endregion

#region MAIN PROGRAM

class Program
{
    static void Main()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   🔧 EXTENSION METHODS - UTILITY EXTENSIONS           ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

        // 1. String Extensions
        DemonstrateStringExtensions();
        Console.WriteLine();

        // 2. Collection Extensions
        DemonstrateCollectionExtensions();
        Console.WriteLine();

        // 3. Enum Extensions
        DemonstrateEnumExtensions();
        Console.WriteLine();

        // 4. DateTime Extensions
        DemonstrateDateTimeExtensions();
        Console.WriteLine();

        // 5. Numeric Extensions
        DemonstrateNumericExtensions();
        Console.WriteLine();

        // 6. LINQ Extensions
        DemonstrateLinqExtensions();
        Console.WriteLine();

        // Summary
        Console.WriteLine("\n✅ ÖĞRENİLENLER:");
        Console.WriteLine("   • String extensions - Email validation, title case, slug");
        Console.WriteLine("   • Collection extensions - Batching, shuffling, chunking");
        Console.WriteLine("   • Enum extensions - Descriptions, display names");
        Console.WriteLine("   • DateTime extensions - Weekend checks, date ranges");
        Console.WriteLine("   • Numeric extensions - Even/odd, clamping, percentages");
        Console.WriteLine("   • LINQ extensions - Conditional filtering, indexing");
    }

    // ═══════════════════════════════════════════════════════════
    // 1. STRING EXTENSIONS DEMONSTRATION
    // ═══════════════════════════════════════════════════════════
    static void DemonstrateStringExtensions()
    {
        Console.WriteLine("═══ 1. String Extensions ═══\n");

        // IsValidEmail
        Console.WriteLine("📧 Email Validation:");
        var emails = new[] { "user@example.com", "invalid.email", "test@domain.co.uk", "@invalid.com" };
        foreach (var email in emails)
        {
            Console.WriteLine($"  {email,-25} → {(email.IsValidEmail() ? "✅ Valid" : "❌ Invalid")}");
        }

        Console.WriteLine("\n📝 Title Case:");
        var texts = new[] { "hello world", "LOUD NOISES", "mixed CaSe TeXt" };
        foreach (var text in texts)
        {
            Console.WriteLine($"  \"{text}\" → \"{text.ToTitleCase()}\"");
        }

        Console.WriteLine("\n✂️ Truncate:");
        var longText = "This is a very long text that needs to be truncated";
        Console.WriteLine($"  Original: \"{longText}\"");
        Console.WriteLine($"  Truncated (20): \"{longText.Truncate(20)}\"");

        Console.WriteLine("\n🔗 URL Slug:");
        var titles = new[] { "Hello World!", "C# Programming Tips", "  Spaces & Special@#$ Chars  " };
        foreach (var title in titles)
        {
            Console.WriteLine($"  \"{title}\" → \"{title.ToSlug()}\"");
        }

        Console.WriteLine("\n🧹 Remove Whitespace:");
        var spacedText = "  This   has   many    spaces  ";
        Console.WriteLine($"  Original: \"{spacedText}\"");
        Console.WriteLine($"  Removed: \"{spacedText.RemoveWhitespace()}\"");

        Console.WriteLine("\n🔄 Reverse:");
        var word = "Hello";
        Console.WriteLine($"  \"{word}\" → \"{word.Reverse()}\"");

        Console.WriteLine("\n🔍 Contains Any:");
        var message = "This is a test message";
        var keywords = new[] { "test", "hello", "world" };
        Console.WriteLine($"  Text: \"{message}\"");
        Console.WriteLine($"  Contains any of [{string.Join(", ", keywords)}]: {message.ContainsAny(keywords)}");
    }

    // ═══════════════════════════════════════════════════════════
    // 2. COLLECTION EXTENSIONS DEMONSTRATION
    // ═══════════════════════════════════════════════════════════
    static void DemonstrateCollectionExtensions()
    {
        Console.WriteLine("═══ 2. Collection Extensions ═══\n");

        var numbers = Enumerable.Range(1, 15).ToList();

        Console.WriteLine("📦 Batch (size 5):");
        var batches = numbers.Batch(5).ToList();
        for (int i = 0; i < batches.Count(); i++)
        {
            Console.WriteLine($"  Batch {i + 1}: [{string.Join(", ", batches.ElementAt(i))}]");
        }

        Console.WriteLine("\n🎲 Shuffle:");
        Console.WriteLine($"  Original: [{string.Join(", ", numbers.Take(10))}]");
        Console.WriteLine($"  Shuffled: [{string.Join(", ", numbers.Take(10).Shuffle())}]");

        Console.WriteLine("\n🔄 ForEach:");
        Console.Write("  Squares: ");
        numbers.Take(5).ForEach(n => Console.Write($"{n * n} "));
        Console.WriteLine();

        Console.WriteLine("\n❓ IsNullOrEmpty:");
        List<int>? nullList = null;
        var emptyList = new List<int>();
        var fullList = new List<int> { 1, 2, 3 };
        Console.WriteLine($"  null list: {nullList.IsNullOrEmpty()}");
        Console.WriteLine($"  empty list: {emptyList.IsNullOrEmpty()}");
        Console.WriteLine($"  full list: {fullList.IsNullOrEmpty()}");

        Console.WriteLine("\n🎯 DistinctBy:");
        var products = new[]
        {
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 1000 },
            new Product { Id = 2, Name = "Mouse", Category = "Electronics", Price = 20 },
            new Product { Id = 3, Name = "Desk", Category = "Furniture", Price = 500 },
            new Product { Id = 4, Name = "Chair", Category = "Furniture", Price = 200 }
        };

        var distinctCategories = products.DistinctBy(p => p.Category).ToList();
        Console.WriteLine("  Distinct categories:");
        foreach (var product in distinctCategories)
        {
            Console.WriteLine($"    - {product.Category} (example: {product.Name})");
        }

        Console.WriteLine("\n📊 Chunk (size 4):");
        var chunks = numbers.Chunk(4).ToList();
        for (int i = 0; i < chunks.Count; i++)
        {
            Console.WriteLine($"  Chunk {i + 1}: [{string.Join(", ", chunks[i])}]");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 3. ENUM EXTENSIONS DEMONSTRATION
    // ═══════════════════════════════════════════════════════════
    static void DemonstrateEnumExtensions()
    {
        Console.WriteLine("═══ 3. Enum Extensions ═══\n");

        Console.WriteLine("📋 Get Description:");
        foreach (Priority priority in Enum.GetValues(typeof(Priority)))
        {
            Console.WriteLine($"  {priority,-10} → {priority.GetDescription()}");
        }

        Console.WriteLine("\n📝 Get Display Name:");
        foreach (OrderStatus status in Enum.GetValues(typeof(OrderStatus)))
        {
            Console.WriteLine($"  {status,-12} → {status.GetDisplayName()}");
        }

        Console.WriteLine("\n📚 Enum To List:");
        var priorities = EnumExtensions.ToList<Priority>();
        Console.WriteLine($"  All priorities: [{string.Join(", ", priorities)}]");

        Console.WriteLine("\n🏁 Has Flag (example with custom flags):");
        var currentPriority = Priority.High;
        Console.WriteLine($"  Current priority: {currentPriority}");
        Console.WriteLine($"  Has High flag: {currentPriority.HasFlag(Priority.High)}");
        Console.WriteLine($"  Has Low flag: {currentPriority.HasFlag(Priority.Low)}");
    }

    // ═══════════════════════════════════════════════════════════
    // 4. DATETIME EXTENSIONS DEMONSTRATION
    // ═══════════════════════════════════════════════════════════
    static void DemonstrateDateTimeExtensions()
    {
        Console.WriteLine("═══ 4. DateTime Extensions ═══\n");

        var today = DateTime.Today;
        var birthday = new DateTime(1990, 5, 15);

        Console.WriteLine("📅 Is Weekend:");
        for (int i = 0; i < 7; i++)
        {
            var date = today.AddDays(i);
            Console.WriteLine($"  {date:ddd, MMM dd} → {(date.IsWeekend() ? "🏖️  Weekend" : "💼 Weekday")}");
        }

        Console.WriteLine("\n📆 Start/End of Week:");
        Console.WriteLine($"  Today: {today:yyyy-MM-dd (dddd)}");
        Console.WriteLine($"  Start of week: {today.StartOfWeek():yyyy-MM-dd (dddd)}");

        Console.WriteLine("\n📆 End of Month:");
        Console.WriteLine($"  Today: {today:yyyy-MM-dd}");
        Console.WriteLine($"  End of month: {today.EndOfMonth():yyyy-MM-dd}");

        Console.WriteLine("\n🎂 Age Calculation:");
        Console.WriteLine($"  Birth date: {birthday:yyyy-MM-dd}");
        Console.WriteLine($"  Age: {birthday.Age()} years old");

        Console.WriteLine("\n⏰ Start/End of Day:");
        var now = DateTime.Now;
        Console.WriteLine($"  Current: {now:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"  Start of day: {now.StartOfDay():yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"  End of day: {now.EndOfDay():yyyy-MM-dd HH:mm:ss}");

        Console.WriteLine("\n📌 Is Today:");
        Console.WriteLine($"  {today:yyyy-MM-dd} is today: {today.IsToday()}");
        Console.WriteLine($"  {today.AddDays(1):yyyy-MM-dd} is today: {today.AddDays(1).IsToday()}");
    }

    // ═══════════════════════════════════════════════════════════
    // 5. NUMERIC EXTENSIONS DEMONSTRATION
    // ═══════════════════════════════════════════════════════════
    static void DemonstrateNumericExtensions()
    {
        Console.WriteLine("═══ 5. Numeric Extensions ═══\n");

        Console.WriteLine("🔢 Even/Odd:");
        for (int i = 1; i <= 10; i++)
        {
            Console.WriteLine($"  {i,2} → {(i.IsEven() ? "Even ⚪" : "Odd  ⚫")}");
        }

        Console.WriteLine("\n🎯 Clamp:");
        var values = new[] { -5, 0, 5, 10, 15, 20 };
        foreach (var value in values)
        {
            Console.WriteLine($"  {value,3} clamped [0, 10] → {value.Clamp(0, 10),3}");
        }

        Console.WriteLine("\n📊 To Percentage:");
        var decimals = new[] { 0.25m, 0.5m, 0.75m, 1.0m, 1.25m };
        foreach (var value in decimals)
        {
            Console.WriteLine($"  {value,5:F2} → {value.ToPercentage()}");
        }

        Console.WriteLine("\n📏 In Range:");
        var testValues = new[] { 5, 15, 25, 35 };
        foreach (var value in testValues)
        {
            Console.WriteLine($"  {value,2} in range [10, 30]: {value.InRange(10, 30)}");
        }

        Console.WriteLine("\n🥇 Ordinal Numbers:");
        var positions = new[] { 1, 2, 3, 4, 11, 21, 22, 23, 101, 111 };
        foreach (var position in positions)
        {
            Console.WriteLine($"  {position,3} → {position.ToOrdinal()}");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 6. LINQ EXTENSIONS DEMONSTRATION
    // ═══════════════════════════════════════════════════════════
    static void DemonstrateLinqExtensions()
    {
        Console.WriteLine("═══ 6. LINQ Extensions ═══\n");

        var products = new[]
        {
            new Product { Id = 1, Name = "Laptop", Price = 1000, Category = "Electronics", InStock = true },
            new Product { Id = 2, Name = "Mouse", Price = 20, Category = "Electronics", InStock = true },
            new Product { Id = 3, Name = "Desk", Price = 500, Category = "Furniture", InStock = false },
            new Product { Id = 4, Name = "Chair", Price = 200, Category = "Furniture", InStock = true },
            new Product { Id = 5, Name = "Monitor", Price = 300, Category = "Electronics", InStock = true }
        };

        Console.WriteLine("🔍 WhereIf (conditional filter):");
        bool filterExpensive = true;
        var filtered = products.WhereIf(filterExpensive, p => p.Price > 100);
        Console.WriteLine($"  Filter expensive (enabled): {string.Join(", ", filtered.Select(p => p.Name))}");

        filterExpensive = false;
        filtered = products.WhereIf(filterExpensive, p => p.Price > 100);
        Console.WriteLine($"  Filter expensive (disabled): {string.Join(", ", filtered.Select(p => p.Name))}");

        Console.WriteLine("\n📊 OrderByIf (conditional sorting):");
        bool sortByPrice = true;
        var sorted = products.OrderByIf(sortByPrice, p => p.Price);
        Console.WriteLine($"  Sort by price (enabled): {string.Join(", ", sorted.Select(p => $"{p.Name}(${p.Price})"))}");

        sortByPrice = false;
        sorted = products.OrderByIf(sortByPrice, p => p.Price);
        Console.WriteLine($"  Sort by price (disabled): {string.Join(", ", sorted.Select(p => $"{p.Name}(${p.Price})"))}");

        Console.WriteLine("\n⏸️  TakeUntil (stop at condition):");
        var numbers = Enumerable.Range(1, 20);
        var upTo10 = numbers.TakeUntil(n => n == 10);
        Console.WriteLine($"  Take until 10: [{string.Join(", ", upTo10)}]");

        Console.WriteLine("\n🔢 ForEachWithIndex:");
        Console.WriteLine("  Products with index:");
        products.Take(3).ForEachWithIndex((product, index) =>
        {
            Console.WriteLine($"    [{index}] {product.Name} - ${product.Price}");
        });

        Console.WriteLine("\n🔄 DefaultIfEmpty:");
        var emptyProducts = new List<Product>();
        var defaultProducts = new[]
        {
            new Product { Id = 0, Name = "No products", Price = 0, Category = "N/A", InStock = false }
        };

        var result = emptyProducts.DefaultIfEmpty(defaultProducts);
        Console.WriteLine($"  Empty list with default: {string.Join(", ", result.Select(p => p.Name))}");

        var result2 = products.DefaultIfEmpty(defaultProducts);
        Console.WriteLine($"  Full list with default: {string.Join(", ", result2.Take(3).Select(p => p.Name))}...");
    }
}

#endregion
