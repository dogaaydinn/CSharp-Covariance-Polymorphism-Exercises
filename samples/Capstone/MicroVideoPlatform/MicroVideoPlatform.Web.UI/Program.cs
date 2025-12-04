using Blazored.LocalStorage;
using MicroVideoPlatform.Web.UI.Hubs;
using MicroVideoPlatform.Web.UI.Services;
using MicroVideoPlatform.Web.UI.State;
using MudBlazor.Services;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// MudBlazor Services
builder.Services.AddMudServices();

// Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// HTTP Client for API calls
builder.Services.AddHttpClient<IVideoApiClient, VideoApiClient>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IAnalyticsApiClient, AnalyticsApiClient>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiSettings:AnalyticsUrl"] ?? "http://localhost:7071";
    client.BaseAddress = new Uri(apiBaseUrl);
});

// State Management
builder.Services.AddScoped<AppState>();
builder.Services.AddScoped<VideoStore>();

// SignalR Client Service (Singleton for persistent connection)
builder.Services.AddSingleton<VideoHubClient>();

// Logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog(dispose: true);
});

// CORS (for SignalR)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();

// Map SignalR hub
app.MapHub<VideoHub>("/videohub");

// Map Blazor
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

Log.Information("Web.UI starting on {Urls}", app.Urls.FirstOrDefault() ?? "http://localhost:5000");

try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Web.UI terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
