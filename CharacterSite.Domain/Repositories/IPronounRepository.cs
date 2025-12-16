using CharacterSite.Domain.Entities;

namespace CharacterSite.Domain.Repositories;

public interface IPronounRepository
{
    void Add(Pronoun pronoun);
    void Update(Pronoun pronoun);
    void Delete(Pronoun pronoun);
}