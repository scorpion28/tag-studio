using FastEndpoints;
using FluentValidation;

namespace TagStudio.Tags.Features.Entries.EntryTag;

public class RemoveEntryTagsRequestValidator : Validator<RemoveEntryTagsRequest>
{
    public RemoveEntryTagsRequestValidator()
    {
        RuleFor(x => x.EntryId)
            .NotEmpty();

        RuleFor(x => x.TagId)
            .NotEmpty();
    }
}