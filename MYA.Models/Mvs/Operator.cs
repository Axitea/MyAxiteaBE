using System.Text.Json.Serialization;

namespace MYA.Models.Mvs
{
    public class OperatorsResponse
    {
        [JsonPropertyName("data")]
        public List<Operator> Data { get; set; } = [];

        public string Result { get; set; }
    }

    public class Operator
    {
        [JsonPropertyName("codope")]
        public int Codope { get; set; }
        [JsonPropertyName("nick")]
        public string Nick { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        public string Soc { get; set; }
        [JsonPropertyName("dutyType")]
        public string DutyType { get; set; }
        [JsonPropertyName("group")]
        public int Group { get; set; }
    }
}
