using Microsoft.AspNetCore.Mvc;
using MYA.Models.Common;
using MYA.Models.Mvs;
using MYA.Models.Puzzle;
using System.Text.Json;

namespace MYA.Api.Controllers.Puzzle
{
    public sealed partial class MvsController
    {
        [HttpGet("GetMvsCartelle")]
        [ProducesResponseType(typeof(ApiResponse<CartelleResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<CartelleResponse>>> GetMvsCartelle(
            string soc, CancellationToken cancellationToken)
        {
            try
            {
                string responseContent = await _mvsService.GetCartelleAsync(soc);
                CartelleResponse? response =
                    JsonSerializer.Deserialize<CartelleResponse>(responseContent);
                if (response is not null)
                {
                    foreach (Cartella item in response.Data)
                    {
                        item.Soc = soc.ToUpper();
                    }
                }
                return Ok(new
                {
                    Response = "OK",
                    Data = response,
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
