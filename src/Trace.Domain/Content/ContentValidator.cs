namespace Trace.Domain.Content;

/// <summary>Checks that the three locales are translations of the same content (same keys, same IDs, same shape).</summary>
public static class ContentValidator
{
    public static IEnumerable<string> Validate(ContentStore store)
    {
        var reference = store.For(Locales.Default);

        foreach (var locale in Locales.All.Where(l => l != Locales.Default))
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

            if (reference.Log.Count != other.Log.Count)
            {
                yield return $"log.json has {reference.Log.Count} entries in {Locales.Default} but {other.Log.Count} in {locale}";
            }

            if (reference.Identity.CareerStartYear != other.Identity.CareerStartYear)
            {
                yield return $"identity.json careerStartYear differs in {locale}";
            }

            if (reference.Identity.LinkedIn != other.Identity.LinkedIn)
            {
                yield return $"identity.json LinkedIn URL differs in {locale}";
            }
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
            yield return $"{locale} has {what}(s) that {Locales.Default} doesn't: {string.Join(", ", extra)}";
        }
    }
}
