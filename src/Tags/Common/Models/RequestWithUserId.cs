using System.Security.Claims;
using System.Text.Json.Serialization;
using FastEndpoints;
using NSwag.Annotations;

namespace TagStudio.Tags.Common.Models;

/// <summary>
/// Base class for queries that require a user id. The id is derived from the claims
/// </summary>
[OpenApiIgnore]
public class RequestWithUserId
{
    [JsonIgnore]
    [FromClaim(ClaimTypes.NameIdentifier, removeFromSchema: true)]
    public Guid UserId { get; set; }
}