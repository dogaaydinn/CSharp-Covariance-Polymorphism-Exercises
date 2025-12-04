# API Design Review - V2 (Fixed Design)

**Pull Request:** #2367 - User Management API V2 (After Review Fixes)  
**Author:** Jordan Lee (Junior Backend Developer)  
**Reviewer:** Marcus Rodriguez (Staff Engineer)  
**Status:** ✅ **APPROVED** - Ready for Production  
**Review Date:** 2025-12-05

---

## What Changed

After the initial review feedback, I've completely refactored the User Management API to address all security and design issues. This document shows the corrected implementation.

**Major Changes:**
- ✅ Removed password from all responses
- ✅ Added authentication/authorization
- ✅ Proper HTTP status codes (not always 200)
- ✅ RESTful endpoint design
- ✅ PATCH support for partial updates
- ✅ Soft delete instead of hard delete
- ✅ Pagination metadata
- ✅ ProblemDetails for errors
- ✅ API versioning
- ✅ Rate limiting

---

## API Endpoints (RESTful)

### 1. Create User

```http
POST /api/v1/users
Content-Type: application/json
Authorization: Bearer {admin_token}

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "SecureP@ssw0rd!",
  "phone": "555-1234",
  "role": "user"
}
```

**Response (Success - 201 Created):**
```json
HTTP/1.1 201 Created
Location: /api/v1/users/123
Content-Type: application/json

{
  "id": 123,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "phone": "555-1234",
  "role": "user",
  "status": "active",
  "createdAt": "2025-12-05T10:30:00Z",
  "updatedAt": "2025-12-05T10:30:00Z"
}
```
**Note:** ✅ No password in response!

**Response (Error - 409 Conflict):**
```json
HTTP/1.1 409 Conflict
Content-Type: application/problem+json

{
  "type": "https://api.example.com/errors/duplicate-email",
  "title": "Email already in use",
  "detail": "An account with this email already exists. Try logging in or resetting your password.",
  "status": 409,
  "instance": "/api/v1/users"
}
```

**Response (Error - 400 Bad Request):**
```json
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json

{
  "type": "https://api.example.com/errors/validation-failed",
  "title": "Validation failed",
  "detail": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "email": ["Email is required", "Email format is invalid"],
    "password": ["Password must be at least 8 characters"]
  }
}
```

---

### 2. Get User

```http
GET /api/v1/users/{id}
Authorization: Bearer {user_token}
```

**Authorization Rules:**
- Users can only see their own data
- Admins can see any user

**Response (Success - 200 OK):**
```json
HTTP/1.1 200 OK
Content-Type: application/json

{
  "id": 123,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "phone": "555-1234",
  "status": "active",
  "role": "user",
  "createdAt": "2025-12-05T10:30:00Z",
  "updatedAt": "2025-12-05T14:20:00Z"
}
```

**Response (Error - 404 Not Found):**
```json
HTTP/1.1 404 Not Found
Content-Type: application/problem+json

{
  "type": "https://api.example.com/errors/user-not-found",
  "title": "User not found",
  "detail": "The requested user does not exist or has been deleted.",
  "status": 404
}
```

**Response (Error - 403 Forbidden):**
```json
HTTP/1.1 403 Forbidden
Content-Type: application/problem+json

{
  "type": "https://api.example.com/errors/forbidden",
  "title": "Access denied",
  "detail": "You don't have permission to view this user's data.",
  "status": 403
}
```

---

### 3. Update User (Full Update)

```http
PUT /api/v1/users/{id}
Content-Type: application/json
Authorization: Bearer {user_token}

{
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@example.com",
  "phone": "555-5678"
}
```

**Response (Success - 200 OK):**
```json
HTTP/1.1 200 OK
Content-Type: application/json

{
  "id": 123,
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@example.com",
  "phone": "555-5678",
  "status": "active",
  "role": "user",
  "updatedAt": "2025-12-05T15:45:00Z"
}
```

---

### 4. Update User (Partial Update)

```http
PATCH /api/v1/users/{id}
Content-Type: application/json
Authorization: Bearer {user_token}

{
  "phone": "555-9999"
}
```

**Note:** ✅ Only send the fields you want to update!

**Response (Success - 200 OK):**
```json
HTTP/1.1 200 OK
Content-Type: application/json

{
  "id": 123,
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@example.com",
  "phone": "555-9999",
  "status": "active",
  "role": "user",
  "updatedAt": "2025-12-05T16:00:00Z"
}
```

---

### 5. Update User Status

```http
PATCH /api/v1/users/{id}/status
Content-Type: application/json
Authorization: Bearer {admin_token}

{
  "status": "inactive",
  "reason": "User requested account suspension"
}
```

**Authorization:** Admin only

**Response (Success - 200 OK):**
```json
HTTP/1.1 200 OK
Content-Type: application/json

{
  "id": 123,
  "status": "inactive",
  "updatedAt": "2025-12-05T16:15:00Z"
}
```

---

### 6. Update User Role

```http
PATCH /api/v1/users/{id}/role
Content-Type: application/json
Authorization: Bearer {super_admin_token}

{
  "role": "admin"
}
```

**Authorization:** Super Admin only

---

### 7. Delete User (Soft Delete)

```http
DELETE /api/v1/users/{id}
Authorization: Bearer {admin_token}
```

**Response (Success - 204 No Content):**
```http
HTTP/1.1 204 No Content
```

**Note:** ✅ User is soft-deleted (marked as deleted, not removed from database)

---

### 8. Permanently Delete User

```http
DELETE /api/v1/users/{id}/permanent
Content-Type: application/json
Authorization: Bearer {super_admin_token}

{
  "confirmation": "DELETE",
  "userId": 123
}
```

**Authorization:** Super Admin only + requires confirmation

**Response (Success - 204 No Content):**
```http
HTTP/1.1 204 No Content
```

---

### 9. List Users

```http
GET /api/v1/users?page=1&pageSize=10&status=active
Authorization: Bearer {admin_token}
```

**Query Parameters:**
- `page` (default: 1)
- `pageSize` (default: 10, max: 100)
- `status` (optional: active, inactive, deleted)
- `role` (optional: user, admin, superadmin)
- `sortBy` (optional: createdAt, lastName, email)
- `sortOrder` (optional: asc, desc)

**Response (Success - 200 OK):**
```json
HTTP/1.1 200 OK
Content-Type: application/json

{
  "data": [
    {
      "id": 1,
      "firstName": "Alice",
      "lastName": "Smith",
      "email": "alice@example.com",
      "status": "active",
      "role": "user"
    },
    {
      "id": 2,
      "firstName": "Bob",
      "lastName": "Jones",
      "email": "bob@example.com",
      "status": "active",
      "role": "admin"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 247,
    "totalPages": 25,
    "hasNextPage": true,
    "hasPreviousPage": false,
    "links": {
      "self": "/api/v1/users?page=1&pageSize=10",
      "next": "/api/v1/users?page=2&pageSize=10",
      "previous": null,
      "first": "/api/v1/users?page=1&pageSize=10",
      "last": "/api/v1/users?page=25&pageSize=10"
    }
  }
}
```

**Note:** ✅ Minimal data in list (only 5 fields per user, not 20+)

---

### 10. Search Users

```http
GET /api/v1/users/search?q=John&page=1&pageSize=10
Authorization: Bearer {admin_token}
```

**Query Parameters:**
- `q` (required: search query)
- `page`, `pageSize` (optional: same as list)

**Response:** Same format as List Users

---

## Controller Implementation

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using System.Security.Claims;

namespace UserManagementApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/users")]
    [Authorize] // ✅ All endpoints require authentication
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new user (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var user = await _userService.CreateAsync(request);
                var userDto = UserDto.FromEntity(user); // ✅ DTO excludes password
                
                return CreatedAtAction(
                    nameof(GetUser), 
                    new { id = user.Id }, 
                    userDto
                );
            }
            catch (DuplicateEmailException ex)
            {
                _logger.LogWarning(ex, "Duplicate email attempt: {Email}", request.Email);
                
                return Conflict(new ProblemDetails
                {
                    Type = "https://api.example.com/errors/duplicate-email",
                    Title = "Email already in use",
                    Detail = "An account with this email already exists. Try logging in or resetting your password.",
                    Status = StatusCodes.Status409Conflict,
                    Instance = HttpContext.Request.Path
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ValidationProblemDetails(ex.Errors)
                {
                    Type = "https://api.example.com/errors/validation-failed",
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });
            }
        }

        /// <summary>
        /// Get user by ID (Users can see themselves, Admins can see anyone)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "SelfOrAdmin")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUser(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            // ✅ Authorization check
            if (currentUserId != id && !isAdmin)
            {
                return Forbid(); // Returns 403 Forbidden
            }

            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
            {
                return NotFound(new ProblemDetails // ✅ Returns 404, not 200
                {
                    Type = "https://api.example.com/errors/user-not-found",
                    Title = "User not found",
                    Detail = "The requested user does not exist or has been deleted.",
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(UserDto.FromEntity(user)); // ✅ Returns 200 OK
        }

        /// <summary>
        /// Full update of user (Users can update themselves, Admins can update anyone)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "SelfOrAdmin")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
                return NotFound();

            await _userService.UpdateAsync(id, request);
            var updatedUser = await _userService.GetByIdAsync(id);
            
            return Ok(UserDto.FromEntity(updatedUser));
        }

        /// <summary>
        /// Partial update of user (only provided fields are updated)
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Policy = "SelfOrAdmin")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchUser(int id, [FromBody] PatchUserRequest request)
        {
            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
                return NotFound();

            // ✅ Only non-null fields are updated
            await _userService.PatchAsync(id, request);
            var updatedUser = await _userService.GetByIdAsync(id);
            
            return Ok(UserDto.FromEntity(updatedUser));
        }

        /// <summary>
        /// Update user status (Admin only)
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserStatusDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            await _userService.UpdateStatusAsync(id, request.Status, request.Reason);
            var user = await _userService.GetByIdAsync(id);
            
            return Ok(new UserStatusDto 
            { 
                Id = user.Id, 
                Status = user.Status, 
                UpdatedAt = user.UpdatedAt 
            });
        }

        /// <summary>
        /// Update user role (Super Admin only)
        /// </summary>
        [HttpPatch("{id}/role")]
        [Authorize(Roles = "SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
        {
            await _userService.UpdateRoleAsync(id, request.Role);
            return NoContent(); // ✅ Returns 204 No Content
        }

        /// <summary>
        /// Soft delete user (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
                return NotFound();

            await _userService.SoftDeleteAsync(id); // ✅ Soft delete
            
            _logger.LogInformation("User {UserId} soft-deleted by {AdminId}", id, User.Identity.Name);
            
            return NoContent();
        }

        /// <summary>
        /// Permanently delete user (Super Admin only, requires confirmation)
        /// </summary>
        [HttpDelete("{id}/permanent")]
        [Authorize(Roles = "SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PermanentlyDeleteUser(
            int id, 
            [FromBody] DeleteConfirmationRequest confirmation)
        {
            if (confirmation.Confirmation != "DELETE" || confirmation.UserId != id)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Confirmation required",
                    Detail = "You must provide correct confirmation to permanently delete a user.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            await _userService.HardDeleteAsync(id); // ✅ Hard delete
            
            _logger.LogWarning(
                "User {UserId} PERMANENTLY deleted by {AdminId}", 
                id, 
                User.Identity.Name
            );
            
            return NoContent();
        }

        /// <summary>
        /// List all users with pagination (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PagedResponse<UserListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListUsers(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? role = null,
            [FromQuery] string sortBy = "createdAt",
            [FromQuery] string sortOrder = "desc")
        {
            if (pageSize > 100)
                pageSize = 100; // ✅ Cap maximum page size

            var result = await _userService.GetPagedAsync(
                page, 
                pageSize, 
                status, 
                role, 
                sortBy, 
                sortOrder
            );

            var response = new PagedResponse<UserListDto>
            {
                Data = result.Items.Select(UserListDto.FromEntity).ToList(),
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = result.TotalCount,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                    HasNextPage = page < (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                    HasPreviousPage = page > 1,
                    Links = new PaginationLinks
                    {
                        Self = $"/api/v1/users?page={page}&pageSize={pageSize}",
                        Next = page < result.TotalCount / pageSize 
                            ? $"/api/v1/users?page={page + 1}&pageSize={pageSize}" 
                            : null,
                        Previous = page > 1 
                            ? $"/api/v1/users?page={page - 1}&pageSize={pageSize}" 
                            : null,
                        First = $"/api/v1/users?page=1&pageSize={pageSize}",
                        Last = $"/api/v1/users?page={result.TotalCount / pageSize}&pageSize={pageSize}"
                    }
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// Search users (Admin only)
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PagedResponse<UserListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string q,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Search query required",
                    Detail = "The 'q' parameter is required for search.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var result = await _userService.SearchAsync(q, page, pageSize);
            
            // ... same pagination response as ListUsers
            
            return Ok(response);
        }
    }
}
```

---

## Data Transfer Objects (DTOs)

```csharp
// ✅ UserDto - NEVER includes password!
public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Status { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static UserDto FromEntity(User user) => new UserDto
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Phone = user.Phone,
        Status = user.Status,
        Role = user.Role,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
        // ✅ Password is explicitly excluded
    };
}

// ✅ UserListDto - Minimal data for list views
public class UserListDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
    public string Role { get; set; }

    public static UserListDto FromEntity(User user) => new UserListDto
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Status = user.Status,
        Role = user.Role
        // ✅ Only 6 fields instead of 20+
    };
}

// ✅ PatchUserRequest - All fields nullable
public class PatchUserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    
    // ✅ Only non-null fields will be updated
}
```

---

## Authentication & Authorization Setup

```csharp
// Program.cs or Startup.cs

// ✅ Add JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });

// ✅ Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SelfOrAdmin", policy =>
        policy.RequireAssertion(context =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var requestedUserId = context.Resource as string;
            var isAdmin = context.User.IsInRole("Admin");
            
            return userId == requestedUserId || isAdmin;
        }));
});

// ✅ Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

// ✅ Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

app.UseAuthentication(); // ✅ Must come before UseAuthorization
app.UseAuthorization();
app.UseRateLimiter();
```

---

## What I Learned

### Security
- ✅ **Never return passwords** - Even hashed passwords shouldn't be exposed
- ✅ **Always authenticate** - Every endpoint requires authentication
- ✅ **Proper authorization** - Users can only see/modify their own data, admins have broader access
- ✅ **Don't expose exception messages** - Use ProblemDetails with user-friendly messages
- ✅ **Rate limiting** - Prevent abuse and DDoS attacks

### REST API Design
- ✅ **Use proper HTTP methods** - GET (read), POST (create), PUT (full update), PATCH (partial), DELETE
- ✅ **Use proper status codes** - 200 OK, 201 Created, 204 No Content, 400 Bad Request, 404 Not Found, 409 Conflict
- ✅ **RESTful URLs** - `/users/{id}` not `/users/get?id=123`
- ✅ **Nouns in URLs, verbs in HTTP methods** - `/users` not `/getUsers`

### Data Management
- ✅ **DTOs separate from entities** - Don't expose database models directly
- ✅ **Soft delete by default** - Keep audit trail, allow recovery
- ✅ **Pagination metadata** - Make it easy for clients to navigate
- ✅ **Minimal data in lists** - Don't return 20 fields when 5 will do

### Best Practices
- ✅ **API versioning** - Prepare for breaking changes
- ✅ **PATCH for partial updates** - Don't require all fields
- ✅ **Logging** - Log important actions (deletions, role changes)
- ✅ **ProblemDetails** - Standard error format (RFC 7807)

---

## Comparison: V1 vs V2

| Feature | V1 (Bad) | V2 (Fixed) |
|---------|----------|------------|
| **Password in response** | ❌ Exposed | ✅ Never returned |
| **Authentication** | ❌ None | ✅ JWT required |
| **Authorization** | ❌ None | ✅ Role-based + policies |
| **HTTP status codes** | ❌ Always 200 | ✅ Proper codes |
| **Endpoint design** | ❌ `/users/get?id=` | ✅ `/users/{id}` |
| **Error format** | ❌ `{success: false}` | ✅ ProblemDetails |
| **Partial updates** | ❌ Must send all fields | ✅ PATCH support |
| **Delete strategy** | ❌ Hard delete | ✅ Soft delete |
| **Pagination** | ❌ No metadata | ✅ Full metadata |
| **Rate limiting** | ❌ None | ✅ Implemented |
| **API versioning** | ❌ None | ✅ /v1/ prefix |
| **List response** | ❌ 20+ fields | ✅ 6 fields |

---

## Testing the API

```bash
# 1. Get JWT token
curl -X POST https://api.example.com/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@example.com", "password": "password"}'

# Response: {"token": "eyJhbGc..."}

# 2. Create user (Admin only)
curl -X POST https://api.example.com/api/v1/users \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGc..." \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "password": "SecureP@ss123",
    "phone": "555-1234"
  }'

# Response: 201 Created
# {
#   "id": 123,
#   "firstName": "John",
#   ...
#   // ✅ No password!
# }

# 3. Get user
curl https://api.example.com/api/v1/users/123 \
  -H "Authorization: Bearer eyJhbGc..."

# Response: 200 OK (if authorized) or 403 Forbidden

# 4. Partial update (only phone)
curl -X PATCH https://api.example.com/api/v1/users/123 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGc..." \
  -d '{"phone": "555-9999"}'

# Response: 200 OK with updated user

# 5. List users with pagination
curl "https://api.example.com/api/v1/users?page=1&pageSize=10&status=active" \
  -H "Authorization: Bearer eyJhbGc..."

# Response: 200 OK with data + pagination metadata

# 6. Try to access without authentication
curl https://api.example.com/api/v1/users/123

# Response: 401 Unauthorized

# 7. Try to access other user's data (non-admin)
curl https://api.example.com/api/v1/users/999 \
  -H "Authorization: Bearer <user_123_token>"

# Response: 403 Forbidden
```

---

## Next Steps

**Done:**
- ✅ All critical security issues fixed
- ✅ RESTful API design implemented
- ✅ Proper HTTP status codes
- ✅ Authentication and authorization
- ✅ PATCH support for partial updates
- ✅ Soft delete with audit logging
- ✅ Pagination metadata
- ✅ API versioning

**Future Enhancements:**
- [ ] OpenAPI/Swagger documentation
- [ ] Request validation with FluentValidation
- [ ] Integration tests (xUnit + WebApplicationFactory)
- [ ] CORS configuration
- [ ] Response caching
- [ ] Health checks
- [ ] Distributed tracing (OpenTelemetry)

---

## Resources

- **Marcus's Review:** See `review-feedback.md` for detailed explanations
- **REST API Best Practices:** https://restfulapi.net/
- **ProblemDetails Spec:** RFC 7807
- **OWASP API Security:** https://owasp.org/www-project-api-security/
- **This Repository:** `samples/05-RealWorld/WebApiAdvanced/` for production examples

---

## Acknowledgments

**Thank you, Marcus!** Your detailed review helped me understand:
- Why security matters (not just "best practices")
- How REST conventions make APIs predictable
- The importance of proper HTTP semantics
- How to write maintainable, production-ready code

This was a huge learning opportunity. I went from "code that works" to "code that's secure, maintainable, and follows industry standards."

**Status:** ✅ Ready for production deployment

---

**Reviewer's Final Comment (Marcus Rodriguez):**

> Jordan, this is EXCELLENT work! You took every piece of feedback seriously and implemented all the critical fixes. The API now follows industry standards and is production-ready.
> 
> Highlights:
> - Security is solid (no password exposure, proper auth)
> - RESTful design is correct
> - Error handling is professional (ProblemDetails)
> - Code is well-documented with XML comments
> - Authorization logic is clear
> 
> **APPROVED FOR PRODUCTION** ✅
> 
> You've leveled up significantly with this PR. Keep this standard for all future APIs!
> 
> — Marcus

