# Configuration Management

> Environment-based configuration for Development, Staging, and Production.

## Features
- **Environment-Specific Config** - Different settings per environment
- **Configuration Hierarchy** - Base → Environment → Env Vars → CLI Args
- **Strongly-Typed Options** - Type-safe configuration binding
- **Secrets Management** - Keep sensitive data out of source control

## Run with Different Environments

### Development (default)
```bash
cd samples/07-CloudNative/ConfigManagement
dotnet run

# Or explicitly:
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

### Staging
```bash
ASPNETCORE_ENVIRONMENT=Staging dotnet run
```

### Production
```bash
ASPNETCORE_ENVIRONMENT=Production dotnet run
```

## Test Configuration
```bash
# View current configuration
curl http://localhost:5000/

# Get specific config value
curl http://localhost:5000/config/AppSettings:ApiUrl
```

## Configuration Hierarchy

Configuration is loaded in order (later sources override earlier):

1. **appsettings.json** - Base settings (committed to git)
2. **appsettings.{Environment}.json** - Environment overrides
3. **Environment Variables** - Runtime overrides (secrets, cloud config)
4. **Command-line Arguments** - Developer overrides

### Example Override Flow
```json
// appsettings.json
{
  "AppSettings": {
    "MaxConnections": 10  ← Base value
  }
}

// appsettings.Production.json
{
  "AppSettings": {
    "MaxConnections": 100  ← Overrides for Production
  }
}
```

```bash
# Environment variable override (highest priority)
export AppSettings__MaxConnections=200
dotnet run  # Uses 200 connections
```

## Environment Variables

### ASP.NET Core Conventions
```bash
# Double underscore (__) represents nested keys
export AppSettings__ApiUrl="https://custom-api.com"
export ConnectionStrings__DefaultConnection="Server=..."

# ASPNETCORE_ prefix for framework settings
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://+:8080"
```

### Docker
```bash
docker run -e ASPNETCORE_ENVIRONMENT=Production \
           -e AppSettings__MaxConnections=200 \
           my-app
```

### Kubernetes
```yaml
env:
- name: ASPNETCORE_ENVIRONMENT
  value: "Production"
- name: AppSettings__MaxConnections
  value: "200"
- name: ConnectionStrings__DefaultConnection
  valueFrom:
    secretKeyRef:
      name: app-secrets
      key: db-connection
```

## Strongly-Typed Options Pattern

### 1. Define Options Class
```csharp
public class AppSettings
{
    public string AppName { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public bool EnableFeatureX { get; set; }
    public int MaxConnections { get; set; }
}
```

### 2. Register Options
```csharp
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings")
);
```

### 3. Inject Options
```csharp
app.MapGet("/", (IOptions<AppSettings> options) =>
{
    var settings = options.Value;
    return settings.AppName;
});
```

## Best Practices

### ✅ DO
- Store environment-specific settings in `appsettings.{Environment}.json`
- Use environment variables for secrets in production
- Commit `appsettings.json` to git
- Use strongly-typed options for configuration
- Validate configuration at startup

### ❌ DON'T
- Commit secrets to git (passwords, API keys)
- Use hardcoded values in code
- Store production config in development files
- Access `IConfiguration` directly in business logic

## Secrets Management

### Development
```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DevDb;Trusted_Connection=true"
  }
}
```

### Production (Environment Variables)
```bash
# Never commit this!
export ConnectionStrings__DefaultConnection="Server=prod;Database=ProdDb;User=prod_user;Password=SecretP@ssw0rd"
```

### Azure App Service
```bash
# Configure in Azure Portal → Configuration → Application Settings
az webapp config appsettings set \
  --name myapp \
  --settings ConnectionStrings__DefaultConnection="..."
```

### Kubernetes Secrets
```bash
# Create secret
kubectl create secret generic app-secrets \
  --from-literal=db-connection="Server=..."

# Reference in deployment
env:
- name: ConnectionStrings__DefaultConnection
  valueFrom:
    secretKeyRef:
      name: app-secrets
      key: db-connection
```

## Configuration Sources by Environment

| Environment | Source | Example |
|-------------|--------|---------|
| Development | `appsettings.Development.json` | Local database |
| Staging | Environment Variables | Staging database |
| Production | Azure Key Vault / K8s Secrets | Production database |

## Validation at Startup

```csharp
builder.Services.AddOptions<AppSettings>()
    .Bind(builder.Configuration.GetSection("AppSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

public class AppSettings
{
    [Required]
    public string AppName { get; set; } = string.Empty;

    [Url]
    public string ApiUrl { get; set; } = string.Empty;

    [Range(1, 1000)]
    public int MaxConnections { get; set; }
}
```

**Use Cases:** Multi-environment deployments, cloud-native apps, containerized apps, CI/CD pipelines.
