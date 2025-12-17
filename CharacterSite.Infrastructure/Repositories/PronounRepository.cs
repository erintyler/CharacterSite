using CharacterSite.Domain.Entities;
using CharacterSite.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CharacterSite.Infrastructure.Repositories;

public class PronounRepository(CharacterDbContext context) : IPronounRepository
{
    public Task<bool> ExistsAsync(string subject, string @object, string possessive,
        CancellationToken cancellationToken = default)
    {
        return context.Pronouns.AnyAsync(p =>
            p.Subject == subject &&
            p.Object == @object &&
            p.Possessive == possessive, cancellationToken);
    }

    public IAsyncEnumerable<Pronoun> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return context.Pronouns
            .Where(p => ids.Contains(p.Id))
            .AsAsyncEnumerable();
    }

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