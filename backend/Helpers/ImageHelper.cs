using System;
using System.IO;
using System.Linq;

namespace backend.Helpers
{
    public static class ImageHelper
    {
        // Liste der erlaubten Bild-Endungen
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public static bool IsValidImageExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return AllowedExtensions.Contains(extension);
        }

        public static string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName).ToLower();
            // Erzeugt einen absolut eindeutigen Namen
            return $"{Guid.NewGuid()}{extension}";
        }
    }
}