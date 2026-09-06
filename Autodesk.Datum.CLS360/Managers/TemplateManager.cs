using System.Runtime.CompilerServices;
using Autodesk.Datum.CLS360.Domains.Item.Templates;
using Autodesk.Datum.CLS360.Domains.Item.Templates.Item;
using Autodesk.Datum.CLS360.Domains.Item.Templates.TypeNamespace.Item.EssentialAttributes;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Datum.CLS360.Managers;

/// <summary>
/// Manager for template operations.
/// </summary>
public class TemplateManager
{
    private readonly BaseDatumCLS360Client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateManager"/> class.
    /// </summary>
    /// <param name="api">The API client.</param>
    public TemplateManager(BaseDatumCLS360Client api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns essential attributes required by item type.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /domains/{domHdl}/templates/type/{type}/EssentialAttributes
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="type">Item type (for example: TAGGED_ITEM, EQUIPMENT_ITEM, EQUIPMENT_MODEL, DOCUMENT).</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="EssentialAttributesGetResponse"/> containing essential attributes.</returns>
    /// <example>
    /// <code>
    /// EssentialAttributesGetResponse? response = await client.TemplateManager.GetEssentialAttributesAsync("domain-handle", "TAGGED_ITEM");
    /// </code>
    /// </example>
    public async Task<EssentialAttributesGetResponse?> GetEssentialAttributesAsync(
        string domHdl,
        string type,
        RequestConfiguration<EssentialAttributesRequestBuilder.EssentialAttributesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].Templates.Type[type].EssentialAttributes.GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }

    /// <summary>
    /// Returns template objects for the given domain.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /domains/{domHdl}/templates
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="TemplatesGetResponse_rows"/> items.</returns>
    /// <example>
    /// <code>
    /// await foreach (TemplatesGetResponse_rows template in client.TemplateManager.ListTemplatesAsync("domain-handle"))
    /// {
    ///     Console.WriteLine(template.Hdl);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<TemplatesGetResponse_rows> ListTemplatesAsync(
        string domHdl,
        RequestConfiguration<TemplatesRequestBuilder.TemplatesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // The API has no page/pageSize params for this endpoint; it always returns the full list in one call.
        TemplatesGetResponse? response = await _api.Domains[domHdl].Templates.GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);

        if (response?.Rows is null)
            yield break;

        foreach (var item in response.Rows)
            yield return item;
    }

    /// <summary>
    /// Creates a new template in the given domain.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /domains/{domHdl}/templates
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="body">The create request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The handle of the newly created template.</returns>
    /// <example>
    /// <code>
    /// string? handle = await client.TemplateManager.CreateTemplateAsync("domain-handle", new TemplatesPostRequestBody());
    /// </code>
    /// </example>
    public async Task<string?> CreateTemplateAsync(
        string domHdl,
        TemplatesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].Templates.PostAsync(body, r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }

    /// <summary>
    /// Returns a template object selected by handle.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /domains/{domHdl}/templates/{tmpHdl}
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="tmpHdl">Template handle.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithTmpHdlGetResponse"/> containing the template object.</returns>
    /// <example>
    /// <code>
    /// WithTmpHdlGetResponse? response = await client.TemplateManager.GetTemplateAsync("domain-handle", "template-handle");
    /// </code>
    /// </example>
    public async Task<WithTmpHdlGetResponse?> GetTemplateAsync(
        string domHdl,
        string tmpHdl,
        RequestConfiguration<WithTmpHdlItemRequestBuilder.WithTmpHdlItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].Templates[tmpHdl].GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }

    /// <summary>
    /// Updates an existing template.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /domains/{domHdl}/templates/{tmpHdl}
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="tmpHdl">Template handle.</param>
    /// <param name="body">The update request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>"OK" if successful, otherwise an error message.</returns>
    /// <example>
    /// <code>
    /// string? result = await client.TemplateManager.UpdateTemplateAsync("domain-handle", "template-handle", new WithTmpHdlPostRequestBody());
    /// </code>
    /// </example>
    public async Task<string?> UpdateTemplateAsync(
        string domHdl,
        string tmpHdl,
        WithTmpHdlPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].Templates[tmpHdl].PostAsync(body, r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }
}
