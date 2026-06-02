using System.Text.RegularExpressions;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Common.HttpClientLibrary.Middleware.Options;

public class ErrorHandlerOption : IRequestOption
{
    private static readonly Regex DefaultPattern = new(@"^2\d{2}$");

    /// <summary>
    /// If true, the error handler will throw an exception if the response has not a success status.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Regex pattern matched against the HTTP status code (as a string) to determine success.
    /// Defaults to 2xx.
    /// </summary>
    public Regex ValidStatusPattern { get; set; } = DefaultPattern;

    /// <summary>
    /// Returns true if the given HTTP status code matches <see cref="ValidStatusPattern"/>.
    /// </summary>
    public bool IsValidStatus(int statusCode) => ValidStatusPattern.IsMatch(statusCode.ToString());
}
