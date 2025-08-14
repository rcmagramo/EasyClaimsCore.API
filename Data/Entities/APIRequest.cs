namespace EasyClaimsCore.API.Data.Entities
{
    public class APIRequest
    {
        public int Id { get; set; }
        public string HospitalId { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<APIRequestLog> APIRequestLogs { get; set; } = new List<APIRequestLog>();
    }
}