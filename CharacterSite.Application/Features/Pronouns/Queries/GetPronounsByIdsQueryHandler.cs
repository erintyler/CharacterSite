using CharacterSite.Domain.Common;
using CharacterSite.Domain.Entities;
using CharacterSite.Domain.Repositories;

namespace CharacterSite.Application.Features.Pronouns.Queries;

public class GetPronounsByIdsQueryHandler(IPronounRepository pronounRepository)
{
    public async Task<Result<IReadOnlyList<Pronoun>>> Handle(GetPronounsByIdsQuery query,
        CancellationToken cancellationToken)
    {
        if (!query.PronounIds.Any()) return new List<Pronoun>();

        var pronouns = new List<Pronoun>();
        await foreach (var pronoun in pronounRepository.GetByIdsAsync(query.PronounIds, cancellationToken))
            pronouns.Add(pronoun);

        if (pronouns.Count == query.PronounIds.Count) return pronouns;

        var missingIds = query.PronounIds.Except(pronouns.Select(p => p.Id)).ToHashSet();
        return new Error("Pronoun.NotFound",
            $"The following pronoun IDs were not found: {string.Join(", ", missingIds)}");
    }
}