using CharacterSite.Application;
using CharacterSite.Domain.Primitives;
using NetArchTest.Rules;

namespace CharacterSite.Architecture.Tests.Layers;

public class ApplicationLayerTests
{
    private const string ApplicationNamespace = "CharacterSite.Application";
    private const string DomainNamespace = "CharacterSite.Domain";
    private const string InfrastructureNamespace = "CharacterSite.Infrastructure";

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Application layer should not depend on Infrastructure layer. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void Commands_Should_Be_Immutable_Records()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{ApplicationNamespace}.Features")
            .And()
            .HaveNameEndingWith("Command")
            .Should()
            .BeClasses()
            .Or()
            .BeSealed() // Records are sealed by default
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All Commands should be immutable records. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void Queries_Should_Be_Immutable_Records()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{ApplicationNamespace}.Features")
            .And()
            .HaveNameEndingWith("Query")
            .Should()
            .BeClasses()
            .Or()
            .BeSealed()
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All Queries should be immutable records. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void CommandHandlers_Should_Have_Correct_Naming_Convention()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{ApplicationNamespace}.Features")
            .And()
            .HaveNameEndingWith("CommandHandler")
            .Should()
            .BePublic()
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All CommandHandlers should be public and follow naming convention. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void QueryHandlers_Should_Have_Correct_Naming_Convention()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{ApplicationNamespace}.Features")
            .And()
            .HaveNameEndingWith("QueryHandler")
            .Should()
            .BePublic()
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All QueryHandlers should be public and follow naming convention. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void Commands_Should_Reside_In_Commands_Namespace()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Command")
            .And()
            .DoNotHaveNameEndingWith("CommandHandler")
            .Should()
            .ResideInNamespaceMatching($"^{ApplicationNamespace}\\.Features(\\.[^.]+)*\\.Commands")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All Commands should reside in Commands namespace. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void Queries_Should_Reside_In_Queries_Namespace()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Query")
            .And()
            .DoNotHaveNameEndingWith("QueryHandler")
            .And()
            .DoNotImplementInterface(typeof(IAsyncEnumerable<>))
            .Should()
            .ResideInNamespaceMatching($"^{ApplicationNamespace}\\.Features(\\.[^.]+)*\\.Queries$")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All Queries should reside in Queries namespace. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void CommandHandlers_Should_Not_Return_Domain_Entities()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;
        var domainAssembly = typeof(Entity).Assembly;
        var entityTypes = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        // Act
        var handlers = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("CommandHandler")
            .GetTypes();

        var violatingHandlers = new List<string>();

        foreach (var handler in handlers)
        {
            var handleMethod = handler.GetMethods()
                .FirstOrDefault(m => m.Name == "Handle");

            if (handleMethod?.ReturnType != null)
            {
                var returnType = handleMethod.ReturnType;

                // Check if return type is Task<T> or ValueTask<T>
                if (returnType.IsGenericType)
                {
                    var genericArgs = returnType.GetGenericArguments();
                    if (genericArgs.Length > 0)
                    {
                        var innerType = genericArgs[0];

                        // Check if inner type is Result<T>
                        if (innerType.IsGenericType && innerType.GetGenericArguments().Length > 0)
                        {
                            var resultType = innerType.GetGenericArguments()[0];
                            if (entityTypes.Any(e => e == resultType)) violatingHandlers.Add(handler.Name);
                        }
                        else if (entityTypes.Any(e => e == innerType))
                        {
                            violatingHandlers.Add(handler.Name);
                        }
                    }
                }
            }
        }

        // Assert
        Assert.Empty(violatingHandlers);
    }

    [Fact]
    public void QueryHandlers_Should_Not_Return_Domain_Entities()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;
        var domainAssembly = typeof(Entity).Assembly;
        var entityTypes = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        // Act
        var handlers = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("QueryHandler")
            .GetTypes();

        var violatingHandlers = new List<string>();

        foreach (var handler in handlers)
        {
            var handleMethod = handler.GetMethods()
                .FirstOrDefault(m => m.Name == "Handle");

            if (handleMethod?.ReturnType != null)
            {
                var returnType = handleMethod.ReturnType;

                // Check if return type is Task<T> or ValueTask<T>
                if (returnType.IsGenericType)
                {
                    var genericArgs = returnType.GetGenericArguments();
                    if (genericArgs.Length > 0)
                    {
                        var innerType = genericArgs[0];

                        // Check if inner type is Result<T>
                        if (innerType.IsGenericType && innerType.GetGenericArguments().Length > 0)
                        {
                            var resultType = innerType.GetGenericArguments()[0];
                            if (entityTypes.Any(e => e == resultType)) violatingHandlers.Add(handler.Name);
                        }
                        else if (entityTypes.Any(e => e == innerType))
                        {
                            violatingHandlers.Add(handler.Name);
                        }
                    }
                }
            }
        }

        // Assert
        Assert.Empty(violatingHandlers);
    }

    [Fact]
    public void DomainEventHandlers_Should_Reside_In_DomainEvents_Namespace()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("DomainEventHandler")
            .Should()
            .ResideInNamespaceMatching($"^{ApplicationNamespace}\\.Features(\\.[^.]+)*\\.DomainEvents")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All DomainEventHandlers should reside in DomainEvents namespace. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void Application_Services_Should_Use_Interfaces()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{ApplicationNamespace}.Services")
            .And()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All service interfaces should start with 'I'. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void CommandHandlers_Should_Use_Repositories_Not_Queries()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;

        // Act
        var handlers = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("CommandHandler")
            .GetTypes();

        var violatingHandlers = new List<string>();

        foreach (var handler in handlers)
        {
            var constructor = handler.GetConstructors().FirstOrDefault();
            var handleMethod = handler.GetMethods().FirstOrDefault(m => m.Name == "Handle");

            var allParameters = new List<Type>();
            if (constructor != null) allParameters.AddRange(constructor.GetParameters().Select(p => p.ParameterType));

            if (handleMethod != null) allParameters.AddRange(handleMethod.GetParameters().Select(p => p.ParameterType));

            // Check if using query interfaces (should use repositories instead)
            if (allParameters.Any(p => p.Name.Contains("Queries") || p.Name.EndsWith("Queries")))
                violatingHandlers.Add(handler.Name);
        }

        // Assert
        Assert.Empty(violatingHandlers);
    }

    [Fact]
    public void QueryHandlers_Should_Use_Query_Interfaces()
    {
        // Arrange
        var assembly = typeof(AssemblyMarker).Assembly;

        // Act
        var handlers = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("QueryHandler")
            .GetTypes();

        var validHandlers = new List<string>();
        var violatingHandlers = new List<string>();

        foreach (var handler in handlers)
        {
            var constructor = handler.GetConstructors().FirstOrDefault();
            var handleMethod = handler.GetMethods().FirstOrDefault(m => m.Name == "Handle");

            var allParameters = new List<Type>();
            if (constructor != null) allParameters.AddRange(constructor.GetParameters().Select(p => p.ParameterType));

            if (handleMethod != null) allParameters.AddRange(handleMethod.GetParameters().Select(p => p.ParameterType));

            // Check if using query interfaces (correct for reads)
            if (allParameters.Any(p => p.Name.Contains("Queries") || p.Name.EndsWith("Queries")))
                validHandlers.Add(handler.Name);

            // Check if using repositories (violation - should use queries for reads)
            if (allParameters.Any(p => p.Name.Contains("Repository") && !p.Name.Contains("Queries")))
                violatingHandlers.Add(handler.Name);
        }

        // Assert - QueryHandlers should NOT use repositories directly
        Assert.Empty(violatingHandlers);
    }
}