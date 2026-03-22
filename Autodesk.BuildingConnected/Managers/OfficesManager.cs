using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Offices;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Offices.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Offices.OfficesRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected office operations — lists offices and retrieves an office by id.
/// </summary>
public class OfficesManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfficesManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public OfficesManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists BuildingConnected offices with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/offices
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-offices-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request, including cursor and filter query parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="OfficesGetResponse_results"/>.</returns>
    /// <example>
    /// <code>
    /// await foreach (var office in client.OfficesManager.ListOfficesAsync())
    /// {
    ///     Console.WriteLine(office);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<OfficesGetResponse_results> ListOfficesAsync(
        RequestConfiguration<OfficesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            OfficesGetResponse? response = await _api.Construction.Buildingconnected.V2.Offices
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (OfficesGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Gets a BuildingConnected office by id.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/offices/{officeId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-offices-officeId-GET
    /// </remarks>
    /// <param name="officeId">The office id.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithOfficeGetResponse"/> instance, or <c>null</c> if the response is empty.</returns>
    /// <example>
    /// <code>
    /// WithOfficeGetResponse? office = await client.OfficesManager.GetOfficeAsync("office-id-here");
    /// </code>
    /// </example>
    public async Task<WithOfficeGetResponse?> GetOfficeAsync(
        string officeId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Offices[officeId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
