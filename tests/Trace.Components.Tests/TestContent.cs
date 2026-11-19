using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Trace.Domain.Content;
using Trace.Web.Site;

namespace Trace.Components.Tests;

/// <summary>The real content (copied next to the test binaries by Trace.Web) and the services pages need.</summary>
internal static class TestContent
{
    public static ContentStore Store { get; } = ContentStore.Load(Path.Combine(AppContext.BaseDirectory, "content"));

    public static void AddSite(this BunitContext context, string path)
    {
        context.Services.AddSingleton(Store);
        context.Services.AddSingleton(new SiteOptions(new Uri("https://www.manuelbravo.com.br/")));
        context.Services.AddScoped<PageContext>();
        context.Services.GetRequiredService<NavigationManager>().NavigateTo(path);
    }
}
