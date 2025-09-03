namespace EasyClaimsCore.API.Models.Analytics
{
    public class OverviewDto
    {
        public int TotalCalls { get; set; }
        public int SuccessfulCalls { get; set; }
        public int FailedCalls { get; set; }
        public double SuccessRate { get; set; }
        public double LastResponseTime { get; set; } // Changed from AverageResponseTime
        public int ActiveHospitals { get; set; }
        public int TotalChargeableItems { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    // New DTO for pending uploads
    public class PendingUploadDto
    {
        public string Pmcc { get; set; } = string.Empty;
        public int PendingCount { get; set; }
        public DateTime LastUpload { get; set; }
        public decimal TotalBillAmount { get; set; }
    }

    public class ApiUsageDto
    {
        public string MethodName { get; set; } = string.Empty;
        public int CallCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public double SuccessRate { get; set; }
        public double AverageResponseTime { get; set; }
        public int TotalChargeableItems { get; set; }
        public DateTime LastUsed { get; set; }
    }

    public class PerformanceMetricsDto
    {
        public double AverageResponseTime { get; set; }
        public double MedianResponseTime { get; set; }
        public double MinResponseTime { get; set; }
        public double MaxResponseTime { get; set; }
        public List<ResponseTimeDistributionDto> ResponseTimeDistribution { get; set; } = new();
        public List<PerformanceTrendDto> ResponseTimeTrends { get; set; } = new();
    }

    public class ResponseTimeDistributionDto
    {
        public string Range { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class PerformanceTrendDto
    {
        public DateTime Timestamp { get; set; }
        public double AverageResponseTime { get; set; }
        public int CallCount { get; set; }
    }

    public class HospitalAnalyticsDto
    {
        public string HospitalId { get; set; } = string.Empty;
        public int TotalCalls { get; set; }
        public int SuccessfulCalls { get; set; }
        public int FailedCalls { get; set; }
        public double SuccessRate { get; set; }
        public double AverageResponseTime { get; set; }
        public int TotalChargeableItems { get; set; }
        public DateTime LastActivity { get; set; }
        public List<string> MostUsedEndpoints { get; set; } = new();
        public Dictionary<string, int> EndpointUsage { get; set; } = new();
    }

    public class ErrorAnalysisDto
    {
        public int TotalErrors { get; set; }
        public double ErrorRate { get; set; }
        public List<ErrorCategoryDto> ErrorsByCategory { get; set; } = new();
        public List<ErrorTrendDto> ErrorTrends { get; set; } = new();
        public List<TopErrorDto> TopErrors { get; set; } = new();
    }

    public class ErrorCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class ErrorTrendDto
    {
        public DateTime Timestamp { get; set; }
        public int ErrorCount { get; set; }
        public int TotalCount { get; set; }
        public double ErrorRate { get; set; }
    }

    public class TopErrorDto
    {
        public string MethodName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public int Frequency { get; set; }
        public DateTime LastOccurrence { get; set; }
    }

    public class RecentActivityDto
    {
        public int Id { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public string HospitalId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime Requested { get; set; }
        public DateTime? Responded { get; set; }
        public double? ResponseTimeMs { get; set; }
        public int ChargeableItems { get; set; }
    }

    public class TrendDataDto
    {
        public DateTime Timestamp { get; set; }
        public int CallCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public double AverageResponseTime { get; set; }
        public int TotalChargeableItems { get; set; }
    }

    public class ExportDataDto
    {
        public DateTime Timestamp { get; set; }
        public string HospitalId { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double? ResponseTimeMs { get; set; }
        public int ChargeableItems { get; set; }
        public string? ErrorMessage { get; set; }
    }
}