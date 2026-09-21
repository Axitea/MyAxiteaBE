using MYA.Models.MyCliente;

namespace MYA.Data.MyClienteRepo;

public interface IMyClienteRepository
{
    Task<List<InfoCliente>?> GetLocazioneContrattiClienteAsync( string CodCliente, CancellationToken cancellationToken = default);
    Task<List<InfoCliente>?> GetClienteAsync(CancellationToken cancellationToken = default);

}
