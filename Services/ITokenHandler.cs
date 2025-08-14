namespace EasyClaimsCore.API.Services
{
    public interface ITokenHandler
    {
        Task<string> MakeApiRequestAsync(string pmcc, string certificateId);
    }
}