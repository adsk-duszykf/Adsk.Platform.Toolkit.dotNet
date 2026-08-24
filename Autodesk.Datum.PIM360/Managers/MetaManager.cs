using System.Runtime.CompilerServices;
using Autodesk.Datum.PIM360.Attributes;
using Autodesk.Datum.PIM360.Attributes.Tree;
using Autodesk.Datum.PIM360.Snapshotversion;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>Manager for PIM360 metadata and explorer operations.</summary>
public sealed class MetaManager : Pim360ManagerBase
{
    private readonly BaseDatumPIM360Client _api;

    /// <summary>Initializes a new instance of the <see cref="MetaManager"/> class.</summary>
    /// <param name="api">The generated PIM360 API client.</param>
    public MetaManager(BaseDatumPIM360Client api) => _api = api;

    /// <summary>Gets the current class library snapshot metadata.</summary>
    /// <param name="requestConfiguration">(Optional) Request configuration.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>The generated snapshot metadata response.</returns>
    public Task<SnapshotversionGetResponse?> GetSnapshotVersionAsync(
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default) =>
        _api.Snapshotversion.GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);

    /// <summary>Streams the available attribute tree entries.</summary>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of attribute tree entries.</returns>
    /// <example>
    /// <code>
    /// await foreach (Tree attribute in client.MetaManager.ListAttributeTreeAsync())
    /// {
    ///     Console.WriteLine(attribute.Hdl);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Tree> ListAttributeTreeAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        List<Tree>? attributes = await _api.Attributes.Tree.GetAsync(null, cancellationToken);
        if (attributes is null)
            yield break;

        foreach (Tree attribute in attributes)
            yield return attribute;
    }

    /// <summary>Provides direct access to generated attribute request builders.</summary>
    public AttributesRequestBuilder AttributesApi => _api.Attributes;

}
