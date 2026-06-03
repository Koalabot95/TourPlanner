public class ImageUploadDto
{
	public IFormFile File { get; set; } = null!;
	public string? Caption { get; set; }
	public Guid? LogId { get; set; }   // Optional 
	public Guid? TourId { get; set; }  // Optional
}