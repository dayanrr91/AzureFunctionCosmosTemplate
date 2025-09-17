using Newtonsoft.Json;

namespace AzureFunctionCosmosTemplate.Domain.Entities;

public abstract class BaseEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("_etag")]
    public string? ETag { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonProperty("partitionKey")]
    public abstract string PartitionKey { get; }
}
