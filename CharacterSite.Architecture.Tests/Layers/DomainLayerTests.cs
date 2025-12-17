using System.Reflection;
using CharacterSite.Domain.Common;
using CharacterSite.Domain.Primitives;
using NetArchTest.Rules;

namespace CharacterSite.Architecture.Tests.Layers;

public class DomainLayerTests
{
    private const string DomainNamespace = "CharacterSite.Domain";
    private const string ApplicationNamespace = "CharacterSite.Application";
    private const string InfrastructureNamespace = "CharacterSite.Infrastructure";

    [Fact]
    public void Domain_Should_Not_Depend_On_Application()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn(ApplicationNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Domain layer should not depend on Application layer. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Domain layer should not depend on Infrastructure layer. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void Entities_Should_Inherit_From_Entity_Or_AggregateRoot()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.Entities")
            .Should()
            .Inherit(typeof(Entity))
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All Entities should inherit from Entity or AggregateRoot. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void AggregateRoots_Should_Have_Private_Or_Protected_Constructors()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var aggregateRoots = Types.InAssembly(assembly)
            .That()
            .Inherit(typeof(AggregateRoot))
            .GetTypes();

        var violatingTypes = new List<string>();

        foreach (var aggregateRoot in aggregateRoots)
        {
            var publicConstructors = aggregateRoot.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Where(c => c.GetParameters().Length > 0) // Exclude parameterless constructors for EF
                .ToList();

            if (publicConstructors.Any()) violatingTypes.Add(aggregateRoot.Name);
        }

        // Assert
        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void Entities_Should_Have_Factory_Methods()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var entities = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.Entities")
            .And()
            .DoNotHaveNameEndingWith("Configuration")
            .GetTypes();

        var violatingTypes = new List<string>();

        foreach (var entity in entities)
        {
            var hasCreateMethod = entity.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Any(m => m.Name == "Create");

            if (!hasCreateMethod) violatingTypes.Add(entity.Name);
        }

        // Assert
        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void ValueObjects_Should_Inherit_From_ValueObject()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var valueObjectTypes = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.ValueObjects")
            .GetTypes();

        if (!valueObjectTypes.Any())
        {
            // No value objects yet, test passes
            Assert.True(true);
            return;
        }

        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.ValueObjects")
            .Should()
            .Inherit(typeof(ValueObject))
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All ValueObjects should inherit from ValueObject base class. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void ValueObjects_Should_Be_Immutable()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var valueObjects = Types.InAssembly(assembly)
            .That()
            .Inherit(typeof(ValueObject))
            .GetTypes();

        var violatingTypes = new List<string>();

        foreach (var valueObject in valueObjects)
        {
            var mutableProperties = valueObject.GetProperties()
                .Where(p => p.CanWrite && p.SetMethod?.IsPublic == true)
                .ToList();

            if (mutableProperties.Any())
                violatingTypes.Add($"{valueObject.Name} ({string.Join(", ", mutableProperties.Select(p => p.Name))})");
        }

        // Assert
        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void DomainEvents_Should_Implement_IDomainEvent()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var domainEventTypes = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.DomainEvents")
            .GetTypes();

        if (!domainEventTypes.Any())
        {
            // No domain events yet, test passes
            Assert.True(true);
            return;
        }

        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.DomainEvents")
            .Should()
            .ImplementInterface(typeof(IDomainEvent))
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All DomainEvents should implement IDomainEvent. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void DomainEvents_Should_Be_Immutable_Records()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var domainEvents = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .GetTypes();

        var violatingTypes = new List<string>();

        foreach (var domainEvent in domainEvents)
        {
            // Check if it's a record (records are sealed and have specific characteristics)
            var isRecord = domainEvent.GetMethod("<Clone>$") != null;

            if (!isRecord && !domainEvent.IsSealed) violatingTypes.Add(domainEvent.Name);
        }

        // Assert
        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void Repositories_Should_Be_Interfaces()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.Repositories")
            .Should()
            .BeInterfaces()
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All Repositories in Domain should be interfaces. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void Repository_Interfaces_Should_Start_With_I()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.Repositories")
            .And()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All Repository interfaces should start with 'I'. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void Entities_Should_Not_Have_Public_Setters_For_Collections()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var entities = Types.InAssembly(assembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        var violatingTypes = new List<string>();

        foreach (var entity in entities)
        {
            var collectionProperties = entity.GetProperties()
                .Where(p => p.PropertyType.IsGenericType &&
                            (p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                             p.PropertyType.GetGenericTypeDefinition() == typeof(IList<>) ||
                             p.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                .Where(p => p.CanWrite && p.SetMethod?.IsPublic == true)
                .ToList();

            if (collectionProperties.Any())
                violatingTypes.Add($"{entity.Name} ({string.Join(", ", collectionProperties.Select(p => p.Name))})");
        }

        // Assert
        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void Domain_Should_Not_Reference_EntityFramework()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Domain should not reference Entity Framework. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void Entities_Should_Expose_Collections_As_ReadOnly()
    {
        // Arrange
        var assembly = typeof(Entity).Assembly;

        // Act
        var entities = Types.InAssembly(assembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        var violatingTypes = new List<string>();

        foreach (var entity in entities)
        {
            var publicCollectionProperties = entity.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType)
                .Where(p => !p.PropertyType.GetGenericTypeDefinition().Name.Contains("ReadOnly"))
                .Where(p => p.PropertyType.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                     i.GetGenericTypeDefinition() == typeof(ICollection<>))))
                .Where(p => p.GetMethod?.IsPublic == true)
                .ToList();

            foreach (var prop in publicCollectionProperties)
                // Check if it's truly a collection that should be readonly
                if (!prop.PropertyType.Name.Contains("ReadOnly") &&
                    (prop.PropertyType.Name.Contains("List") || prop.PropertyType.Name.Contains("Collection")))
                    violatingTypes.Add($"{entity.Name}.{prop.Name}");
        }

        // Assert
        Assert.Empty(violatingTypes);
    }
}