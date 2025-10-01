namespace EasyClaimsCore.API.Models.Requests
{
    public class TokenRequest : IBaseRequest
    {
        //public string accreditationNo { get; set; } = string.Empty;
        //public string softwareCertificateId { get; set; } = string.Empty;
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
    }
}