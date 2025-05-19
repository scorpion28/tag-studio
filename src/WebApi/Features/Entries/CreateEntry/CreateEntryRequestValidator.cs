using FastEndpoints;
using FluentValidation;

namespace TagStudio.WebApi.Features.Entries;

public class CreateEntryRequestValidator : Validator<CreateEntryRequest>
{
    public CreateEntryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty");
    }
}