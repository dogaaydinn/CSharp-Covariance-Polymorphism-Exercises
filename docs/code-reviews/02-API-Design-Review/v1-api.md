# API Design Review - V1 (Bad Design)

**Pull Request:** #2345 - Add User Management API  
**Author:** Jordan Lee (Junior Backend Developer)  
**Reviewer:** Marcus Rodriguez (Staff Engineer)

---

## Proposed API Endpoints

### 1. Create User

```http
POST /api/users/create
Content-Type: application/json

{
  "firstname": "John",
  "lastname": "Doe",
  "email": "john@example.com",
  "password": "password123",
  "phone": "555-1234",
  "address": "123 Main St",
  "city": "NYC",
  "state": "NY",
  "zip": "10001",
  "country": "USA",
  "dob": "1990-01-15",
  "role": "user",
  "status": "active",
  "newsletter": true
}
```

**Response (Success):**
```json
{
  "success": true,
  "user": {
    "id": 123,
    "firstname": "John",
    "lastname": "Doe",
    "email": "john@example.com",
    "phone": "555-1234",
    "address": "123 Main St",
    "city": "NYC",
    "state": "NY",
    "zip": "10001",
    "country": "USA",
    "dob": "1990-01-15",
    "role": "user",
    "status": "active",
    "newsletter": true,
    "created_at": "2025-12-03T10:30:00Z"
  }
}
```

**Response (Error):**
```json
{
  "success": false,
  "error": "Email already exists"
}
```

---

### 2. Get User

```http
GET /api/users/get?id=123
```

**Response:**
```json
{
  "success": true,
  "user": {
    "id": 123,
    "firstname": "John",
    "lastname": "Doe",
    "email": "john@example.com",
    "password": "$2b$10$abcdefg...", // ❌ SECURITY ISSUE!
    "phone": "555-1234",
    "address": "123 Main St",
    "city": "NYC",
    "state": "NY",
    "zip": "10001",
    "country": "USA",
    "dob": "1990-01-15",
    "role": "user",
    "status": "active",
    "newsletter": true,
    "created_at": "2025-12-03T10:30:00Z",
    "updated_at": "2025-12-03T10:30:00Z",
    "last_login": "2025-12-03T14:20:00Z",
    "login_count": 47,
    "failed_login_attempts": 0,
    "ip_address": "192.168.1.100"
  }
}
```

---

### 3. Update User

```http
PUT /api/users/update
Content-Type: application/json

{
  "id": 123,
  "firstname": "John",
  "lastname": "Smith",
  "email": "john.smith@example.com",
  "phone": "555-5678",
  "address": "456 Oak Ave",
  "city": "LA",
  "state": "CA",
  "zip": "90001",
  "country": "USA",
  "dob": "1990-01-15",
  "role": "user",
  "status": "active",
  "newsletter": false
}
```

**Note:** ALL fields are required in the request, even if only updating one field!

---

### 4. Delete User

```http
DELETE /api/users/delete?id=123
```

**Response:**
```json
{
  "success": true,
  "message": "User deleted"
}
```

**Note:** Hard delete - user data is permanently removed from database!

---

### 5. List Users

```http
GET /api/users/list?page=1&size=10
```

**Response:**
```json
{
  "success": true,
  "users": [
    {
      "id": 1,
      "firstname": "Alice",
      "lastname": "Smith",
      "email": "alice@example.com",
      "password": "$2b$10$xyz...", // ❌ Exposed in list too!
      // ... all user fields ...
    },
    {
      "id": 2,
      "firstname": "Bob",
      "lastname": "Jones",
      // ... all user fields ...
    }
    // ... 10 users total
  ],
  "total": 247
}
```

---

### 6. Search Users

```http
GET /api/users/search?query=John&field=firstname
```

**Response:** Same as List Users

---

### 7. Update User Status

```http
POST /api/users/updateStatus
Content-Type: application/json

{
  "id": 123,
  "status": "inactive"
}
```

---

### 8. Update User Role

```http
POST /api/users/updateRole
Content-Type: application/json

{
  "id": 123,
  "role": "admin"
}
```

---

## Controller Implementation

```csharp
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    [HttpPost("create")]
    public IActionResult CreateUser([FromBody] UserRequest request)
    {
        try
        {
            var user = _userService.Create(request);
            return Ok(new { success = true, user });
        }
        catch (Exception ex)
        {
            return Ok(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("get")]
    public IActionResult GetUser([FromQuery] int id)
    {
        var user = _userService.GetById(id);
        if (user == null)
            return Ok(new { success = false, error = "User not found" });
        
        return Ok(new { success = true, user });
    }

    [HttpPut("update")]
    public IActionResult UpdateUser([FromBody] UserRequest request)
    {
        if (request.Id == 0)
            return Ok(new { success = false, error = "ID is required" });

        try
        {
            var user = _userService.Update(request);
            return Ok(new { success = true, user });
        }
        catch (Exception ex)
        {
            return Ok(new { success = false, error = ex.Message });
        }
    }

    [HttpDelete("delete")]
    public IActionResult DeleteUser([FromQuery] int id)
    {
        try
        {
            _userService.Delete(id);
            return Ok(new { success = true, message = "User deleted" });
        }
        catch (Exception ex)
        {
            return Ok(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("list")]
    public IActionResult ListUsers([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var users = _userService.GetAll(page, size);
        var total = _userService.Count();
        return Ok(new { success = true, users, total });
    }

    [HttpGet("search")]
    public IActionResult SearchUsers([FromQuery] string query, [FromQuery] string field)
    {
        var users = _userService.Search(query, field);
        return Ok(new { success = true, users });
    }

    [HttpPost("updateStatus")]
    public IActionResult UpdateStatus([FromBody] UpdateStatusRequest request)
    {
        try
        {
            _userService.UpdateStatus(request.Id, request.Status);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return Ok(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("updateRole")]
    public IActionResult UpdateRole([FromBody] UpdateRoleRequest request)
    {
        try
        {
            _userService.UpdateRole(request.Id, request.Role);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return Ok(new { success = false, error = ex.Message });
        }
    }
}
```

---

## What Could Go Wrong?

**Author's Note:**
> "I tested all endpoints with Postman and they work! Users can be created, updated, and deleted successfully. Ready to deploy to production!"

**Read `review-feedback.md` to see what the Staff Engineer caught... 😱**
