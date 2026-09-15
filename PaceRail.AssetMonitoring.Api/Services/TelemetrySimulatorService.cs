using Microsoft.EntityFrameworkCore;
using PaceRail.AssetMonitoring.Api.Data;
using PaceRail.AssetMonitoring.Api.Models;

namespace PaceRail.AssetMonitoring.Api.Services;

public class TelemetrySimulatorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelemetrySimulatorService> _logger;
    private readonly Random _random = new();

    public TelemetrySimulatorService(IServiceProvider serviceProvider, ILogger<TelemetrySimulatorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Rail Telemetry Simulator Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var assets = await context.RailAssets.Select(a => a.Id).ToListAsync(stoppingToken);

                if (assets.Any())
                {
                    var readings = new List<TelemetryReading>();

                    foreach (var assetId in assets)
                    {
                        readings.Add(new TelemetryReading
                        {
                            RailAssetId = assetId,
                            MetricType = "TrackTemperature",
                            Value = Math.Round(15.0 + (_random.NextDouble() * 25.0), 2),
                            Unit = "°C",
                            Timestamp = DateTime.UtcNow
                        });

                        readings.Add(new TelemetryReading
                        {
                            RailAssetId = assetId,
                            MetricType = "VibrationLevel",
                            Value = Math.Round(0.1 + (_random.NextDouble() * 4.9), 2),
                            Unit = "mm/s",
                            Timestamp = DateTime.UtcNow
                        });
                    }

                    context.TelemetryReadings.AddRange(readings);
                    await context.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation("Generated {Count} telemetry readings across active assets.", readings.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while simulating telemetry data.");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
