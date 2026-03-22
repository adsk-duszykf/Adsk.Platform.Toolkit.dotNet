using Microsoft.Kiota.Abstractions;

namespace Autodesk.Automation.Managers;

/// <summary>
/// Manager for Health operations — checks the health status of Design Automation engines.
/// </summary>
public class HealthManager
{
    private readonly BaseAutomationClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public HealthManager(BaseAutomationClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Gets the health status by Engine or for all Engines.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/health/{engine}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/health-engine-GET
    /// </remarks>
    /// <param name="engine">Engine name, e.g. "AutoCAD"</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the health status response</returns>
    /// <example>
    /// <code>
    /// Stream? health = await client.HealthManager.GetHealthAsync("AutoCAD");
    /// </code>
    /// </example>
    public async Task<Stream?> GetHealthAsync(
        string engine,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Health[engine]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
