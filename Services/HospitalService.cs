using EasyClaimsCore.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EasyClaimsCore.API.Services
{
    public interface IHospitalService
    {
        Task<string> GetHospitalCodeAsync(string hospitalId);
        Task<string> GetCipherKeyAsync(string hospitalId);
        Task<(string HospitalCode, string CipherKey)> GetHospitalCredentialsAsync(string hospitalId);
        Task<bool> UpdateHospitalCodeAsync(string hospitalId, string newHospitalCode);
        Task<Dictionary<string, string>> GetAllHospitalCodesAsync();
    }

    public class HospitalService : IHospitalService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly ILogger<HospitalService> _logger;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        public HospitalService(
            ApplicationDbContext dbContext,
            IMemoryCache cache,
            ILogger<HospitalService> logger,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> GetHospitalCodeAsync(string hospitalId)
        {
            if (string.IsNullOrWhiteSpace(hospitalId))
            {
                throw new ArgumentException("Hospital ID cannot be null or empty", nameof(hospitalId));
            }

            // Try to get from cache first
            var cacheKey = $"hospital_code_{hospitalId}";
            if (_cache.TryGetValue(cacheKey, out string? cachedHospitalCode) && !string.IsNullOrEmpty(cachedHospitalCode))
            {
                _logger.LogDebug("Retrieved hospital code from cache for hospital {HospitalId}", hospitalId);
                return cachedHospitalCode;
            }

            try
            {
                // Query database for hospital code
                var apiRequest = await _dbContext.APIRequests
                    .Where(ar => ar.HospitalId == hospitalId && ar.IsActive)
                    .Select(ar => new { ar.HospitalCode })
                    .FirstOrDefaultAsync();

                if (apiRequest == null || string.IsNullOrWhiteSpace(apiRequest.HospitalCode))
                {
                    _logger.LogWarning("No hospital code found for hospital {HospitalId}, using fallback", hospitalId);

                    // Fallback to configuration if not found in database
                    var fallbackCode = _configuration["PhilHealth:DefaultHospitalCode"] ?? "311630";

                    // Cache the fallback code for a shorter time
                    _cache.Set(cacheKey, fallbackCode, TimeSpan.FromMinutes(5));
                    return fallbackCode;
                }

                // Cache the hospital code
                _cache.Set(cacheKey, apiRequest.HospitalCode, _cacheExpiration);

                _logger.LogDebug("Retrieved hospital code from database for hospital {HospitalId}", hospitalId);
                return apiRequest.HospitalCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hospital code for hospital {HospitalId}", hospitalId);

                // Fallback to configuration in case of database error
                var fallbackCode = _configuration["PhilHealth:DefaultHospitalCode"] ?? "311630";
                return fallbackCode;
            }
        }

        public async Task<string> GetCipherKeyAsync(string hospitalId)
        {
            if (string.IsNullOrWhiteSpace(hospitalId))
            {
                throw new ArgumentException("Hospital ID cannot be null or empty", nameof(hospitalId));
            }

            // Try to get from cache first
            var cacheKey = $"cipher_key_{hospitalId}";
            if (_cache.TryGetValue(cacheKey, out string? cachedCipherKey) && !string.IsNullOrEmpty(cachedCipherKey))
            {
                _logger.LogDebug("Retrieved cipher key from cache for hospital {HospitalId}", hospitalId);
                return cachedCipherKey;
            }

            try
            {
                // Query database for cipher key
                var apiRequest = await _dbContext.APIRequests
                    .Where(ar => ar.HospitalId == hospitalId && ar.IsActive)
                    .Select(ar => new { ar.CipherKey })
                    .FirstOrDefaultAsync();

                if (apiRequest == null || string.IsNullOrWhiteSpace(apiRequest.CipherKey))
                {
                    _logger.LogWarning("No cipher key found for hospital {HospitalId}, using fallback", hospitalId);

                    // Fallback to configuration if not found in database
                    var fallbackKey = _configuration["PhilHealth:CipherKey"] ?? "PHilheaLthDuMmy311630";

                    // Cache the fallback key for a shorter time
                    _cache.Set(cacheKey, fallbackKey, TimeSpan.FromMinutes(5));
                    return fallbackKey;
                }

                // Cache the cipher key
                _cache.Set(cacheKey, apiRequest.CipherKey, _cacheExpiration);

                _logger.LogDebug("Retrieved cipher key from database for hospital {HospitalId}", hospitalId);
                return apiRequest.CipherKey;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cipher key for hospital {HospitalId}", hospitalId);

                // Fallback to configuration in case of database error
                var fallbackKey = _configuration["PhilHealth:CipherKey"] ?? "PHilheaLthDuMmy311630";
                return fallbackKey;
            }
        }

        public async Task<(string HospitalCode, string CipherKey)> GetHospitalCredentialsAsync(string hospitalId)
        {
            if (string.IsNullOrWhiteSpace(hospitalId))
            {
                throw new ArgumentException("Hospital ID cannot be null or empty", nameof(hospitalId));
            }

            // Try to get from cache first
            var cacheKey = $"hospital_credentials_{hospitalId}";
            if (_cache.TryGetValue(cacheKey, out (string HospitalCode, string CipherKey)? cachedCredentials) &&
                cachedCredentials.HasValue)
            {
                _logger.LogDebug("Retrieved hospital credentials from cache for hospital {HospitalId}", hospitalId);
                return cachedCredentials.Value;
            }

            try
            {
                // Query database for both hospital code and cipher key
                var apiRequest = await _dbContext.APIRequests
                    .Where(ar => ar.HospitalId == hospitalId && ar.IsActive)
                    .Select(ar => new { ar.HospitalCode, ar.CipherKey })
                    .FirstOrDefaultAsync();

                var hospitalCode = apiRequest?.HospitalCode ?? _configuration["PhilHealth:DefaultHospitalCode"] ?? "311630";
                var cipherKey = apiRequest?.CipherKey ?? _configuration["PhilHealth:CipherKey"] ?? "PHilheaLthDuMmy311630";

                var credentials = (hospitalCode, cipherKey);

                // Cache the credentials
                _cache.Set(cacheKey, credentials, _cacheExpiration);

                _logger.LogDebug("Retrieved hospital credentials from database for hospital {HospitalId}", hospitalId);
                return credentials;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hospital credentials for hospital {HospitalId}", hospitalId);

                // Fallback to configuration in case of database error
                var fallbackCode = _configuration["PhilHealth:DefaultHospitalCode"] ?? "311630";
                var fallbackKey = _configuration["PhilHealth:CipherKey"] ?? "PHilheaLthDuMmy311630";
                return (fallbackCode, fallbackKey);
            }
        }

        public async Task<bool> UpdateHospitalCodeAsync(string hospitalId, string newHospitalCode)
        {
            if (string.IsNullOrWhiteSpace(hospitalId))
            {
                throw new ArgumentException("Hospital ID cannot be null or empty", nameof(hospitalId));
            }

            if (string.IsNullOrWhiteSpace(newHospitalCode))
            {
                throw new ArgumentException("Hospital code cannot be null or empty", nameof(newHospitalCode));
            }

            try
            {
                var apiRequests = await _dbContext.APIRequests
                    .Where(ar => ar.HospitalId == hospitalId)
                    .ToListAsync();

                if (!apiRequests.Any())
                {
                    _logger.LogWarning("No API requests found for hospital {HospitalId}", hospitalId);
                    return false;
                }

                // Update all API requests for this hospital
                foreach (var apiRequest in apiRequests)
                {
                    apiRequest.HospitalCode = newHospitalCode;
                }

                await _dbContext.SaveChangesAsync();

                // Invalidate cache
                var hospitalCodeCacheKey = $"hospital_code_{hospitalId}";
                var credentialsCacheKey = $"hospital_credentials_{hospitalId}";
                _cache.Remove(hospitalCodeCacheKey);
                _cache.Remove(credentialsCacheKey);

                _logger.LogInformation("Updated hospital code for hospital {HospitalId} to {HospitalCode}", hospitalId, newHospitalCode);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating hospital code for hospital {HospitalId}", hospitalId);
                return false;
            }
        }

        public async Task<Dictionary<string, string>> GetAllHospitalCodesAsync()
        {
            try
            {
                var hospitalCodes = await _dbContext.APIRequests
                    .Where(ar => ar.IsActive)
                    .GroupBy(ar => ar.HospitalId)
                    .Select(g => new { HospitalId = g.Key, HospitalCode = g.First().HospitalCode })
                    .ToDictionaryAsync(x => x.HospitalId, x => x.HospitalCode);

                _logger.LogDebug("Retrieved {Count} hospital codes from database", hospitalCodes.Count);
                return hospitalCodes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all hospital codes");
                return new Dictionary<string, string>();
            }
        }
    }
}