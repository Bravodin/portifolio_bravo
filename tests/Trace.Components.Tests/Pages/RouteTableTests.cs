using System.Reflection;
using Microsoft.AspNetCore.Components;
using Trace.Domain.Content;

namespace Trace.Components.Tests.Pages;

public sealed class RouteTableTests
{
    // Every localized path in SiteRoutes must have a matching @page, and vice versa.
    [Fact]
    public void PageRoutes_MatchSiteRoutes()
    {
        var templates = typeof(Program).Assembly.GetTypes()
            .SelectMany(t => t.GetCustomAttributes<RouteAttribute>())
            .Select(r => r.Template.TrimEnd('/') + "/")
            .Where(t => t is not "/Error/" and not "/not-found/")
            .Order(StringComparer.Ordinal);

        Assert.Equal(SiteRoutes.AllPaths.Order(StringComparer.Ordinal), templates);
    }
}
