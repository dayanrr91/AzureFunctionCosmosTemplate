using Newtonsoft.Json;

namespace AzureFunctionCosmosTemplate.Domain.Entities;

public class Users : BaseEntity
{
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("isActive")]
    public bool IsActive { get; set; } = true;

    [JsonProperty("partitionKey")]
    public override string PartitionKey => "users";
}
