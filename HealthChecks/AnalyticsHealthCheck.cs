using EasyClaimsCore.API.Data;
using EasyClaimsCore.API.Services.Analytics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EasyClaimsCore.API.HealthChecks
{
    public class AnalyticsHealthCheck : IHealthCheck
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AnalyticsHealthCheck> _logger;

        public AnalyticsHealthCheck(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILogger<AnalyticsHealthCheck> logger)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, object>();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var analyticsService = scope.ServiceProvider.GetRequiredService<IAnalyticsService>();

                // Check database connectivity
                await CheckDatabaseHealth(dbContext, data, cancellationToken);

                // Check analytics service performance
                await CheckAnalyticsServiceHealth(analyticsService, data, cancellationToken);

                // Check system metrics
                await CheckSystemMetrics(dbContext, data, cancellationToken);

                stopwatch.Stop();
                data["responseTimeMs"] = stopwatch.ElapsedMilliseconds;

                // Determine overall health status
                var isHealthy = DetermineHealthStatus(data);

                return isHealthy
                    ? HealthCheckResult.Healthy("Analytics system is healthy", data)
                    : HealthCheckResult.Degraded("Analytics system is degraded", data: data);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                data["responseTimeMs"] = stopwatch.ElapsedMilliseconds;
                data["error"] = ex.Message;

                _logger.LogError(ex, "Analytics health check failed");
                return HealthCheckResult.Unhealthy("Analytics system is unhealthy", ex, data);
            }
        }

        private async Task CheckDatabaseHealth(ApplicationDbContext dbContext, Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            var dbStopwatch = Stopwatch.StartNew();

            try
            {
                // Test database connectivity
                var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
                data["database.canConnect"] = canConnect;

                if (canConnect)
                {
                    // Check recent data availability
                    var recentLogsCount = await dbContext.APIRequestLogs
                        .Where(l => l.Requested >= DateTime.UtcNow.AddHours(-1))
                        .CountAsync(cancellationToken);

                    data["database.recentLogsCount"] = recentLogsCount;

                    // Check total records
                    var totalLogsCount = await dbContext.APIRequestLogs.CountAsync(cancellationToken);
                    data["database.totalLogsCount"] = totalLogsCount;
                }

                dbStopwatch.Stop();
                data["database.responseTimeMs"] = dbStopwatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                dbStopwatch.Stop();
                data["database.responseTimeMs"] = dbStopwatch.ElapsedMilliseconds;
                data["database.error"] = ex.Message;
                data["database.canConnect"] = false;
            }
        }

        private async Task CheckAnalyticsServiceHealth(IAnalyticsService analyticsService, Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            var serviceStopwatch = Stopwatch.StartNew();

            try
            {
                // Test analytics service responsiveness
                var overview = await analyticsService.GetOverviewAsync(
                    DateTime.UtcNow.AddDays(-1),
                    DateTime.UtcNow);

                data["analytics.overviewAvailable"] = overview != null;
                data["analytics.totalCalls"] = overview?.TotalCalls ?? 0;
                data["analytics.successRate"] = overview?.SuccessRate ?? 0;

                serviceStopwatch.Stop();
                data["analytics.responseTimeMs"] = serviceStopwatch.ElapsedMilliseconds;

                // Check if response time is within acceptable limits
                var maxResponseTime = _configuration.GetValue<int>("HealthChecks:Analytics:UnhealthyResponseTime", 5000);
                data["analytics.performanceHealthy"] = serviceStopwatch.ElapsedMilliseconds < maxResponseTime;
            }
            catch (Exception ex)
            {
                serviceStopwatch.Stop();
                data["analytics.responseTimeMs"] = serviceStopwatch.ElapsedMilliseconds;
                data["analytics.error"] = ex.Message;
                data["analytics.overviewAvailable"] = false;
                data["analytics.performanceHealthy"] = false;
            }
        }

        private async Task CheckSystemMetrics(ApplicationDbContext dbContext, Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            try
            {
                var lastHour = DateTime.UtcNow.AddHours(-1);
                var recentLogs = await dbContext.APIRequestLogs
                    .Where(l => l.Requested >= lastHour)
                    .ToListAsync(cancellationToken);

                if (recentLogs.Any())
                {
                    var errorRate = (double)recentLogs.Count(l => l.Status != "Success") / recentLogs.Count * 100;
                    data["metrics.errorRate"] = Math.Round(errorRate, 2);

                    var responseTimes = recentLogs
                        .Where(l => l.Responded.HasValue)
                        .Select(l => (l.Responded!.Value - l.Requested).TotalMilliseconds)
                        .ToList();

                    if (responseTimes.Any())
                    {
                        data["metrics.averageResponseTime"] = Math.Round(responseTimes.Average(), 2);
                        data["metrics.maxResponseTime"] = Math.Round(responseTimes.Max(), 2);
                    }

                    // Check against thresholds
                    var errorThreshold = _configuration.GetValue<double>("Analytics:AlertThresholds:HighErrorRate", 10.0);
                    var responseThreshold = _configuration.GetValue<double>("Analytics:AlertThresholds:HighResponseTime", 2000);

                    data["metrics.errorRateHealthy"] = errorRate < errorThreshold;
                    data["metrics.responseTimeHealthy"] = responseTimes.Any() ? responseTimes.Average() < responseThreshold : true;
                }
                else
                {
                    data["metrics.noRecentActivity"] = true;
                    data["metrics.errorRateHealthy"] = true;
                    data["metrics.responseTimeHealthy"] = true;
                }
            }
            catch (Exception ex)
            {
                data["metrics.error"] = ex.Message;
                data["metrics.errorRateHealthy"] = false;
                data["metrics.responseTimeHealthy"] = false;
            }
        }

        private bool DetermineHealthStatus(Dictionary<string, object> data)
        {
            // Check critical components
            var databaseHealthy = data.TryGetValue("database.canConnect", out var dbConnect) && (bool)dbConnect;
            var analyticsHealthy = data.TryGetValue("analytics.overviewAvailable", out var analyticsAvailable) && (bool)analyticsAvailable;
            var performanceHealthy = data.TryGetValue("analytics.performanceHealthy", out var perfHealthy) && (bool)perfHealthy;

            // System is healthy if all critical components are working
            return databaseHealthy && analyticsHealthy && performanceHealthy;
        }
    }

    public class DatabaseConnectionHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DatabaseConnectionHealthCheck> _logger;

        public DatabaseConnectionHealthCheck(ApplicationDbContext dbContext, ILogger<DatabaseConnectionHealthCheck> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
                stopwatch.Stop();

                var data = new Dictionary<string, object>
                {
                    { "responseTimeMs", stopwatch.ElapsedMilliseconds },
                    { "canConnect", canConnect }
                };

                return canConnect
                    ? HealthCheckResult.Healthy($"Database connection successful in {stopwatch.ElapsedMilliseconds}ms", data)
                    : HealthCheckResult.Unhealthy("Cannot connect to database", data: data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return HealthCheckResult.Unhealthy("Database health check failed", ex);
            }
        }
    }

    // Extension methods for registering health checks
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddAnalyticsHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<AnalyticsHealthCheck>("analytics", HealthStatus.Degraded, new[] { "analytics", "ready" })
                .AddCheck<DatabaseConnectionHealthCheck>("database", HealthStatus.Unhealthy, new[] { "database", "ready" });

            return services;
        }
    }
}