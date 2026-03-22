using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Invites;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Invites.Item;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.InvitesBatchImportEmails;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.InvitesImportEmails;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Invites.InvitesRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected invite and email-import operations.
/// </summary>
public class InvitesManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvitesManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public InvitesManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists invites sent by the current user&apos;s company with cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/invites
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-invites-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Query parameters (including <see cref="InvitesRequestBuilderGetQueryParameters.CursorState"/> for the first page), headers, and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="InvitesGetResponse_results"/> across all pages.</returns>
    /// <example>
    /// <code>
    /// await foreach (InvitesGetResponse_results invite in client.InvitesManager.ListInvitesAsync())
    /// {
    ///     Console.WriteLine(invite.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<InvitesGetResponse_results> ListInvitesAsync(
        RequestConfiguration<InvitesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            InvitesGetResponse? response = await _api.Construction.Buildingconnected.V2.Invites
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (InvitesGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Retrieves a single invite by identifier.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/invites/{inviteId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-invites-inviteId-GET
    /// </remarks>
    /// <param name="inviteId">The ID of the invite.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for headers and middleware options.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithInviteGetResponse"/> for the invite, or <c>null</c> if not found.</returns>
    /// <example>
    /// <code>
    /// WithInviteGetResponse? invite = await client.InvitesManager.GetInviteAsync("invite-id");
    /// </code>
    /// </example>
    public async Task<WithInviteGetResponse?> GetInviteAsync(
        string inviteId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Invites[inviteId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Adds bidders to a single bid package using their email addresses.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/invites:import-emails
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-invites-import-emails-POST
    /// </remarks>
    /// <param name="body">The import-emails payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="InvitesImportEmailsPostResponse"/> describing the operation result.</returns>
    /// <example>
    /// <code>
    /// InvitesImportEmailsPostResponse? result =
    ///     await client.InvitesManager.ImportEmailsAsync(new InvitesImportEmailsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<InvitesImportEmailsPostResponse?> ImportEmailsAsync(
        InvitesImportEmailsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.InvitesImportEmails
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Adds bidders to multiple bid packages using their email addresses.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/buildingconnected/v2/invites:batch-import-emails
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-invites-batch-import-emails-POST
    /// </remarks>
    /// <param name="body">The batch import-emails payload.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="InvitesBatchImportEmailsPostResponse"/> describing the operation result.</returns>
    /// <example>
    /// <code>
    /// InvitesBatchImportEmailsPostResponse? result =
    ///     await client.InvitesManager.BatchImportEmailsAsync(new InvitesBatchImportEmailsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<InvitesBatchImportEmailsPostResponse?> BatchImportEmailsAsync(
        InvitesBatchImportEmailsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.InvitesBatchImportEmails
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
