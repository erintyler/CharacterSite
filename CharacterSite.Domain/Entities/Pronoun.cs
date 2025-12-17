using CharacterSite.Domain.Common;
using CharacterSite.Domain.Primitives;

namespace CharacterSite.Domain.Entities;

public class Pronoun : Entity
{
    public const int MaxLength = 50;
    
    private Pronoun(Guid id, string subject, string @object, string possessive) : base(id)
    {
        Subject = subject;
        Object = @object;
        Possessive = possessive;
    }
    
    private Pronoun()
    {
    }
    
    public string Subject { get; private set; }
    public string Object { get; private set; }
    public string Possessive { get; private set; }
    
    public static Result<Pronoun> Create(Guid id, string subject, string @object, string possessive)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            return new Error("Pronoun.Subject.Empty", "Subject pronoun cannot be empty.");
        }
        
        if (string.IsNullOrWhiteSpace(@object))
        {
            return new Error("Pronoun.Object.Empty", "Object pronoun cannot be empty.");
        }
        
        if (string.IsNullOrWhiteSpace(possessive))
        {
            return new Error("Pronoun.Possessive.Empty", "Possessive pronoun cannot be empty.");
        }

        return new Pronoun(id, subject, @object, possessive);
    }
}