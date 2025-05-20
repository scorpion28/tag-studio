using Riok.Mapperly.Abstractions;
using TagStudio.Tags.Common.Models;
using TagStudio.Tags.Domain;
using TagStudio.Tags.Features.Entries;

namespace TagStudio.Tags.Common;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class EntryMapper
{
    public static partial EntryDetailedDto ToEntryDetailedDto(this Entry tag);
    
    public static partial IQueryable<EntryBriefDto> ProjectToEntryBriefDto(this IQueryable<Entry> query);
}