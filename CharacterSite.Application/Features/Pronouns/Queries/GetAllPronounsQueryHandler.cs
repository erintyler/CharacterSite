using CharacterSite.Application.Models.Responses;
using CharacterSite.Domain.Common;

namespace CharacterSite.Application.Features.Pronouns.Queries;

public class GetAllPronounsQueryHandler
{
    public async Task<Result<IReadOnlyList<PronounResponse>>> Handle(GetAllPronounsQuery query, IPronounQueries queries,
        CancellationToken cancellationToken)
    {
        var pronouns = await queries.GetAllPronounsAsync(cancellationToken);

        if (pronouns.Count == 0)
        {
            return new Error("Pronoun.NoneFound", "No pronouns were found.");
        }

        return Result.Success(pronouns);
    }
}