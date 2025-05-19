using TagStudio.WebApi.Common.Models;

namespace TagStudio.WebApi.Features.Tags;

public class CreateTagRequest : RequestWithUserId
{
    public string Name { get; init; } = "";
    public List<Guid>? ParentTagsIds { get; init; }
}