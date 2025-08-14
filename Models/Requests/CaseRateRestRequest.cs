namespace EasyClaimsCore.API.Models.Requests
{
    public class CaseRateRestRequest : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string icdcode { get; set; } = string.Empty;
        public string rvscode { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string targetdate { get; set; } = string.Empty;
    }
}