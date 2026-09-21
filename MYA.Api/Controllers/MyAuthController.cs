using Microsoft.AspNetCore.Mvc;
using MYA.Business.Auth;
using MYA.Models.Auth;
using MYA.Models.Common;

namespace MYA.Api.Controllers;

[ApiController]
[Route("api/myAuth")]
public sealed class MyAuthController : ControllerBase
{
    private readonly AuthService _authService;

    public MyAuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginStartedResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginStartedResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginStartedResponse>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.StartLoginAsync(request, cancellationToken)
                .ConfigureAwait(false);

            return Ok(ApiResponse<LoginStartedResponse>.Ok(result));
        }
        catch (AuthDomainException exception)
        {
            return Unauthorized(ApiResponse<LoginStartedResponse>.Fail(exception.Message));
        }
    }

    [HttpPost("verifyMfa")]
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<TokenResponse>>> VerifyMfa(
        [FromBody] VerifyMfaRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.VerifyMfaAsync(request, cancellationToken)
                .ConfigureAwait(false);

            return Ok(ApiResponse<TokenResponse>.Ok(result));
        }
        catch (AuthDomainException exception)
        {
            return Unauthorized(ApiResponse<TokenResponse>.Fail(exception.Message));
        }
    }
}
