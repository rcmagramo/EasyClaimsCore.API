using Azure.Core;
using EasyClaimsCore.API.Models.Requests;
using EasyClaimsCore.API.Models.Responses;
using EasyClaimsCore.API.Security.Cryptography.DataContracts;
using EasyClaimsCore.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyClaimsCore.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EClaimsController : ControllerBase
    {
        private readonly IEClaimsService _eClaimsService;
        private readonly ILogger<EClaimsController> _logger;

        public EClaimsController(IEClaimsService eClaimsService, ILogger<EClaimsController> logger)
        {
            _eClaimsService = eClaimsService;
            _logger = logger;
        }

        /// <summary>
        /// Generates authentication token for API access
        /// </summary>
        [HttpPost("token")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> GetToken([FromBody] TokenRequest request)
        {
            var result = await _eClaimsService.GetRestTokenAsync(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Retrieves Member PIN based on personal information
        /// </summary>
        [HttpPost("member-pin")]
        public async Task<ActionResult<ApiResponse<object>>> GetMemberPIN([FromBody] MemberPinViewModelRest request)
        {
            var result = await _eClaimsService.GetRestMemberPINAsync(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Searches for case rates based on criteria
        /// </summary>
        [HttpPost("case-rates")]
        public async Task<ActionResult<ApiResponse<object>>> SearchCaseRates([FromBody] CaseRateRestRequest request)
        {
            var result = await _eClaimsService.SearchRestCaseRateAsync(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Retrieves server date and time
        /// </summary>
        [HttpPost("server-datetime")]
        public async Task<ActionResult<ApiResponse<object>>> GetServerDateTime([FromBody] TokenCredentialsRequest request)
        {
            var result = await _eClaimsService.SearchServerDateTime(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Retrieves database date and time
        /// </summary>
        [HttpPost("database-datetime")]
        public async Task<ActionResult<ApiResponse<object>>> GetDatabaseDateTime([FromBody] TokenCredentialsRequest request)
        {
            var result = await _eClaimsService.SearchDatabaseDateTime(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Retrieves Doctor's Professional Association Number (PAN)
        /// </summary>
        [HttpPost("doctor-pan")]
        public async Task<ActionResult<ApiResponse<object>>> GetDoctorPAN([FromBody] DoctorPANRestRequest request)
        {
            var result = await _eClaimsService.GetRestDoctorPANAsync(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Checks if doctor is accredited for given dates
        /// </summary>
        [HttpPost("doctor-accreditation")]
        public async Task<ActionResult<ApiResponse<object>>> CheckDoctorAccreditation([FromBody] DoctorAccreditationRestViewModel request)
        {
            var result = await _eClaimsService.CheckDoctorIfAccredited(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Retrieves PhilHealth server version
        /// </summary>
        [HttpPost("server-version")]
        public async Task<ActionResult<ApiResponse<object>>> GetServerVersion([FromBody] TokenCredentialsRequest request)
        {
            var result = await _eClaimsService.FetchServerVersion(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Searches for employer information
        /// </summary>
        [HttpPost("employers")]
        public async Task<ActionResult<ApiResponse<object>>> SearchEmployer([FromBody] EmployerRequestViewModel request)
        {
            var result = await _eClaimsService.FindEmployer(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Retrieves uploaded claims map by receipt ticket number
        /// </summary>
        [HttpPost("uploaded-claims-map")]
        public async Task<ActionResult<ApiResponse<object>>> GetUploadedClaimsMap([FromBody] UploadedClaimsMapRestRequest request)
        {
            var result = await _eClaimsService.FetchUploadedClaimsMap(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Retrieves claim status by series LHIO numbers
        /// </summary>
        [HttpPost("claim-status")]
        public async Task<ActionResult<ApiResponse<object>>> GetClaimStatus([FromBody] ClaimStatusApiRequest request)
        {
            var result = await _eClaimsService.FetchClaimStatus(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Checks eligibility for claim
        /// </summary>
        [HttpPost("eligibility")]
        public async Task<ActionResult<ApiResponse<object>>> CheckEligibility([FromBody] EligibilityRequestViewModel request)
        {
            var result = await _eClaimsService.IsClaimEligibleAPI(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Retrieves voucher details by voucher number
        /// </summary>
        [HttpPost("voucher-details")]
        public async Task<ActionResult<ApiResponse<object>>> GetVoucherDetails([FromBody] VoucherRestRequest request)
        {
            var result = await _eClaimsService.FetchVoucherDetails(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Validates eClaims file format and content
        /// </summary>
        [HttpPost("validate-eclaims")]
        public async Task<ActionResult<ApiResponse<object>>> ValidateEClaims([FromBody] CommonAPIRequest request)
        {
            var result = await _eClaimsService.EClaimsFileCheckAPI(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Validates eSOA (electronic Statement of Account)
        /// </summary>
        [HttpPost("validate-esoa")]
        public async Task<ActionResult<ApiResponse<object>>> ValidateESOA([FromBody] CommonAPIRequest request)
        {
            var result = await _eClaimsService.IsESOAValid(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Adds required documents to a claim
        /// </summary>
        [HttpPost("required-documents")]
        public async Task<ActionResult<ApiResponse<object>>> AddRequiredDocument([FromBody] RequiredApiDocumentRequest request)
        {
            var result = await _eClaimsService.AddRequiredApiDocument(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Generates PBEF (PhilHealth Benefit Eligibility Form) PDF
        /// </summary>
        [HttpPost("generate-pbef")]
        public async Task<ActionResult<ApiResponse<object>>> GeneratePBEF([FromBody] PBEFRequest request)
        {
            var result = await _eClaimsService.GeneratePBEFPDF(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Uploads eClaims data to PhilHealth
        /// </summary>
        [HttpPost("upload-eclaims")]
        public async Task<ActionResult<ApiResponse<object>>> UploadEClaims([FromBody] CommonAPIRequest request)
        {
            var result = await _eClaimsService.EClaimsApiUpload(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Validates DRG (Diagnosis Related Group) data
        /// </summary>
        [HttpPost("validate-drg")]
        public async Task<ActionResult<ApiResponse<object>>> ValidateDRG([FromBody] DRGRequest request)
        {
            var result = await _eClaimsService.IsDRGValidAPI(request);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Mock response for testing purposes
        /// </summary>
        [HttpPost("mock-response")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ApiResponse<object>>> MockResponse([FromBody] MockRequest request)
        {
            var result = await _eClaimsService.MockResponseAPI(request);
            return HandleServiceResult(result);
        }

        [HttpPost("mock-decypted-response")]
        public async Task<ActionResult<ApiResponse<object>>> MockDecryptResponse([FromBody] MockDecryptedRequest payload)
        {
            var result = await _eClaimsService.MockDecryptResponseAPI(payload);
            return HandleServiceResult(result);
        }

        [HttpPost("mock-encrypt")]
        public async Task<ActionResult<ApiResponse<object>>> MockEncryptResponse([FromBody] CommonAPIRequest request)
        {
            var result = await _eClaimsService.MockEncryptResponseAPI(request);
            return HandleServiceResult(result);
        }

        private ActionResult<ApiResponse<object>> HandleServiceResult(object result)
        {
            return result switch
            {
                ApiResponse<object> apiResponse => StatusCode((int)apiResponse.StatusCode, apiResponse),
                _ => Ok(ApiResponse<object>.CreateSuccess(result))
            };
        }
    }
}