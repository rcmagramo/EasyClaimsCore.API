namespace EasyClaimsCore.API.Models.Requests
{
    public class ClaimStatusApiRequest : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string serieslhionos { get; set; } = string.Empty;
    }

    public class ClaimStatusLhionos
    {
        public string serieslhionos { get; set; } = string.Empty;
    }
}