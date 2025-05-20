using TagStudio.Tags.Common.Models;

namespace TagStudio.Tags.Features.Tags;

public class CreateTagRequest : RequestWithUserId
{
    public string Name { get; init; } = "";
    public List<Guid>? ParentTagsIds { get; init; }
}