using CharacterSite.Domain.Entities;
using CharacterSite.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CharacterSite.Infrastructure.Configurations;

public class PronounConfiguration : IEntityTypeConfiguration<Pronoun>
{
    public void Configure(EntityTypeBuilder<Pronoun> builder)
    {
        builder.ToTable(TableNames.Pronouns);
        
        builder.HasKey(x => x.Id);
    }
}