namespace TagStudio.Tags.Common.Models.Tags;

public class TagDetailedDto : TagBriefDto
{
    public List<TagBriefDto>? Parents { get; init; }
    public List<TagBriefDto>? Children { get; init; }

    public DateTimeOffset Created { get; init; }
    public DateTimeOffset LastModified { get; init; }
}