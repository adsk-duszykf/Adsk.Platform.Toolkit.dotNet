using System.Collections.Concurrent;
using System.Diagnostics;

namespace Autodesk.Common.HttpClientLibrary.Middleware;

/// <summary>
/// Caches <see cref="ActivitySource"/> instances by instrumentation name so middleware
/// handlers can reuse a single source per name across requests.
/// </summary>
internal sealed class ActivitySourceRegistry
{
    public static readonly ActivitySourceRegistry DefaultInstance = new();

    private readonly ConcurrentDictionary<string, ActivitySource> _sources = new(StringComparer.Ordinal);

    public ActivitySource GetOrCreateActivitySource(string name)
    {
        return _sources.GetOrAdd(name, static n => new ActivitySource(n));
    }
}
