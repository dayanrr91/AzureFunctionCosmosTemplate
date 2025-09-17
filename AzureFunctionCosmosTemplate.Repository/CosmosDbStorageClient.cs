using Microsoft.Azure.Cosmos;

namespace AzureFunctionCosmosTemplate.Repository;

/// <summary>
/// Client for managing Cosmos DB operations including database and container creation.
/// Automatically creates the database if it doesn't exist during initialization.
/// </summary>
public class CosmosDbStorageClient
{
    private readonly CosmosClient _cosmosClient;
    private readonly Database _database;
    private readonly string _databaseName;

    /// <summary>
    /// Initializes a new instance of the CosmosDbStorageClient.
    /// Automatically creates the database if it doesn't exist.
    /// </summary>
    /// <param name="connectionString">The Cosmos DB connection string.</param>
    /// <param name="databaseName">The name of the database to connect to or create.</param>
    public CosmosDbStorageClient(string connectionString, string databaseName)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));
        }

        if (string.IsNullOrEmpty(databaseName))
        {
            throw new ArgumentException("Database name cannot be null or empty", nameof(databaseName));
        }

        _cosmosClient = new CosmosClient(connectionString);
        _databaseName = databaseName;

        // Create database if it doesn't exist
        _database = CreateDatabaseIfNotExistsAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Creates the database if it doesn't exist.
    /// </summary>
    /// <returns>The database instance.</returns>
    private async Task<Database> CreateDatabaseIfNotExistsAsync()
    {
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);
        return databaseResponse.Database;
    }

    /// <summary>
    /// Gets a reference to an existing container.
    /// Note: This method assumes the container already exists. Use GetContainerAsync for automatic creation.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <returns>A container reference.</returns>
    public Container GetContainer(string containerName)
    {
        return _database.GetContainer(containerName);
    }

    /// <summary>
    /// Gets a container reference, creating it if it doesn't exist.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="partitionKeyPath">The partition key path for the container.</param>
    /// <param name="throughput">Optional throughput setting for the container.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the container reference.</returns>
    public async Task<Container> GetContainerAsync(string containerName, string partitionKeyPath, int? throughput = null)
    {
        var containerResponse = await _database.CreateContainerIfNotExistsAsync(containerName, partitionKeyPath, throughput);
        return containerResponse.Container;
    }

    /// <summary>
    /// Creates a container if it doesn't exist.
    /// </summary>
    /// <param name="containerName">The name of the container to create.</param>
    /// <param name="partitionKeyPath">The partition key path for the container.</param>
    /// <param name="throughput">Optional throughput setting for the container.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the container reference.</returns>
    public async Task<Container> CreateContainerIfNotExistsAsync(string containerName, string partitionKeyPath, int? throughput = null)
    {
        return await _database.CreateContainerIfNotExistsAsync(containerName, partitionKeyPath, throughput);
    }

    /// <summary>
    /// Disposes the Cosmos DB client and releases associated resources.
    /// </summary>
    public void Dispose()
    {
        _cosmosClient?.Dispose();
    }
}
