var builder = WebApplication.CreateBuilder(args);

// Configuration is loaded in order (later sources override earlier):
// 1. appsettings.json
// 2. appsettings.{Environment}.json
// 3. Environment variables
// 4. Command-line arguments

// Bind configuration to strongly-typed options
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var app = builder.Build();

app.MapGet("/", (IConfiguration config) => new
{
    environment = app.Environment.EnvironmentName,
    configuration = new
    {
        appName = config["AppSettings:AppName"],
        apiUrl = config["AppSettings:ApiUrl"],
        enableFeatureX = config.GetValue<bool>("AppSettings:EnableFeatureX"),
        maxConnections = config.GetValue<int>("AppSettings:MaxConnections"),
        connectionString = MaskSensitiveData(config.GetConnectionString("DefaultConnection")),
    },
    sources = new[]
    {
        "1. appsettings.json (base)",
        $"2. appsettings.{app.Environment.EnvironmentName}.json (environment-specific)",
        "3. Environment variables (ASPNETCORE_ prefix)",
        "4. Command-line arguments (--key=value)"
    }
});

app.MapGet("/config/{key}", (string key, IConfiguration config) =>
{
    var value = config[key];
    return value != null
        ? Results.Ok(new { key, value })
        : Results.NotFound(new { error = $"Configuration key '{key}' not found" });
});

app.Run();

static string? MaskSensitiveData(string? value)
{
    if (string.IsNullOrEmpty(value)) return value;
    return value.Length > 10 ? value[..10] + "***" : "***";
}

public class AppSettings
{
    public string AppName { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public bool EnableFeatureX { get; set; }
    public int MaxConnections { get; set; }
}
