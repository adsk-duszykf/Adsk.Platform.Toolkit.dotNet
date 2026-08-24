using System.Runtime.CompilerServices;
using Autodesk.Datum.PIM360.Associations;
using Autodesk.Datum.PIM360.Objects.Item.Facility.Item.Id.Item;
using Autodesk.Datum.PIM360.Objects.Item.Handles;
using Autodesk.Datum.PIM360.Objects.Item.Id.Item;
using Autodesk.Datum.PIM360.Objects.Item.Item;
using Autodesk.Datum.PIM360.Objects.Item.Item.Changes;
using Microsoft.Kiota.Abstractions;
using ItemByFacilityIdRequestBuilder = Autodesk.Datum.PIM360.Objects.Item.Facility.Item.Id.Item.IdItemRequestBuilder;
using ItemByIdRequestBuilder = Autodesk.Datum.PIM360.Objects.Item.Id.Item.IdItemRequestBuilder;
using ItemByIdResponse = Autodesk.Datum.PIM360.Objects.Item.Id.Item.IdGetResponse;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>Manager for PIM360 object and item operations.</summary>
public sealed class ItemManager : Pim360ManagerBase
{
    private readonly BaseDatumPIM360Client _api;

    /// <summary>Initializes a new instance of the <see cref="ItemManager"/> class.</summary>
    /// <param name="api">The generated PIM360 API client.</param>
    public ItemManager(BaseDatumPIM360Client api) => _api = api;

    /// <summary>Lists item handles for an object type, automatically paging through results.</summary>
    /// <param name="type">Object type.</param>
    /// <param name="name">Required attribute name.</param>
    /// <param name="value">Required attribute value.</param>
    /// <param name="requestConfiguration">(Optional) Request filters and transport options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of handles.</returns>
    public async IAsyncEnumerable<string> ListHandlesAsync(
        string type,
        string name,
        string value,
        RequestConfiguration<HandlesRequestBuilder.HandlesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        long skip = ParseNumber(requestConfiguration?.QueryParameters?.Skip) ?? 0;
        long limit = Math.Clamp(ParseNumber(requestConfiguration?.QueryParameters?.Limit) ?? 200, 1, 200);
        var requestBuilder = _api.Objects[type].Handles;

        while (true)
        {
            List<string>? handles = await requestBuilder.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
                r.QueryParameters.Name = name;
                r.QueryParameters.Value = value;
                r.QueryParameters.Eic = requestConfiguration?.QueryParameters?.Eic;
                r.QueryParameters.Skip = skip.ToString();
                r.QueryParameters.Limit = limit.ToString();
            }, cancellationToken);

            if (handles is not { Count: > 0 })
                yield break;

            foreach (string handle in handles)
                yield return handle;

            if (handles.Count < limit)
                yield break;

            skip += handles.Count;
        }
    }

    /// <summary>Gets an item by object type and handle.</summary>
    /// <param name="type">Object type.</param>
    /// <param name="handle">Item handle.</param>
    /// <param name="requestConfiguration">(Optional) Request configuration.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated item response.</returns>
    public async Task<WithHandleGetResponse?> GetItemAsync(
        string type,
        string handle,
        RequestConfiguration<WithHandleItemRequestBuilder.WithHandleItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Objects[type][handle].GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }

    /// <summary>Gets an item by object type and item ID.</summary>
    /// <param name="type">Object type.</param>
    /// <param name="id">Item ID.</param>
    /// <param name="requestConfiguration">(Optional) Request configuration.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated item-by-ID response.</returns>
    public async Task<ItemByIdResponse?> GetItemByIdAsync(
        string type,
        string id,
        RequestConfiguration<ItemByIdRequestBuilder.IdItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Objects[type].Id[id].GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
            r.QueryParameters.Eic = requestConfiguration?.QueryParameters?.Eic;
        }, cancellationToken);
    }

    /// <summary>Gets an item by object type, facility ID, and item ID.</summary>
    /// <param name="type">Object type.</param>
    /// <param name="facility">Facility ID.</param>
    /// <param name="id">Item ID.</param>
    /// <param name="requestConfiguration">(Optional) Request configuration.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated facility item response.</returns>
    public async Task<Autodesk.Datum.PIM360.Objects.Item.Facility.Item.Id.Item.IdGetResponse?> GetItemByFacilityIdAsync(
        string type,
        string facility,
        string id,
        RequestConfiguration<ItemByFacilityIdRequestBuilder.IdItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Objects[type].Facility[facility].Id[id].GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
            r.QueryParameters.Eic = requestConfiguration?.QueryParameters?.Eic;
        }, cancellationToken);
    }

    /// <summary>Streams associated objects for an item.</summary>
    /// <param name="requestConfiguration">Request filters, item handle, type, and association type.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of associated objects.</returns>
    public async IAsyncEnumerable<AssociationsGetResponse_items> ListAssociationsAsync(
        RequestConfiguration<AssociationsRequestBuilder.AssociationsRequestBuilderGetQueryParameters> requestConfiguration,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        long skip = requestConfiguration.QueryParameters?.Skip ?? 0;
        while (true)
        {
            AssociationsGetResponse? response = await _api.Associations.GetAsync(r =>
            {
                r.Headers = requestConfiguration.Headers ?? r.Headers;
                r.Options = requestConfiguration.Options ?? r.Options;
                r.QueryParameters.Assoc = requestConfiguration.QueryParameters?.Assoc;
                r.QueryParameters.Eic = requestConfiguration.QueryParameters?.Eic;
                r.QueryParameters.Hdl = requestConfiguration.QueryParameters?.Hdl;
                r.QueryParameters.Type = requestConfiguration.QueryParameters?.Type;
                r.QueryParameters.Skip = skip;
            }, cancellationToken);

            if (response?.Items is not { Count: > 0 })
                yield break;

            foreach (AssociationsGetResponse_items item in response.Items)
                yield return item;

            if (response.GetMore is not > 0)
                yield break;

            skip += response.Items.Count;
        }
    }

    /// <summary>Lists changes for an item.</summary>
    /// <param name="type">Object type.</param>
    /// <param name="handle">Item handle.</param>
    /// <param name="requestConfiguration">(Optional) Request configuration.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of changes.</returns>
    public async IAsyncEnumerable<Changes> ListChangesAsync(
        string type,
        string handle,
        RequestConfiguration<ChangesRequestBuilder.ChangesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        List<Changes>? changes = await _api.Objects[type][handle].Changes.GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);

        if (changes is null)
            yield break;

        foreach (Changes change in changes)
            yield return change;
    }

    private static long? ParseNumber(string? value) => long.TryParse(value, out long result) ? result : null;
}
