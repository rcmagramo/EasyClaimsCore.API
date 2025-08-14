namespace EasyClaimsCore.API.Models.Requests
{
    public class PBEFRequest : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string referenceNumber { get; set; } = string.Empty;
    }
}