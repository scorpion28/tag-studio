using TagStudio.Tags.Common.Models;

namespace TagStudio.Tags.Features.Search;

public class SearchRequest : RequestWithUserId
{
    public string Query { get; set; }
}