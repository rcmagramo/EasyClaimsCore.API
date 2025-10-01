namespace EasyClaimsCore.API.Models.Requests
{
    public class DoctorPANRestRequest : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string lastname { get; set; } = string.Empty;
        public string firstname { get; set; } = string.Empty;
        public string middlename { get; set; } = string.Empty;
        public string suffix { get; set; } = string.Empty;
        public string birthdate { get; set; } = string.Empty;
    }

    public class DocPANRequest
    {
        public string lastname { get; set; } = string.Empty;
        public string firstname { get; set; } = string.Empty;
        public string middlename { get; set; } = string.Empty;
        public string suffix { get; set; } = string.Empty;
        public string birthdate { get; set; } = string.Empty;
    }
}