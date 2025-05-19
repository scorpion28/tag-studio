using TagStudio.WebApi.Domain.Exceptions;

// ReSharper disable once CheckNamespace
namespace Ardalis.GuardClauses;

public static class GuardClausesExtensions
{
    /// <summary>
    /// Throws an <see cref="ForbiddenAccessException" /> if <paramref name="entityOwnerId"/> is not equal to <paramref name="userId"/>.
    /// </summary>
    /// <param name="guardClause"></param>
    /// <param name="entityOwnerId"></param>
    /// <param name="userId"></param>
    /// <exception cref="NotFoundException"></exception>
    public static void Forbidden(this IGuardClause guardClause, Guid entityOwnerId, Guid userId)
    {
        guardClause.Null(entityOwnerId, nameof(entityOwnerId));
        guardClause.Null(userId, nameof(userId));

        ForbiddenAccessException.ThrowIfForbidden(entityOwnerId, userId);
    }
}