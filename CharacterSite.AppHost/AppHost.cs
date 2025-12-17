var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false);

var postgresdb = postgres.AddDatabase("characterdb");

var blobs = builder.AddAzureStorage("storage")
    .RunAsEmulator()
    .AddBlobs("characterblobs");

builder.AddProject<Projects.CharacterSite_Api>("charactersite-api")
    .WithReference(postgresdb)
    .WithReference(blobs);

builder.AddProject<Projects.CharacterSite_Migrator>("charactersite-migrator")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.Build().Run();