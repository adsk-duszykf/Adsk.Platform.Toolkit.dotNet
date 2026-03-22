using System.Runtime.CompilerServices;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.BIM360.Managers;

/// <summary>
/// Manager for BIM 360 Model Coordination operations — clash tests, clashes, model sets,
/// model set versions, screenshots, and clash viewpoints.
/// </summary>
public class ModelCoordinationManager
{
    private readonly BaseBIM360client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelCoordinationManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ModelCoordinationManager(BaseBIM360client api)
    {
        _api = api;
    }

    #region Model Sets

    /// <summary>
    /// Creates a model set within a given container.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/modelset/v3/containers/{containerId}/modelsets
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-create-model-set-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="body">The model set creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.ModelsetsPostResponse"/> containing the job response with model set and job IDs</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.ModelsetsPostResponse? response = await client.ModelCoordinationManager.CreateModelSetAsync(containerId, new ModelsetsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.ModelsetsPostResponse?> CreateModelSetAsync(
        Guid containerId,
        Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.ModelsetsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the first page of model sets in a given container.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/modelsets
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-model-sets-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.ModelsetsGetResponse_modelSets"/> items from the first page</returns>
    /// <example>
    /// <code>
    /// await foreach (var modelSet in client.ModelCoordinationManager.ListModelSetsAsync(containerId))
    /// {
    ///     Console.WriteLine(modelSet.ModelSetId);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.ModelsetsGetResponse_modelSets> ListModelSetsAsync(
        Guid containerId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets
            .GetAsync(r =>
                {
                    if (requestConfiguration != null)
                    {
                        r.Headers = requestConfiguration.Headers ?? r.Headers;
                        r.QueryParameters = requestConfiguration.QueryParameters ?? r.QueryParameters;
                        r.Options = requestConfiguration.Options ?? r.Options;
                    }
                }, cancellationToken);

        if (response?.ModelSets is not { Count: > 0 })
            yield break;

        foreach (var item in response.ModelSets)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Retrieves a single model set by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-model-set-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.WithModelSetGetResponse"/> containing the model set details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.WithModelSetGetResponse? modelSet = await client.ModelCoordinationManager.GetModelSetAsync(containerId, modelSetId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.WithModelSetGetResponse?> GetModelSetAsync(
        Guid containerId,
        Guid modelSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Updates a model set name and/or description.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-patch-model-set-name-description-PATCH
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="body">The update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.WithModelSetPatchResponse"/> containing the updated model set</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.WithModelSetPatchResponse? updated = await client.ModelCoordinationManager.UpdateModelSetAsync(containerId, modelSetId, new WithModelSetPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.WithModelSetPatchResponse?> UpdateModelSetAsync(
        Guid containerId,
        Guid modelSetId,
        Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.WithModelSetPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId]
            .PatchAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion

    #region Model Set Issues

    /// <summary>
    /// Adds a model set issue.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/issues
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-add-model-set-issue-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="body">The issue creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Issues.IssuesPostResponse"/> containing the created issue job response</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Issues.IssuesPostResponse? response = await client.ModelCoordinationManager.AddModelSetIssueAsync(containerId, modelSetId, new IssuesPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Issues.IssuesPostResponse?> AddModelSetIssueAsync(
        Guid containerId,
        Guid modelSetId,
        Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Issues.IssuesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Issues
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves model set issue view context.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/modelset/v3/containers/{containerId}/issues/viewcontext
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-model-set-issue-view-context-POST.rst
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="body">The view context request</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Issues.Viewcontext.ViewcontextPostResponse"/> containing the view context</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Issues.Viewcontext.ViewcontextPostResponse? context = await client.ModelCoordinationManager.GetModelSetIssueViewContextAsync(containerId, new ViewcontextPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Issues.Viewcontext.ViewcontextPostResponse?> GetModelSetIssueViewContextAsync(
        Guid containerId,
        Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Issues.Viewcontext.ViewcontextPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Issues.Viewcontext
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion

    #region Jobs

    /// <summary>
    /// Retrieves container-level job information.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/jobs/{jobId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-container-job-by-container-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="jobId">The GUID that uniquely identifies the job</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Jobs.Item.WithJobGetResponse"/> containing the job status and details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Jobs.Item.WithJobGetResponse? job = await client.ModelCoordinationManager.GetContainerJobAsync(containerId, jobId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Jobs.Item.WithJobGetResponse?> GetContainerJobAsync(
        Guid containerId,
        Guid jobId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Jobs[jobId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves model set job information.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/jobs/{jobId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-model-set-job-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="jobId">The GUID that uniquely identifies the job</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Jobs.Item.WithJobGetResponse"/> containing the job status and details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Jobs.Item.WithJobGetResponse? job = await client.ModelCoordinationManager.GetModelSetJobAsync(containerId, modelSetId, jobId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Jobs.Item.WithJobGetResponse?> GetModelSetJobAsync(
        Guid containerId,
        Guid modelSetId,
        Guid jobId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Jobs[jobId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion

    #region Model Set Screenshots (Modelset API)

    /// <summary>
    /// Adds a screenshot to a model set.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/screenshots
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-add-screen-shot-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="body">The screenshot creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Screenshots.ScreenshotsPostResponse"/> containing the created screenshot response</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Screenshots.ScreenshotsPostResponse? screenshot = await client.ModelCoordinationManager.CreateModelSetScreenshotAsync(containerId, modelSetId, new ScreenshotsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Screenshots.ScreenshotsPostResponse?> CreateModelSetScreenshotAsync(
        Guid containerId,
        Guid modelSetId,
        Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Screenshots.ScreenshotsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Screenshots
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a model set screenshot by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/screenshots/{screenShotId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-screen-shot-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="screenShotId">The GUID that uniquely identifies the screenshot</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the screenshot data</returns>
    /// <example>
    /// <code>
    /// System.IO.Stream? screenshot = await client.ModelCoordinationManager.GetModelSetScreenshotAsync(containerId, modelSetId, screenShotId);
    /// </code>
    /// </example>
    public async Task<Stream?> GetModelSetScreenshotAsync(
        Guid containerId,
        Guid modelSetId,
        Guid screenShotId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Screenshots[screenShotId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion

    #region Model Set Versions

    /// <summary>
    /// Creates a new model set version.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/versions
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-create-model-set-version-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.VersionsPostResponse"/> containing the created version job response</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.VersionsPostResponse? response = await client.ModelCoordinationManager.CreateModelSetVersionAsync(containerId, modelSetId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.VersionsPostResponse?> CreateModelSetVersionAsync(
        Guid containerId,
        Guid modelSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Versions
            .PostAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the first page of model set versions.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/versions
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-model-set-versions-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.VersionsGetResponse_modelSetVersions"/> items from the first page</returns>
    /// <example>
    /// <code>
    /// await foreach (var version in client.ModelCoordinationManager.ListModelSetVersionsAsync(containerId, modelSetId))
    /// {
    ///     Console.WriteLine(version.Version);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.VersionsGetResponse_modelSetVersions> ListModelSetVersionsAsync(
        Guid containerId,
        Guid modelSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Versions
            .GetAsync(r =>
                {
                    if (requestConfiguration != null)
                    {
                        r.Headers = requestConfiguration.Headers ?? r.Headers;
                        r.QueryParameters = requestConfiguration.QueryParameters ?? r.QueryParameters;
                        r.Options = requestConfiguration.Options ?? r.Options;
                    }
                }, cancellationToken);

        if (response?.ModelSetVersions is not { Count: > 0 })
            yield break;

        foreach (var item in response.ModelSetVersions)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Retrieves the latest model set version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/versions/latest
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-model-set-version-latest-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.Latest.LatestGetResponse"/> containing the latest version details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.Latest.LatestGetResponse? latest = await client.ModelCoordinationManager.GetModelSetVersionLatestAsync(containerId, modelSetId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.Latest.LatestGetResponse?> GetModelSetVersionLatestAsync(
        Guid containerId,
        Guid modelSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Versions.Latest
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a specific model set version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/versions/{version}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-model-set-version-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="version">The version number</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.Item.WithVersionGetResponse"/> containing the version details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.Item.WithVersionGetResponse? versionInfo = await client.ModelCoordinationManager.GetModelSetVersionAsync(containerId, modelSetId, 1);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.Item.WithVersionGetResponse?> GetModelSetVersionAsync(
        Guid containerId,
        Guid modelSetId,
        int version,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Versions[version]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Enables version creation for a model set.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/versions:enable
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-enable-model-set-versions-PATCH
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.VersionsEnable.VersionsEnablePatchResponse"/> containing the job response</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.VersionsEnable.VersionsEnablePatchResponse? enabled = await client.ModelCoordinationManager.EnableModelSetVersionsAsync(containerId, modelSetId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.VersionsEnable.VersionsEnablePatchResponse?> EnableModelSetVersionsAsync(
        Guid containerId,
        Guid modelSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].VersionsEnable
            .PatchAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Disables version creation for a model set.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/versions:disable
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-disable-model-set-versions-PATCH
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.VersionsDisable.VersionsDisablePatchResponse"/> containing the job response</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.VersionsDisable.VersionsDisablePatchResponse? disabled = await client.ModelCoordinationManager.DisableModelSetVersionsAsync(containerId, modelSetId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.VersionsDisable.VersionsDisablePatchResponse?> DisableModelSetVersionsAsync(
        Guid containerId,
        Guid modelSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].VersionsDisable
            .PatchAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion

    #region Model Set Views

    /// <summary>
    /// Creates a model set view.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/views
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-create-model-set-view-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="body">The view creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.ViewsPostResponse"/> containing the created view job response</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.ViewsPostResponse? response = await client.ModelCoordinationManager.CreateModelSetViewAsync(containerId, modelSetId, new ViewsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.ViewsPostResponse?> CreateModelSetViewAsync(
        Guid containerId,
        Guid modelSetId,
        Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.ViewsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Views
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the first page of model set views.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/views
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-model-set-views-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.ViewsGetResponse_modelSetViews"/> items from the first page</returns>
    /// <example>
    /// <code>
    /// await foreach (var view in client.ModelCoordinationManager.ListModelSetViewsAsync(containerId, modelSetId))
    /// {
    ///     Console.WriteLine(view.ViewId);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.ViewsGetResponse_modelSetViews> ListModelSetViewsAsync(
        Guid containerId,
        Guid modelSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Views
            .GetAsync(r =>
                {
                    if (requestConfiguration != null)
                    {
                        r.Headers = requestConfiguration.Headers ?? r.Headers;
                        r.QueryParameters = requestConfiguration.QueryParameters ?? r.QueryParameters;
                        r.Options = requestConfiguration.Options ?? r.Options;
                    }
                }, cancellationToken);

        if (response?.ModelSetViews is not { Count: > 0 })
            yield break;

        foreach (var item in response.ModelSetViews)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Searches model set view lineages.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/views:lineages
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-search-model-set-view-lineages-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="body">The search request</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.ViewsLineages.ViewsLineagesPostResponse"/> containing the lineage search response</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.ViewsLineages.ViewsLineagesPostResponse? lineages = await client.ModelCoordinationManager.SearchModelSetViewLineagesAsync(containerId, modelSetId, new ViewsLineagesPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.ViewsLineages.ViewsLineagesPostResponse?> SearchModelSetViewLineagesAsync(
        Guid containerId,
        Guid modelSetId,
        Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.ViewsLineages.ViewsLineagesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].ViewsLineages
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a model set view by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/views/{viewId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-model-set-view-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="viewId">The GUID that uniquely identifies the view</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.Item.WithViewGetResponse"/> containing the view details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.Item.WithViewGetResponse? view = await client.ModelCoordinationManager.GetModelSetViewAsync(containerId, modelSetId, viewId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.Item.WithViewGetResponse?> GetModelSetViewAsync(
        Guid containerId,
        Guid modelSetId,
        Guid viewId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Views[viewId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Updates a model set view.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/views/{viewId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-update-model-set-view-PATCH
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="viewId">The GUID that uniquely identifies the view</param>
    /// <param name="body">The update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.Item.WithViewPatchResponse"/> containing the updated view</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.Item.WithViewPatchResponse? updated = await client.ModelCoordinationManager.UpdateModelSetViewAsync(containerId, modelSetId, viewId, new WithViewPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.Item.WithViewPatchResponse?> UpdateModelSetViewAsync(
        Guid containerId,
        Guid modelSetId,
        Guid viewId,
        Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.Item.WithViewPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Views[viewId]
            .PatchAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Deletes a model set view.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/views/{viewId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-delete-model-set-view-DELETE
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="viewId">The GUID that uniquely identifies the view</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.ModelCoordinationManager.DeleteModelSetViewAsync(containerId, modelSetId, viewId);
    /// </code>
    /// </example>
    public async Task DeleteModelSetViewAsync(
        Guid containerId,
        Guid modelSetId,
        Guid viewId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Views[viewId]
            .DeleteAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves views for a model set version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/versions/{version}/views
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-model-set-view-versions-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="version">The model set version number</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.Item.Views.ViewsGetResponse"/> containing the views</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.Item.Views.ViewsGetResponse? views = await client.ModelCoordinationManager.GetModelSetVersionViewsAsync(containerId, modelSetId, 1);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.Item.Views.ViewsGetResponse?> GetModelSetVersionViewsAsync(
        Guid containerId,
        Guid modelSetId,
        int version,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Versions[version].Views
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a specific view within a model set version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/versions/{version}/views/{viewId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-model-set-view-version-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="version">The model set version number</param>
    /// <param name="viewId">The GUID that uniquely identifies the view</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.Item.Views.Item.WithViewGetResponse"/> containing the view details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.Item.Views.Item.WithViewGetResponse? view = await client.ModelCoordinationManager.GetModelSetVersionViewAsync(containerId, modelSetId, 1, viewId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Versions.Item.Views.Item.WithViewGetResponse?> GetModelSetVersionViewAsync(
        Guid containerId,
        Guid modelSetId,
        int version,
        Guid viewId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Versions[version].Views[viewId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves model set view job status.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/modelset/v3/containers/{containerId}/modelsets/{modelSetId}/views/{viewId}/jobs/{jobId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-get-model-set-view-job-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="viewId">The GUID that uniquely identifies the view</param>
    /// <param name="jobId">The GUID that uniquely identifies the job</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.Item.Jobs.Item.WithJobGetResponse"/> containing the job status</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.Item.Jobs.Item.WithJobGetResponse? job = await client.ModelCoordinationManager.GetModelSetViewJobAsync(containerId, modelSetId, viewId, jobId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Modelset.V3.Containers.Item.Modelsets.Item.Views.Item.Jobs.Item.WithJobGetResponse?> GetModelSetViewJobAsync(
        Guid containerId,
        Guid modelSetId,
        Guid viewId,
        Guid jobId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Modelset.V3.Containers[containerId].Modelsets[modelSetId].Views[viewId].Jobs[jobId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion

    #region Clash Tests

    /// <summary>
    /// Retrieves clash tests for a model set with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/clash/v3/containers/{containerId}/modelsets/{modelSetId}/tests
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-get-model-set-clash-tests-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Tests.TestsGetResponse"/> containing the clash tests</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Tests.TestsGetResponse? tests = await client.ModelCoordinationManager.GetModelSetClashTestsAsync(containerId, modelSetId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Tests.TestsGetResponse?> GetModelSetClashTestsAsync(
        Guid containerId,
        Guid modelSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Modelsets[modelSetId].Tests
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves clash tests for a model set version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/clash/v3/containers/{containerId}/modelsets/{modelSetId}/versions/{version}/tests
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-get-model-set-version-clash-tests-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="version">The model set version number</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Versions.Item.Tests.TestsGetResponse"/> containing the clash tests</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Versions.Item.Tests.TestsGetResponse? tests = await client.ModelCoordinationManager.GetModelSetVersionClashTestsAsync(containerId, modelSetId, 1);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Versions.Item.Tests.TestsGetResponse?> GetModelSetVersionClashTestsAsync(
        Guid containerId,
        Guid modelSetId,
        int version,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Modelsets[modelSetId].Versions[version].Tests
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a clash test by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/clash/v3/containers/{containerId}/tests/{testId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-get-clash-test-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="testId">The GUID that uniquely identifies the clash test</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.WithTestGetResponse"/> containing the clash test details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.WithTestGetResponse? test = await client.ModelCoordinationManager.GetClashTestAsync(containerId, testId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.WithTestGetResponse?> GetClashTestAsync(
        Guid containerId,
        Guid testId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Tests[testId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves clash test resources.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/clash/v3/containers/{containerId}/tests/{testId}/resources
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-get-clash-test-resources-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="testId">The GUID that uniquely identifies the clash test</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Resources.ResourcesGetResponse"/> containing the clash test resources</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Resources.ResourcesGetResponse? resources = await client.ModelCoordinationManager.GetClashTestResourcesAsync(containerId, testId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Resources.ResourcesGetResponse?> GetClashTestResourcesAsync(
        Guid containerId,
        Guid testId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Tests[testId].Resources
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion

    #region Closed Clash Groups

    /// <summary>
    /// Adds closed clash groups in batch.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/clash/v3/containers/{containerId}/tests/{testId}/clashes:close
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-add-closed-clash-group-batch-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="testId">The GUID that uniquely identifies the clash test</param>
    /// <param name="body">The batch close request</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.ClashesClose.ClashesClosePostResponse"/> containing the close job response</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.ClashesClose.ClashesClosePostResponse? response = await client.ModelCoordinationManager.AddClosedClashGroupsAsync(containerId, testId, new ClashesClosePostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.ClashesClose.ClashesClosePostResponse?> AddClosedClashGroupsAsync(
        Guid containerId,
        Guid testId,
        Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.ClashesClose.ClashesClosePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Tests[testId].ClashesClose
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the state of all closed clash groups for a clash test.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/clash/v3/containers/{containerId}/tests/{testId}/clashes/closed
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-get-clash-test-closed-clash-group-intersection-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="testId">The GUID that uniquely identifies the clash test</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Closed.ClosedGetResponse"/> containing the intersection state</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Closed.ClosedGetResponse? intersection = await client.ModelCoordinationManager.GetClosedClashGroupIntersectionAsync(containerId, testId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Closed.ClosedGetResponse?> GetClosedClashGroupIntersectionAsync(
        Guid containerId,
        Guid testId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Tests[testId].Clashes.Closed
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the state of specified closed clash groups for a clash test.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/clash/v3/containers/{containerId}/tests/{testId}/clashes/closed
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-get-closed-clash-group-data-batch-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="testId">The GUID that uniquely identifies the clash test</param>
    /// <param name="body">The batch request</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Closed.ClosedPostResponse"/> containing the closed clash group data</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Closed.ClosedPostResponse? data = await client.ModelCoordinationManager.GetClosedClashGroupBatchAsync(containerId, testId, new ClosedPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Closed.ClosedPostResponse?> GetClosedClashGroupBatchAsync(
        Guid containerId,
        Guid testId,
        Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Closed.ClosedPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Tests[testId].Clashes.Closed
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Reopens closed clash groups in batch.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/clash/v3/containers/{containerId}/modelsets/{modelSetId}/clashes:reopen
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-reopen-closed-clash-group-batch-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="body">The reopen request</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.ClashesReopen.ClashesReopenPostResponse"/> containing the reopen job response</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.ClashesReopen.ClashesReopenPostResponse? response = await client.ModelCoordinationManager.ReopenClosedClashGroupsAsync(containerId, modelSetId, new ClashesReopenPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.ClashesReopen.ClashesReopenPostResponse?> ReopenClosedClashGroupsAsync(
        Guid containerId,
        Guid modelSetId,
        Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.ClashesReopen.ClashesReopenPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Modelsets[modelSetId].ClashesReopen
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Searches closed clash groups for a container and model set.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/clash/v3/containers/{containerId}/modelsets/{modelSetId}/clashes/closed
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-search-container-model-set-closed-clash-groups-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Clashes.Closed.ClosedGetResponse"/> containing the closed clash groups</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Clashes.Closed.ClosedGetResponse? groups = await client.ModelCoordinationManager.SearchClosedClashGroupsAsync(containerId, modelSetId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Clashes.Closed.ClosedGetResponse?> SearchClosedClashGroupsAsync(
        Guid containerId,
        Guid modelSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Modelsets[modelSetId].Clashes.Closed
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion

    #region Assigned Clash Groups

    /// <summary>
    /// Adds assigned clash groups in batch.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/clash/v3/containers/{containerId}/tests/{testId}/clashes:assign
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-add-assigned-clash-group-batch-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="testId">The GUID that uniquely identifies the clash test</param>
    /// <param name="body">The batch assign request</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.ClashesAssign.ClashesAssignPostResponse"/> containing the assign job response</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.ClashesAssign.ClashesAssignPostResponse? response = await client.ModelCoordinationManager.AddAssignedClashGroupsAsync(containerId, testId, new ClashesAssignPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.ClashesAssign.ClashesAssignPostResponse?> AddAssignedClashGroupsAsync(
        Guid containerId,
        Guid testId,
        Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.ClashesAssign.ClashesAssignPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Tests[testId].ClashesAssign
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the state of all assigned clash groups for a clash test.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/clash/v3/containers/{containerId}/tests/{testId}/clashes/assigned
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-get-clash-test-assigned-clash-group-intersection-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="testId">The GUID that uniquely identifies the clash test</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Assigned.AssignedGetResponse"/> containing the intersection state</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Assigned.AssignedGetResponse? intersection = await client.ModelCoordinationManager.GetAssignedClashGroupIntersectionAsync(containerId, testId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Assigned.AssignedGetResponse?> GetAssignedClashGroupIntersectionAsync(
        Guid containerId,
        Guid testId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Tests[testId].Clashes.Assigned
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the state of specified assigned clash groups for a clash test.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/clash/v3/containers/{containerId}/tests/{testId}/clashes/assigned
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-get-assigned-clash-group-batch-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="testId">The GUID that uniquely identifies the clash test</param>
    /// <param name="body">The batch request</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Assigned.AssignedPostResponse"/> containing the assigned clash group data</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Assigned.AssignedPostResponse? data = await client.ModelCoordinationManager.GetAssignedClashGroupBatchAsync(containerId, testId, new AssignedPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Assigned.AssignedPostResponse?> GetAssignedClashGroupBatchAsync(
        Guid containerId,
        Guid testId,
        Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Tests.Item.Clashes.Assigned.AssignedPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Tests[testId].Clashes.Assigned
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Searches assigned clash groups in a model set (assigned clashes linked to issues).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/clash/v3/containers/{containerId}/modelsets/{modelSetId}/clashes/assigned
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-search-container-issue-clash-groups-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Clashes.Assigned.AssignedGetResponse"/> containing the assigned clash groups</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Clashes.Assigned.AssignedGetResponse? groups = await client.ModelCoordinationManager.SearchAssignedClashGroupsAsync(containerId, modelSetId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Clashes.Assigned.AssignedGetResponse?> SearchAssignedClashGroupsAsync(
        Guid containerId,
        Guid modelSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Modelsets[modelSetId].Clashes.Assigned
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves assigned clash group view context.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/clash/v3/containers/{containerId}/clashes/assigned/viewcontext
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-get-assigned-clash-group-view-context-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="body">The view context request</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Clashes.Assigned.Viewcontext.ViewcontextPostResponse"/> containing the view context</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Clashes.Assigned.Viewcontext.ViewcontextPostResponse? context = await client.ModelCoordinationManager.GetAssignedClashGroupViewContextAsync(containerId, new ViewcontextPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Clashes.Assigned.Viewcontext.ViewcontextPostResponse?> GetAssignedClashGroupViewContextAsync(
        Guid containerId,
        Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Clashes.Assigned.Viewcontext.ViewcontextPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Clashes.Assigned.Viewcontext
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves clash group job status.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/clash/v3/containers/{containerId}/clashes/jobs/{jobId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-get-clash-group-job-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="jobId">The GUID that uniquely identifies the job</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Clashes.Jobs.Item.WithJobGetResponse"/> containing the job status</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Clashes.Jobs.Item.WithJobGetResponse? job = await client.ModelCoordinationManager.GetClashGroupJobAsync(containerId, jobId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Clashes.Jobs.Item.WithJobGetResponse?> GetClashGroupJobAsync(
        Guid containerId,
        Guid jobId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Clashes.Jobs[jobId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion

    #region Clash Screenshots (Clash API)

    /// <summary>
    /// Adds a screenshot to a clash model set.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/clash/v3/containers/{containerId}/modelsets/{modelSetId}/screenshots
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-add-screen-shot-POST
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="body">The screenshot creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Screenshots.ScreenshotsPostResponse"/> containing the created screenshot response</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Screenshots.ScreenshotsPostResponse? screenshot = await client.ModelCoordinationManager.CreateClashScreenshotAsync(containerId, modelSetId, new ScreenshotsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Screenshots.ScreenshotsPostResponse?> CreateClashScreenshotAsync(
        Guid containerId,
        Guid modelSetId,
        Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Screenshots.ScreenshotsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Modelsets[modelSetId].Screenshots
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a clash screenshot by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/clash/v3/containers/{containerId}/modelsets/{modelSetId}/screenshots/{screenShotId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-get-screen-shot-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="screenShotId">The GUID that uniquely identifies the screenshot</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the screenshot data</returns>
    /// <example>
    /// <code>
    /// System.IO.Stream? screenshot = await client.ModelCoordinationManager.GetClashScreenshotAsync(containerId, modelSetId, screenShotId);
    /// </code>
    /// </example>
    public async Task<Stream?> GetClashScreenshotAsync(
        Guid containerId,
        Guid modelSetId,
        Guid screenShotId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Modelsets[modelSetId].Screenshots[screenShotId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion

    #region Grouped Clashes

    /// <summary>
    /// Retrieves grouped clashes for a model set.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/clash/v3/containers/{containerId}/modelsets/{modelSetId}/clashes/grouped
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/mc-clash-service-v3-get-grouped-clashes-GET
    /// </remarks>
    /// <param name="containerId">The GUID that uniquely identifies the container</param>
    /// <param name="modelSetId">The GUID that uniquely identifies the model set</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Clashes.Grouped.GroupedGetResponse"/> containing the grouped clashes</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Clashes.Grouped.GroupedGetResponse? grouped = await client.ModelCoordinationManager.GetGroupedClashesAsync(containerId, modelSetId);
    /// </code>
    /// </example>
    public async Task<Autodesk.BIM360.Bim360.Clash.V3.Containers.Item.Modelsets.Item.Clashes.Grouped.GroupedGetResponse?> GetGroupedClashesAsync(
        Guid containerId,
        Guid modelSetId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Clash.V3.Containers[containerId].Modelsets[modelSetId].Clashes.Grouped
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    #endregion
}
