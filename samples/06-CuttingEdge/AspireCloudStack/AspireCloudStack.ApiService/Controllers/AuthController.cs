using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspireCloudStack.ApiService.Controllers;

/// <summary>
/// Authentication controller for JWT token generation
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Login and get JWT token
    /// </summary>
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);

        // Simple validation (in production, check against database)
        if (request.Username == "admin" && request.Password == "admin123")
        {
            var token = GenerateJwtToken(request.Username, "Admin");
            _logger.LogInformation("Login successful for user: {Username}", request.Username);

            return Ok(new LoginResponse
            {
                Token = token,
                Username = request.Username,
                ExpiresIn = 3600
            });
        }

        if (request.Username == "user" && request.Password == "user123")
        {
            var token = GenerateJwtToken(request.Username, "User");
            _logger.LogInformation("Login successful for user: {Username}", request.Username);

            return Ok(new LoginResponse
            {
                Token = token,
                Username = request.Username,
                ExpiresIn = 3600
            });
        }

        _logger.LogWarning("Login failed for user: {Username}", request.Username);
        return Unauthorized(new { Message = "Invalid credentials" });
    }

    private string GenerateJwtToken(string username, string role)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest(string Username, string Password);
public record LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public int ExpiresIn { get; init; }
}
