using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAdvanced.Models;

namespace WebApiAdvanced.Controllers;

/// <summary>
/// Authentication controller - JWT token generation
/// Demonstrates JWT authentication pattern
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint - generates JWT token
    /// Demo users: admin/admin123, user/user123
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);

        // Validate credentials (in real app, check against database)
        if (!ValidateCredentials(request.Username, request.Password))
        {
            _logger.LogWarning("Invalid login attempt for user: {Username}", request.Username);
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResult("Invalid username or password"));
        }

        // Generate JWT token
        var token = GenerateJwtToken(request.Username);
        var expiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiryMinutes"));

        var response = new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Username = request.Username
        };

        _logger.LogInformation("User {Username} logged in successfully", request.Username);

        return Ok(ApiResponse<LoginResponse>.SuccessResult(response, "Login successful"));
    }

    private bool ValidateCredentials(string username, string password)
    {
        // Demo validation - in real app, check against database with hashed passwords
        var validUsers = new Dictionary<string, string>
        {
            { "admin", "admin123" },
            { "user", "user123" }
        };

        return validUsers.TryGetValue(username, out var validPassword) && validPassword == password;
    }

    private string GenerateJwtToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, username == "admin" ? "Admin" : "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiryMinutes")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
