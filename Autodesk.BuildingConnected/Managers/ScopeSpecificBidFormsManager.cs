using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ScopeSpecificBidForms;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ScopeSpecificBidForms.Item;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ScopeSpecificBidForms.Item.LineItems;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ScopeSpecificBidForms.Item.LineItems.Item;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ScopeSpecificBidForms.Item.LineItemsBatchCreate;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ScopeSpecificBidForms.Item.LineItemsBatchDelete;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ScopeSpecificBidForms.Item.LineItemsBatchPatch;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ScopeSpecificBidForms.Item.LineItems.LineItemsRequestBuilder;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ScopeSpecificBidForms.ScopeSpecificBidFormsRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected scope-specific bid forms and their line items (cursor-based list pagination).
/// </summary>
public class ScopeSpecificBidFormsManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeSpecificBidFormsManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public ScopeSpecificBidFormsManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists scope-specific bid forms for the user&apos;s company with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/scope-specific-bid-forms
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-scope-specific-bid-forms-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="ScopeSpecificBidFormsRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ScopeSpecificBidFormsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (ScopeSpecificBidFormsGetResponse_results form in client.ScopeSpecificBidFormsManager.ListScopeSpecificBidFormsAsync())
    /// {
    ///     Console.WriteLine(form.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ScopeSpecificBidFormsGetResponse_results> ListScopeSpecificBidFormsAsync(
        RequestConfiguration<ScopeSpecificBidFormsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ScopeSpecificBidFormsGetResponse? response = await _api.Construction.Buildingconnected.V2.ScopeSpecificBidForms
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (ScopeSpecificBidFormsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Retrieves a single scope-specific bid form by identifier.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/scope-specific-bid-forms/{formId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-scope-specific-bid-forms-formId-GET
    /// </remarks>
    /// <param name="formId">The scope-specific bid form identifier.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for headers and middleware options (default query parameters).</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithFormGetResponse"/> for the form, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// WithFormGetResponse? form = await client.ScopeSpecificBidFormsManager.GetScopeSpecificBidFormAsync("form-id");
    /// </code>
    /// </example>
    public async Task<WithFormGetResponse?> GetScopeSpecificBidFormAsync(
        string formId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.ScopeSpecificBidForms[formId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a scope-specific bid form for a bid package.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/scope-specific-bid-forms
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-scope-specific-bid-forms-POST
    /// </remarks>
    /// <param name="body">The request body for creating the form.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="ScopeSpecificBidFormsPostResponse"/> from the API, or <c>null</c>.</returns>
    /// <example>
    /// <code>
    /// ScopeSpecificBidFormsPostResponse? created = await client.ScopeSpecificBidFormsManager.CreateScopeSpecificBidFormAsync(new ScopeSpecificBidFormsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ScopeSpecificBidFormsPostResponse?> CreateScopeSpecificBidFormAsync(
        ScopeSpecificBidFormsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.ScopeSpecificBidForms
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists line items for a scope-specific bid form with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/scope-specific-bid-forms/{formId}/line-items
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-scope-specific-bid-forms-formId-line-items-GET
    /// </remarks>
    /// <param name="formId">The scope-specific bid form identifier.</param>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="LineItemsRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="LineItemsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (LineItemsGetResponse_results lineItem in client.ScopeSpecificBidFormsManager.ListLineItemsAsync("form-id"))
    /// {
    ///     Console.WriteLine(lineItem.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<LineItemsGetResponse_results> ListLineItemsAsync(
        string formId,
        RequestConfiguration<LineItemsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            LineItemsGetResponse? response = await _api.Construction.Buildingconnected.V2.ScopeSpecificBidForms[formId].LineItems
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
    /// Creates a line item on a scope-specific bid form.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/scope-specific-bid-forms/{formId}/line-items
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-scope-specific-bid-forms-formId-line-items-POST
    /// </remarks>
    /// <param name="formId">The scope-specific bid form identifier.</param>
    /// <param name="body">The line item creation payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="LineItemsPostResponse"/> from the API, or <c>null</c>.</returns>
    /// <example>
    /// <code>
    /// LineItemsPostResponse? created = await client.ScopeSpecificBidFormsManager.CreateLineItemAsync("form-id", new LineItemsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<LineItemsPostResponse?> CreateLineItemAsync(
        string formId,
        LineItemsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.ScopeSpecificBidForms[formId].LineItems
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a line item on a scope-specific bid form.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/buildingconnected/v2/scope-specific-bid-forms/{formId}/line-items/{lineItemId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-scope-specific-bid-forms-formId-line-items-lineItemId-PATCH
    /// </remarks>
    /// <param name="formId">The scope-specific bid form identifier.</param>
    /// <param name="lineItemId">The line item identifier.</param>
    /// <param name="body">The line item patch payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithLineItemPatchResponse"/> from the API, or <c>null</c>.</returns>
    /// <example>
    /// <code>
    /// WithLineItemPatchResponse? updated = await client.ScopeSpecificBidFormsManager.UpdateLineItemAsync(
    ///     "form-id",
    ///     "line-item-id",
    ///     new WithLineItemPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithLineItemPatchResponse?> UpdateLineItemAsync(
        string formId,
        string lineItemId,
        WithLineItemPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.ScopeSpecificBidForms[formId].LineItems[lineItemId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a line item from a scope-specific bid form.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /construction/buildingconnected/v2/scope-specific-bid-forms/{formId}/line-items/{lineItemId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-scope-specific-bid-forms-formId-line-items-lineItemId-DELETE
    /// </remarks>
    /// <param name="formId">The scope-specific bid form identifier.</param>
    /// <param name="lineItemId">The line item identifier.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A task that completes when the delete request finishes.</returns>
    /// <example>
    /// <code>
    /// await client.ScopeSpecificBidFormsManager.DeleteLineItemAsync("form-id", "line-item-id");
    /// </code>
    /// </example>
    public async Task DeleteLineItemAsync(
        string formId,
        string lineItemId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Buildingconnected.V2.ScopeSpecificBidForms[formId].LineItems[lineItemId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates multiple line items on a scope-specific bid form (batch).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/scope-specific-bid-forms/{formId}/line-items:batch-create
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-scope-specific-bid-forms-formId-line-items-batch-create-POST
    /// </remarks>
    /// <param name="formId">The scope-specific bid form identifier.</param>
    /// <param name="body">The batch-create request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="LineItemsBatchCreatePostResponse"/> from the API, or <c>null</c>.</returns>
    /// <example>
    /// <code>
    /// LineItemsBatchCreatePostResponse? batch = await client.ScopeSpecificBidFormsManager.BatchCreateLineItemsAsync(
    ///     "form-id",
    ///     new LineItemsBatchCreatePostRequestBody());
    /// </code>
    /// </example>
    public async Task<LineItemsBatchCreatePostResponse?> BatchCreateLineItemsAsync(
        string formId,
        LineItemsBatchCreatePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.ScopeSpecificBidForms[formId].LineItemsBatchCreate
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates multiple line items on a scope-specific bid form (batch).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/buildingconnected/v2/scope-specific-bid-forms/{formId}/line-items:batch-patch
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-scope-specific-bid-forms-formId-line-items-batch-patch-PATCH
    /// </remarks>
    /// <param name="formId">The scope-specific bid form identifier.</param>
    /// <param name="body">The batch-patch request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="LineItemsBatchPatchPatchResponse"/> from the API, or <c>null</c>.</returns>
    /// <example>
    /// <code>
    /// LineItemsBatchPatchPatchResponse? batch = await client.ScopeSpecificBidFormsManager.BatchPatchLineItemsAsync(
    ///     "form-id",
    ///     new LineItemsBatchPatchPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<LineItemsBatchPatchPatchResponse?> BatchPatchLineItemsAsync(
        string formId,
        LineItemsBatchPatchPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.ScopeSpecificBidForms[formId].LineItemsBatchPatch
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes multiple line items from a scope-specific bid form (batch).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/scope-specific-bid-forms/{formId}/line-items:batch-delete
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-scope-specific-bid-forms-formId-line-items-batch-delete-POST
    /// </remarks>
    /// <param name="formId">The scope-specific bid form identifier.</param>
    /// <param name="body">The list of line item identifiers to delete.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A task that completes when the batch delete request finishes.</returns>
    /// <example>
    /// <code>
    /// List&lt;string&gt; ids = new List&lt;string&gt; { "line-item-1", "line-item-2" };
    /// await client.ScopeSpecificBidFormsManager.BatchDeleteLineItemsAsync("form-id", ids);
    /// </code>
    /// </example>
    public async Task BatchDeleteLineItemsAsync(
        string formId,
        List<string> body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Buildingconnected.V2.ScopeSpecificBidForms[formId].LineItemsBatchDelete
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
