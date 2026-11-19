using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Trace.Domain.Content;
using Trace.Web.Site;

namespace Trace.Components.Tests;

/// <summary>The real content (copied next to the test binaries by Trace.Web) and the services pages need.</summary>
internal static class TestContent
{
    public static ContentStore Store { get; } = ContentStore.Load(Path.Combine(AppContext.BaseDirectory, "content"));

    public static MediaCatalog Media { get; } = new(MediaCatalog.FindRoot(null));

    public static void AddSite(this BunitContext context, string path, RepoStats? repo = null)
    {
        context.Services.AddSingleton(Store);
        context.Services.AddSingleton(new SiteOptions(new Uri("https://www.manuelbravo.com.br/")));
        context.Services.AddScoped<PageContext>();
        context.Services.AddSingleton(Media);
        context.Services.AddSingleton(repo ?? new RepoStats(Commits: 42, Adrs: 10, TestMethods: 136));
        context.Services.AddSingleton<TimeProvider>(new FakeTimeProvider(new DateTimeOffset(2026, 11, 19, 12, 0, 0, TimeSpan.Zero)));
        context.Services.GetRequiredService<NavigationManager>().NavigateTo(path);
    }
}
