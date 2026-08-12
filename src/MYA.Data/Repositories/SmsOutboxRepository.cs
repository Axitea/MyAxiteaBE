using Microsoft.Data.SqlClient;
using MYA.Data.Database;
using MYA.Models.Auth;

namespace MYA.Data.Repositories;

public sealed class SmsOutboxRepository : ISmsOutboxRepository
{
    private readonly IDbExecutor _db;

    public SmsOutboxRepository(IDbExecutor db)
    {
        _db = db;
    }

    public async Task EnqueueMfaSmsAsync(MfaSmsMessage message, CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.NChar("@PhoneNumber", message.PhoneNumber, 20),
            SqlParameterFactory.VarChar("@Message", message.Text, 160)
        ];

        await _db.ExecuteAsync(DatabaseTarget.Sat, "dbo.sp_My_EnqueueMfaSms", parameters, cancellationToken)
            .ConfigureAwait(false);
    }
}
