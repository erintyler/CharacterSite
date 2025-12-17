using CharacterSite.Application.Features.Pronouns.Commands.CreatePronoun;
using CharacterSite.Application.Services;

namespace CharacterSite.Web.Endpoints;

public static class CharacterApiEndpoints
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public void MapCharacterApiEndpoints()
        {
            var charactersGroup = endpoints.MapGroup("/api/characters");

            charactersGroup.MapGet("/{id:guid}", async (Guid id, ICharacterApiClient characterApiClient) =>
            {
                var character = await characterApiClient.GetCharacterByIdAsync(id);

                return character is not null ? Results.Ok(character) : Results.NotFound();
            });

            var pronounsGroup = endpoints.MapGroup("/api/pronouns");

            pronounsGroup.MapGet("/", async (ICharacterApiClient characterApiClient) =>
            {
                var pronouns = await characterApiClient.GetAllPronounsAsync();
                return Results.Ok(pronouns);
            });

            pronounsGroup.MapPost("/", async (CreatePronounCommand command, ICharacterApiClient characterApiClient) =>
            {
                var pronoun = await characterApiClient.CreatePronounAsync(command);
                return Results.Created($"/api/pronouns/{pronoun.Id}", pronoun);
            });

            pronounsGroup.MapGet("/{id:guid}", async (Guid id, ICharacterApiClient characterApiClient) =>
            {
                var pronoun = await characterApiClient.GetPronounByIdAsync(id);
                return pronoun is not null ? Results.Ok(pronoun) : Results.NotFound();
            });
        }
    }
}