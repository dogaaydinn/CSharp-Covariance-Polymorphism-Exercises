// PR #145: Add User Management API - REFACTORED VERSION
// Author: Junior Developer (with Senior mentorship)
// Date: 2024-12-06 (After pair programming + 3 days work)
// Description: Production-ready REST API with security, versioning, DTOs, pagination

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UserManagementAPI.V1
{
    // ✅ FIX 1: API Versioning
    // ✅ FIX 2: Authentication required for all endpoints
    [ApiController]
    [Route("api/v1/users")] // ← Version number!
    [Authorize] // ← Authentication required
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        // ✅ FIX 3: Dependency injection (testable, loosely coupled)
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ✅ FIX 4: Proper HTTP verb (POST for create)
        // ✅ FIX 5: Returns DTO (no password)
        // ✅ FIX 6: Proper status codes (201 Created)
        // ✅ FIX 7: Swagger documentation
        /// <summary>
        /// Creates a new user account
        /// </summary>
        /// <param name="request">User creation request</param>
        /// <returns>Created user details</returns>
        /// <response code="201">User successfully created</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="409">Email already exists</response>
        [HttpPost]
        [AllowAnonymous] // Public registration
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
        {
            // FIX: Model validation
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid user creation request: {Errors}", ModelState);
                return BadRequest(ModelState);
            }

            // FIX: Check if email exists
            if (await _userService.EmailExistsAsync(request.Email))
            {
                _logger.LogWarning("Attempted to create user with existing email: {Email}", request.Email);
                return Conflict(new ProblemDetails
                {
                    Title = "Email already exists",
                    Detail = $"A user with email '{request.Email}' already exists.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            // FIX: Service layer handles business logic
            var user = await _userService.CreateAsync(request);

            _logger.LogInformation("User created successfully: {UserId}", user.Id);

            // FIX: Return 201 Created with location header
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // ✅ FIX 8: Pagination (not returning all users)
        /// <summary>
        /// Gets a paginated list of users
        /// </summary>
        /// <param name="pagingParams">Pagination parameters</param>
        /// <returns>Paginated user list</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")] // Only admins can list all users
        [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<UserDto>>> GetUsers([FromQuery] PagingParams pagingParams)
        {
            _logger.LogInformation("Fetching users: Page {Page}, PageSize {PageSize}",
                pagingParams.PageNumber, pagingParams.PageSize);

            var users = await _userService.GetPagedUsersAsync(pagingParams);

            return Ok(users);
        }

        // ✅ FIX 9: Proper 404 handling
        /// <summary>
        /// Gets a user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        /// <response code="200">User found</response>
        /// <response code="404">User not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            _logger.LogDebug("Fetching user: {UserId}", id);

            var user = await _userService.GetByIdAsync(id);

            // FIX: Return 404 instead of null
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", id);
                return NotFound(new ProblemDetails
                {
                    Title = "User not found",
                    Detail = $"User with ID {id} was not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(user);
        }

        // ✅ FIX 10: Proper HTTP verb (DELETE)
        // ✅ FIX 11: Proper status code (204 No Content)
        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="id">User ID to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">User deleted successfully</response>
        /// <response code="404">User not found</response>
        /// <response code="403">Not authorized to delete this user</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("Attempting to delete user: {UserId}", id);

            // FIX: Authorization check (can't delete yourself)
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (currentUserId == id)
            {
                _logger.LogWarning("User attempted to delete themselves: {UserId}", id);
                return Forbid();
            }

            var deleted = await _userService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Attempted to delete non-existent user: {UserId}", id);
                return NotFound(new ProblemDetails
                {
                    Title = "User not found",
                    Detail = $"User with ID {id} was not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            _logger.LogInformation("User deleted successfully: {UserId}", id);

            // FIX: Return 204 No Content (standard for successful DELETE)
            return NoContent();
        }

        // ✅ FIX 12: PUT for update (proper HTTP verb)
        // ✅ FIX 13: Uses UpdateUserRequest DTO (doesn't accept Id, Password, etc.)
        /// <summary>
        /// Updates a user's profile
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="request">Update request</param>
        /// <returns>Updated user details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // FIX: Authorization check (can only update yourself unless admin)
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var isAdmin = User.IsInRole("Admin");

            if (currentUserId != id && !isAdmin)
            {
                _logger.LogWarning("User {CurrentUserId} attempted to update user {TargetUserId} without permission",
                    currentUserId, id);
                return Forbid();
            }

            var updated = await _userService.UpdateAsync(id, request);

            if (updated == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User not found",
                    Detail = $"User with ID {id} was not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            _logger.LogInformation("User updated successfully: {UserId}", id);

            return Ok(updated);
        }

        // ✅ FIX 14: Search with pagination
        /// <summary>
        /// Searches users by name or email
        /// </summary>
        /// <param name="query">Search query</param>
        /// <param name="pagingParams">Pagination parameters</param>
        /// <returns>Paginated search results</returns>
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<UserDto>>> SearchUsers(
            [FromQuery] string query,
            [FromQuery] PagingParams pagingParams)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid search query",
                    Detail = "Search query cannot be empty.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _logger.LogInformation("Searching users: Query={Query}, Page={Page}",
                query, pagingParams.PageNumber);

            var results = await _userService.SearchAsync(query, pagingParams);

            return Ok(results);
        }

        // ✅ FIX 15: Separate endpoint for password change
        /// <summary>
        /// Changes user's password
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="request">Password change request</param>
        /// <returns>No content</returns>
        [HttpPost("{id}/change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // FIX: Can only change your own password
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (currentUserId != id)
            {
                return Forbid();
            }

            var success = await _userService.ChangePasswordAsync(id, request.CurrentPassword, request.NewPassword);

            if (!success)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Password change failed",
                    Detail = "Current password is incorrect.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _logger.LogInformation("Password changed successfully: {UserId}", id);

            return NoContent();
        }
    }

    // ===================================================================
    // DTOs (Data Transfer Objects)
    // ===================================================================

    // ✅ FIX 16: Request DTO (only fields client can provide)
    /// <summary>
    /// Request to create a new user
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// User's full name
        /// </summary>
        /// <example>John Doe</example>
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        /// <summary>
        /// User's email address
        /// </summary>
        /// <example>john.doe@example.com</example>
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        /// <summary>
        /// User's password
        /// </summary>
        /// <example>SecureP@ssw0rd!</example>
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$",
            ErrorMessage = "Password must contain uppercase, lowercase, digit, and special character")]
        public string Password { get; set; }
    }

    // ✅ FIX 17: Update DTO (only fields that can be updated)
    /// <summary>
    /// Request to update user profile
    /// </summary>
    public class UpdateUserRequest
    {
        /// <summary>
        /// User's full name
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        // Note: Email and Password NOT included in update
        // Email change requires verification
        // Password change has separate endpoint
    }

    // ✅ FIX 18: Password change DTO
    /// <summary>
    /// Request to change password
    /// </summary>
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$")]
        public string NewPassword { get; set; }
    }

    // ✅ FIX 19: Response DTO (NEVER includes password!)
    /// <summary>
    /// User details returned by API
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        // NO PASSWORD FIELD!
    }

    // ✅ FIX 20: Pagination parameters
    /// <summary>
    /// Pagination parameters
    /// </summary>
    public class PagingParams
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 20;

        /// <summary>
        /// Page number (1-based)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of items per page (max 100)
        /// </summary>
        [Range(1, MaxPageSize, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Min(value, MaxPageSize);
        }
    }

    // ✅ FIX 21: Paginated response
    /// <summary>
    /// Paginated result
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }

    // ===================================================================
    // Service Layer
    // ===================================================================

    // ✅ FIX 22: Service interface
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id);
        Task<PagedResult<UserDto>> GetPagedUsersAsync(PagingParams pagingParams);
        Task<PagedResult<UserDto>> SearchAsync(string query, PagingParams pagingParams);
        Task<UserDto> CreateAsync(CreateUserRequest request);
        Task<UserDto> UpdateAsync(int id, UpdateUserRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword);
    }

    // ✅ FIX 23: Service implementation (business logic)
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository repository,
            IPasswordHasher<User> passwordHasher,
            ILogger<UserService> logger)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<UserDto> CreateAsync(CreateUserRequest request)
        {
            // FIX: Hash password before storing
            var user = new User
            {
                Name = request.Name,
                Email = request.Email.ToLowerInvariant(), // Normalize email
                PasswordHash = _passwordHasher.HashPassword(null, request.Password),
                CreatedAt = DateTime.UtcNow // FIX: Use UTC!
            };

            await _repository.AddAsync(user);

            _logger.LogInformation("User created: {Email}", user.Email);

            // Map to DTO
            return MapToDto(user);
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<PagedResult<UserDto>> GetPagedUsersAsync(PagingParams pagingParams)
        {
            var (users, totalCount) = await _repository.GetPagedAsync(pagingParams.PageNumber, pagingParams.PageSize);

            return new PagedResult<UserDto>
            {
                Items = users.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pagingParams.PageNumber,
                PageSize = pagingParams.PageSize
            };
        }

        public async Task<PagedResult<UserDto>> SearchAsync(string query, PagingParams pagingParams)
        {
            var (users, totalCount) = await _repository.SearchAsync(
                query.ToLowerInvariant(),
                pagingParams.PageNumber,
                pagingParams.PageSize);

            return new PagedResult<UserDto>
            {
                Items = users.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pagingParams.PageNumber,
                PageSize = pagingParams.PageSize
            };
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserRequest request)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            user.Name = request.Name;
            // Note: NOT updating email or password here

            await _repository.UpdateAsync(user);

            _logger.LogInformation("User updated: {UserId}", id);

            return MapToDto(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (deleted)
            {
                _logger.LogInformation("User deleted: {UserId}", id);
            }

            return deleted;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _repository.EmailExistsAsync(email.ToLowerInvariant());
        }

        public async Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            // Verify current password
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Failed password change attempt: {UserId}", id);
                return false;
            }

            // Hash new password
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await _repository.UpdateAsync(user);

            _logger.LogInformation("Password changed: {UserId}", id);

            return true;
        }

        // FIX: Mapping method (converts domain model to DTO)
        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
                // NO PASSWORD!
            };
        }
    }

    // ===================================================================
    // Domain Model (NEVER exposed directly to API)
    // ===================================================================

    // ✅ FIX 24: Domain model separate from API
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // NOT Password!
        public DateTime CreatedAt { get; set; }
    }

    // ===================================================================
    // Repository Interface
    // ===================================================================

    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<(List<User> users, int totalCount)> GetPagedAsync(int pageNumber, int pageSize);
        Task<(List<User> users, int totalCount)> SearchAsync(string query, int pageNumber, int pageSize);
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}

/*
 * ✅ FIXES SUMMARY:
 *
 * SECURITY:
 * 1. ✅ Password hashing (IPasswordHasher, not plaintext)
 * 2. ✅ Authentication ([Authorize] attribute)
 * 3. ✅ Authorization (role-based, self-only for updates)
 * 4. ✅ DTOs (no Password field in responses)
 * 5. ✅ Input validation (data annotations)
 *
 * API DESIGN:
 * 6. ✅ API versioning (/api/v1/users)
 * 7. ✅ Correct HTTP verbs (POST create, GET read, PUT update, DELETE delete)
 * 8. ✅ Proper status codes (201, 204, 400, 404, 409, etc.)
 * 9. ✅ DTOs (Request/Response separation)
 * 10. ✅ Pagination (all list endpoints)
 *
 * ARCHITECTURE:
 * 11. ✅ Service layer (business logic out of controller)
 * 12. ✅ Repository pattern (data access abstraction)
 * 13. ✅ Dependency injection (testable, loosely coupled)
 * 14. ✅ Logging (ILogger, structured)
 *
 * DOCUMENTATION:
 * 15. ✅ Swagger XML comments
 * 16. ✅ ProducesResponseType attributes
 * 17. ✅ Example values in DTOs
 *
 * PERFORMANCE:
 * 18. ✅ Pagination (prevents large response payloads)
 * 19. ✅ Async/await (scalability)
 *
 * BEFORE vs AFTER:
 *
 * Security:
 * BEFORE: Passwords stored/returned in plaintext, no auth
 * AFTER: Hashed passwords, JWT auth, role-based authorization
 *
 * HTTP Verbs:
 * BEFORE: GET for create/delete (dangerous!)
 * AFTER: POST for create, DELETE for delete (correct!)
 *
 * Status Codes:
 * BEFORE: Always 200 OK (even for errors)
 * AFTER: 201 Created, 204 No Content, 404 Not Found, etc.
 *
 * DTOs:
 * BEFORE: Domain model exposed directly (includes Password!)
 * AFTER: Separate Request/Response DTOs (no Password in response)
 *
 * Pagination:
 * BEFORE: Returns all 100,000 users (timeout!)
 * AFTER: Returns 20 users per page (fast!)
 *
 * Architecture:
 * BEFORE: Controller does everything (untestable)
 * AFTER: Controller → Service → Repository (testable, maintainable)
 */
