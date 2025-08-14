namespace EasyClaimsCore.API.Models.Requests
{
    public class DoctorAccreditationRestViewModel : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string accrecode { get; set; } = string.Empty;
        public string admissiondate { get; set; } = string.Empty;
        public string dischargedate { get; set; } = string.Empty;
    }

    public class DoctorAccreditationVM
    {
        public string accrecode { get; set; } = string.Empty;
        public string admissiondate { get; set; } = string.Empty;
        public string dischargedate { get; set; } = string.Empty;
    }
}