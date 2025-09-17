using AzureFunctionCosmosTemplate.Application.Services;
using AzureFunctionCosmosTemplate.Domain.DTOs;
using AzureFunctionCosmosTemplate.Domain.Entities;
using AzureFunctionCosmosTemplate.Repository.Repositories.UsersRepository;
using Microsoft.Extensions.Logging;

namespace AzureFunctionCosmosTemplate.UnitTests.Application.Services;

/// <summary>
/// Unit tests for UsersService business logic.
/// </summary>
public class UsersServiceTests : TestBase
{
    private readonly Mock<IUsersCosmosRepository> _mockRepository;
    private readonly Mock<ILogger<UsersService>> _mockLogger;
    private readonly UsersService _service;

    public UsersServiceTests()
    {
        _mockRepository = new Mock<IUsersCosmosRepository>();
        _mockLogger = CreateMockLogger<UsersService>();
        _service = new UsersService(_mockRepository.Object, _mockLogger.Object);
    }

    #region GetAllUsersAsync Tests

    [Fact]
    public async Task GetAllUsersAsync_WithUsers_ShouldReturnUserDtos()
    {
        // Arrange
        var users = new List<Users>
        {
            new() { Id = "1", FirstName = "John", LastName = "Doe", Email = "john@example.com", IsActive = true },
            new() { Id = "2", FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", IsActive = false }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        result.Should().HaveCount(2);
        var resultList = result.ToList();
        resultList[0].FirstName.Should().Be("John");
        resultList[1].FirstName.Should().Be("Jane");

        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once());
        VerifyLogWasCalled(_mockLogger, LogLevel.Information, "Getting all users from service", Times.Once());
    }

    [Fact]
    public async Task GetAllUsersAsync_WithException_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var exception = new Exception("Database error");
        _mockRepository.Setup(r => r.GetAllAsync())
            .ThrowsAsync(exception);

        // Act & Assert
        var action = async () => await _service.GetAllUsersAsync();
        await action.Should().ThrowAsync<Exception>().WithMessage("Database error");

        VerifyLogWasCalled(_mockLogger, LogLevel.Error, Times.Once());
    }

    #endregion

    #region GetUserByIdAsync Tests

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ShouldReturnUserDto()
    {
        // Arrange
        var userId = "123";
        var user = new Users
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            IsActive = true
        };

        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("John");
        result.Email.Should().Be("john@example.com");

        _mockRepository.Verify(r => r.GetByIdAsync(userId), Times.Once());
    }

    [Fact]
    public async Task GetUserByIdAsync_WithEmptyId_ShouldReturnNull()
    {
        // Arrange & Act
        var result = await _service.GetUserByIdAsync("");

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(It.IsAny<string>()), Times.Never());
        VerifyLogWasCalled(_mockLogger, LogLevel.Warning, "GetUserByIdAsync called with empty id", Times.Once());
    }

    [Fact]
    public async Task GetUserByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var userId = "nonexistent";
        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((Users?)null);

        // Act
        var result = await _service.GetUserByIdAsync(userId);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(userId), Times.Once());
    }

    #endregion

    #region CreateUserAsync Tests

    [Fact]
    public async Task CreateUserAsync_WithValidDto_ShouldCreateAndReturnUserDto()
    {
        // Arrange
        var dto = new UsersDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            IsActive = true
        };

        var createdUser = new Users
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            IsActive = dto.IsActive
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(dto.Email))
            .ReturnsAsync((Users?)null);

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Users>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _service.CreateUserAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be(dto.FirstName);
        result.Email.Should().Be(dto.Email);

        _mockRepository.Verify(r => r.GetByEmailAsync(dto.Email), Times.Once());
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Users>()), Times.Once());
    }

    [Fact]
    public async Task CreateUserAsync_WithNullDto_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = async () => await _service.CreateUserAsync(null!);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CreateUserAsync_WithEmptyEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var dto = new UsersDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "",
            IsActive = true
        };

        // Act & Assert
        var action = async () => await _service.CreateUserAsync(dto);
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Email is required");
    }

    [Fact]
    public async Task CreateUserAsync_WithEmptyFirstName_ShouldThrowArgumentException()
    {
        // Arrange
        var dto = new UsersDto
        {
            FirstName = "",
            LastName = "Doe",
            Email = "john@example.com",
            IsActive = true
        };

        // Act & Assert
        var action = async () => await _service.CreateUserAsync(dto);
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("FirstName is required");
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var dto = new UsersDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "existing@example.com",
            IsActive = true
        };

        var existingUser = new Users { Email = dto.Email };
        _mockRepository.Setup(r => r.GetByEmailAsync(dto.Email))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var action = async () => await _service.CreateUserAsync(dto);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User with email existing@example.com already exists");
    }

    #endregion

    #region UpdateUserAsync Tests

    [Fact]
    public async Task UpdateUserAsync_WithValidData_ShouldUpdateAndReturnUserDto()
    {
        // Arrange
        var userId = "123";
        var dto = new UsersDto
        {
            FirstName = "Updated",
            LastName = "User",
            Email = "updated@example.com",
            IsActive = false
        };

        var existingUser = new Users
        {
            Id = userId,
            FirstName = "Original",
            LastName = "User",
            Email = "original@example.com",
            IsActive = true
        };

        var updatedUser = new Users
        {
            Id = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            IsActive = dto.IsActive
        };

        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Users>()))
            .ReturnsAsync(updatedUser);

        // Act
        var result = await _service.UpdateUserAsync(userId, dto);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be(dto.FirstName);
        result.Email.Should().Be(dto.Email);

        _mockRepository.Verify(r => r.GetByIdAsync(userId), Times.Once());
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Users>()), Times.Once());
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = "nonexistent";
        var dto = new UsersDto { FirstName = "John", LastName = "Doe", Email = "john@example.com" };

        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((Users?)null);

        // Act & Assert
        var action = async () => await _service.UpdateUserAsync(userId, dto);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"User with id {userId} not found");
    }

    #endregion

    #region DeleteUserAsync Tests

    [Fact]
    public async Task DeleteUserAsync_WithExistingUser_ShouldDeleteSuccessfully()
    {
        // Arrange
        var userId = "123";
        var existingUser = new Users { Id = userId, FirstName = "John" };

        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        // Act
        await _service.DeleteUserAsync(userId);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(userId), Times.Once());
        _mockRepository.Verify(r => r.DeleteAsync(userId), Times.Once());
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = "nonexistent";
        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((Users?)null);

        // Act & Assert
        var action = async () => await _service.DeleteUserAsync(userId);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"User with id {userId} not found");
    }

    #endregion

    #region GetUserByEmailAsync Tests

    [Fact]
    public async Task GetUserByEmailAsync_WithValidEmail_ShouldReturnUserDto()
    {
        // Arrange
        var email = "john@example.com";
        var user = new Users
        {
            Id = "123",
            FirstName = "John",
            LastName = "Doe",
            Email = email,
            IsActive = true
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
        result.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithEmptyEmail_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetUserByEmailAsync("");

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never());
        VerifyLogWasCalled(_mockLogger, LogLevel.Warning, "GetUserByEmailAsync called with empty email", Times.Once());
    }

    #endregion

    #region GetActiveUsersAsync Tests

    [Fact]
    public async Task GetActiveUsersAsync_WithActiveUsers_ShouldReturnActiveUserDtos()
    {
        // Arrange
        var activeUsers = new List<Users>
        {
            new() { Id = "1", FirstName = "John", LastName = "Doe", Email = "john@example.com", IsActive = true },
            new() { Id = "2", FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", IsActive = true }
        };

        _mockRepository.Setup(r => r.GetActiveUsersAsync())
            .ReturnsAsync(activeUsers);

        // Act
        var result = await _service.GetActiveUsersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(u => u.IsActive);

        _mockRepository.Verify(r => r.GetActiveUsersAsync(), Times.Once());
    }

    #endregion
}
