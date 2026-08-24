using System.Runtime.CompilerServices;
using Autodesk.Datum.PIM360.Eic;
using Autodesk.Datum.PIM360.Eic.Attributes.Unique;
using Autodesk.Datum.PIM360.Eic.Close;
using Autodesk.Datum.PIM360.Eic.Item.Deliverables;
using Autodesk.Datum.PIM360.Eic.List;
using Autodesk.Datum.PIM360.Eic.Publish;
using Autodesk.Datum.PIM360.Eic.Publish.Multiple;
using Autodesk.Datum.PIM360.Eic.Query;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>Manager for Engineering Information Change operations.</summary>
public sealed class EicManager : Pim360ManagerBase
{
    private readonly BaseDatumPIM360Client _api;

    /// <summary>Initializes a new instance of the <see cref="EicManager"/> class.</summary>
    /// <param name="api">The generated PIM360 API client.</param>
    public EicManager(BaseDatumPIM360Client api) => _api = api;

    /// <summary>Streams the list of EIC objects.</summary>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of EIC rows.</returns>
    /// <example>
    /// <code>
    /// await foreach (ListGetResponse_rows eic in client.EicManager.ListAsync())
    /// {
    ///     Console.WriteLine(eic.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ListGetResponse_rows> ListAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ListGetResponse? response = await _api.Eic.List.GetAsync(null, cancellationToken);
        if (response?.Rows is null)
            yield break;

        foreach (ListGetResponse_rows row in response.Rows)
            yield return row;
    }

    /// <summary>Creates a new EIC, or updates an existing one when a handle is provided.</summary>
    /// <remarks>Wraps: POST /eic/list</remarks>
    /// <param name="body">The EIC handle (for updates) and attribute values.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated create/update response.</returns>
    public Task<ListPostResponse?> CreateOrUpdateAsync(ListPostRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Eic.List.PostAsync(body, null, cancellationToken);

    /// <summary>Gets the deliverables created against an EIC.</summary>
    /// <remarks>Wraps: GET /eic/{eicHdl}/deliverables</remarks>
    /// <param name="eicHdl">EIC handle.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated deliverables response.</returns>
    public Task<DeliverablesGetResponse?> GetDeliverablesAsync(string eicHdl, CancellationToken cancellationToken = default) =>
        _api.Eic[eicHdl].Deliverables.GetAsync(null, cancellationToken);

    /// <summary>Creates deliverables against an EIC.</summary>
    /// <remarks>Wraps: POST /eic/{eicHdl}/deliverables</remarks>
    /// <param name="eicHdl">EIC handle.</param>
    /// <param name="body">The deliverables to create.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated created-deliverables response.</returns>
    public Task<DeliverablesPostResponse?> CreateDeliverablesAsync(string eicHdl, List<Deliverables> body, CancellationToken cancellationToken = default) =>
        _api.Eic[eicHdl].Deliverables.PostAsync(body, null, cancellationToken);

    /// <summary>Submits a publish activity for an EIC.</summary>
    /// <remarks>Wraps: POST /eic/publish</remarks>
    /// <param name="body">The EIC handle to publish.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated publish activity response.</returns>
    public Task<PublishPostResponse?> PublishAsync(PublishPostRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Eic.Publish.PostAsync(body, null, cancellationToken);

    /// <summary>Submits a publish activity for multiple EICs.</summary>
    /// <remarks>Wraps: POST /eic/publish/multiple</remarks>
    /// <param name="body">The EIC handles to publish.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated publish activity response.</returns>
    public Task<MultiplePostResponse?> PublishMultipleAsync(MultiplePostRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Eic.Publish.Multiple.PostAsync(body, null, cancellationToken);

    /// <summary>Closes an EIC.</summary>
    /// <remarks>Wraps: POST /eic/close</remarks>
    /// <param name="body">The EIC handle to close.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated close activity response.</returns>
    public Task<ClosePostResponse?> CloseAsync(ClosePostRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Eic.Close.PostAsync(body, null, cancellationToken);

    /// <summary>Submits a new EIC query, returning results in the same shape as <see cref="ListAsync"/>.</summary>
    /// <remarks>Wraps: POST /eic/query</remarks>
    /// <param name="body">The attribute filters to apply.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated EIC query response.</returns>
    public Task<QueryPostResponse?> QueryAsync(QueryPostRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Eic.Query.PostAsync(body, null, cancellationToken);

    /// <summary>Gets unique values of an EIC attribute, optionally filtered.</summary>
    /// <remarks>Wraps: POST /eic/attributes/unique</remarks>
    /// <param name="body">The attribute to inspect and an optional filter.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated unique-values response.</returns>
    public Task<UniquePostResponse?> GetUniqueAttributesAsync(UniquePostRequestBody body, CancellationToken cancellationToken = default) =>
        _api.Eic.Attributes.Unique.PostAsync(body, null, cancellationToken);

    /// <summary>Provides direct access to all generated EIC request builders.</summary>
    public EicRequestBuilder Api => _api.Eic;
}
