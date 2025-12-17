using CharacterSite.Application.Features.Pronouns.Queries;
using CharacterSite.Application.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace CharacterSite.Infrastructure.Queries;

public class PronounQueries(CharacterDbContext context) : IPronounQueries
{
    public async Task<PronounResponse?> GetPronounByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Pronouns
            .Where(p => p.Id == id)
            .Select(p => new PronounResponse(p.Id, p.Subject, p.Object, p.Possessive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PronounResponse>> GetAllPronounsAsync(CancellationToken cancellationToken = default)
    {
        return await context.Pronouns
            .Select(p => new PronounResponse(p.Id, p.Subject, p.Object, p.Possessive))
            .ToListAsync(cancellationToken);
    }
}