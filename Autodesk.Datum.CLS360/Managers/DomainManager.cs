using System.Runtime.CompilerServices;
using Autodesk.Datum.CLS360.Domains;
using Autodesk.Datum.CLS360.Domains.Item;
using Autodesk.Datum.CLS360.Domains.Item.Snapshot;
using Autodesk.Datum.CLS360.Domains.Seed;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Datum.CLS360.Managers;

/// <summary>
/// Manager for domain operations.
/// </summary>
public class DomainManager
{
    private readonly BaseDatumCLS360Client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainManager"/> class.
    /// </summary>
    /// <param name="api">The API client.</param>
    public DomainManager(BaseDatumCLS360Client api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns a list of all domain objects.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /domains
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="DomainsGetResponse_value"/> items, automatically paginated.</returns>
    /// <example>
    /// <code>
    /// await foreach (DomainsGetResponse_value domain in client.DomainManager.ListDomainsAsync())
    /// {
    ///     Console.WriteLine(domain.Hdl);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<DomainsGetResponse_value> ListDomainsAsync(
        RequestConfiguration<DomainsRequestBuilder.DomainsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var requestBuilder = _api.Domains;
        while (true)
        {
            DomainsGetResponse? response = await requestBuilder.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

            if (response?.Value is not { Count: > 0 })
                yield break;

            foreach (DomainsGetResponse_value domain in response.Value)
                yield return domain;

            if (string.IsNullOrWhiteSpace(response.NextLink))
                yield break;

            requestBuilder = requestBuilder.WithUrl(response.NextLink);
        }
    }

    /// <summary>
    /// Seeds a new domain from another domain.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /domains/seed
    /// </remarks>
    /// <param name="body">The seed request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="SeedPostResponse"/> containing the created domain handle.</returns>
    /// <example>
    /// <code>
    /// SeedPostResponse? response = await client.DomainManager.SeedDomainAsync(new SeedPostRequestBody());
    /// </code>
    /// </example>
    public async Task<SeedPostResponse?> SeedDomainAsync(
        SeedPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains.Seed.PostAsync(body, r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }

    /// <summary>
    /// Returns a domain object selected by handle.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /domains/{domHdl}
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithDomHdlGetResponse"/> containing the domain object.</returns>
    /// <example>
    /// <code>
    /// WithDomHdlGetResponse? response = await client.DomainManager.GetDomainAsync("domain-handle");
    /// </code>
    /// </example>
    public async Task<WithDomHdlGetResponse?> GetDomainAsync(
        string domHdl,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }

    /// <summary>
    /// Returns a snapshot of the domain for the specified version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /domains/{domHdl}/snapshot
    /// </remarks>
    /// <param name="domHdl">Domain handle or name.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="SnapshotGetResponse"/> containing the domain snapshot.</returns>
    /// <example>
    /// <code>
    /// SnapshotGetResponse? response = await client.DomainManager.GetDomainSnapshotAsync("domain-handle", new RequestConfiguration&lt;SnapshotRequestBuilder.SnapshotRequestBuilderGetQueryParameters&gt;
    /// {
    ///     QueryParameters = new SnapshotRequestBuilder.SnapshotRequestBuilderGetQueryParameters { Version = 10 }
    /// });
    /// </code>
    /// </example>
    public async Task<SnapshotGetResponse?> GetDomainSnapshotAsync(
        string domHdl,
        RequestConfiguration<SnapshotRequestBuilder.SnapshotRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].Snapshot.GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }
}
