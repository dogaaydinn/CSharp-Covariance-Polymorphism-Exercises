# ⚠️ Nullable Reference Types - User Profile Management

## 📚 Overview

**NullableReferenceTypes** demonstrates C# 8.0+ nullable reference types with a complete user profile management system. Learn compile-time null safety with `string?`, `??`, `?.`, `??=`, `!`, and `ArgumentNullException.ThrowIfNull`.

**Lines of Code**: 659
**Build Status**: ✅ 0 errors
**C# Version**: 8.0+ (Nullable enabled)

## 🎯 Key Features

### Nullable Operators
1. **`string?`** - Nullable reference type (can be null)
2. **`??`** - Null-coalescing operator (provide default)
3. **`?.`** - Null-conditional operator (safe navigation)
4. **`??=`** - Null-coalescing assignment (assign if null)
5. **`!`** - Null-forgiving operator (tell compiler "not null")
6. **`ArgumentNullException.ThrowIfNull`** - Modern null validation (C# 11+)
7. **`HasValue`** - Nullable value type check
8. **`Value`** - Nullable value type access

## 💻 Quick Start

```bash
cd samples/02-Intermediate/NullableReferenceTypes
dotnet build
dotnet run
```

## 🎓 Core Concepts

### 1. Nullable vs Non-Nullable Properties

```csharp
public class UserProfile
{
    // ✅ Non-nullable: Required fields (must be assigned)
    public string Username { get; set; }
    public string Email { get; set; }

    // ⚠️ Nullable: Optional fields (can be null)
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Constructor ensures non-nullable fields are initialized
    public UserProfile(string username, string email)
    {
        Username = username;
        Email = email;
    }
}
```

**Benefits**:
- Compiler warns if non-nullable might be null
- Clear API: `string?` means "this can be null"
- Prevents `NullReferenceException` at compile-time

### 2. Null-Coalescing Operator (??)

**Purpose**: Provide default value if null

```csharp
// If Bio is null, use default value
string bio = profile.Bio ?? "No bio available";

// Chaining: Try multiple sources
string display = profile.Bio ?? profile.PhoneNumber ?? "No information";

// With method calls
string avatar = profile.GetAvatarOrDefault();  // Uses ?? internally
```

**When to use**: Need a fallback value for nullable properties

### 3. Null-Conditional Operator (?.)

**Purpose**: Safely access members of nullable objects

```csharp
// If LastLoginAt is null, whole expression returns null
string lastLogin = profile.LastLoginAt?.ToString("dd/MM/yyyy") ?? "Never";

// Chaining: Only continues if each step is non-null
int? bioLength = profile?.Bio?.Length;

// With method calls
bool hasValue = profile?.IsEmailVerified() ?? false;
```

**When to use**: Accessing properties/methods on potentially null objects

### 4. Null-Coalescing Assignment (??=)

**Purpose**: Assign only if current value is null

```csharp
// Only set if AvatarUrl is currently null
profile.AvatarUrl ??= "https://example.com/default.png";

// Useful for lazy initialization
_cache ??= new Dictionary<string, UserProfile>();
```

**When to use**: Set default values without overwriting existing values

### 5. Null-Forgiving Operator (!)

**Purpose**: Tell compiler "I know this is not null"

```csharp
// GetProfile returns UserProfile?, but we KNOW it exists
var profile = service.GetProfile("alice")!;
Console.WriteLine(profile.Username);  // No warning

// After null check
if (user.LastLoginAt.HasValue)
{
    var days = (DateTime.UtcNow - user.LastLoginAt!.Value).Days;
}
```

**⚠️ WARNING**: Only use when you're absolutely sure value is not null, otherwise runtime `NullReferenceException`

### 6. ArgumentNullException.ThrowIfNull (C# 11+)

**Purpose**: Modern null parameter validation

```csharp
// ✅ Modern way (C# 11+)
public UserProfile CreateProfile(string username, string email)
{
    ArgumentNullException.ThrowIfNull(username);
    ArgumentNullException.ThrowIfNull(email);

    return new UserProfile(username, email);
}

// ❌ Old way (pre-C# 11)
public UserProfile CreateProfile(string username, string email)
{
    if (username == null)
        throw new ArgumentNullException(nameof(username));

    if (email == null)
        throw new ArgumentNullException(nameof(email));

    return new UserProfile(username, email);
}
```

**When to use**: Validating non-nullable parameters

### 7. Nullable Return Types

**Purpose**: Clearly indicate methods that might return null

```csharp
// ⚠️ Nullable return: Might be null
public UserProfile? GetProfile(string username)
{
    return _profiles.GetValueOrDefault(username);  // Can return null
}

// ✅ Non-nullable return: Guaranteed non-null (or throws)
public UserProfile GetProfileOrThrow(string username)
{
    var profile = GetProfile(username);
    if (profile == null)
        throw new KeyNotFoundException($"Profile '{username}' not found");

    return profile;  // Compiler knows this is non-null
}

// Usage
UserProfile? maybeProfile = service.GetProfile("bob");  // Might be null
if (maybeProfile != null)
{
    Console.WriteLine(maybeProfile.Username);
}

UserProfile definitelyProfile = service.GetProfileOrThrow("alice");  // Not null
Console.WriteLine(definitelyProfile.Username);  // No null check needed
```

### 8. Nullable Value Types (DateTime?)

**Purpose**: Allow value types to be null

```csharp
public DateTime? LastLoginAt { get; set; }  // Can be null

// Check with HasValue
if (profile.LastLoginAt.HasValue)
{
    DateTime loginTime = profile.LastLoginAt.Value;
    Console.WriteLine($"Last login: {loginTime}");
}

// Or use null-conditional
string display = profile.LastLoginAt?.ToString("dd/MM/yyyy") ?? "Never";
```

## 📊 10 Demonstrations

### 1. Creating Profiles with Null Safety
```csharp
var service = new ProfileService();

// ✅ Valid: Non-null required parameters
service.CreateProfile("alice", "alice@example.com", "Software Engineer");

// ✅ Valid: Bio is optional (null is OK)
service.CreateProfile("bob", "bob@example.com", null);

// ❌ Invalid: Null username causes ArgumentNullException
service.CreateProfile(null, "test@example.com");  // Runtime exception
```

### 2. Nullable vs Non-Nullable Properties
```csharp
var profile = new UserProfile("charlie", "charlie@example.com");

// ✅ Non-nullable: Always have value
Console.WriteLine(profile.Username);   // Never null
Console.WriteLine(profile.Email);      // Never null
Console.WriteLine(profile.CreatedAt);  // Never null

// ⚠️ Nullable: Might be null
Console.WriteLine(profile.Bio ?? "null");
Console.WriteLine(profile.AvatarUrl ?? "null");
Console.WriteLine(profile.LastLoginAt?.ToString() ?? "null");
```

### 3. Null-Coalescing Operator (??)
```csharp
string bio = profile.Bio ?? "No bio available";
string avatar = profile.AvatarUrl ?? "https://example.com/default.png";
string display = profile.Bio ?? profile.PhoneNumber ?? "No information";
```

### 4. Null-Conditional Operator (?.)
```csharp
// LastLoginAt is null, so ?. returns null, then ?? provides default
string lastLogin = profile.LastLoginAt?.ToString("dd/MM/yyyy") ?? "Never";

// Chain safely
int? bioLength = profile?.Bio?.Length;  // null if Bio is null
```

### 5. Null-Coalescing Assignment (??=)
```csharp
profile.AvatarUrl ??= "https://example.com/frank1.jpg";  // Assigns
profile.AvatarUrl ??= "https://example.com/frank2.jpg";  // Doesn't assign (already set)
```

### 6. Null-Forgiving Operator (!)
```csharp
// We KNOW this profile exists
var profile = service.GetProfile("grace")!;  // ! suppresses warning
Console.WriteLine(profile.Username);  // No warning

// After HasValue check
if (user.LastLoginAt.HasValue)
{
    var days = (DateTime.UtcNow - user.LastLoginAt!.Value).Days;
}
```

### 7. ArgumentNullException.ThrowIfNull
```csharp
public UserProfile CreateProfile(string username, string email)
{
    // ✅ Modern null validation (C# 11+)
    ArgumentNullException.ThrowIfNull(username);
    ArgumentNullException.ThrowIfNull(email);

    return new UserProfile(username, email);
}
```

### 8. Nullable Return Types
```csharp
// Nullable return
UserProfile? maybeProfile = service.GetProfile("jack");
if (maybeProfile != null)
{
    Console.WriteLine(maybeProfile.Username);
}

// Non-nullable return (throws if not found)
UserProfile profile = service.GetProfileOrThrow("jack");
Console.WriteLine(profile.Username);  // No null check needed
```

### 9. Null-Safe Collections
```csharp
var profiles = service.GetAllProfiles();  // Never null (might be empty)

// Filter by nullable property
var withBio = profiles.Where(p => p.Bio != null);
var withoutBio = profiles.Where(p => p.Bio == null);

// Using HasValue for nullable value types
var loggedIn = profiles.Where(p => p.LastLoginAt.HasValue);

// Safe null handling in LINQ
var bioLengths = profiles.Select(p => p.Bio?.Length ?? 0);
```

### 10. Profile Search with Null Safety
```csharp
// Search with non-null term
var results = service.SearchProfiles("C#");  // Returns matching profiles

// Search with null term (returns empty)
var nullResults = service.SearchProfiles(null);  // Empty collection

// Null-safe search implementation
return profiles.Where(p =>
    p.Username.Contains(searchTerm) ||
    p.Email.Contains(searchTerm) ||
    (p.Bio?.Contains(searchTerm) ?? false)  // Null-safe Bio search
);
```

## 💡 Best Practices

### DO ✅

1. **Enable Nullable Reference Types**
   ```csharp
   #nullable enable
   ```
   Or in `.csproj`:
   ```xml
   <Nullable>enable</Nullable>
   ```

2. **Use `?` for Nullable Types**
   ```csharp
   public string? OptionalField { get; set; }  // Can be null
   ```

3. **Initialize Non-Nullable in Constructor**
   ```csharp
   public UserProfile(string username, string email)
   {
       Username = username;  // Ensures non-null
       Email = email;
   }
   ```

4. **Use `??` for Defaults**
   ```csharp
   string bio = profile.Bio ?? "No bio available";
   ```

5. **Use `?.` for Safe Navigation**
   ```csharp
   int? length = profile?.Bio?.Length;
   ```

6. **Use `ArgumentNullException.ThrowIfNull`** (C# 11+)
   ```csharp
   ArgumentNullException.ThrowIfNull(username);
   ```

7. **Return Nullable Types When Appropriate**
   ```csharp
   public UserProfile? FindProfile(string username) { }
   ```

### DON'T ❌

1. **Don't Use `!` Unless Absolutely Sure**
   ```csharp
   // ❌ BAD: Runtime NullReferenceException if wrong
   var profile = service.GetProfile("unknown")!;
   profile.Username;  // 💥 CRASH if profile is null

   // ✅ GOOD: Check first
   var profile = service.GetProfile("unknown");
   if (profile != null)
       Console.WriteLine(profile.Username);
   ```

2. **Don't Ignore Warnings**
   ```csharp
   // ❌ BAD: Suppressing warnings
   #pragma warning disable CS8600
   string name = nullableString;

   // ✅ GOOD: Handle nullability properly
   string name = nullableString ?? "default";
   ```

3. **Don't Return Null Without `?`**
   ```csharp
   // ❌ BAD: Misleading signature
   public UserProfile GetProfile() => null;  // Warning!

   // ✅ GOOD: Honest signature
   public UserProfile? GetProfile() => null;
   ```

4. **Don't Forget Constructor Initialization**
   ```csharp
   // ❌ BAD: Non-nullable not initialized
   public class User
   {
       public string Name { get; set; }  // Warning!
   }

   // ✅ GOOD: Initialize in constructor
   public class User
   {
       public string Name { get; set; }
       public User(string name) => Name = name;
   }
   ```

## 🔍 Common Patterns

### Pattern 1: Lazy Initialization with ??=
```csharp
private Dictionary<string, UserProfile>? _cache;

public Dictionary<string, UserProfile> Cache
{
    get
    {
        _cache ??= new Dictionary<string, UserProfile>();
        return _cache;
    }
}
```

### Pattern 2: Optional Parameters
```csharp
public UserProfile CreateProfile(
    string username,
    string email,
    string? bio = null,      // Optional
    string? avatarUrl = null  // Optional
)
{
    // ...
}
```

### Pattern 3: Safe Collection Operations
```csharp
public IEnumerable<UserProfile> SearchProfiles(string? searchTerm)
{
    if (string.IsNullOrWhiteSpace(searchTerm))
        return Enumerable.Empty<UserProfile>();

    return profiles.Where(p =>
        p.Username.Contains(searchTerm) ||
        (p.Bio?.Contains(searchTerm) ?? false)
    );
}
```

### Pattern 4: Null-Safe Chaining
```csharp
var displayName = user?.Profile?.Bio?.Substring(0, 50) ?? "No bio";
```

### Pattern 5: HasValue Check for Nullable Value Types
```csharp
if (profile.LastLoginAt.HasValue)
{
    var daysSince = (DateTime.UtcNow - profile.LastLoginAt.Value).Days;
    Console.WriteLine($"Last login: {daysSince} days ago");
}
```

## 🎯 Use Cases

1. **User Profile Management** - Optional bio, avatar, phone number
2. **API Responses** - Nullable fields for optional data
3. **Database Entities** - Nullable foreign keys
4. **Configuration** - Optional settings with defaults
5. **Search Results** - Methods that might not find anything

## 📈 Benefits

### Compile-Time Safety
```csharp
string name = null;  // ⚠️ Warning: Assigning null to non-nullable
```

### Clear API Contracts
```csharp
public UserProfile? GetProfile(string username);  // Might return null
public UserProfile GetProfileOrThrow(string username);  // Never null
```

### Prevents NullReferenceException
```csharp
// ✅ Compiler forces null check
UserProfile? profile = GetProfile("bob");
// profile.Username;  // ⚠️ Warning: Possible null reference
if (profile != null)
    Console.WriteLine(profile.Username);  // ✅ Safe
```

## 🔗 Related Patterns

- **Optional Pattern** - Alternative to nullable with more explicit semantics
- **Maybe Monad** - Functional approach to nullable values
- **Result Pattern** - Return success/failure instead of null
- **Null Object Pattern** - Use default object instead of null

**See**: [WHY_THIS_PATTERN.md](WHY_THIS_PATTERN.md) for detailed explanation

## 📚 Operators Summary

| Operator | Purpose | Example | Result |
|----------|---------|---------|--------|
| `string?` | Nullable type | `string? bio = null;` | Allows null assignment |
| `??` | Null-coalescing | `bio ?? "default"` | Returns "default" if bio is null |
| `?.` | Null-conditional | `bio?.Length` | Returns null if bio is null |
| `??=` | Null-coalescing assignment | `bio ??= "default"` | Assigns only if bio is null |
| `!` | Null-forgiving | `bio!.Length` | Suppresses null warning |
| `HasValue` | Nullable check | `date.HasValue` | True if date has value |
| `Value` | Nullable access | `date.Value` | Gets value (throws if null) |

## ✨ Key Takeaways

1. ✅ **Enable nullable reference types** for compile-time null safety
2. ✅ **Use `string?` for nullable**, `string` for non-nullable
3. ✅ **`??` provides defaults**, `?.` enables safe navigation
4. ✅ **`??=` for lazy initialization**, `!` when you're absolutely sure
5. ✅ **`ArgumentNullException.ThrowIfNull`** for modern validation
6. ✅ **Nullable return types** (`UserProfile?`) clearly indicate "might be null"
7. ✅ **Constructor initialization** ensures non-nullable fields are set

---

**Remember**: Nullable reference types catch null errors at compile-time, preventing runtime `NullReferenceException`! ⚠️
