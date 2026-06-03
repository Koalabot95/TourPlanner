using Microsoft.AspNetCore.Http;
using backend.Interfaces;
using backend.Helpers;
using backend.Models;
using backend.DTOs; 
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; 

namespace backend.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository; 
        private readonly string _storageDirectory;

        public ImageService(IImageRepository imageRepository, IConfiguration configuration)
        {
            _imageRepository = imageRepository;

            // Holt das Basisverzeichnis aus appsettings.json 
            // Falls dort nichts steht, wird standardmäßig ein Ordner "Uploads" erstellt
            _storageDirectory = configuration["Storage:Directory"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }
        }

        public async Task<Image> SaveImageAsync(ImageUploadDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                throw new ArgumentException("Datei ist leer oder ungültig.");

            // 1. Validierung über den Helper (Schutz vor .exe, etc.)
            if (!ImageHelper.IsValidImageExtension(dto.File.FileName))
                throw new ArgumentException("Ungültiges Dateiformat. Nur Bilder sind erlaubt.");

            // 2. Mindestens eine ID muss da sein (Tour oder Log)
            if (dto.LogId == null && dto.TourId == null)
                throw new ArgumentException("Das Bild muss entweder einer Tour oder einem Tour-Log zugewiesen werden.");

            // 3. Eindeutigen Dateinamen über den Helper generieren
            var uniqueFileName = ImageHelper.GenerateUniqueFileName(dto.File.FileName);
            var fullPath = Path.Combine(_storageDirectory, uniqueFileName);

            // 4. Datei auf die Festplatte schreiben
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            // 5. Neues Image-Modell für die Datenbank erstellen
            var imageEntity = new Image
            {
                FilePath = uniqueFileName,
                Caption = dto.Caption,
                LogId = dto.LogId,
                TourId = dto.TourId,
                UploadedAt = DateTime.UtcNow
            };

            // 6. In die Datenbank speichern 
            var createdImage = await _imageRepository.CreateAsync(imageEntity);

            // 7. Image-Objekt zurückgeben
            return createdImage;
        }

        public byte[] GetImage(string fileName)
        {
            var fullPath = Path.Combine(_storageDirectory, fileName);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Das angeforderte Bild existiert nicht.");

            return File.ReadAllBytes(fullPath);
        }

        public void DeleteImage(string fileName)
        {
            var fullPath = Path.Combine(_storageDirectory, fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}