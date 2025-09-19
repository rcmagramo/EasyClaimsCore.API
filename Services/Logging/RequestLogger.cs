using EasyClaimsCore.API.Data;
using EasyClaimsCore.API.Data.Entities;
using EasyClaimsCore.API.Models.Requests;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EasyClaimsCore.API.Services.Logging
{
    public class RequestLogger : IRequestLogger
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<RequestLogger> _logger;
        private readonly IAPIRequestSuccessLogService _successLogService;

        public RequestLogger(
            ApplicationDbContext dbContext,
            ILogger<RequestLogger> logger,
            IAPIRequestSuccessLogService successLogService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _successLogService = successLogService;
        }

        public async Task<APIRequestLog> LogRequestAsync<TRequest>(RequestName requestName, TRequest request, int chargeableItems = 0)
        {
            try
            {
                // First, get the APIRequest ID for this hospital and method
                var hospitalId = GetHospitalIdFromRequest(request);
                var apiRequest = await _dbContext.APIRequests
                    .FirstOrDefaultAsync(ar => ar.HospitalId == hospitalId && ar.MethodName == requestName.ToString());

                if (apiRequest == null)
                {
                    throw new InvalidOperationException($"No API request configuration found for hospital {hospitalId} and method {requestName}");
                }

                var apiRequestLog = new APIRequestLog
                {
                    APIRequestId = apiRequest.Id, // Use the actual APIRequest.Id, not the enum value
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

                //// Mask sensitive data in logs
                //responseString = MaskSensitiveData(responseString);

                log.Response = responseString;
                log.Status = status.ToString();
                log.Responded = DateTime.UtcNow;
                log.UpdatedBy = "System";
                log.Updated = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Updated response log {RequestLogId} with status {Status}",
                    log.Id, status);

                // Check if this is a successful EClaimsApiUpload and log to success table
                await LogSuccessfulEClaimsUploadIfApplicable(log, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update response log {RequestLogId}", log.Id);
                // Don't throw here to avoid masking the original error
            }
        }

        private async Task LogSuccessfulEClaimsUploadIfApplicable(APIRequestLog log, RequestStatus status)
        {
            try
            {
                // Only log if status is Success
                if (status != RequestStatus.Success)
                    return;

                // Check if this is an EClaimsApiUpload request
                var apiRequest = await _dbContext.APIRequests
                    .FirstOrDefaultAsync(ar => ar.Id == log.APIRequestId);

                if (apiRequest?.MethodName != RequestName.eClaimsApiUpload.ToString())
                    return;

                // Log to success table
                await _successLogService.LogSuccessfulEClaimsUploadAsync(log);

                _logger.LogInformation("Successfully logged EClaimsApiUpload success to APIRequestSuccessLogs for log ID: {LogId}", log.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log successful EClaimsApiUpload to success table for log ID: {LogId}", log.Id);
                // Don't throw to avoid affecting the main flow
            }
        }

        private string GetHospitalIdFromRequest<TRequest>(TRequest request)
        {
            if (request is IBaseRequest baseRequest)
            {
                return baseRequest.pmcc;
            }
            return string.Empty;
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