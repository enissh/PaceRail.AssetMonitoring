using Microsoft.EntityFrameworkCore;
using Npgsql;
using PaceRail.AssetMonitoring.Api.Data;
using PaceRail.AssetMonitoring.Api.Data.Interceptors;
using PaceRail.AssetMonitoring.Api.Middleware;
using PaceRail.AssetMonitoring.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Parse Render's database URL or fallback to configuration
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Database connection string is missing.");

string connectionString;

if (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://"))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    
    connectionString = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Username = userInfo[0],
        Password = userInfo.Length > 1 ? userInfo[1] : string.Empty,
        Database = uri.AbsolutePath.TrimStart('/'),
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    }.ToString();
}
else
{
    connectionString = databaseUrl;
}

// Register EF Core Interceptor
builder.Services.AddSingleton<AuditDbContextInterceptor>();

// Database Context with Interceptor
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<AuditDbContextInterceptor>();
    options.UseNpgsql(connectionString)
           .AddInterceptors(interceptor);
});

// Domain & Dispatcher Services
builder.Services.AddScoped<WorkOrderDispatcher>();
builder.Services.AddScoped<RailChainageEvaluator>();

// Background Hosted Service for Telemetry Simulation
builder.Services.AddHostedService<TelemetrySimulatorService>();

// Database Health Check
builder.Services.AddHealthChecks()
    .AddAsyncCheck("PostgreSQL", async () =>
    {
        using var db = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options);

        var canConnect = await db.Database.CanConnectAsync();
        return canConnect 
            ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Database is responsive.")
            : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Cannot connect to PostgreSQL.");
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Fallback route for Single Page Applications serving static files from wwwroot
app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(context);
}

app.Run();