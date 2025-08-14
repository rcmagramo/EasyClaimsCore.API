namespace EasyClaimsCore.API.Models.Requests
{
    public class VoucherRestRequest : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string VoucherNo { get; set; } = string.Empty;
    }

    public class VoucherRequest
    {
        public string VoucherNo { get; set; } = string.Empty;
    }
}