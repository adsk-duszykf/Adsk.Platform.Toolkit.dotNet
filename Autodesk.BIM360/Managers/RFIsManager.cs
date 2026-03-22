using System.Runtime.CompilerServices;
using Autodesk.BIM360.Bim360.Rfis.V2.Containers.Item.Rfis;
using Autodesk.BIM360.Bim360.Rfis.V2.Containers.Item.Rfis.Item;
using Autodesk.BIM360.Bim360.Rfis.V2.Containers.Item.Rfis.Item.Attachments;
using Autodesk.BIM360.Bim360.Rfis.V2.Containers.Item.Rfis.Item.Comments;
using Autodesk.BIM360.Bim360.Rfis.V2.Containers.Item.Users.Me;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BIM360.Bim360.Rfis.V2.Containers.Item.Rfis.Item.Attachments.AttachmentsRequestBuilder;
using static Autodesk.BIM360.Bim360.Rfis.V2.Containers.Item.Rfis.Item.Comments.CommentsRequestBuilder;
using static Autodesk.BIM360.Bim360.Rfis.V2.Containers.Item.Rfis.RfisRequestBuilder;

namespace Autodesk.BIM360.Managers;

/// <summary>
/// Manager for BIM 360 RFIs (Project Management) — <c>/bim360/rfis/v2/containers/{containerId}/rfis</c>, comments, attachments, and current user.
/// The generated client exposes v2 only for this service.
/// </summary>
public class RFIsManager
{
    private readonly BaseBIM360client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="RFIsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public RFIsManager(BaseBIM360client api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves RFIs in the container with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/rfis/v2/containers/{containerId}/rfis
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/rfis-v2-rfis-GET
    /// </remarks>
    /// <param name="containerId">The RFI container ID for the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filtering, sorting, Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{RfisGetResponse_results}"/> of <see cref="RfisGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (RfisGetResponse_results rfi in client.RFIsManager.ListRfisAsync("container-guid"))
    /// {
    ///     Console.WriteLine(rfi.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<RfisGetResponse_results> ListRfisAsync(
        string containerId,
        RequestConfiguration<RfisRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Bim360.Rfis.V2.Containers[containerId]
                .Rfis
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
    /// Retrieves detailed information about a single RFI.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/rfis/v2/containers/{containerId}/rfis/{rfiId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/rfis-v2-rfis-id-GET
    /// </remarks>
    /// <param name="containerId">The RFI container ID for the project</param>
    /// <param name="rfiId">The RFI ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithRfiGetResponse"/> containing the RFI details, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithRfiGetResponse? rfi = await client.RFIsManager.GetRfiAsync("container-guid", "rfi-id");
    /// </code>
    /// </example>
    public async Task<WithRfiGetResponse?> GetRfiAsync(
        string containerId,
        string rfiId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Rfis.V2.Containers[containerId]
            .Rfis[rfiId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a new RFI in the container.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/rfis/v2/containers/{containerId}/rfis
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/rfis-v2-rfis-POST
    /// </remarks>
    /// <param name="containerId">The RFI container ID for the project</param>
    /// <param name="body">The RFI creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RfisPostResponse"/> containing the created RFI, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// RfisPostResponse? rfi = await client.RFIsManager.CreateRfiAsync("container-guid", new RfisPostRequestBody());
    /// </code>
    /// </example>
    public async Task<RfisPostResponse?> CreateRfiAsync(
        string containerId,
        RfisPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Rfis.V2.Containers[containerId]
            .Rfis
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates an existing RFI.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /bim360/rfis/v2/containers/{containerId}/rfis/{rfiId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/rfis-v2-rfis-id-PATCH
    /// </remarks>
    /// <param name="containerId">The RFI container ID for the project</param>
    /// <param name="rfiId">The RFI ID</param>
    /// <param name="body">The RFI update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithRfiPatchResponse"/> containing the updated RFI, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// WithRfiPatchResponse? updated = await client.RFIsManager.UpdateRfiAsync("container-guid", "rfi-id", new WithRfiPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithRfiPatchResponse?> UpdateRfiAsync(
        string containerId,
        string rfiId,
        WithRfiPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Rfis.V2.Containers[containerId]
            .Rfis[rfiId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns the current user context for RFIs in the container.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/rfis/v2/containers/{containerId}/users/me
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/rfis-v2-users-me-GET
    /// </remarks>
    /// <param name="containerId">The RFI container ID for the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="MeGetResponse"/> containing the current user profile and permissions, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// MeGetResponse? me = await client.RFIsManager.GetCurrentUserAsync("container-guid");
    /// </code>
    /// </example>
    public async Task<MeGetResponse?> GetCurrentUserAsync(
        string containerId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Rfis.V2.Containers[containerId]
            .Users
            .Me
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves comments for an RFI with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/rfis/v2/containers/{containerId}/rfis/{rfiId}/comments
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/rfis-v2-comments-GET
    /// </remarks>
    /// <param name="containerId">The RFI container ID for the project</param>
    /// <param name="rfiId">The RFI ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, sort, fields, filter)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{CommentsGetResponse_results}"/> of <see cref="CommentsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (CommentsGetResponse_results c in client.RFIsManager.ListRfiCommentsAsync("container-guid", "rfi-id"))
    /// {
    ///     Console.WriteLine(c.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CommentsGetResponse_results> ListRfiCommentsAsync(
        string containerId,
        string rfiId,
        RequestConfiguration<CommentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Bim360.Rfis.V2.Containers[containerId]
                .Rfis[rfiId]
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
    /// Adds a comment to an RFI.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/rfis/v2/containers/{containerId}/rfis/{rfiId}/comments
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/rfis-v2-comments-POST
    /// </remarks>
    /// <param name="containerId">The RFI container ID for the project</param>
    /// <param name="rfiId">The RFI ID</param>
    /// <param name="body">The comment creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CommentsPostResponse"/> containing the created comment, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// CommentsPostResponse? comment = await client.RFIsManager.CreateRfiCommentAsync("container-guid", "rfi-id", new CommentsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<CommentsPostResponse?> CreateRfiCommentAsync(
        string containerId,
        string rfiId,
        CommentsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Rfis.V2.Containers[containerId]
            .Rfis[rfiId]
            .Comments
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves attachments for an RFI with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/rfis/v2/containers/{containerId}/rfis/{rfiId}/attachments
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/rfis-v2-attachments-GET
    /// </remarks>
    /// <param name="containerId">The RFI container ID for the project</param>
    /// <param name="rfiId">The RFI ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset, sort, fields, filter)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{AttachmentsGetResponse_results}"/> of <see cref="AttachmentsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (AttachmentsGetResponse_results a in client.RFIsManager.ListRfiAttachmentsAsync("container-guid", "rfi-id"))
    /// {
    ///     Console.WriteLine(a.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<AttachmentsGetResponse_results> ListRfiAttachmentsAsync(
        string containerId,
        string rfiId,
        RequestConfiguration<AttachmentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Bim360.Rfis.V2.Containers[containerId]
                .Rfis[rfiId]
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
    /// Adds an attachment to an RFI.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /bim360/rfis/v2/containers/{containerId}/rfis/{rfiId}/attachments
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/rfis-v2-attachments-POST
    /// </remarks>
    /// <param name="containerId">The RFI container ID for the project</param>
    /// <param name="rfiId">The RFI ID</param>
    /// <param name="body">The attachment creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AttachmentsPostResponse"/> containing the created attachment, or <see langword="null"/>.</returns>
    /// <example>
    /// <code>
    /// AttachmentsPostResponse? attachment = await client.RFIsManager.CreateRfiAttachmentAsync("container-guid", "rfi-id", new AttachmentsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<AttachmentsPostResponse?> CreateRfiAttachmentAsync(
        string containerId,
        string rfiId,
        AttachmentsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Rfis.V2.Containers[containerId]
            .Rfis[rfiId]
            .Attachments
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes an attachment from an RFI.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /bim360/rfis/v2/containers/{containerId}/rfis/{rfiId}/attachments/{attachmentId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/rfis-v2-attachments-attachmentId-DELETE
    /// </remarks>
    /// <param name="containerId">The RFI container ID for the project</param>
    /// <param name="rfiId">The RFI ID</param>
    /// <param name="attachmentId">The attachment ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.RFIsManager.DeleteRfiAttachmentAsync("container-guid", "rfi-id", "attachment-id");
    /// </code>
    /// </example>
    public async Task DeleteRfiAttachmentAsync(
        string containerId,
        string rfiId,
        string attachmentId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Bim360.Rfis.V2.Containers[containerId]
            .Rfis[rfiId]
            .Attachments[attachmentId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
