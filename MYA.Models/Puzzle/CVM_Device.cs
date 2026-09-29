namespace MYA.Models.Puzzle
{
    public class CVM_Device
    {
        public int Id { get; set; }

        public int Cvm_Id { get; set; }

        public int Zona_Id { get; set; }

        public int? Padre_Id { get; set; }

        public string Periferica { get; set; }

        public string Nome { get; set; }

        public string Soc { get; set; }

        public string Modello { get; set; }
    }
}
