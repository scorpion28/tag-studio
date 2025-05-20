using FastEndpoints;
using FluentValidation;

namespace TagStudio.Tags.Features.Tags;

public class UpdateTagRequestValidator : Validator<UpdateTagRequest>
{
    public UpdateTagRequestValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        When(x => x.ParentTagsIds != null, () =>
        {
            RuleForEach(x => x.ParentTagsIds).NotEmpty();
        });
    }
}