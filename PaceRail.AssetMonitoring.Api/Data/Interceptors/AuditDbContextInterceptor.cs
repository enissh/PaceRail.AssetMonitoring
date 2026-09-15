using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PaceRail.AssetMonitoring.Api.Models;
using System.Text.Json;

namespace PaceRail.AssetMonitoring.Api.Data.Interceptors;

public class AuditDbContextInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var auditEntries = new List<AuditLog>();
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            if (entry.Entity is AuditLog || entry.Entity is TelemetryReading) continue;

            var changes = new Dictionary<string, object?>();
            foreach (var property in entry.Properties)
            {
                if (entry.State == EntityState.Modified && property.IsModified)
                {
                    changes[property.Metadata.Name] = new
                    {
                        Original = property.OriginalValue,
                        Current = property.CurrentValue
                    };
                }
                else if (entry.State == EntityState.Added)
                {
                    changes[property.Metadata.Name] = property.CurrentValue;
                }
            }

            var auditLog = new AuditLog
            {
                EntityName = entry.Entity.GetType().Name,
                Action = entry.State.ToString(),
                EntityId = entry.Property("Id").CurrentValue?.ToString() ?? "0",
                Changes = JsonSerializer.Serialize(changes),
                Timestamp = DateTime.UtcNow
            };

            auditEntries.Add(auditLog);
        }

        if (auditEntries.Any())
        {
            context.Set<AuditLog>().AddRange(auditEntries);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}