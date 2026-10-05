using Microsoft.AspNetCore.Mvc;
using MYA.Business.Mvs;

namespace MYA.Api.Controllers.Puzzle;

[ApiController]
[Route("api/[controller]")]
public sealed partial class MvsController : ControllerBase
{
    private readonly MvsService _mvsService;

    public MvsController(MvsService mvsService)
    {
        _mvsService = mvsService;
    }
}
