using Microsoft.Data.SqlClient;
using MYA.Data.Common;
using MYA.Models.Auth;
using MYA.Models.Common;
using MYA.Models.Puzzle;

namespace MYA.Data.Puzzle;

public sealed class PuzzleDataAccess
{
    private readonly SqlExecutor _sql;

    public PuzzleDataAccess(SqlExecutor sql)
    {
        _sql = sql;
    }

    #region Authentication

    public Task<LoginUser?> GetLoginAsync(
        string codiceCliente,
        string login,
        string encryptedPassword,
        int appId,
        CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.VarChar("@CodCliente", codiceCliente, 50),
            SqlParameterFactory.VarChar("@Login", login, 255),
            SqlParameterFactory.NVarChar("@Password", encryptedPassword, 255),
            SqlParameterFactory.Int("@IdApp", appId)
        ];

        return _sql.QuerySingleOrDefaultAsync(
            DatabaseTarget.Puzzle,
            "dbo.sp_My_GetLogin",
            parameters,
            MapLoginUser,
            cancellationToken);
    }

    public Task SaveMfaCodeAsync(
        long userId,
        int appId,
        string code,
        DateTimeOffset requestedAt,
        CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.BigInt("@IdUtente", userId),
            SqlParameterFactory.Int("@IdApp", appId),
            SqlParameterFactory.NVarChar("@Code", code, 6),
            SqlParameterFactory.DateTime("@RequestedAtUtc", requestedAt)
        ];

        return _sql.ExecuteAsync(DatabaseTarget.Puzzle, "dbo.sp_My_SetMfaCode", parameters, cancellationToken);
    }

    public Task<LoginUser?> VerifyMfaCodeAsync(
        long userId,
        int appId,
        string code,
        DateTimeOffset now,
        int ttlMinutes,
        CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.BigInt("@IdUtente", userId),
            SqlParameterFactory.Int("@IdApp", appId),
            SqlParameterFactory.NVarChar("@Code", code, 6),
            SqlParameterFactory.DateTime("@NowUtc", now),
            SqlParameterFactory.Int("@TtlMinutes", ttlMinutes)
        ];

        return _sql.QuerySingleOrDefaultAsync(
            DatabaseTarget.Puzzle,
            "dbo.sp_My_VerifyMfaCode",
            parameters,
            MapLoginUser,
            cancellationToken);
    }

    #endregion

    #region Tokens

    public Task SaveIssuedTokenAsync(JwtTokenWrite token, CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.NVarCharMax("@TokenValue", token.TokenValue),
            SqlParameterFactory.NVarChar("@Username", token.Username, 30),
            SqlParameterFactory.Int("@Permission", 0),
            SqlParameterFactory.NVarChar("@RefreshToken", token.RefreshToken, 200),
            SqlParameterFactory.Int("@IdApp", token.AppId)
        ];

        return _sql.ExecuteAsync(DatabaseTarget.Puzzle, "dbo.sp_My_InsertJWTokenValue", parameters, cancellationToken);
    }

    public Task<JwtTokenRecord?> GetTokenByRefreshTokenAsync(
        string refreshToken,
        int appId,
        CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.NVarChar("@RefreshToken", refreshToken, 200),
            SqlParameterFactory.Int("@IdApp", appId)
        ];

        return _sql.QuerySingleOrDefaultAsync(
            DatabaseTarget.Puzzle,
            "dbo.sp_My_GetJWTokenValue",
            parameters,
            MapToken,
            cancellationToken);
    }

    #endregion

    #region Puzzle

    #region Periferiche

    public async Task<List<Periferica>> GetPerifericheByIdSito(int idSito, string soc, 
        CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.Int("@IdSito", idSito),
            SqlParameterFactory.NChar("@SOC", soc, 2),
        ];

        var rows = await _sql.QueryAsyncNoSequential(
                DatabaseTarget.Puzzle,
                "dbo.sp_Get_PZ_PerifericaByIdSito_New",
                parameters,
                MapPeriferica,
                cancellationToken)
            .ConfigureAwait(false);

        return rows.ToList();
    }

    /*
    public async DataTable SP_GetPerifericaByNPeriferica(int nPeriferica, string SOCRif)
    {
        try
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@IdPerifericaSicep", nPeriferica.ToString()));
            parameters.Add(new SqlParameter("@SOC", SOCRif));

            var rows = await _sql.QueryAsyncNoSequential(
                DatabaseTarget.Puzzle,
                "dbo.sp_Get_PZ_PerifericaByIdSito_New",
                parameters,
                MapPeriferica,
                cancellationToken)
            .ConfigureAwait(false);

            // Eseguo la SP di get
            DataTable dt = sqlServHlp.ExecuteStoredProcedure("dbo.sp_GetPerifericaSicepById", parameters.ToArray());
            return dt;
        }
        catch (Exception exc)
        {
            // this.SP_InsertLogSP(MethodBase.GetCurrentMethod().Name, exc.Message);
            throw;
        }
    }
    */

    #endregion Periferiche

    #endregion Puzzle

    private Periferica MapPeriferica(SqlDataReader dr)
    {
        /*
        return new Periferica
        {
            Id_Periferica = dr.GetInt32Required("Id_Periferica"),
            n_Periferica = dr.GetStringRequired("n_Periferica"),
            Code = dr.GetStringRequired("Code"),
            Id_Produttore = dr.GetInt32Required("Id_Produttore"),
            id_sito_monitoraggio = dr.GetInt32Required("id_sito_monitoraggio"),
            last_rec = dr.GetDateTimeRequired("last_rec"),
            last_msg_time = dr.GetDateTimeRequired("last_msg_time"),
            disabilitata = dr.GetBooleanRequired("disabilitata"),
            Soc = dr.GetStringRequired("SOC"),
            Modello = HasColumn(dr, "Modello") ? dr.GetNullableString("Modello") : null,
            DataInizioCollaudo = HasColumn(dr, "DataInizioCollaudo") ? dr.GetNullableDateTime("DataInizioCollaudo") : null,
            DataFineCollaudo = HasColumn(dr, "DataFineCollaudo") ? dr.GetNullableDateTime("DataFineCollaudo") : null,
            UltimaOra = HasColumn(dr, "UltimaOra") ? dr.GetNullableInt32("UltimaOra") ?? 0 : 0,
            Ultime24Ore = HasColumn(dr, "Ultime24Ore") ? dr.GetNullableInt32("Ultime24Ore") ?? 0 : 0,
            UltimaSettimana = HasColumn(dr, "UltimaSettimana") ? dr.GetNullableInt32("UltimaSettimana") ?? 0 : 0,
            UltimoMese = HasColumn(dr, "UltimoMese") ? dr.GetNullableInt32("UltimoMese") ?? 0 : 0
        };
        */

        Periferica periferica = new Periferica();

        if (dr["Id_Periferica"] != DBNull.Value)
            periferica.Id_Periferica = Convert.ToInt32(dr["Id_Periferica"]);

        if (dr["n_Periferica"] != DBNull.Value)
            periferica.n_Periferica = dr["n_Periferica"].ToString();

        if (dr["Code"] != DBNull.Value)
            periferica.Code = dr["Code"].ToString();

        if (dr["Id_Produttore"] != DBNull.Value)
            periferica.Id_Produttore = Convert.ToInt32(dr["Id_Produttore"]);

        if (dr["id_sito_monitoraggio"] != DBNull.Value)
            periferica.id_sito_monitoraggio = Convert.ToInt32(dr["id_sito_monitoraggio"]);

        if (dr["last_rec"] != DBNull.Value)
            periferica.last_rec = Convert.ToDateTime(dr["last_rec"]);

        if (dr["last_msg_time"] != DBNull.Value)
            periferica.last_msg_time = Convert.ToDateTime(dr["last_msg_time"]);

        if (dr["disabilitata"] != DBNull.Value)
            periferica.disabilitata = Convert.ToBoolean(dr["disabilitata"]);

        if (dr["SOC"] != DBNull.Value)
            periferica.Soc = dr["SOC"].ToString();

        if (HasColumn(dr, "Modello") && dr["Modello"] != DBNull.Value)
            periferica.Modello = dr["Modello"].ToString();

        if (HasColumn(dr, "DataInizioCollaudo") && dr["DataInizioCollaudo"] != DBNull.Value)
            periferica.DataInizioCollaudo = Convert.ToDateTime(dr["DataInizioCollaudo"]);

        if (HasColumn(dr, "DataFineCollaudo") && dr["DataFineCollaudo"] != DBNull.Value)
            periferica.DataFineCollaudo = Convert.ToDateTime(dr["DataFineCollaudo"]);

        if (HasColumn(dr, "UltimaOra") && dr["UltimaOra"] != DBNull.Value)
            periferica.UltimaOra = Convert.ToInt32(dr["UltimaOra"]);

        if (HasColumn(dr, "Ultime24Ore") && dr["Ultime24Ore"] != DBNull.Value)
            periferica.Ultime24Ore = Convert.ToInt32(dr["Ultime24Ore"]);

        if (HasColumn(dr, "UltimaSettimana") && dr["UltimaSettimana"] != DBNull.Value)
            periferica.UltimaSettimana = Convert.ToInt32(dr["UltimaSettimana"]);

        if (HasColumn(dr, "UltimoMese") && dr["UltimoMese"] != DBNull.Value)
            periferica.UltimoMese = Convert.ToInt32(dr["UltimoMese"]);

        return periferica;
    }

    private bool HasColumn(SqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (string.Equals(
                reader.GetName(i),
                columnName,
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static LoginUser MapLoginUser(SqlDataReader reader)
    {
        return new LoginUser(
            UserIdToken: reader.GetInt64Required("Wus_IdUtenteToken"),
            UserId: reader.GetInt64Required("Wus_IdUtente"),
            Login: reader.GetNullableString("Wus_Login") ?? string.Empty,
            DisplayName: reader.GetNullableString("Wus_Nome") ?? string.Empty,
            Permission: reader.GetNullableInt32("Wus_Permessi") ?? 0,
            Email: reader.GetNullableString("Wus_Email"),
            CustomerName: reader.GetNullableString("Wus_CliRagSociale"),
            ScenarioId: reader.GetNullableInt32("Wus_IdScenario"),
            Language: reader.GetNullableString("Wus_Lingua"),
            PhoneNumber: reader.GetNullableString("Wus_2FA_NumCell"),
            IsMfaActive: string.Equals(reader.GetNullableString("Wus_2FA_IsActive")?.Trim(), "S", StringComparison.OrdinalIgnoreCase),
            IsMfaLocked: reader.GetNullableBoolean("Wus_2FA_Locked") ?? false);
    }

    private static JwtTokenRecord MapToken(SqlDataReader reader)
    {
        return new JwtTokenRecord(
            IdToken: reader.GetInt32Required("idToken"),
            TokenValue: reader.GetNullableString("tokenValue") ?? string.Empty,
            Username: reader.GetNullableString("username") ?? string.Empty,
            DateToken: reader.GetDateTimeRequired("dateToken"),
            Permission: reader.GetNullableInt32("permission") ?? 0,
            RefreshToken: reader.GetNullableString("refreshToken") ?? string.Empty,
            IdApp: reader.GetNullableInt32("idApp"));
    }
}
