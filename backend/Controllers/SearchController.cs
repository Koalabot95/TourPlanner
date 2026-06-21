using backend.DTOs;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[Route("api/tours")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTours(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? transportMode = null,
        [FromQuery] string? startLocation = null,
        [FromQuery] string? endLocation = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not authenticated." });

        if (skip < 0 || take < 1 || take > 100)
            return BadRequest(new { message = "Invalid pagination: skip >= 0, 1 <= take <= 100" });

        var result = await _searchService.SearchToursAsync(
            userId,
            searchTerm,
            transportMode,
            startLocation,
            endLocation,
            skip,
            take
        );

        return Ok(result);
    }
}