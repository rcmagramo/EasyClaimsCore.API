namespace EasyClaimsCore.API.Models.Requests
{
    public class TokenCredentialsRequest : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
    }
}