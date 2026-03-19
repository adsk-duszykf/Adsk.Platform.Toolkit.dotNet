using Autodesk.ACC.Construction.Reviews.V1.Projects.Item.Reviews;
using Autodesk.ACC.Construction.Reviews.V1.Projects.Item.Reviews.Item;
using Autodesk.ACC.Construction.Reviews.V1.Projects.Item.Reviews.Item.Progress;
using Autodesk.ACC.Construction.Reviews.V1.Projects.Item.Reviews.Item.Workflow;
using Autodesk.ACC.Construction.Reviews.V1.Projects.Item.Versions.Item.ApprovalStatuses;
using Autodesk.ACC.Construction.Reviews.V1.Projects.Item.Workflows;
using Autodesk.ACC.Construction.Reviews.V1.Projects.Item.Workflows.Item;
using Microsoft.Kiota.Abstractions;
using ReviewVersionsRequestBuilder = Autodesk.ACC.Construction.Reviews.V1.Projects.Item.Reviews.Item.Versions.VersionsRequestBuilder;
using static Autodesk.ACC.Construction.Reviews.V1.Projects.Item.Reviews.ReviewsRequestBuilder;
using static Autodesk.ACC.Construction.Reviews.V1.Projects.Item.Workflows.WorkflowsRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Reviews operations — manages file reviews, approval workflows, and version
/// approval statuses in ACC projects.
/// </summary>
public class ReviewsManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ReviewsManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves all approval workflows used for file reviews in a given project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/reviews/v1/projects/{projectId}/workflows
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/reviews-workflows-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering by initiator, status; sorting)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WorkflowsGetResponse"/> containing the workflows</returns>
    /// <example>
    /// <code>
    /// WorkflowsGetResponse? workflows = await client.ReviewsManager.GetWorkflowsAsync("projectId");
    /// </code>
    /// </example>
    public async Task<WorkflowsGetResponse?> GetWorkflowsAsync(
        Guid projectId,
        Action<RequestConfiguration<WorkflowsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Reviews.V1.Projects[projectId]
            .Workflows
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates a new approval workflow in the specified project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/reviews/v1/projects/{projectId}/workflows
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/reviews-createworkflow-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The workflow creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WorkflowsPostResponse"/> containing the created workflow</returns>
    /// <example>
    /// <code>
    /// WorkflowsPostResponse? workflow = await client.ReviewsManager.CreateWorkflowAsync("projectId", new WorkflowsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<WorkflowsPostResponse?> CreateWorkflowAsync(
        Guid projectId,
        WorkflowsPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Reviews.V1.Projects[projectId]
            .Workflows
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves a specific approval workflow by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/reviews/v1/projects/{projectId}/workflows/{workflowId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/reviews-getworkflow-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="workflowId">The ID of the workflow</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithWorkflowGetResponse"/> containing the workflow details</returns>
    /// <example>
    /// <code>
    /// WithWorkflowGetResponse? workflow = await client.ReviewsManager.GetWorkflowAsync("projectId", "workflowId");
    /// </code>
    /// </example>
    public async Task<WithWorkflowGetResponse?> GetWorkflowAsync(
        Guid projectId,
        Guid workflowId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Reviews.V1.Projects[projectId]
            .Workflows[workflowId]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves the list of reviews created in the specified project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/reviews/v1/projects/{projectId}/reviews
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/reviews-reviews-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports extensive filtering and sorting)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ReviewsGetResponse"/> containing the reviews</returns>
    /// <example>
    /// <code>
    /// ReviewsGetResponse? reviews = await client.ReviewsManager.GetReviewsAsync("projectId");
    /// </code>
    /// </example>
    public async Task<ReviewsGetResponse?> GetReviewsAsync(
        Guid projectId,
        Action<RequestConfiguration<ReviewsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Reviews.V1.Projects[projectId]
            .Reviews
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Creates a new review in the specified project using an existing approval workflow.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/reviews/v1/projects/{projectId}/reviews
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/reviews-createreview-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The review creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ReviewsPostResponse"/> containing the created review</returns>
    /// <example>
    /// <code>
    /// ReviewsPostResponse? review = await client.ReviewsManager.CreateReviewAsync("projectId", new ReviewsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ReviewsPostResponse?> CreateReviewAsync(
        Guid projectId,
        ReviewsPostRequestBody body,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Reviews.V1.Projects[projectId]
            .Reviews
            .PostAsync(body, requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves a specific review by ID.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/reviews/v1/projects/{projectId}/reviews/{reviewId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/reviews-getreview-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="reviewId">The ID of the review</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithReviewGetResponse"/> containing the review details</returns>
    /// <example>
    /// <code>
    /// WithReviewGetResponse? review = await client.ReviewsManager.GetReviewAsync("projectId", "reviewId");
    /// </code>
    /// </example>
    public async Task<WithReviewGetResponse?> GetReviewAsync(
        Guid projectId,
        Guid reviewId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Reviews.V1.Projects[projectId]
            .Reviews[reviewId]
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves the approval workflow associated with a specific review.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/reviews/v1/projects/{projectId}/reviews/{reviewId}/workflow
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/reviews-getreviewworkflow-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="reviewId">The ID of the review</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WorkflowGetResponse"/> containing the workflow associated with the review</returns>
    /// <example>
    /// <code>
    /// WorkflowGetResponse? workflow = await client.ReviewsManager.GetReviewWorkflowAsync("projectId", "reviewId");
    /// </code>
    /// </example>
    public async Task<WorkflowGetResponse?> GetReviewWorkflowAsync(
        Guid projectId,
        Guid reviewId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Reviews.V1.Projects[projectId]
            .Reviews[reviewId]
            .Workflow
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves the progress of a specific review.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/reviews/v1/projects/{projectId}/reviews/{reviewId}/progress
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/reviews-getreviewprogress-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="reviewId">The ID of the review</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ProgressGetResponse"/> containing the review progress</returns>
    /// <example>
    /// <code>
    /// ProgressGetResponse? progress = await client.ReviewsManager.GetReviewProgressAsync("projectId", "reviewId");
    /// </code>
    /// </example>
    public async Task<ProgressGetResponse?> GetReviewProgressAsync(
        Guid projectId,
        Guid reviewId,
        Action<RequestConfiguration<ProgressRequestBuilder.ProgressRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Reviews.V1.Projects[projectId]
            .Reviews[reviewId]
            .Progress
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves the file versions included in the latest round of the specified review.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/reviews/v1/projects/{projectId}/reviews/{reviewId}/versions
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/reviews-getreviewversions-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="reviewId">The ID of the review</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports FilterapproveStatus, Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Construction.Reviews.V1.Projects.Item.Reviews.Item.Versions.VersionsGetResponse"/> containing the version details for the review</returns>
    /// <example>
    /// <code>
    /// Construction.Reviews.V1.Projects.Item.Reviews.Item.Versions.VersionsGetResponse? versions = await client.ReviewsManager.GetReviewVersionsAsync("projectId", "reviewId");
    /// </code>
    /// </example>
    public async Task<Construction.Reviews.V1.Projects.Item.Reviews.Item.Versions.VersionsGetResponse?> GetReviewVersionsAsync(
        Guid projectId,
        Guid reviewId,
        Action<RequestConfiguration<ReviewVersionsRequestBuilder.VersionsRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Reviews.V1.Projects[projectId]
            .Reviews[reviewId]
            .Versions
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves the full approval records and review references of a specific file version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/reviews/v1/projects/{projectId}/versions/{versionId}/approval-statuses
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/reviews-getversionapprovalstatuses-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="versionId">The ID of the version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ApprovalStatusesGetResponse"/> containing the approval statuses for the version</returns>
    /// <example>
    /// <code>
    /// ApprovalStatusesGetResponse? statuses = await client.ReviewsManager.GetApprovalStatusesAsync("projectId", "versionId");
    /// </code>
    /// </example>
    public async Task<ApprovalStatusesGetResponse?> GetApprovalStatusesAsync(
        Guid projectId,
        string versionId,
        Action<RequestConfiguration<ApprovalStatusesRequestBuilder.ApprovalStatusesRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Reviews.V1.Projects[projectId]
            .Versions[versionId]
            .ApprovalStatuses
            .GetAsync(requestConfiguration, cancellationToken);
    }
}
