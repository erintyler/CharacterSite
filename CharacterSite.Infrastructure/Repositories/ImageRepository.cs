using CharacterSite.Domain.Entities;
using CharacterSite.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CharacterSite.Infrastructure.Repositories;

public class ImageRepository(CharacterDbContext context) : IImageRepository
{
    public IAsyncEnumerable<Image> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return context.Images
            .Where(i => ids.Contains(i.Id))
            .AsAsyncEnumerable();
    }

    public void Add(Image image)
    {
        context.Images.Add(image);
    }

    public void Update(Image image)
    {
        context.Images.Update(image);
    }

    public void Delete(Image image)
    {
        context.Images.Remove(image);
    }
}