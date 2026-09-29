using Microsoft.AspNetCore.Mvc;
using MYA.Models.Common;
using MYA.Models.Puzzle;

namespace MYA.Api.Controllers.Puzzle;

public sealed partial class PuzzleController
{
    [HttpGet("GetSitoById")]
    [ProducesResponseType(typeof(ApiResponse<Pz_Sito>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<Pz_Sito>>> GetSitoById(
        int idSito, string soc, CancellationToken cancellationToken)
    {
        try
        {
            Pz_Sito sito = await _puzzleService
                .GetSitoById(idSito, soc, cancellationToken)
                .ConfigureAwait(false);

            return Ok(new
            {
                Response = "OK",
                Data = sito,
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
