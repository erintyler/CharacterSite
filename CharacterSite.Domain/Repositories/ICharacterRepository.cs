using CharacterSite.Domain.Entities;

namespace CharacterSite.Domain.Repositories;

public interface ICharacterRepository
{
    Task<Character?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Character> GetByCreatedByAsync(Guid userId, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Character> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(Character character);
    void Update(Character character);
    void Delete(Character character);
}