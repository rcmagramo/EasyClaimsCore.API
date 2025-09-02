using EasyClaimsCore.API.Data;
using EasyClaimsCore.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyClaimsCore.API.Services
{
    public class APIRequestSuccessLogService : IAPIRequestSuccessLogService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<APIRequestSuccessLogService> _logger;

        public APIRequestSuccessLogService(ApplicationDbContext dbContext, ILogger<APIRequestSuccessLogService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<APIRequestSuccessLog> LogSuccessfulEClaimsUploadAsync(APIRequestLog originalLog)
        {
            try
            {
                // Extract PMCC from RequestData
                var pmcc = ExtractPmccFromRequestData(originalLog.RequestData);

                // Get current bill amount
                var billAmount = await GetCurrentBillAmountAsync();

                var successLog = new APIRequestSuccessLog
                {
                    APIRequestId = originalLog.APIRequestId,
                    ChargeableItems = originalLog.ChargeableItems,
                    RequestData = originalLog.RequestData,
                    Response = originalLog.Response,
                    Status = originalLog.Status,
                    Requested = originalLog.Requested,
                    Responded = originalLog.Responded,
                    CreatedBy = originalLog.CreatedBy,
                    Created = originalLog.Created,
                    UpdatedBy = originalLog.UpdatedBy,
                    Updated = originalLog.Updated,
                    Pmcc = pmcc,
                    IsBilled = false,
                    IsActive = true,
                    BillAmount = billAmount
                };

                _dbContext.APIRequestSuccessLogs.Add(successLog);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully logged EClaimsApiUpload success for PMCC: {Pmcc}, ID: {Id}",
                    pmcc, successLog.Id);

                return successLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging successful EClaimsApiUpload for log ID: {LogId}", originalLog.Id);
                throw;
            }
        }

        public async Task<int> GetCurrentBillAmountAsync()
        {
            try
            {
                var activeBillParameter = await _dbContext.BillAmountParameters
                    .Where(b => b.IsActive)
                    .OrderByDescending(b => b.Id)
                    .FirstOrDefaultAsync();

                return activeBillParameter?.Price ?? 10; // Default to 10 if no active parameter found
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current bill amount, using default value");
                return 10; // Fallback to default
            }
        }

        public async Task UpdateBillAmountAsync(int newPrice)
        {
            try
            {
                // Deactivate all current active parameters
                var activeParameters = await _dbContext.BillAmountParameters
                    .Where(b => b.IsActive)
                    .ToListAsync();

                foreach (var param in activeParameters)
                {
                    param.IsActive = false;
                }

                // Create new active parameter
                var newParameter = new BillAmountParameter
                {
                    Price = newPrice,
                    IsActive = true
                };

                _dbContext.BillAmountParameters.Add(newParameter);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Updated bill amount to {NewPrice}", newPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bill amount to {NewPrice}", newPrice);
                throw;
            }
        }

        private string ExtractPmccFromRequestData(string requestData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestData))
                    return string.Empty;

                var jsonObject = JObject.Parse(requestData);
                return jsonObject["pmcc"]?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not extract PMCC from request data: {RequestData}",
                    requestData?.Length > 100 ? requestData.Substring(0, 100) + "..." : requestData);
                return string.Empty;
            }
        }
    }
}