using FastEndpoints;
using TagStudio.Tags.Common.Models;
using TagStudio.Tags.Domain;

namespace TagStudio.Tags.Features.Entries;

public class CreateEntryMapper : Mapper<CreateEntryRequest, EntryDetailedDto, Entry>
{
    public override Entry ToEntity(CreateEntryRequest r)
    {
        return new Entry(r.Name)
        {
            Description = r.Description,
            OwnerId = r.UserId
        };
    }

    public override EntryDetailedDto FromEntity(Entry e)
    {
        return new EntryDetailedDto
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            Created = e.Created,
            LastModified = e.LastModified
        };
    }
}