namespace CharacterSite.Application.Features.Characters.Queries.GetCharacterById;

public record CharacterResponse(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyList<ImageResponse> Images,
    IReadOnlyList<PronounResponse> Pronouns,
    UserResponse CreatedBy);