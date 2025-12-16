var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("characterdb");

var blobs = builder.AddAzureStorage("storage")
    .RunAsEmulator()
    .AddBlobs("blobs");

builder.AddProject<Projects.CharacterSite_Api>("charactersite-api")
    .WithReference(postgresdb)
    .WithReference(blobs);

builder.Build().Run();