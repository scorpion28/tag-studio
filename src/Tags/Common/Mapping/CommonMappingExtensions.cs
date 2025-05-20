using TagStudio.Tags.Common.Constants;
using TagStudio.Tags.Common.Models;

namespace TagStudio.Tags.Common;

public static class CommonMappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable,
        int pageNumber, int pageSize) where TDestination : class =>
        PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize % RequestConstants.MaxItemsPerPage);
}