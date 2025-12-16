using CharacterSite.Application.Services;
using CharacterSite.Domain.DomainEvents.Character;
using Microsoft.Extensions.Logging;

namespace CharacterSite.Application.Features.Characters.DomainEvents;

public class ImageRemovedDomainEventHandler(ILogger<ImageRemovedDomainEventHandler> logger)
{
    public async Task Handle(ImageRemovedDomainEvent evt, IImageStorageService storageService, CancellationToken cancellationToken)
    {
        var result = await storageService.DeleteAsync(evt.ImageId, cancellationToken);
        
        if (result.IsSuccess)
        {
            logger.LogInformation("Image with ID {ImageId} was successfully deleted from storage.", evt.ImageId);
        }
        else
        {
            logger.LogError("Failed to delete image with ID {ImageId} from storage. Error: {Error}", evt.ImageId, result.Error);
        }
    }
}