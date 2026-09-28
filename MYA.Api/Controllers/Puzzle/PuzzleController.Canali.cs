using Microsoft.AspNetCore.Mvc;
using MYA.Models.Common;
using MYA.Models.Puzzle;

namespace MYA.Api.Controllers.Puzzle;

public sealed partial class PuzzleController
{
    [HttpGet("GetCanaliByNPeriferica")]
    [ProducesResponseType(typeof(ApiResponse<List<Pz_Canale>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<Pz_Canale>>>> GetCanaliByNPeriferica(
        int idPeriferica, string soc, CancellationToken cancellationToken)
    {
        try
        {
            List<Pz_Canale> canali = await _puzzleService
                .GetCanaliByNPeriferica(idPeriferica, soc, cancellationToken)
                .ConfigureAwait(false);

            return Ok(new
            {
                Response = "OK",
                Data = canali,
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