using Microsoft.Data.SqlClient;

namespace MYA.Data.Database;

public interface ISqlConnectionFactory
{
    int CommandTimeoutSeconds { get; }

    SqlConnection Create(DatabaseTarget target);
}
