using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MYA.Business.Security;
using MYA.Data.MyClienteRepo;
using MYA.Models.MyCliente;
using MYA.Models.Configuration;

namespace MYA.Business.MyCliente;

public sealed class MyInfoClienti : IMyCliente
{
      
    private readonly IMyClienteRepository _authRepository;
    public MyInfoClienti(IMyClienteRepository authRepository)
    {
        _authRepository = authRepository;
    } 
    public async Task<List<InfoCliente>> GetAllContrattiCliente(string CodCliente, CancellationToken cancellationToken = default)
    {
        var Lista = await _authRepository.GetLocazioneContrattiClienteAsync(CodCliente, cancellationToken);

        return Lista ?? new List<InfoCliente>();
    }

    public async Task<List<InfoCliente>> GetCliente( CancellationToken cancellationToken = default)
    {
        var Lista = await _authRepository.GetClienteAsync(cancellationToken);
        return Lista ?? new List<InfoCliente>();
    }


}
