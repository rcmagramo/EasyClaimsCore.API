using Newtonsoft.Json;
using System.Xml.Serialization;

namespace EasyClaimsCore.API.Models.DTOs
{
    public class IsClaimEligibleDto
    {
        [JsonProperty("isok")]
        public string IsOk { get; set; }
        [JsonProperty("trackingno")]
        public string TrackingNo { get; set; }
        [JsonProperty("referenceno")]
        public string ReferenceNo { get; set; }
        [JsonProperty("asof")]
        public string AsOf { get; set; }
    }
}
