# 🔒 Advanced Security Guide - Defense-in-Depth

**Last Updated:** 2025-12-02
**Status:** Production-Ready Security Implementation
**Compliance:** OWASP Top 10 Coverage

---

## 📋 Table of Contents

1. [Security Philosophy](#security-philosophy)
2. [Defense-in-Depth Layers](#defense-in-depth-layers)
3. [Security Headers](#security-headers)
4. [SQL Injection Protection](#sql-injection-protection)
5. [Authentication & Authorization](#authentication--authorization)
6. [Sensitive Data Protection](#sensitive-data-protection)
7. [Rate Limiting & DDoS Protection](#rate-limiting--ddos-protection)
8. [HTTPS & Transport Security](#https--transport-security)
9. [Security Testing](#security-testing)
10. [Incident Response](#incident-response)

---

## 🛡️ Security Philosophy

### Why Defense-in-Depth?

**Security is NOT a single layer.** It's multiple layers that protect even when one layer fails.

```
┌─────────────────────────────────────────────────────┐
│  Layer 7: Monitoring & Alerting                     │  ← Detect breaches
├─────────────────────────────────────────────────────┤
│  Layer 6: Rate Limiting                             │  ← Prevent DDoS
├─────────────────────────────────────────────────────┤
│  Layer 5: Input Validation                          │  ← Block bad data
├─────────────────────────────────────────────────────┤
│  Layer 4: Authorization                             │  ← Control access
├─────────────────────────────────────────────────────┤
│  Layer 3: Authentication                            │  ← Verify identity
├─────────────────────────────────────────────────────┤
│  Layer 2: Security Headers                          │  ← Browser protection
├─────────────────────────────────────────────────────┤
│  Layer 1: HTTPS/TLS                                 │  ← Encrypted transport
└─────────────────────────────────────────────────────┘
```

**Key Principle:** If an attacker bypasses JWT authentication, they still face rate limiting, input validation, and security headers.

---

## 🔐 Defense-in-Depth Layers

### Layer 1: HTTPS/TLS Encryption

**What It Protects Against:**
- Man-in-the-Middle (MITM) attacks
- Eavesdropping
- Data tampering

**Implementation:**

```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();  // Force HTTPS
    app.UseHsts();              // HTTP Strict Transport Security
}
```

**HSTS (HTTP Strict Transport Security):**
- Forces browsers to ONLY use HTTPS
- Prevents SSL stripping attacks
- Header: `Strict-Transport-Security: max-age=31536000; includeSubDomains`

**Configuration:**

```csharp
builder.Services.AddHsts(options =>
{
    options.Preload = true;              // Include in browser preload list
    options.IncludeSubDomains = true;    // Apply to all subdomains
    options.MaxAge = TimeSpan.FromDays(365);  // 1 year
});
```

---

### Layer 2: Security Headers

**What They Protect Against:**
- XSS (Cross-Site Scripting)
- Clickjacking
- MIME type sniffing
- Data leakage

**Implementation:** `Middleware/SecurityHeadersMiddleware.cs`

#### Content Security Policy (CSP)

**Purpose:** Controls which resources can load on your page.

```
Content-Security-Policy: default-src 'self'; script-src 'self' 'unsafe-inline'
```

**What This Means:**
- `default-src 'self'` - Only load resources from same origin
- `script-src 'self' 'unsafe-inline'` - Scripts from same origin + inline scripts
- Blocks: `<script src="https://evil.com/malware.js">` ❌
- Allows: `<script src="/js/app.js">` ✅

**Attack Prevented:**

```html
<!-- Attacker injects this -->
<script src="https://evil.com/steal-cookies.js"></script>

<!-- CSP blocks it! -->
Refused to load the script 'https://evil.com/steal-cookies.js' because it
violates the Content-Security-Policy directive: "script-src 'self'"
```

#### X-Frame-Options

**Purpose:** Prevents clickjacking attacks.

```
X-Frame-Options: DENY
```

**Attack Prevented:**

```html
<!-- Attacker's page -->
<iframe src="https://yoursite.com/transfer-money"></iframe>
<!-- User thinks they're clicking on attacker's button -->
<!-- But they're actually clicking the hidden iframe -->

<!-- X-Frame-Options prevents this! -->
```

#### X-Content-Type-Options

**Purpose:** Prevents MIME type sniffing.

```
X-Content-Type-Options: nosniff
```

**Attack Prevented:**

```
// Attacker uploads "image.jpg" (actually JavaScript)
GET /uploads/image.jpg
Content-Type: text/html  <!-- Declared as HTML -->

// Without nosniff: Browser executes it as JavaScript!
// With nosniff: Browser respects Content-Type, refuses execution
```

#### X-XSS-Protection

**Purpose:** Enables browser's XSS filter.

```
X-XSS-Protection: 1; mode=block
```

**Attack Prevented:**

```
// URL: https://site.com/search?q=<script>alert('XSS')</script>
// Without XSS protection: Script executes
// With XSS protection: Page rendering blocked
```

#### Referrer-Policy

**Purpose:** Controls how much information is sent in Referer header.

```
Referrer-Policy: no-referrer-when-downgrade
```

**Why It Matters:**

```
User visits: https://yoursite.com/user/john/profile?token=secret123
Then clicks: http://evil.com

Without Referrer-Policy:
Referer: https://yoursite.com/user/john/profile?token=secret123
<!-- Token leaked! -->

With no-referrer-when-downgrade:
Referer: (empty)
<!-- No leak on HTTPS → HTTP transition -->
```

---

### Layer 3: Authentication (JWT)

**What It Protects Against:**
- Unauthorized access
- Session hijacking
- Replay attacks

**Implementation:**

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,           // Prevent token from other services
            ValidateAudience = true,         // Ensure token is for this API
            ValidateLifetime = true,         // Check expiration
            ValidateIssuerSigningKey = true, // Verify signature
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)
            )
        };
    });
```

**JWT Structure:**

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.  ← Header (algorithm)
eyJzdWIiOiJ1c2VyMTIzIiwiZXhwIjoxNjQw.  ← Payload (claims)
SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV.   ← Signature (verify integrity)
```

**Attack Prevented:**

```csharp
// Attacker modifies token payload
// Original: {"sub": "user123", "role": "user"}
// Modified: {"sub": "user123", "role": "admin"}

// Verification fails because signature doesn't match!
Token validation failed: Invalid signature
```

**Best Practices:**

1. **Short expiration times** - 15-60 minutes
2. **Refresh tokens** - For long-term access
3. **Strong secret** - Minimum 32 characters
4. **HTTPS only** - Never send over HTTP
5. **httpOnly cookies** - Prevent XSS theft

---

### Layer 4: Authorization

**What It Protects Against:**
- Privilege escalation
- Unauthorized resource access

**Implementation:**

```csharp
[Authorize]  // Require authentication
[Authorize(Roles = "Admin")]  // Require specific role
[Authorize(Policy = "VideoOwner")]  // Custom policy
```

**Attack Prevented:**

```csharp
// User A tries to delete User B's video
DELETE /api/videos/user-b-video-id
Authorization: Bearer user-a-token

// Authorization check:
if (video.UploadedBy != currentUser.Id && !currentUser.IsAdmin)
{
    return Forbid();  // 403 Forbidden
}
```

---

### Layer 5: Input Validation

**What It Protects Against:**
- SQL injection
- XSS
- Command injection
- Path traversal

**Implementation:**

```csharp
public async Task<ActionResult> Create([FromBody] VideoDto dto)
{
    // 1. Model validation
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    // 2. Business rule validation
    if (dto.FileSizeBytes > 2GB)
        return BadRequest("File too large");

    // 3. Sanitize input
    dto.Title = SanitizeHtml(dto.Title);

    // 4. Use parameterized queries (EF Core does this automatically)
    await _repository.CreateAsync(dto);
}
```

**Attack Prevented:**

```csharp
// Attacker sends:
POST /api/videos
{
    "title": "<script>alert('XSS')</script>",
    "fileSizeBytes": -1
}

// Validation catches:
{
    "errors": {
        "title": ["Invalid characters detected"],
        "fileSizeBytes": ["Must be positive"]
    }
}
```

---

### Layer 6: Rate Limiting

**What It Protects Against:**
- Brute force attacks
- DDoS attacks
- API abuse

**Implementation:** `Middleware/RequestRateLimitingMiddleware.cs`

```csharp
// Configuration
"RateLimiting": {
    "RequestLimit": 100,        // Max requests
    "TimeWindowSeconds": 60     // Per time window
}

// Result
100 requests in 60 seconds = OK
101st request = 429 Too Many Requests
```

**Attack Prevented:**

```
Attacker sends 1000 requests/second:
Request 1-100: 200 OK
Request 101+: 429 Too Many Requests
Retry-After: 45 seconds

// Without rate limiting: Server crashes
// With rate limiting: Server stays healthy
```

**Advanced: Distributed Rate Limiting (Redis)**

```csharp
// For multi-server deployment
var key = $"rate-limit:{clientIp}:{endpoint}";
var count = await _redis.StringIncrementAsync(key);
if (count == 1)
    await _redis.KeyExpireAsync(key, TimeSpan.FromMinutes(1));

if (count > 100)
    return TooManyRequests();
```

---

### Layer 7: Sensitive Data Protection

**What It Protects Against:**
- Data leakage in logs
- Accidental credential exposure
- Compliance violations (PCI DSS, GDPR)

**Implementation:** `Configuration/SensitiveDataDestructuringPolicy.cs`

#### Serilog Masking

```csharp
Log.Logger = new LoggerConfiguration()
    .MaskSensitiveData()  // Custom policy
    .CreateLogger();
```

**Before Masking:**

```json
{
    "message": "User login",
    "user": {
        "email": "john@example.com",
        "password": "SecretPassword123",
        "creditCard": "4111-1111-1111-1111",
        "apiKey": "sk_live_51Hg8YzABC123..."
    }
}
```

**After Masking:**

```json
{
    "message": "User login",
    "user": {
        "email": "j***n@example.com",
        "password": "***MASKED***",
        "creditCard": "****-****-****-1111",
        "apiKey": "sk_l...123"
    }
}
```

#### Sensitive Property Names

Automatically masked properties (case-insensitive):

```csharp
private static readonly HashSet<string> SensitivePropertyNames = new()
{
    "password", "pwd", "secret", "token", "apikey", "api_key",
    "authorization", "auth", "creditcard", "credit_card", "cvv",
    "ssn", "social_security", "privatekey", "private_key"
};
```

#### Pattern Matching

```csharp
// Credit card: 4111-1111-1111-1111 → ****-****-****-1111
private static readonly Regex CreditCardPattern =
    new(@"\b\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}\b");

// SSN: 123-45-6789 → ***-**-6789
private static readonly Regex SsnPattern =
    new(@"\b\d{3}-\d{2}-\d{4}\b");

// Email: john@example.com → j**n@example.com
private static readonly Regex EmailPattern =
    new(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b");
```

---

## 🛡️ SQL Injection Protection

### Why Entity Framework Core is Safe

**EF Core automatically uses parameterized queries:**

```csharp
// ✅ SAFE - EF Core parameterizes automatically
var videos = await _context.Videos
    .Where(v => v.Category == userInput)  // userInput = "'; DROP TABLE videos; --"
    .ToListAsync();

// Generated SQL:
// SELECT * FROM videos WHERE category = @p0
// Parameters: @p0 = "'; DROP TABLE videos; --"  (treated as string literal)
```

**What happens:**

```sql
-- Attacker sends: category = "'; DROP TABLE videos; --"

-- Without parameterization (DANGEROUS):
SELECT * FROM videos WHERE category = ''; DROP TABLE videos; --'
-- Result: Table deleted! 💀

-- With parameterization (SAFE):
SELECT * FROM videos WHERE category = @p0
-- @p0 = "'; DROP TABLE videos; --"  (just a string, not SQL)
-- Result: No videos found (expected behavior) ✅
```

### Unsafe Patterns to Avoid

❌ **NEVER concatenate user input into SQL:**

```csharp
// ❌ DANGEROUS - SQL INJECTION VULNERABILITY
var sql = $"SELECT * FROM videos WHERE category = '{userInput}'";
var videos = _context.Videos.FromSqlRaw(sql).ToList();

// If userInput = "'; DROP TABLE videos; --"
// SQL becomes: SELECT * FROM videos WHERE category = ''; DROP TABLE videos; --'
// Tables deleted!
```

✅ **Always use parameters:**

```csharp
// ✅ SAFE - Parameterized query
var sql = "SELECT * FROM videos WHERE category = {0}";
var videos = _context.Videos.FromSqlRaw(sql, userInput).ToList();

// EF Core converts to: SELECT * FROM videos WHERE category = @p0
```

### Additional Protection Layers

```csharp
// 1. Input validation
if (!IsValidCategory(userInput))
    return BadRequest("Invalid category");

// 2. Whitelist validation
if (!AllowedCategories.Contains(userInput))
    return BadRequest("Category not allowed");

// 3. Strong typing (best)
public async Task<IEnumerable<VideoDto>> GetAllAsync(
    VideoStatus? status,    // Enum - can't be injected
    string? category,       // Validated
    int skip,              // Numeric - safe
    int take)              // Numeric - safe
```

---

## 🧪 Security Testing

### 1. Automated Security Scans

**OWASP ZAP (Zed Attack Proxy):**

```bash
# Run automated security scan
docker run -t owasp/zap2docker-stable \
    zap-baseline.py -t http://localhost:5001 -r report.html

# Check for:
# - XSS vulnerabilities
# - SQL injection
# - Missing security headers
# - Weak authentication
```

**Burp Suite:**

```
1. Configure browser proxy → Burp Suite
2. Browse application
3. Run "Active Scan"
4. Review findings
```

### 2. Penetration Testing

**SQL Injection Test:**

```bash
# Test 1: Basic injection
curl -X GET "http://localhost:5001/api/videos?category=' OR '1'='1"

# Expected: 400 Bad Request (input validation)
# If returns data: VULNERABLE ❌

# Test 2: Time-based blind injection
curl -X GET "http://localhost:5001/api/videos?category=' OR SLEEP(5)--"

# Expected: Immediate response
# If delays 5 seconds: VULNERABLE ❌
```

**XSS Test:**

```bash
# Test: Reflected XSS
curl -X POST http://localhost:5001/api/videos \
  -H "Content-Type: application/json" \
  -d '{"title": "<script>alert(\"XSS\")</script>", "fileName": "test.mp4"}'

# Expected: Input sanitized or rejected
# Check response and database
```

**Authentication Bypass Test:**

```bash
# Test 1: No token
curl -X GET http://localhost:5001/api/videos/protected-endpoint

# Expected: 401 Unauthorized

# Test 2: Invalid token
curl -X GET http://localhost:5001/api/videos/protected-endpoint \
  -H "Authorization: Bearer invalid_token"

# Expected: 401 Unauthorized

# Test 3: Expired token
curl -X GET http://localhost:5001/api/videos/protected-endpoint \
  -H "Authorization: Bearer expired_token"

# Expected: 401 Unauthorized
```

**Rate Limiting Test:**

```bash
# Send 150 requests in 10 seconds
for i in {1..150}; do
    curl -X GET http://localhost:5001/api/videos &
done

# Expected: First 100 succeed, rest return 429
```

### 3. Security Headers Test

```bash
# Check all security headers
curl -I http://localhost:5001/api/videos

# Expected headers:
HTTP/1.1 200 OK
Content-Security-Policy: default-src 'self'; ...
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Strict-Transport-Security: max-age=31536000
```

---

## 🚨 Incident Response

### Detection

**Abnormal Activity Indicators:**

```csharp
// Log suspicious patterns
if (failedLoginAttempts > 5)
    _logger.LogWarning("Potential brute force attack from {IP}", clientIp);

if (requestRate > 1000)
    _logger.LogWarning("Potential DDoS attack from {IP}", clientIp);

if (sqlPattern.IsMatch(userInput))
    _logger.LogWarning("SQL injection attempt detected: {Input}", userInput);
```

### Response Plan

**1. Detect → 2. Contain → 3. Eradicate → 4. Recover → 5. Learn**

```
DETECT:     Monitoring alerts trigger
   ↓
CONTAIN:    Block attacker IP, disable compromised account
   ↓
ERADICATE:  Patch vulnerability, rotate secrets
   ↓
RECOVER:    Restore from backup, verify integrity
   ↓
LEARN:      Post-mortem, update defenses
```

---

## ✅ Security Checklist

### Before Production Deployment

- [ ] **HTTPS enforced** - All traffic encrypted
- [ ] **Security headers** - CSP, X-Frame-Options, etc.
- [ ] **JWT authentication** - Strong secret, short expiration
- [ ] **Rate limiting** - Protection against DDoS
- [ ] **Input validation** - All user input validated
- [ ] **SQL injection** - EF Core parameterized queries
- [ ] **Sensitive data** - Passwords/tokens masked in logs
- [ ] **CORS configured** - Specific origins only
- [ ] **Error messages** - No sensitive info exposed
- [ ] **Dependencies updated** - No known vulnerabilities
- [ ] **Security testing** - OWASP ZAP scan passed
- [ ] **Monitoring** - Alerts for suspicious activity

### Production Environment Variables

```bash
# ❌ Development (insecure)
JWT_SECRET="dev-secret-123"
ASPNETCORE_ENVIRONMENT=Development
ENABLE_SWAGGER=true

# ✅ Production (secure)
JWT_SECRET="$(openssl rand -base64 32)"  # Strong random secret
ASPNETCORE_ENVIRONMENT=Production
ENABLE_SWAGGER=false
HTTPS_ONLY=true
RATE_LIMITING=strict
```

---

## 📚 References

### OWASP Top 10 Coverage

| Vulnerability | Protection Layer | Status |
|--------------|------------------|--------|
| **A01:2021 – Broken Access Control** | JWT + Authorization | ✅ |
| **A02:2021 – Cryptographic Failures** | HTTPS/TLS + HSTS | ✅ |
| **A03:2021 – Injection** | EF Core Parameterized Queries | ✅ |
| **A04:2021 – Insecure Design** | Defense-in-Depth Architecture | ✅ |
| **A05:2021 – Security Misconfiguration** | Security Headers | ✅ |
| **A06:2021 – Vulnerable Components** | Regular Updates | ✅ |
| **A07:2021 – Authentication Failures** | JWT + Rate Limiting | ✅ |
| **A08:2021 – Software/Data Integrity** | Signed JWTs | ✅ |
| **A09:2021 – Logging Failures** | Serilog + Sensitive Data Masking | ✅ |
| **A10:2021 – SSRF** | Input Validation | ✅ |

### External Resources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [OWASP Cheat Sheet Series](https://cheatsheetseries.owasp.org/)
- [ASP.NET Core Security Best Practices](https://docs.microsoft.com/aspnet/core/security/)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
- [Content Security Policy](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP)

---

## 🎯 Interview Talking Points

### Defense-in-Depth Strategy

**Q: "How do you secure your APIs?"**

**A:** "I implement defense-in-depth with 7 security layers:

1. **HTTPS/TLS** - Encrypted transport
2. **Security Headers** - Browser-level protection (CSP, X-Frame-Options)
3. **JWT Authentication** - Verify identity
4. **Authorization** - Control access
5. **Input Validation** - Block malicious data
6. **Rate Limiting** - DDoS protection
7. **Monitoring** - Detect breaches

Example: Even if JWT is compromised, rate limiting prevents brute force, and CSP blocks XSS attacks."

### SQL Injection Prevention

**Q: "How do you prevent SQL injection?"**

**A:** "Multiple layers:

1. **EF Core parameterized queries** - Automatic protection
2. **Input validation** - Reject suspicious patterns
3. **Strong typing** - Use Guid/enum instead of strings
4. **Principle of least privilege** - Database user has minimal permissions

Example: `query.Where(v => v.Category == userInput)` becomes `WHERE category = @p0`, treating input as literal string, not SQL."

### Sensitive Data Protection

**Q: "How do you protect sensitive data?"**

**A:** "I use Serilog destructuring policies to automatically mask:

- Passwords → '***MASKED***'
- Credit cards → '****-****-****-1111'
- API keys → 'sk_l...123'
- Emails → 'j***n@example.com'

This prevents accidental leaks in logs, meeting PCI DSS and GDPR compliance."

---

**Last Updated:** 2025-12-02
**Security Level:** Production-Ready
**OWASP Coverage:** 10/10 ✅

---

**🔒 Security is everyone's responsibility. Stay vigilant! 🔒**
