using System.Runtime.CompilerServices;
using Autodesk.Automation.Da.UsEast.V3.Activities;
using Autodesk.Automation.Da.UsEast.V3.Activities.Item.Aliases;
using Autodesk.Automation.Da.UsEast.V3.Activities.Item.Aliases.Item;
using Autodesk.Automation.Da.UsEast.V3.Activities.Item.Versions;
using Autodesk.Automation.Da.UsEast.V3.Activities.Item.Versions.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.Automation.Da.UsEast.V3.Activities.ActivitiesRequestBuilder;
using static Autodesk.Automation.Da.UsEast.V3.Activities.Item.Aliases.AliasesRequestBuilder;
using static Autodesk.Automation.Da.UsEast.V3.Activities.Item.Versions.VersionsRequestBuilder;
using ActivityDetails = Autodesk.Automation.Da.UsEast.V3.Activities.Item.ActivitiesGetResponse;

namespace Autodesk.Automation.Managers;

/// <summary>
/// Manager for Activity operations — manages Design Automation activities, aliases, and versions.
/// </summary>
public class ActivitiesManager
{
    private readonly BaseAutomationClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivitiesManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ActivitiesManager(BaseAutomationClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists all available Activities, including Activities shared with this app, with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/activities
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="string"/> activity IDs, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var activityId in client.ActivitiesManager.ListActivitiesAsync())
    /// {
    ///     Console.WriteLine(activityId);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<string> ListActivitiesAsync(
        RequestConfiguration<ActivitiesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? page = requestConfiguration?.QueryParameters?.Page;

        while (true)
        {
            var response = await _api.Da.UsEast.V3.Activities
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
    /// Creates a new Activity.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /da/us-east/v3/activities
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-POST
    /// </remarks>
    /// <param name="body">The Activity creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ActivitiesPostResponse"/> containing the created Activity</returns>
    /// <example>
    /// <code>
    /// ActivitiesPostResponse? activity = await client.ActivitiesManager.CreateActivityAsync(new ActivitiesPostRequestBody
    /// {
    ///     Id = "MyActivity",
    ///     Engine = "Autodesk.AutoCAD+24",
    ///     CommandLine = ["$(engine.path)\\accoreconsole.exe /i $(args[input].path) /s $(settings[script].path)"]
    /// });
    /// </code>
    /// </example>
    public async Task<ActivitiesPostResponse?> CreateActivityAsync(
        ActivitiesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Activities
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets the details of the specified Activity. The id parameter must be a QualifiedId (owner.name+label).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/activities/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-id-GET
    /// </remarks>
    /// <param name="id">Full qualified id of the Activity (owner.name+label)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ActivityDetails"/> containing the Activity details</returns>
    /// <example>
    /// <code>
    /// ActivityDetails? activity = await client.ActivitiesManager.GetActivityAsync("MyNickname.MyActivity+MyAlias");
    /// </code>
    /// </example>
    public async Task<ActivityDetails?> GetActivityAsync(
        string id,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Activities[id]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes the specified Activity, including all versions and aliases.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /da/us-east/v3/activities/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-id-DELETE
    /// </remarks>
    /// <param name="id">Name of Activity (unqualified)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.ActivitiesManager.DeleteActivityAsync("MyActivity");
    /// </code>
    /// </example>
    public async Task DeleteActivityAsync(
        string id,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Da.UsEast.V3.Activities[id]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists all aliases for the specified Activity with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/activities/{id}/aliases
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-id-aliases-GET
    /// </remarks>
    /// <param name="id">Name of Activity (unqualified)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="AliasesGetResponse_data"/> alias items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var alias in client.ActivitiesManager.ListActivityAliasesAsync("MyActivity"))
    /// {
    ///     Console.WriteLine($"{alias.Id} -> v{alias.Version}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<AliasesGetResponse_data> ListActivityAliasesAsync(
        string id,
        RequestConfiguration<AliasesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? page = requestConfiguration?.QueryParameters?.Page;

        while (true)
        {
            var response = await _api.Da.UsEast.V3.Activities[id].Aliases
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
    /// Creates a new alias for the specified Activity.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /da/us-east/v3/activities/{id}/aliases
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-id-aliases-POST
    /// </remarks>
    /// <param name="id">Name of Activity (unqualified)</param>
    /// <param name="body">The alias creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="AliasesPostResponse"/> containing the created alias</returns>
    /// <example>
    /// <code>
    /// AliasesPostResponse? alias = await client.ActivitiesManager.CreateActivityAliasAsync("MyActivity", new AliasesPostRequestBody
    /// {
    ///     Id = "MyAlias",
    ///     Version = 1
    /// });
    /// </code>
    /// </example>
    public async Task<AliasesPostResponse?> CreateActivityAliasAsync(
        string id,
        AliasesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Activities[id].Aliases
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets the details of a specific Activity alias.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/activities/{id}/aliases/{aliasId}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-id-aliases-aliasId-GET
    /// </remarks>
    /// <param name="id">Name of Activity (unqualified)</param>
    /// <param name="aliasId">Name of alias</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithAliasGetResponse"/> containing the alias details</returns>
    /// <example>
    /// <code>
    /// WithAliasGetResponse? alias = await client.ActivitiesManager.GetActivityAliasAsync("MyActivity", "MyAlias");
    /// </code>
    /// </example>
    public async Task<WithAliasGetResponse?> GetActivityAliasAsync(
        string id,
        string aliasId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Activities[id].Aliases[aliasId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Modifies alias details for the specified Activity.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /da/us-east/v3/activities/{id}/aliases/{aliasId}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-id-aliases-aliasId-PATCH
    /// </remarks>
    /// <param name="id">Name of Activity (unqualified)</param>
    /// <param name="aliasId">Name of alias</param>
    /// <param name="body">The alias update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithAliasPatchResponse"/> containing the updated alias</returns>
    /// <example>
    /// <code>
    /// WithAliasPatchResponse? alias = await client.ActivitiesManager.UpdateActivityAliasAsync("MyActivity", "MyAlias", new WithAliasPatchRequestBody
    /// {
    ///     Version = 2
    /// });
    /// </code>
    /// </example>
    public async Task<WithAliasPatchResponse?> UpdateActivityAliasAsync(
        string id,
        string aliasId,
        WithAliasPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Activities[id].Aliases[aliasId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes the specified Activity alias.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /da/us-east/v3/activities/{id}/aliases/{aliasId}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-id-aliases-aliasId-DELETE
    /// </remarks>
    /// <param name="id">Name of Activity (unqualified)</param>
    /// <param name="aliasId">Name of alias to delete</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.ActivitiesManager.DeleteActivityAliasAsync("MyActivity", "MyAlias");
    /// </code>
    /// </example>
    public async Task DeleteActivityAliasAsync(
        string id,
        string aliasId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Da.UsEast.V3.Activities[id].Aliases[aliasId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Lists all versions of the specified Activity with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/activities/{id}/versions
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-id-versions-GET
    /// </remarks>
    /// <param name="id">Name of Activity (unqualified)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="int"/> version numbers, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var version in client.ActivitiesManager.ListActivityVersionsAsync("MyActivity"))
    /// {
    ///     Console.WriteLine($"Version: {version}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<int> ListActivityVersionsAsync(
        string id,
        RequestConfiguration<VersionsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? page = requestConfiguration?.QueryParameters?.Page;

        while (true)
        {
            var response = await _api.Da.UsEast.V3.Activities[id].Versions
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
    /// Creates a new version of the specified Activity.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /da/us-east/v3/activities/{id}/versions
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-id-versions-POST
    /// </remarks>
    /// <param name="id">Name of Activity (unqualified)</param>
    /// <param name="body">The Activity version creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="VersionsPostResponse"/> containing the created version</returns>
    /// <example>
    /// <code>
    /// VersionsPostResponse? version = await client.ActivitiesManager.CreateActivityVersionAsync("MyActivity", new VersionsPostRequestBody
    /// {
    ///     Engine = "Autodesk.AutoCAD+24",
    ///     CommandLine = ["$(engine.path)\\accoreconsole.exe /i $(args[input].path)"]
    /// });
    /// </code>
    /// </example>
    public async Task<VersionsPostResponse?> CreateActivityVersionAsync(
        string id,
        VersionsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Activities[id].Versions
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Gets the details of the specified version of the Activity.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/activities/{id}/versions/{version}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-id-versions-version-GET
    /// </remarks>
    /// <param name="id">Name of Activity (unqualified)</param>
    /// <param name="version">Version number to retrieve</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithVersionGetResponse"/> containing the version details</returns>
    /// <example>
    /// <code>
    /// WithVersionGetResponse? version = await client.ActivitiesManager.GetActivityVersionAsync("MyActivity", 1);
    /// </code>
    /// </example>
    public async Task<WithVersionGetResponse?> GetActivityVersionAsync(
        string id,
        int version,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Activities[id].Versions[version]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes the specified version of the Activity.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /da/us-east/v3/activities/{id}/versions/{version}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/activities-id-versions-version-DELETE
    /// </remarks>
    /// <param name="id">Name of Activity (unqualified)</param>
    /// <param name="version">Version number to delete</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.ActivitiesManager.DeleteActivityVersionAsync("MyActivity", 1);
    /// </code>
    /// </example>
    public async Task DeleteActivityVersionAsync(
        string id,
        int version,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Da.UsEast.V3.Activities[id].Versions[version]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
