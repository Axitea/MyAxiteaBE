using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MYA.Business.Clienti;
using MYA.Models.Common;

namespace MYA.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/MyClienti")]
public sealed class MyClientiController : ControllerBase
{
    private readonly ClientiService _clientiService;

    public MyClientiController(ClientiService clientiService)
    {
        _clientiService = clientiService;
    }

    [HttpGet("GetAllContrattiByCliente")]
    [ProducesResponseType(typeof(ApiResponse<List<InfoCliente>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<InfoCliente>>>> GetAllContratti(
        string CodCliente,
        CancellationToken cancellationToken)
    {
        try
        {
            List<InfoCliente> result = await _clientiService
                .GetAllContrattiClienteAsync(CodCliente, cancellationToken)
                .ConfigureAwait(false);

            return Ok(new
            {
                Response = "OK",
                Data = result,
                Message = "successo"
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                Response = "KO",
                Message = "Errore:" + ex.Message
            });
        }
    }

    [HttpGet("GetAllClienti")]
    [ProducesResponseType(typeof(ApiResponse<List<InfoCliente>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<InfoCliente>>>> GetAllClienti(CancellationToken cancellationToken)
    {
        try
        {
            List<InfoCliente> result = await _clientiService
                .GetClientiAsync(cancellationToken)
                .ConfigureAwait(false);

            return Ok(new
            {
                Response = "OK",
                Data = result,
                Message = "successo"
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                Response = "KO",
                Message = "Errore:" + ex.Message
            });
        }
    }
}
