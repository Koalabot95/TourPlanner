using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[Route("api/tours")]
[Authorize]
public class ImportExportController : ControllerBase
{
    private readonly IImportExportService _importExportService;

    public ImportExportController(IImportExportService importExportService)
    {
        _importExportService = importExportService;
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportTours()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not authenticated." });

        var jsonContent = await _importExportService.ExportToursAsJsonAsync(userId);

        var bytes = System.Text.Encoding.UTF8.GetBytes(jsonContent);
        return File(bytes, "application/json", $"tours_export_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.json");
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportTours([FromBody] ImportDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not authenticated." });

        if (string.IsNullOrEmpty(dto.JsonContent))
            return BadRequest(new { message = "JSON content is required." });

        var (success, errorMessage, importedTours) = await _importExportService.ImportToursFromJsonAsync(userId, dto.JsonContent);

        if (!success)
            return BadRequest(new { message = errorMessage });

        return Ok(new { totalCount = importedTours?.Count ?? 0, tours = importedTours });
    }
}

public class ImportDto
{
    public string? JsonContent { get; set; }
}