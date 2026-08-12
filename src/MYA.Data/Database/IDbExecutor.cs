using Microsoft.Data.SqlClient;

namespace MYA.Data.Database;

public interface IDbExecutor
{
    Task<IReadOnlyList<T>> QueryAsync<T>(
        DatabaseTarget target,
        string storedProcedure,
        IReadOnlyCollection<SqlParameter> parameters,
        Func<SqlDataReader, T> map,
        CancellationToken cancellationToken = default);

    Task<T?> QuerySingleOrDefaultAsync<T>(
        DatabaseTarget target,
        string storedProcedure,
        IReadOnlyCollection<SqlParameter> parameters,
        Func<SqlDataReader, T> map,
        CancellationToken cancellationToken = default);

    Task<int> ExecuteAsync(
        DatabaseTarget target,
        string storedProcedure,
        IReadOnlyCollection<SqlParameter> parameters,
        CancellationToken cancellationToken = default);
}
