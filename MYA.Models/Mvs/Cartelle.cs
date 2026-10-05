
using System.Text.Json.Serialization;

namespace MYA.Models.Mvs
{
    public sealed class CartelleResponse
    {
        [JsonPropertyName("data")]
        public List<Cartella> Data { get; set; } = [];

        [JsonPropertyName("result")]
        public string? Result { get; set; }
    }

    /// <summary>
    /// Entità del JSON.
    /// </summary>
    public sealed class Cartella
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("descr")]
        public string Descr { get; set; } = string.Empty;

        [JsonPropertyName("soc")]
        public string? Soc { get; set; }
    }
}
