using CharacterSite.Domain.Entities;
using CharacterSite.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CharacterSite.Infrastructure.Repositories;

public class CharacterRepository(CharacterDbContext context) : ICharacterRepository
{
    public async Task<Character?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Characters
            .Include(c => c.Images)
            .Include(c => c.Pronouns)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public IAsyncEnumerable<Character> GetByCreatedByAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return context.Characters
            .Include(c => c.Images)
            .Include(c => c.Pronouns)
            .Where(c => c.CreatedBy == userId)
            .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<Character> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return context.Characters
            .Include(c => c.Images)
            .Include(c => c.Pronouns)
            .AsAsyncEnumerable();
    }

    public void Add(Character character)
    {
        context.Characters.Add(character);
    }

    public void Update(Character character)
    {
        context.Characters.Update(character);
    }

    public void Delete(Character character)
    {
        context.Characters.Remove(character);
    }
}