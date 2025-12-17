using CharacterSite.Application;
using CharacterSite.Application.Features.Characters.Queries;
using CharacterSite.Application.Features.Pronouns.Commands.CreatePronoun;
using CharacterSite.Application.Features.Pronouns.Queries;
using CharacterSite.Application.Models.Responses;
using CharacterSite.Domain.Common;
using CharacterSite.Infrastructure;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.AddInfrastructure();

builder.UseWolverine(o =>
{
    o.ApplicationAssembly = typeof(AssemblyMarker).Assembly;
    o.Durability.Mode = DurabilityMode.MediatorOnly;
});

if (builder.Environment.IsDevelopment())
    builder.Services.AddCors(o =>
    {
        o.AddPolicy("OpenApiPolicy", p => p
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
    });

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors();

    app.MapOpenApi()
        .RequireCors("OpenApiPolicy");
}

app.UseHttpsRedirection();

app.MapGet("/characters/{id:guid}", async (Guid id, IMessageBus bus) =>
    {
        var query = new GetCharacterByIdQuery(id);
        var character = await bus.InvokeAsync<Result<CharacterResponse>>(query);

        return character.IsSuccess ? Results.Ok(character.Value) : Results.NotFound();
    })
    .WithName("GetCharacterById")
    .WithSummary("Get a character by ID")
    .WithTags("Characters")
    .Produces<CharacterResponse>()
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("/pronouns", async (IMessageBus bus) =>
{
    var query = new GetAllPronounsQuery();
    var pronouns = await bus.InvokeAsync<Result<IReadOnlyList<PronounResponse>>>(query);

    return pronouns.IsSuccess ? Results.Ok(pronouns.Value) : Results.BadRequest(pronouns.Error);
});

app.MapPost("/pronouns", async (CreatePronounCommand command, IMessageBus bus) =>
{
    var result = await bus.InvokeAsync<Result<PronounResponse>>(command);

    return result.IsSuccess
        ? Results.CreatedAtRoute("GetPronounById", new { id = result.Value.Id })
        : Results.BadRequest(result.Error);
});

app.MapGet("/pronouns/{id:guid}", async (Guid id, IMessageBus bus) =>
    {
        var query = new GetPronounByIdQuery(id);
        var pronoun = await bus.InvokeAsync<Result<PronounResponse>>(query);

        return pronoun.IsSuccess ? Results.Ok(pronoun.Value) : Results.NotFound();
    })
    .WithName("GetPronounById");

app.Run();