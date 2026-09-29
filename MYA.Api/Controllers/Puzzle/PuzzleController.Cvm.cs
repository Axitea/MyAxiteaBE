using Microsoft.AspNetCore.Mvc;
using MYA.Models.Common;
using MYA.Models.Puzzle;

namespace MYA.Api.Controllers.Puzzle;

public sealed partial class PuzzleController
{
    [HttpGet("GetTelecamereByCodePerif")]
    [ProducesResponseType(typeof(ApiResponse<List<CVM_Device>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<CVM_Device>>>> GetTelecamereByCodePerif(
        string codePerif, string soc, CancellationToken cancellationToken)
    {
        try
        {
            List<CVM_Device> telecamere = await _puzzleService
                .GetTelecamereByCodePerif(codePerif, soc, cancellationToken)
                .ConfigureAwait(false);

            return Ok(new
            {
                Response = "OK",
                Data = telecamere,
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
