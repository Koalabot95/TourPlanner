using Moq;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using backend.Services;

namespace Tests;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserRepository> _mockRepo;
    private AuthService _authService;
    private RegisterDto _validDto;

    [SetUp]
    public void SetUp()
    {
        _mockRepo = new Mock<IUserRepository>();
        _authService = new AuthService(_mockRepo.Object);

        // Valid registration data used as default for all tests
        _validDto = new RegisterDto
        {
            Username = "Melanie_99",
            Email = "melanie@example.com",
            Password = "Secure!123"
        };

        // Default setup: nothing exists in DB, user creation succeeds
        _mockRepo.Setup(r => r.UsernameExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(Guid.NewGuid());
    }

    // Test: valid registration data should return success
    [Test]
    public async Task Register_ValidData_ReturnsSuccess()
    {
        var result = await _authService.RegisterAsync(_validDto);

        Assert.That(result.success, Is.True);
        Assert.That(result.userId, Is.Not.EqualTo(Guid.Empty));
    }

    // Test: if username is already taken, registration should fail with field "username"
    [Test]
    public async Task Register_UsernameTaken_ReturnsConflict()
    {
        _mockRepo.Setup(r => r.UsernameExistsAsync(_validDto.Username)).ReturnsAsync(true);

        var result = await _authService.RegisterAsync(_validDto);

        Assert.That(result.success, Is.False);
        Assert.That(result.field, Is.EqualTo("username"));
    }

    // Test: if email is already taken, registration should fail with field "email"
    [Test]
    public async Task Register_EmailTaken_ReturnsConflict()
    {
        _mockRepo.Setup(r => r.EmailExistsAsync(_validDto.Email)).ReturnsAsync(true);

        var result = await _authService.RegisterAsync(_validDto);

        Assert.That(result.success, Is.False);
        Assert.That(result.field, Is.EqualTo("email"));
    }

    // Test: if username is taken, email check should never be called
    [Test]
    public async Task Register_UsernameTaken_DoesNotCheckEmail()
    {
        _mockRepo.Setup(r => r.UsernameExistsAsync(_validDto.Username)).ReturnsAsync(true);

        var result = await _authService.RegisterAsync(_validDto);

        _mockRepo.Verify(r => r.EmailExistsAsync(It.IsAny<string>()), Times.Never);
    }

    // Test: password must be hashed before saving, plain text must never be stored
    [Test]
    public async Task Register_PasswordIsHashed_NotPlaintext()
    {
        User? savedUser = null;
        _mockRepo.Setup(r => r.CreateUserAsync(It.IsAny<User>()))
                 .Callback<User>(u => savedUser = u)
                 .ReturnsAsync(Guid.NewGuid());

        await _authService.RegisterAsync(_validDto);

        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser!.PasswordHash, Is.Not.EqualTo(_validDto.Password));
        Assert.That(savedUser.PasswordHash, Does.StartWith("$2")); // BCrypt hashes start with $2
    }

    // Test: the stored BCrypt hash must be verifiable with the original password
    [Test]
    public async Task Register_PasswordHash_IsVerifiableWithBCrypt()
    {
        User? savedUser = null;
        _mockRepo.Setup(r => r.CreateUserAsync(It.IsAny<User>()))
                 .Callback<User>(u => savedUser = u)
                 .ReturnsAsync(Guid.NewGuid());

        await _authService.RegisterAsync(_validDto);

        Assert.That(BCrypt.Net.BCrypt.Verify(_validDto.Password, savedUser!.PasswordHash), Is.True);
    }

    // Test: CreateUserAsync should be called exactly once on successful registration
    [Test]
    public async Task Register_ValidData_CallsCreateUserOnce()
    {
        await _authService.RegisterAsync(_validDto);

        _mockRepo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Once);
    }

    // Test: the saved user must contain the correct username and email
    [Test]
    public async Task Register_ValidData_SavesCorrectUserData()
    {
        User? savedUser = null;
        _mockRepo.Setup(r => r.CreateUserAsync(It.IsAny<User>()))
                 .Callback<User>(u => savedUser = u)
                 .ReturnsAsync(Guid.NewGuid());

        await _authService.RegisterAsync(_validDto);

        Assert.That(savedUser!.Username, Is.EqualTo(_validDto.Username));
        Assert.That(savedUser.Email, Is.EqualTo(_validDto.Email));
    }

    // Test: returned userId must be a valid non-empty GUID
    [Test]
    public async Task Register_ValidData_ReturnsValidGuid()
    {
        var result = await _authService.RegisterAsync(_validDto);

        Assert.That(result.userId, Is.Not.EqualTo(Guid.Empty));
    }

    // Test: error message must not be empty when username is already taken
    [Test]
    public async Task Register_UsernameTaken_ReturnsCorrectMessage()
    {
        _mockRepo.Setup(r => r.UsernameExistsAsync(_validDto.Username)).ReturnsAsync(true);

        var result = await _authService.RegisterAsync(_validDto);

        Assert.That(result.message, Is.Not.Empty);
    }

    // Test: error message must not be empty when email is already taken
    [Test]
    public async Task Register_EmailTaken_ReturnsCorrectMessage()
    {
        _mockRepo.Setup(r => r.EmailExistsAsync(_validDto.Email)).ReturnsAsync(true);

        var result = await _authService.RegisterAsync(_validDto);

        Assert.That(result.message, Is.Not.Empty);
    }
}