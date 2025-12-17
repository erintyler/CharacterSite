using CharacterSite.Domain.Entities;

namespace CharacterSite.Domain.Repositories;

public interface IImageRepository
{
    IAsyncEnumerable<Image> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    void Add(Image image);
    void Update(Image image);
    void Delete(Image image);
}