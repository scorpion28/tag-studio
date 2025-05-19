using System.Security.Claims;
using System.Text.Json.Serialization;
using FastEndpoints;

namespace TagStudio.WebApi.Common.Models;

/// <summary>
/// Base class for queries that require a user id. The id is derived from the claims
/// </summary>
public class RequestWithUserId
{
    [JsonIgnore]
    [FromClaim(ClaimTypes.NameIdentifier, removeFromSchema: true)]
    public Guid UserId { get; set; }
}