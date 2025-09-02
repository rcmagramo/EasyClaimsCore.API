using EasyClaimsCore.API.Data.Entities;

namespace EasyClaimsCore.API.Services
{
    public interface IAPIRequestSuccessLogService
    {
        Task<APIRequestSuccessLog> LogSuccessfulEClaimsUploadAsync(APIRequestLog originalLog);
        Task<int> GetCurrentBillAmountAsync();
        Task UpdateBillAmountAsync(int newPrice);
    }
}