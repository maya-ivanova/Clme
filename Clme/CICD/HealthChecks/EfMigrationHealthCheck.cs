using Clme.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace Clme.ProjectSystems.HealthChecks
    {

    public class EfMigrationHealthCheck : IHealthCheck
        {
        private readonly ApplicationDbContext _context;

        public EfMigrationHealthCheck(ApplicationDbContext context)
            {
            _context = context;
            }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
            {
            // Get pending migrations
            var pending = await _context.Database.GetPendingMigrationsAsync(cancellationToken);

            if (pending.Any())
                {
                return HealthCheckResult.Unhealthy(
                    $"Pending migrations: {string.Join(", ", pending)}");
                }

            return HealthCheckResult.Healthy("Database schema is up to date.");
            }
        }
    }


