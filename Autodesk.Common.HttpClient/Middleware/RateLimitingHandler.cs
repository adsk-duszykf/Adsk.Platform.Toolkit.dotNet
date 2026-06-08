using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Autodesk.Common.HttpClientLibrary.Middleware.Options;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Http.HttpClientLibrary.Extensions;

namespace Autodesk.Common.HttpClientLibrary.Middleware;

public class RateLimitingHandler : DelegatingHandler
{
    private readonly RateLimitingHandlerOption _options;
    private readonly ConcurrentDictionary<string, RateLimiter> _rateLimiters = new();

    public RateLimitingHandler(RateLimitingHandlerOption? rateLimitingHandlerOption = null)
    {
        _options = rateLimitingHandlerOption ?? new RateLimitingHandlerOption();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var rateOptions = request.GetRequestOption<RateLimitingHandlerOption>() ?? _options;

        Activity? activity = null;
        if (request.GetRequestOption<ObservabilityOptions>() is { } obsOptions)
        {
            var activitySource = ActivitySourceRegistry.DefaultInstance.GetOrCreateActivitySource(obsOptions.TracerInstrumentationName);
            activity = activitySource.StartActivity($"{nameof(RateLimitingHandler)}_{nameof(SendAsync)}");
            activity?.SetTag("com.autodesk.handler.ratelimiting.enable", rateOptions.Enabled);
        }

        try
        {
            if (!rateOptions.Enabled)
                return await base.SendAsync(request, cancellationToken);

            var endpoint = GetEndpoint(request);
            var (maxConcurrentRequests, timeWindow) = ResolveRateLimit(rateOptions, endpoint);

            activity?.SetTag("com.autodesk.handler.ratelimiting.endpoint", endpoint);
            activity?.SetTag("com.autodesk.handler.ratelimiting.max_concurrent_requests", maxConcurrentRequests);
            activity?.SetTag("com.autodesk.handler.ratelimiting.time_window_ms", timeWindow.TotalMilliseconds);

            var rateLimiter = _rateLimiters.GetOrAdd(endpoint, _ => new RateLimiter(maxConcurrentRequests, timeWindow));

            var waitStart = Stopwatch.GetTimestamp();
            await rateLimiter.WaitForAvailabilityAsync();
            var waitMs = (Stopwatch.GetTimestamp() - waitStart) * 1000.0 / Stopwatch.Frequency;
            activity?.SetTag("com.autodesk.handler.ratelimiting.wait_ms", waitMs);

            return await base.SendAsync(request, cancellationToken);
        }
        finally
        {
            activity?.Dispose();
        }
    }

    private static (int MaxConcurrentRequests, TimeSpan TimeWindow) ResolveRateLimit(RateLimitingHandlerOption options, string endpoint)
    {
        foreach (var (pattern, limit) in options.EndpointOverrides)
        {
            if (IsGlobMatch(pattern, endpoint))
                return (limit.MaxConcurrentRequests, limit.TimeWindow);
        }

        return (options.MaxConcurrentRequests, options.TimeWindow);
    }

    private static bool IsGlobMatch(string pattern, string value)
    {
        // Convert glob pattern (with * wildcard) to regex
        var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
        return Regex.IsMatch(value, regexPattern, RegexOptions.IgnoreCase);
    }

    private static string GetEndpoint(HttpRequestMessage request)
    {
        return $"{request.Method}|{request.RequestUri?.GetLeftPart(UriPartial.Path) ?? ""}";
    }
}

public class RateLimiter
{
    private int _requestCount;
    private DateTime _resetTime;
    private readonly int _maxRequests;
    private readonly TimeSpan _timeWindow;
    private readonly SemaphoreSlim _semaphore;

    public RateLimiter(int maxRequests, TimeSpan timeWindow)
    {
        _maxRequests = maxRequests;
        _timeWindow = timeWindow;
        _resetTime = DateTime.UtcNow + _timeWindow;
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public async Task WaitForAvailabilityAsync()
    {
        await _semaphore.WaitAsync();

        try
        {
            if (DateTime.UtcNow >= _resetTime)
            {
                _requestCount = 0;
                _resetTime = DateTime.UtcNow + _timeWindow;
            }

            if (_requestCount >= _maxRequests)
            {
                var delay = _resetTime - DateTime.UtcNow;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay);
                }

                _requestCount = 0;
                _resetTime = DateTime.UtcNow + _timeWindow;
            }

            _requestCount++;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
