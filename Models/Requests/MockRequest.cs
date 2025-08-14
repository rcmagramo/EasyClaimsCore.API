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
}