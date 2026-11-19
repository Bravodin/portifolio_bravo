using Trace.Domain.Content;
using Trace.Web.Components;
using Trace.Web.Site;

// This host never serves visitors. It runs in two places only:
// - `dotnet run` during development;
// - in-process inside Trace.Prerender, which renders every page to dist/ (ADR-0003).
// HTTPS, HSTS and caching are the static host's job (staticwebapp.config.json, ADR-0009).
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton(_ => ContentStore.Load(Path.Combine(AppContext.BaseDirectory, "content")));
builder.Services.AddSingleton(new SiteOptions(new Uri(builder.Configuration["Trace:SiteUrl"] ?? "https://www.manuelbravo.com.br/")));
builder.Services.AddSingleton(new MediaCatalog(MediaCatalog.FindRoot(builder.Configuration["Trace:MediaRoot"])));
builder.Services.AddScoped<PageContext>();
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

// Trace.Prerender points this at the *published* endpoints manifest, so the fingerprinted
// URLs in the HTML match the files it ships. Null = the default manifest next to the app.
var staticAssetsManifest = app.Configuration["Trace:StaticAssetsManifest"];
app.MapStaticAssets(staticAssetsManifest);
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Trace.Client._Imports).Assembly)
    .WithStaticAssets(staticAssetsManifest);

await app.RunAsync();

/// <summary>Entry point, exposed so Trace.Prerender can host the app in-process.</summary>
public partial class Program;
