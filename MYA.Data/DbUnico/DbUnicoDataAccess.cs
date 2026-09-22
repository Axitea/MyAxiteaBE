using Microsoft.Data.SqlClient;
using MYA.Data.Common;
using MYA.Models.Common;

namespace MYA.Data.DbUnico;

public sealed class DbUnicoDataAccess
{
    private readonly SqlExecutor _sql;

    public DbUnicoDataAccess(SqlExecutor sql)
    {
        _sql = sql;
    }

    #region Clienti

    public async Task<List<InfoCliente>> GetContrattiByClienteAsync(
        string codiceCliente,
        CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters =
        [
            SqlParameterFactory.VarChar("@CodCliente", codiceCliente, 100)
        ];

        var rows = await _sql.QueryAsyncNoSequential(
                DatabaseTarget.DbUnico,
                "dbo.sp_Get_CordinateCliente_byCodCliente",
                parameters,
                MapPosizioneContrattiCliente,
                cancellationToken)
            .ConfigureAwait(false);

        var clientiPerIndirizzo = new Dictionary<string, InfoCliente>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in rows)
        {
            var key = $"{item.Provincia}|{item.Indirizzo}";
            if (!clientiPerIndirizzo.TryGetValue(key, out var existing))
            {
                foreach (var servizio in SplitServizi(item.TipologiaServizio))
                {
                    item.Servizio.Add(new Tipologia { Nome = servizio });
                }

                clientiPerIndirizzo[key] = item;
                continue;
            }

            var serviceNames = new HashSet<string>(
                existing.Servizio.Select(x => x.Nome),
                StringComparer.OrdinalIgnoreCase);

            foreach (var servizio in SplitServizi(item.TipologiaServizio))
            {
                if (serviceNames.Add(servizio))
                {
                    existing.Servizio.Add(new Tipologia { Nome = servizio });
                }
            }
        }

        return clientiPerIndirizzo.Values.ToList();
    }

    public async Task<List<InfoCliente>> GetAllClientiAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _sql.QueryAsyncNoSequential(
                DatabaseTarget.DbUnico,
                "dbo.sp_GetALL_MY_ViewDBUnico",
                Array.Empty<SqlParameter>(),
                MapPosizioneCliente,
                cancellationToken)
            .ConfigureAwait(false);

        return rows.ToList();
    }

    #endregion

    private static IEnumerable<string> SplitServizi(string tipologiaServizio)
    {
        return tipologiaServizio
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(servizio => servizio.Trim());
    }

    private static InfoCliente MapPosizioneContrattiCliente(SqlDataReader reader)
    {
        return new InfoCliente
        {
            Id = reader.GetNullableInt32("Id"),
            Contratto = reader.GetStringOrEmpty("Contratto"),
            Posizione = reader.GetNullableInt32("Posizione"),
            Longitudine = reader.GetNullableDouble("Longitudine"),
            Latitudine = reader.GetNullableDouble("Latitudine"),
            Provincia = reader.GetTrimmedStringOrEmpty("ProvinciaServizio"),
            Indirizzo = reader.GetStringOrEmpty("STRAS"),
            TipologiaServizio = reader.GetStringOrEmpty("TipologiaServizio")
        };
    }

    private static InfoCliente MapPosizioneCliente(SqlDataReader reader)
    {
        return new InfoCliente
        {
            NomeCliente = reader.GetTrimmedStringOrEmpty("NAME1"),
            CodiceCliente = reader.GetTrimmedStringOrEmpty("CODCLIENTE")
        };
    }
}
