using AzureFunctionCosmosTemplate.Domain.Entities;
using AzureFunctionCosmosTemplate.Repository.Repositories.UsersRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFunctionCosmosTemplate.Repository.Extensions;

/// <summary>
/// Extension methods for configuring Cosmos DB services in the dependency injection container.
/// Automatically creates databases and containers if they don't exist.
/// </summary>
public static class ServiceCollectionExtensions
{
    private static CosmosDbStorageClient? _cosmosDbStorageClientGeneric;

    /// <summary>
    /// Gets or creates a singleton instance of the Cosmos DB client.
    /// Automatically creates the database if it doesn't exist.
    /// </summary>
    /// <param name="dbConnection">The Cosmos DB connection string.</param>
    /// <param name="dbName">The name of the database.</param>
    /// <returns>The Cosmos DB storage client instance.</returns>
    private static CosmosDbStorageClient GetOrCreateCosmosClient(string dbConnection, string dbName)
    {
        _cosmosDbStorageClientGeneric ??= new CosmosDbStorageClient(dbConnection, dbName);
        return _cosmosDbStorageClientGeneric;
    }

    /// <summary>
    /// Registers a generic Cosmos DB repository in the dependency injection container.
    /// Automatically creates the database and container if they don't exist.
    /// </summary>
    /// <typeparam name="TEntity">The entity type that extends BaseEntity.</typeparam>
    /// <typeparam name="TRepository">The repository implementation type.</typeparam>
    /// <typeparam name="TInterface">The repository interface type.</typeparam>
    /// <param name="services">The service collection to add the repository to.</param>
    /// <returns>The service collection for method chaining.</returns>
    private static IServiceCollection AddGenericCosmosRepository<TEntity, TRepository, TInterface>(
        this IServiceCollection services)
        where TEntity : BaseEntity
        where TRepository : CosmosRepositoryBase<TEntity>, TInterface
        where TInterface : class
    {
        services.AddSingleton<TInterface>(provider =>
        {
            var conf = provider.GetRequiredService<IConfiguration>();
            var dbConnection = conf["CosmosDbConnection"];
            var dbName = conf["CosmosDbName"];

            if (string.IsNullOrEmpty(dbConnection))
            {
                throw new InvalidOperationException("CosmosDbConnection configuration is missing");
            }

            if (string.IsNullOrEmpty(dbName))
            {
                throw new InvalidOperationException("CosmosDbName configuration is missing");
            }

            var cosmosClient = GetOrCreateCosmosClient(dbConnection, dbName);
            return (TRepository)Activator.CreateInstance(typeof(TRepository), cosmosClient)!;
        });

        return services;
    }

    /// <summary>
    /// Registers all Cosmos DB dependencies including repositories and services.
    /// Automatically creates databases and containers if they don't exist.
    /// </summary>
    /// <param name="services">The service collection to add dependencies to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddAllCosmosDependencies(this IServiceCollection services)
    {
        // Register Users Repository
        services.AddGenericCosmosRepository<Users, UsersCosmosRepository, IUsersCosmosRepository>();

        return services;
    }

    /// <summary>
    /// Registers a specific Cosmos DB repository in the dependency injection container.
    /// Automatically creates the database and container if they don't exist.
    /// </summary>
    /// <typeparam name="TEntity">The entity type that extends BaseEntity.</typeparam>
    /// <typeparam name="TRepository">The repository implementation type.</typeparam>
    /// <typeparam name="TInterface">The repository interface type.</typeparam>
    /// <param name="services">The service collection to add the repository to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddCosmosRepository<TEntity, TRepository, TInterface>(
        this IServiceCollection services)
        where TEntity : BaseEntity
        where TRepository : CosmosRepositoryBase<TEntity>, TInterface
        where TInterface : class
    {
        return services.AddGenericCosmosRepository<TEntity, TRepository, TInterface>();
    }
}
