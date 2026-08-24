using Autodesk.Datum.CLS360.Domains.Item.Attributes;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Datum.CLS360.Managers;

/// <summary>
/// Manager for attribute operations.
/// </summary>
public class AttributeManager
{
    private readonly BaseDatumCLS360Client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeManager"/> class.
    /// </summary>
    /// <param name="api">The API client.</param>
    public AttributeManager(BaseDatumCLS360Client api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns attributes (information and measure) for a domain.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /domains/{domHdl}/attributes
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="AttributesGetResponse"/> containing matched attributes.</returns>
    /// <example>
    /// <code>
    /// AttributesGetResponse? response = await client.AttributeManager.GetAttributesForDomainAsync("domain-handle");
    /// </code>
    /// </example>
    public async Task<AttributesGetResponse?> GetAttributesForDomainAsync(
        string domHdl,
        RequestConfiguration<AttributesRequestBuilder.AttributesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].Attributes.GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }
}
