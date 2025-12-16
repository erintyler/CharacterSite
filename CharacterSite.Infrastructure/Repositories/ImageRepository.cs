using CharacterSite.Domain.Entities;
using CharacterSite.Domain.Repositories;

namespace CharacterSite.Infrastructure.Repositories;

public class ImageRepository(CharacterDbContext context) : IImageRepository
{
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