using System.Security.Claims;
using backend.DTOs;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tours")]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;

        public TourController(ITourService tourService)
        {
            _tourService = tourService;
        }

        // GET: /api/tours
        // Holt alle Tours des authentifizierten Users
        [HttpGet]
        public async Task<IActionResult> GetAllTours()
        {
            var userId = GetUserId();
            var result = await _tourService.GetAllToursAsync(userId);

            if (result.StatusCode != 200)
                return StatusCode(result.StatusCode, new { field = result.ErrorField, message = result.ErrorMessage });

            return Ok(result.Tours);
        }

        // GET: /api/tours/{id}
        // Holt eine spezifische Tour nach ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTourById(Guid id)
        {
            var userId = GetUserId();
            var result = await _tourService.GetTourByIdAsync(id, userId);

            if (result.StatusCode != 200)
                return StatusCode(result.StatusCode, new { field = result.ErrorField, message = result.ErrorMessage });

            return Ok(result.Tour);
        }

        // POST: /api/tours
        // Erstellt eine neue Tour
        [HttpPost]
        public async Task<IActionResult> CreateTour([FromBody] TourDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _tourService.CreateTourAsync(dto, userId);

            if (result.StatusCode != 201)
                return StatusCode(result.StatusCode, new { field = result.ErrorField, message = result.ErrorMessage });

            return StatusCode(201, result.Tour);
        }

        // PUT: /api/tours/{id}
        // Aktualisiert eine existierende Tour
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTour(Guid id, [FromBody] TourDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _tourService.UpdateTourAsync(id, dto, userId);

            if (result.StatusCode != 200)
                return StatusCode(result.StatusCode, new { field = result.ErrorField, message = result.ErrorMessage });

            return StatusCode(201, result.Tour);
        }

        // DELETE: /api/tours/{id}
        // Loescht eine Tour mit allen zugehorigen TourLogs und Images
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTour(Guid id)
        {
            var userId = GetUserId();
            var result = await _tourService.DeleteTourAsync(id, userId);

            if (result.StatusCode != 204)
                return StatusCode(result.StatusCode, new { field = result.ErrorField, message = result.ErrorMessage });

            return NoContent();
        }

        // PRIVATE HELPER 
        // Extrahiert die User-ID aus dem JWT Token
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}