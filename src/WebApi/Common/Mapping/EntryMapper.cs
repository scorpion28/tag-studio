using Riok.Mapperly.Abstractions;
using TagStudio.WebApi.Domain;
using TagStudio.WebApi.Features.Entries;

namespace TagStudio.WebApi.Common;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class EntryMapper
{
    public static partial EntryDetailedDto ToEntryDetailedDto(this Entry tag);
    
    public static partial IQueryable<EntryBriefDto> ProjectToEntryBriefDto(this IQueryable<Entry> query);
}