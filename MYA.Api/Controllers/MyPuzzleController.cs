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

        [HttpGet("GetPerifericheByIdSito")]
        [ProducesResponseType(typeof(ApiResponse<List<Periferica>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<Periferica>>>> GetPerifericheByIdSito(int idSito, string soc,
            CancellationToken cancellationToken)
        {
            // Recupero periferiche...
            List<Periferica> periferiche = await _puzzleService
                .GetPerifericheByIdSito(idSito, soc, cancellationToken)
                .ConfigureAwait(false);

            return Ok(new
            {
                Response = "OK",
                Data = periferiche,
                Message = "successo"
            });
        }

        [HttpGet("GetCanaliByNPeriferica")]
        public IActionResult GetCanaliByNPeriferica(int nPeriferica, string soc)
        {
            // Recupero i Canali di una periferiche...

            return Ok();
        }

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
       
    }
}
