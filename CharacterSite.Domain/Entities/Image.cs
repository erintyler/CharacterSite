using CharacterSite.Domain.Common;
using CharacterSite.Domain.Enums;
using CharacterSite.Domain.Primitives;

namespace CharacterSite.Domain.Entities;

public class Image : Entity
{
    internal Image(Guid id, Guid characterId, string name, UploadStatus status) : base(id)
    {
        CharacterId = characterId;
        Name = name;
        Status = status;
    }
    
    private Image()
    {
    }
    
    public Guid CharacterId { get; private set; }
    public string Name { get; private set; }
    public UploadStatus Status { get; private set; }

    public Result SetProcessing()
    {
        if (Status is not UploadStatus.Pending)
        {
            return new Error("Image.SetProcessing.InvalidStatus", "Can only set image to processing from pending status.");
        }
        
        Status = UploadStatus.Processing;
        
        return Result.Success();
    }
    
    public Result SetCompleted()
    {
        if (Status is not UploadStatus.Processing)
        {
            return new Error("Image.SetCompleted.InvalidStatus", "Can only set image to completed from processing status.");
        }
        
        Status = UploadStatus.Completed;
        
        return Result.Success();
    }
    
    public Result SetFailed()
    {
        if (Status is UploadStatus.Completed)
        {
            return new Error("Image.SetFailed.InvalidStatus", "Cannot set image to failed from completed status.");
        }
        
        Status = UploadStatus.Failed;
        
        return Result.Success();
    }
}