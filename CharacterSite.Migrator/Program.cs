using CharacterSite.Infrastructure;
using CharacterSite.Migrator;
using Wolverine;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddInfrastructure();

builder.UseWolverine(o =>
{
    o.ApplicationAssembly = typeof(CharacterSite.Application.AssemblyMarker).Assembly;
    o.Durability.Mode = DurabilityMode.MediatorOnly;
});

builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();
host.Run();