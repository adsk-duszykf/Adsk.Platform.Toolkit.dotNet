using Microsoft.Kiota.Abstractions;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>
/// Provides shared request configuration behavior for PIM360 managers.
/// </summary>
public abstract class Pim360ManagerBase
{
    /// <summary>
    /// Copies common request settings from a public manager configuration to a generated request configuration.
    /// </summary>
    protected static void Configure<T>(RequestConfiguration<T>? source, RequestConfiguration<T> target)
        where T : class, new()
    {
        target.Headers = source?.Headers ?? target.Headers;
        target.Options = source?.Options ?? target.Options;
    }
}
