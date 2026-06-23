using Moq;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using backend.Services;

namespace Tests;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _mockRepo;
    private IUserService _userService;
    private Guid _userId;
    private string _userIdString;
    private User _existingUser;

    [SetUp]
    public void SetUp()
    {
        _mockRepo = new Mock<IUserRepository>();
        _userService = new UserService(_mockRepo.Object);

        _userId = Guid.NewGuid();
        _userIdString = _userId.ToString();

        _existingUser = new User
        {
            UserId = _userId,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            FirstName = "John",
            LastName = "Doe",
            Bio = "Test bio",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepo.Setup(r => r.GetByIdAsync(_userId)).ReturnsAsync(_existingUser);
    }

    [Test]
    public async Task GetUserByIdAsync_ValidUser_ReturnsSuccess()
    {
        var result = await _userService.GetUserByIdAsync(_userId, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.User, Is.Not.Null);
        Assert.That(result.User!.Username, Is.EqualTo("testuser"));
    }

    [Test]
    public async Task GetUserByIdAsync_UserNotFound_Returns404()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await _userService.GetUserByIdAsync(Guid.NewGuid(), _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(404));
        Assert.That(result.User, Is.Null);
    }

    [Test]
    public async Task GetUserByIdAsync_WrongUser_Returns403()
    {
        var differentUserId = Guid.NewGuid().ToString();

        var result = await _userService.GetUserByIdAsync(_userId, differentUserId);

        Assert.That(result.StatusCode, Is.EqualTo(403));
    }

    [Test]
    public async Task GetUserByIdAsync_ReturnsCorrectData()
    {
        var result = await _userService.GetUserByIdAsync(_userId, _userIdString);

        Assert.That(result.User!.Email, Is.EqualTo("test@example.com"));
        Assert.That(result.User.FirstName, Is.EqualTo("John"));
        Assert.That(result.User.LastName, Is.EqualTo("Doe"));
        Assert.That(result.User.Bio, Is.EqualTo("Test bio"));
    }

    [Test]
    public async Task UpdateUserAsync_ValidData_ReturnsSuccess()
    {
        var updateDto = new UpdateUserDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Bio = "Updated bio"
        };

        var result = await _userService.UpdateUserAsync(_userId, updateDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.User, Is.Not.Null);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public async Task UpdateUserAsync_UserNotFound_Returns404()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var updateDto = new UpdateUserDto { FirstName = "Jane" };
        var result = await _userService.UpdateUserAsync(Guid.NewGuid(), updateDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task UpdateUserAsync_WrongUser_Returns403()
    {
        var differentUserId = Guid.NewGuid().ToString();
        var updateDto = new UpdateUserDto { FirstName = "Jane" };

        var result = await _userService.UpdateUserAsync(_userId, updateDto, differentUserId);

        Assert.That(result.StatusCode, Is.EqualTo(403));
    }

    [Test]
    public async Task UpdateUserAsync_FirstNameTooLong_ReturnsFail()
    {
        var updateDto = new UpdateUserDto
        {
            FirstName = new string('a', 51)
        };

        var result = await _userService.UpdateUserAsync(_userId, updateDto, _userIdString);

        Assert.That(result.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task UpdateUserAsync_PartialUpdate_PreservesOtherFields()
    {
        var updateDto = new UpdateUserDto { FirstName = "Jane" };

        User? savedUser = null;
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                 .Callback<User>(u => savedUser = u)
                 .Returns(Task.CompletedTask);

        await _userService.UpdateUserAsync(_userId, updateDto, _userIdString);

        Assert.That(savedUser!.FirstName, Is.EqualTo("Jane"));
        Assert.That(savedUser.LastName, Is.EqualTo("Doe"));
        Assert.That(savedUser.Bio, Is.EqualTo("Test bio"));
    }
}