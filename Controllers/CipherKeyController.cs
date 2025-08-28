using EasyClaimsCore.API.Models.Responses;
using EasyClaimsCore.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyClaimsCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    //[Authorize] // Uncomment when authentication is set up
    public class CipherKeyController : ControllerBase
    {
        private readonly ICipherKeyService _cipherKeyService;
        private readonly ILogger<CipherKeyController> _logger;

        public CipherKeyController(ICipherKeyService cipherKeyService, ILogger<CipherKeyController> logger)
        {
            _cipherKeyService = cipherKeyService;
            _logger = logger;
        }



        #region CipherKeyNote  

        //MUST be DISABLED so that API user's cannot see/update all cipher keys

        ///// <summary>
        ///// Get cipher key for a specific hospital
        ///// </summary>
        ///// <param name="hospitalId">Hospital ID</param>
        //[HttpGet("{hospitalId}")]
        //public async Task<ActionResult<ApiResponse<object>>> GetCipherKey(string hospitalId)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(hospitalId))
        //        {
        //            return BadRequest(ApiResponse<object>.CreateError("Hospital ID is required"));
        //        }

        //        var cipherKey = await _cipherKeyService.GetCipherKeyAsync(hospitalId);

        //        // Return masked cipher key for security (only show first 4 and last 4 characters)
        //        var maskedKey = MaskCipherKey(cipherKey);

        //        return Ok(ApiResponse<object>.CreateSuccess(new
        //        {
        //            HospitalId = hospitalId,
        //            CipherKey = maskedKey,
        //            Length = cipherKey.Length
        //        }, "Cipher key retrieved successfully"));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error retrieving cipher key for hospital {HospitalId}", hospitalId);
        //        return StatusCode(500, ApiResponse<object>.CreateError("Internal server error"));
        //    }
        //}

        ///// <summary>
        ///// Get all cipher keys (masked for security)
        ///// </summary>
        //[HttpGet]
        //public async Task<ActionResult<ApiResponse<object>>> GetAllCipherKeys()
        //{
        //    try
        //    {
        //        var cipherKeys = await _cipherKeyService.GetAllCipherKeysAsync();

        //        var maskedKeys = cipherKeys.ToDictionary(
        //            kvp => kvp.Key,
        //            kvp => new {
        //                CipherKey = MaskCipherKey(kvp.Value),
        //                Length = kvp.Value.Length
        //            });

        //        return Ok(ApiResponse<object>.CreateSuccess(maskedKeys, "All cipher keys retrieved successfully"));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error retrieving all cipher keys");
        //        return StatusCode(500, ApiResponse<object>.CreateError("Internal server error"));
        //    }
        //}

        ///// <summary>
        ///// Update cipher key for a specific hospital
        ///// </summary>
        ///// <param name="hospitalId">Hospital ID</param>
        ///// <param name="request">Cipher key update request</param>
        //[HttpPut("{hospitalId}")]
        //public async Task<ActionResult<ApiResponse<object>>> UpdateCipherKey(string hospitalId, [FromBody] UpdateCipherKeyRequest request)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(hospitalId))
        //        {
        //            return BadRequest(ApiResponse<object>.CreateError("Hospital ID is required"));
        //        }

        //        if (string.IsNullOrWhiteSpace(request.NewCipherKey))
        //        {
        //            return BadRequest(ApiResponse<object>.CreateError("New cipher key is required"));
        //        }

        //        if (request.NewCipherKey.Length < 16)
        //        {
        //            return BadRequest(ApiResponse<object>.CreateError("Cipher key must be at least 16 characters long"));
        //        }

        //        var success = await _cipherKeyService.UpdateCipherKeyAsync(hospitalId, request.NewCipherKey);

        //        if (success)
        //        {
        //            _logger.LogInformation("Cipher key updated successfully for hospital {HospitalId}", hospitalId);
        //            return Ok(ApiResponse<object>.CreateSuccess(new { HospitalId = hospitalId }, "Cipher key updated successfully"));
        //        }
        //        else
        //        {
        //            return BadRequest(ApiResponse<object>.CreateError("Failed to update cipher key. Hospital may not exist."));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating cipher key for hospital {HospitalId}", hospitalId);
        //        return StatusCode(500, ApiResponse<object>.CreateError("Internal server error"));
        //    }
        //}

        #endregion

        /// <summary>
        /// Validate cipher key format
        /// </summary>
        /// <param name="request">Cipher key validation request</param>
        [HttpPost("validate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<ApiResponse<object>> ValidateCipherKey([FromBody] ValidateCipherKeyRequest request)
        {
            try
            {
                var validationResult = ValidateCipherKeyFormat(request.CipherKey);

                return Ok(ApiResponse<object>.CreateSuccess(new
                {
                    IsValid = validationResult.IsValid,
                    Message = validationResult.Message,
                    Recommendations = validationResult.Recommendations
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating cipher key");
                return StatusCode(500, ApiResponse<object>.CreateError("Internal server error"));
            }
        }

        /// <summary>
        /// Generate a secure cipher key
        /// </summary>
        [HttpPost("generate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<ApiResponse<object>> GenerateCipherKey([FromBody] GenerateCipherKeyRequest? request = null)
        {
            try
            {
                var length = request?.Length ?? 24;
                if (length < 16 || length > 64)
                {
                    return BadRequest(ApiResponse<object>.CreateError("Cipher key length must be between 16 and 64 characters"));
                }

                var newCipherKey = GenerateSecureCipherKey(length);

                return Ok(ApiResponse<object>.CreateSuccess(new
                {
                    CipherKey = newCipherKey,
                    Length = newCipherKey.Length,
                    Strength = "Strong"
                }, "Secure cipher key generated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating cipher key");
                return StatusCode(500, ApiResponse<object>.CreateError("Internal server error"));
            }
        }

        private string MaskCipherKey(string cipherKey)
        {
            if (string.IsNullOrEmpty(cipherKey) || cipherKey.Length <= 8)
                return "****";

            return $"{cipherKey.Substring(0, 4)}{"*".PadLeft(Math.Max(4, cipherKey.Length - 8), '*')}{cipherKey.Substring(cipherKey.Length - 4)}";
        }

        private CipherKeyValidationResult ValidateCipherKeyFormat(string cipherKey)
        {
            var result = new CipherKeyValidationResult();
            var recommendations = new List<string>();

            if (string.IsNullOrWhiteSpace(cipherKey))
            {
                result.IsValid = false;
                result.Message = "Cipher key cannot be empty";
                return result;
            }

            if (cipherKey.Length < 16)
            {
                result.IsValid = false;
                result.Message = "Cipher key must be at least 16 characters long";
                recommendations.Add("Use a cipher key with at least 16 characters");
            }

            if (cipherKey.Length > 64)
            {
                result.IsValid = false;
                result.Message = "Cipher key should not exceed 64 characters";
                recommendations.Add("Use a cipher key with maximum 64 characters");
            }

            // Check for character diversity
            bool hasUpper = cipherKey.Any(char.IsUpper);
            bool hasLower = cipherKey.Any(char.IsLower);
            bool hasDigit = cipherKey.Any(char.IsDigit);
            bool hasSpecial = cipherKey.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c));

            if (!hasUpper) recommendations.Add("Consider including uppercase letters");
            if (!hasLower) recommendations.Add("Consider including lowercase letters");
            if (!hasDigit) recommendations.Add("Consider including numbers");
            if (!hasSpecial) recommendations.Add("Consider including special characters");

            if (result.IsValid != false) // Not explicitly set to false above
            {
                result.IsValid = true;
                result.Message = "Cipher key format is valid";
            }

            result.Recommendations = recommendations;
            return result;
        }

        private string GenerateSecureCipherKey(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public class UpdateCipherKeyRequest
    {
        public string NewCipherKey { get; set; } = string.Empty;
    }

    public class ValidateCipherKeyRequest
    {
        public string CipherKey { get; set; } = string.Empty;
    }

    public class GenerateCipherKeyRequest
    {
        public int Length { get; set; } = 24;
    }

    public class CipherKeyValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Recommendations { get; set; } = new List<string>();
    }
}