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

                // Clean RequestData to keep only Xml field
                var cleanedRequestData = CleanRequestDataForSuccessLog(originalLog.RequestData);

                // Clean Response data to keep only XML content
                var cleanedResponse = CleanResponseDataForSuccessLog(originalLog.Response);

                // Get current bill amount
                var billAmount = await GetCurrentBillAmountAsync();
                
                var successLog = new APIRequestSuccessLog
                {
                    APIRequestId = originalLog.APIRequestId,
                    ChargeableItems = originalLog.ChargeableItems,
                    RequestData = cleanedRequestData, // Use cleaned data
                    Response = cleanedResponse, // Use cleaned response
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

        private string CleanResponseDataForSuccessLog(string? responseData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(responseData))
                    return string.Empty;

                // Handle case where response is already clean XML
                if (responseData.Trim().StartsWith("<") && responseData.Trim().EndsWith(">"))
                {
                    return responseData.Trim();
                }

                // Handle case where response has format: { Message = , Result = <XML...> }
                // or similar patterns
                var response = responseData.Trim();

                // Look for "Result =" pattern and extract XML after it
                var resultPattern = "Result =";
                var resultIndex = response.IndexOf(resultPattern, StringComparison.OrdinalIgnoreCase);

                if (resultIndex >= 0)
                {
                    // Extract everything after "Result ="
                    var xmlStart = resultIndex + resultPattern.Length;
                    var xmlContent = response.Substring(xmlStart).Trim();

                    // Remove trailing "}" or other unwanted characters
                    if (xmlContent.EndsWith("}"))
                    {
                        xmlContent = xmlContent.Substring(0, xmlContent.Length - 1).Trim();
                    }

                    // Find the actual XML content (should start with < and end with >)
                    var firstAngleBracket = xmlContent.IndexOf('<');
                    if (firstAngleBracket >= 0)
                    {
                        xmlContent = xmlContent.Substring(firstAngleBracket);

                        // Find the last closing angle bracket for the XML tag
                        var lastAngleBracket = xmlContent.LastIndexOf('>');
                        if (lastAngleBracket >= 0)
                        {
                            xmlContent = xmlContent.Substring(0, lastAngleBracket + 1);
                        }
                    }

                    return xmlContent.Trim();
                }

                // If no "Result =" pattern found, look for XML content directly
                var firstXmlTag = response.IndexOf('<');
                var lastXmlTag = response.LastIndexOf('>');

                if (firstXmlTag >= 0 && lastXmlTag > firstXmlTag)
                {
                    return response.Substring(firstXmlTag, lastXmlTag - firstXmlTag + 1);
                }

                // If no XML found, return original
                return responseData;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not clean response data, using original: {ResponseData}",
                    responseData?.Length > 100 ? responseData.Substring(0, 100) + "..." : responseData);
                return responseData ?? string.Empty; // Return original if cleaning fails
            }
        }

        private string CleanRequestDataForSuccessLog(string requestData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestData))
                    return string.Empty;

                var jsonObject = JObject.Parse(requestData);

                // Create new JSON object with only the Xml field
                var cleanedObject = new JObject();
                if (jsonObject["Xml"] != null)
                {
                    cleanedObject["Xml"] = jsonObject["Xml"];
                }

                return JsonConvert.SerializeObject(cleanedObject, Formatting.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not clean request data, using original: {RequestData}",
                    requestData?.Length > 100 ? requestData.Substring(0, 100) + "..." : requestData);
                return requestData; // Return original if cleaning fails
            }
        }
    }
}