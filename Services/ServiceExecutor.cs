using EasyClaimsCore.API.Data;
using EasyClaimsCore.API.Data.Entities;
using EasyClaimsCore.API.Extensions;
using EasyClaimsCore.API.Models.Exceptions;
using EasyClaimsCore.API.Models.Requests;
using EasyClaimsCore.API.Models.Responses;
using EasyClaimsCore.API.Security.Cryptography;
using EasyClaimsCore.API.Services.Logging;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;

namespace EasyClaimsCore.API.Services
{
    public class ServiceExecutor : IServiceExecutor
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICryptoEngine _cryptoEngine;
        private readonly IRequestLogger _requestLogger;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceExecutor> _logger;

        public ServiceExecutor(
            ApplicationDbContext dbContext,
            ICryptoEngine cryptoEngine,
            IRequestLogger requestLogger,
            IConfiguration configuration,
            ILogger<ServiceExecutor> logger)
        {
            _dbContext = dbContext;
            _cryptoEngine = cryptoEngine;
            _requestLogger = requestLogger;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ApiResponse<object>> ExecuteServiceAsync<TRequest>(
            RequestName requestName,
            TRequest request,
            Func<TRequest, Task<object>> serviceImplementation,
            int chargeableItems = 0) where TRequest : IBaseRequest
        {
            var stopwatch = Stopwatch.StartNew();
            APIRequestLog? apiRequestLog = null;

            try
            {
                // Step 1: Request Validation
                ValidateRequest(request);

                // Step 2: Hospital/Service Authorization Check
                var apiRequest = await ValidateHospitalAccessAsync(request, requestName);

                // Step 3: Request Logging (if configured)
                if (ShouldLogRequest(requestName))
                {
                    apiRequestLog = await _requestLogger.LogRequestAsync(requestName, request, chargeableItems);
                }

                // Step 4-8: Execute Service Implementation
                var result = await serviceImplementation(request);

                // Process Response
                var response = ProcessResponse(result, request);

                // Update Success Log
                if (apiRequestLog != null)
                {
                    await _requestLogger.UpdateResponseAsync(apiRequestLog, response, RequestStatus.Success);
                }

                stopwatch.Stop();
                _logger.LogInformation("Service {RequestName} completed successfully in {ElapsedMs}ms",
                    requestName, stopwatch.ElapsedMilliseconds);

                return ApiResponse<object>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Log Error
                if (apiRequestLog != null)
                {
                    await _requestLogger.UpdateResponseAsync(
                        apiRequestLog,
                        $"Error: {ex.GetUserMessage()} ({stopwatch.ElapsedMilliseconds}ms)",
                        RequestStatus.Failed);
                }

                // Classify Error and Create Response
                var (statusCode, errorCode) = ClassifyError(ex);
                var errorMessage = $"{ex.GetUserMessage()} ({stopwatch.ElapsedMilliseconds}ms)";

                if (apiRequestLog?.Id > 0)
                {
                    errorMessage += $" [TID#{apiRequestLog.Id}]";
                }

                _logger.LogError(ex, "Service {RequestName} failed after {ElapsedMs}ms: {ErrorMessage}",
                    requestName, stopwatch.ElapsedMilliseconds, ex.Message);

                return ApiResponse<object>.CreateError(errorMessage, errorCode, statusCode);
            }
        }

        private void ValidateRequest<TRequest>(TRequest request) where TRequest : IBaseRequest
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.pmcc))
                throw new ValidationException("PMCC (Hospital Code) is required");

            //if (string.IsNullOrWhiteSpace(request.certificateId))
            //    throw new ValidationException("Certificate ID is required");

            // Request-specific validations
            switch (request)
            {
                case MemberPinViewModelRest memberRequest:
                    if (string.IsNullOrWhiteSpace(memberRequest.lastname))
                        throw new ValidationException("Last Name is required for member PIN lookup");
                    if (string.IsNullOrWhiteSpace(memberRequest.birthdate))
                        throw new ValidationException("Birth Date is required for member PIN lookup");
                    if (string.IsNullOrWhiteSpace(memberRequest.firstname))
                        throw new ValidationException("First Name is required for member PIN lookup");
                    break;

                case EligibilityRequestViewModel eligibilityRequest:
                    if (string.IsNullOrWhiteSpace(eligibilityRequest.MemberPIN))
                        throw new ValidationException("Member PIN is required for eligibility check");
                    if (string.IsNullOrWhiteSpace(eligibilityRequest.AdmissionDate))
                        throw new ValidationException("Admission date is required for eligibility check");
                    break;

                case ClaimStatusApiRequest claimRequest:
                    if (string.IsNullOrWhiteSpace(claimRequest.serieslhionos))
                        throw new ValidationException("Series LHIO numbers are required for claim status");
                    break;

                case CommonAPIRequest commonRequest:
                    if (string.IsNullOrWhiteSpace(commonRequest.Xml))
                        throw new ValidationException("XML data is required");
                    break;
            }
        }

        private async Task<APIRequest> ValidateHospitalAccessAsync<TRequest>(TRequest request, RequestName requestName)
            where TRequest : IBaseRequest
        {
            // For now, use a simple hospital ID lookup based on PMCC
            // In production, this would involve proper hospital authentication
            //var hospitalId = GetHospitalId(request.pmcc);
            var hospitalId = request.pmcc;

            var apiRequest = await _dbContext.APIRequests
                .FirstOrDefaultAsync(a => a.HospitalId == hospitalId && a.MethodName == requestName.ToString());

            if (apiRequest == null || !apiRequest.IsActive)
                throw new System.MethodAccessException("Hospital is not allowed or registered to use this service call!");

            return apiRequest;
        }

        private int GetHospitalId(string pmcc)
        {
            // Simple hash-based hospital ID generation for demo
            // In production, this would be a proper lookup
            return Math.Abs(pmcc.GetHashCode()) % 1000 + 1;
        }

        private bool ShouldLogRequest(RequestName requestName)
        {
            // Configure which requests should be logged
            var loggedRequests = _configuration.GetSection("Logging:LoggedRequests")
                .Get<string[]>() ?? Array.Empty<string>();

            return loggedRequests.Contains(requestName.ToString()) ||
                   _configuration.GetValue<bool>("Logging:LogAllRequests", true);
        }

        private object ProcessResponse<TRequest>(object result, TRequest request) where TRequest : IBaseRequest
        {
            // Apply any response processing logic here
            if (result == null)
                return new { Message = "No data found", Success = false };

            // Handle different response types
            return result;
        }

        private (HttpStatusCode statusCode, string errorCode) ClassifyError(Exception ex)
        {
            return ex switch
            {
                ArgumentException or ArgumentNullException or ValidationException =>
                    (HttpStatusCode.BadRequest, "VALIDATION_ERROR"),
                System.MethodAccessException =>
                    (HttpStatusCode.ServiceUnavailable, "AUTH_ERROR"),
                UnauthorizedAccessException =>
                    (HttpStatusCode.Forbidden, "ACCESS_DENIED"),
                FormatException =>
                    (HttpStatusCode.BadRequest, "FORMAT_ERROR"),
                DbUpdateException =>
                    (HttpStatusCode.BadRequest, "DATABASE_ERROR"),
                HttpRequestException =>
                    (HttpStatusCode.BadGateway, "EXT_API_ERROR"),
                TimeoutException =>
                    (HttpStatusCode.RequestTimeout, "TIMEOUT_ERROR"),
                ExternalApiException =>
                    (HttpStatusCode.BadGateway, "EXT_API_ERROR"),
                _ => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR")
            };
        }
    }
}