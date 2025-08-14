namespace EasyClaimsCore.API.Security.Cryptography.DataContracts
{
    public class Payload
    {
        public string docMimeType { get; set; } = string.Empty;
        public string hash { get; set; } = string.Empty;
        public string key1 { get; set; } = string.Empty;
        public string key2 { get; set; } = string.Empty;
        public string iv { get; set; } = string.Empty;
        public string doc { get; set; } = string.Empty;
    }
}