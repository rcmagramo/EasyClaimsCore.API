using EasyClaimsCore.API.Data;
using EasyClaimsCore.API.Models.Analytics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Text.Json;

namespace EasyClaimsCore.API.Services.Analytics
{
    public interface IAnalyticsCacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class;
    }

    public class AnalyticsCacheService : IAnalyticsCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AnalyticsCacheService> _logger;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _defaultExpiration;

        public AnalyticsCacheService(
            IMemoryCache memoryCache,
            ILogger<AnalyticsCacheService> logger,
            IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _configuration = configuration;
            _defaultExpiration = TimeSpan.FromMinutes(
                _configuration.GetValue<int>("Analytics:CacheExpirationMinutes", 5));
        }

        public Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                var cacheKey = BuildCacheKey(key);
                if (_memoryCache.TryGetValue(cacheKey, out var cachedValue))
                {
                    if (cachedValue is T typedValue)
                    {
                        _logger.LogDebug("Cache hit for key: {Key}", key);
                        return Task.FromResult<T?>(typedValue);
                    }
                }

                _logger.LogDebug("Cache miss for key: {Key}", key);
                return Task.FromResult<T?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
                return Task.FromResult<T?>(null);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var cacheKey = BuildCacheKey(key);
                var expirationTime = expiration ?? _defaultExpiration;

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime,
                    SlidingExpiration = TimeSpan.FromMinutes(expirationTime.TotalMinutes / 2),
                    Priority = CacheItemPriority.Normal
                };

                _memoryCache.Set(cacheKey, value, cacheOptions);
                _logger.LogDebug("Cache set for key: {Key}, expiration: {Expiration}", key, expirationTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
            }

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                var cacheKey = BuildCacheKey(key);
                _memoryCache.Remove(cacheKey);
                _logger.LogDebug("Cache removed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
            }

            return Task.CompletedTask;
        }

        public Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                // Note: IMemoryCache doesn't natively support pattern removal
                // This is a simplified implementation - for production, consider using Redis

                if (_memoryCache is MemoryCache memoryCache)
                {
                    var field = typeof(MemoryCache).GetField("_coherentState",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (field?.GetValue(memoryCache) is object coherentState)
                    {
                        var entriesCollection = coherentState.GetType()
                            .GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                        if (entriesCollection?.GetValue(coherentState) is IDictionary entries)
                        {
                            var keysToRemove = new List<object>();

                            foreach (DictionaryEntry entry in entries)
                            {
                                if (entry.Key.ToString()?.Contains(pattern) == true)
                                {
                                    keysToRemove.Add(entry.Key);
                                }
                            }

                            foreach (var key in keysToRemove)
                            {
                                _memoryCache.Remove(key);
                            }

                            _logger.LogDebug("Removed {Count} cache entries matching pattern: {Pattern}",
                                keysToRemove.Count, pattern);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache entries by pattern: {Pattern}", pattern);
            }

            return Task.CompletedTask;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class
        {
            var cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
            {
                return cachedValue;
            }

            try
            {
                var value = await factory();
                await SetAsync(key, value, expiration);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrSetAsync factory for key: {Key}", key);
                throw;
            }
        }

        private string BuildCacheKey(string key)
        {
            return $"analytics:{key}";
        }
    }

    // Enhanced Analytics Service with Caching
    public class CachedAnalyticsService : IAnalyticsService
    {
        private readonly AnalyticsService _baseService;
        private readonly IAnalyticsCacheService _cacheService;
        private readonly ILogger<CachedAnalyticsService> _logger;
        private readonly ApplicationDbContext _dbContext;


        public CachedAnalyticsService(
            AnalyticsService baseService,
            IAnalyticsCacheService cacheService,
            ILogger<CachedAnalyticsService> logger)
        {
            _baseService = baseService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<OverviewDto> GetOverviewAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var cacheKey = $"overview:{startDate?.ToString("yyyyMMdd")}:{endDate?.ToString("yyyyMMdd")}";

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                () => _baseService.GetOverviewAsync(startDate, endDate),
                TimeSpan.FromMinutes(2)
            );
        }

        public async Task<List<PendingUploadDto>> GetPendingUploadsAsync()
        {
            try
            {
                var pendingUploads = await _dbContext.APIRequestSuccessLogs
                    .Where(log => log.IsBilled == false && log.IsActive == true)
                    .Join(_dbContext.APIRequests,
                          log => log.APIRequestId,
                          req => req.Id,
                          (log, req) => new { log, req })
                    .Where(x => x.req.MethodName == "eClaimsApiUpload")
                    .GroupBy(x => x.log.Pmcc)
                    .Select(g => new PendingUploadDto
                    {
                        Pmcc = g.Key,
                        PendingCount = g.Count(),
                        LastUpload = g.Max(x => x.log.Requested),
                        TotalBillAmount = g.Sum(x => x.log.BillAmount)
                    })
                    .OrderByDescending(p => p.PendingCount)
                    .ToListAsync();

                return pendingUploads;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending uploads data");
                return new List<PendingUploadDto>();
            }
        }

        public async Task<List<ApiUsageDto>> GetApiUsageAsync(DateTime? startDate = null, DateTime? endDate = null, string? hospitalId = null)
        {
            var cacheKey = $"apiusage:{startDate?.ToString("yyyyMMdd")}:{endDate?.ToString("yyyyMMdd")}:{hospitalId}";

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                () => _baseService.GetApiUsageAsync(startDate, endDate, hospitalId),
                TimeSpan.FromMinutes(5)
            );
        }

        public async Task<PerformanceMetricsDto> GetPerformanceMetricsAsync(DateTime? startDate = null, DateTime? endDate = null, string? methodName = null)
        {
            var cacheKey = $"performance:{startDate?.ToString("yyyyMMdd")}:{endDate?.ToString("yyyyMMdd")}:{methodName}";

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                () => _baseService.GetPerformanceMetricsAsync(startDate, endDate, methodName),
                TimeSpan.FromMinutes(5)
            );
        }

        public async Task<List<HospitalAnalyticsDto>> GetHospitalAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var cacheKey = $"hospitals:{startDate?.ToString("yyyyMMdd")}:{endDate?.ToString("yyyyMMdd")}";

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                () => _baseService.GetHospitalAnalyticsAsync(startDate, endDate),
                TimeSpan.FromMinutes(10)
            );
        }

        public async Task<ErrorAnalysisDto> GetErrorAnalysisAsync(DateTime? startDate = null, DateTime? endDate = null, string? hospitalId = null)
        {
            var cacheKey = $"errors:{startDate?.ToString("yyyyMMdd")}:{endDate?.ToString("yyyyMMdd")}:{hospitalId}";

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                () => _baseService.GetErrorAnalysisAsync(startDate, endDate, hospitalId),
                TimeSpan.FromMinutes(5)
            );
        }

        public async Task<List<RecentActivityDto>> GetRecentActivityAsync(int limit = 50, string? status = null)
        {
            // Don't cache recent activity as it changes frequently
            return await _baseService.GetRecentActivityAsync(limit, status);
        }

        public async Task<List<TrendDataDto>> GetTrendsAsync(DateTime? startDate = null, DateTime? endDate = null, string groupBy = "hour")
        {
            var cacheKey = $"trends:{startDate?.ToString("yyyyMMdd")}:{endDate?.ToString("yyyyMMdd")}:{groupBy}";

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                () => _baseService.GetTrendsAsync(startDate, endDate, groupBy),
                TimeSpan.FromMinutes(5)
            );
        }

        public async Task<byte[]> ExportDataAsync(DateTime? startDate = null, DateTime? endDate = null, string format = "csv")
        {
            // Don't cache exports as they can be large and are typically one-time requests
            return await _baseService.ExportDataAsync(startDate, endDate, format);
        }

        public async Task<bool> GenerateSampleDataAsync()
        {
            // Don't cache sample data generation - delegate directly to base service
            // Also clear all cache after generating sample data
            var result = await _baseService.GenerateSampleDataAsync();

            if (result)
            {
                // Clear all analytics cache since we now have new data
                await _cacheService.RemoveByPatternAsync("analytics:");
                _logger.LogInformation("Cleared analytics cache after sample data generation");
            }

            return result;
        }
    }
}