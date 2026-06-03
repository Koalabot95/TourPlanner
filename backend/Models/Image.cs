using System;

namespace backend.Models
{
    public class Image
    {
        public Guid ImageId { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public DateTime UploadedAt { get; set; }

        // Fremdschlüssel (Foreign Keys)
        // LogId muss optional sein, falls das Bild zu einer Tour gehört
        public Guid? LogId { get; set; }
        public TourLog? TourLog { get; set; } 

        public Guid? TourId { get; set; }
        public Tour? Tour { get; set; }       
    }
}