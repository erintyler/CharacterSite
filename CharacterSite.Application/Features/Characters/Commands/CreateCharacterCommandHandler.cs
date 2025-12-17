using CharacterSite.Application.Features.Images.Queries;
using CharacterSite.Application.Features.Pronouns.Queries;
using CharacterSite.Domain.Common;
using CharacterSite.Domain.Entities;
using CharacterSite.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace CharacterSite.Application.Features.Characters.Commands;

public class CreateCharacterCommandHandler
{
    public async Task<Result<Guid>> Handle(
        CreateCharacterCommand command,
        ICharacterRepository characterRepository,
        IMessageBus bus,
        IUnitOfWork unitOfWork,
        ILogger<CreateCharacterCommandHandler> logger,
        CancellationToken cancellationToken)
    {
        // TODO: Get the actual user ID from the context
        var userId = Guid.NewGuid();
        var characterResult = Character.Create(Guid.NewGuid(), command.Name, command.Description, userId);

        if (characterResult.IsFailure)
        {
            logger.LogError("Failed to create character: {Error}", characterResult.Error);
            return characterResult.Error;
        }

        var character = characterResult.Value;

        var pronounsResult =
            await AddPronounsToCharacter(character, command.PronounIds, bus, logger, userId, cancellationToken);
        if (pronounsResult.IsFailure) return pronounsResult.Error;

        var imagesResult =
            await AddImagesToCharacter(character, command.ImageIds, bus, logger, userId, cancellationToken);
        if (imagesResult.IsFailure) return imagesResult.Error;

        characterRepository.Add(character);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Character {CharacterId} created successfully by user {UserId}", character.Id, userId);
        return character.Id;
    }

    private async Task<Result> AddPronounsToCharacter(
        Character character,
        IEnumerable<Guid> pronounIds,
        IMessageBus bus,
        ILogger logger,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var pronounQuery = new GetPronounsByIdsQuery(pronounIds.ToList());
        var pronounsResult = await bus.InvokeAsync<Result<IReadOnlyList<Pronoun>>>(pronounQuery, cancellationToken);

        if (pronounsResult.IsFailure)
        {
            logger.LogError("Failed to retrieve pronouns: {Error}", pronounsResult.Error);
            return pronounsResult.Error;
        }

        foreach (var pronoun in pronounsResult.Value)
        {
            var result = character.AddPronoun(pronoun, userId);
            if (result.IsFailure)
            {
                logger.LogError("Failed to add pronoun {PronounId} to character {CharacterId}: {Error}", pronoun.Id,
                    character.Id, result.Error);
                return result.Error;
            }
        }

        return Result.Success();
    }

    private async Task<Result> AddImagesToCharacter(
        Character character,
        IEnumerable<Guid> imageIds,
        IMessageBus bus,
        ILogger logger,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var imageQuery = new GetImagesByIdsQuery(imageIds.ToList());
        var imagesResult = await bus.InvokeAsync<Result<IReadOnlyList<Image>>>(imageQuery, cancellationToken);

        if (imagesResult.IsFailure)
        {
            logger.LogError("Failed to retrieve images: {Error}", imagesResult.Error);
            return imagesResult.Error;
        }

        foreach (var image in imagesResult.Value)
        {
            var result = character.AddImage(image, userId);
            if (result.IsFailure)
            {
                logger.LogError("Failed to add image {ImageId} to character {CharacterId}: {Error}", image.Id,
                    character.Id, result.Error);
                return result.Error;
            }
        }

        return Result.Success();
    }
}