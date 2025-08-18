using Newtonsoft.Json;
using System.Xml.Serialization;

namespace EasyClaimsCore.API.Models.DTOs
{
    public class IsClaimEligibleDto
    {
        [JsonProperty("isok")]
        public string IsOk;
        [JsonProperty("trackingno")]
        public string TrackingNo;
        [JsonProperty("referenceno")]
        public string ReferenceNo;
        [JsonProperty("asof")]
        public string AsOf;
    }
}
