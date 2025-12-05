using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Development: Use User Secrets (local machine)
if (builder.Environment.IsDevelopment())
{
    // User Secrets are automatically loaded in Development
    // Run: dotnet user-secrets set "ApiKey" "dev-api-key-123"
}

// Production: Use Azure Key Vault
if (builder.Environment.IsProduction())
{
    var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
    if (!string.IsNullOrEmpty(keyVaultUrl))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUrl),
            new DefaultAzureCredential());  // Uses Managed Identity in Azure
    }
}

var app = builder.Build();

app.MapGet("/", () => new
{
    environment = app.Environment.EnvironmentName,
    secretSources = app.Environment.IsDevelopment()
        ? new[] { "User Secrets (local machine)", "appsettings.json" }
        : new[] { "Azure Key Vault", "Managed Identity", "Environment Variables" },
    usage = new[]
    {
        "GET /secrets - View all secrets (masked)",
        "GET /secrets/{name} - Get specific secret (masked)"
    }
});

app.MapGet("/secrets", (IConfiguration config) =>
{
    // Never expose actual secrets - only masked values
    return new
    {
        secrets = new
        {
            apiKey = MaskSecret(config["ApiKey"]),
            databasePassword = MaskSecret(config["DatabasePassword"]),
            connectionString = MaskSecret(config["ConnectionStrings:DefaultConnection"]),
            externalApiKey = MaskSecret(config["ExternalApiKey"])
        },
        warning = "Secrets are masked for security. Never log or expose real secrets!"
    };
});

app.MapGet("/secrets/{name}", (string name, IConfiguration config) =>
{
    var value = config[name];
    if (value == null)
    {
        return Results.NotFound(new { error = $"Secret '{name}' not found" });
    }

    return Results.Ok(new
    {
        name,
        value = MaskSecret(value),
        warning = "Secret is masked for security"
    });
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

static string MaskSecret(string? secret)
{
    if (string.IsNullOrEmpty(secret))
        return "***";

    if (secret.Length <= 4)
        return "***";

    return secret[..4] + new string('*', Math.Min(secret.Length - 4, 20));
}
