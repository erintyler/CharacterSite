using Projects;
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false);

var postgresdb = postgres.AddDatabase("characterdb");

var blobs = builder.AddAzureStorage("storage")
    .RunAsEmulator()
    .AddBlobs("characterblobs");

var apiservice = builder.AddProject<CharacterSite_Api>("charactersite-api")
    .WithReference(postgresdb)
    .WithReference(blobs);

builder.AddProject<CharacterSite_Web>("charactersite-web")
    .WithReference(apiservice)
    .WaitFor(apiservice);

builder.AddProject<CharacterSite_Migrator>("charactersite-migrator")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddScalarApiReference(o =>
    {
        o.DefaultProxy = false;
        o.PreferHttpsEndpoint = true;
    })
    .WithApiReference(apiservice);

builder.Build().Run();