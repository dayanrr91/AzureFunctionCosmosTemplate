using AzureFunctionCosmosTemplate.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace AzureFunctionCosmosTemplate.Domain.DTOs;

/// <summary>
/// Data Transfer Object for Users.
/// Contains only the data that should be exposed through the API.
/// </summary>
public class UsersDto
{
    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the user is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Extension methods for UsersDto mapping.
/// </summary>
public static class UsersDtoExtensions
{
    /// <summary>
    /// Converts a UsersDto to a Users entity for creation.
    /// Sets a new GUID as Id and initializes internal properties.
    /// </summary>
    /// <param name="dto">The UsersDto to convert.</param>
    /// <returns>A new Users entity ready for creation.</returns>
    public static Users ToEntity(this UsersDto dto)
    {
        return new Users
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            IsActive = dto.IsActive
        };
    }

    /// <summary>
    /// Updates an existing Users entity with data from a UsersDto.
    /// Preserves internal properties like Id, CreatedAt, etc.
    /// </summary>
    /// <param name="dto">The UsersDto with updated data.</param>
    /// <param name="existingEntity">The existing Users entity to update.</param>
    /// <returns>The updated Users entity.</returns>
    public static Users UpdateEntity(this UsersDto dto, Users existingEntity)
    {
        existingEntity.FirstName = dto.FirstName;
        existingEntity.LastName = dto.LastName;
        existingEntity.Email = dto.Email;
        existingEntity.IsActive = dto.IsActive;

        return existingEntity;
    }
}
