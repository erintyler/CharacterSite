namespace CharacterSite.Domain.Primitives;

public interface IAuditableEntity
{
    DateTimeOffset CreatedOn { get; set; }
    Guid CreatedBy { get; set; }
    DateTimeOffset? ModifiedOn { get; set; }
    Guid? ModifiedBy { get; set; }
}