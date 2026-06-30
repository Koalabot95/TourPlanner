using System;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.DTOs;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/logs")]
    public class TourLogController : ControllerBase
    {
        private readonly ITourLogService _logService;

        public TourLogController(ITourLogService logService)
        {
            _logService = logService;
        }

        [HttpGet("tour/{tourId}")]
        public async Task<IActionResult> GetLogsForTour(Guid tourId)
        {
            var userId = GetUserId();
            var result = await _logService.GetLogsForTourAsync(tourId, userId);

            if (result.StatusCode != 200)
                return StatusCode(result.StatusCode, new { field = result.ErrorField, message = result.ErrorMessage });

            return Ok(result.Logs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLogById(Guid id)
        {
            var userId = GetUserId();
            var result = await _logService.GetLogByIdAsync(id, userId);

            if (result.StatusCode != 200)
                return StatusCode(result.StatusCode, new { field = result.ErrorField, message = result.ErrorMessage });

            return Ok(result.Log);
        }

        [HttpPost("tour/{tourId}")]
        public async Task<IActionResult> CreateLog(Guid tourId, [FromBody] TourLogCreateUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _logService.CreateLogAsync(tourId, dto, userId);

            if (result.StatusCode != 201)
                return StatusCode(result.StatusCode, new { field = result.ErrorField, message = result.ErrorMessage });

            return StatusCode(201, result.Log);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLog(Guid id, [FromBody] TourLogCreateUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _logService.UpdateLogAsync(id, dto, userId);

            if (result.StatusCode != 200)
                return StatusCode(result.StatusCode, new { field = result.ErrorField, message = result.ErrorMessage });

            return StatusCode(201, result.Log);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(Guid id)
        {
            var userId = GetUserId();
            var result = await _logService.DeleteLogAsync(id, userId);

            if (result.StatusCode != 204)
                return StatusCode(result.StatusCode, new { field = result.ErrorField, message = result.ErrorMessage });

            return NoContent();
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            var userId = GetUserId();
            var result = await _logService.GetAllLogsAsync(userId);
            return Ok(result.Logs);
        }
    }
}