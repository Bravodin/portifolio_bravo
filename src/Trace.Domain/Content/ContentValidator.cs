namespace Trace.Domain.Content;

/// <summary>Checks that the three locales are translations of the same content (same keys, same IDs, same shape).</summary>
public static class ContentValidator
{
    public static IEnumerable<string> Validate(ContentStore store)
    {
        var reference = store.For(Locales.ContentReference);

        foreach (var locale in Locales.All.Where(l => l != Locales.ContentReference))
        {
            var other = store.For(locale);

            foreach (var problem in SameSet("ui key", reference.Ui.Keys, other.Ui.Keys, locale))
            {
                yield return problem;
            }

            foreach (var problem in SameSet("metric", reference.Metrics.Select(m => m.Id), other.Metrics.Select(m => m.Id), locale))
            {
                yield return problem;
            }

            foreach (var problem in SameSet("stage", reference.Stages.Select(s => s.Id), other.Stages.Select(s => s.Id), locale))
            {
                yield return problem;
            }

            foreach (var problem in SameSet("toolbox drawer", reference.Toolbox.Select(d => d.Id), other.Toolbox.Select(d => d.Id), locale))
            {
                yield return problem;
            }

            foreach (var problem in SameSet("garage card", reference.Garage.Select(c => c.Id), other.Garage.Select(c => c.Id), locale))
            {
                yield return problem;
            }

            foreach (var (mine, theirs) in reference.Garage.Join(other.Garage, c => c.Id, c => c.Id, (a, b) => (a, b)))
            {
                var names = (GarageCard c) => string.Join('|', c.Favorites?.SelectMany(g => g.Items) ?? []);
                if (names(mine) != names(theirs))
                {
                    yield return $"garage card '{mine.Id}' lists different favourites in {locale}";
                }
            }

            foreach (var (mine, theirs) in reference.Stages.Join(other.Stages, s => s.Id, s => s.Id, (a, b) => (a, b)))
            {
                if (mine.Number != theirs.Number || mine.Metric != theirs.Metric)
                {
                    yield return $"stage '{mine.Id}' has a different number or metric in {locale}";
                }

                if (!(mine.Parts ?? []).Select(p => (p.Id, p.Metric)).SequenceEqual((theirs.Parts ?? []).Select(p => (p.Id, p.Metric))))
                {
                    yield return $"stage '{mine.Id}' has different parts or part metrics in {locale}";
                }

                if (CaseShape(mine.Case) != CaseShape(theirs.Case))
                {
                    yield return $"stage '{mine.Id}' has a different case (decisions or architecture layers) in {locale}";
                }
            }

            if (reference.About.Paragraphs.Count != other.About.Paragraphs.Count
                || reference.About.Principles.Count != other.About.Principles.Count)
            {
                yield return $"about.json has a different number of paragraphs or principles in {locale}";
            }

            var (ours, translated) = (reference.Colophon, other.Colophon);
            if (ours.Roles.Count != translated.Roles.Count || ours.Reviews.Count != translated.Reviews.Count || ours.Usage.Count != translated.Usage.Count)
            {
                yield return $"colophon.json has a different number of roles, reviews or usage figures in {locale}";
            }

            var adrs = (ColophonCopy c) => string.Join('|', c.Architecture.Nodes.Select(n => n.Flow + ":" + string.Join(',', n.Adrs)));
            if (adrs(ours) != adrs(translated))
            {
                yield return $"colophon.json architecture has different steps or ADRs in {locale}";
            }

            if (reference.Log.Count != other.Log.Count)
            {
                yield return $"log.json has {reference.Log.Count} entries in {Locales.ContentReference} but {other.Log.Count} in {locale}";
            }
            else if (!reference.Log.Select(e => e.Scope?.Count ?? 0).SequenceEqual(other.Log.Select(e => e.Scope?.Count ?? 0)))
            {
                yield return $"log.json scope lines differ between {Locales.ContentReference} and {locale}";
            }

            if (reference.Identity.CareerStartYear != other.Identity.CareerStartYear)
            {
                yield return $"identity.json careerStartYear differs in {locale}";
            }

            if (reference.Identity.LinkedIn != other.Identity.LinkedIn)
            {
                yield return $"identity.json LinkedIn URL differs in {locale}";
            }

            if (reference.Identity.GitHub != other.Identity.GitHub)
            {
                yield return $"identity.json GitHub URL differs in {locale}";
            }

            if (reference.Identity.Email != other.Identity.Email)
            {
                yield return $"identity.json e-mail differs in {locale}";
            }
        }

        foreach (var stage in reference.Stages.Where(s => reference.Metrics.All(m => m.Id != s.Metric)))
        {
            yield return $"stage '{stage.Id}' uses metric '{stage.Metric}', which metrics.json doesn't have";
        }

        foreach (var (stage, part) in reference.Stages.SelectMany(s => (s.Parts ?? []).Select(p => (s, p)))
                     .Where(x => x.p.Metric is { } id && reference.Metrics.All(m => m.Id != id)))
        {
            yield return $"stage '{stage.Id}' part '{part.Id}' uses metric '{part.Metric}', which metrics.json doesn't have";
        }

        foreach (var id in store.Recruiter.Metrics.Where(id => reference.Metrics.All(m => m.Id != id)))
        {
            yield return $"recruiter.json lists metric '{id}', which metrics.json doesn't have";
        }

        if (store.Recruiter.Roles > reference.Log.Count)
        {
            yield return $"recruiter.json asks for {store.Recruiter.Roles} roles, log.json has {reference.Log.Count}";
        }
    }

    /// <summary>The countable shape of a case (decisions, lanes, layers per lane), so translations can't drop a line.</summary>
    private static string CaseShape(StageCase? @case) => @case is null
        ? "none"
        : $"{@case.Decisions.Count}|{Lanes(@case.Architecture.Before)}|{Lanes(@case.Architecture.After)}|{@case.HardPart is not null}";

    private static string Lanes(IReadOnlyList<CaseLane> lanes) => string.Join(',', lanes.Select(l => l.Layers.Count));

    private static IEnumerable<string> SameSet(string what, IEnumerable<string> expected, IEnumerable<string> actual, string locale)
    {
        var missing = expected.Except(actual, StringComparer.Ordinal).ToList();
        var extra = actual.Except(expected, StringComparer.Ordinal).ToList();

        if (missing.Count > 0)
        {
            yield return $"{locale} is missing {what}(s): {string.Join(", ", missing)}";
        }

        if (extra.Count > 0)
        {
            yield return $"{locale} has {what}(s) that {Locales.ContentReference} doesn't: {string.Join(", ", extra)}";
        }
    }
}
