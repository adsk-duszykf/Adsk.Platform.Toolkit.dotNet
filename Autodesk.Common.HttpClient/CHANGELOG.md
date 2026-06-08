# Changelog — Adsk.Platform.HttpClient

All notable changes to this package are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [Unreleased]

### Breaking Changes

- **`ErrorHandler` and `ErrorHandlerOption` removed** — replaced by `CustomErrorHandler` and `CustomErrorHandlerOption`. Update any direct handler registrations or per-request option usage.
- **Default error handler now throws `ApiException`** instead of `HttpRequestException`. Update `catch` sites accordingly.
- **`CustomErrorHandlerOption.CustomErrorHandler`** delegate signature changed from `Action<HttpContext>` to `Func<HttpContext, HandlerOutcome>`:
  - Delegates that did not throw must now return `HandlerOutcome.Ok`.
  - Delegates that threw can still throw — no return needed on throw paths.
- **`RateLimitingHandlerOption.GetRateLimit()`** removed — use `Enabled`, `MaxConcurrentRequests`, and `TimeWindow` properties directly.
- **`RateLimitingHandlerOption.Disable()`** removed — rate limiting is disabled by default (call `SetRateLimit()` to enable).

### Added

- **`CustomErrorHandlerOption.CustomErrorHandler`** — `Func<HttpContext, HandlerOutcome>` delegate invoked for every response. The delegate decides what counts as an error; the middleware itself never throws.
  - Defaults to `DefaultCustomErrorHandler`, which throws `ApiException` for non-2xx responses (same behaviour as before).
  - Override to log, suppress, or conditionally throw based on full request/response context.
- **`CustomErrorHandlerOption.DefaultCustomErrorHandler(ctx, Regex pattern)`** — reuse the built-in logic with a custom success-status pattern:
  ```csharp
  var option = new CustomErrorHandlerOption
  {
      CustomErrorHandler = ctx =>
          CustomErrorHandlerOption.DefaultCustomErrorHandler(ctx, new Regex(@"^(2\d{2}|404)$"))
  };
  ```
- **`HandlerOutcome`** — return type for the delegate:
  - `HandlerOutcome.Ok` — response is success, no action needed.
  - `new HandlerOutcome(IsError: true, Description: "…")` — mark the span as an error without throwing, response is still returned to the caller.
- **`HttpContext`** record — full request/response snapshot passed to the delegate and attached to thrown exceptions via `Data["context"]`:
  - Properties: `RequestUri`, `RequestMethod`, `RequestHeaders`, `RequestContent`, `StatusCode`, `ResponseHeaders`, `ResponseContent`, `ResponseContentAsString`.
  - Streams are buffered and rewindable; detached copies survive response disposal.

---

