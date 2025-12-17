namespace CharacterSite.Application.Models.Responses;

public record PronounResponse(Guid Id, string Subject, string Object, string Possessive);