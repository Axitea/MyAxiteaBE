using System;
using System.Collections.Generic;
using System.Text;

namespace MYA.Models.Puzzle
{
    public class Pz_Canale
    {
        public int Id_Canale { get; set; }

        public int Id_Periferica { get; set; }

        public int Id_Sito_Monitoraggio { get; set; }

        public string Codice { get; set; }

        public bool BadState { get; set; }

        public string Fascia { get; set; }

        public string Descr { get; set; }

        public string StatoOn { get; set; }

        public string StatoOff { get; set; }

        public string UltimoStato { get; set; }

        public string UltimoColore { get; set; }

        public DateTime UltimoRX { get; set; }

        public int Id_TipoCanale { get; set; }

        public string Soc { get; set; }

        public int Id_TipoComunicazione { get; set; }

        public string RcItem { get; set; }

        public int RcHours { get; set; }

        public int Disabilitato { get; set; }

        public string Code_Periferica { get; set; }

        public string Modello { get; set; }

        public string ArrayTlc { get; set; }

        // Sezione Collaudo TO
        public bool Collaudato { get; set; }

    }
}
