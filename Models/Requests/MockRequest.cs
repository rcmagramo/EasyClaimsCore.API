namespace EasyClaimsCore.API.Models.Requests
{
    public class MockRequest : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string Refiled { get; set; } = string.Empty;
        public string Processed { get; set; } = string.Empty;
        public string ClaimSeriesLHIO { get; set; } = string.Empty;
        public string Xml { get; set; } = string.Empty;
    }

    public class MockDecryptedRequest : IBaseRequest
    {
        public string docMimeType { get; set; } = string.Empty;
        public string hash { get; set; } = string.Empty;
        public string key1 { get; set; } = string.Empty;
        public string key2 { get; set; } = string.Empty;
        public string iv { get; set; } = string.Empty;
        public string doc { get; set; } = string.Empty;
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
    }

    public class MockDecryptRequest 
    {
        public string docMimeType { get; set; } = string.Empty;
        public string hash { get; set; } = string.Empty;
        public string key1 { get; set; } = string.Empty;
        public string key2 { get; set; } = string.Empty;
        public string iv { get; set; } = string.Empty;
        public string doc { get; set; } = string.Empty;
    }
}