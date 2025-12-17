using CharacterSite.Application.Models.Responses;

namespace CharacterSite.Application.Features.Characters.Queries;

public interface ICharacterQueries
{
    Task<CharacterResponse?> GetCharacterByIdAsync(Guid id, CancellationToken cancellationToken = default);
}