using EasyClaimsCore.API.Models.Analytics;

namespace EasyClaimsCore.API.Services.Analytics
{
    public interface IAnalyticsService
    {
        Task<OverviewDto> GetOverviewAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<ApiUsageDto>> GetApiUsageAsync(DateTime? startDate = null, DateTime? endDate = null, string? hospitalId = null);
        Task<PerformanceMetricsDto> GetPerformanceMetricsAsync(DateTime? startDate = null, DateTime? endDate = null, string? methodName = null);
        Task<List<HospitalAnalyticsDto>> GetHospitalAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<ErrorAnalysisDto> GetErrorAnalysisAsync(DateTime? startDate = null, DateTime? endDate = null, string? hospitalId = null);
        Task<List<RecentActivityDto>> GetRecentActivityAsync(int limit = 50, string? status = null);
        Task<List<TrendDataDto>> GetTrendsAsync(DateTime? startDate = null, DateTime? endDate = null, string groupBy = "hour");
        Task<byte[]> ExportDataAsync(DateTime? startDate = null, DateTime? endDate = null, string format = "csv");
    }
}