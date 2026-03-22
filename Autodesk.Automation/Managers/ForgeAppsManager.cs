using Autodesk.Automation.Da.UsEast.V3.Forgeapps.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Automation.Managers;

/// <summary>
/// Manager for ForgeApps (nickname) operations — manages app nicknames and app data deletion.
/// </summary>
public class ForgeAppsManager
{
    private readonly BaseAutomationClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgeAppsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ForgeAppsManager(BaseAutomationClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns the app's nickname.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/forgeapps/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/forgeapps-id-GET
    /// </remarks>
    /// <param name="id">Must be "me" for the call to succeed</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the nickname response</returns>
    /// <example>
    /// <code>
    /// Stream? nickname = await client.ForgeAppsManager.GetNicknameAsync("me");
    /// </code>
    /// </example>
    public async Task<Stream?> GetNicknameAsync(
        string id = "me",
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Forgeapps[id]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates or updates the nickname for the current app.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /da/us-east/v3/forgeapps/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/forgeapps-id-PATCH
    /// </remarks>
    /// <param name="body">The nickname update data</param>
    /// <param name="id">Must be "me" for the call to succeed</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Stream"/> containing the response</returns>
    /// <example>
    /// <code>
    /// Stream? result = await client.ForgeAppsManager.UpdateNicknameAsync(new ForgeappsPatchRequestBody
    /// {
    ///     Nickname = "MyNickname"
    /// });
    /// </code>
    /// </example>
    public async Task<Stream?> UpdateNicknameAsync(
        ForgeappsPatchRequestBody body,
        string id = "me",
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Forgeapps[id]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes all data associated with the given app.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /da/us-east/v3/forgeapps/{id}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/forgeapps-id-DELETE
    /// </remarks>
    /// <param name="id">Must be "me" for the call to succeed</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.ForgeAppsManager.DeleteAppDataAsync();
    /// </code>
    /// </example>
    public async Task DeleteAppDataAsync(
        string id = "me",
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Da.UsEast.V3.Forgeapps[id]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
