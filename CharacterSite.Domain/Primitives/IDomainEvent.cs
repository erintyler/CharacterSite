namespace CharacterSite.Domain.Primitives;

public interface IDomainEvent
{
    Guid Id { get; init; }
}