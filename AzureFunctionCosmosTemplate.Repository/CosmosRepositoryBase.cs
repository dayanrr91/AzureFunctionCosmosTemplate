using AzureFunctionCosmosTemplate.Domain.Entities;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace AzureFunctionCosmosTemplate.Repository;

/// <summary>
/// Base repository class for Cosmos DB operations.
/// Provides common CRUD operations and automatically creates containers if they don't exist.
/// </summary>
/// <typeparam name="T">The entity type that extends BaseEntity.</typeparam>
public abstract class CosmosRepositoryBase<T> where T : BaseEntity
{
    protected readonly Container _container;

    /// <summary>
    /// Gets the name of the container for this repository.
    /// </summary>
    protected abstract string ContainerName { get; }

    /// <summary>
    /// Gets the partition key path for the container (e.g., "/partitionKey").
    /// </summary>
    protected abstract string PartitionKeyPath { get; }

    /// <summary>
    /// Gets the throughput setting for the container. Override to specify custom throughput.
    /// </summary>
    protected virtual int? Throughput => null;

    /// <summary>
    /// Initializes a new instance of the repository.
    /// Automatically creates the container if it doesn't exist.
    /// </summary>
    /// <param name="cosmosDbStorageClient">The Cosmos DB storage client.</param>
    protected CosmosRepositoryBase(CosmosDbStorageClient cosmosDbStorageClient)
    {
        // Create container if it doesn't exist and get reference
        _container = InitializeContainerAsync(cosmosDbStorageClient).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Initializes the container, creating it if it doesn't exist.
    /// </summary>
    /// <param name="cosmosDbStorageClient">The Cosmos DB storage client.</param>
    /// <returns>The container reference.</returns>
    private async Task<Container> InitializeContainerAsync(CosmosDbStorageClient cosmosDbStorageClient)
    {
        return await cosmosDbStorageClient.GetContainerAsync(ContainerName, PartitionKeyPath, Throughput);
    }

    /// <summary>
    /// Retrieves an entity by its unique identifier and partition key.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="partitionKey">The partition key value.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found; otherwise, null.</returns>
    public virtual async Task<T?> GetByIdAsync(string id, string partitionKey)
    {
        try
        {
            var response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <summary>
    /// Retrieves all entities within a specific partition.
    /// </summary>
    /// <param name="partitionKey">The partition key value to filter by.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of entities.</returns>
    public virtual async Task<IEnumerable<T>> GetAllAsync(string partitionKey)
    {
        var query = _container.GetItemQueryIterator<T>(
            new QueryDefinition("SELECT * FROM c WHERE c.partitionKey = @partitionKey")
                .WithParameter("@partitionKey", partitionKey));

        var results = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    /// <summary>
    /// Creates a new entity in the container.
    /// Automatically sets CreatedAt and UpdatedAt timestamps.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created entity.</returns>
    public virtual async Task<T> CreateAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        var response = await _container.CreateItemAsync(entity, new PartitionKey(entity.PartitionKey));
        return response.Resource;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;

        var response = await _container.ReplaceItemAsync(entity, entity.Id, new PartitionKey(entity.PartitionKey));
        return response.Resource;
    }

    public virtual async Task<T> UpsertAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        if (string.IsNullOrEmpty(entity.Id))
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.CreatedAt = DateTime.UtcNow;
        }

        var response = await _container.UpsertItemAsync(entity, new PartitionKey(entity.PartitionKey));
        return response.Resource;
    }

    public virtual async Task DeleteAsync(string id, string partitionKey)
    {
        await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
    }

    public virtual async Task<bool> ExistsAsync(string id, string partitionKey)
    {
        try
        {
            await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public virtual async Task<int> CountAsync(string partitionKey)
    {
        var query = _container.GetItemQueryIterator<int>(
            new QueryDefinition("SELECT VALUE COUNT(1) FROM c WHERE c.partitionKey = @partitionKey")
                .WithParameter("@partitionKey", partitionKey));

        var response = await query.ReadNextAsync();
        return response.FirstOrDefault();
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync(string partitionKey, int pageSize = 10, string? continuationToken = null)
    {
        var queryOptions = new QueryRequestOptions
        {
            MaxItemCount = pageSize
        };

        var query = _container.GetItemQueryIterator<T>(
            new QueryDefinition("SELECT * FROM c WHERE c.partitionKey = @partitionKey")
                .WithParameter("@partitionKey", partitionKey),
            continuationToken,
            queryOptions);

        var results = new List<T>();
        if (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    public virtual async Task<IEnumerable<T>> QueryAsync(string query, object? parameters = null)
    {
        var queryDefinition = new QueryDefinition(query);

        if (parameters != null)
        {
            var properties = parameters.GetType().GetProperties();
            foreach (var property in properties)
            {
                queryDefinition.WithParameter($"@{property.Name}", property.GetValue(parameters));
            }
        }

        var queryIterator = _container.GetItemQueryIterator<T>(queryDefinition);
        var results = new List<T>();

        while (queryIterator.HasMoreResults)
        {
            var response = await queryIterator.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }
}
