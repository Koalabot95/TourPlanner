using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[Route("api/logs")]
[Authorize]
public class TourLogSearchController : ControllerBase
{
    private readonly ITourLogSearchService _logSearchService;

    public TourLogSearchController(ITourLogSearchService logSearchService)
    {
        _logSearchService = logSearchService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchLogs(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? difficulty = null,
        [FromQuery] int? minRating = null,
        [FromQuery] int? maxRating = null,
        [FromQuery] double? minDistance = null,
        [FromQuery] double? maxDistance = null,
        [FromQuery] int? minTime = null,
        [FromQuery] int? maxTime = null)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not authenticated." });

        var result = await _logSearchService.SearchLogsAsync(
            userId,
            searchTerm,
            difficulty,
            minRating,
            maxRating,
            minDistance,
            maxDistance,
            minTime,
            maxTime
        );

        return Ok(result);
    }
}