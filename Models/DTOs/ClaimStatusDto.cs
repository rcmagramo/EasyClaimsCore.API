using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EasyClaimsCore.API.Models.DTOs
{
    public sealed class ClaimStatusDto
    {
        [JsonProperty("pAsOf")]
        public string AsOf { get; set; }

        [JsonProperty("pAsOfTime")]
        public string AsOfTime { get; set; }

        [JsonProperty("CLAIM")]
        public Claim Claim { get; set; } 
    }

    public sealed class Claim
    {
        [JsonProperty("pClaimSeriesLhio")]
        public string ClaimSeriesLhio { get; set; }

        [JsonProperty("pPin")]
        public string Pin { get; set; }

        [JsonProperty("pPatientLastName")]
        public string PatientLastName { get; set; }

        [JsonProperty("pPatientFirstName")]
        public string PatientFirstName { get; set; }

        [JsonProperty("pPatientMiddleName")]
        public string PatientMiddleName { get; set; }

        [JsonProperty("pPatientSuffix")]
        public string PatientSuffix { get; set; }

        [JsonProperty("pAdmissionDate")]
        public string AdmissionDate { get; set; }

        [JsonProperty("pDischargeDate")]
        public string DischargeDate { get; set; }

        [JsonProperty("pClaimDateReceived")]
        public string ClaimDateReceived { get; set; }

        [JsonProperty("pClaimDateRefile")]
        public string ClaimDateRefile { get; set; }

        [JsonProperty("pStatus")]
        public string Status { get; set; }

        [JsonProperty("RETURN")]
        public Return Return { get; set; }

        [JsonProperty("DENIED")]
        public List<ReasonDetails> Reasons { get; set; }

        [JsonProperty("PAYMENT")]
        public List<Payment> Payment { get; set; }

        [JsonProperty("PROCESS")]
        public List<Process> Process { get; set; }
    }

    public class Return
    {
        [JsonProperty("DEFECTS")]
        public List<Defects> Defects { get; set; }
    }

    public class Defects
    {
        [JsonProperty("pDeficiency")]
        public string Deficiency { get; set; }
    }

    public sealed class ReasonDetails
    {
        [JsonProperty("pReason")]
        public string Reason { get; set; }
    }

    public sealed class Payment
    {
        [JsonIgnore]
        private decimal totalClaimAmountPaid;

        [JsonProperty("pTotalClaimAmountPaid")]
        public string TotalClaimAmountPaid
        {
            get => totalClaimAmountPaid.ToString("0.#0");
            set
            {
                if (decimal.TryParse(value, out var amount))
                    totalClaimAmountPaid = amount;
            }
        }
    }

    public sealed class Process
    {
        [JsonProperty("pProcessStage")]
        public string ProcessStage { get; set; }

        [JsonProperty("pProcessDate")]
        public string ProcessDate { get; set; }
    }
}
