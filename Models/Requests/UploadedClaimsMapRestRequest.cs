namespace EasyClaimsCore.API.Models.Requests
{
    public class UploadedClaimsMapRestRequest : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string receiptTicketNumber { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
    }

    public class UploadedClaimsMapAPIRequest
    {
        public string receiptTicketNumber { get; set; } = string.Empty;
    }
}