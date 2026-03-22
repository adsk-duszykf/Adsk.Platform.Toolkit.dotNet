using System.Runtime.CompilerServices;
using Autodesk.Automation.Da.UsEast.V3.Appbundles;
using Autodesk.Automation.Da.UsEast.V3.Appbundles.Item.Aliases;
using Autodesk.Automation.Da.UsEast.V3.Appbundles.Item.Aliases.Item;
using Autodesk.Automation.Da.UsEast.V3.Appbundles.Item.Versions;
using Autodesk.Automation.Da.UsEast.V3.Appbundles.Item.Versions.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Automation.Da.UsEast.V3.Appbundles.AppbundlesRequestBuilder;
using static Autodesk.Automation.Da.UsEast.V3.Appbundles.Item.Aliases.AliasesRequestBuilder;
using static Autodesk.Automation.Da.UsEast.V3.Appbundles.Item.Versions.VersionsRequestBuilder;
using AppBundleDetails = Autodesk.Automation.Da.UsEast.V3.Appbundles.Item.AppbundlesGetResponse;

namespace Autodesk.Automation.Managers;

/// <summary>
/// Manager for AppBundle operations — manages Design Automation app bundles, aliases, and versions.
/// </summary>
public class AppBundlesManager
{
    private readonly BaseAutomationClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppBundlesManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public AppBundlesManager(BaseAutomationClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists all available AppBundles, including AppBundles shared with this app, with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/appbundles
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="string"/> AppBundle IDs, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var bundleId in client.AppBundlesManager.ListAppBundlesAsync())
    /// {
    ///     Console.WriteLine(bundleId);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<string> ListAppBundlesAsync(
        RequestConfiguration<AppbundlesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? page = requestConfiguration?.QueryParameters?.Page;

        while (true)
        {
            var response = await _api.Da.UsEast.V3.Appbundles
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
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

    /// <summary>
    /// Creates a new AppBundle.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /da/us-east/v3/appbundles
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-POST
    /// </remarks>
    /// <param name="body">The AppBundle creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AppbundlesPostResponse"/> containing the created AppBundle with upload parameters</returns>
    /// <example>
    /// <code>
    /// AppbundlesPostResponse? bundle = await client.AppBundlesManager.CreateAppBundleAsync(new AppbundlesPostRequestBody
    /// {
    ///     Id = "MyAppBundle",
    ///     Engine = "Autodesk.AutoCAD+24"
    /// });
    /// </code>
    /// </example>
    public async Task<AppbundlesPostResponse?> CreateAppBundleAsync(
        AppbundlesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Appbundles
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets the details of the specified AppBundle. The id parameter must be a QualifiedId (owner.name+label).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/appbundles/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-id-GET
    /// </remarks>
    /// <param name="id">Full qualified id of the AppBundle (owner.name+label)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AppBundleDetails"/> containing the AppBundle details</returns>
    /// <example>
    /// <code>
    /// AppBundleDetails? bundle = await client.AppBundlesManager.GetAppBundleAsync("MyNickname.MyAppBundle+MyAlias");
    /// </code>
    /// </example>
    public async Task<AppBundleDetails?> GetAppBundleAsync(
        string id,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Appbundles[id]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes the specified AppBundle, including all versions and aliases.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /da/us-east/v3/appbundles/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-id-DELETE
    /// </remarks>
    /// <param name="id">Name of AppBundle (unqualified)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.AppBundlesManager.DeleteAppBundleAsync("MyAppBundle");
    /// </code>
    /// </example>
    public async Task DeleteAppBundleAsync(
        string id,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Da.UsEast.V3.Appbundles[id]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists all aliases for the specified AppBundle with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/appbundles/{id}/aliases
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-id-aliases-GET
    /// </remarks>
    /// <param name="id">Name of AppBundle (unqualified)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="AliasesGetResponse_data"/> alias items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var alias in client.AppBundlesManager.ListAppBundleAliasesAsync("MyAppBundle"))
    /// {
    ///     Console.WriteLine($"{alias.Id} -> v{alias.Version}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<AliasesGetResponse_data> ListAppBundleAliasesAsync(
        string id,
        RequestConfiguration<AliasesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? page = requestConfiguration?.QueryParameters?.Page;

        while (true)
        {
            var response = await _api.Da.UsEast.V3.Appbundles[id].Aliases
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
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

    /// <summary>
    /// Creates a new alias for the specified AppBundle.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /da/us-east/v3/appbundles/{id}/aliases
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-id-aliases-POST
    /// </remarks>
    /// <param name="id">Name of AppBundle (unqualified)</param>
    /// <param name="body">The alias creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AliasesPostResponse"/> containing the created alias</returns>
    /// <example>
    /// <code>
    /// AliasesPostResponse? alias = await client.AppBundlesManager.CreateAppBundleAliasAsync("MyAppBundle", new AliasesPostRequestBody
    /// {
    ///     Id = "MyAlias",
    ///     Version = 1
    /// });
    /// </code>
    /// </example>
    public async Task<AliasesPostResponse?> CreateAppBundleAliasAsync(
        string id,
        AliasesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Appbundles[id].Aliases
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets the details of a specific AppBundle alias.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/appbundles/{id}/aliases/{aliasId}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-id-aliases-aliasId-GET
    /// </remarks>
    /// <param name="id">Name of AppBundle (unqualified)</param>
    /// <param name="aliasId">Name of alias</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithAliasGetResponse"/> containing the alias details</returns>
    /// <example>
    /// <code>
    /// WithAliasGetResponse? alias = await client.AppBundlesManager.GetAppBundleAliasAsync("MyAppBundle", "MyAlias");
    /// </code>
    /// </example>
    public async Task<WithAliasGetResponse?> GetAppBundleAliasAsync(
        string id,
        string aliasId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Appbundles[id].Aliases[aliasId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Modifies alias details for the specified AppBundle.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /da/us-east/v3/appbundles/{id}/aliases/{aliasId}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-id-aliases-aliasId-PATCH
    /// </remarks>
    /// <param name="id">Name of AppBundle (unqualified)</param>
    /// <param name="aliasId">Name of alias</param>
    /// <param name="body">The alias update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithAliasPatchResponse"/> containing the updated alias</returns>
    /// <example>
    /// <code>
    /// WithAliasPatchResponse? alias = await client.AppBundlesManager.UpdateAppBundleAliasAsync("MyAppBundle", "MyAlias", new WithAliasPatchRequestBody
    /// {
    ///     Version = 2
    /// });
    /// </code>
    /// </example>
    public async Task<WithAliasPatchResponse?> UpdateAppBundleAliasAsync(
        string id,
        string aliasId,
        WithAliasPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Appbundles[id].Aliases[aliasId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes the specified AppBundle alias.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /da/us-east/v3/appbundles/{id}/aliases/{aliasId}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-id-aliases-aliasId-DELETE
    /// </remarks>
    /// <param name="id">Name of AppBundle (unqualified)</param>
    /// <param name="aliasId">Name of alias to delete</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.AppBundlesManager.DeleteAppBundleAliasAsync("MyAppBundle", "MyAlias");
    /// </code>
    /// </example>
    public async Task DeleteAppBundleAliasAsync(
        string id,
        string aliasId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Da.UsEast.V3.Appbundles[id].Aliases[aliasId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists all versions of the specified AppBundle with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/appbundles/{id}/versions
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-id-versions-GET
    /// </remarks>
    /// <param name="id">Name of AppBundle (unqualified)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="int"/> version numbers, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var version in client.AppBundlesManager.ListAppBundleVersionsAsync("MyAppBundle"))
    /// {
    ///     Console.WriteLine($"Version: {version}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<int> ListAppBundleVersionsAsync(
        string id,
        RequestConfiguration<VersionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? page = requestConfiguration?.QueryParameters?.Page;

        while (true)
        {
            var response = await _api.Da.UsEast.V3.Appbundles[id].Versions
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Page = page;
                }, cancellationToken);

            if (response?.Data is not { Count: > 0 })
                yield break;

            foreach (var item in response.Data)
            {
                if (item.HasValue)
                    yield return item.Value;
            }

            if (string.IsNullOrEmpty(response.PaginationToken))
                yield break;

            page = response.PaginationToken;
        }
    }

    /// <summary>
    /// Creates a new version of the specified AppBundle.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /da/us-east/v3/appbundles/{id}/versions
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-id-versions-POST
    /// </remarks>
    /// <param name="id">Name of AppBundle (unqualified)</param>
    /// <param name="body">The AppBundle version creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VersionsPostResponse"/> containing the created version with upload parameters</returns>
    /// <example>
    /// <code>
    /// VersionsPostResponse? version = await client.AppBundlesManager.CreateAppBundleVersionAsync("MyAppBundle", new VersionsPostRequestBody
    /// {
    ///     Engine = "Autodesk.AutoCAD+24",
    ///     Description = "Updated version"
    /// });
    /// </code>
    /// </example>
    public async Task<VersionsPostResponse?> CreateAppBundleVersionAsync(
        string id,
        VersionsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Appbundles[id].Versions
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets the details of the specified version of the AppBundle.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/appbundles/{id}/versions/{version}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-id-versions-version-GET
    /// </remarks>
    /// <param name="id">Name of AppBundle (unqualified)</param>
    /// <param name="version">Version number to retrieve</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithVersionGetResponse"/> containing the version details</returns>
    /// <example>
    /// <code>
    /// WithVersionGetResponse? version = await client.AppBundlesManager.GetAppBundleVersionAsync("MyAppBundle", 1);
    /// </code>
    /// </example>
    public async Task<WithVersionGetResponse?> GetAppBundleVersionAsync(
        string id,
        int version,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Appbundles[id].Versions[version]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes the specified version of the AppBundle.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /da/us-east/v3/appbundles/{id}/versions/{version}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/appbundles-id-versions-version-DELETE
    /// </remarks>
    /// <param name="id">Name of AppBundle (unqualified)</param>
    /// <param name="version">Version number to delete</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.AppBundlesManager.DeleteAppBundleVersionAsync("MyAppBundle", 1);
    /// </code>
    /// </example>
    public async Task DeleteAppBundleVersionAsync(
        string id,
        int version,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Da.UsEast.V3.Appbundles[id].Versions[version]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
