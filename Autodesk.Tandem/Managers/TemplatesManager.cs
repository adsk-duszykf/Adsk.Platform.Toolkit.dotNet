using Autodesk.Tandem.Tandem.V1.Twins.Item.Inlinetemplate;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Tandem.Managers;

/// <summary>
/// Manager for Templates operations
/// </summary>
public class TemplatesManager
{
    private readonly BaseTandemClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplatesManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public TemplatesManager(BaseTandemClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Get twin template (including classification).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /tandem/v1/twins/{twinID}/inlinetemplate
    /// </remarks>
    /// <param name="twinId">Twin URN</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="InlinetemplateGetResponse"/> containing the twin template and classification</returns>
    /// <example>
    /// <code>
    /// InlinetemplateGetResponse? template = await client.TemplatesManager.GetTwinTemplateAsync("urn:adsk.dtt:...");
    /// </code>
    /// </example>
    public async Task<InlinetemplateGetResponse?> GetTwinTemplateAsync(
        string twinId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Twins[twinId].Inlinetemplate
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
