using System.Reflection;
using CharacterSite.Application;
using CharacterSite.Domain.Primitives;
using CharacterSite.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using NetArchTest.Rules;

namespace CharacterSite.Architecture.Tests.Layers;

public class InfrastructureLayerTests
{
    private const string InfrastructureNamespace = "CharacterSite.Infrastructure";
    private const string DomainNamespace = "CharacterSite.Domain";
    private const string ApplicationNamespace = "CharacterSite.Application";

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        // Arrange
        var assembly = typeof(CharacterDbContext).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn("CharacterSite.Api")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Infrastructure layer should not depend on API layer. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void Repository_Implementations_Should_Implement_Domain_Interface()
    {
        // Arrange
        var assembly = typeof(CharacterDbContext).Assembly;

        // Act
        var repositories = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{InfrastructureNamespace}.Repositories")
            .And()
            .HaveNameEndingWith("Repository")
            .GetTypes();

        var violatingTypes = new List<string>();

        foreach (var repository in repositories)
        {
            var implementsInterface = repository.GetInterfaces()
                .Any(i => i.Namespace?.StartsWith(DomainNamespace) == true);

            if (!implementsInterface) violatingTypes.Add(repository.Name);
        }

        // Assert
        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void Repository_Implementations_Should_Have_Internal_Or_Public_Visibility()
    {
        // Arrange
        var assembly = typeof(CharacterDbContext).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{InfrastructureNamespace}.Repositories")
            .And()
            .HaveNameEndingWith("Repository")
            .Should()
            .BePublic()
            .Or()
            .NotBePublic() // Internal classes
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Repository implementations should be public or internal. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void Query_Implementations_Should_Implement_Application_Interface()
    {
        // Arrange
        var assembly = typeof(CharacterDbContext).Assembly;

        // Act
        var queries = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{InfrastructureNamespace}.Queries")
            .And()
            .HaveNameEndingWith("Queries")
            .GetTypes();

        var violatingTypes = new List<string>();

        foreach (var query in queries)
        {
            var implementsInterface = query.GetInterfaces()
                .Any(i => i.Namespace?.StartsWith(ApplicationNamespace) == true);

            if (!implementsInterface) violatingTypes.Add(query.Name);
        }

        // Assert
        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void Entity_Configurations_Should_Reside_In_Configurations_Namespace()
    {
        // Arrange
        var assembly = typeof(CharacterDbContext).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Configuration")
            .Should()
            .ResideInNamespace($"{InfrastructureNamespace}.Configurations")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All entity configurations should reside in Configurations namespace. Violating types: {string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? [])}");
    }

    [Fact]
    public void DbContext_Should_Not_Be_In_Domain_Or_Application()
    {
        // Arrange
        var infrastructureAssembly = typeof(CharacterDbContext).Assembly;
        var domainAssembly = typeof(Entity).Assembly;
        var applicationAssembly = typeof(AssemblyMarker).Assembly;

        // Act
        var domainTypes = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(DbContext))
            .GetTypes();

        var applicationTypes = Types.InAssembly(applicationAssembly)
            .That()
            .Inherit(typeof(DbContext))
            .GetTypes();

        // Assert
        Assert.Empty(domainTypes);
        Assert.Empty(applicationTypes);
    }

    [Fact]
    public void Repositories_Should_Not_Expose_IQueryable()
    {
        // Arrange
        var domainAssembly = typeof(Entity).Assembly;

        // Act
        var repositories = Types.InAssembly(domainAssembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.Repositories")
            .And()
            .AreInterfaces()
            .GetTypes();

        var violatingMethods = new List<string>();

        foreach (var repository in repositories)
        {
            var methods = repository.GetMethods()
                .Where(m => m.ReturnType.Name.Contains("IQueryable"))
                .ToList();

            foreach (var method in methods) violatingMethods.Add($"{repository.Name}.{method.Name}");
        }

        // Assert
        Assert.Empty(violatingMethods);
    }

    [Fact]
    public void Infrastructure_Services_Should_Not_Reference_Domain_Entities_In_Public_Api()
    {
        // Arrange
        var assembly = typeof(CharacterDbContext).Assembly;
        var domainAssembly = typeof(Entity).Assembly;

        var entityTypes = Types.InAssembly(domainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes()
            .ToHashSet();

        // Act
        var services = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{InfrastructureNamespace}.Services")
            .GetTypes();

        var violatingServices = new List<string>();

        foreach (var service in services)
        {
            var publicMethods = service.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.DeclaringType == service);

            foreach (var method in publicMethods)
            {
                // Check return type
                if (entityTypes.Contains(method.ReturnType))
                    violatingServices.Add($"{service.Name}.{method.Name} returns {method.ReturnType.Name}");

                // Check parameters
                foreach (var param in method.GetParameters())
                    if (entityTypes.Contains(param.ParameterType))
                        violatingServices.Add(
                            $"{service.Name}.{method.Name} has parameter {param.Name} of type {param.ParameterType.Name}");
            }
        }

        // Assert - Note: Repositories are exempt as they work with entities
        var nonRepositoryViolations = violatingServices.Where(v => !v.Contains("Repository")).ToList();
        Assert.Empty(nonRepositoryViolations);
    }

    [Fact]
    public void Query_Services_Should_Not_Modify_Data()
    {
        // Arrange
        var assembly = typeof(CharacterDbContext).Assembly;

        // Act
        var queries = Types.InAssembly(assembly)
            .That()
            .ResideInNamespace($"{InfrastructureNamespace}.Queries")
            .GetTypes();

        var violatingMethods = new List<string>();

        foreach (var query in queries)
        {
            var methods = query.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.DeclaringType == query);

            foreach (var method in methods)
            {
                // Query methods should return Task, ValueTask, IEnumerable, IAsyncEnumerable or value types
                var methodName = method.Name.ToLower();
                if (methodName.Contains("add") || methodName.Contains("update") ||
                    methodName.Contains("delete") || methodName.Contains("remove") ||
                    methodName.Contains("save") || methodName.Contains("insert"))
                    violatingMethods.Add($"{query.Name}.{method.Name}");
            }
        }

        // Assert
        Assert.Empty(violatingMethods);
    }

    [Fact]
    public void Migrations_Should_Reside_In_Migrations_Namespace()
    {
        // Arrange
        var assembly = typeof(CharacterDbContext).Assembly;

        // Act
        var migrations = Types.InAssembly(assembly)
            .That()
            .Inherit(typeof(Migration))
            .GetTypes();

        var violatingTypes = migrations
            .Where(m => !m.Namespace?.Contains("Migrations") == true)
            .Select(m => m.Name)
            .ToList();

        // Assert
        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void Infrastructure_Should_Register_Services_Through_DependencyInjection()
    {
        // Arrange
        var assembly = typeof(CharacterDbContext).Assembly;

        // Act
        var hasDependencyInjectionClass = Types.InAssembly(assembly)
            .That()
            .HaveNameMatching("DependencyInjection")
            .GetTypes()
            .Any();

        // Assert
        Assert.True(hasDependencyInjectionClass,
            "Infrastructure should have a DependencyInjection class for service registration");
    }
}