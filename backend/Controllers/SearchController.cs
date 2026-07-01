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
        [FromQuery] int? minPopularity = null,
        [FromQuery] int? maxPopularity = null,
        [FromQuery] double? minChildFriendliness = null,
        [FromQuery] double? maxChildFriendliness = null,
        [FromQuery] bool isFavoritesOnly = false)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not authenticated." });

        var result = await _searchService.SearchToursAsync(
            userId,
            searchTerm,
            transportMode,
            startLocation,
            endLocation,
            minPopularity,
            maxPopularity,
            minChildFriendliness,
            maxChildFriendliness
        );

        // Filter favorites if requested
        if (isFavoritesOnly && result.Tours != null)
        {
            result.Tours = result.Tours.Where(t => t.IsFavorite).ToList();
            result.TotalCount = result.Tours.Count;
        }

        return Ok(result);
    }
}