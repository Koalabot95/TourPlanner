using Moq;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using backend.Services;

namespace Tests;

[TestFixture]
public class TourServiceTests
{
    private Mock<ITourRepository> _mockRepo;
    private Mock<IOpenRouteServiceClient> _mockOrsClient;
    private Mock<IImageService> _mockImageService;
    private ITourService _tourService;
    private Guid _userId;
    private string _userIdString;
    private Guid _tourId;
    private TourDto _validTourDto;
    private Tour _existingTour;

    [SetUp]
    public void SetUp()
    {
        _mockRepo = new Mock<ITourRepository>();
        _mockOrsClient = new Mock<IOpenRouteServiceClient>();
        _mockImageService = new Mock<IImageService>();

        _tourService = new TourService(_mockRepo.Object, _mockOrsClient.Object, _mockImageService.Object);

        _userId = Guid.NewGuid();
        _userIdString = _userId.ToString();
        _tourId = Guid.NewGuid();

        _validTourDto = new TourDto
        {
            Name = "Vienna to Salzburg",
            Description = "A scenic drive",
            StartLocation = "Vienna",
            EndLocation = "Salzburg",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow,
            TransportType = TransportMode.Driving
        };

        _existingTour = new Tour
        {
            TourId = _tourId,
            UserId = _userId,
            Name = _validTourDto.Name,
            Description = _validTourDto.Description,
            StartLocation = _validTourDto.StartLocation,
            EndLocation = _validTourDto.EndLocation,
            StartDate = _validTourDto.StartDate,
            EndDate = _validTourDto.EndDate,
            TransportType = _validTourDto.TransportType,
            Distance = 300.5,
            EstimatedTime = 3.5,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Tour>())).ReturnsAsync(_existingTour);
        _mockRepo.Setup(r => r.GetByIdAsync(_tourId)).ReturnsAsync(_existingTour);
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { _existingTour });
    }


    [Test]
    public async Task CreateTourAsync_ValidData_ReturnsSuccess()
    {
        _mockOrsClient.Setup(o => o.GetRouteAsync("Vienna", "Salzburg", TransportMode.Driving))
                     .ReturnsAsync(new RouteResult { Distance = 300.5, EstimatedTime = 3.5 });

        var result = await _tourService.CreateTourAsync(_validTourDto, _userIdString);

        Assert.That(result.Tour, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(201));
    }

    [Test]
    public async Task CreateTourAsync_ValidData_CallsCreateOnce()
    {
        await _tourService.CreateTourAsync(_validTourDto, _userIdString);

        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Tour>()), Times.Once);
    }

    [Test]
    public async Task CreateTourAsync_ValidData_SavesCorrectUserAssignment()
    {
        Tour? savedTour = null;
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Tour>()))
                 .Callback<Tour>(t => savedTour = t)
                 .ReturnsAsync(new Tour { TourId = _tourId, UserId = _userId });

        await _tourService.CreateTourAsync(_validTourDto, _userIdString);

        Assert.That(savedTour!.UserId, Is.EqualTo(_userId));
    }

    [Test]
    public async Task CreateTourAsync_StartDateAfterEndDate_ReturnsFail()
    {
        var invalidDto = new TourDto
        {
            Name = "Test",
            Description = "Test",
            StartLocation = "A",
            EndLocation = "B",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(-1), // End before start!
            TransportType = TransportMode.Walking
        };

        var result = await _tourService.CreateTourAsync(invalidDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(400));
        Assert.That(result.ErrorMessage, Is.Not.Empty);
    }

    [Test]
    public async Task CreateTourAsync_EmptyName_ReturnsFail()
    {
        var invalidDto = new TourDto
        {
            Name = "", // Empty!
            Description = "Test",
            StartLocation = "A",
            EndLocation = "B",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow,
            TransportType = TransportMode.Walking
        };

        var result = await _tourService.CreateTourAsync(invalidDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task CreateTourAsync_OrsClientFails_StillCreatesTour()
    {
        var failDto = new TourDto
        {
            Name = "Failed Route Tour",
            Description = "Test",
            StartLocation = "BadStart",
            EndLocation = "BadEnd",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow,
            TransportType = TransportMode.Cycling
        };

        _mockOrsClient.Setup(o => o.GetRouteAsync("BadStart", "BadEnd", TransportMode.Cycling))
                     .ReturnsAsync((RouteResult?)null);

        Tour? savedTour = null;
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Tour>()))
                 .Callback<Tour>(t => savedTour = t)
                 .ReturnsAsync(new Tour { TourId = _tourId, UserId = _userId });

        var result = await _tourService.CreateTourAsync(failDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(201));
        Assert.That(savedTour!.Distance, Is.Null);
        Assert.That(savedTour.EstimatedTime, Is.Null);
    }


    [Test]
    public async Task GetTourByIdAsync_ValidTour_ReturnsSuccess()
    {
        var result = await _tourService.GetTourByIdAsync(_tourId, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Tour, Is.Not.Null);
        Assert.That(result.Tour!.TourId, Is.EqualTo(_tourId));
    }

    [Test]
    public async Task GetTourByIdAsync_TourNotFound_ReturnsFail()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Tour?)null);

        var result = await _tourService.GetTourByIdAsync(Guid.NewGuid(), _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(404));
        Assert.That(result.ErrorMessage, Contains.Substring("existiert nicht"));
    }

    [Test]
    public async Task GetTourByIdAsync_WrongUser_ReturnsFail()
    {
        var differentUserId = Guid.NewGuid().ToString();

        var result = await _tourService.GetTourByIdAsync(_tourId, differentUserId);

        Assert.That(result.StatusCode, Is.EqualTo(403));
    }

    [Test]
    public async Task GetAllToursAsync_MultipleToursExist_ReturnsOnlyUserTours()
    {
        var tour2 = new Tour { TourId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        var tour3 = new Tour { TourId = Guid.NewGuid(), UserId = _userId };

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { _existingTour, tour2, tour3 });

        var result = await _tourService.GetAllToursAsync(_userIdString);

        Assert.That(result.Tours, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Tours!.Count(), Is.EqualTo(2));
        Assert.That(result.Tours.All(t => t.TourId == _tourId || t.TourId == tour3.TourId), Is.True);
    }


    [Test]
    public async Task UpdateTourAsync_ValidData_ReturnsSuccess()
    {
        _mockOrsClient.Setup(o => o.GetRouteAsync("Vienna", "Salzburg", TransportMode.Cycling))
                     .ReturnsAsync(new RouteResult { Distance = 350.0, EstimatedTime = 4.0 });

        var updateDto = new TourDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            StartLocation = "Vienna",
            EndLocation = "Salzburg",
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow.AddDays(-1),
            TransportType = TransportMode.Cycling
        };

        var result = await _tourService.UpdateTourAsync(_tourId, updateDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Tour, Is.Not.Null);
    }

    [Test]
    public async Task UpdateTourAsync_TourNotFound_ReturnsFail()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Tour?)null);

        var updateDto = new TourDto
        {
            Name = "Test",
            Description = "Test",
            StartLocation = "A",
            EndLocation = "B",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow,
            TransportType = TransportMode.Walking
        };

        var result = await _tourService.UpdateTourAsync(Guid.NewGuid(), updateDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task UpdateTourAsync_WrongUser_ReturnsFail()
    {
        var differentUserId = Guid.NewGuid().ToString();
        var updateDto = new TourDto
        {
            Name = "Test",
            Description = "Test",
            StartLocation = "A",
            EndLocation = "B",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow,
            TransportType = TransportMode.Walking
        };

        var result = await _tourService.UpdateTourAsync(_tourId, updateDto, differentUserId);

        Assert.That(result.StatusCode, Is.EqualTo(403));
    }

    [Test]
    public async Task UpdateTourAsync_RouteChanged_CallsOrsClient()
    {
        _mockOrsClient.Setup(o => o.GetRouteAsync("NewStart", "NewEnd", TransportMode.Walking))
                     .ReturnsAsync(new RouteResult { Distance = 200.0, EstimatedTime = 2.5 });

        var updateDto = new TourDto
        {
            Name = "Test",
            Description = "Test",
            StartLocation = "NewStart", // Different from original!
            EndLocation = "NewEnd",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow,
            TransportType = TransportMode.Walking
        };

        await _tourService.UpdateTourAsync(_tourId, updateDto, _userIdString);

        _mockOrsClient.Verify(o => o.GetRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TransportMode>()),
                             Times.Once);
    }

    [Test]
    public async Task UpdateTourAsync_RouteUnchanged_SkipsOrsClient()
    {
        var updateDto = new TourDto
        {
            Name = "Different Name",
            Description = "Test",
            StartLocation = _validTourDto.StartLocation, // Same!
            EndLocation = _validTourDto.EndLocation, // Same!
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow,
            TransportType = _validTourDto.TransportType // Same! (not Walking)
        };

        await _tourService.UpdateTourAsync(_tourId, updateDto, _userIdString);

        _mockOrsClient.Verify(o => o.GetRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TransportMode>()),
                             Times.Never);
    }


    [Test]
    public async Task DeleteTourAsync_ValidTour_ReturnsSuccess()
    {
        var result = await _tourService.DeleteTourAsync(_tourId, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(204));
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task DeleteTourAsync_TourNotFound_ReturnsFail()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Tour?)null);

        var result = await _tourService.DeleteTourAsync(Guid.NewGuid(), _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(404));
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task DeleteTourAsync_WrongUser_ReturnsFail()
    {
        var differentUserId = Guid.NewGuid().ToString();

        var result = await _tourService.DeleteTourAsync(_tourId, differentUserId);

        Assert.That(result.StatusCode, Is.EqualTo(403));
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task DeleteTourAsync_ValidTour_CallsDeleteOnce()
    {
        await _tourService.DeleteTourAsync(_tourId, _userIdString);

        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Tour>()), Times.Once);
    }

    [Test]
    public async Task DeleteTourAsync_WithMapSnapshot_DeletesImageFile()
    {
        var tourWithSnapshot = new Tour
        {
            TourId = _tourId,
            UserId = _userId,
            MapSnapshotPath = "snapshot-123.png"
        };
        _mockRepo.Setup(r => r.GetByIdAsync(_tourId)).ReturnsAsync(tourWithSnapshot);

        await _tourService.DeleteTourAsync(_tourId, _userIdString);

        _mockImageService.Verify(i => i.DeleteImage("snapshot-123.png"), Times.Once);
    }
}