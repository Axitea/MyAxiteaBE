using System;
using System.Collections.Generic;
using System.Text;

namespace MYA.Models.Puzzle
{
    public class Pz_Periferica
    {
        public int Id_Periferica { get; set; }

        public string n_Periferica { get; set; }

        public string Code { get; set; }

        public int Id_Produttore { get; set; }

        public int id_sito_monitoraggio { get; set; }

        public DateTime last_rec { get; set; }

        public DateTime last_msg_time { get; set; }

        public bool disabilitata { get; set; }

        public string Soc { get; set; }

        public string Modello { get; set; }

        // Sezione Collaudo TO
        public DateTime? DataInizioCollaudo { get; set; }

        public DateTime? DataFineCollaudo { get; set; }

        public int UltimaOra { get; set; }
        public int Ultime24Ore { get; set; }
        public int UltimaSettimana { get; set; }
        public int UltimoMese { get; set; }
    }
}
