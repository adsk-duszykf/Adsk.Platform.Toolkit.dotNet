using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ProjectBidForms;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ProjectBidForms.Item;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ProjectBidForms.Item.LineItems;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ProjectBidForms.Item.LineItems.Item;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ProjectBidForms.Item.LineItemsBatchCreate;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ProjectBidForms.Item.LineItemsBatchDelete;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ProjectBidForms.Item.LineItemsBatchPatch;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ProjectBidForms.Item.LineItems.LineItemsRequestBuilder;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.ProjectBidForms.ProjectBidFormsRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected project bid forms and their line items (cursor-based list pagination).
/// </summary>
public class ProjectBidFormsManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectBidFormsManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public ProjectBidFormsManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists project bid forms for the user&apos;s company with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/project-bid-forms
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-bid-forms-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="ProjectBidFormsRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ProjectBidFormsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (ProjectBidFormsGetResponse_results form in client.ProjectBidFormsManager.ListProjectBidFormsAsync())
    /// {
    ///     Console.WriteLine(form.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ProjectBidFormsGetResponse_results> ListProjectBidFormsAsync(
        RequestConfiguration<ProjectBidFormsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ProjectBidFormsGetResponse? response = await _api.Construction.Buildingconnected.V2.ProjectBidForms
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (ProjectBidFormsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Retrieves a single project bid form by identifier.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/project-bid-forms/{formId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-bid-forms-formId-GET
    /// </remarks>
    /// <param name="formId">The project bid form identifier.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for headers and middleware options (default query parameters).</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithFormGetResponse"/> for the form, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// WithFormGetResponse? form = await client.ProjectBidFormsManager.GetProjectBidFormAsync("form-id");
    /// </code>
    /// </example>
    public async Task<WithFormGetResponse?> GetProjectBidFormAsync(
        string formId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.ProjectBidForms[formId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a project bid form for a BuildingConnected project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/project-bid-forms
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-bid-forms-POST
    /// </remarks>
    /// <param name="body">The request body for creating the form.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="ProjectBidFormsPostResponse"/> from the API, or <c>null</c>.</returns>
    /// <example>
    /// <code>
    /// ProjectBidFormsPostResponse? created = await client.ProjectBidFormsManager.CreateProjectBidFormAsync(new ProjectBidFormsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ProjectBidFormsPostResponse?> CreateProjectBidFormAsync(
        ProjectBidFormsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.ProjectBidForms
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists line items for a project bid form with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/project-bid-forms/{formId}/line-items
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-bid-forms-formId-line-items-GET
    /// </remarks>
    /// <param name="formId">The project bid form identifier.</param>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="LineItemsRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="LineItemsGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (LineItemsGetResponse_results lineItem in client.ProjectBidFormsManager.ListLineItemsAsync("form-id"))
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
            LineItemsGetResponse? response = await _api.Construction.Buildingconnected.V2.ProjectBidForms[formId].LineItems
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
    /// Creates a line item on a project bid form.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/project-bid-forms/{formId}/line-items
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-bid-forms-formId-line-items-POST
    /// </remarks>
    /// <param name="formId">The project bid form identifier.</param>
    /// <param name="body">The line item creation payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="LineItemsPostResponse"/> from the API, or <c>null</c>.</returns>
    /// <example>
    /// <code>
    /// LineItemsPostResponse? created = await client.ProjectBidFormsManager.CreateLineItemAsync("form-id", new LineItemsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<LineItemsPostResponse?> CreateLineItemAsync(
        string formId,
        LineItemsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.ProjectBidForms[formId].LineItems
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a line item on a project bid form.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/buildingconnected/v2/project-bid-forms/{formId}/line-items/{lineItemId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-bid-forms-formId-line-items-lineItemId-PATCH
    /// </remarks>
    /// <param name="formId">The project bid form identifier.</param>
    /// <param name="lineItemId">The line item identifier.</param>
    /// <param name="body">The line item patch payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithLineItemPatchResponse"/> from the API, or <c>null</c>.</returns>
    /// <example>
    /// <code>
    /// WithLineItemPatchResponse? updated = await client.ProjectBidFormsManager.UpdateLineItemAsync(
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
        return await _api.Construction.Buildingconnected.V2.ProjectBidForms[formId].LineItems[lineItemId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a line item from a project bid form.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /construction/buildingconnected/v2/project-bid-forms/{formId}/line-items/{lineItemId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-bid-forms-formId-line-items-lineItemId-DELETE
    /// </remarks>
    /// <param name="formId">The project bid form identifier.</param>
    /// <param name="lineItemId">The line item identifier.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A task that completes when the delete request finishes.</returns>
    /// <example>
    /// <code>
    /// await client.ProjectBidFormsManager.DeleteLineItemAsync("form-id", "line-item-id");
    /// </code>
    /// </example>
    public async Task DeleteLineItemAsync(
        string formId,
        string lineItemId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Buildingconnected.V2.ProjectBidForms[formId].LineItems[lineItemId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates multiple line items on a project bid form (batch).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/project-bid-forms/{formId}/line-items:batch-create
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-bid-forms-formId-line-items-batch-create-POST
    /// </remarks>
    /// <param name="formId">The project bid form identifier.</param>
    /// <param name="body">The batch-create request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="LineItemsBatchCreatePostResponse"/> from the API, or <c>null</c>.</returns>
    /// <example>
    /// <code>
    /// LineItemsBatchCreatePostResponse? batch = await client.ProjectBidFormsManager.BatchCreateLineItemsAsync(
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
        return await _api.Construction.Buildingconnected.V2.ProjectBidForms[formId].LineItemsBatchCreate
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates multiple line items on a project bid form (batch).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/buildingconnected/v2/project-bid-forms/{formId}/line-items:batch-patch
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-bid-forms-formId-line-items-batch-patch-PATCH
    /// </remarks>
    /// <param name="formId">The project bid form identifier.</param>
    /// <param name="body">The batch-patch request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="LineItemsBatchPatchPatchResponse"/> from the API, or <c>null</c>.</returns>
    /// <example>
    /// <code>
    /// LineItemsBatchPatchPatchResponse? batch = await client.ProjectBidFormsManager.BatchPatchLineItemsAsync(
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
        return await _api.Construction.Buildingconnected.V2.ProjectBidForms[formId].LineItemsBatchPatch
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes multiple line items from a project bid form (batch).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/project-bid-forms/{formId}/line-items:batch-delete
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-project-bid-forms-formId-line-items-batch-delete-POST
    /// </remarks>
    /// <param name="formId">The project bid form identifier.</param>
    /// <param name="body">The list of line item identifiers to delete.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A task that completes when the batch delete request finishes.</returns>
    /// <example>
    /// <code>
    /// List&lt;string&gt; ids = new List&lt;string&gt; { "line-item-1", "line-item-2" };
    /// await client.ProjectBidFormsManager.BatchDeleteLineItemsAsync("form-id", ids);
    /// </code>
    /// </example>
    public async Task BatchDeleteLineItemsAsync(
        string formId,
        List<string> body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Buildingconnected.V2.ProjectBidForms[formId].LineItemsBatchDelete
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
