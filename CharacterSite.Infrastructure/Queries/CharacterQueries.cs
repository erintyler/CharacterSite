using CharacterSite.Application.Features.Characters.Queries;
using CharacterSite.Application.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace CharacterSite.Infrastructure.Queries;

public class CharacterQueries(CharacterDbContext context) : ICharacterQueries
{
    public async Task<CharacterResponse?> GetCharacterByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Characters
            .AsSplitQuery()
            .Where(c => c.Id == id)
            .Select(c => new CharacterResponse(
                c.Id,
                c.Name,
                c.Description,
                c.Images
                    .Select(i => new ImageResponse(i.Id, i.Name))
                    .ToList(),
                c.Pronouns
                    .Select(p => new PronounResponse(p.Id, p.Subject, p.Object, p.Possessive))
                    .ToList(),
                new UserResponse(c.CreatedBy, "meow")
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}