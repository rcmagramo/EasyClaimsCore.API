using EasyClaimsCore.API.Data.Entities;
using EasyClaimsCore.API.Models.Requests;

namespace EasyClaimsCore.API.Services.Logging
{
    public interface IRequestLogger
    {
        Task<APIRequestLog> LogRequestAsync<TRequest>(RequestName requestName, TRequest request, int chargeableItems = 0);
        Task UpdateResponseAsync(APIRequestLog log, object response, RequestStatus status);
    }

    public enum RequestStatus
    {
        Pending,
        Success,
        Failed
    }
}