namespace EasyClaimsCore.API.Data.Entities
{
    public class APIRequestLog
    {
        public int Id { get; set; }
        public int APIRequestId { get; set; }
        public int ChargeableItems { get; set; }
        public string RequestData { get; set; } = string.Empty;
        public string? Response { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Requested { get; set; }
        public DateTime? Responded { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? Created { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? Updated { get; set; }

        // Navigation property
        public APIRequest? APIRequest { get; set; }
    }
}