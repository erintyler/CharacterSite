namespace CharacterSite.Application.Features.Characters.Commands;

public record CreateCharacterCommand(
    string Name,
    string? Description,
    IReadOnlyList<Guid> PronounIds,
    IReadOnlyList<Guid> ImageIds);