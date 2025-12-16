using CharacterSite.Domain.Entities;
using CharacterSite.Domain.Repositories;

namespace CharacterSite.Infrastructure.Repositories;

public class PronounRepository(CharacterDbContext context) : IPronounRepository
{
    public void Add(Pronoun pronoun)
    {
        context.Pronouns.Add(pronoun);
    }

    public void Update(Pronoun pronoun)
    {
        context.Pronouns.Update(pronoun);
    }

    public void Delete(Pronoun pronoun)
    {
        context.Pronouns.Remove(pronoun);
    }
}