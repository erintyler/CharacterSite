using CharacterSite.Application.Models.Responses;

namespace CharacterSite.Application.Features.Pronouns.Queries;

public interface IPronounQueries
{
    Task<PronounResponse?> GetPronounByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PronounResponse>> GetAllPronounsAsync(CancellationToken cancellationToken = default);
}