namespace MYA.Models.Puzzle
{
    public class StoricoEventiSite_VW
    {
        public List<EventoSito_VW> data { get; set; }
    }

    public class EventoSito_VW
    {
        public int id { get; set; }
        public string dataEv { get; set; }        // "2025-09-14 00:11:53"
        public int idcolor { get; set; }        // 7
        public string descr { get; set; }       // "ALLARME GENERALE"
        public string state { get; set; }       // "*ALLARME"
        public long idtx { get; set; }          // 407027327137
        public string code_periferica { get; set; }          // Oggetto device
        public string code_item { get; set; }   // canale o zona dell'evento
        public List<FullMsg> fullMsgs { get; set; }
        public string descrPuzzle
        {
            get
            {
                if (!string.IsNullOrEmpty(state))
                    return descr + " " + state;
                else
                    return descr;
            }
        }       // "ALLARME GENERALE *ALLARME"
        public long idevesicep { get; set; }
    }

    public class FullMsg
    {
        public string proto { get; set; }       // "CONTACTID"
        public string code { get; set; }        // "E602"
        public string descr { get; set; }       // "Prova automatica"
    }
}
