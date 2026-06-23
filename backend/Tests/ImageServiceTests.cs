using Moq;
using Microsoft.AspNetCore.Http;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using backend.Services;
using Microsoft.Extensions.Configuration;

namespace Tests;

[TestFixture]
public class ImageServiceTests
{
    private Mock<IImageRepository> _mockRepo;
    private Mock<IConfiguration> _mockConfig;
    private IImageService _imageService;
    private string _testStorageDir;

    [SetUp]
    public void SetUp()
    {
        _mockRepo = new Mock<IImageRepository>();
        _mockConfig = new Mock<IConfiguration>();

        _testStorageDir = Path.Combine(Path.GetTempPath(), $"test-images-{Guid.NewGuid()}");
        _mockConfig.Setup(c => c["Storage:Directory"]).Returns(_testStorageDir);

        _imageService = new ImageService(_mockRepo.Object, _mockConfig.Object);

        if (!Directory.Exists(_testStorageDir))
        {
            Directory.CreateDirectory(_testStorageDir);
        }
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testStorageDir))
        {
            Directory.Delete(_testStorageDir, true);
        }
    }

    [Test]
    public async Task SaveImageAsync_ValidFile_SavesSuccessfully()
    {
        var fileContent = "fake image content";
        var fileName = "test-image.jpg";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));

        var file = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };

        var dto = new ImageUploadDto
        {
            File = file,
            Caption = "Test Image",
            TourId = Guid.NewGuid(),
            LogId = null
        };

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Image>()))
                 .ReturnsAsync(new Image { ImageId = Guid.NewGuid(), FilePath = "test.jpg" });

        var result = await _imageService.SaveImageAsync(dto);

        Assert.That(result, Is.Not.Null);
        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Image>()), Times.Once);
    }

    [Test]
    public void SaveImageAsync_EmptyFile_ThrowsException()
    {
        var stream = new MemoryStream();
        var file = new FormFile(stream, 0, 0, "file", "empty.jpg");

        var dto = new ImageUploadDto
        {
            File = file,
            Caption = "Empty",
            TourId = Guid.NewGuid(),
            LogId = null
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await _imageService.SaveImageAsync(dto));
    }

    [Test]
    public void SaveImageAsync_InvalidFormat_ThrowsException()
    {
        var fileContent = "malicious content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
        var file = new FormFile(stream, 0, stream.Length, "file", "malware.exe");

        var dto = new ImageUploadDto
        {
            File = file,
            Caption = "Bad file",
            TourId = Guid.NewGuid(),
            LogId = null
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await _imageService.SaveImageAsync(dto));
    }

    [Test]
    public void SaveImageAsync_NoTourOrLogId_ThrowsException()
    {
        var fileContent = "image content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
        var file = new FormFile(stream, 0, stream.Length, "file", "test.jpg");

        var dto = new ImageUploadDto
        {
            File = file,
            Caption = "No owner",
            TourId = null,
            LogId = null
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await _imageService.SaveImageAsync(dto));
    }

    [Test]
    public async Task SaveImageAsync_WithCaption_SavesCaptionCorrectly()
    {
        var fileContent = "image content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
        var file = new FormFile(stream, 0, stream.Length, "file", "test.jpg");

        var caption = "Beautiful landscape";
        var dto = new ImageUploadDto
        {
            File = file,
            Caption = caption,
            TourId = Guid.NewGuid(),
            LogId = null
        };

        Image? savedImage = null;
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Image>()))
                 .Callback<Image>(img => savedImage = img)
                 .ReturnsAsync(new Image { ImageId = Guid.NewGuid() });

        await _imageService.SaveImageAsync(dto);

        Assert.That(savedImage!.Caption, Is.EqualTo(caption));
    }

    [Test]
    public async Task SaveImageAsync_CallsRepositoryOnce()
    {
        var fileContent = "image content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
        var file = new FormFile(stream, 0, stream.Length, "file", "test.jpg");

        var dto = new ImageUploadDto
        {
            File = file,
            Caption = "Test",
            TourId = Guid.NewGuid(),
            LogId = null
        };

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Image>()))
                 .ReturnsAsync(new Image { ImageId = Guid.NewGuid() });

        await _imageService.SaveImageAsync(dto);

        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Image>()), Times.Once);
    }

    [Test]
    public void GetImage_FileExists_ReturnsBytes()
    {
        var fileName = "test-image.jpg";
        var fullPath = Path.Combine(_testStorageDir, fileName);
        var testContent = "test image content"u8.ToArray();
        File.WriteAllBytes(fullPath, testContent);

        var result = _imageService.GetImage(fileName);

        Assert.That(result, Is.EqualTo(testContent));
    }

    [Test]
    public void GetImage_FileNotFound_ThrowsException()
    {
        Assert.Throws<FileNotFoundException>(() => _imageService.GetImage("nonexistent.jpg"));
    }

    [Test]
    public void DeleteImage_FileExists_DeletesSuccessfully()
    {
        var fileName = "to-delete.jpg";
        var fullPath = Path.Combine(_testStorageDir, fileName);
        File.WriteAllText(fullPath, "content to delete");

        _imageService.DeleteImage(fileName);

        Assert.That(File.Exists(fullPath), Is.False);
    }
}