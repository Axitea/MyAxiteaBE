namespace MYA.Models.Configuration
{
    public sealed class MvsOptions
    {
        public string UrlServerNordJson { get; set; } = string.Empty;

        public string UrlServerCentroJson { get; set; } = string.Empty;

        public int ServerPortJson { get; set; } = 9800;

        public string ApiKeyNord { get; set; } = string.Empty;

        public string ApiKeyCentro { get; set; } = string.Empty;
    }
}
