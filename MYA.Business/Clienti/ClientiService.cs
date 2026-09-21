using MYA.Data.DbUnico;
using MYA.Models.Common;

namespace MYA.Business.Clienti;

public sealed class ClientiService
{
    private readonly DbUnicoDataAccess _dbUnicoDataAccess;

    public ClientiService(DbUnicoDataAccess dbUnicoDataAccess)
    {
        _dbUnicoDataAccess = dbUnicoDataAccess;
    }

    public Task<List<InfoCliente>> GetAllContrattiClienteAsync(
        string codiceCliente,
        CancellationToken cancellationToken = default)
    {
        return _dbUnicoDataAccess.GetContrattiByClienteAsync(codiceCliente, cancellationToken);
    }

    public Task<List<InfoCliente>> GetClientiAsync(CancellationToken cancellationToken = default)
    {
        return _dbUnicoDataAccess.GetAllClientiAsync(cancellationToken);
    }
}
