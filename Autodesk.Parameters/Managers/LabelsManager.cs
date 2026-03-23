using System.Runtime.CompilerServices;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections.Item.LabelsAttach;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections.Item.LabelsDetach;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Labels;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Labels.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Parameters.Managers;

/// <summary>
/// Manager for Label operations
/// </summary>
public class LabelsManager
{
    private readonly BaseParametersClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public LabelsManager(BaseParametersClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists the labels in an account with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/accounts/{accountId}/labels
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-listlabelsv2-GET
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="LabelsGetResponse_result"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var label in client.LabelsManager.ListLabelsAsync(accountId))
    /// {
    ///     Console.WriteLine($"{label.Id}: {label.Name}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<LabelsGetResponse_result> ListLabelsAsync(
        Guid accountId,
        RequestConfiguration<LabelsRequestBuilder.LabelsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Parameters.V1.Accounts[accountId].Labels
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Result is not { Count: > 0 })
                yield break;

            foreach (var item in response.Result)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Result.Count;
        }
    }

    /// <summary>
    /// Retrieves information about a given label.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/accounts/{accountId}/labels/{labelId}
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-getlabelv2-GET
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="labelId">The label ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithLabelGetResponse"/> containing the label details</returns>
    /// <example>
    /// <code>
    /// WithLabelGetResponse? label = await client.LabelsManager.GetLabelAsync(accountId, "my-label-id");
    /// Console.WriteLine(label?.Name);
    /// </code>
    /// </example>
    public async Task<WithLabelGetResponse?> GetLabelAsync(
        Guid accountId,
        string labelId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Labels[labelId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates new labels (max 20 per request).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /parameters/v1/accounts/{accountId}/labels
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-createlabelsv2-POST
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="body">The label creation payload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="LabelsPostResponse"/> containing the created labels</returns>
    /// <example>
    /// <code>
    /// LabelsPostResponse? created = await client.LabelsManager.CreateLabelsAsync(accountId, body);
    /// </code>
    /// </example>
    public async Task<LabelsPostResponse?> CreateLabelsAsync(
        Guid accountId,
        LabelsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Labels
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates information of a given label.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /parameters/v1/accounts/{accountId}/labels/{labelId}
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-updatelabelv2-PATCH
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="labelId">The label ID</param>
    /// <param name="body">The label update payload</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithLabelPatchResponse"/> containing the updated label</returns>
    /// <example>
    /// <code>
    /// WithLabelPatchResponse? updated = await client.LabelsManager.UpdateLabelAsync(accountId, "my-label-id", body);
    /// </code>
    /// </example>
    public async Task<WithLabelPatchResponse?> UpdateLabelAsync(
        Guid accountId,
        string labelId,
        WithLabelPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Labels[labelId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes a label. It can only be removed if the label is not attached to any parameter.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /parameters/v1/accounts/{accountId}/labels/{labelId}
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-deletelabelv2-DELETE
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="labelId">The label ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.LabelsManager.DeleteLabelAsync(accountId, "my-label-id");
    /// </code>
    /// </example>
    public async Task DeleteLabelAsync(
        Guid accountId,
        string labelId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Parameters.V1.Accounts[accountId].Labels[labelId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Attaches a set of labels to a set of parameters within a given collection.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /parameters/v1/accounts/{accountId}/groups/{groupId}/collections/{collectionId}/labels:attach
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-attachlabelsv2-POST
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="body">The attach payload with label IDs and parameter IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="LabelsAttachPostResponse"/> containing the attached label and parameter IDs</returns>
    /// <example>
    /// <code>
    /// LabelsAttachPostResponse? result = await client.LabelsManager.AttachLabelsAsync(accountId, groupId, collectionId, body);
    /// </code>
    /// </example>
    public async Task<LabelsAttachPostResponse?> AttachLabelsAsync(
        Guid accountId,
        string groupId,
        string collectionId,
        LabelsAttachPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections[collectionId].LabelsAttach
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Detaches labels from parameters within a given collection.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /parameters/v1/accounts/{accountId}/groups/{groupId}/collections/{collectionId}/labels:detach
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-detachlabelsv2-POST
    /// </remarks>
    /// <param name="accountId">The Autodesk Construction Cloud account ID</param>
    /// <param name="groupId">The group ID</param>
    /// <param name="collectionId">The collection ID</param>
    /// <param name="body">The detach payload with label IDs and parameter IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="LabelsDetachPostResponse"/> containing the detached label and parameter IDs</returns>
    /// <example>
    /// <code>
    /// LabelsDetachPostResponse? result = await client.LabelsManager.DetachLabelsAsync(accountId, groupId, collectionId, body);
    /// </code>
    /// </example>
    public async Task<LabelsDetachPostResponse?> DetachLabelsAsync(
        Guid accountId,
        string groupId,
        string collectionId,
        LabelsDetachPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections[collectionId].LabelsDetach
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
