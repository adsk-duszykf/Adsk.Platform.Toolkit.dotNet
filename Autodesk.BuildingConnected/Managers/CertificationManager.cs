using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.CertificateAgencies;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.CertificateTypes;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.CertificateAgencies.CertificateAgenciesRequestBuilder;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.CertificateTypes.CertificateTypesRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected certification metadata — certificate types and certificate agencies.
/// </summary>
public class CertificationManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="CertificationManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public CertificationManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists certificate types with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/certificate-types
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-certificate-types-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request, including cursor and limit query parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="CertificateTypesGetResponse_results"/>.</returns>
    /// <example>
    /// <code>
    /// await foreach (var certificateType in client.CertificationManager.ListCertificateTypesAsync())
    /// {
    ///     Console.WriteLine(certificateType);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CertificateTypesGetResponse_results> ListCertificateTypesAsync(
        RequestConfiguration<CertificateTypesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            CertificateTypesGetResponse? response = await _api.Construction.Buildingconnected.V2.CertificateTypes
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (CertificateTypesGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Lists certificate agencies with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/certificate-agencies
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-certificate-agencies-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request, including cursor and limit query parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="CertificateAgenciesGetResponse_results"/>.</returns>
    /// <example>
    /// <code>
    /// await foreach (var agency in client.CertificationManager.ListCertificateAgenciesAsync())
    /// {
    ///     Console.WriteLine(agency);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CertificateAgenciesGetResponse_results> ListCertificateAgenciesAsync(
        RequestConfiguration<CertificateAgenciesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            CertificateAgenciesGetResponse? response = await _api.Construction.Buildingconnected.V2.CertificateAgencies
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (CertificateAgenciesGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }
}
