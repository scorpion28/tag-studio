using FastEndpoints;
using FluentValidation;

namespace TagStudio.WebApi.Features.Tags;

public class CreateTagRequestValidator : Validator<CreateTagRequest>
{
    public CreateTagRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty")
            .MaximumLength(100);

        When(x => x.ParentTagsIds != null, () =>
        {
            RuleForEach(x => x.ParentTagsIds).NotEmpty();
        });
    }
}