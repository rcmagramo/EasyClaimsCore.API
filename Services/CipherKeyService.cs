using EasyClaimsCore.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EasyClaimsCore.API.Services
{
    public interface ICipherKeyService
    {
        Task<string> GetCipherKeyAsync(string hospitalId);
        Task<bool> UpdateCipherKeyAsync(string hospitalId, string newCipherKey);
        Task<Dictionary<string, string>> GetAllCipherKeysAsync();
    }

    public class CipherKeyService : ICipherKeyService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CipherKeyService> _logger;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        public CipherKeyService(
            ApplicationDbContext dbContext,
            IMemoryCache cache,
            ILogger<CipherKeyService> logger,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
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

        public async Task<bool> UpdateCipherKeyAsync(string hospitalId, string newCipherKey)
        {
            if (string.IsNullOrWhiteSpace(hospitalId))
            {
                throw new ArgumentException("Hospital ID cannot be null or empty", nameof(hospitalId));
            }

            if (string.IsNullOrWhiteSpace(newCipherKey))
            {
                throw new ArgumentException("Cipher key cannot be null or empty", nameof(newCipherKey));
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
                    apiRequest.CipherKey = newCipherKey;
                }

                await _dbContext.SaveChangesAsync();

                // Invalidate cache
                var cacheKey = $"cipher_key_{hospitalId}";
                _cache.Remove(cacheKey);

                _logger.LogInformation("Updated cipher key for hospital {HospitalId}", hospitalId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cipher key for hospital {HospitalId}", hospitalId);
                return false;
            }
        }

        public async Task<Dictionary<string, string>> GetAllCipherKeysAsync()
        {
            try
            {
                var cipherKeys = await _dbContext.APIRequests
                    .Where(ar => ar.IsActive)
                    .GroupBy(ar => ar.HospitalId)
                    .Select(g => new { HospitalId = g.Key, CipherKey = g.First().CipherKey })
                    .ToDictionaryAsync(x => x.HospitalId, x => x.CipherKey);

                _logger.LogDebug("Retrieved {Count} cipher keys from database", cipherKeys.Count);
                return cipherKeys;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all cipher keys");
                return new Dictionary<string, string>();
            }
        }
    }
}