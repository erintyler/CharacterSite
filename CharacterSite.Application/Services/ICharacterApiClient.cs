using CharacterSite.Application.Features.Pronouns.Commands.CreatePronoun;
using CharacterSite.Application.Models.Responses;
using Refit;

namespace CharacterSite.Application.Services;

public interface ICharacterApiClient
{
    [Get("/characters/{id}")]
    Task<CharacterResponse?> GetCharacterByIdAsync(Guid id);

    [Get("/pronouns")]
    Task<IReadOnlyList<PronounResponse>> GetAllPronounsAsync();

    [Get("/pronouns/{id}")]
    Task<PronounResponse?> GetPronounByIdAsync(Guid id);

    [Post("/pronouns")]
    Task<PronounResponse> CreatePronounAsync([Body] CreatePronounCommand request);
}