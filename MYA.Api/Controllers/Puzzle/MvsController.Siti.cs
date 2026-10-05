using Microsoft.AspNetCore.Mvc;
using MYA.Business.Mvs;
using MYA.Business.Puzzle;
using MYA.Models.Common;
using MYA.Models.Puzzle;

namespace MYA.Api.Controllers.Puzzle
{
    public sealed partial class MvsController
    {
        [HttpGet("GetMvsSitesId")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<string>>> GetMvsSitesId(
            string soc, CancellationToken cancellationToken)
        {
            try
            {
                string responseContent = await _mvsService.GetSitesIdAsync(soc);

                return Ok(new
                {
                    Response = "OK",
                    Data = responseContent,
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
}
