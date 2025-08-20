using Newtonsoft.Json;

public class EConfirmationDto
{
    [JsonProperty("mapping")]
    public List<Mapping> Mapping { get; set; }

    [JsonProperty("preceiptTicketNumber")]
    public string ReceiptTicketNumber { get; set; }

    [JsonProperty("phospitalCode")]
    public string HospitalCode { get; set; }

    [JsonProperty("phospitalTransmittalNo")]
    public string HospitalTransmittalNo { get; set; }

    [JsonProperty("ptotalClaims")]
    public int TotalClaims { get; set; }

    [JsonProperty("preceivedDate")]
    public string ReceivedDate { get; set; }
}

public class Mapping
{
    [JsonProperty("pclaimNumber")]
    public string ClaimNumber { get; set; }

    [JsonProperty("ppatientLastName")]
    public string PatientLastName { get; set; }

    [JsonProperty("ppatientFirstName")]
    public string PatientFirstName { get; set; }

    [JsonProperty("ppatientMiddleName")]
    public string PatientMiddleName { get; set; }

    [JsonProperty("ppatientSuffix")]
    public string PatientSuffix { get; set; }

    [JsonProperty("padmissionDate")]
    public string AdmissionDate { get; set; }

    [JsonProperty("pdischargeDate")]
    public string DischargeDate { get; set; }

    [JsonProperty("pclaimSeriesLhio")]
    public string ClaimSeriesLhio { get; set; }
}
