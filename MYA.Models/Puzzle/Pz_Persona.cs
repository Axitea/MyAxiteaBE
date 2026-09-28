using System;
using System.Collections.Generic;
using System.Text;

namespace MYA.Models.Puzzle
{
    public class Pz_Persona
    {
        public int Id_Persona { get; set; }

        public string Cell1 { get; set; }

        public string Cell2 { get; set; }

        public string Note { get; set; }

        public string Persona { get; set; }

        public string Nome { get; set; }

        public string Cognome { get; set; }

        public string NomeSito { get; set; }

        public int Id_sito_monitoraggio { get; set; }

        public int Ordine { get; set; }

        public string Email { get; set; }

        public string Tel_Ufficio { get; set; }

        public string PWD { get; set; }

        public Tipologia_Persona TipologiaPersona { get; set; }

        public int? Id_Calendario { get; set; }
    }

    public class PZ_PersonaCalendario
    {
        public string DataReale { get; set; }

        public string Descrizione { get; set; }
    }

    public class Tipologia_Persona
    {
        public int Id_TipologiaPersona { get; set; }

        public string TipologiaPersonaDesc { get; set; }
    }
}
