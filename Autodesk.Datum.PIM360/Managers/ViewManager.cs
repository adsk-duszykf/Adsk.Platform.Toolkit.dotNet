using System.Runtime.CompilerServices;
using Autodesk.Datum.PIM360.Customviews;
using Microsoft.Kiota.Abstractions;
using CustomviewModel = Autodesk.Datum.PIM360.Customviews.Customviews;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>Manager for saved custom view operations.</summary>
public sealed class ViewManager : Pim360ManagerBase
{
    private readonly BaseDatumPIM360Client _api;

    /// <summary>Initializes a new instance of the <see cref="ViewManager"/> class.</summary>
    /// <param name="api">The generated PIM360 API client.</param>
    public ViewManager(BaseDatumPIM360Client api) => _api = api;

    /// <summary>Streams available custom views.</summary>
    /// <param name="requestConfiguration">(Optional) View type and transport options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of custom views.</returns>
    /// <example>
    /// <code>
    /// await foreach (Customviews view in client.ViewManager.ListAsync())
    /// {
    ///     Console.WriteLine(view.Hdl);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CustomviewModel> ListAsync(
        RequestConfiguration<CustomviewsRequestBuilder.CustomviewsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        List<CustomviewModel>? views = await _api.Customviews.GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);

        if (views is null)
            yield break;

        foreach (CustomviewModel view in views)
            yield return view;
    }

    /// <summary>Provides direct access to generated custom view request builders.</summary>
    public CustomviewsRequestBuilder Api => _api.Customviews;
}
