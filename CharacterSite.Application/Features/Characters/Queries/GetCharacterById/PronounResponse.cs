namespace CharacterSite.Application.Features.Characters.Queries.GetCharacterById;

public record PronounResponse(Guid Id, string Subject, string Object, string Possessive);