using CharacterSite.Domain.Common;
using CharacterSite.Domain.Enums;
using CharacterSite.Domain.Primitives;

namespace CharacterSite.Domain.Entities;

public class Character : AggregateRoot, IAuditableEntity
{
    public const int NameMaxLength = 200;
    public const int DescriptionMaxLength = 5000;
    
    private readonly List<Pronoun> _pronouns = [];
    private readonly List<Image> _images = [];

    private Character(
        Guid id,
        string name,
        string? description,
        Guid createdBy) : base(id)
    {
        Name = name;
        Description = description;
        CreatedBy = createdBy;
        CreatedOn = DateTimeOffset.UtcNow;
    }
    
    private Character()
    {
    }
    
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public Guid? ModifiedBy { get; set; }
    
    public IReadOnlyList<Pronoun> Pronouns => _pronouns.AsReadOnly();
    public IReadOnlyList<Image> Images => _images.AsReadOnly();
    
    public static Result<Character> Create(
        Guid id,
        string name,
        string? description,
        Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new Error("Character.Name.Invalid", "Character name cannot be empty.");
        }
        
        if (name.Length > NameMaxLength) 
        {
            return new Error("Character.Name.TooLong", $"Character name cannot exceed {NameMaxLength} characters.");
        }
        
        if (description?.Length > DescriptionMaxLength)
        {
            return new Error("Character.Description.TooLong", $"Character description cannot exceed {DescriptionMaxLength} characters.");
        }
        
        if (createdBy == Guid.Empty)
        {
            return new Error("Character.CreatedBy.Invalid", "CreatedBy must be a valid user ID.");
        }
        
        var character = new Character(
            id,
            name,
            description,
            createdBy);
        
        return character;
    }
    
    public Result ChangeName(string name,  Guid modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new Error("Character.Name.Invalid", "Character name cannot be empty.");
        }
        
        if (name.Length > NameMaxLength) 
        {
            return new Error("Character.Name.TooLong", $"Character name cannot exceed {NameMaxLength} characters.");
        }
        
        if (Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Success();
        }

        Name = name;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
        
        return Result.Success();
    }
    
    public Result ChangeDescription(string? description, Guid modifiedBy) 
    {
        if (description?.Length > DescriptionMaxLength)
        {
            return new Error("Character.Description.TooLong", $"Character description cannot exceed {DescriptionMaxLength} characters.");
        }
        
        if (Description == description)
        {
            return Result.Success();
        }

        Description = description;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
        
        return Result.Success();
    }
    
    public Result<Pronoun> AddPronoun(string subject, string obj, string possessive, Guid modifiedBy)
    {
        if (_pronouns.Any(p =>
                p.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase) &&
                p.Object.Equals(obj, StringComparison.OrdinalIgnoreCase) &&
                p.Possessive.Equals(possessive, StringComparison.OrdinalIgnoreCase)))
        {
            return new Error("Character.Pronoun.Duplicate", "This pronoun already exists for the character.");
        }
        
        var pronounResult = Pronoun.Create(Guid.NewGuid(), subject, obj, possessive);
        if (pronounResult.IsFailure)
        {
            return pronounResult;
        }
        
        _pronouns.Add(pronounResult.Value);
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;

        return pronounResult;
    }
    
    public Result<Pronoun> RemovePronoun(string subject, string obj, string possessive, Guid modifiedBy) 
    {
        var pronoun = _pronouns.FirstOrDefault(p =>
            p.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase) &&
            p.Object.Equals(obj, StringComparison.OrdinalIgnoreCase) &&
            p.Possessive.Equals(possessive, StringComparison.OrdinalIgnoreCase));

        if (pronoun is null)
        {
            return new Error("Character.Pronoun.NotFound", "The specified pronoun does not exist for the character.");
        }
        
        _pronouns.Remove(pronoun);
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
        
        return pronoun;
    }

    public Result<Image> AddImage(string name, Guid modifiedBy)
    {
        var image = new Image(Guid.NewGuid(), Id, name, UploadStatus.Pending);
        
        _images.Add(image);
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;

        return image;
    }

    public Result<Image> SetImageProcessing(Guid imageId, Guid modifiedBy)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image is null)
        {
            return new Error("Character.Image.NotFound", "The specified image does not exist for the character.");
        }

        var result = image.SetProcessing();
        if (result.IsFailure)
        {
            return Result.Failure<Image>(result.Error);
        }
        
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;

        return image;
    }
    
    public Result<Image> SetImageCompleted(Guid imageId, Guid modifiedBy)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image is null)
        {
            return new Error("Character.Image.NotFound", "The specified image does not exist for the character.");
        }

        var result = image.SetCompleted();
        if (result.IsFailure)
        {
            return Result.Failure<Image>(result.Error);
        }
        
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;

        return image;
    }
    
    public Result<Image> SetImageFailed(Guid imageId, Guid modifiedBy)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image is null)
        {
            return new Error("Character.Image.NotFound", "The specified image does not exist for the character.");
        }

        var result = image.SetFailed();
        if (result.IsFailure)
        {
            return Result.Failure<Image>(result.Error);
        }
        
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;

        return image;
    }
    
    public Result<Image> RemoveImage(Guid imageId, Guid modifiedBy)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image is null)
        {
            return new Error("Character.Image.NotFound", "The specified image does not exist for the character.");
        }
        
        _images.Remove(image);
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;

        return image;
    }
}