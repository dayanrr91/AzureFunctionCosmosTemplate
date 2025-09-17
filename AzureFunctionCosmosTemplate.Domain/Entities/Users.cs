using AzureFunctionCosmosTemplate.Domain.DTOs;
using Newtonsoft.Json;

namespace AzureFunctionCosmosTemplate.Domain.Entities;

/// <summary>
/// Users entity for Cosmos DB storage.
/// Contains all internal properties including metadata.
/// </summary>
public class Users : BaseEntity
{
    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the user is active.
    /// </summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets the partition key for this entity.
    /// </summary>
    [JsonProperty("partitionKey")]
    public override string PartitionKey => "users";
}

/// <summary>
/// Extension methods for Users entity mapping.
/// </summary>
public static class UsersExtensions
{
    /// <summary>
    /// Converts a Users entity to a UsersDto.
    /// </summary>
    /// <param name="user">The Users entity to convert.</param>
    /// <returns>A UsersDto with the public properties.</returns>
    public static UsersDto ToDto(this Users user)
    {
        return new UsersDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive
        };
    }

    /// <summary>
    /// Converts a collection of Users entities to a collection of UsersDtos.
    /// </summary>
    /// <param name="users">The Users entities to convert.</param>
    /// <returns>A collection of UsersDtos.</returns>
    public static IEnumerable<UsersDto> ToDto(this IEnumerable<Users> users)
    {
        return users.Select(user => user.ToDto());
    }
}
