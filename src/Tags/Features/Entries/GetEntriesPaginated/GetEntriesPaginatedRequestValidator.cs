using FastEndpoints;
using FluentValidation;

namespace TagStudio.Tags.Features.Entries;

public class GetEntriesPaginatedRequestValidator : Validator<GetEntriesPaginatedRequest>
{
    public GetEntriesPaginatedRequestValidator()
    {
        When(x => x.PageNumber != null, () =>
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
        });
    }
}