using System.Runtime.CompilerServices;
using Autodesk.Datum.PIM360.Queries;
using Autodesk.Datum.PIM360.Queryresults.Item;
using Microsoft.Kiota.Abstractions;
using PivotQueryRequestBody = Autodesk.Datum.PIM360.Pivot.Query.QueryPostRequestBody;
using PivotQueryResponse = Autodesk.Datum.PIM360.Pivot.Query.QueryPostResponse;
using PivotResultRow = Autodesk.Datum.PIM360.Queryresults.Pivot.Item.WithHandleGetResponse_rows;
using PivotResultsRequestBuilder = Autodesk.Datum.PIM360.Queryresults.Pivot.Item.WithHandleItemRequestBuilder;
using PivotResultsResponse = Autodesk.Datum.PIM360.Queryresults.Pivot.Item.WithHandleGetResponse;
using ViewResultItem = Autodesk.Datum.PIM360.Queryresults.Views.Item.WithHandleGetResponse_data;
using ViewResultsRequestBuilder = Autodesk.Datum.PIM360.Queryresults.Views.Item.WithHandleItemRequestBuilder;
using ViewResultsResponse = Autodesk.Datum.PIM360.Queryresults.Views.Item.WithHandleGetResponse;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>Manager for PIM360 query and query-result operations.</summary>
public sealed class QueryManager : Pim360ManagerBase
{
    private readonly BaseDatumPIM360Client _api;

    /// <summary>Initializes a new instance of the <see cref="QueryManager"/> class.</summary>
    /// <param name="api">The generated PIM360 API client.</param>
    public QueryManager(BaseDatumPIM360Client api) => _api = api;

    /// <summary>Creates a PIM360 query.</summary>
    /// <param name="body">The query configuration.</param>
    /// <param name="requestConfiguration">(Optional) Request configuration.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated query creation response.</returns>
    public Task<QueriesPostResponse?> CreateQueryAsync(
        QueriesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default) =>
        _api.Queries.PostAsync(body, r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);

    /// <summary>Streams query results by handle using skip/limit pagination.</summary>
    /// <param name="handle">Query result handle.</param>
    /// <param name="requestConfiguration">(Optional) Query filters and transport options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of query-result items.</returns>
    public async IAsyncEnumerable<WithHandleGetResponse_data> ListResultsAsync(
        string handle,
        RequestConfiguration<WithHandleItemRequestBuilder.WithHandleItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        long skip = requestConfiguration?.QueryParameters?.Skip ?? 0;
        long limit = Math.Clamp(requestConfiguration?.QueryParameters?.Limit ?? 200, 1, 200);
        var requestBuilder = _api.Queryresults[handle];

        while (true)
        {
            WithHandleGetResponse? response = await requestBuilder.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
                r.QueryParameters.Skip = skip;
                r.QueryParameters.Limit = limit;
            }, cancellationToken);

            if (response?.Data is not { Count: > 0 })
                yield break;

            foreach (WithHandleGetResponse_data item in response.Data)
                yield return item;

            if (response.Data.Count < limit)
                yield break;

            skip += response.Data.Count;
        }
    }

    /// <summary>Streams results of a custom view by handle using skip/limit pagination.</summary>
    /// <remarks>Wraps: GET /queryresults/views/{handle}</remarks>
    /// <param name="handle">Custom view handle.</param>
    /// <param name="requestConfiguration">(Optional) Query filters and transport options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of view-result items.</returns>
    public async IAsyncEnumerable<ViewResultItem> ListViewResultsAsync(
        string handle,
        RequestConfiguration<ViewResultsRequestBuilder.WithHandleItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        long skip = requestConfiguration?.QueryParameters?.Skip ?? 0;
        long limit = Math.Clamp(requestConfiguration?.QueryParameters?.Limit ?? 200, 1, 200);
        var requestBuilder = _api.Queryresults.Views[handle];

        while (true)
        {
            ViewResultsResponse? response = await requestBuilder.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
                r.QueryParameters.Eic = requestConfiguration?.QueryParameters?.Eic;
                r.QueryParameters.Type = requestConfiguration?.QueryParameters?.Type;
                r.QueryParameters.Skip = skip;
                r.QueryParameters.Limit = limit;
            }, cancellationToken);

            if (response?.Data is not { Count: > 0 })
                yield break;

            foreach (ViewResultItem item in response.Data)
                yield return item;

            if (response.Data.Count < limit)
                yield break;

            skip += response.Data.Count;
        }
    }

    /// <summary>Streams rows of a saved Pivot view by handle using skip/limit pagination.</summary>
    /// <remarks>Wraps: GET /queryresults/pivot/{handle}</remarks>
    /// <param name="handle">Pivot view handle.</param>
    /// <param name="requestConfiguration">(Optional) Query filters and transport options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of pivot result rows.</returns>
    public async IAsyncEnumerable<PivotResultRow> ListPivotResultsAsync(
        string handle,
        RequestConfiguration<PivotResultsRequestBuilder.WithHandleItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        long skip = requestConfiguration?.QueryParameters?.Skip ?? 0;
        long limit = Math.Clamp(requestConfiguration?.QueryParameters?.Limit ?? 200, 1, 200);
        var requestBuilder = _api.Queryresults.Pivot[handle];

        while (true)
        {
            PivotResultsResponse? response = await requestBuilder.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
                r.QueryParameters.Eic = requestConfiguration?.QueryParameters?.Eic;
                r.QueryParameters.Type = requestConfiguration?.QueryParameters?.Type;
                r.QueryParameters.Skip = skip;
                r.QueryParameters.Limit = limit;
            }, cancellationToken);

            if (response?.Rows is not { Count: > 0 })
                yield break;

            foreach (PivotResultRow row in response.Rows)
                yield return row;

            if (response.Rows.Count < limit)
                yield break;

            skip += response.Rows.Count;
        }
    }

    /// <summary>Submits a new pivot query and gets the results.</summary>
    /// <remarks>Wraps: POST /pivot/query</remarks>
    /// <param name="body">The pivot query configuration.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated pivot query response.</returns>
    public Task<PivotQueryResponse?> SubmitPivotQueryAsync(PivotQueryRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Pivot.Query.PostAsync(body, null, cancellationToken);

    /// <summary>Provides direct access to generated query request builders.</summary>
    public Autodesk.Datum.PIM360.Queries.QueriesRequestBuilder QueriesApi => _api.Queries;
}
