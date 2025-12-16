using CharacterSite.Application.Features.Characters.Queries;
using CharacterSite.Application.Services;
using CharacterSite.Domain.Repositories;
using CharacterSite.Infrastructure.Queries;
using CharacterSite.Infrastructure.Repositories;
using CharacterSite.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;

namespace CharacterSite.Infrastructure;

public static class DependencyInjection
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddInfrastructure()
        {
            builder.AddNpgsqlDbContext<CharacterDbContext>("characterdb");
            builder.AddAzureBlobServiceClient("characterblobs");

            builder.Services.AddSingleton<IImageStorageService, AzureBlobImageStorageService>();
            
            builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
            builder.Services.AddScoped<IImageRepository, ImageRepository>();
            builder.Services.AddScoped<IPronounRepository, PronounRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<ICharacterQueries, CharacterQueries>();
            
            return builder;
        }
    }
}