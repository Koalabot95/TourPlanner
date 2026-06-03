using Microsoft.AspNetCore.Http;
using backend.Models; 
using backend.DTOs;  

namespace backend.Interfaces
{
	public interface IImageService
	{
		Task<Image> SaveImageAsync(ImageUploadDto dto);
		byte[] GetImage(string fileName);
		void DeleteImage(string fileName);
	}
}