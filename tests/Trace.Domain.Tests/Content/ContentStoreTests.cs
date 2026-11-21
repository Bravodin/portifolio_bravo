using Trace.Domain.Content;

namespace Trace.Domain.Tests.Content;

public sealed class ContentStoreTests
{
    private static readonly ContentStore Store = ContentStore.Load(RepoContent.Root);

    [Fact]
    public void Load_RepoContent_IsConsistentAcrossLocales() =>
        Assert.Empty(ContentValidator.Validate(Store));

    [Theory]
    [InlineData("pt-BR", "~1,2 s")]
    [InlineData("en", "~1.2 s")]
    [InlineData("es", "~1,2 s")]
    public void Metric_M1_IsPreformattedPerLocale(string locale, string after) =>
        Assert.Equal(after, Store.For(locale).Metric("M1").After);

    [Fact]
    public void Metric_M1_MatchesTheContentInventory()
    {
        var m1 = Store.For(Locales.English).Metric("M1");

        Assert.Equal("12–20 s", m1.Before);
        Assert.Equal("~1.2 s", m1.After);
    }

    [Fact]
    public void Identity_ContactIsLinkedInAndEmail_InEveryLocale()
    {
        foreach (var locale in Locales.All)
        {
            var identity = Store.For(locale).Identity;
            Assert.Equal(new Uri("https://www.linkedin.com/in/manuelbravoc/"), identity.LinkedIn);
            Assert.Equal("manuelbravoc@gmail.com", identity.Email);
        }
    }

    [Fact]
    public void Identity_LinksToGitHub_InEveryLocale() =>
        Assert.All(Locales.All, locale =>
            Assert.Equal(new Uri("https://github.com/Bravodin"), Store.For(locale).Identity.GitHub));

    [Fact]
    public void Content_HasNoPhone_AndNoEmailButTheContactOne()
    {
        // ADR-0013: the one published address is identity.json's; nothing else may leak into the content.
        var text = string.Concat(Directory.EnumerateFiles(RepoContent.Root, "*.json", SearchOption.AllDirectories).Select(File.ReadAllText))
            .Replace("manuelbravoc@gmail.com", string.Empty, StringComparison.Ordinal);

        Assert.DoesNotMatch(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}", text);
        Assert.DoesNotMatch(@"\(?\d{2}\)?\s?9?\d{4}-\d{4}", text);
    }

    [Fact]
    public void Log_EndsWithTheDelphiYears()
    {
        var first = Store.For(Locales.English).Log[^1];

        Assert.Equal("2006 → 2008", first.Period);
        Assert.Equal("Delphi · Pascal", first.Highlight);
        Assert.Equal(2006, Store.For(Locales.Spanish).Identity.CareerStartYear);
    }

    [Fact]
    public void T_ReturnsUiString() =>
        Assert.Equal("Modo recrutador (30 s)", Store.For(Locales.Portuguese).T("cta.recruiter"));

    [Fact]
    public void T_MissingKey_ThrowsWithFileName()
    {
        var error = Assert.Throws<KeyNotFoundException>(() => Store.For(Locales.Spanish).T("nope"));
        Assert.Contains("content/es/ui.json", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Metric_And_Stage_Missing_Throw()
    {
        Assert.Throws<KeyNotFoundException>(() => Store.For(Locales.English).Metric("M99"));
        Assert.Throws<KeyNotFoundException>(() => Store.For(Locales.English).Stage("nope"));
    }

    [Fact]
    public void Stages_AreTheSevenLayersInOrder_EachWithAMetric()
    {
        var stages = Store.For(Locales.English).Stages;

        Assert.Equal(["client", "edge", "gateway", "services", "cache", "database", "pit"], stages.Select(s => s.Id));
        Assert.Equal(Enumerable.Range(1, 7), stages.Select(s => s.Number));
        Assert.Equal(["M1", "M2", "M3", "M4", "M5", "M6", "M7"], stages.Select(s => s.Metric));
    }

    [Fact]
    public void Toolbox_HasFourDrawers_WithDelphiUnderPreviousTechnologies()
    {
        var toolbox = Store.For(Locales.Portuguese).Toolbox;

        Assert.Equal(["core", "architecture", "practices", "previous"], toolbox.Select(d => d.Id));
        Assert.Contains("Delphi", toolbox.Single(d => d.Id == "previous").Tools);
        Assert.Contains("C#", toolbox.Single(d => d.Id == "core").Tools);
    }

    [Fact]
    public void Garage_HasThreeCards_EachWithAnImage()
    {
        var garage = Store.For(Locales.English).Garage;

        Assert.Equal(["motorcycles", "mechanics", "games"], garage.Select(c => c.Id));
        Assert.Equal(["track-03", "workbench-01", "games-01"], garage.Select(c => c.Photo));
    }

    [Fact]
    public void Garage_Games_ListsRetroAndCurrentFavourites_AsTextOnly()
    {
        var favorites = Store.For(Locales.Portuguese).Garage.Single(c => c.Id == "games").Favorites!;

        Assert.Equal(["Clássicos", "Hoje"], favorites.Select(g => g.Label));
        Assert.Contains("Demon's Crest", favorites[0].Items);
        Assert.Contains("Overwatch", favorites[1].Items);
        Assert.All(Store.For(Locales.English).Garage.Where(c => c.Id != "games"), c => Assert.Null(c.Favorites));
    }

    [Fact]
    public void About_HasThreeParagraphsAndSixPrinciples_InEveryLocale()
    {
        foreach (var locale in Locales.All)
        {
            var about = Store.For(locale).About;
            Assert.Equal(3, about.Paragraphs.Count);
            Assert.Equal(6, about.Principles.Count);
            Assert.Equal(about.Principles[^1].Title, Store.For(locale).About.Principles[5].Title);
        }

        Assert.Contains("Delphi em 2006", Store.For(Locales.Portuguese).About.Paragraphs[0], StringComparison.Ordinal);
    }

    [Fact]
    public void Colophon_TellsWhoDidWhat_WithTheReportedUsage()
    {
        var colophon = Store.For(Locales.Portuguese).Colophon;

        Assert.Equal(["Manuel", "Claude", "Juntos"], colophon.Roles.Select(r => r.Who));
        Assert.Equal(4, colophon.Reviews.Count);
        Assert.Equal(["285,5 milhões", "742 mil", "US$ 81,72", "≈ 6 h"], colophon.Usage.Select(u => u.Value));
        Assert.Equal("US$81.72", Store.For(Locales.English).Colophon.Usage[2].Value);
    }

    [Fact]
    public void Stage_Client_HasBothStoryBeats() =>
        Assert.Equal(2, Store.For(Locales.English).Stage("client").Story.Count);

    [Fact]
    public void For_UnsupportedLocale_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Store.For("fr"));

    [Fact]
    public void Recruiter_PicksSixMetricsAndThreeRoles()
    {
        Assert.Equal(6, Store.Recruiter.Metrics.Count);
        Assert.Equal(3, Store.Recruiter.Roles);
    }
}
