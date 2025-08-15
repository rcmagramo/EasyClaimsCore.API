using Newtonsoft.Json;

namespace EasyClaimsCore.API.Models.DTOs
{
    public class EAccreditationDto
    {
        [JsonProperty("isaccredited")]
        public string IsAccredited { get; set; }

        [JsonProperty("accrecode")]
        public string DoctorAccreCode { get; set; }

        [JsonProperty("admissiondate")]
        public string AdmissionDate { get; set; }

        [JsonProperty("dischargedate")]
        public string DischargeDate { get; set; }

        [JsonProperty("accreditationstart")]
        public string AccreditationStart { get; set; }

        [JsonProperty("accreditationend")]
        public string AccreditationEnd { get; set; }
    }
}
