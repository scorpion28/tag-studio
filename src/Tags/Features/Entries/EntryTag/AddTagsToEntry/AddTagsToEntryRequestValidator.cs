using FastEndpoints;
using FluentValidation;

namespace TagStudio.Tags.Features.Entries.EntryTag;

public class AddTagsToEntryRequestValidator : Validator<AddTagsToEntryRequest>
{
    public AddTagsToEntryRequestValidator()
    {
        RuleFor(x => x.EntryId)
            .NotEmpty();

        RuleFor(x => x.TagIds)
            .NotEmpty();
        RuleForEach(x => x.TagIds)
            .NotEmpty();
    }
}