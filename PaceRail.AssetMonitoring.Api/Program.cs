using Microsoft.EntityFrameworkCore;
using PaceRail.AssetMonitoring.Api.Data;
using PaceRail.AssetMonitoring.Api.Data.Interceptors;
using PaceRail.AssetMonitoring.Api.Middleware;
using PaceRail.AssetMonitoring.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("DefaultConnection string is missing.");

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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(context);
}

app.Run();