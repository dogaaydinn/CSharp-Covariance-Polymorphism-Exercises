#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace NullableReferenceTypes;

// ═══════════════════════════════════════════════════════════
// USER PROFILE MODEL
// ═══════════════════════════════════════════════════════════

/// <summary>
/// User profile with nullable and non-nullable properties
/// </summary>
public class UserProfile
{
    // ✅ Non-nullable: Required fields
    public string Username { get; set; }
    public string Email { get; set; }

    // ⚠️ Nullable: Optional fields
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? PhoneNumber { get; set; }

    // ✅ Non-nullable with default value
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int LoginCount { get; set; } = 0;

    // ⚠️ Nullable: Optional timestamp
    public DateTime? LastLoginAt { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }

    // ✅ Constructor ensures non-nullable fields are initialized
    public UserProfile(string username, string email)
    {
        // ArgumentNullException.ThrowIfNull (C# 11+)
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(email);

        Username = username;
        Email = email;
    }

    /// <summary>
    /// Check if profile is complete
    /// </summary>
    public bool IsComplete()
    {
        return !string.IsNullOrWhiteSpace(Bio) &&
               !string.IsNullOrWhiteSpace(AvatarUrl) &&
               !string.IsNullOrWhiteSpace(PhoneNumber);
    }

    /// <summary>
    /// Get bio with null-coalescing operator (??)
    /// </summary>
    public string GetBioOrDefault()
    {
        // ?? operator: If Bio is null, return default
        return Bio ?? "No bio available";
    }

    /// <summary>
    /// Get avatar URL with null-coalescing operator
    /// </summary>
    public string GetAvatarOrDefault()
    {
        return AvatarUrl ?? "https://example.com/default-avatar.png";
    }

    /// <summary>
    /// Format last login with null-conditional operator (?.)
    /// </summary>
    public string GetLastLoginDisplay()
    {
        // ?. operator: If LastLoginAt is null, whole expression is null
        // Then ?? provides default value
        return LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "Never logged in";
    }

    /// <summary>
    /// Check if email is verified
    /// </summary>
    public bool IsEmailVerified()
    {
        // HasValue property for nullable value types
        return EmailVerifiedAt.HasValue;
    }

    /// <summary>
    /// Get days since last login
    /// </summary>
    public int? GetDaysSinceLastLogin()
    {
        // Return nullable int - might not have value
        if (!LastLoginAt.HasValue)
            return null;

        return (DateTime.UtcNow - LastLoginAt.Value).Days;
    }

    public override string ToString()
    {
        return $"UserProfile(Username: {Username}, Email: {Email}, " +
               $"Bio: {Bio ?? "null"}, LastLogin: {GetLastLoginDisplay()})";
    }
}

// ═══════════════════════════════════════════════════════════
// PROFILE SERVICE
// ═══════════════════════════════════════════════════════════

/// <summary>
/// Service for managing user profiles with null safety
/// </summary>
public class ProfileService
{
    private readonly Dictionary<string, UserProfile> _profiles = new();

    /// <summary>
    /// Create profile with null validation
    /// </summary>
    public UserProfile CreateProfile(string username, string email, string? bio = null)
    {
        // ✅ C# 11: ArgumentNullException.ThrowIfNull
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(email);

        // Additional validation
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        // Check if already exists
        if (_profiles.ContainsKey(username))
            throw new InvalidOperationException($"Profile '{username}' already exists");

        var profile = new UserProfile(username, email)
        {
            Bio = bio  // Nullable parameter, can be null
        };

        _profiles[username] = profile;
        Console.WriteLine($"  ✅ Created profile: {username}");
        return profile;
    }

    /// <summary>
    /// Get profile - returns nullable
    /// </summary>
    public UserProfile? GetProfile(string username)
    {
        // ⚠️ Return type is UserProfile? - can be null
        return _profiles.GetValueOrDefault(username);
    }

    /// <summary>
    /// Get profile or throw exception
    /// </summary>
    public UserProfile GetProfileOrThrow(string username)
    {
        // ✅ Return type is UserProfile - guaranteed non-null
        var profile = GetProfile(username);
        if (profile == null)
            throw new KeyNotFoundException($"Profile '{username}' not found");

        return profile;  // Compiler knows this is non-null
    }

    /// <summary>
    /// Update bio with null handling
    /// </summary>
    public bool UpdateBio(string username, string? newBio)
    {
        var profile = GetProfile(username);

        // Null check before accessing
        if (profile != null)
        {
            profile.Bio = newBio;  // Nullable assignment is OK
            Console.WriteLine($"  ✏️  Updated bio for {username}");
            return true;
        }

        Console.WriteLine($"  ❌ Profile not found: {username}");
        return false;
    }

    /// <summary>
    /// Update avatar with null-coalescing assignment (??=)
    /// </summary>
    public void UpdateAvatarIfEmpty(string username, string? avatarUrl)
    {
        var profile = GetProfile(username);

        if (profile != null)
        {
            // ??= operator: Assign only if current value is null
            profile.AvatarUrl ??= avatarUrl;
            Console.WriteLine($"  🖼️  Avatar set for {username}: {profile.AvatarUrl ?? "null"}");
        }
    }

    /// <summary>
    /// Record login with timestamp
    /// </summary>
    public void RecordLogin(string username)
    {
        var profile = GetProfile(username);

        if (profile != null)
        {
            profile.LastLoginAt = DateTime.UtcNow;
            profile.LoginCount++;
            Console.WriteLine($"  🔐 Login recorded for {username} (Count: {profile.LoginCount})");
        }
    }

    /// <summary>
    /// Verify email - set verification timestamp
    /// </summary>
    public void VerifyEmail(string username)
    {
        var profile = GetProfile(username);

        if (profile != null)
        {
            profile.EmailVerifiedAt = DateTime.UtcNow;
            Console.WriteLine($"  ✉️  Email verified for {username}");
        }
    }

    /// <summary>
    /// Get all profiles - non-null collection
    /// </summary>
    public IEnumerable<UserProfile> GetAllProfiles()
    {
        // ✅ Return type is non-nullable collection (never null, might be empty)
        return _profiles.Values;
    }

    /// <summary>
    /// Get active users (logged in within last 30 days)
    /// </summary>
    public IEnumerable<UserProfile> GetActiveUsers()
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        return _profiles.Values
            .Where(p => p.LastLoginAt.HasValue && p.LastLoginAt.Value > thirtyDaysAgo)
            .OrderByDescending(p => p.LastLoginAt!.Value);  // ! = null-forgiving operator
    }

    /// <summary>
    /// Search profiles with null-safe string operations
    /// </summary>
    public IEnumerable<UserProfile> SearchProfiles(string? searchTerm)
    {
        // Handle null search term
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<UserProfile>();

        return _profiles.Values
            .Where(p =>
                p.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                // Null-conditional operator for Bio (might be null)
                (p.Bio?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            );
    }

    /// <summary>
    /// Get profiles with incomplete information
    /// </summary>
    public IEnumerable<UserProfile> GetIncompleteProfiles()
    {
        return _profiles.Values
            .Where(p => !p.IsComplete());
    }

    /// <summary>
    /// Delete profile - returns bool indicating success
    /// </summary>
    public bool DeleteProfile(string username)
    {
        if (_profiles.Remove(username))
        {
            Console.WriteLine($"  🗑️  Deleted profile: {username}");
            return true;
        }

        Console.WriteLine($"  ❌ Profile not found: {username}");
        return false;
    }

    public int Count => _profiles.Count;
}

// ═══════════════════════════════════════════════════════════
// DEMONSTRATIONS
// ═══════════════════════════════════════════════════════════

class Program
{
    static void Main()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   ⚠️  NULLABLE REFERENCE TYPES - USER PROFILES        ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("═══ 1. Creating Profiles with Null Safety ═══\n");
        DemonstrateProfileCreation();

        Console.WriteLine("\n═══ 2. Nullable vs Non-Nullable Properties ═══\n");
        DemonstrateNullableProperties();

        Console.WriteLine("\n═══ 3. Null-Coalescing Operator (??) ═══\n");
        DemonstrateNullCoalescing();

        Console.WriteLine("\n═══ 4. Null-Conditional Operator (?.) ═══\n");
        DemonstrateNullConditional();

        Console.WriteLine("\n═══ 5. Null-Coalescing Assignment (??=) ═══\n");
        DemonstrateNullCoalescingAssignment();

        Console.WriteLine("\n═══ 6. Null-Forgiving Operator (!) ═══\n");
        DemonstrateNullForgiving();

        Console.WriteLine("\n═══ 7. ArgumentNullException.ThrowIfNull ═══\n");
        DemonstrateArgumentValidation();

        Console.WriteLine("\n═══ 8. Nullable Return Types ═══\n");
        DemonstrateNullableReturns();

        Console.WriteLine("\n═══ 9. Null-Safe Collections ═══\n");
        DemonstrateNullSafeCollections();

        Console.WriteLine("\n═══ 10. Profile Search with Null Safety ═══\n");
        DemonstrateNullSafeSearch();

        Console.WriteLine("\n✅ ÖĞRENİLENLER:");
        Console.WriteLine("   • #nullable enable - Nullable reference types açar");
        Console.WriteLine("   • string? - Nullable string (null olabilir)");
        Console.WriteLine("   • string - Non-nullable string (null olamaz)");
        Console.WriteLine("   • ?? - Null-coalescing operator (null ise varsayılan)");
        Console.WriteLine("   • ?. - Null-conditional operator (null ise işlem yapma)");
        Console.WriteLine("   • ??= - Null-coalescing assignment (null ise ata)");
        Console.WriteLine("   • ! - Null-forgiving operator (compiler'a \"null değil\" de)");
        Console.WriteLine("   • ArgumentNullException.ThrowIfNull - Modern null check");
        Console.WriteLine("   • HasValue - Nullable value type kontrolü");
        Console.WriteLine("   • Value - Nullable value type değeri");
    }

    static void DemonstrateProfileCreation()
    {
        var service = new ProfileService();

        Console.WriteLine("✅ Creating profiles with required fields:");

        // ✅ Valid: Non-null required parameters
        var alice = service.CreateProfile("alice", "alice@example.com", "Software Engineer");

        // ✅ Valid: Bio is optional (null is OK)
        var bob = service.CreateProfile("bob", "bob@example.com", null);

        // ❌ Invalid: Would cause compile-time warning and runtime exception
        try
        {
#pragma warning disable CS8625  // Suppress warning for demonstration
            service.CreateProfile(null!, "test@example.com");
#pragma warning restore CS8625
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"  ❌ Caught exception: {ex.Message.Split('\n')[0]}");
        }

        Console.WriteLine($"\n  Total profiles: {service.Count}");
    }

    static void DemonstrateNullableProperties()
    {
        var profile = new UserProfile("charlie", "charlie@example.com");

        Console.WriteLine("📋 Non-nullable properties (always have value):");
        Console.WriteLine($"  Username: {profile.Username}");  // ✅ Never null
        Console.WriteLine($"  Email: {profile.Email}");        // ✅ Never null
        Console.WriteLine($"  CreatedAt: {profile.CreatedAt}");  // ✅ Never null

        Console.WriteLine("\n⚠️ Nullable properties (might be null):");
        Console.WriteLine($"  Bio: {profile.Bio ?? "null"}");              // ⚠️ Can be null
        Console.WriteLine($"  AvatarUrl: {profile.AvatarUrl ?? "null"}");  // ⚠️ Can be null
        Console.WriteLine($"  LastLoginAt: {profile.LastLoginAt?.ToString() ?? "null"}");  // ⚠️ Can be null

        // Assigning values
        profile.Bio = "Developer and writer";
        profile.AvatarUrl = "https://example.com/charlie.jpg";

        Console.WriteLine("\n✅ After assignment:");
        Console.WriteLine($"  Bio: {profile.Bio}");
        Console.WriteLine($"  AvatarUrl: {profile.AvatarUrl}");
    }

    static void DemonstrateNullCoalescing()
    {
        var service = new ProfileService();
        service.CreateProfile("david", "david@example.com", null);

        var profile = service.GetProfile("david")!;  // ! = we know it's not null

        Console.WriteLine("?? operator: Provide default value if null\n");

        // Bio is null, so return default
        string bio = profile.Bio ?? "No bio available";
        Console.WriteLine($"  Bio: {bio}");

        // AvatarUrl is null, so return default
        string avatar = profile.AvatarUrl ?? "https://example.com/default.png";
        Console.WriteLine($"  Avatar: {avatar}");

        // Chaining ?? operators
        string display = profile.Bio ?? profile.PhoneNumber ?? "No information";
        Console.WriteLine($"  Display: {display}");

        // With method
        Console.WriteLine($"  GetBioOrDefault(): {profile.GetBioOrDefault()}");
        Console.WriteLine($"  GetAvatarOrDefault(): {profile.GetAvatarOrDefault()}");
    }

    static void DemonstrateNullConditional()
    {
        var service = new ProfileService();
        service.CreateProfile("eve", "eve@example.com");

        var profile = service.GetProfile("eve");

        Console.WriteLine("?. operator: Call method/property only if not null\n");

        // LastLoginAt is null, so ?. returns null, then ?? provides default
        string lastLogin = profile?.LastLoginAt?.ToString("dd/MM/yyyy") ?? "Never";
        Console.WriteLine($"  Last Login: {lastLogin}");

        // Record a login
        service.RecordLogin("eve");

        // Now LastLoginAt has value
        lastLogin = profile?.LastLoginAt?.ToString("dd/MM/yyyy") ?? "Never";
        Console.WriteLine($"  Last Login: {lastLogin}");

        // Null-conditional with method call
        int? bioLength = profile?.Bio?.Length;  // Returns null if Bio is null
        Console.WriteLine($"  Bio Length: {bioLength?.ToString() ?? "N/A"}");

        profile!.Bio = "Security researcher";
        bioLength = profile.Bio?.Length;
        Console.WriteLine($"  Bio Length: {bioLength}");
    }

    static void DemonstrateNullCoalescingAssignment()
    {
        var service = new ProfileService();
        service.CreateProfile("frank", "frank@example.com");

        var profile = service.GetProfile("frank")!;

        Console.WriteLine("??= operator: Assign only if current value is null\n");

        Console.WriteLine($"  Initial AvatarUrl: {profile.AvatarUrl ?? "null"}");

        // First call: AvatarUrl is null, so assign
        profile.AvatarUrl ??= "https://example.com/frank1.jpg";
        Console.WriteLine($"  After first ??=: {profile.AvatarUrl}");

        // Second call: AvatarUrl is NOT null, so don't assign
        profile.AvatarUrl ??= "https://example.com/frank2.jpg";
        Console.WriteLine($"  After second ??=: {profile.AvatarUrl}");

        // Using with service method
        service.UpdateAvatarIfEmpty("frank", "https://example.com/frank3.jpg");
    }

    static void DemonstrateNullForgiving()
    {
        var service = new ProfileService();
        service.CreateProfile("grace", "grace@example.com");
        service.RecordLogin("grace");

        Console.WriteLine("! operator: Tell compiler \"I know this is not null\"\n");

        // GetProfile returns UserProfile?, but we KNOW it's not null
        var profile = service.GetProfile("grace")!;  // ! suppresses warning
        Console.WriteLine($"  Username: {profile.Username}");  // No warning

        // Accessing LastLoginAt.Value after checking HasValue
        var activeUsers = service.GetActiveUsers();
        foreach (var user in activeUsers)
        {
            // We filtered by HasValue, so we KNOW Value exists
            var daysSince = (DateTime.UtcNow - user.LastLoginAt!.Value).Days;  // ! needed here
            Console.WriteLine($"  {user.Username}: {daysSince} days since login");
        }

        // ⚠️ WARNING: Only use ! when you're absolutely sure value is not null
        // Otherwise you'll get NullReferenceException at runtime
    }

    static void DemonstrateArgumentValidation()
    {
        Console.WriteLine("ArgumentNullException.ThrowIfNull (C# 11+)\n");

        // ✅ Modern way (C# 11+)
        void ModernValidation(string username, string email)
        {
            ArgumentNullException.ThrowIfNull(username);
            ArgumentNullException.ThrowIfNull(email);

            Console.WriteLine($"  ✅ Modern validation passed: {username}");
        }

        // ❌ Old way (pre-C# 11)
        void OldValidation(string username, string email)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username));

            if (email == null)
                throw new ArgumentNullException(nameof(email));

            Console.WriteLine($"  ✅ Old validation passed: {username}");
        }

        ModernValidation("hannah", "hannah@example.com");
        OldValidation("ian", "ian@example.com");

        // Test null
        try
        {
#pragma warning disable CS8625
            ModernValidation(null!, "test@example.com");
#pragma warning restore CS8625
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"  ❌ Caught: {ex.ParamName} cannot be null");
        }
    }

    static void DemonstrateNullableReturns()
    {
        var service = new ProfileService();
        service.CreateProfile("jack", "jack@example.com");

        Console.WriteLine("Nullable vs Non-Nullable return types\n");

        // ⚠️ Nullable return: Might be null
        UserProfile? nullable = service.GetProfile("jack");
        Console.WriteLine($"  GetProfile (nullable): {nullable?.Username ?? "null"}");

        UserProfile? notFound = service.GetProfile("nonexistent");
        Console.WriteLine($"  GetProfile (not found): {notFound?.Username ?? "null"}");

        // ✅ Non-nullable return: Guaranteed non-null (or throws)
        try
        {
            UserProfile nonNull = service.GetProfileOrThrow("jack");
            Console.WriteLine($"  GetProfileOrThrow: {nonNull.Username}");

            UserProfile willThrow = service.GetProfileOrThrow("nonexistent");
            Console.WriteLine($"  This won't execute: {willThrow.Username}");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"  ❌ Exception: {ex.Message}");
        }

        // Nullable value type return
        var profile = service.GetProfile("jack")!;
        int? daysSince = profile.GetDaysSinceLastLogin();
        Console.WriteLine($"  Days since login: {daysSince?.ToString() ?? "Never logged in"}");

        service.RecordLogin("jack");
        daysSince = profile.GetDaysSinceLastLogin();
        Console.WriteLine($"  Days since login: {daysSince}");
    }

    static void DemonstrateNullSafeCollections()
    {
        var service = new ProfileService();
        service.CreateProfile("kate", "kate@example.com", "Designer");
        service.CreateProfile("leo", "leo@example.com", null);
        service.CreateProfile("mia", "mia@example.com", "Manager");

        service.RecordLogin("kate");
        service.RecordLogin("mia");

        Console.WriteLine("Working with nullable properties in collections\n");

        // ✅ GetAllProfiles() returns non-null collection (never null, might be empty)
        var allProfiles = service.GetAllProfiles();
        Console.WriteLine($"  All profiles: {allProfiles.Count()}");

        // Filter by nullable property
        var withBio = allProfiles.Where(p => p.Bio != null).ToList();
        Console.WriteLine($"  Profiles with bio: {withBio.Count}");

        var withoutBio = allProfiles.Where(p => p.Bio == null).ToList();
        Console.WriteLine($"  Profiles without bio: {withoutBio.Count}");

        // Using HasValue for nullable value types
        var loggedIn = allProfiles.Where(p => p.LastLoginAt.HasValue).ToList();
        Console.WriteLine($"  Profiles with login: {loggedIn.Count}");

        // Safe null handling in LINQ
        var bioLengths = allProfiles
            .Select(p => p.Bio?.Length ?? 0)  // Use 0 if Bio is null
            .ToList();
        Console.WriteLine($"  Bio lengths: {string.Join(", ", bioLengths)}");
    }

    static void DemonstrateNullSafeSearch()
    {
        var service = new ProfileService();
        service.CreateProfile("noah", "noah@example.com", "Developer interested in C#");
        service.CreateProfile("olivia", "olivia@example.com", null);
        service.CreateProfile("peter", "peter@example.com", "C# enthusiast");

        Console.WriteLine("Null-safe search operations\n");

        // Search with non-null term
        var results = service.SearchProfiles("C#");
        Console.WriteLine($"  Search 'C#': {results.Count()} results");
        foreach (var profile in results)
        {
            Console.WriteLine($"    - {profile.Username}: {profile.Bio ?? "(no bio)"}");
        }

        // Search with null term (returns empty collection)
        var nullResults = service.SearchProfiles(null);
        Console.WriteLine($"\n  Search null: {nullResults.Count()} results");

        // Search with empty string
        var emptyResults = service.SearchProfiles("");
        Console.WriteLine($"  Search empty: {emptyResults.Count()} results");

        // Incomplete profiles
        var incomplete = service.GetIncompleteProfiles();
        Console.WriteLine($"\n  Incomplete profiles: {incomplete.Count()}");
        foreach (var profile in incomplete)
        {
            Console.WriteLine($"    - {profile.Username}: Missing " +
                $"{(profile.Bio == null ? "bio " : "")}" +
                $"{(profile.AvatarUrl == null ? "avatar " : "")}" +
                $"{(profile.PhoneNumber == null ? "phone" : "")}");
        }
    }
}
