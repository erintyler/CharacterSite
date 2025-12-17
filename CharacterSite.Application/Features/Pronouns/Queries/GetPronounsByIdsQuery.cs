namespace CharacterSite.Application.Features.Pronouns.Queries;

public record GetPronounsByIdsQuery(IReadOnlyList<Guid> PronounIds);