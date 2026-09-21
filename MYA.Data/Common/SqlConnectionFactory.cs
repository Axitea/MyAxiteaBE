using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MYA.Models.Configuration;

namespace MYA.Data.Common;

public sealed class SqlConnectionFactory
{
    private readonly DatabaseOptions _options;

    public SqlConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }

    public int CommandTimeoutSeconds => Math.Max(1, _options.CommandTimeoutSeconds);

    public SqlConnection Create(DatabaseTarget target)
    {
        var connectionString = target switch
        {
            DatabaseTarget.Puzzle => _options.PuzzleConnectionString,
            DatabaseTarget.Sat => _options.SatConnectionString,
            DatabaseTarget.DbUnico => _options.DBUNICOConnectionString,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"Connection string for {target} is not configured.");
        }

        return new SqlConnection(connectionString);
    }
}
