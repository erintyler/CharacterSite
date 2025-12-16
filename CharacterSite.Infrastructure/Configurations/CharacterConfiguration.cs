using CharacterSite.Domain.Entities;
using CharacterSite.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CharacterSite.Infrastructure.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.ToTable(TableNames.Characters);
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(Character.NameMaxLength);
        
        builder.Property(x => x.Description)
            .HasMaxLength(Character.DescriptionMaxLength);

        builder.HasMany(x => x.Images)
            .WithOne()
            .HasForeignKey(x => x.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Pronouns)
            .WithMany()
            .UsingEntity(j => j.ToTable(TableNames.CharacterPronouns));

        builder.HasIndex(x => x.CreatedBy);
    }
}