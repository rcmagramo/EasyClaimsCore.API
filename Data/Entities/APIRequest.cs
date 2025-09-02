namespace EasyClaimsCore.API.Data.Entities
{
    public class APIRequest
    {
        public int Id { get; set; }
        public string HospitalId { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string CipherKey { get; set; } = string.Empty; // New property
        public string HospitalCode { get; set; } = string.Empty; // New property


        // Navigation property
        public ICollection<APIRequestLog> APIRequestLogs { get; set; } = new List<APIRequestLog>();
    }
}