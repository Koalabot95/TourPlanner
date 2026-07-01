using backend.Data;
using backend.Interfaces;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace Tests;

[TestFixture]
public class SearchServiceTests
{
    private TourPlannerContext _context;
    private ISearchService _searchService;
    private Guid _userId;
    private string _userIdString;
    private Guid _otherUserId;
    private string _otherUserIdString;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<TourPlannerContext>()
            .UseInMemoryDatabase(databaseName: $"SearchServiceTest_{Guid.NewGuid()}")
            .Options;

        _context = new TourPlannerContext(options);
        _searchService = new SearchService(_context);

        _userId = Guid.NewGuid();
        _userIdString = _userId.ToString();
        _otherUserId = Guid.NewGuid();
        _otherUserIdString = _otherUserId.ToString();

        SeedTestData();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SeedTestData()
    {
        var tours = new List<Tour>
        {
            new Tour
            {
                TourId = Guid.NewGuid(),
                UserId = _userId,
                Name = "Vienna to Salzburg",
                Description = "Beautiful alpine route",
                StartLocation = "Vienna",
                EndLocation = "Salzburg",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Distance = 350.5,
                EstimatedTime = 360,
                TransportType = TransportMode.Driving,
                Popularity = 5,
                ChildFriendliness = 7.5,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Tour
            {
                TourId = Guid.NewGuid(),
                UserId = _userId,
                Name = "Salzburg City Walk",
                Description = "Historic city exploration",
                StartLocation = "Salzburg",
                EndLocation = "Salzburg",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Distance = 5.2,
                EstimatedTime = 120,
                TransportType = TransportMode.Walking,
                Popularity = 3,
                ChildFriendliness = 9.0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Tour
            {
                TourId = Guid.NewGuid(),
                UserId = _userId,
                Name = "Lake Bike Tour",
                Description = "Cycling around Hallstatt lake",
                StartLocation = "Hallstatt",
                EndLocation = "Hallstatt",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Distance = 25.0,
                EstimatedTime = 180,
                TransportType = TransportMode.Cycling,
                Popularity = 8,
                ChildFriendliness = 6.0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Tour
            {
                TourId = Guid.NewGuid(),
                UserId = _otherUserId,
                Name = "Other User Tour",
                Description = "This belongs to another user",
                StartLocation = "Berlin",
                EndLocation = "Munich",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Distance = 500.0,
                EstimatedTime = 450,
                TransportType = TransportMode.Driving,
                Popularity = 10,
                ChildFriendliness = 2.0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Tours.AddRange(tours);
        _context.SaveChanges();
    }

    [Test]
    public async Task SearchToursAsync_NoFilters_ReturnsAllUserTours()
    {
        var result = await _searchService.SearchToursAsync(_userIdString);

        Assert.That(result.TotalCount, Is.EqualTo(3));
        Assert.That(result.Tours, Is.Not.Null);
        Assert.That(result.Tours!.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task SearchToursAsync_SearchByName_ReturnsMatchingTours()
    {
        var result = await _searchService.SearchToursAsync(_userIdString, searchTerm: "Vienna");

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Tours, Is.Not.Null);
        Assert.That(result.Tours![0].Name, Is.EqualTo("Vienna to Salzburg"));
    }

    [Test]
    public async Task SearchToursAsync_SearchByDescription_ReturnsMatchingTours()
    {
        var result = await _searchService.SearchToursAsync(_userIdString, searchTerm: "alpine");

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Tours, Is.Not.Null);
        Assert.That(result.Tours![0].Name, Is.EqualTo("Vienna to Salzburg"));
    }

    [Test]
    public async Task SearchToursAsync_FilterByTransportMode_ReturnsMatchingTours()
    {
        var result = await _searchService.SearchToursAsync(_userIdString, transportMode: "Walking");

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Tours, Is.Not.Null);
        Assert.That(result.Tours![0].TransportMode, Is.EqualTo("Walking"));
    }

    [Test]
    public async Task SearchToursAsync_FilterByStartLocation_ReturnsMatchingTours()
    {
        var result = await _searchService.SearchToursAsync(_userIdString, startLocation: "Vienna");

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Tours, Is.Not.Null);
        Assert.That(result.Tours![0].StartLocation, Is.EqualTo("Vienna"));
    }

    [Test]
    public async Task SearchToursAsync_FilterByEndLocation_ReturnsMatchingTours()
    {
        var result = await _searchService.SearchToursAsync(_userIdString, endLocation: "Salzburg");

        Assert.That(result.TotalCount, Is.EqualTo(2));
    }

    [Test]
    public async Task SearchToursAsync_MultipleFilters_ReturnsMatchingTours()
    {
        var result = await _searchService.SearchToursAsync(
            _userIdString,
            transportMode: "Walking"
        );

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Tours, Is.Not.Null);
        Assert.That(result.Tours![0].Name, Is.EqualTo("Salzburg City Walk"));
    }

    [Test]
    public async Task SearchToursAsync_FilterByMinPopularity_ReturnsMatchingTours()
    {
        var result = await _searchService.SearchToursAsync(_userIdString, minPopularity: 5);

        Assert.That(result.TotalCount, Is.EqualTo(2));
    }

    [Test]
    public async Task SearchToursAsync_FilterByMaxPopularity_ReturnsMatchingTours()
    {
        var result = await _searchService.SearchToursAsync(_userIdString, maxPopularity: 5);

        Assert.That(result.TotalCount, Is.EqualTo(2));
    }

    [Test]
    public async Task SearchToursAsync_FilterByChildFriendliness_ReturnsMatchingTours()
    {
        var result = await _searchService.SearchToursAsync(_userIdString, minChildFriendliness: 7.0);

        Assert.That(result.TotalCount, Is.EqualTo(2));
    }

    [Test]
    public async Task SearchToursAsync_NoMatches_ReturnsEmptyList()
    {
        var result = await _searchService.SearchToursAsync(_userIdString, searchTerm: "NonExistent");

        Assert.That(result.TotalCount, Is.EqualTo(0));
        Assert.That(result.Tours, Is.Not.Null);
        Assert.That(result.Tours!.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task SearchToursAsync_DifferentUser_ReturnsOnlyTheirTours()
    {
        var result = await _searchService.SearchToursAsync(_otherUserIdString);

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Tours, Is.Not.Null);
        Assert.That(result.Tours![0].Name, Is.EqualTo("Other User Tour"));
    }

    [Test]
    public async Task SearchToursAsync_CaseInsensitiveSearch_ReturnsMatches()
    {
        var result = await _searchService.SearchToursAsync(_userIdString, searchTerm: "VIENNA");

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Tours, Is.Not.Null);
        Assert.That(result.Tours![0].Name, Is.EqualTo("Vienna to Salzburg"));
    }

    [Test]
    public async Task SearchToursAsync_ReturnsPopularityAndChildFriendliness()
    {
        var result = await _searchService.SearchToursAsync(_userIdString);

        Assert.That(result.Tours, Is.Not.Null);
        Assert.That(result.Tours!.Count > 0);
        Assert.That(result.Tours!.All(t => t.Popularity >= 0), Is.True);
        Assert.That(result.Tours!.All(t => t.ChildFriendliness >= 0), Is.True);
    }
}