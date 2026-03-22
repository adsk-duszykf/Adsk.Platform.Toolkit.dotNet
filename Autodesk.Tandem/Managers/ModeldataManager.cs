using Autodesk.Tandem.Tandem.V1.Modeldata.Item.Create;
using Autodesk.Tandem.Tandem.V1.Modeldata.Item.History;
using Autodesk.Tandem.Tandem.V1.Modeldata.Item.Mutate;
using Autodesk.Tandem.Tandem.V1.Modeldata.Item.Scan;
using Autodesk.Tandem.Tandem.V1.Modeldata.Item.Schema;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Tandem.Managers;

/// <summary>
/// Manager for Modeldata operations
/// </summary>
public class ModeldataManager
{
    private readonly BaseTandemClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModeldataManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ModeldataManager(BaseTandemClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Creates elements based on the input payload. The scope of creation is one model.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/modeldata/{modelID}/create
    /// </remarks>
    /// <param name="modelId">Model URN</param>
    /// <param name="body">The element creation request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CreatePostResponse"/> containing the key and timestamp of the created element</returns>
    /// <example>
    /// <code>
    /// CreatePostResponse? result = await client.ModeldataManager.CreateElementsAsync("urn:adsk.dtm:...", new CreatePostRequestBody());
    /// </code>
    /// </example>
    public async Task<CreatePostResponse?> CreateElementsAsync(
        string modelId,
        CreatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Modeldata[modelId].Create
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns history of all changes for a given model.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/modeldata/{modelID}/history
    /// </remarks>
    /// <param name="modelId">Model URN</param>
    /// <param name="body">The history query request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="HistoryPostResponse"/> containing the change history</returns>
    /// <example>
    /// <code>
    /// HistoryPostResponse? history = await client.ModeldataManager.GetModelHistoryAsync("urn:adsk.dtm:...", new HistoryPostRequestBody { IncludeChanges = true });
    /// </code>
    /// </example>
    public async Task<HistoryPostResponse?> GetModelHistoryAsync(
        string modelId,
        HistoryPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Modeldata[modelId].History
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Bulk update of properties for multiple elements and attributes at the same time. The scope of mutation is always one model.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/modeldata/{modelID}/mutate
    /// </remarks>
    /// <param name="modelId">Model URN</param>
    /// <param name="body">The mutation request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="MutatePostResponse"/> containing the timestamp of when the change was applied</returns>
    /// <example>
    /// <code>
    /// MutatePostResponse? result = await client.ModeldataManager.MutateElementsAsync("urn:adsk.dtm:...", new MutatePostRequestBody());
    /// </code>
    /// </example>
    public async Task<MutatePostResponse?> MutateElementsAsync(
        string modelId,
        MutatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Modeldata[modelId].Mutate
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns an array of elements (including attribute values) matching your query.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/modeldata/{modelID}/scan
    /// </remarks>
    /// <param name="modelId">Model URN</param>
    /// <param name="body">The scan query request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ScanPostResponse"/> containing the matching elements</returns>
    /// <example>
    /// <code>
    /// ScanPostResponse? result = await client.ModeldataManager.ScanElementsAsync("urn:adsk.dtm:...", new ScanPostRequestBody { Families = ["std"] });
    /// </code>
    /// </example>
    public async Task<ScanPostResponse?> ScanElementsAsync(
        string modelId,
        ScanPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Modeldata[modelId].Scan
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns a model schema for a given model.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /tandem/v1/modeldata/{modelID}/schema
    /// </remarks>
    /// <param name="modelId">Model URN</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SchemaGetResponse"/> containing the model schema</returns>
    /// <example>
    /// <code>
    /// SchemaGetResponse? schema = await client.ModeldataManager.GetModelSchemaAsync("urn:adsk.dtm:...");
    /// </code>
    /// </example>
    public async Task<SchemaGetResponse?> GetModelSchemaAsync(
        string modelId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Modeldata[modelId].Schema
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
