using Microsoft.AspNetCore.Mvc;
using MYA.Business.Puzzle;

namespace MYA.Api.Controllers.Puzzle;

[ApiController]
[Route("api/[controller]")]
public sealed partial class PuzzleController : ControllerBase
{
    private readonly PuzzleService _puzzleService;

    public PuzzleController(PuzzleService puzzleService)
    {
        _puzzleService = puzzleService;
    }
}