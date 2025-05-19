using TagStudio.WebApi.Common.Constants;
using TagStudio.WebApi.Common.Models;

namespace TagStudio.WebApi.Common;

public static class CommonMappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable,
        int pageNumber, int pageSize) where TDestination : class =>
        PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize % RequestConstants.MaxItemsPerPage);
}