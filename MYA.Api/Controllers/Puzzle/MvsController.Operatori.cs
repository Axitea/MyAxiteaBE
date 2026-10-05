using Microsoft.AspNetCore.Mvc;
using MYA.Models.Common;
using MYA.Models.Mvs;
using System.Text.Json;

namespace MYA.Api.Controllers.Puzzle
{
    public sealed partial class MvsController
    {
        [HttpGet("GetMvsOperatori")]
        [ProducesResponseType(typeof(ApiResponse<OperatorsResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<OperatorsResponse>>> GetMvsOperatori(
            string soc, CancellationToken cancellationToken)
        {
            try
            {
                string responseContent = await _mvsService.GetOperatoriAsync(soc);
                OperatorsResponse? response =
                    JsonSerializer.Deserialize<OperatorsResponse>(responseContent);
                if (response is not null)
                {
                    foreach (Operator item in response.Data)
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
