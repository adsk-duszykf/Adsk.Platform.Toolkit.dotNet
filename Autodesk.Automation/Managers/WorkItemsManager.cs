using System.Runtime.CompilerServices;
using Autodesk.Automation.Da.UsEast.V3.Workitems;
using Autodesk.Automation.Da.UsEast.V3.Workitems.Batch;
using Autodesk.Automation.Da.UsEast.V3.Workitems.Combine;
using Autodesk.Automation.Da.UsEast.V3.Workitems.Item;
using Autodesk.Automation.Da.UsEast.V3.Workitems.Status;
using Autodesk.Automation.Da.UsEast.V3.WorkitemsStartAfterTimeEpochSecondsInUTC;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Automation.Da.UsEast.V3.WorkitemsStartAfterTimeEpochSecondsInUTC.WorkitemsStartAfterTimeEpochSecondsInUTCRequestBuilder;

namespace Autodesk.Automation.Managers;

/// <summary>
/// Manager for WorkItem operations — creates and monitors Design Automation work items.
/// </summary>
public class WorkItemsManager
{
    private readonly BaseAutomationClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkItemsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public WorkItemsManager(BaseAutomationClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Creates a new WorkItem. The WorkItem is placed on a queue and later picked up by an engine.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /da/us-east/v3/workitems
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/workitems-POST
    /// </remarks>
    /// <param name="body">The WorkItem creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WorkitemsPostResponse"/> containing the created WorkItem status</returns>
    /// <example>
    /// <code>
    /// WorkitemsPostResponse? workItem = await client.WorkItemsManager.CreateWorkItemAsync(new WorkitemsPostRequestBody
    /// {
    ///     ActivityId = "MyNickname.MyActivity+MyAlias"
    /// });
    /// </code>
    /// </example>
    public async Task<WorkitemsPostResponse?> CreateWorkItemAsync(
        WorkitemsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Workitems
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets the status of a specific WorkItem.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/workitems/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/workitems-id-GET
    /// </remarks>
    /// <param name="id">The ID of the WorkItem</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WorkitemsGetResponse"/> containing the WorkItem status</returns>
    /// <example>
    /// <code>
    /// WorkitemsGetResponse? status = await client.WorkItemsManager.GetWorkItemStatusAsync("workitem-guid");
    /// </code>
    /// </example>
    public async Task<WorkitemsGetResponse?> GetWorkItemStatusAsync(
        string id,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Workitems[id]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Cancels a specific WorkItem.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /da/us-east/v3/workitems/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/workitems-id-DELETE
    /// </remarks>
    /// <param name="id">The ID of the WorkItem to cancel</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.WorkItemsManager.CancelWorkItemAsync("workitem-guid");
    /// </code>
    /// </example>
    public async Task CancelWorkItemAsync(
        string id,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Da.UsEast.V3.Workitems[id]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a batch of WorkItems. The WorkItems are placed on the queue and later picked up by an engine.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /da/us-east/v3/workitems/batch
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/workitems-batch-POST
    /// </remarks>
    /// <param name="body">The batch WorkItem creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="BatchPostResponse"/> containing the batch WorkItem status</returns>
    /// <example>
    /// <code>
    /// BatchPostResponse? batch = await client.WorkItemsManager.CreateBatchWorkItemsAsync(new BatchPostRequestBody
    /// {
    ///     ActivityId = "MyNickname.MyActivity+MyAlias"
    /// });
    /// </code>
    /// </example>
    public async Task<BatchPostResponse?> CreateBatchWorkItemsAsync(
        BatchPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Workitems.Batch
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets WorkItem status for an array of WorkItem IDs.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /da/us-east/v3/workitems/status
    /// </remarks>
    /// <param name="body">The batch status request containing WorkItem IDs</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="StatusPostResponse"/> containing the batch status results</returns>
    /// <example>
    /// <code>
    /// StatusPostResponse? statuses = await client.WorkItemsManager.GetBatchWorkItemStatusAsync(new StatusPostRequestBody
    /// {
    ///     Ids = [new() { }, new() { }]
    /// });
    /// </code>
    /// </example>
    public async Task<StatusPostResponse?> GetBatchWorkItemStatusAsync(
        StatusPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Workitems.Status
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a fan-in workflow where 1-N workitems (parts) must complete before the final workitem (combinator) is processed.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /da/us-east/v3/workitems/combine
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/workitems-combine-POST
    /// </remarks>
    /// <param name="body">The combine WorkItem creation data with parts and combinator</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="CombinePostResponse"/> containing the combined WorkItem status</returns>
    /// <example>
    /// <code>
    /// CombinePostResponse? combined = await client.WorkItemsManager.CreateCombineWorkItemAsync(new CombinePostRequestBody());
    /// </code>
    /// </example>
    public async Task<CombinePostResponse?> CreateCombineWorkItemAsync(
        CombinePostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Workitems.Combine
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists all WorkItem IDs that have been updated after a specified time, with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/workitems?startAfterTime=:epochSecondsInUTC
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/workitems-startAfterTime-GET
    /// </remarks>
    /// <param name="startAfterTime">Start time in epoch seconds (UTC). Returns WorkItem IDs updated after this time.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="WorkitemsStartAfterTimeEpochSecondsInUTCGetResponse_data"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var item in client.WorkItemsManager.ListWorkItemsAsync(1700000000))
    /// {
    ///     Console.WriteLine($"{item.Id} - last modified: {item.LastModifiedInUTCEpochSecond}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<WorkitemsStartAfterTimeEpochSecondsInUTCGetResponse_data> ListWorkItemsAsync(
        int startAfterTime,
        RequestConfiguration<WorkitemsStartAfterTimeEpochSecondsInUTCRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? page = requestConfiguration?.QueryParameters?.Page;

        while (true)
        {
            var response = await _api.Da.UsEast.V3.WorkitemsStartAfterTimeEpochSecondsInUTC
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.StartAfterTime = startAfterTime;
                    r.QueryParameters.Page = page;
                }, cancellationToken);

            if (response?.Data is not { Count: > 0 })
                yield break;

            foreach (var item in response.Data)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.PaginationToken))
                yield break;

            page = response.PaginationToken;
        }
    }
}
