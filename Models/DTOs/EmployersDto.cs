using Newtonsoft.Json;

namespace EasyClaimsCore.API.Models.DTOs
{
    public class EmployersDto
    {
        [JsonProperty("asof")]
        public string AsOf { get; set; }

        [JsonProperty("employers")]
        public List<Employer> Employers { get; set; }
    }

    public class Employer
    {
        [JsonProperty("philhealthno")]
        public string PhilHealthNo { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
