using CharacterSite.Application.Features.Characters.Queries.GetCharacterById;

namespace CharacterSite.Application.Features.Characters.Queries;

public interface ICharacterQueries
{
    Task<CharacterResponse?> GetCharacterByIdAsync(Guid id, CancellationToken cancellationToken = default);
}