using Riok.Mapperly.Abstractions;
using TagStudio.WebApi.Domain;
using TagStudio.WebApi.Features.Tags;

namespace TagStudio.WebApi.Common;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class TagMapper
{
    public static partial TagBriefDto ToTagBriefDto(this Tag tag);

    public static partial TagDetailedDto ToTagDetailedDto(this Tag tag);

    public static partial List<TagBriefDto> ToTagBriefDtoList(this List<Tag> collection);

    public static partial IQueryable<TagBriefDto> ProjectToTagBriefDto(this IQueryable<Tag> tags);
}