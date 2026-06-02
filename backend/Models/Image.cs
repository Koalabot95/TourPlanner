using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

public class Image
{
    public Guid ImageId { get; set; }
    public Guid LogId { get; set; }
    public string FilePath { get; set; } = null!;
    public string? Caption { get; set; }
    public DateTime UploadedAt { get; set; }
}