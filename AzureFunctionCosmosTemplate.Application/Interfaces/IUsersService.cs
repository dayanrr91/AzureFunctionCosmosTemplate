using AzureFunctionCosmosTemplate.Domain.DTOs;

namespace AzureFunctionCosmosTemplate.Application.Interfaces;

/// <summary>
/// Service interface for managing user business logic operations.
/// Provides high-level methods for user management including CRUD operations and specialized queries.
/// Works with DTOs to maintain proper separation between internal entities and external contracts.
/// </summary>
public interface IUsersService
{
    /// <summary>
    /// Retrieves all users from the system.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all users as DTOs.</returns>
    Task<IEnumerable<UsersDto>> GetAllUsersAsync();

    /// <summary>
    /// Retrieves a specific user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user DTO if found; otherwise, null.</returns>
    Task<UsersDto?> GetUserByIdAsync(string id);

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="userDto">The user DTO with the data to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the newly created user as a DTO.</returns>
    Task<UsersDto> CreateUserAsync(UsersDto userDto);

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="userDto">The user DTO with updated information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated user as a DTO.</returns>
    Task<UsersDto> UpdateUserAsync(string id, UsersDto userDto);

    /// <summary>
    /// Deletes a user from the system by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteUserAsync(string id);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to find.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user DTO if found; otherwise, null.</returns>
    Task<UsersDto?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Retrieves all active users from the system.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of active users as DTOs.</returns>
    Task<IEnumerable<UsersDto>> GetActiveUsersAsync();
}
