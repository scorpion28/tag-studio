namespace TagStudio.Tags.Domain.Exceptions;

public class ForbiddenAccessException : Exception
{
    public static void ThrowIfForbidden(Guid entityOwnerId, Guid userId)
    {
        if (entityOwnerId != userId)
        {
            throw new ForbiddenAccessException();
        }
    }
}