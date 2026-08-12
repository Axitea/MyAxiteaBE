namespace MYA.Models.Configuration;

public sealed class DatabaseOptions
{
    public string PuzzleConnectionString { get; set; } = string.Empty;

    public string SatConnectionString { get; set; } = string.Empty;

    public int CommandTimeoutSeconds { get; set; } = 30;
}
