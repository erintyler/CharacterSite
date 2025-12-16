using CharacterSite.Domain.Entities;

namespace CharacterSite.Domain.Repositories;

public interface IImageRepository
{
    void Add(Image image);
    void Update(Image image);
    void Delete(Image image);
}