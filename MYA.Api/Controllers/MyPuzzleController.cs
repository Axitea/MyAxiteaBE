using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MYA.Business.Puzzle;
using MYA.Models.Common;
using MYA.Models.Puzzle;

namespace MYA.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class MyPuzzleController : ControllerBase
    {
        private readonly PuzzleService _puzzleService;

        public MyPuzzleController(PuzzleService puzzleService)
        {
            _puzzleService = puzzleService;
        }

        #region Periferiche

        [HttpGet("GetPerifericheByIdSito")]
        [ProducesResponseType(typeof(ApiResponse<List<Pz_Periferica>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<Pz_Periferica>>>> GetPerifericheByIdSito(int idSito, string soc,
            CancellationToken cancellationToken)
        {
            try
            {
                // Recupero tutte le periferiche di un determinato sito
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

        [HttpGet("GetPerifericheByNPeriferica")]
        [ProducesResponseType(typeof(ApiResponse<List<Pz_Periferica>>), StatusCodes.Status200OK)]
        /// <summary>
        /// Recupera le periferiche in base al id di Mvs.
        /// </summary>
        /// <param name="nPeriferica">E l'id device di Mvs</param>
        /// <param name="soc">Il codice SOC.</param>
        /// <param name="cancellationToken">Il token di cancellazione.</param>
        /// <returns>Una lista di periferiche.</returns>
        public async Task<ActionResult<ApiResponse<List<Pz_Periferica>>>> GetPerifericheByNPeriferica(string nPeriferica, string soc,
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

        [HttpGet("GetPerifericheByIdPeriferica")]
        [ProducesResponseType(typeof(ApiResponse<List<Pz_Periferica>>), StatusCodes.Status200OK)]
        /// <summary>
        /// Recupera le periferiche in base al id.
        /// </summary>
        /// <param name="idPeriferica">E' l'id_periferica del DB Puzzle</param>
        /// <param name="soc">Il codice SOC.</param>
        /// <param name="cancellationToken">Il token di cancellazione.</param>
        /// <returns>Una lista di periferiche.</returns>
        public async Task<ActionResult<ApiResponse<List<Pz_Periferica>>>> GetPerifericheByIdPeriferica(int  idPeriferica, string soc,
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

        #endregion Periferiche

        #region Canali

        [HttpGet("GetCanaliByNPeriferica")]
        public async Task<ActionResult<ApiResponse<List<Pz_Periferica>>>> GetCanaliByNPeriferica(int idPeriferica, string soc,
            CancellationToken cancellationToken)
        {
            try
            {
                // Recupero i Canali di una periferica
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

        #endregion Canali

        /*
        [HttpGet("GetFasceOrarieByIdSito")]
        public IActionResult GetFasceOrarieByIdSito(int idSito, string soc)
        {
            // Recupero le fasce orarie di un sito...

            return Ok();
        }

        [HttpGet("GetReportAllarmiByIdSito")]
        public IActionResult GetReportAllarmiByIdSito(int idSito, string soc)
        {
            // Recupero i report degli allarmi di un sito (Eventi Passati di Puzzle)...

            return Ok();
        }

        [HttpGet("GetRecapitiByIdSito")]
        public IActionResult GetRecapitiByIdSito(int idSito, string soc)
        {
            // Recupero i recapiti di un sito...

            return Ok();
        }

        [HttpGet("GetTelecamereByNPeriferica")]
        public IActionResult GetTelecamereByNPeriferica(int nPeriferica, string soc)
        {
            // Recupero le telecamere di una periferiche...

            return Ok();
        }

        [HttpGet("GetAllarmiByNPeriferica")]
        public IActionResult GetAllarmiByNPeriferica(int nPeriferica, string soc)
        {
            // Recupero Elenco Report Allarmi della Periferica (Eventi Passati di Puzzle)...

            return Ok();
        }
        */
       
    }
}
