using Moq;
using Microsoft.Extensions.Configuration;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using backend.Services;

namespace Tests;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserRepository> _mockRepo;
    private Mock<IConfiguration> _mockConfig;
    private AuthService _authService;
    private RegisterDto _validDto;
    private LoginDto _validLoginDto;

    [SetUp]
    public void SetUp()
    {
        _mockRepo = new Mock<IUserRepository>();
        _mockConfig = new Mock<IConfiguration>();

        // Simulate JWT secret from appsettings.json
        _mockConfig.Setup(c => c["Jwt:Secret"]).Returns("Xk9#mP2$vL7nQw4@jR6yTb8&hF3eDcZ1");

        _authService = new AuthService(_mockRepo.Object, _mockConfig.Object);

        // Valid registration data used as default for all tests
        _validDto = new RegisterDto
        {
            Username = "Melanie_99",
            Email = "melanie@example.com",
            Password = "Secure!123"
        };

        _validLoginDto = new LoginDto
        {
            Username = "Melanie_99",
            Password = "Secure!123"
        };

        // Default setup: nothing exists in DB, user creation succeeds
        _mockRepo.Setup(r => r.UsernameExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(Guid.NewGuid());
        _mockRepo.Setup(r => r.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
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

    // Test: login with non-existing username should fail
    [Test]
    public async Task Login_UsernameNotFound_ReturnsUnauthorized()
    {
        _mockRepo.Setup(r => r.GetByUsernameAsync(_validLoginDto.Username)).ReturnsAsync((User?)null);

        var result = await _authService.LoginAsync(_validLoginDto);

        Assert.That(result.success, Is.False);
        Assert.That(result.message, Is.Not.Empty);
    }

    // Test: login with wrong password should fail
    [Test]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = _validLoginDto.Username,
            Email = "melanie@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("DifferentPassword!1")
        };
        _mockRepo.Setup(r => r.GetByUsernameAsync(_validLoginDto.Username)).ReturnsAsync(user);

        var result = await _authService.LoginAsync(_validLoginDto);

        Assert.That(result.success, Is.False);
    }

    // Test: login with correct credentials should return a JWT token
    [Test]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = _validLoginDto.Username,
            Email = "melanie@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(_validLoginDto.Password)
        };
        _mockRepo.Setup(r => r.GetByUsernameAsync(_validLoginDto.Username)).ReturnsAsync(user);

        var result = await _authService.LoginAsync(_validLoginDto);

        Assert.That(result.success, Is.True);
        Assert.That(result.token, Is.Not.Empty);
    }

    // Test: returned JWT token must contain three parts separated by dots
    [Test]
    public async Task Login_ValidCredentials_ReturnsValidJwtFormat()
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = _validLoginDto.Username,
            Email = "melanie@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(_validLoginDto.Password)
        };
        _mockRepo.Setup(r => r.GetByUsernameAsync(_validLoginDto.Username)).ReturnsAsync(user);

        var result = await _authService.LoginAsync(_validLoginDto);

        // JWT format: header.claims.signature
        Assert.That(result.token.Split('.').Length, Is.EqualTo(3));
    }
}