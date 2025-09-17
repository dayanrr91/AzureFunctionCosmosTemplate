using AzureFunctionCosmosTemplate.Domain.DTOs;
using System.ComponentModel.DataAnnotations;

namespace AzureFunctionCosmosTemplate.UnitTests.Domain.DTOs;

/// <summary>
/// Unit tests for UsersDto validation and behavior.
/// </summary>
public class UsersDtoTests
{
    [Fact]
    public void UsersDto_WithValidData_ShouldPassValidation()
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
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "Doe", "john@example.com", true)] // Empty FirstName
    [InlineData("John", "", "john@example.com", true)] // Empty LastName
    [InlineData("John", "Doe", "", true)] // Empty Email
    [InlineData("John", "Doe", "invalid-email", true)] // Invalid Email
    public void UsersDto_WithInvalidData_ShouldFailValidation(string firstName, string lastName, string email, bool isActive)
    {
        // Arrange
        var dto = new UsersDto
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            IsActive = isActive
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().NotBeEmpty();
    }

    [Fact]
    public void UsersDto_WithTooLongFirstName_ShouldFailValidation()
    {
        // Arrange
        var dto = new UsersDto
        {
            FirstName = new string('A', 51), // Exceeds 50 character limit
            LastName = "Doe",
            Email = "john@example.com",
            IsActive = true
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().ContainSingle()
            .Which.ErrorMessage.Should().Contain("cannot exceed 50 characters");
    }

    [Fact]
    public void UsersDto_WithTooLongLastName_ShouldFailValidation()
    {
        // Arrange
        var dto = new UsersDto
        {
            FirstName = "John",
            LastName = new string('A', 51), // Exceeds 50 character limit
            Email = "john@example.com",
            IsActive = true
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().ContainSingle()
            .Which.ErrorMessage.Should().Contain("cannot exceed 50 characters");
    }

    [Fact]
    public void UsersDto_WithTooLongEmail_ShouldFailValidation()
    {
        // Arrange
        var dto = new UsersDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = new string('a', 90) + "@example.com", // Exceeds 100 character limit
            IsActive = true
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        validationResults.Should().ContainSingle()
            .Which.ErrorMessage.Should().Contain("cannot exceed 100 characters");
    }

    [Fact]
    public void UsersDto_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var dto = new UsersDto();

        // Assert
        dto.FirstName.Should().Be(string.Empty);
        dto.LastName.Should().Be(string.Empty);
        dto.Email.Should().Be(string.Empty);
        dto.IsActive.Should().BeTrue();
    }

    /// <summary>
    /// Helper method to validate a model using DataAnnotations.
    /// </summary>
    /// <param name="model">The model to validate.</param>
    /// <returns>A list of validation results.</returns>
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
