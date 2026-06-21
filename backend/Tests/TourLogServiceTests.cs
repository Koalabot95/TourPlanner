using Moq;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using backend.Services;

namespace Tests;

[TestFixture]
public class TourLogServiceTests
{
    private Mock<ITourLogRepository> _mockLogRepo;
    private Mock<ITourRepository> _mockTourRepo;
    private ITourLogService _logService;
    private Guid _userId;
    private string _userIdString;
    private Guid _tourId;
    private Guid _logId;
    private TourLogCreateUpdateDto _validLogDto;
    private Tour _existingTour;
    private TourLog _existingLog;

    [SetUp]
    public void SetUp()
    {
        _mockLogRepo = new Mock<ITourLogRepository>();
        _mockTourRepo = new Mock<ITourRepository>();
        _logService = new TourLogService(_mockLogRepo.Object, _mockTourRepo.Object);

        _userId = Guid.NewGuid();
        _userIdString = _userId.ToString();
        _tourId = Guid.NewGuid();
        _logId = Guid.NewGuid();

        _validLogDto = new TourLogCreateUpdateDto
        {
            Name = "Day 1 Log",
            Comment = "Great weather today",
            DateTime = DateTime.UtcNow.AddDays(-1),
            Difficulty = "Moderate",
            TotalDistance = 25.5,
            TotalTime = 4.5,
            Rating = 4
        };

        _existingTour = new Tour
        {
            TourId = _tourId,
            UserId = _userId,
            Name = "Test Tour",
            Description = "Test",
            StartLocation = "A",
            EndLocation = "B",
            StartDate = DateTime.UtcNow.AddDays(-7),
            EndDate = DateTime.UtcNow
        };

        _existingLog = new TourLog
        {
            LogId = _logId,
            TourId = _tourId,
            Tour = _existingTour,
            Name = _validLogDto.Name,
            Comment = _validLogDto.Comment,
            DateTime = _validLogDto.DateTime,
            Difficulty = _validLogDto.Difficulty,
            TotalDistance = _validLogDto.TotalDistance,
            TotalTime = _validLogDto.TotalTime,
            Rating = _validLogDto.Rating,
            CreatedAt = DateTime.UtcNow
        };

        _mockTourRepo.Setup(r => r.GetByIdAsync(_tourId)).ReturnsAsync(_existingTour);
        _mockLogRepo.Setup(r => r.CreateAsync(It.IsAny<TourLog>())).ReturnsAsync(_existingLog);
        _mockLogRepo.Setup(r => r.GetByIdAsync(_logId)).ReturnsAsync(_existingLog);
        _mockLogRepo.Setup(r => r.GetLogsByTourIdAsync(_tourId)).ReturnsAsync(new[] { _existingLog });
    }

    [Test]
    public async Task CreateLogAsync_ValidData_ReturnsSuccess()
    {
        var result = await _logService.CreateLogAsync(_tourId, _validLogDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(201));
        Assert.That(result.Log, Is.Not.Null);
        Assert.That(result.Log!.LogId, Is.EqualTo(_logId));
    }

    [Test]
    public async Task CreateLogAsync_ValidData_CallsCreateOnce()
    {
        await _logService.CreateLogAsync(_tourId, _validLogDto, _userIdString);

        _mockLogRepo.Verify(r => r.CreateAsync(It.IsAny<TourLog>()), Times.Once);
    }

    [Test]
    public async Task CreateLogAsync_ValidData_SavesCorrectTourAssignment()
    {
        TourLog? savedLog = null;
        _mockLogRepo.Setup(r => r.CreateAsync(It.IsAny<TourLog>()))
                    .Callback<TourLog>(l => savedLog = l)
                    .ReturnsAsync(_existingLog);

        await _logService.CreateLogAsync(_tourId, _validLogDto, _userIdString);

        Assert.That(savedLog!.TourId, Is.EqualTo(_tourId));
    }

    [Test]
    public async Task CreateLogAsync_TourNotFound_ReturnsFail()
    {
        _mockTourRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Tour?)null);

        var result = await _logService.CreateLogAsync(Guid.NewGuid(), _validLogDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(404));
        Assert.That(result.Log, Is.Null);
    }

    [Test]
    public async Task CreateLogAsync_WrongTourOwner_ReturnsFail()
    {
        var differentUserId = Guid.NewGuid().ToString();

        var result = await _logService.CreateLogAsync(_tourId, _validLogDto, differentUserId);

        Assert.That(result.StatusCode, Is.EqualTo(403));
        Assert.That(result.ErrorMessage, Contains.Substring("Berechtigung"));
    }

    [Test]
    public async Task CreateLogAsync_FutureDateTime_ReturnsFail()
    {
        var invalidDto = new TourLogCreateUpdateDto
        {
            Name = "Test",
            Comment = "Test",
            DateTime = DateTime.UtcNow.AddDays(1),
            Difficulty = "Easy",
            TotalDistance = 10,
            TotalTime = 2,
            Rating = 5
        };

        var result = await _logService.CreateLogAsync(_tourId, invalidDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(400));
        Assert.That(result.ErrorMessage, Contains.Substring("Zukunft"));
    }

    [Test]
    public async Task CreateLogAsync_InvalidDifficulty_ReturnsFail()
    {
        var invalidDto = new TourLogCreateUpdateDto
        {
            Name = "Test",
            Comment = "Test",
            DateTime = DateTime.UtcNow.AddDays(-1),
            Difficulty = "VeryHard",
            TotalDistance = 10,
            TotalTime = 2,
            Rating = 5
        };

        var result = await _logService.CreateLogAsync(_tourId, invalidDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task CreateLogAsync_NegativeTotalDistance_ReturnsFail()
    {
        var invalidDto = new TourLogCreateUpdateDto
        {
            Name = "Test",
            Comment = "Test",
            DateTime = DateTime.UtcNow.AddDays(-1),
            Difficulty = "Easy",
            TotalDistance = -5,
            TotalTime = 2,
            Rating = 5
        };

        var result = await _logService.CreateLogAsync(_tourId, invalidDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task GetLogByIdAsync_ValidLog_ReturnsSuccess()
    {
        var result = await _logService.GetLogByIdAsync(_logId, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Log, Is.Not.Null);
        Assert.That(result.Log!.LogId, Is.EqualTo(_logId));
    }

    [Test]
    public async Task GetLogByIdAsync_LogNotFound_ReturnsFail()
    {
        _mockLogRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TourLog?)null);

        var result = await _logService.GetLogByIdAsync(Guid.NewGuid(), _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task GetLogByIdAsync_LogBelongsToDifferentUser_ReturnsFail()
    {
        var differentUserId = Guid.NewGuid().ToString();

        var result = await _logService.GetLogByIdAsync(_logId, differentUserId);

        Assert.That(result.StatusCode, Is.EqualTo(403));
    }

    [Test]
    public async Task GetLogsForTourAsync_TourNotFound_ReturnsFail()
    {
        _mockTourRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Tour?)null);

        var result = await _logService.GetLogsForTourAsync(Guid.NewGuid(), _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(404));
        Assert.That(result.Logs, Is.Null);
    }

    [Test]
    public async Task GetLogsForTourAsync_WrongUser_ReturnsFail()
    {
        var differentUserId = Guid.NewGuid().ToString();

        var result = await _logService.GetLogsForTourAsync(_tourId, differentUserId);

        Assert.That(result.StatusCode, Is.EqualTo(403));
        Assert.That(result.Logs, Is.Null);
    }

    [Test]
    public async Task GetLogsForTourAsync_MultipleLogsExist_ReturnsAll()
    {
        var log2 = new TourLog { LogId = Guid.NewGuid(), TourId = _tourId, Tour = _existingTour };
        _mockLogRepo.Setup(r => r.GetLogsByTourIdAsync(_tourId))
                    .ReturnsAsync(new[] { _existingLog, log2 });

        var result = await _logService.GetLogsForTourAsync(_tourId, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Logs, Is.Not.Null);
        Assert.That(result.Logs!.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task UpdateLogAsync_ValidData_ReturnsSuccess()
    {
        var updateDto = new TourLogCreateUpdateDto
        {
            Name = "Updated Log",
            Comment = "Updated comment",
            DateTime = DateTime.UtcNow.AddDays(-1),
            Difficulty = "Hard",
            TotalDistance = 50,
            TotalTime = 8,
            Rating = 5
        };

        var result = await _logService.UpdateLogAsync(_logId, updateDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Log, Is.Not.Null);
    }

    [Test]
    public async Task UpdateLogAsync_LogNotFound_ReturnsFail()
    {
        _mockLogRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TourLog?)null);

        var updateDto = new TourLogCreateUpdateDto
        {
            Name = "Test",
            Comment = "Test",
            DateTime = DateTime.UtcNow.AddDays(-1),
            Difficulty = "Easy",
            TotalDistance = 10,
            TotalTime = 2,
            Rating = 3
        };

        var result = await _logService.UpdateLogAsync(Guid.NewGuid(), updateDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task UpdateLogAsync_WrongUser_ReturnsFail()
    {
        var differentUserId = Guid.NewGuid().ToString();
        var updateDto = new TourLogCreateUpdateDto
        {
            Name = "Test",
            Comment = "Test",
            DateTime = DateTime.UtcNow.AddDays(-1),
            Difficulty = "Easy",
            TotalDistance = 10,
            TotalTime = 2,
            Rating = 3
        };

        var result = await _logService.UpdateLogAsync(_logId, updateDto, differentUserId);

        Assert.That(result.StatusCode, Is.EqualTo(403));
    }

    [Test]
    public async Task UpdateLogAsync_FutureDateTime_ReturnsFail()
    {
        var updateDto = new TourLogCreateUpdateDto
        {
            Name = "Test",
            Comment = "Test",
            DateTime = DateTime.UtcNow.AddDays(2),
            Difficulty = "Easy",
            TotalDistance = 10,
            TotalTime = 2,
            Rating = 3
        };

        var result = await _logService.UpdateLogAsync(_logId, updateDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(400));
        Assert.That(result.ErrorMessage, Contains.Substring("Zukunft"));
    }

    [Test]
    public async Task UpdateLogAsync_InvalidDifficulty_ReturnsFail()
    {
        var updateDto = new TourLogCreateUpdateDto
        {
            Name = "Test",
            Comment = "Test",
            DateTime = DateTime.UtcNow.AddDays(-1),
            Difficulty = "SuperHard",
            TotalDistance = 10,
            TotalTime = 2,
            Rating = 3
        };

        var result = await _logService.UpdateLogAsync(_logId, updateDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task UpdateLogAsync_ValidData_CallsUpdateOnce()
    {
        var updateDto = new TourLogCreateUpdateDto
        {
            Name = "Test",
            Comment = "Test",
            DateTime = DateTime.UtcNow.AddDays(-1),
            Difficulty = "Easy",
            TotalDistance = 10,
            TotalTime = 2,
            Rating = 3
        };

        await _logService.UpdateLogAsync(_logId, updateDto, _userIdString);

        _mockLogRepo.Verify(r => r.UpdateAsync(It.IsAny<TourLog>()), Times.Once);
    }

    [Test]
    public async Task DeleteLogAsync_ValidLog_ReturnsSuccess()
    {
        var result = await _logService.DeleteLogAsync(_logId, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(204));
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task DeleteLogAsync_LogNotFound_ReturnsFail()
    {
        _mockLogRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TourLog?)null);

        var result = await _logService.DeleteLogAsync(Guid.NewGuid(), _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(404));
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task DeleteLogAsync_WrongUser_ReturnsFail()
    {
        var differentUserId = Guid.NewGuid().ToString();

        var result = await _logService.DeleteLogAsync(_logId, differentUserId);

        Assert.That(result.StatusCode, Is.EqualTo(403));
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task DeleteLogAsync_ValidLog_CallsDeleteOnce()
    {
        await _logService.DeleteLogAsync(_logId, _userIdString);

        _mockLogRepo.Verify(r => r.DeleteAsync(It.IsAny<TourLog>()), Times.Once);
    }
}