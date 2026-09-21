using System.ComponentModel.DataAnnotations;

namespace MYA.Models.MyCliente;

public sealed class InfoCliente
{
    public int Id { get; init; }
    public string Contratto { get; init; } = string.Empty;
    public int Posizione { get; init; }
    public double Longitudine { get; init; } = 0;
    public double Latitudine { get; init; } = 0;
    public string NomeCliente { get; init; } = string.Empty;
    public string CodiceCliente { get; init; } = string.Empty;
    public string Provincia { get; init; } = string.Empty;
    public string Indirizzo { get; init; } = string.Empty;

    public string TipologiaServizio { get; init; } = string.Empty;

    public List<Tipologia> Servizio { get; init; } = new List<Tipologia>();
}

public sealed class Tipologia
{
    public string Nome { get; init; }
}

