using System.Data;
using Microsoft.Data.SqlClient;

namespace MYA.Data.Database;

public sealed class SqlDbExecutor : IDbExecutor
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public SqlDbExecutor(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(
        DatabaseTarget target,
        string storedProcedure,
        IReadOnlyCollection<SqlParameter> parameters,
        Func<SqlDataReader, T> map,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.Create(target);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = CreateCommand(connection, storedProcedure, parameters);
        await using var reader = await command
            .ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken)
            .ConfigureAwait(false);

        var rows = new List<T>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            rows.Add(map(reader));
        }

        return rows;
    }

    public async Task<IReadOnlyList<T>> QueryAsyncNoSequential<T>(
        DatabaseTarget target,
        string storedProcedure,
        IReadOnlyCollection<SqlParameter> parameters,
        Func<SqlDataReader, T> map,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.Create(target);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = CreateCommand(connection, storedProcedure, parameters);
        await using var reader = await command
            .ExecuteReaderAsync( cancellationToken)
            .ConfigureAwait(false);

        var rows = new List<T>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            rows.Add(map(reader));
        }

        return rows;
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(
        DatabaseTarget target,
        string storedProcedure,
        IReadOnlyCollection<SqlParameter> parameters,
        Func<SqlDataReader, T> map,
        CancellationToken cancellationToken = default)
    {
        var rows = await QueryAsync(target, storedProcedure, parameters, map, cancellationToken)
            .ConfigureAwait(false);

        return rows.Count == 0 ? default : rows[0];
    }

    public async Task<int> ExecuteAsync(
        DatabaseTarget target,
        string storedProcedure,
        IReadOnlyCollection<SqlParameter> parameters,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.Create(target);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = CreateCommand(connection, storedProcedure, parameters);
        return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private SqlCommand CreateCommand(
        SqlConnection connection,
        string storedProcedure,
        IReadOnlyCollection<SqlParameter> parameters)
    {
        var command = connection.CreateCommand();
        command.CommandText = storedProcedure;
        command.CommandType = CommandType.StoredProcedure;
        command.CommandTimeout = _connectionFactory.CommandTimeoutSeconds;

        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        return command;
    }
}
