using EasyClaimsCore.API.Data;
using EasyClaimsCore.API.Services.Analytics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EasyClaimsCore.API.Services.Background
{
    public class AnalyticsBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AnalyticsBackgroundService> _logger;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public AnalyticsBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<AnalyticsBackgroundService> logger,
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _cache = cache;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Analytics background service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessAnalyticsData(stoppingToken);
                    await CheckSystemHealth(stoppingToken);
                    await CleanupOldData(stoppingToken);

                    var intervalSeconds = _configuration.GetValue<int>("Analytics:RefreshIntervalSeconds", 30);
                    await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in analytics background service");
                    await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken); // Wait longer on error
                }
            }

            _logger.LogInformation("Analytics background service stopped");
        }

        private async Task ProcessAnalyticsData(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var analyticsService = scope.ServiceProvider.GetRequiredService<IAnalyticsService>();

            try
            {
                // Pre-compute frequently accessed analytics data
                var overview = await analyticsService.GetOverviewAsync();
                var cacheKey = "analytics:overview:current";
                _cache.Set(cacheKey, overview, TimeSpan.FromMinutes(5));

                // Cache recent activity
                var recentActivity = await analyticsService.GetRecentActivityAsync(50);
                _cache.Set("analytics:activity:recent", recentActivity, TimeSpan.FromMinutes(2));

                // Cache hospital analytics
                var hospitalAnalytics = await analyticsService.GetHospitalAnalyticsAsync();
                _cache.Set("analytics:hospitals:current", hospitalAnalytics, TimeSpan.FromMinutes(10));

                _logger.LogDebug("Analytics data cached successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process analytics data");
            }
        }

        private async Task CheckSystemHealth(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var alertService = scope.ServiceProvider.GetService<IAlertService>();

            try
            {
                var now = DateTime.UtcNow;
                var lastHour = now.AddHours(-1);

                // Check error rates
                var recentLogs = await dbContext.APIRequestLogs
                    .Where(l => l.Requested >= lastHour)
                    .ToListAsync(cancellationToken);

                if (recentLogs.Any())
                {
                    var errorRate = (double)recentLogs.Count(l => l.Status != "Success") / recentLogs.Count * 100;
                    var criticalThreshold = _configuration.GetValue<double>("Analytics:AlertThresholds:HighErrorRate", 10.0);

                    if (errorRate >= criticalThreshold && alertService != null)
                    {
                        await alertService.SendAlertAsync($"High error rate detected: {errorRate:F1}%", AlertLevel.Critical);
                    }

                    // Check response times
                    var responseTimes = recentLogs
                        .Where(l => l.Responded.HasValue)
                        .Select(l => (l.Responded!.Value - l.Requested).TotalMilliseconds)
                        .ToList();

                    if (responseTimes.Any())
                    {
                        var avgResponseTime = responseTimes.Average();
                        var responseThreshold = _configuration.GetValue<double>("Analytics:AlertThresholds:HighResponseTime", 2000);

                        if (avgResponseTime >= responseThreshold && alertService != null)
                        {
                            await alertService.SendAlertAsync($"High response time detected: {avgResponseTime:F0}ms", AlertLevel.Warning);
                        }
                    }
                }

                _logger.LogDebug("System health check completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check system health");
            }
        }

        private async Task CleanupOldData(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                var retentionDays = _configuration.GetValue<int>("Analytics:DataRetentionDays", 90);
                var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

                // Clean up old request logs (keep only summary data)
                var oldLogs = await dbContext.APIRequestLogs
                    .Where(l => l.Requested < cutoffDate)
                    .CountAsync(cancellationToken);

                if (oldLogs > 0)
                {
                    // Instead of deleting, consider archiving to a separate table
                    _logger.LogInformation("Found {OldLogCount} old logs that could be archived", oldLogs);

                    // Implementation would depend on your archival strategy
                    // For now, we'll just log the count
                }

                _logger.LogDebug("Data cleanup check completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup old data");
            }
        }
    }

    public interface IAlertService
    {
        Task SendAlertAsync(string message, AlertLevel level);
    }

    public enum AlertLevel
    {
        Info,
        Warning,
        Critical
    }

    public class AlertService : IAlertService
    {
        private readonly ILogger<AlertService> _logger;
        private readonly IConfiguration _configuration;

        public AlertService(ILogger<AlertService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendAlertAsync(string message, AlertLevel level)
        {
            var alertingEnabled = _configuration.GetValue<bool>("Analytics:AlertingEnabled", false);
            if (!alertingEnabled) return;

            _logger.LogWarning("ALERT [{Level}]: {Message}", level, message);

            // Implement your alerting mechanism here:
            // - Send email notifications
            // - Send Slack/Teams messages  
            // - Send SMS alerts
            // - Post to monitoring systems (e.g., PagerDuty, Datadog)
            // - Write to event log

            try
            {
                // Example: Email notification (implement based on your email service)
                // await _emailService.SendAlertEmailAsync(message, level);

                // Example: Slack notification (implement based on your Slack integration)
                // await _slackService.SendAlertAsync(message, level);

                await Task.CompletedTask; // Placeholder
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send alert: {Message}", message);
            }
        }
    }
}