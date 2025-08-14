using EasyClaimsCore.API.Data;
using EasyClaimsCore.API.Data.Entities;
using EasyClaimsCore.API.Models.Requests;
using Newtonsoft.Json;

namespace EasyClaimsCore.API.Services.Logging
{
    public class RequestLogger : IRequestLogger
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<RequestLogger> _logger;

        public RequestLogger(ApplicationDbContext dbContext, ILogger<RequestLogger> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<APIRequestLog> LogRequestAsync<TRequest>(RequestName requestName, TRequest request, int chargeableItems = 0)
        {
            try
            {
                var apiRequestLog = new APIRequestLog
                {
                    APIRequestId = (int)requestName,
                    ChargeableItems = chargeableItems,
                    RequestData = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.None
                    }),
                    Status = RequestStatus.Pending.ToString(),
                    Requested = DateTime.UtcNow,
                    CreatedBy = "System",
                    Created = DateTime.UtcNow
                };

                _dbContext.APIRequestLogs.Add(apiRequestLog);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Request logged with ID {RequestLogId} for service {RequestName}",
                    apiRequestLog.Id, requestName);

                return apiRequestLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log request for service {RequestName}", requestName);
                throw new ApplicationException("Request failed on logging! Please try again later.");
            }
        }

        public async Task UpdateResponseAsync(APIRequestLog log, object response, RequestStatus status)
        {
            try
            {
                var responseString = response?.ToString() ?? "No response";

                // Mask sensitive data in logs
                responseString = MaskSensitiveData(responseString);

                log.Response = responseString;
                log.Status = status.ToString();
                log.Responded = DateTime.UtcNow;
                log.UpdatedBy = "System";
                log.Updated = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Updated response log {RequestLogId} with status {Status}",
                    log.Id, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update response log {RequestLogId}", log.Id);
                // Don't throw here to avoid masking the original error
            }
        }

        private string MaskSensitiveData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return data;

            var maskedData = data;

            // Mask PIN numbers (assuming they're 12 digits)
            maskedData = System.Text.RegularExpressions.Regex.Replace(
                maskedData, @"\b\d{12}\b", "************");

            // Mask other sensitive patterns as needed

            return maskedData;
        }
    }
}