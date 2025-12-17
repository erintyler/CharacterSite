namespace CharacterSite.Application.Models.Responses;

public record CharacterResponse(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyList<ImageResponse> Images,
    IReadOnlyList<PronounResponse> Pronouns,
    UserResponse CreatedBy);