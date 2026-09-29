
namespace MYA.Models.Puzzle
{
    public class Pz_Sito
    {
        public string ID_UTE { get; set; }

        public string CONTRATTO { get; set; }

        public string CODICEAMM { get; set; }

        public string GRUPPO { get; set; }

        public string NOME { get; set; }

        public string INDIRIZZO { get; set; }

        public string LOCALITA { get; set; }

        public string CITTA { get; set; }

        public string PROV { get; set; }

        public string CAP { get; set; }

        public string COMPETENZA { get; set; }

        public string SF0 { get; set; }

        public string SF1 { get; set; }

        public string SF2 { get; set; }

        public string SF3 { get; set; }

        public string SF4 { get; set; }

        public string SF5 { get; set; }

        public string SF6 { get; set; }

        public string SF7 { get; set; }

        public string SF8 { get; set; }

        public string SF9 { get; set; }

        public string RIF_DISTR { get; set; }

        public string SOC { get; set; }

        public string LAT { get; set; }

        public string LON { get; set; }

        public decimal Latitudine { get; set; }

        public decimal Longitudine { get; set; }

        public List<Pz_Persona> Recapiti { get; set; }

        public bool? IsMatched { get; set; }

        public bool? Disabilitato { get; set; }

        public int? ScoreCommerciale { get; set; }

        public bool? Prioritario { get; set; }

        public bool? SpedisciMailPerModificaDati { get; set; }

        public int Id_SezioneAlbero { get; set; }

        public bool IsAivvDisabilitato { get; set; }
    }
}
