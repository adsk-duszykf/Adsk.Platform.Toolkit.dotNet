using Microsoft.Kiota.Abstractions;

namespace Autodesk.Common.HttpClientLibrary.Middleware.Options;

public class RateLimitingHandlerOption : IRequestOption
{
    /// <summary>
    /// Whether rate limiting is enabled. Default: false.
    /// </summary>
    public bool Enabled { get; private set; }

    /// <summary>
    /// Maximum number of requests allowed within the specified time window.
    /// </summary>
    public int MaxConcurrentRequests { get; private set; }

    /// <summary>
    /// Time window for the rate limiting.
    /// </summary>
    public TimeSpan TimeWindow { get; private set; }

    /// <summary>
    /// Per-endpoint rate limit overrides. Keys support glob-style patterns with * as wildcard.
    /// Format: "METHOD|/path/pattern" (e.g. "GET|/api/projects/*/items", "*|/api/slow-endpoint").
    /// The first matching pattern is used. When no pattern matches, the default limits apply.
    /// </summary>
    public Dictionary<string, (int MaxConcurrentRequests, TimeSpan TimeWindow)> EndpointOverrides { get; } = new();

    /// <summary>
    /// Sets the rate limit parameters and enables rate limiting.
    /// </summary>
    /// <param name="maxConcurrentRequests">Maximum number of concurrent requests.</param>
    /// <param name="timeWindow">Time window for the rate limiting.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetRateLimit(int maxConcurrentRequests, TimeSpan timeWindow)
    {
        if (maxConcurrentRequests <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxConcurrentRequests), "Max concurrent requests must be greater than zero.");
        if (timeWindow <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeWindow), "Time window must be greater than zero.");

        MaxConcurrentRequests = maxConcurrentRequests;
        TimeWindow = timeWindow;
        Enabled = true;
    }
}
