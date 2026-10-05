using Microsoft.AspNetCore.Mvc;
using MYA.Business.Puzzle;

namespace MYA.Api.Controllers.Puzzle;

[ApiController]
[Route("api/[controller]")]
public sealed partial class EventController : ControllerBase
{
    private readonly EventService _eventService;

    public EventController(EventService eventService)
    {
        _eventService = eventService;
    }
}
