// PR #145: Add User Management API
// Author: Junior Developer (8 months experience)
// Date: 2024-12-03
// Description: REST API endpoints for user management (CRUD operations)

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace UserManagementAPI
{
    // ❌ PROBLEM: No versioning in API
    [ApiController]
    [Route("api/users")] // ❌ No version number
    public class UserController : ControllerBase
    {
        // ❌ PROBLEM: Static list (not using database or service layer)
        private static List<User> _users = new List<User>();

        // ❌ PROBLEM 1: Using GET for operations that modify data
        // ❌ PROBLEM 2: No input validation
        // ❌ PROBLEM 3: Returns User directly (exposing password!)
        [HttpGet("create")]
        public User CreateUser(string name, string email, string password)
        {
            // ❌ PROBLEM: No null checks
            var user = new User
            {
                Id = _users.Count + 1, // ❌ Not thread-safe
                Name = name,
                Email = email,
                Password = password, // ❌ Storing plaintext password!
                CreatedAt = DateTime.Now // ❌ Should use UTC
            };

            _users.Add(user);

            // ❌ PROBLEM: Returns user with password
            return user;
        }

        // ❌ PROBLEM: No pagination (returns ALL users)
        [HttpGet]
        public List<User> GetAllUsers()
        {
            // ❌ PROBLEM: Returns password to client!
            return _users;
        }

        // ❌ PROBLEM: No 404 handling
        [HttpGet("{id}")]
        public User GetUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            // ❌ PROBLEM: Returns null instead of 404
            // ❌ PROBLEM: Returns password to client
            return user;
        }

        // ❌ PROBLEM: Using GET for delete operation
        [HttpGet("delete/{id}")]
        public string DeleteUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                // ❌ PROBLEM: Returning string instead of proper HTTP status
                return "User not found";
            }

            _users.Remove(user);

            // ❌ PROBLEM: Returning string instead of proper HTTP status
            return "User deleted successfully";
        }

        // ❌ PROBLEM: Accepts entire User object (including Id, which shouldn't be changed)
        [HttpPost("update")]
        public User UpdateUser(User user)
        {
            // ❌ PROBLEM: No validation that user exists
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);

            if (existingUser == null)
            {
                // ❌ PROBLEM: Creating user if doesn't exist (should be 404)
                _users.Add(user);
                return user;
            }

            // ❌ PROBLEM: Replacing entire object (loses data not in request)
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password; // ❌ Updating password without hashing!

            return existingUser; // ❌ Returns password
        }

        // ❌ PROBLEM: Custom authentication (not using ASP.NET Identity)
        [HttpPost("login")]
        public string Login(string email, string password)
        {
            var user = _users.FirstOrDefault(u => u.Email == email);

            // ❌ PROBLEM: Different error messages (security vulnerability)
            if (user == null)
            {
                return "Email not found"; // ❌ Information leak!
            }

            // ❌ PROBLEM: Plaintext password comparison
            if (user.Password != password)
            {
                return "Incorrect password"; // ❌ Information leak!
            }

            // ❌ PROBLEM: Returning user info with password
            // ❌ PROBLEM: No JWT or session token
            return $"Login successful. User: {user.Name}";
        }

        // ❌ PROBLEM: Inconsistent naming (search vs get)
        [HttpGet("search")]
        public List<User> SearchUsers(string query)
        {
            // ❌ PROBLEM: Case-sensitive search
            // ❌ PROBLEM: No pagination
            // ❌ PROBLEM: Returns passwords
            return _users.Where(u => u.Name.Contains(query) || u.Email.Contains(query)).ToList();
        }

        // ❌ PROBLEM: Exposing internal details
        [HttpGet("count")]
        public int GetUserCount()
        {
            return _users.Count; // ❌ Could be used to enumerate all users
        }

        // ❌ PROBLEM: Batch operation without proper validation
        [HttpPost("deleteMultiple")]
        public string DeleteMultipleUsers(int[] ids)
        {
            // ❌ PROBLEM: No validation of input
            // ❌ PROBLEM: No authorization check
            // ❌ PROBLEM: No transaction (partial failures possible)

            foreach (var id in ids)
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    _users.Remove(user);
                }
            }

            return $"Deleted {ids.Length} users"; // ❌ Misleading (some might have failed)
        }

        // ❌ PROBLEM: No rate limiting
        // ❌ PROBLEM: Accepting file upload without validation
        [HttpPost("uploadProfilePicture")]
        public string UploadProfilePicture(int userId, string base64Image)
        {
            // ❌ PROBLEM: No file size validation
            // ❌ PROBLEM: No file type validation
            // ❌ PROBLEM: No virus scanning
            // ❌ PROBLEM: Base64 in request body (inefficient for large files)

            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return "User not found";
            }

            user.ProfilePicture = base64Image; // ❌ Storing in memory (will crash with many users)

            return "Profile picture uploaded";
        }
    }

    // ❌ PROBLEM: Domain model exposed directly to API
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // ❌ Should NEVER be in API response
        public DateTime CreatedAt { get; set; }
        public string ProfilePicture { get; set; } // ❌ Storing base64 in memory

        // ❌ PROBLEM: No validation attributes
        // ❌ PROBLEM: No required fields marked
    }
}

/*
 * PROBLEMS SUMMARY (API Design Anti-Patterns):
 *
 * REST API VIOLATIONS:
 * 1. ❌ Using GET for operations that modify data (create, delete)
 * 2. ❌ Inconsistent HTTP verbs (GET for delete, POST for update)
 * 3. ❌ No proper HTTP status codes (200 for everything)
 * 4. ❌ Returning strings instead of proper responses
 * 5. ❌ No API versioning (breaking changes will break clients)
 *
 * SECURITY ISSUES:
 * 6. ❌ Plaintext passwords stored and returned
 * 7. ❌ No authentication/authorization
 * 8. ❌ Information leakage ("Email not found" vs "Incorrect password")
 * 9. ❌ No rate limiting (DoS vulnerability)
 * 10. ❌ No input validation (SQL injection if using database)
 * 11. ❌ File upload without validation (virus, size, type)
 *
 * DESIGN ISSUES:
 * 12. ❌ Domain model exposed directly (User includes Password)
 * 13. ❌ No DTOs (Data Transfer Objects)
 * 14. ❌ No service layer (controller doing business logic)
 * 15. ❌ Static data (not scalable, not thread-safe)
 * 16. ❌ No pagination (performance issue with large datasets)
 * 17. ❌ No filtering/sorting options
 * 18. ❌ Inconsistent naming (search vs get)
 *
 * PERFORMANCE ISSUES:
 * 19. ❌ Returning all users without pagination
 * 20. ❌ Storing base64 images in memory
 * 21. ❌ No caching
 * 22. ❌ Linear search (O(n)) instead of indexed lookup
 *
 * ERROR HANDLING:
 * 23. ❌ No 404 responses (returns null)
 * 24. ❌ No proper error responses
 * 25. ❌ No validation error details
 * 26. ❌ No exception handling
 *
 * DOCUMENTATION:
 * 27. ❌ No XML comments for Swagger
 * 28. ❌ No examples in documentation
 * 29. ❌ No API description
 *
 * TESTABILITY:
 * 30. ❌ Tightly coupled (static data, no DI)
 * 31. ❌ Hard to unit test
 * 32. ❌ No interfaces
 *
 * MONITORING:
 * 33. ❌ No logging
 * 34. ❌ No metrics (request count, latency)
 * 35. ❌ No health checks
 */
