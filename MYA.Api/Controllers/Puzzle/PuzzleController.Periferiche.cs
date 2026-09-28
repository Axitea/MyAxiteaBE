using Microsoft.AspNetCore.Mvc;
using MYA.Models.Common;
using MYA.Models.Puzzle;

namespace MYA.Api.Controllers.Puzzle;

public sealed partial class PuzzleController
{
    [HttpGet("GetPerifericheByIdSito")]
    [ProducesResponseType(typeof(ApiResponse<List<Pz_Periferica>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<Pz_Periferica>>>> GetPerifericheByIdSito(
        int idSito, string soc, CancellationToken cancellationToken)
    {
        try
        {
            List<Pz_Periferica> periferiche = await _puzzleService
                .GetPerifericheByIdSito(idSito, soc, cancellationToken)
                .ConfigureAwait(false);

            return Ok(new
            {
                Response = "OK",
                Data = periferiche,
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

    /// <summary>
    /// Recupera le periferiche in base all'id device MVS.
    /// </summary>
    /// <param name="nPeriferica">Id device di MVS.</param>
    /// <param name="soc">Codice SOC.</param>
    /// <param name="cancellationToken">Token di cancellazione.</param>
    /// <returns>Lista delle periferiche.</returns>
    [HttpGet("GetPerifericheByNPeriferica")]
    [ProducesResponseType(
        typeof(ApiResponse<List<Pz_Periferica>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<Pz_Periferica>>>> GetPerifericheByNPeriferica(
        string nPeriferica,
        string soc,
        CancellationToken cancellationToken)
    {
        try
        {
            List<Pz_Periferica> periferiche = await _puzzleService
                .GetPerifericheByNPeriferica(nPeriferica, soc, cancellationToken)
                .ConfigureAwait(false);

            return Ok(new
            {
                Response = "OK",
                Data = periferiche,
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

    /// <summary>
    /// Recupera le periferiche in base all'id della periferica Puzzle.
    /// </summary>
    /// <param name="idPeriferica">Id_Periferica del database Puzzle.</param>
    /// <param name="soc">Codice SOC.</param>
    /// <param name="cancellationToken">Token di cancellazione.</param>
    /// <returns>Lista delle periferiche.</returns>
    [HttpGet("GetPerifericheByIdPeriferica")]
    [ProducesResponseType(
        typeof(ApiResponse<List<Pz_Periferica>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<Pz_Periferica>>>> GetPerifericheByIdPeriferica(
        int idPeriferica,
        string soc,
        CancellationToken cancellationToken)
    {
        try
        {
            List<Pz_Periferica> periferiche = await _puzzleService
                .GetPerifericheByIdPeriferica(idPeriferica, soc, cancellationToken)
                .ConfigureAwait(false);

            return Ok(new
            {
                Response = "OK",
                Data = periferiche,
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

    [HttpGet("GetPerifericheByCode")]
    [ProducesResponseType(
        typeof(ApiResponse<List<Pz_Periferica>>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<Pz_Periferica>>>> GetPerifericheByCode(
        string code, string soc, bool? disabilitata, CancellationToken cancellationToken)
    {
        try
        {
            List<Pz_Periferica> periferiche = await _puzzleService
                .GetPerifericheByCode(
                    code,
                    soc,
                    disabilitata,
                    cancellationToken)
                .ConfigureAwait(false);

            return Ok(new
            {
                Response = "OK",
                Data = periferiche,
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