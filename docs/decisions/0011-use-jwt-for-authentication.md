# 11. Use JWT for Authentication

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, Security Team

**Technical Story:** Implementation in API authentication middleware

---

## Context and Problem Statement

Microservices need stateless authentication that works across:
- Multiple API instances (load balanced)
- Multiple services (distributed architecture)
- Mobile apps, SPAs, and server-side clients
- Cross-domain scenarios (CORS)

**Traditional session-based authentication problems:**
- Sessions stored server-side (not stateless)
- Doesn't work with load balancers (sticky sessions required)
- Doesn't scale horizontally
- Can't share authentication across services

**Requirements:**
- Stateless (no server-side session storage)
- Works with load balancers
- Can be validated by any service
- Supports claims (roles, permissions)
- Industry standard

---

## Decision Drivers

* **Stateless** - No server-side session storage
* **Scalability** - Works with multiple instances
* **Standards-Based** - RFC 7519 (JWT)
* **Self-Contained** - Token includes all claims
* **Cross-Service** - Share authentication across microservices
* **Mobile/SPA Support** - Works with modern frontends

---

## Considered Options

* **Option 1** - JWT (JSON Web Tokens)
* **Option 2** - Session-based authentication (cookies)
* **Option 3** - OAuth 2.0 + OpenID Connect (OIDC)
* **Option 4** - API Keys

---

## Decision Outcome

**Chosen option:** "JWT", because it provides stateless, self-contained tokens that can be validated by any service without database lookups, works seamlessly with load balancers, and is the industry standard for API authentication.

### Positive Consequences

* **Stateless** - No session storage required
* **Scalable** - Works with any number of instances
* **Self-Contained** - All claims in token
* **Cross-Service** - Share authentication across microservices
* **Industry Standard** - RFC 7519, widely supported
* **Mobile/SPA Friendly** - Easy to use in modern frontends
* **Performance** - No database lookup for every request

### Negative Consequences

* **Token Size** - Larger than session ID (200-1000 bytes)
* **Can't Revoke** - Tokens valid until expiration (use short TTL + refresh tokens)
* **Stolen Tokens** - If token leaked, attacker has access until expiration
* **Implementation Complexity** - Must handle refresh tokens, token rotation

---

## Pros and Cons of the Options

### JWT (Chosen)

**What is JWT?**

JSON Web Token (JWT) is a compact, URL-safe token format for securely transmitting claims between parties. Consists of three parts: Header.Payload.Signature.

**Structure:**
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c

Decoded:
Header:  { "alg": "HS256", "typ": "JWT" }
Payload: { "sub": "1234567890", "name": "John Doe", "iat": 1516239022 }
Signature: HMACSHA256(base64UrlEncode(header) + "." + base64UrlEncode(payload), secret)
```

**Pros:**
* **Stateless** - Server doesn't store sessions
* **Self-contained** - All user info in token
* **Portable** - Works across services, languages, platforms
* **Compact** - Suitable for HTTP headers
* **Standard** - RFC 7519, battle-tested
* **Flexible claims** - Add custom data (roles, permissions, tenant ID)

**Cons:**
* **Can't revoke** - Valid until expiration (mitigation: short TTL)
* **Size** - Larger than session ID (sent with every request)
* **Complexity** - Need refresh token mechanism
* **Secret management** - Must protect signing key

**Implementation (.NET 8):**

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

```csharp
// Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add JWT authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero  // No tolerance for expired tokens
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
```

**appsettings.json:**
```json
{
  "Jwt": {
    "Key": "your-256-bit-secret-key-here-min-32-chars",
    "Issuer": "https://yourdomain.com",
    "Audience": "https://yourdomain.com",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

**Token Generation Service:**
```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            // Custom claims
            new Claim("tenant_id", user.TenantId.ToString()),
            new Claim("subscription_tier", user.SubscriptionTier)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]!)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
            ValidateLifetime = false  // Don't validate expiration for refresh
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
```

**Authentication Controller:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly IUserRepository _userRepository;

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        // Validate credentials
        var user = await _userRepository.ValidateCredentialsAsync(request.Email, request.Password);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Store refresh token (database or Redis)
        await _userRepository.SaveRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

        return Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 900  // 15 minutes
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest request)
    {
        // Validate refresh token
        var storedToken = await _userRepository.GetRefreshTokenAsync(request.RefreshToken);
        if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        // Get user from expired access token
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userId = int.Parse(principal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

        var user = await _userRepository.GetByIdAsync(userId);

        // Generate new tokens
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Update refresh token in database
        await _userRepository.UpdateRefreshTokenAsync(userId, newRefreshToken, DateTime.UtcNow.AddDays(7));

        return Ok(new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = 900
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

        // Revoke refresh token
        await _userRepository.RevokeRefreshTokensAsync(userId);

        return Ok(new { message = "Logged out successfully" });
    }
}
```

**Usage in Controllers:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Requires valid JWT
public class ProductsController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,User")]  // Role-based authorization
    public async Task<ActionResult<List<Product>>> GetAll()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;
        var userEmail = User.FindFirst(JwtRegisteredClaimNames.Email)!.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

        // Business logic
        return Ok(products);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]  // Policy-based authorization
    public async Task<ActionResult<Product>> Create([FromBody] Product product)
    {
        // Only admins can create products
        return Ok(createdProduct);
    }
}

// Define policies in Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("PremiumUser", policy =>
        policy.RequireClaim("subscription_tier", "Premium", "Enterprise"));
});
```

**Client Usage (Frontend):**
```javascript
// Login
const response = await fetch('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email: 'user@example.com', password: 'password123' })
});

const { accessToken, refreshToken } = await response.json();

// Store tokens (localStorage or sessionStorage)
localStorage.setItem('accessToken', accessToken);
localStorage.setItem('refreshToken', refreshToken);

// Use token for API calls
const products = await fetch('/api/products', {
    headers: {
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
    }
});

// Refresh token when access token expires
async function refreshAccessToken() {
    const response = await fetch('/api/auth/refresh', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            accessToken: localStorage.getItem('accessToken'),
            refreshToken: localStorage.getItem('refreshToken')
        })
    });

    const { accessToken, refreshToken } = await response.json();
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
}
```

### Session-Based Authentication (Cookies)

**Pros:**
* Simple implementation
* Can revoke immediately (delete session)
* Smaller overhead (just session ID)
* Automatic CSRF protection with SameSite cookies

**Cons:**
* **Stateful** - Requires server-side session storage
* **Doesn't scale** - Need sticky sessions or shared session store
* **CORS issues** - Cookies don't work well cross-domain
* **Not API-friendly** - Requires cookie support

**Why Not Chosen:**
Session-based auth works for monolithic web apps, but microservices need **stateless** authentication that scales horizontally.

### OAuth 2.0 + OpenID Connect

**Pros:**
* Industry standard for delegated authorization
* Works with social logins (Google, GitHub, etc.)
* Federated identity
* Fine-grained scopes

**Cons:**
* **Complex setup** - Requires Authorization Server
* **Overkill** - For internal APIs, JWT is simpler
* **External dependency** - Need IdP (IdentityServer, Auth0, Okta)

**When to Use:**
- Need social login
- Federated identity across organizations
- Third-party API access

**Decision:**
For **internal microservices**, JWT is simpler. For **external APIs** or **social login**, use OAuth 2.0/OIDC with a provider like Auth0 or IdentityServer.

### API Keys

**Pros:**
* Very simple
* Good for machine-to-machine
* Never expire (if needed)

**Cons:**
* **No user context** - Just identifies application
* **No claims** - Can't encode roles, permissions
* **Rotation complexity** - Hard to rotate keys
* **Not standard** - No RFC

**When to Use:**
- Machine-to-machine communication
- Third-party API access (rate limiting)

**Why Not Primary Choice:**
API keys lack user context and claims. JWT provides both authentication and authorization.

---

## Security Best Practices

**1. Use HTTPS Only:**
```csharp
// Require HTTPS
app.UseHttpsRedirection();
```

**2. Short Access Token TTL:**
```json
{
  "Jwt": {
    "AccessTokenExpirationMinutes": 15,  // ✅ Short TTL
    "RefreshTokenExpirationDays": 7
  }
}
```

**3. Secure Secret Storage:**
```bash
# Azure Key Vault
dotnet user-secrets set "Jwt:Key" "your-secret-key"

# Environment variable (production)
export JWT_KEY="your-secret-key"
```

**4. Validate All Claims:**
```csharp
ValidateIssuer = true,
ValidateAudience = true,
ValidateLifetime = true,
ValidateIssuerSigningKey = true,
ClockSkew = TimeSpan.Zero  // No tolerance
```

**5. Use Refresh Token Rotation:**
```csharp
// Generate new refresh token on every refresh
var newRefreshToken = _tokenService.GenerateRefreshToken();
await _userRepository.RevokeOldRefreshTokenAsync(oldRefreshToken);
await _userRepository.SaveRefreshTokenAsync(userId, newRefreshToken, expiresAt);
```

---

## Links

* [JWT.io](https://jwt.io/)
* [RFC 7519 - JSON Web Token](https://datatracker.ietf.org/doc/html/rfc7519)
* [OWASP JWT Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html)
* [Microsoft JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)

---

## Notes

**Token Lifecycle:**
1. User logs in → Receive access token (15 min) + refresh token (7 days)
2. Use access token for API calls
3. When access token expires → Use refresh token to get new access token
4. When refresh token expires → User must log in again

**Common Mistakes:**
- ❌ Storing JWT in localStorage (XSS vulnerability) - Use httpOnly cookies for web
- ❌ Not validating token expiration
- ❌ Using weak secret keys (< 256 bits)
- ❌ Not revoking refresh tokens on logout

**Review Date:** 2025-12-01
