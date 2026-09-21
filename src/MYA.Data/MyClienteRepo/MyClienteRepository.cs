using Microsoft.Data.SqlClient;
using MYA.Data.Database;
using MYA.Models.MyCliente;
using System.Data;
using System.Globalization;

namespace MYA.Data.MyClienteRepo;

public sealed class MyClienteRepository : IMyClienteRepository
{
    private readonly IDbExecutor _db;

    public MyClienteRepository(IDbExecutor db)
    {
        _db = db;
    }
    private static double ParseDouble(string? s) => double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var v) ? v : 0.0;

    public async Task<List<InfoCliente>?> GetLocazioneContrattiClienteAsync(string CodCliente, CancellationToken cancellationToken = default)
    {
        SqlParameter[] parameters = new[]
        {
            SqlParameterFactory.VarChar("@CodCliente", CodCliente, 100),
        };

        var rows = await _db.QueryAsyncNoSequential(DatabaseTarget.DBUNICO, "dbo.sp_Get_CordinateCliente_byCodCliente", parameters, MapPosizioneContrattiCliente, cancellationToken);

        var Lista = new Dictionary<string, InfoCliente>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in rows ?? Enumerable.Empty<InfoCliente>())
        {
            var key = $"{item.Provincia}|{item.Indirizzo}";
            // check se la chiave esiste già nel dizionario
            if (!Lista.TryGetValue(key, out var existing))
            {
                var services = (item.TipologiaServizio ?? string.Empty)
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => new Tipologia { Nome = s.Trim() });

                foreach (var s in services)
                {
                    item.Servizio.Add(s);
                }

                Lista[key] = item;
                continue;
            }
            // Se la chiave esiste già, unisci le tipologie di servizio
            var existingNames = new HashSet<string>(existing.Servizio.Select(x => x.Nome), StringComparer.OrdinalIgnoreCase);
            // Aggiungi le nuove tipologie di servizio solo se non esistono già
            var newNames = (item.TipologiaServizio ?? string.Empty)
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim());

            // Aggiungi le nuove tipologie di servizio solo se non esistono già
            foreach (var n in newNames)
            {
                if (existingNames.Add(n))
                {
                    existing.Servizio.Add(new Tipologia { Nome = n });
                }
            }


        }

        return Lista.Values.ToList();
    }

    private static InfoCliente MapPosizioneContrattiCliente(SqlDataReader reader)
    {
        return new InfoCliente
        {
            Id = reader.GetInt32Required("Id"),
            Contratto = reader.GetNullableString("Contratto") ?? string.Empty,
            Posizione = reader.GetInt32Required("Posizione"),
            Longitudine = ParseDouble(reader.GetNullableString("Longitudine")), 
            Latitudine = ParseDouble(reader.GetNullableString("Latitudine")),
            Provincia = reader.GetString("ProvinciaServizio").Trim(),
            Indirizzo = reader.GetNullableString("STRAS") ?? string.Empty,
            TipologiaServizio = reader.GetNullableString("TipologiaServizio") ?? string.Empty
        };
    }


    public async Task<List<InfoCliente>?> GetClienteAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _db.QueryAsyncNoSequential(DatabaseTarget.DBUNICO, "dbo.sp_GetALL_MY_ViewDBUnico", Array.Empty<SqlParameter>(), MapPosizioneCliente, cancellationToken);

        return rows?.ToList();
    }

    private static InfoCliente MapPosizioneCliente(SqlDataReader reader)
    {
        return new InfoCliente
        {
            NomeCliente = reader.GetString("NAME1").Trim(),
            CodiceCliente = reader.GetString("CODCLIENTE").Trim()
        };
    }

}
