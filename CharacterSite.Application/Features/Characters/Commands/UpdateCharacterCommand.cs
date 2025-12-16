namespace CharacterSite.Application.Features.Characters.Commands;

public record UpdateCharacterCommand(
    Guid CharacterId,
    string Name,
    string? Description,
    List<PronounDto> Pronouns);
    
public record PronounDto(string Subject, string Object, string Possessive);