using Riok.Mapperly.Abstractions;
using TagStudio.Tags.Common.Models.Tags;
using TagStudio.Tags.Domain;

// ReSharper disable once CheckNamespace
namespace TagStudio.Tags.Common;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class TagMapper
{
    public static partial TagBriefDto ToTagBriefDto(this Tag tag);

    public static partial TagDetailedDto ToTagDetailedDto(this Tag tag);

    public static partial List<TagBriefDto> ToTagBriefDtoList(this List<Tag> collection);

    public static partial IQueryable<TagDetailedDto> ProjectToTagDetailedDto(this IQueryable<Tag> tags);
}