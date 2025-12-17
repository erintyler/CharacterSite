namespace CharacterSite.Application.Features.Images.Queries;

public record GetImagesByIdsQuery(IReadOnlyList<Guid> ImageIds);