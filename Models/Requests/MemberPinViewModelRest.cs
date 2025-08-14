namespace EasyClaimsCore.API.Models.Requests
{
    public class MemberPinViewModelRest : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string? certificateId { get; set; } = string.Empty;
        public string lastname { get; set; } = string.Empty;
        public string firstname { get; set; } = string.Empty;
        public string middlename { get; set; } = string.Empty;
        public string? suffix { get; set; } = string.Empty;
        public string birthdate { get; set; } = string.Empty;
    }

    public class MemberPin
    {
        public string lastname { get; set; } = string.Empty;
        public string firstname { get; set; } = string.Empty;
        public string middlename { get; set; } = string.Empty;
        public string suffix { get; set; } = string.Empty;
        public string birthdate { get; set; } = string.Empty;
    }
}