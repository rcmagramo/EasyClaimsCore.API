using EasyClaimsCore.API.Models.Requests;
using EasyClaimsCore.API.Models.Responses;

namespace EasyClaimsCore.API.Services
{
    public interface IServiceExecutor
    {
        Task<ApiResponse<object>> ExecuteServiceAsync<TRequest>(
            RequestName requestName,
            TRequest request,
            Func<TRequest, Task<object>> serviceImplementation,
            int chargeableItems = 0) where TRequest : IBaseRequest;
        
    }
}