using CharacterSite.Application.Models.Responses;
using CharacterSite.Domain.Common;

namespace CharacterSite.Application.Features.Pronouns.Queries.GetPronounById;

public class GetPronounByIdQueryHandler
{
    public async Task<Result<PronounResponse>> Handle(GetPronounByIdQuery query, IPronounQueries queries, CancellationToken cancellationToken)
    {
        var pronoun = await queries.GetPronounByIdAsync(query.Id, cancellationToken);

        if (pronoun is null)
        {
            return new Error("Pronoun.NotFound", $"Pronoun with ID {query.Id} was not found.");
        }

        return pronoun;
    }
}