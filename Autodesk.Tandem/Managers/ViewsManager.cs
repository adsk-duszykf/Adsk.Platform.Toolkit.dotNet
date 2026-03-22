using Autodesk.Tandem.Tandem.V1.Twins.Item.Views;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Tandem.Managers;

/// <summary>
/// Manager for Views operations
/// </summary>
public class ViewsManager
{
    private readonly BaseTandemClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ViewsManager(BaseTandemClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns list of saved views for a given twin.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /tandem/v1/twins/{twinID}/views
    /// </remarks>
    /// <param name="twinId">Twin URN</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ViewsGetResponse"/> containing the saved views</returns>
    /// <example>
    /// <code>
    /// ViewsGetResponse? views = await client.ViewsManager.GetTwinViewsAsync("urn:adsk.dtt:...");
    /// </code>
    /// </example>
    public async Task<ViewsGetResponse?> GetTwinViewsAsync(
        string twinId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Twins[twinId].Views
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
