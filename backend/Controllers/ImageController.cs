using System;
using System.IO;
using System.Threading.Tasks;
using backend.DTOs;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize] 
    [ApiController]
    [Route("api/image")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        
        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] Guid? tourId, [FromForm] Guid? logId, [FromForm] string? caption)
        {
            try
            {
                // mappen der Formulardaten aufs ImageUploadDto
                var dto = new ImageUploadDto
                {
                    File = file,
                    TourId = tourId,
                    LogId = logId,
                    Caption = caption
                };

                // Service übernimmt Validierung, Speichern auf HDD + DB-Eintrag
                var savedImage = await _imageService.SaveImageAsync(dto);

                //  URL zum Abrufen des Bildes zurückgeben
                var relativeUrl = $"/api/image/file/{savedImage.FilePath}";
                return Ok(new { imageUrl = relativeUrl });
            }
            catch (ArgumentException ex)
            {
                // Fängt Validierungen (z.B. falsche Extension, fehlende IDs) ab
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Interner Serverfehler beim Upload.", details = ex.Message });
            }
        }

        // Dieser Endpunkt liefert das Bild wieder an das Angular-Frontend aus
        [AllowAnonymous]
        [HttpGet("file/{fileName}")]
        public IActionResult GetImageFile(string fileName)
        {
            try
            {
                // Nutzt GetImage-Methode aus dem Service
                var fileBytes = _imageService.GetImage(fileName);

                // Liefert die Datei mit dem passenden Image-Typ zurück
                return File(fileBytes, "image/jpeg");
            }
            catch (FileNotFoundException)
            {
                return NotFound("Das Bild wurde nicht gefunden.");
            }
            catch (Exception)
            {
                return BadRequest("Fehler beim Laden des Bildes.");
            }
        }
    }
}