using Microsoft.AspNetCore.Mvc;
using MYA.Models.Common;
using MYA.Models.Puzzle;

namespace MYA.Api.Controllers.Puzzle;

public sealed partial class PuzzleController
{
    [HttpGet("GetRecapitiByIdSito")]
    [ProducesResponseType(
        typeof(ApiResponse<List<Pz_Persona>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<Pz_Persona>>>> GetRecapitiByIdSito(
        int idSito, string soc, CancellationToken cancellationToken)
    {
        try
        {
            List<Pz_Persona> recapiti = await _puzzleService
                .GetRecapitiByIdSito(idSito, soc, cancellationToken)
                .ConfigureAwait(false);

            return Ok(new
            {
                Response = "OK",
                Data = recapiti,
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