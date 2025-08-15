using Newtonsoft.Json;

namespace EasyClaimsCore.API.Models.DTOs
{
    public class ECaseRatesDto
    {
        [JsonProperty("caserates")]
        public List<CaseRateDto> CaseRates { get; set; }
    }

    public class CaseRateDto
    {
        [JsonProperty("pcaseRateCode")]
        public string CaseRateCode { get; set; }

        [JsonProperty("pcaseRateDescription")]
        public string CaseRateDescription { get; set; }

        [JsonProperty("pitemCode")]
        public string ItemCode { get; set; }

        [JsonProperty("pitemDescription")]
        public string ItemDescription { get; set; }

        [JsonProperty("peffectivityDate")]
        public string EffectivityDate { get; set; }

        [JsonProperty("amount")]
        public List<AmountDto> Amounts { get; set; }
    }

    public class AmountDto
    {
        [JsonProperty("pprimaryHCIFee")]
        public string PrimaryHCIFee { get; set; }

        [JsonProperty("pprimaryProfFee")]
        public string PrimaryProfFee { get; set; }

        [JsonProperty("pprimaryCaseRate")]
        public string PrimaryCaseRate { get; set; }

        [JsonProperty("psecondaryHCIFee")]
        public string SecondaryHCIFee { get; set; }

        [JsonProperty("psecondaryProfFee")]
        public string SecondaryProfFee { get; set; }

        [JsonProperty("psecondaryCaseRate")]
        public string SecondaryCaseRate { get; set; }

        [JsonProperty("pcheckFacilityH1")]
        public string CheckFacilityH1 { get; set; }

        [JsonProperty("pcheckFacilityH2")]
        public string CheckFacilityH2 { get; set; }

        [JsonProperty("pcheckFacilityH3")]
        public string CheckFacilityH3 { get; set; }

        [JsonProperty("pcheckFacilityASC")]
        public string CheckFacilityASC { get; set; }

        [JsonProperty("pcheckFacilityPCF")]
        public string CheckFacilityPCF { get; set; }

        [JsonProperty("pcheckFacilityMAT")]
        public string CheckFacilityMAT { get; set; }

        [JsonProperty("pcheckFacilityFSDC")]
        public string CheckFacilityFSDC { get; set; }
    }
}
