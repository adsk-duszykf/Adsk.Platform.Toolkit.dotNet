using Autodesk.Tandem.Tandem.V1.ModelsRequests.Item.StreamConfigs;
using Autodesk.Tandem.Tandem.V1.ModelsRequests.Item.StreamConfigs.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Tandem.Managers;

/// <summary>
/// Manager for Stream Configs operations
/// </summary>
public class StreamConfigsManager
{
    private readonly BaseTandemClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamConfigsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public StreamConfigsManager(BaseTandemClient api)
    {
        _api = api;
    }

    /// <summary>
    /// List all stream configurations for a model.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /tandem/v1/models/{modelID}/stream-configs
    /// </remarks>
    /// <param name="modelId">Model URN</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="StreamConfigsGetResponse"/> containing the stream configurations</returns>
    /// <example>
    /// <code>
    /// StreamConfigsGetResponse? configs = await client.StreamConfigsManager.GetStreamConfigsAsync("urn:adsk.dtm:...");
    /// </code>
    /// </example>
    public async Task<StreamConfigsGetResponse?> GetStreamConfigsAsync(
        string modelId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Models[modelId].StreamConfigs
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Bulk update stream configurations. Full stream configuration for each stream must be included in the request, not just the updated fields.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /tandem/v1/models/{modelID}/stream-configs
    /// </remarks>
    /// <param name="modelId">Model URN</param>
    /// <param name="body">The bulk update request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="StreamConfigsPatchResponse"/> containing the timestamp of the applied change</returns>
    /// <example>
    /// <code>
    /// StreamConfigsPatchResponse? result = await client.StreamConfigsManager.BulkUpdateStreamConfigsAsync("urn:adsk.dtm:...", new StreamConfigsPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<StreamConfigsPatchResponse?> BulkUpdateStreamConfigsAsync(
        string modelId,
        StreamConfigsPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Models[modelId].StreamConfigs
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieve a specific stream configuration.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /tandem/v1/models/{modelID}/stream-configs/{elementID}
    /// </remarks>
    /// <param name="modelId">Model URN</param>
    /// <param name="elementId">Stream element ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithElementGetResponse"/> containing the stream configuration</returns>
    /// <example>
    /// <code>
    /// WithElementGetResponse? config = await client.StreamConfigsManager.GetStreamConfigAsync("urn:adsk.dtm:...", "elementId");
    /// </code>
    /// </example>
    public async Task<WithElementGetResponse?> GetStreamConfigAsync(
        string modelId,
        string elementId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Models[modelId].StreamConfigs[elementId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Save a stream configuration.
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /tandem/v1/models/{modelID}/stream-configs/{elementID}
    /// </remarks>
    /// <param name="modelId">Model URN</param>
    /// <param name="elementId">Stream element ID</param>
    /// <param name="body">The stream configuration to save</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithElementPutResponse"/> containing the timestamp of the applied change</returns>
    /// <example>
    /// <code>
    /// WithElementPutResponse? result = await client.StreamConfigsManager.SaveStreamConfigAsync("urn:adsk.dtm:...", "elementId", new WithElementPutRequestBody());
    /// </code>
    /// </example>
    public async Task<WithElementPutResponse?> SaveStreamConfigAsync(
        string modelId,
        string elementId,
        WithElementPutRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Models[modelId].StreamConfigs[elementId]
            .PutAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
