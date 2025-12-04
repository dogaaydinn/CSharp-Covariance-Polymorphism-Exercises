// PR #189: Add User Search and Admin Features
// Author: Junior Developer (12 months experience)
// Date: 2024-12-03
// Description: User search functionality and admin management features

using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel
{
    public class AdminController : ControllerBase
    {
        private readonly string _connectionString = "Server=localhost;Database=Users;User Id=sa;Password=Admin123;";

        // ❌ PROBLEM 1: SQL Injection vulnerability
        [HttpGet("search")]
        public IActionResult SearchUsers(string query)
        {
            // ❌ String concatenation with user input!
            var sql = $"SELECT * FROM Users WHERE Name LIKE '%{query}%' OR Email LIKE '%{query}%'";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            connection.Open();
            using var reader = command.ExecuteReader();

            var users = new List<object>();
            while (reader.Read())
            {
                users.Add(new
                {
                    Id = reader["Id"],
                    Name = reader["Name"],
                    Email = reader["Email"],
                    Password = reader["Password"] // ❌ Exposing password!
                });
            }

            return Ok(users);
        }

        // ❌ PROBLEM 2: Authentication bypass
        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            // ❌ SQL Injection + No password hashing
            var sql = $"SELECT * FROM Users WHERE Username = '{username}' AND Password = '{password}'";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                // ❌ No session management, just return user data
                return Ok(new
                {
                    Id = reader["Id"],
                    Username = reader["Username"],
                    IsAdmin = reader["IsAdmin"],
                    // ❌ Sending password back!
                    Password = reader["Password"]
                });
            }

            return Unauthorized();
        }

        // ❌ PROBLEM 3: Insecure direct object reference (IDOR)
        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            // ❌ No authorization check - anyone can view any user!
            var sql = $"SELECT * FROM Users WHERE Id = {id}";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return Ok(new
                {
                    Id = reader["Id"],
                    Name = reader["Name"],
                    Email = reader["Email"],
                    SSN = reader["SSN"], // ❌ Exposing sensitive data!
                    CreditCard = reader["CreditCard"] // ❌ PCI violation!
                });
            }

            return NotFound();
        }

        // ❌ PROBLEM 4: XSS (Cross-Site Scripting) vulnerability
        [HttpGet("profile")]
        public IActionResult GetProfile(int userId)
        {
            var sql = $"SELECT Bio FROM Users WHERE Id = {userId}";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            connection.Open();
            var bio = command.ExecuteScalar() as string;

            // ❌ Returning HTML without sanitization
            return Content($"<div>User Bio: {bio}</div>", "text/html");
        }

        // ❌ PROBLEM 5: Mass assignment vulnerability
        [HttpPost("update-profile")]
        public IActionResult UpdateProfile([FromBody] User user)
        {
            // ❌ User can set ANY field, including IsAdmin!
            var sql = $@"UPDATE Users SET
                Name = '{user.Name}',
                Email = '{user.Email}',
                IsAdmin = {user.IsAdmin},
                Balance = {user.Balance}
                WHERE Id = {user.Id}";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            connection.Open();
            command.ExecuteNonQuery();

            return Ok();
        }

        // ❌ PROBLEM 6: Weak cryptography
        [HttpPost("reset-password")]
        public IActionResult ResetPassword(string email)
        {
            // ❌ Weak token generation
            var token = new Random().Next(100000, 999999).ToString(); // Predictable!

            // ❌ Storing in plain text
            var sql = $"UPDATE Users SET ResetToken = '{token}' WHERE Email = '{email}'";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            connection.Open();
            command.ExecuteNonQuery();

            // ❌ Sending token in URL (will be logged!)
            return Ok(new { ResetUrl = $"https://example.com/reset?token={token}" });
        }

        // ❌ PROBLEM 7: Missing CSRF protection
        [HttpPost("delete-account")]
        public IActionResult DeleteAccount(int userId)
        {
            // ❌ No CSRF token validation
            // Attacker can delete any account via CSRF attack!

            var sql = $"DELETE FROM Users WHERE Id = {userId}";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            connection.Open();
            command.ExecuteNonQuery();

            return Ok();
        }

        // ❌ PROBLEM 8: Hardcoded credentials
        [HttpGet("backup")]
        public IActionResult BackupDatabase()
        {
            // ❌ Hardcoded credentials in source code!
            var backupConnection = "Server=backup-server;Database=Users;User Id=backup_user;Password=BackupPass123!;";

            // ❌ No authorization check - anyone can backup database!
            // ... backup logic

            return Ok("Backup started");
        }

        // ❌ PROBLEM 9: Insecure deserialization
        [HttpPost("import")]
        public IActionResult ImportUsers(string serializedData)
        {
            // ❌ Deserializing untrusted data
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using var stream = new System.IO.MemoryStream(Convert.FromBase64String(serializedData));

            // ❌ Can execute arbitrary code!
            var users = formatter.Deserialize(stream);

            return Ok("Imported");
        }

        // ❌ PROBLEM 10: Path traversal
        [HttpGet("download")]
        public IActionResult DownloadFile(string filename)
        {
            // ❌ No validation - path traversal attack!
            var filePath = $"/uploads/{filename}";

            // Attacker can use: ../../etc/passwd
            if (System.IO.File.Exists(filePath))
            {
                var content = System.IO.File.ReadAllBytes(filePath);
                return File(content, "application/octet-stream");
            }

            return NotFound();
        }

        // ❌ PROBLEM 11: Information disclosure
        [HttpGet("debug")]
        public IActionResult Debug()
        {
            // ❌ Exposing internal system information
            return Ok(new
            {
                Environment.MachineName,
                Environment.UserName,
                ConnectionString = _connectionString, // ❌ Exposing DB credentials!
                DatabaseVersion = GetDatabaseVersion()
            });
        }

        // ❌ PROBLEM 12: No rate limiting
        [HttpPost("send-email")]
        public IActionResult SendEmail(string to, string message)
        {
            // ❌ No rate limiting - email bombing attack possible!
            // Attacker can send thousands of emails

            // ... send email logic

            return Ok("Email sent");
        }

        private string GetDatabaseVersion()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT @@VERSION", connection);
            connection.Open();
            return command.ExecuteScalar()?.ToString();
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; } // ❌ Should not be settable by user!
        public decimal Balance { get; set; } // ❌ Should not be settable by user!
        public string SSN { get; set; }
        public string CreditCard { get; set; }
    }
}

/*
 * SECURITY VULNERABILITIES SUMMARY:
 *
 * INJECTION ATTACKS:
 * 1. ❌ SQL Injection (SearchUsers, Login, GetUser, etc.)
 * 2. ❌ XSS (Cross-Site Scripting) - GetProfile
 * 3. ❌ Path Traversal - DownloadFile
 *
 * AUTHENTICATION/AUTHORIZATION:
 * 4. ❌ Authentication bypass (SQL Injection in login)
 * 5. ❌ No authorization checks (IDOR)
 * 6. ❌ Missing CSRF protection
 * 7. ❌ No session management
 *
 * DATA EXPOSURE:
 * 8. ❌ Sensitive data in responses (Password, SSN, CreditCard)
 * 9. ❌ Information disclosure (Debug endpoint)
 * 10. ❌ Hardcoded credentials
 *
 * CRYPTOGRAPHY:
 * 11. ❌ Weak random token generation
 * 12. ❌ No password hashing (plaintext passwords)
 * 13. ❌ Insecure reset token in URL
 *
 * MASS ASSIGNMENT:
 * 14. ❌ User can set IsAdmin, Balance fields
 *
 * DESERIALIZATION:
 * 15. ❌ Insecure deserialization (BinaryFormatter)
 *
 * RATE LIMITING:
 * 16. ❌ No rate limiting (email bombing, brute force)
 *
 * OWASP TOP 10 VIOLATIONS:
 * - A01:2021 – Broken Access Control ✓
 * - A02:2021 – Cryptographic Failures ✓
 * - A03:2021 – Injection ✓
 * - A04:2021 – Insecure Design ✓
 * - A05:2021 – Security Misconfiguration ✓
 * - A06:2021 – Vulnerable Components ✓
 * - A07:2021 – Authentication Failures ✓
 * - A08:2021 – Software/Data Integrity Failures ✓
 */
