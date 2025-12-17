using CharacterSite.Application.Models.Responses;
using CharacterSite.Domain.Common;

namespace CharacterSite.Application.Features.Characters.Queries;

public class GetCharacterByIdQueryHandler
{
    public async Task<Result<CharacterResponse>> Handle(GetCharacterByIdQuery query, ICharacterQueries queries,
        CancellationToken cancellationToken)
    {
        var character = await queries.GetCharacterByIdAsync(query.Id, cancellationToken);

        if (character is null)
        {
            return new Error("Character.NotFound", $"Character with ID {query.Id} was not found.");
        }

        return character;
    }
}