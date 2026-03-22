using System.Runtime.CompilerServices;
using Autodesk.Automation.Da.UsEast.V3.Engines;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Automation.Da.UsEast.V3.Engines.EnginesRequestBuilder;
using EngineDetails = Autodesk.Automation.Da.UsEast.V3.Engines.Item.EnginesGetResponse;

namespace Autodesk.Automation.Managers;

/// <summary>
/// Manager for Engine operations — lists and retrieves details of Design Automation engines.
/// </summary>
public class EnginesManager
{
    private readonly BaseAutomationClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnginesManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public EnginesManager(BaseAutomationClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists all available Engines with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/engines
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/engines-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="string"/> engine IDs, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var engineId in client.EnginesManager.ListEnginesAsync())
    /// {
    ///     Console.WriteLine(engineId);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<string> ListEnginesAsync(
        RequestConfiguration<EnginesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? page = requestConfiguration?.QueryParameters?.Page;

        while (true)
        {
            var response = await _api.Da.UsEast.V3.Engines
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Page = page;
                }, cancellationToken);

            if (response?.Data is not { Count: > 0 })
                yield break;

            foreach (var item in response.Data)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.PaginationToken))
                yield break;

            page = response.PaginationToken;
        }
    }

    /// <summary>
    /// Gets the details of the specified Engine. The id parameter must be a QualifiedId (owner.name+label).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/engines/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/engines-id-GET
    /// </remarks>
    /// <param name="id">Full qualified id of the Engine (owner.name+label)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="EngineDetails"/> containing the engine details</returns>
    /// <example>
    /// <code>
    /// EngineDetails? engine = await client.EnginesManager.GetEngineAsync("Autodesk.AutoCAD+24");
    /// </code>
    /// </example>
    public async Task<EngineDetails?> GetEngineAsync(
        string id,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Engines[id]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
