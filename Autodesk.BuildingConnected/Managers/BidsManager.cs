using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Bids;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Bids.Attachments;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Bids.Item;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Bids.Item.Attachments.Item;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Bids.Item.LineItems;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Bids.Item.Plugs;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Bids.BidsRequestBuilder;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Bids.Item.LineItems.LineItemsRequestBuilder;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Bids.Item.Plugs.PlugsRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected bid, bid attachment, line item, and plug operations.
/// </summary>
public class BidsManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="BidsManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public BidsManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists bids received by the user&apos;s company with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/bids
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bids-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="BidsRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="BidsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (BidsGetResponse_results bid in client.BidsManager.ListBidsAsync())
    /// {
    ///     Console.WriteLine(bid.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<BidsGetResponse_results> ListBidsAsync(
        RequestConfiguration<BidsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            BidsGetResponse? response = await _api.Construction.Buildingconnected.V2.Bids
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (BidsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Retrieves a single bid by identifier.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/bids/{bidId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bids-bidId-GET
    /// </remarks>
    /// <param name="bidId">The unique ID of the bid.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for headers and middleware options (default query parameters).</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithBidGetResponse"/> for the bid, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// WithBidGetResponse? bid = await client.BidsManager.GetBidAsync("bid-id");
    /// </code>
    /// </example>
    public async Task<WithBidGetResponse?> GetBidAsync(
        string bidId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Bids[bidId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a host bid (creator type <c>HOST</c>).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/bids
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bids-POST
    /// </remarks>
    /// <param name="body">The bid creation payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="BidsPostResponse"/> describing the created bid.</returns>
    /// <example>
    /// <code>
    /// BidsPostResponse? created = await client.BidsManager.CreateBidAsync(new BidsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<BidsPostResponse?> CreateBidAsync(
        BidsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Bids
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a bid attachment that can be referenced from a bid&apos;s attachments array.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/bids/attachments
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bids-attachments-POST
    /// </remarks>
    /// <param name="body">The attachment creation payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="AttachmentsPostResponse"/> describing the created attachment.</returns>
    /// <example>
    /// <code>
    /// AttachmentsPostResponse? attachment =
    ///     await client.BidsManager.CreateBidAttachmentAsync(new AttachmentsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<AttachmentsPostResponse?> CreateBidAttachmentAsync(
        AttachmentsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Bids.Attachments
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a bid attachment by identifier (top-level attachments path).
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /construction/buildingconnected/v2/bids/attachments/{attachmentId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bids-attachments-attachmentId-DELETE
    /// </remarks>
    /// <param name="attachmentId">The ID of the file attached to a bid proposal.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="Task"/> that completes when the attachment is deleted.</returns>
    /// <example>
    /// <code>
    /// await client.BidsManager.DeleteBidAttachmentAsync("attachment-id");
    /// </code>
    /// </example>
    public async Task DeleteBidAttachmentAsync(
        string attachmentId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Buildingconnected.V2.Bids.Attachments[attachmentId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves metadata for a file attached to a specific bid.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/bids/{bidId}/attachments/{attachmentId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bids-bidId-attachments-attachmentId-GET
    /// </remarks>
    /// <param name="bidId">The unique ID of the bid.</param>
    /// <param name="attachmentId">The ID of the file attached to the bid proposal.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for headers and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithAttachmentGetResponse"/>, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// WithAttachmentGetResponse? file = await client.BidsManager.GetBidAttachmentAsync("bid-id", "attachment-id");
    /// </code>
    /// </example>
    public async Task<WithAttachmentGetResponse?> GetBidAttachmentAsync(
        string bidId,
        string attachmentId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Bids[bidId].Attachments[attachmentId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists line items for a bid with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/bids/{bidId}/line-items
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bids-bidId-line-items-GET
    /// </remarks>
    /// <param name="bidId">The unique ID of the bid.</param>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="LineItemsRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="LineItemsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (LineItemsGetResponse_results lineItem in client.BidsManager.ListBidLineItemsAsync("bid-id"))
    /// {
    ///     Console.WriteLine(lineItem.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<LineItemsGetResponse_results> ListBidLineItemsAsync(
        string bidId,
        RequestConfiguration<LineItemsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            LineItemsGetResponse? response = await _api.Construction.Buildingconnected.V2.Bids[bidId].LineItems
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (LineItemsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Lists plugs (bid leveling adjustments) for a bid with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/bids/{bidId}/plugs
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-bids-bidId-plugs-GET
    /// </remarks>
    /// <param name="bidId">The unique ID of the bid.</param>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="PlugsRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="PlugsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (PlugsGetResponse_results plug in client.BidsManager.ListBidPlugsAsync("bid-id"))
    /// {
    ///     Console.WriteLine(plug.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<PlugsGetResponse_results> ListBidPlugsAsync(
        string bidId,
        RequestConfiguration<PlugsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            PlugsGetResponse? response = await _api.Construction.Buildingconnected.V2.Bids[bidId].Plugs
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (PlugsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }
}
