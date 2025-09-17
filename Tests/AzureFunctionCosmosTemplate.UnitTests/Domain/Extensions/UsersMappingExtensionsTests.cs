using AzureFunctionCosmosTemplate.Domain.DTOs;
using AzureFunctionCosmosTemplate.Domain.Entities;

namespace AzureFunctionCosmosTemplate.UnitTests.Domain.Extensions;

/// <summary>
/// Unit tests for Users mapping extension methods.
/// </summary>
public class UsersMappingExtensionsTests
{
    [Fact]
    public void ToDto_WithValidUser_ShouldMapCorrectly()
    {
        // Arrange
        var user = new Users
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = user.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.FirstName.Should().Be(user.FirstName);
        dto.LastName.Should().Be(user.LastName);
        dto.Email.Should().Be(user.Email);
        dto.IsActive.Should().Be(user.IsActive);
    }

    [Fact]
    public void ToDto_WithUserCollection_ShouldMapAllItems()
    {
        // Arrange
        var users = new List<Users>
        {
            new() { FirstName = "John", LastName = "Doe", Email = "john@example.com", IsActive = true },
            new() { FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", IsActive = false }
        };

        // Act
        var dtos = users.ToDto().ToList();

        // Assert
        dtos.Should().HaveCount(2);
        dtos[0].FirstName.Should().Be("John");
        dtos[0].LastName.Should().Be("Doe");
        dtos[0].Email.Should().Be("john@example.com");
        dtos[0].IsActive.Should().BeTrue();

        dtos[1].FirstName.Should().Be("Jane");
        dtos[1].LastName.Should().Be("Smith");
        dtos[1].Email.Should().Be("jane@example.com");
        dtos[1].IsActive.Should().BeFalse();
    }

    [Fact]
    public void ToEntity_WithValidDto_ShouldCreateNewUserWithGuid()
    {
        // Arrange
        var dto = new UsersDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            IsActive = true
        };

        // Act
        var entity = dto.ToEntity();

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().NotBeNullOrEmpty();
        Guid.TryParse(entity.Id, out _).Should().BeTrue("Id should be a valid GUID");
        entity.FirstName.Should().Be(dto.FirstName);
        entity.LastName.Should().Be(dto.LastName);
        entity.Email.Should().Be(dto.Email);
        entity.IsActive.Should().Be(dto.IsActive);
        entity.PartitionKey.Should().Be("users");
    }

    [Fact]
    public void ToEntity_MultipleCallsWithSameDto_ShouldCreateDifferentIds()
    {
        // Arrange
        var dto = new UsersDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            IsActive = true
        };

        // Act
        var entity1 = dto.ToEntity();
        var entity2 = dto.ToEntity();

        // Assert
        entity1.Id.Should().NotBe(entity2.Id, "Each call should generate a new GUID");
    }

    [Fact]
    public void UpdateEntity_WithValidDto_ShouldUpdateExistingEntity()
    {
        // Arrange
        var originalEntity = new Users
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Original",
            LastName = "User",
            Email = "original@example.com",
            IsActive = false,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var dto = new UsersDto
        {
            FirstName = "Updated",
            LastName = "Name",
            Email = "updated@example.com",
            IsActive = true
        };

        // Act
        var updatedEntity = dto.UpdateEntity(originalEntity);

        // Assert
        updatedEntity.Should().BeSameAs(originalEntity, "Should return the same entity instance");
        updatedEntity.Id.Should().Be(originalEntity.Id, "Id should not change");
        updatedEntity.CreatedAt.Should().Be(originalEntity.CreatedAt, "CreatedAt should not change");
        updatedEntity.FirstName.Should().Be(dto.FirstName);
        updatedEntity.LastName.Should().Be(dto.LastName);
        updatedEntity.Email.Should().Be(dto.Email);
        updatedEntity.IsActive.Should().Be(dto.IsActive);
    }

    [Fact]
    public void UpdateEntity_WithNullEntity_ShouldThrowArgumentNullException()
    {
        // Arrange
        var dto = new UsersDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            IsActive = true
        };

        // Act & Assert
        var action = () => dto.UpdateEntity(null!);
        action.Should().Throw<NullReferenceException>();
    }
}
