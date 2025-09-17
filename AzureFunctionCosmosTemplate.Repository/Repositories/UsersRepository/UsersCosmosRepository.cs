using AzureFunctionCosmosTemplate.Domain.Entities;

namespace AzureFunctionCosmosTemplate.Repository.Repositories.UsersRepository;

/// <summary>
/// Repository implementation for managing Users entities in Cosmos DB.
/// Automatically creates the users container if it doesn't exist.
/// </summary>
public class UsersCosmosRepository : CosmosRepositoryBase<Users>, IUsersCosmosRepository
{
    /// <summary>
    /// Gets the name of the container for users.
    /// </summary>
    protected override string ContainerName => "users";

    /// <summary>
    /// Gets the partition key path for the users container.
    /// </summary>
    protected override string PartitionKeyPath => "/partitionKey";

    /// <summary>
    /// Initializes a new instance of the UsersCosmosRepository.
    /// </summary>
    /// <param name="cosmosDbStorageClient">The Cosmos DB storage client.</param>
    public UsersCosmosRepository(CosmosDbStorageClient cosmosDbStorageClient)
        : base(cosmosDbStorageClient)
    {
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    public async Task<Users?> GetByIdAsync(string id)
    {
        return await GetByIdAsync(id, "users");
    }

    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all users.</returns>
    public async Task<IEnumerable<Users>> GetAllAsync()
    {
        return await GetAllAsync("users");
    }

    /// <summary>
    /// Creates a new user in the database.
    /// </summary>
    /// <param name="user">The user entity to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
    public new async Task<Users> CreateAsync(Users user)
    {
        return await base.CreateAsync(user);
    }

    /// <summary>
    /// Updates an existing user in the database.
    /// </summary>
    /// <param name="user">The user entity with updated information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated user.</returns>
    public new async Task<Users> UpdateAsync(Users user)
    {
        return await base.UpdateAsync(user);
    }

    /// <summary>
    /// Deletes a user from the database by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public async Task DeleteAsync(string id)
    {
        await DeleteAsync(id, "users");
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to find.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    public async Task<Users?> GetByEmailAsync(string email)
    {
        var query = "SELECT * FROM c WHERE c.email = @email AND c.partitionKey = @partitionKey";
        var parameters = new { email, partitionKey = "users" };

        var users = await QueryAsync(query, parameters);
        return users.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves all active users from the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of active users.</returns>
    public async Task<IEnumerable<Users>> GetActiveUsersAsync()
    {
        var query = "SELECT * FROM c WHERE c.isActive = @isActive AND c.partitionKey = @partitionKey";
        var parameters = new { isActive = true, partitionKey = "users" };

        return await QueryAsync(query, parameters);
    }
}
