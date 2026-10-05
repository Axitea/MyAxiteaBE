using System;
using System.Collections.Generic;
using System.Text;

namespace MYA.Models.Puzzle
{
    public class Evento
    {
        public int Id_Evento { get; set; }

        public Int64 Id_EventoSicep { get; set; }

        public int? Id_Periferica { get; set; }

        public int? Id_Automezzo { get; set; }

        public int? Id_Contratto { get; set; }

        public int? Id_Evento_KFT { get; set; }

        public int? IdColor { get; set; }

        public string Nota { get; set; }

        public DateTime? Data { get; set; }

        public int? RimandaAllarme { get; set; }

        public char? SN_Manuale { get; set; }

        public char? SN_Gestito { get; set; }

        public char? SN_Risolto { get; set; }

        public DateTime? Data_chiusura { get; set; }

        public float? Latitudine { get; set; }

        public float? Longitudine { get; set; }

        public int? Id_UtenteChiusura { get; set; }

        public int? Id_Dipendente { get; set; }

        public int? Id_FonteDati { get; set; }

        public string ida { get; set; }

        public string Assignment { get; set; }

        public string SOC { get; set; }

        public int? Mvs_Id_Cartella { get; set; }

        public string Mvs_Codice_Canale_Ev { get; set; }

        public Int64? Id_idtx { get; set; }

        public bool IsEventoNew { get; set; }

        public bool IsReset { get; set; }

        public bool IsRipristinato { get; set; }
    }

    public class Evento_VW : Evento
    {
        public string Code_Perif { get; set; }

        public int Id_Color { get; set; }

    }
}
