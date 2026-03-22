using Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item;
using Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.DiffsBatchStatus;
using Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item;
using Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.IndexesBatchStatus;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.BIM360.Managers;

/// <summary>
/// Manager for BIM 360 Model Properties (Construction Index) operations — indexes, diffs, queries,
/// fields, manifests, and properties for the Construction Index API.
/// </summary>
public class ModelPropertiesManager
{
    private readonly BaseBIM360client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelPropertiesManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ModelPropertiesManager(BaseBIM360client api)
    {
        _api = api;
    }

    #region Index Operations

    /// <summary>
    /// Retrieves the indexing status for the given index ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/index/v2/projects/{projectId}/indexes/{indexId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-index-status-get
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="indexId">The ID of the index</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithIndexGetResponse"/> containing the index status, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithIndexGetResponse? status = await client.ModelPropertiesManager.GetIndexStatusAsync(projectId, indexId);
    /// </code>
    /// </example>
    public async Task<WithIndexGetResponse?> GetIndexStatusAsync(
        Guid projectId,
        string indexId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Indexes[indexId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the fields dictionary associated with a properties index.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/index/v2/projects/{projectId}/indexes/{indexId}/fields
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-index-fields-get
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="indexId">The ID of the index</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Fields.FieldsGetResponse"/> containing the index fields, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Fields.FieldsGetResponse? fields = await client.ModelPropertiesManager.GetIndexFieldsAsync(projectId, indexId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Fields.FieldsGetResponse?> GetIndexFieldsAsync(
        Guid projectId,
        string indexId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Indexes[indexId]
            .Fields
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the manifest associated with a properties index.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/index/v2/projects/{projectId}/indexes/{indexId}/manifest
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-index-manifest-get
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="indexId">The ID of the index</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Manifest.ManifestGetResponse"/> containing the index manifest, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Manifest.ManifestGetResponse? manifest = await client.ModelPropertiesManager.GetIndexManifestAsync(projectId, indexId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Manifest.ManifestGetResponse?> GetIndexManifestAsync(
        Guid projectId,
        string indexId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Indexes[indexId]
            .Manifest
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the specific properties index.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/index/v2/projects/{projectId}/indexes/{indexId}/properties
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-index-properties-get
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="indexId">The ID of the index</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Properties.PropertiesGetResponse"/> containing the index properties, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Properties.PropertiesGetResponse? properties = await client.ModelPropertiesManager.GetIndexPropertiesAsync(projectId, indexId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Properties.PropertiesGetResponse?> GetIndexPropertiesAsync(
        Guid projectId,
        string indexId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Indexes[indexId]
            .Properties
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the job status for several index jobs in a single request.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/index/v2/projects/{projectId}/indexes:batch-status
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-index-jobs-batch-status-post
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The batch status request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="IndexesBatchStatusPostResponse"/> containing the batch status for index jobs, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// IndexesBatchStatusPostResponse? result = await client.ModelPropertiesManager.BatchGetIndexStatusAsync(projectId, new IndexesBatchStatusPostRequestBody());
    /// </code>
    /// </example>
    public async Task<IndexesBatchStatusPostResponse?> BatchGetIndexStatusAsync(
        Guid projectId,
        IndexesBatchStatusPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .IndexesBatchStatus
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Applies the given query on the given properties index.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/index/v2/projects/{projectId}/indexes/{indexId}/queries
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-index-query-post
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="indexId">The ID of the index</param>
    /// <param name="body">The query request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Queries.QueriesPostResponse"/> containing the query creation response, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Queries.QueriesPostResponse? response = await client.ModelPropertiesManager.CreateIndexQueryAsync(projectId, indexId, body);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Queries.QueriesPostResponse?> CreateIndexQueryAsync(
        Guid projectId,
        string indexId,
        Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Queries.QueriesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Indexes[indexId]
            .Queries
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the status of an index query.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/index/v2/projects/{projectId}/indexes/{indexId}/queries/{queryId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-index-query-job-status-get
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="indexId">The ID of the index</param>
    /// <param name="queryId">The ID of the query</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Queries.Item.WithQueryGetResponse"/> containing the index query status, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Queries.Item.WithQueryGetResponse? status = await client.ModelPropertiesManager.GetIndexQueryStatusAsync(projectId, indexId, queryId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Queries.Item.WithQueryGetResponse?> GetIndexQueryStatusAsync(
        Guid projectId,
        string indexId,
        string queryId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Indexes[indexId]
            .Queries[queryId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the query-specific properties index.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/index/v2/projects/{projectId}/indexes/{indexId}/queries/{queryId}/properties
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-index-query-properties-get
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="indexId">The ID of the index</param>
    /// <param name="queryId">The ID of the query</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Queries.Item.Properties.PropertiesGetResponse"/> containing the query-specific index properties, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Queries.Item.Properties.PropertiesGetResponse? properties = await client.ModelPropertiesManager.GetIndexQueryPropertiesAsync(projectId, indexId, queryId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Indexes.Item.Queries.Item.Properties.PropertiesGetResponse?> GetIndexQueryPropertiesAsync(
        Guid projectId,
        string indexId,
        string queryId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Indexes[indexId]
            .Queries[queryId]
            .Properties
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion

    #region Diff Operations

    /// <summary>
    /// Retrieves the diff status for the given diff ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/index/v2/projects/{projectId}/diffs/{diffId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-diff-status-get
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="diffId">The ID of the diff</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithDiffGetResponse"/> containing the diff status, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithDiffGetResponse? status = await client.ModelPropertiesManager.GetDiffStatusAsync(projectId, diffId);
    /// </code>
    /// </example>
    public async Task<WithDiffGetResponse?> GetDiffStatusAsync(
        Guid projectId,
        string diffId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Diffs[diffId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the fields dictionary associated with a diff.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/index/v2/projects/{projectId}/diffs/{diffId}/fields
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-diff-fields-get
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="diffId">The ID of the diff</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Fields.FieldsGetResponse"/> containing the diff fields, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Fields.FieldsGetResponse? fields = await client.ModelPropertiesManager.GetDiffFieldsAsync(projectId, diffId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Fields.FieldsGetResponse?> GetDiffFieldsAsync(
        Guid projectId,
        string diffId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Diffs[diffId]
            .Fields
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the manifest associated with a diff.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/index/v2/projects/{projectId}/diffs/{diffId}/manifest
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-diff-manifest-get
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="diffId">The ID of the diff</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Manifest.ManifestGetResponse"/> containing the diff manifest, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Manifest.ManifestGetResponse? manifest = await client.ModelPropertiesManager.GetDiffManifestAsync(projectId, diffId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Manifest.ManifestGetResponse?> GetDiffManifestAsync(
        Guid projectId,
        string diffId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Diffs[diffId]
            .Manifest
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the properties associated with a diff.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/index/v2/projects/{projectId}/diffs/{diffId}/properties
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-diff-properties-get
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="diffId">The ID of the diff</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Properties.PropertiesGetResponse"/> containing the diff properties, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Properties.PropertiesGetResponse? properties = await client.ModelPropertiesManager.GetDiffPropertiesAsync(projectId, diffId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Properties.PropertiesGetResponse?> GetDiffPropertiesAsync(
        Guid projectId,
        string diffId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Diffs[diffId]
            .Properties
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the job status for several diff jobs in a single request.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/index/v2/projects/{projectId}/diffs:batch-status
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-diff-jobs-batch-status-post
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The batch status request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="DiffsBatchStatusPostResponse"/> containing the batch status for diff jobs, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// DiffsBatchStatusPostResponse? result = await client.ModelPropertiesManager.BatchGetDiffStatusAsync(projectId, new DiffsBatchStatusPostRequestBody());
    /// </code>
    /// </example>
    public async Task<DiffsBatchStatusPostResponse?> BatchGetDiffStatusAsync(
        Guid projectId,
        DiffsBatchStatusPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .DiffsBatchStatus
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Applies the given query to the given diff.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/index/v2/projects/{projectId}/diffs/{diffId}/queries
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-diff-query-post
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="diffId">The ID of the diff</param>
    /// <param name="body">The query request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Queries.QueriesPostResponse"/> containing the diff query creation response, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Queries.QueriesPostResponse? response = await client.ModelPropertiesManager.CreateDiffQueryAsync(projectId, diffId, body);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Queries.QueriesPostResponse?> CreateDiffQueryAsync(
        Guid projectId,
        string diffId,
        Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Queries.QueriesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Diffs[diffId]
            .Queries
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the status of a diff query.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/index/v2/projects/{projectId}/diffs/{diffId}/queries/{queryId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-diff-query-job-status-get
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="diffId">The ID of the diff</param>
    /// <param name="queryId">The ID of the query</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Queries.Item.WithQueryGetResponse"/> containing the diff query status, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Queries.Item.WithQueryGetResponse? status = await client.ModelPropertiesManager.GetDiffQueryStatusAsync(projectId, diffId, queryId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Queries.Item.WithQueryGetResponse?> GetDiffQueryStatusAsync(
        Guid projectId,
        string diffId,
        string queryId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Diffs[diffId]
            .Queries[queryId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the query-specific properties of the given diff.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/index/v2/projects/{projectId}/diffs/{diffId}/queries/{queryId}/properties
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/index-v2-diff-query-properties-get
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="diffId">The ID of the diff</param>
    /// <param name="queryId">The ID of the query</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Queries.Item.Properties.PropertiesGetResponse"/> containing the query-specific diff properties, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Queries.Item.Properties.PropertiesGetResponse? properties = await client.ModelPropertiesManager.GetDiffQueryPropertiesAsync(projectId, diffId, queryId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Construction.IndexNamespace.V2.Projects.Item.Diffs.Item.Queries.Item.Properties.PropertiesGetResponse?> GetDiffQueryPropertiesAsync(
        Guid projectId,
        string diffId,
        string queryId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Index.V2.Projects[projectId]
            .Diffs[diffId]
            .Queries[queryId]
            .Properties
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion
}
