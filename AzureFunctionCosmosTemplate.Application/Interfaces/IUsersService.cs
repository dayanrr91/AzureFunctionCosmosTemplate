using AzureFunctionCosmosTemplate.Domain.Entities;

namespace AzureFunctionCosmosTemplate.Application.Interfaces;

/// <summary>
/// Service interface for managing user business logic operations.
/// Provides high-level methods for user management including CRUD operations and specialized queries.
/// </summary>
public interface IUsersService
{
    /// <summary>
    /// Retrieves all users from the system.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all users.</returns>
    Task<IEnumerable<Users>> GetAllUsersAsync();

    /// <summary>
    /// Retrieves a specific user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    Task<Users?> GetUserByIdAsync(string id);

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="user">The user entity to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the newly created user.</returns>
    Task<Users> CreateUserAsync(Users user);

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="user">The user entity with updated information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated user.</returns>
    Task<Users> UpdateUserAsync(Users user);

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
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    Task<Users?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Retrieves all active users from the system.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of active users.</returns>
    Task<IEnumerable<Users>> GetActiveUsersAsync();
}
