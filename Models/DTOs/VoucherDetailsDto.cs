using Newtonsoft.Json;

namespace EasyClaimsCore.API.Models.DTOs
{
    public class VoucherDetailsDto
    {
        [JsonProperty("pVoucherNo")]
        public string VoucherNo { get; set; }

        [JsonProperty("pVoucherDate")]
        public string pVoucherDate { get; set; }

        [JsonProperty("CLAIM")]
        public Claim2 Claim { get; set; }

        [JsonProperty("SUMMARY")]
        public Summary Summary { get; set; }
    }

    public sealed class Claim2
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

        [JsonProperty("pIsAdjustment")]
        public string IsAdjustment { get; set; }

        [JsonProperty("CHARGE")]
        public Charge Charge { get; set; }
    }

    public sealed class Charge
    {
        [JsonProperty("pPayeeType")]
        public string PayeeType { get; set; }

        [JsonProperty("pPayeeCode")]
        public string PayeeCode { get; set; }

        [JsonProperty("pPayeeName")]
        public string PayeeName { get; set; }

        [JsonProperty("pRMBD")]
        public string RMBD { get; set; }

        [JsonProperty("pDRUGS")]
        public string DRUGS { get; set; }

        [JsonProperty("pXRAY")]
        public string XRAY { get; set; }

        [JsonProperty("pOPRM")]
        public string OPRM { get; set; }

        [JsonProperty("pSPFee")]
        public string SPFee { get; set; }

        [JsonProperty("pGPFee")]
        public string GPFee { get; set; }

        [JsonProperty("pSURFee")]
        public string SURFee { get; set; }

        [JsonProperty("pANESFee")]
        public string ANESFee { get; set; }

        [JsonIgnore]
        private decimal grossAmount;
        [JsonProperty("pGrossAmount")]
        public string GrossAmount
        {
            get => grossAmount.ToString("0.#0");
            set
            {
                if (decimal.TryParse(value, out var amount))
                    grossAmount = amount;
            }
        }

        [JsonIgnore]
        private decimal taxAmount;
        [JsonProperty("pTaxAmount")]
        public string TaxAmount
        {
            get => taxAmount.ToString("0.#0");
            set
            {
                if (decimal.TryParse(value, out var amount))
                    taxAmount = amount;
            }
        }

        [JsonIgnore]
        private decimal netAmount;
        [JsonProperty("pNetAmount")]
        public string NetAmount
        {
            get => netAmount.ToString("0.#0");
            set
            {
                if (decimal.TryParse(value, out var amount))
                    netAmount = amount;
            }
        }

    }

    public sealed class Summary
    {
        [JsonIgnore]
        private decimal totalAmount;
        [JsonProperty("pTotalAmount")]
        public string TotalAmount
        {
            get => totalAmount.ToString("0.#0");
            set
            {
                if (decimal.TryParse(value, out var amount))
                    totalAmount = amount;
            }
        }

        [JsonProperty("pNumberOfClaims")]
        public string NumberOfClaims { get; set; }

        [JsonProperty("PAYEE")]
        public Payee Payee { get; set; }

    }

    public sealed class Payee
    {
        [JsonProperty("pPayeeType")]
        public string PayeeType { get; set; }

        [JsonProperty("pPayeeCode")]
        public string PayeeCode { get; set; }

        [JsonProperty("pPayeeName")]
        public string PayeeName { get; set; }

        [JsonProperty("pRMBD")]
        public string RMBD { get; set; }

        [JsonProperty("pDRUGS")]
        public string DRUGS { get; set; }

        [JsonProperty("pXRAY")]
        public string XRAY { get; set; }

        [JsonProperty("pOPRM")]
        public string OPRM { get; set; }

        [JsonProperty("pSPFee")]
        public string SPFee { get; set; }

        [JsonProperty("pGPFee")]
        public string GPFee { get; set; }

        [JsonProperty("pSURFee")]
        public string SURFee { get; set; }

        [JsonProperty("pANESFee")]
        public string ANESFee { get; set; }

        [JsonIgnore]
        private decimal grossAmount;
        [JsonProperty("pGrossAmount")]
        public string GrossAmount
        {
            get => grossAmount.ToString("0.#0");
            set
            {
                if (decimal.TryParse(value, out var amount))
                    grossAmount = amount;
            }
        }

        [JsonIgnore]
        private decimal taxAmount;
        [JsonProperty("pTaxAmount")]
        public string TaxAmount
        {
            get => taxAmount.ToString("0.#0");
            set
            {
                if (decimal.TryParse(value, out var amount))
                    taxAmount = amount;
            }
        }

        [JsonIgnore]
        private decimal netAmount;
        [JsonProperty("pNetAmount")]
        public string NetAmount
        {
            get => netAmount.ToString("0.#0");
            set
            {
                if (decimal.TryParse(value, out var amount))
                    netAmount = amount;
            }
        }

        [JsonProperty("pCheckNo")]
        public string CheckNo { get; set; }

        [JsonProperty("pCheckDate")]
        public string CheckDate { get; set; }
    }
}
