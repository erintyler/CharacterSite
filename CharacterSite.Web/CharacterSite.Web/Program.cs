using CharacterSite.Application.Services;
using CharacterSite.Web.Components;
using CharacterSite.Web.Constants;
using CharacterSite.Web.Endpoints;
using CharacterSite.Web.Extensions;
using Refit;
using _Imports = CharacterSite.Web.Client._Imports;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddAuthentication()
    .AddCookie();

builder.Services
    .AddHttpClient(ApiClients.CharacterApi, o => { o.BaseAddress = new Uri("http+https://charactersite-api/"); })
    .AddAuthToken();

builder.Services.AddRefitClient<ICharacterApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http+https://charactersite-api"))
    .AddAuthToken();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapCharacterApiEndpoints();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly);

app.Run();