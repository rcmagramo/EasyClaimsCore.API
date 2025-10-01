namespace EasyClaimsCore.API.Models.Requests
{
    public class RequiredApiDocumentRequest : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string SeriesLhioNo { get; set; } = string.Empty;
        public string Xml { get; set; } = string.Empty;
    }

    public class RequiredDocumentRequest
    {
        public string SeriesLhioNo { get; set; } = string.Empty;
        public string Xml { get; set; } = string.Empty;
    }
}