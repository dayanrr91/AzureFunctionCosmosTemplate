using AzureFunctionCosmosTemplate.Domain.Entities;

namespace AzureFunctionCosmosTemplate.Repository.Repositories.UsersRepository;

/// <summary>
/// Repository interface for managing Users entities in Cosmos DB.
/// Provides CRUD operations and specialized query methods for user data.
/// </summary>
public interface IUsersCosmosRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    Task<Users?> GetByIdAsync(string id);

    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all users.</returns>
    Task<IEnumerable<Users>> GetAllAsync();

    /// <summary>
    /// Creates a new user in the database.
    /// </summary>
    /// <param name="user">The user entity to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
    Task<Users> CreateAsync(Users user);

    /// <summary>
    /// Updates an existing user in the database.
    /// </summary>
    /// <param name="user">The user entity with updated information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated user.</returns>
    Task<Users> UpdateAsync(Users user);

    /// <summary>
    /// Deletes a user from the database by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(string id);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to find.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    Task<Users?> GetByEmailAsync(string email);

    /// <summary>
    /// Retrieves all active users from the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of active users.</returns>
    Task<IEnumerable<Users>> GetActiveUsersAsync();
}
