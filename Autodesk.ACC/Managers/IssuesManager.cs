using System.Runtime.CompilerServices;
using Autodesk.ACC.Construction.Issues.V1.Projects.Item.Attachments;
using Autodesk.ACC.Construction.Issues.V1.Projects.Item.Attachments.Item.Items;
using Autodesk.ACC.Construction.Issues.V1.Projects.Item.IssueAttributeDefinitions;
using Autodesk.ACC.Construction.Issues.V1.Projects.Item.IssueAttributeMappings;
using Autodesk.ACC.Construction.Issues.V1.Projects.Item.IssueRootCauseCategories;
using Autodesk.ACC.Construction.Issues.V1.Projects.Item.IssueTypes;
using Autodesk.ACC.Construction.Issues.V1.Projects.Item.Issues;
using Autodesk.ACC.Construction.Issues.V1.Projects.Item.Issues.Item;
using Autodesk.ACC.Construction.Issues.V1.Projects.Item.Issues.Item.Comments;
using Autodesk.ACC.Construction.Issues.V1.Projects.Item.Users.Me;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.Construction.Issues.V1.Projects.Item.IssueAttributeDefinitions.IssueAttributeDefinitionsRequestBuilder;
using static Autodesk.ACC.Construction.Issues.V1.Projects.Item.IssueAttributeMappings.IssueAttributeMappingsRequestBuilder;
using static Autodesk.ACC.Construction.Issues.V1.Projects.Item.IssueRootCauseCategories.IssueRootCauseCategoriesRequestBuilder;
using static Autodesk.ACC.Construction.Issues.V1.Projects.Item.IssueTypes.IssueTypesRequestBuilder;
using static Autodesk.ACC.Construction.Issues.V1.Projects.Item.Issues.IssuesRequestBuilder;
using static Autodesk.ACC.Construction.Issues.V1.Projects.Item.Issues.Item.Comments.CommentsRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for ACC Issues operations — manages issues, comments, attachments, issue types,
/// attribute definitions, attribute mappings, and root cause categories.
/// </summary>
public class IssuesManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="IssuesManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public IssuesManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns the current user permissions for the project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/issues/v1/projects/{projectId}/users/me
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-users-me-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="MeGetResponse"/> containing the current user profile and permissions, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// MeGetResponse? me = await client.IssuesManager.GetCurrentUserAsync(projectId);
    /// </code>
    /// </example>
    public async Task<MeGetResponse?> GetCurrentUserAsync(
        Guid projectId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Issues.V1.Projects[projectId]
            .Users
            .Me
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a project's issue type categories and types with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/issues/v1/projects/{projectId}/issue-types
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-issue-types-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, filter, include)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{IssueTypesGetResponse_results}"/> of <see cref="IssueTypesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (IssueTypesGetResponse_results type in client.IssuesManager.ListIssueTypesAsync(projectId))
    /// {
    ///     Console.WriteLine(type.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<IssueTypesGetResponse_results> ListIssueTypesAsync(
        Guid projectId,
        RequestConfiguration<IssueTypesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Issues.V1.Projects[projectId]
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
            {
                yield return item;
            }

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
    /// Wraps: GET /construction/issues/v1/projects/{projectId}/issue-attribute-definitions
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-issue-attribute-definitions-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, filter)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{IssueAttributeDefinitionsGetResponse_results}"/> of <see cref="IssueAttributeDefinitionsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (IssueAttributeDefinitionsGetResponse_results def in client.IssuesManager.ListAttributeDefinitionsAsync(projectId))
    /// {
    ///     Console.WriteLine(def.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<IssueAttributeDefinitionsGetResponse_results> ListAttributeDefinitionsAsync(
        Guid projectId,
        RequestConfiguration<IssueAttributeDefinitionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Issues.V1.Projects[projectId]
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
            {
                yield return item;
            }

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
    /// Wraps: GET /construction/issues/v1/projects/{projectId}/issue-attribute-mappings
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-issue-attribute-mappings-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{IssueAttributeMappingsGetResponse_results}"/> of <see cref="IssueAttributeMappingsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (IssueAttributeMappingsGetResponse_results mapping in client.IssuesManager.ListAttributeMappingsAsync(projectId))
    /// {
    ///     Console.WriteLine(mapping.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<IssueAttributeMappingsGetResponse_results> ListAttributeMappingsAsync(
        Guid projectId,
        RequestConfiguration<IssueAttributeMappingsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Issues.V1.Projects[projectId]
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
            {
                yield return item;
            }

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
    /// Wraps: GET /construction/issues/v1/projects/{projectId}/issue-root-cause-categories
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-issue-root-cause-categories-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{IssueRootCauseCategoriesGetResponse_results}"/> of <see cref="IssueRootCauseCategoriesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (IssueRootCauseCategoriesGetResponse_results category in client.IssuesManager.ListRootCauseCategoriesAsync(projectId))
    /// {
    ///     Console.WriteLine(category.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<IssueRootCauseCategoriesGetResponse_results> ListRootCauseCategoriesAsync(
        Guid projectId,
        RequestConfiguration<IssueRootCauseCategoriesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Issues.V1.Projects[projectId]
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
            {
                yield return item;
            }

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves all issues in a project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/issues/v1/projects/{projectId}/issues
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-issues-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports extensive filtering and sorting)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{IssuesGetResponse_results}"/> of <see cref="IssuesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (IssuesGetResponse_results issue in client.IssuesManager.ListIssuesAsync(projectId))
    /// {
    ///     Console.WriteLine(issue.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<IssuesGetResponse_results> ListIssuesAsync(
        Guid projectId,
        RequestConfiguration<IssuesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Issues.V1.Projects[projectId]
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
            {
                yield return item;
            }

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Adds a new issue to a project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/issues/v1/projects/{projectId}/issues
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-issues-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The issue creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="IssuesPostResponse"/> containing the created issue, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// IssuesPostResponse? issue = await client.IssuesManager.CreateIssueAsync(projectId, new IssuesPostRequestBody());
    /// </code>
    /// </example>
    public async Task<IssuesPostResponse?> CreateIssueAsync(
        Guid projectId,
        IssuesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Issues.V1.Projects[projectId]
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
    /// Wraps: GET /construction/issues/v1/projects/{projectId}/issues/{issueId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-issues-issueId-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="issueId">The ID of the issue</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithIssueGetResponse"/> containing the issue details, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithIssueGetResponse? issue = await client.IssuesManager.GetIssueAsync(projectId, issueId);
    /// </code>
    /// </example>
    public async Task<WithIssueGetResponse?> GetIssueAsync(
        Guid projectId,
        Guid issueId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Issues.V1.Projects[projectId]
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
    /// Wraps: PATCH /construction/issues/v1/projects/{projectId}/issues/{issueId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-issues-issueId-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="issueId">The ID of the issue</param>
    /// <param name="body">The issue update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithIssuePatchResponse"/> containing the updated issue, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithIssuePatchResponse? updated = await client.IssuesManager.UpdateIssueAsync(projectId, issueId, new WithIssuePatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithIssuePatchResponse?> UpdateIssueAsync(
        Guid projectId,
        Guid issueId,
        WithIssuePatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Issues.V1.Projects[projectId]
            .Issues[issueId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves all comments for a specific issue with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/issues/v1/projects/{projectId}/issues/{issueId}/comments
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-comments-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="issueId">The ID of the issue</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, SortBy)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{CommentsGetResponse_results}"/> of <see cref="CommentsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (CommentsGetResponse_results comment in client.IssuesManager.ListCommentsAsync(projectId, issueId))
    /// {
    ///     Console.WriteLine(comment.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CommentsGetResponse_results> ListCommentsAsync(
        Guid projectId,
        Guid issueId,
        RequestConfiguration<CommentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Issues.V1.Projects[projectId]
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
            {
                yield return item;
            }

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Creates a new comment under a specific issue.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/issues/v1/projects/{projectId}/issues/{issueId}/comments
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-comments-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="issueId">The ID of the issue</param>
    /// <param name="body">The comment creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CommentsPostResponse"/> containing the created comment, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// CommentsPostResponse? comment = await client.IssuesManager.CreateCommentAsync(projectId, issueId, new CommentsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<CommentsPostResponse?> CreateCommentAsync(
        Guid projectId,
        Guid issueId,
        CommentsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Issues.V1.Projects[projectId]
            .Issues[issueId]
            .Comments
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Adds attachments to an existing issue.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/issues/v1/projects/{projectId}/attachments
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-attachments-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The attachments creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AttachmentsPostResponse"/> containing the created attachments, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// AttachmentsPostResponse? attachments = await client.IssuesManager.AddAttachmentsAsync(projectId, new AttachmentsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<AttachmentsPostResponse?> AddAttachmentsAsync(
        Guid projectId,
        AttachmentsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Issues.V1.Projects[projectId]
            .Attachments
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves all attachments for a specific issue in a project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/issues/v1/projects/{projectId}/attachments/{issueId}/items
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-attachments-issueId-items-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="issueId">The ID of the issue</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ItemsGetResponse"/> containing the attachments for the issue, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// ItemsGetResponse? attachments = await client.IssuesManager.GetAttachmentsAsync(projectId, issueId);
    /// </code>
    /// </example>
    public async Task<ItemsGetResponse?> GetAttachmentsAsync(
        Guid projectId,
        Guid issueId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Issues.V1.Projects[projectId]
            .Attachments[issueId]
            .Items
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes an attachment from an issue.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /construction/issues/v1/projects/{projectId}/attachments/{issueId}/items/{attachmentId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-items-attachmentId-DELETE
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="issueId">The ID of the issue</param>
    /// <param name="attachmentId">The ID of the attachment</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.IssuesManager.DeleteAttachmentAsync(projectId, issueId, attachmentId);
    /// </code>
    /// </example>
    public async Task DeleteAttachmentAsync(
        Guid projectId,
        Guid issueId,
        Guid attachmentId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Issues.V1.Projects[projectId]
            .Attachments[issueId]
            .Items[attachmentId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
