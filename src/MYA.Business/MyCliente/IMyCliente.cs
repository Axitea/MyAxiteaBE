using MYA.Models.MyCliente;

namespace MYA.Business.MyCliente;

public interface IMyCliente
{
    Task<List<InfoCliente>> GetAllContrattiCliente(string CodCliente, CancellationToken cancellationToken = default);
    Task<List<InfoCliente>> GetCliente( CancellationToken cancellationToken = default);
}
