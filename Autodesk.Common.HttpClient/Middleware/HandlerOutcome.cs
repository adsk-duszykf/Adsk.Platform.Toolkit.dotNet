namespace Autodesk.Common.HttpClientLibrary.Middleware;

/// <summary>
/// Verdict returned by <see cref="Options.CustomErrorHandlerOption.CustomErrorHandler"/>.
/// The middleware applies this to the current <see cref="System.Diagnostics.Activity"/> but makes
/// no judgment of its own. Throwing from the delegate is also supported and produces a span error
/// with the exception attached.
/// </summary>
/// <param name="IsError">If true, the span is marked <c>ActivityStatusCode.Error</c>.</param>
/// <param name="Description">Optional status description used when <paramref name="IsError"/> is true.</param>
public sealed record HandlerOutcome(bool IsError = false, string? Description = null)
{
    public static readonly HandlerOutcome Ok = new();
}
