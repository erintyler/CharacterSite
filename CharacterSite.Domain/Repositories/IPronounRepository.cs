using CharacterSite.Domain.Entities;

namespace CharacterSite.Domain.Repositories;

public interface IPronounRepository
{
    Task<bool> ExistsAsync(string subject, string @object, string possessive,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<Pronoun> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    void Add(Pronoun pronoun);
    void Update(Pronoun pronoun);
    void Delete(Pronoun pronoun);
}