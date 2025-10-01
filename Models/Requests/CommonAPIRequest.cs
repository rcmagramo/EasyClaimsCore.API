namespace EasyClaimsCore.API.Models.Requests
{
    public class CommonAPIRequest : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string Xml { get; set; } = string.Empty;
    }

    public class CommonRequest
    {
        public string Xml { get; set; } = string.Empty;
    }
}