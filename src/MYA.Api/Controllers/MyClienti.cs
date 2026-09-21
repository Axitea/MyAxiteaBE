using Microsoft.AspNetCore.Mvc;
using MYA.Business.MyCliente;
using MYA.Models.MyCliente;
using MYA.Models.Auth;

namespace MYA.Api.Controllers;

[ApiController]
[Route("api/MyClienti")]
public sealed class MyClienti : ControllerBase
{
    private readonly IMyCliente _myCliente;

    public MyClienti(IMyCliente myCliente)
    {
        _myCliente = myCliente;
    }

    [HttpGet("GetAllContrattiByCliente")]
    [ProducesResponseType(typeof(ApiResponse<List<InfoCliente>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<InfoCliente>>>> GetAllContratti(string CodCliente, CancellationToken cancellationToken)
    {
        try
        {
            List<InfoCliente> result = await _myCliente.GetAllContrattiCliente(CodCliente, cancellationToken).ConfigureAwait(false);

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
            List<InfoCliente> result = await _myCliente.GetCliente(cancellationToken).ConfigureAwait(false);

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
