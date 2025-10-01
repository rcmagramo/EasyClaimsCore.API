namespace EasyClaimsCore.API.Models.Requests
{
    public class DRGRequest : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string CF5Xml { get; set; } = string.Empty;
        public string eClaimXml { get; set; } = string.Empty;
    }

    public class DRGViewRequest
    {
        public string CF5XmlData { get; set; } = string.Empty;
        public string eClaimXmlData { get; set; } = string.Empty;
    }
}