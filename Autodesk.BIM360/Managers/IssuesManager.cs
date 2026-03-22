using System.Runtime.CompilerServices;
using Autodesk.BIM360.Issues.V2.Containers.Item.IssueAttributeDefinitions;
using Autodesk.BIM360.Issues.V2.Containers.Item.IssueAttributeMappings;
using Autodesk.BIM360.Issues.V2.Containers.Item.IssueRootCauseCategories;
using Autodesk.BIM360.Issues.V2.Containers.Item.IssueTypes;
using Autodesk.BIM360.Issues.V2.Containers.Item.Issues;
using Autodesk.BIM360.Issues.V2.Containers.Item.Issues.Item;
using Autodesk.BIM360.Issues.V2.Containers.Item.Issues.Item.Attachments;
using Autodesk.BIM360.Issues.V2.Containers.Item.Issues.Item.Attachments.Item;
using Autodesk.BIM360.Issues.V2.Containers.Item.Issues.Item.Comments;
using Autodesk.BIM360.Issues.V2.Containers.Item.Users.Me;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BIM360.Issues.V2.Containers.Item.IssueAttributeDefinitions.IssueAttributeDefinitionsRequestBuilder;
using static Autodesk.BIM360.Issues.V2.Containers.Item.IssueAttributeMappings.IssueAttributeMappingsRequestBuilder;
using static Autodesk.BIM360.Issues.V2.Containers.Item.IssueRootCauseCategories.IssueRootCauseCategoriesRequestBuilder;
using static Autodesk.BIM360.Issues.V2.Containers.Item.IssueTypes.IssueTypesRequestBuilder;
using static Autodesk.BIM360.Issues.V2.Containers.Item.Issues.IssuesRequestBuilder;
using static Autodesk.BIM360.Issues.V2.Containers.Item.Issues.Item.Attachments.AttachmentsRequestBuilder;
using static Autodesk.BIM360.Issues.V2.Containers.Item.Issues.Item.Comments.CommentsRequestBuilder;

namespace Autodesk.BIM360.Managers;

/// <summary>
/// Manager for BIM 360 Issues API (Issues v2) — issues under <c>/issues/v2/containers/{containerId}/issues</c>,
/// comments, attachments, issue types, attribute definitions, mappings, and root-cause categories.
/// </summary>
public class IssuesManager
{
    private readonly BaseBIM360client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="IssuesManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public IssuesManager(BaseBIM360client api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns the current user permissions for the issues container.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /issues/v2/containers/{containerId}/users/me
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-users-me-GET
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="MeGetResponse"/> containing the current user profile and permissions, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// MeGetResponse? me = await client.IssuesManager.GetCurrentUserAsync("container-guid");
    /// </code>
    /// </example>
    public async Task<MeGetResponse?> GetCurrentUserAsync(
        string containerId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Issues.V2.Containers[containerId]
            .Users
            .Me
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves issue type categories and types with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /issues/v2/containers/{containerId}/issue-types
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-issue-types-GET
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, filter, include)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{IssueTypesGetResponse_results}"/> of <see cref="IssueTypesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (IssueTypesGetResponse_results type in client.IssuesManager.ListIssueTypesAsync("container-guid"))
    /// {
    ///     Console.WriteLine(type.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<IssueTypesGetResponse_results> ListIssueTypesAsync(
        string containerId,
        RequestConfiguration<IssueTypesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Issues.V2.Containers[containerId]
                .IssueTypes
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves issue attribute definitions (custom fields) with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /issues/v2/containers/{containerId}/issue-attribute-definitions
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-issue-attribute-definitions-GET
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, filter)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{IssueAttributeDefinitionsGetResponse_results}"/> of <see cref="IssueAttributeDefinitionsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (IssueAttributeDefinitionsGetResponse_results def in client.IssuesManager.ListAttributeDefinitionsAsync("container-guid"))
    /// {
    ///     Console.WriteLine(def.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<IssueAttributeDefinitionsGetResponse_results> ListAttributeDefinitionsAsync(
        string containerId,
        RequestConfiguration<IssueAttributeDefinitionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Issues.V2.Containers[containerId]
                .IssueAttributeDefinitions
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves issue attribute mappings with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /issues/v2/containers/{containerId}/issue-attribute-mappings
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-issue-attribute-mappings-GET
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{IssueAttributeMappingsGetResponse_results}"/> of <see cref="IssueAttributeMappingsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (IssueAttributeMappingsGetResponse_results mapping in client.IssuesManager.ListAttributeMappingsAsync("container-guid"))
    /// {
    ///     Console.WriteLine(mapping.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<IssueAttributeMappingsGetResponse_results> ListAttributeMappingsAsync(
        string containerId,
        RequestConfiguration<IssueAttributeMappingsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Issues.V2.Containers[containerId]
                .IssueAttributeMappings
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves issue root cause categories with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /issues/v2/containers/{containerId}/issue-root-cause-categories
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-issue-root-cause-categories-GET
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{IssueRootCauseCategoriesGetResponse_results}"/> of <see cref="IssueRootCauseCategoriesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (IssueRootCauseCategoriesGetResponse_results category in client.IssuesManager.ListRootCauseCategoriesAsync("container-guid"))
    /// {
    ///     Console.WriteLine(category.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<IssueRootCauseCategoriesGetResponse_results> ListRootCauseCategoriesAsync(
        string containerId,
        RequestConfiguration<IssueRootCauseCategoriesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Issues.V2.Containers[containerId]
                .IssueRootCauseCategories
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves all issues in the container with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /issues/v2/containers/{containerId}/issues
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-issues-GET
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering, sorting, Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{IssuesGetResponse_results}"/> of <see cref="IssuesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (IssuesGetResponse_results issue in client.IssuesManager.ListIssuesAsync("container-guid"))
    /// {
    ///     Console.WriteLine(issue.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<IssuesGetResponse_results> ListIssuesAsync(
        string containerId,
        RequestConfiguration<IssuesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Issues.V2.Containers[containerId]
                .Issues
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Adds a new issue to the container.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /issues/v2/containers/{containerId}/issues
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-issues-POST
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="body">The issue creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="IssuesPostResponse"/> containing the created issue, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// IssuesPostResponse? issue = await client.IssuesManager.CreateIssueAsync("container-guid", new IssuesPostRequestBody());
    /// </code>
    /// </example>
    public async Task<IssuesPostResponse?> CreateIssueAsync(
        string containerId,
        IssuesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Issues.V2.Containers[containerId]
            .Issues
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves detailed information about a single issue.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /issues/v2/containers/{containerId}/issues/{issueId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-issues-issueId-GET
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="issueId">The issue ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithIssueGetResponse"/> containing the issue details, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithIssueGetResponse? issue = await client.IssuesManager.GetIssueAsync("container-guid", "issue-id");
    /// </code>
    /// </example>
    public async Task<WithIssueGetResponse?> GetIssueAsync(
        string containerId,
        string issueId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Issues.V2.Containers[containerId]
            .Issues[issueId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates an issue.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /issues/v2/containers/{containerId}/issues/{issueId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-issues-issueId-PATCH
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="issueId">The issue ID</param>
    /// <param name="body">The issue update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithIssuePatchResponse"/> containing the updated issue, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithIssuePatchResponse? updated = await client.IssuesManager.UpdateIssueAsync("container-guid", "issue-id", new WithIssuePatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithIssuePatchResponse?> UpdateIssueAsync(
        string containerId,
        string issueId,
        WithIssuePatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Issues.V2.Containers[containerId]
            .Issues[issueId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves comments for an issue with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /issues/v2/containers/{containerId}/issues/{issueId}/comments
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-comments-GET
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="issueId">The issue ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{CommentsGetResponse_results}"/> of <see cref="CommentsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (CommentsGetResponse_results comment in client.IssuesManager.ListCommentsAsync("container-guid", "issue-id"))
    /// {
    ///     Console.WriteLine(comment.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CommentsGetResponse_results> ListCommentsAsync(
        string containerId,
        string issueId,
        RequestConfiguration<CommentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Issues.V2.Containers[containerId]
                .Issues[issueId]
                .Comments
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Creates a comment on an issue.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /issues/v2/containers/{containerId}/issues/{issueId}/comments
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-comments-POST
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="issueId">The issue ID</param>
    /// <param name="body">The comment creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CommentsPostResponse"/> containing the created comment, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// CommentsPostResponse? comment = await client.IssuesManager.CreateCommentAsync("container-guid", "issue-id", new CommentsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<CommentsPostResponse?> CreateCommentAsync(
        string containerId,
        string issueId,
        CommentsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Issues.V2.Containers[containerId]
            .Issues[issueId]
            .Comments
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves attachments for an issue with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /issues/v2/containers/{containerId}/issues/{issueId}/attachments
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-attachments-GET
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="issueId">The issue ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, filter[status])</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{AttachmentsGetResponse_results}"/> of <see cref="AttachmentsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (AttachmentsGetResponse_results att in client.IssuesManager.ListAttachmentsAsync("container-guid", "issue-id"))
    /// {
    ///     Console.WriteLine(att.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<AttachmentsGetResponse_results> ListAttachmentsAsync(
        string containerId,
        string issueId,
        RequestConfiguration<AttachmentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Issues.V2.Containers[containerId]
                .Issues[issueId]
                .Attachments
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
                yield return item;

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Creates a new issue attachment record (metadata); use the returned upload targets to send file bytes, then complete processing via <see cref="CompleteIssueAttachmentUploadAsync"/>.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /issues/v2/containers/{containerId}/issues/{issueId}/attachments
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-attachments-POST
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="issueId">The issue ID</param>
    /// <param name="body">The attachment creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AttachmentsPostResponse"/> containing the created attachment metadata, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// AttachmentsPostResponse? created = await client.IssuesManager.CreateAttachmentAsync("container-guid", "issue-id", new AttachmentsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<AttachmentsPostResponse?> CreateAttachmentAsync(
        string containerId,
        string issueId,
        AttachmentsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Issues.V2.Containers[containerId]
            .Issues[issueId]
            .Attachments
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Notifies the service after uploading attachment file data, or checks association status (task <c>post-upload-process</c>).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /issues/v2/containers/{containerId}/issues/{issueId}/attachments/{attachmentId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/issues-v2-attachments-attachmentId-POST
    /// </remarks>
    /// <param name="containerId">The issues container ID for the project</param>
    /// <param name="issueId">The issue ID</param>
    /// <param name="attachmentId">The attachment ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (query: task)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithAttachmentPostResponse"/>, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithAttachmentPostResponse? status = await client.IssuesManager.CompleteIssueAttachmentUploadAsync("container-guid", "issue-id", "attachment-id", null);
    /// </code>
    /// </example>
    public async Task<WithAttachmentPostResponse?> CompleteIssueAttachmentUploadAsync(
        string containerId,
        string issueId,
        string attachmentId,
        RequestConfiguration<WithAttachmentItemRequestBuilder.WithAttachmentItemRequestBuilderPostQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Issues.V2.Containers[containerId]
            .Issues[issueId]
            .Attachments[attachmentId]
            .PostAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
