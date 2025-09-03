using EasyClaimsCore.API.Data;
using EasyClaimsCore.API.Data.Entities;
using EasyClaimsCore.API.Models.Analytics;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace EasyClaimsCore.API.Services.Analytics
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(ApplicationDbContext dbContext, ILogger<AnalyticsService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<OverviewDto> GetOverviewAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var (start, end) = GetDateRange(startDate, endDate);

                var logs = await _dbContext.APIRequestLogs
                    .Where(l => l.Requested >= start && l.Requested <= end)
                    .ToListAsync();

                var totalCalls = logs.Count;
                var successfulCalls = logs.Count(l => l.Status == "Success");
                var failedCalls = totalCalls - successfulCalls;
                var successRate = totalCalls > 0 ? (double)successfulCalls / totalCalls * 100 : 0;

                // Get the last response time instead of average
                var lastResponseTime = 0.0;
                var lastLogWithResponseTime = logs
                    .Where(l => l.Requested != null && l.Responded != null)
                    .OrderByDescending(l => l.Requested)
                    .FirstOrDefault();

                if (lastLogWithResponseTime != null)
                {
                    lastResponseTime = (lastLogWithResponseTime.Responded!.Value - lastLogWithResponseTime.Requested).TotalMilliseconds;
                }

                var activeHospitals = await _dbContext.APIRequestLogs
                    .Join(_dbContext.APIRequests, log => log.APIRequestId, req => req.Id, (log, req) => new { log, req })
                    .Where(x => x.log.Requested >= start && x.log.Requested <= end)
                    .Select(x => x.req.HospitalId)
                    .Distinct()
                    .CountAsync();

                var totalChargeableItems = logs.Sum(l => l.ChargeableItems);

                return new OverviewDto
                {
                    TotalCalls = totalCalls,
                    SuccessfulCalls = successfulCalls,
                    FailedCalls = failedCalls,
                    SuccessRate = Math.Round(successRate, 1),
                    LastResponseTime = Math.Round(lastResponseTime, 2), // Changed from AverageResponseTime
                    ActiveHospitals = activeHospitals,
                    TotalChargeableItems = totalChargeableItems
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overview data");

                // Return empty data instead of throwing
                return new OverviewDto
                {
                    TotalCalls = 0,
                    SuccessfulCalls = 0,
                    FailedCalls = 0,
                    SuccessRate = 0,
                    LastResponseTime = 0, // Changed from AverageResponseTime
                    ActiveHospitals = 0,
                    TotalChargeableItems = 0
                };
            }
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
            var (start, end) = GetDateRange(startDate, endDate);

            var query = _dbContext.APIRequestLogs
                .Join(_dbContext.APIRequests, log => log.APIRequestId, req => req.Id, (log, req) => new { log, req })
                .Where(x => x.log.Requested >= start && x.log.Requested <= end);

            if (!string.IsNullOrEmpty(hospitalId))
            {
                query = query.Where(x => x.req.HospitalId == hospitalId);
            }

            var usage = await query
                .GroupBy(x => x.req.MethodName)
                .Select(g => new
                {
                    MethodName = g.Key,
                    CallCount = g.Count(),
                    SuccessCount = g.Count(x => x.log.Status == "Success"),
                    TotalChargeableItems = g.Sum(x => x.log.ChargeableItems),
                    LastUsed = g.Max(x => x.log.Requested),
                    ResponseTimes = g.Where(x => x.log.Responded != null)
                                   .Select(x => (x.log.Responded!.Value - x.log.Requested).TotalMilliseconds)
                })
                .ToListAsync();

            return usage.Select(u => new ApiUsageDto
            {
                MethodName = u.MethodName,
                CallCount = u.CallCount,
                SuccessCount = u.SuccessCount,
                FailureCount = u.CallCount - u.SuccessCount,
                SuccessRate = u.CallCount > 0 ? (double)u.SuccessCount / u.CallCount * 100 : 0,
                AverageResponseTime = u.ResponseTimes.Any() ? Math.Round(u.ResponseTimes.Average(), 2) : 0,
                TotalChargeableItems = u.TotalChargeableItems,
                LastUsed = u.LastUsed
            }).OrderByDescending(u => u.CallCount).ToList();
        }

        public async Task<PerformanceMetricsDto> GetPerformanceMetricsAsync(DateTime? startDate = null, DateTime? endDate = null, string? methodName = null)
        {
            var (start, end) = GetDateRange(startDate, endDate);

            var query = _dbContext.APIRequestLogs
                .Where(l => l.Requested >= start && l.Requested <= end && l.Responded != null);

            if (!string.IsNullOrEmpty(methodName))
            {
                query = query.Join(_dbContext.APIRequests, log => log.APIRequestId, req => req.Id, (log, req) => log)
                           .Where(l => _dbContext.APIRequests.Any(r => r.Id == l.APIRequestId && r.MethodName == methodName));
            }

            var logs = await query.ToListAsync();

            var responseTimes = logs
                .Select(l => (l.Responded!.Value - l.Requested).TotalMilliseconds)
                .OrderBy(rt => rt)
                .ToList();

            if (!responseTimes.Any())
            {
                return new PerformanceMetricsDto();
            }

            var averageResponseTime = responseTimes.Average();
            var medianResponseTime = responseTimes.Count % 2 == 0
                ? (responseTimes[responseTimes.Count / 2 - 1] + responseTimes[responseTimes.Count / 2]) / 2
                : responseTimes[responseTimes.Count / 2];

            // Create response time distribution
            var distribution = new List<ResponseTimeDistributionDto>
            {
                new() { Range = "0-100ms", Count = responseTimes.Count(rt => rt <= 100) },
                new() { Range = "100-500ms", Count = responseTimes.Count(rt => rt > 100 && rt <= 500) },
                new() { Range = "500ms-1s", Count = responseTimes.Count(rt => rt > 500 && rt <= 1000) },
                new() { Range = "1s-5s", Count = responseTimes.Count(rt => rt > 1000 && rt <= 5000) },
                new() { Range = "5s+", Count = responseTimes.Count(rt => rt > 5000) }
            };

            foreach (var dist in distribution)
            {
                dist.Percentage = responseTimes.Count > 0 ? (double)dist.Count / responseTimes.Count * 100 : 0;
            }

            // Create hourly trends
            var trends = logs
                .GroupBy(l => new DateTime(l.Requested.Year, l.Requested.Month, l.Requested.Day, l.Requested.Hour, 0, 0))
                .Select(g => new PerformanceTrendDto
                {
                    Timestamp = g.Key,
                    AverageResponseTime = g.Where(l => l.Responded != null)
                                         .Average(l => (l.Responded!.Value - l.Requested).TotalMilliseconds),
                    CallCount = g.Count()
                })
                .OrderBy(t => t.Timestamp)
                .ToList();

            return new PerformanceMetricsDto
            {
                AverageResponseTime = Math.Round(averageResponseTime, 2),
                MedianResponseTime = Math.Round(medianResponseTime, 2),
                MinResponseTime = Math.Round(responseTimes.Min(), 2),
                MaxResponseTime = Math.Round(responseTimes.Max(), 2),
                ResponseTimeDistribution = distribution,
                ResponseTimeTrends = trends
            };
        }

        public async Task<List<HospitalAnalyticsDto>> GetHospitalAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var (start, end) = GetDateRange(startDate, endDate);

            // First get the basic hospital stats
            var hospitalData = await _dbContext.APIRequestLogs
                .Join(_dbContext.APIRequests, log => log.APIRequestId, req => req.Id, (log, req) => new { log, req })
                .Where(x => x.log.Requested >= start && x.log.Requested <= end)
                .ToListAsync();

            var hospitalStats = hospitalData
                .GroupBy(x => x.req.HospitalId)
                .Select(g => new
                {
                    HospitalId = g.Key,
                    TotalCalls = g.Count(),
                    SuccessfulCalls = g.Count(x => x.log.Status == "Success"),
                    TotalChargeableItems = g.Sum(x => x.log.ChargeableItems),
                    LastActivity = g.Max(x => x.log.Requested),
                    ResponseTimes = g.Where(x => x.log.Responded != null)
                                   .Select(x => (x.log.Responded!.Value - x.log.Requested).TotalMilliseconds)
                                   .ToList(),
                    EndpointUsage = g.GroupBy(x => x.req.MethodName)
                                   .ToDictionary(eg => eg.Key, eg => eg.Count())
                })
                .ToList();

            return hospitalStats.Select(h => new HospitalAnalyticsDto
            {
                HospitalId = h.HospitalId,
                TotalCalls = h.TotalCalls,
                SuccessfulCalls = h.SuccessfulCalls,
                FailedCalls = h.TotalCalls - h.SuccessfulCalls,
                SuccessRate = h.TotalCalls > 0 ? (double)h.SuccessfulCalls / h.TotalCalls * 100 : 0,
                AverageResponseTime = h.ResponseTimes.Any() ? Math.Round(h.ResponseTimes.Average(), 2) : 0,
                TotalChargeableItems = h.TotalChargeableItems,
                LastActivity = h.LastActivity,
                MostUsedEndpoints = h.EndpointUsage.OrderByDescending(e => e.Value).Take(3).Select(e => e.Key).ToList(),
                EndpointUsage = h.EndpointUsage
            }).OrderByDescending(h => h.TotalCalls).ToList();
        }

        public async Task<ErrorAnalysisDto> GetErrorAnalysisAsync(DateTime? startDate = null, DateTime? endDate = null, string? hospitalId = null)
        {
            var (start, end) = GetDateRange(startDate, endDate);

            var query = _dbContext.APIRequestLogs
                .Join(_dbContext.APIRequests, log => log.APIRequestId, req => req.Id, (log, req) => new { log, req })
                .Where(x => x.log.Requested >= start && x.log.Requested <= end);

            if (!string.IsNullOrEmpty(hospitalId))
            {
                query = query.Where(x => x.req.HospitalId == hospitalId);
            }

            var allLogs = await query.ToListAsync();
            var errorLogs = allLogs.Where(x => x.log.Status != "Success").ToList();

            var totalErrors = errorLogs.Count;
            var totalCalls = allLogs.Count;
            var errorRate = totalCalls > 0 ? (double)totalErrors / totalCalls * 100 : 0;

            // Error categories (simplified classification)
            var errorCategories = errorLogs
                .GroupBy(x => ClassifyError(x.log.Response))
                .Select(g => new ErrorCategoryDto
                {
                    Category = g.Key,
                    Count = g.Count(),
                    Percentage = totalErrors > 0 ? (double)g.Count() / totalErrors * 100 : 0
                })
                .ToList();

            // Error trends by hour
            var errorTrends = allLogs
                .GroupBy(x => new DateTime(x.log.Requested.Year, x.log.Requested.Month, x.log.Requested.Day, x.log.Requested.Hour, 0, 0))
                .Select(g => new ErrorTrendDto
                {
                    Timestamp = g.Key,
                    ErrorCount = g.Count(x => x.log.Status != "Success"),
                    TotalCount = g.Count(),
                    ErrorRate = g.Count() > 0 ? (double)g.Count(x => x.log.Status != "Success") / g.Count() * 100 : 0
                })
                .OrderBy(t => t.Timestamp)
                .ToList();

            // Top errors by frequency
            var topErrors = errorLogs
                .GroupBy(x => new { x.req.MethodName, ErrorType = ClassifyError(x.log.Response) })
                .Select(g => new TopErrorDto
                {
                    MethodName = g.Key.MethodName,
                    ErrorMessage = g.Key.ErrorType,
                    Frequency = g.Count(),
                    LastOccurrence = g.Max(x => x.log.Requested)
                })
                .OrderByDescending(e => e.Frequency)
                .Take(10)
                .ToList();

            return new ErrorAnalysisDto
            {
                TotalErrors = totalErrors,
                ErrorRate = Math.Round(errorRate, 2),
                ErrorsByCategory = errorCategories,
                ErrorTrends = errorTrends,
                TopErrors = topErrors
            };
        }

        public async Task<List<RecentActivityDto>> GetRecentActivityAsync(int limit = 50, string? status = null)
        {
            var query = _dbContext.APIRequestLogs
                .Join(_dbContext.APIRequests, log => log.APIRequestId, req => req.Id, (log, req) => new { log, req });

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.log.Status == status);
            }

            var recentActivity = await query
                .OrderByDescending(x => x.log.Requested)
                .Take(limit)
                .Select(x => new RecentActivityDto
                {
                    Id = x.log.Id,
                    MethodName = x.req.MethodName,
                    HospitalId = x.req.HospitalId,
                    Status = x.log.Status,
                    Requested = x.log.Requested,
                    Responded = x.log.Responded,
                    ResponseTimeMs = x.log.Responded != null
                        ? (x.log.Responded.Value - x.log.Requested).TotalMilliseconds
                        : null,
                    ChargeableItems = x.log.ChargeableItems
                })
                .ToListAsync();

            return recentActivity;
        }

        public async Task<List<TrendDataDto>> GetTrendsAsync(DateTime? startDate = null, DateTime? endDate = null, string groupBy = "hour")
        {
            var (start, end) = GetDateRange(startDate, endDate);

            var logs = await _dbContext.APIRequestLogs
                .Where(l => l.Requested >= start && l.Requested <= end)
                .ToListAsync();

            Func<DateTime, DateTime> groupingFunc = groupBy.ToLower() switch
            {
                "minute" => dt => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0),
                "hour" => dt => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0),
                "day" => dt => new DateTime(dt.Year, dt.Month, dt.Day),
                _ => dt => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0)
            };

            var trends = logs
                .GroupBy(l => groupingFunc(l.Requested))
                .Select(g => new TrendDataDto
                {
                    Timestamp = g.Key,
                    CallCount = g.Count(),
                    SuccessCount = g.Count(l => l.Status == "Success"),
                    FailureCount = g.Count(l => l.Status != "Success"),
                    AverageResponseTime = g.Where(l => l.Responded != null)
                                         .Select(l => (l.Responded!.Value - l.Requested).TotalMilliseconds)
                                         .DefaultIfEmpty(0)
                                         .Average(),
                    TotalChargeableItems = g.Sum(l => l.ChargeableItems)
                })
                .OrderBy(t => t.Timestamp)
                .ToList();

            return trends;
        }

        public async Task<byte[]> ExportDataAsync(DateTime? startDate = null, DateTime? endDate = null, string format = "csv")
        {
            var (start, end) = GetDateRange(startDate, endDate);

            var exportData = await _dbContext.APIRequestLogs
                .Join(_dbContext.APIRequests, log => log.APIRequestId, req => req.Id, (log, req) => new ExportDataDto
                {
                    Timestamp = log.Requested,
                    HospitalId = req.HospitalId,
                    MethodName = req.MethodName,
                    Status = log.Status,
                    ResponseTimeMs = log.Responded != null
                        ? (log.Responded.Value - log.Requested).TotalMilliseconds
                        : null,
                    ChargeableItems = log.ChargeableItems,
                    ErrorMessage = log.Status != "Success" ? log.Response : null
                })
                .Where(x => x.Timestamp >= start && x.Timestamp <= end)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();

            return format.ToLower() switch
            {
                "json" => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true })),
                "csv" => GenerateCsv(exportData),
                _ => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(exportData))
            };
        }

        private (DateTime start, DateTime end) GetDateRange(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddDays(-7);
            return (start, end);
        }

        private string ClassifyError(string? errorResponse)
        {
            if (string.IsNullOrEmpty(errorResponse))
                return "Unknown Error";

            var response = errorResponse.ToLower();

            return response switch
            {
                var r when r.Contains("timeout") => "Timeout Error",
                var r when r.Contains("network") || r.Contains("connection") => "Network Error",
                var r when r.Contains("validation") || r.Contains("invalid") => "Validation Error",
                var r when r.Contains("auth") || r.Contains("unauthorized") => "Authentication Error",
                var r when r.Contains("not found") => "Not Found Error",
                var r when r.Contains("server") || r.Contains("internal") => "Server Error",
                var r when r.Contains("format") || r.Contains("parse") => "Format Error",
                _ => "Other Error"
            };
        }

        private byte[] GenerateCsv(List<ExportDataDto> data)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Timestamp,HospitalId,MethodName,Status,ResponseTimeMs,ChargeableItems,ErrorMessage");

            foreach (var item in data)
            {
                csv.AppendLine($"{item.Timestamp:yyyy-MM-dd HH:mm:ss},{item.HospitalId},{item.MethodName},{item.Status},{item.ResponseTimeMs},{item.ChargeableItems},\"{item.ErrorMessage?.Replace("\"", "\"\"")}\"");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<bool> GenerateSampleDataAsync()
        {
            try
            {
                // Check if we already have data
                var existingLogs = await _dbContext.APIRequestLogs.CountAsync();
                if (existingLogs > 0) return false; // Don't generate if data exists

                var random = new Random();
                var hospitals = new[] { "H92006568", "H12345678", "H87654321", "H11111111", "H22222222" };
                var methods = new[] { "GetRestToken", "GetRestMemberPIN", "eClaimsApiUpload", "SearchRestCaseRate", "FetchClaimStatus" };
                var statuses = new[] { "Success", "Success", "Success", "Failed", "Success" }; // Mostly success

                var sampleLogs = new List<APIRequestLog>();

                // Generate data for the last 30 days
                for (int days = 30; days >= 0; days--)
                {
                    var baseDate = DateTime.UtcNow.AddDays(-days);

                    // Generate 10-50 requests per day
                    var requestsPerDay = random.Next(10, 51);

                    for (int i = 0; i < requestsPerDay; i++)
                    {
                        var requestTime = baseDate.AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60));
                        var responseTime = requestTime.AddMilliseconds(random.Next(100, 3000));
                        var hospital = hospitals[random.Next(hospitals.Length)];
                        var method = methods[random.Next(methods.Length)];
                        var status = statuses[random.Next(statuses.Length)];

                        // Find or create API request
                        var apiRequest = await _dbContext.APIRequests
                            .FirstOrDefaultAsync(a => a.HospitalId == hospital && a.MethodName == method);

                        if (apiRequest == null)
                        {
                            apiRequest = new APIRequest
                            {
                                HospitalId = hospital,
                                MethodName = method,
                                IsActive = true
                            };
                            _dbContext.APIRequests.Add(apiRequest);
                            await _dbContext.SaveChangesAsync();
                        }

                        var log = new APIRequestLog
                        {
                            APIRequestId = apiRequest.Id,
                            ChargeableItems = method == "eClaimsApiUpload" ? random.Next(1, 10) : 0,
                            RequestData = $"{{\"pmcc\":\"{hospital}\",\"method\":\"{method}\"}}",
                            Response = status == "Success" ? "Success response" : "Error occurred",
                            Status = status,
                            Requested = requestTime,
                            Responded = responseTime,
                            CreatedBy = "SampleData",
                            Created = requestTime
                        };

                        sampleLogs.Add(log);
                    }
                }

                _dbContext.APIRequestLogs.AddRange(sampleLogs);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Generated {sampleLogs.Count} sample log entries");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sample data");
                return false;
            }
        }
    }
}