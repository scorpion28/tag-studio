using FastEndpoints;
using FluentValidation;

namespace TagStudio.Tags.Features.Entries;

class UpdateEntryRequestValidator : Validator<UpdateEntryRequest>
{
    public UpdateEntryRequestValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotEmpty();
    }
}