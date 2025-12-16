using CharacterSite.Domain.Primitives;

namespace CharacterSite.Domain.DomainEvents.Character;

public record ImageRemovedDomainEvent(
    Guid Id,
    Guid ImageId) : IDomainEvent;