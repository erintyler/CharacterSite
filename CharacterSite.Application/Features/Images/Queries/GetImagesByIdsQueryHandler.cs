using CharacterSite.Domain.Common;
using CharacterSite.Domain.Entities;
using CharacterSite.Domain.Repositories;

namespace CharacterSite.Application.Features.Images.Queries;

public class GetImagesByIdsQueryHandler(IImageRepository imageRepository)
{
    public async Task<Result<IReadOnlyList<Image>>> Handle(GetImagesByIdsQuery query,
        CancellationToken cancellationToken)
    {
        if (!query.ImageIds.Any()) return new List<Image>();

        var images = new List<Image>();
        await foreach (var image in imageRepository.GetByIdsAsync(query.ImageIds, cancellationToken)) images.Add(image);

        if (images.Count == query.ImageIds.Count) return images;

        var missingIds = query.ImageIds.Except(images.Select(i => i.Id)).ToHashSet();
        return new Error("Image.NotFound", $"The following image IDs were not found: {string.Join(", ", missingIds)}");
    }
}