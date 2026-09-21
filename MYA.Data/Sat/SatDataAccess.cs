using Microsoft.Data.SqlClient;
using MYA.Data.Common;
using MYA.Models.Auth;

namespace MYA.Data.Sat;

public sealed class SatDataAccess
{
    private readonly SqlExecutor _sql;

    public SatDataAccess(SqlExecutor sql)
    {
        _sql = sql;
    }

    #region Mfa

    public Task EnqueueMfaSmsAsync(MfaSmsMessage message, CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.NChar("@PhoneNumber", message.PhoneNumber, 20),
            SqlParameterFactory.VarChar("@Message", message.Text, 160)
        ];

        return _sql.ExecuteAsync(DatabaseTarget.Sat, "dbo.sp_My_EnqueueMfaSms", parameters, cancellationToken);
    }

    #endregion
}
