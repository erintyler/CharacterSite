using CharacterSite.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CharacterSite.Infrastructure;

public class CharacterDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Character> Characters { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Pronoun> Pronouns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CharacterDbContext).Assembly);
}