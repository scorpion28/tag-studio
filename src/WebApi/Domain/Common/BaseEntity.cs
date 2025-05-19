using System.Text.Json.Serialization;

namespace TagStudio.WebApi.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    
    [JsonIgnore]
    public Guid OwnerId { get; init; }
}