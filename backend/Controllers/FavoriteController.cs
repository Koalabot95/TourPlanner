using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[Route("api/tours")]
[Authorize]
public class FavoriteController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoriteController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpPost("{tourId}/favorite")]
    public async Task<IActionResult> AddFavorite(Guid tourId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not authenticated." });

        var (success, errorMessage, statusCode) = await _favoriteService.AddFavoriteAsync(tourId, userId);

        if (!success)
            return StatusCode(statusCode, new { message = errorMessage });

        return StatusCode(statusCode, new { message = "Tour added to favorites" });
    }

    [HttpDelete("{tourId}/favorite")]
    public async Task<IActionResult> RemoveFavorite(Guid tourId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not authenticated." });

        var (success, errorMessage, statusCode) = await _favoriteService.RemoveFavoriteAsync(tourId, userId);

        if (!success)
            return StatusCode(statusCode, new { message = errorMessage });

        return NoContent();
    }

    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavoriteTours()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not authenticated." });

        var (tours, errorMessage, statusCode) = await _favoriteService.GetFavoritesToursAsync(userId);

        if (tours == null)
            return StatusCode(statusCode, new { message = errorMessage });

        return Ok(tours);
    }

    [HttpGet("{tourId}/is-favorite")]
    public async Task<IActionResult> IsFavoriteTour(Guid tourId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not authenticated." });

        var (isFavorite, statusCode) = await _favoriteService.IsFavoriteTourAsync(tourId, userId);

        return Ok(new { isFavorite });
    }
}