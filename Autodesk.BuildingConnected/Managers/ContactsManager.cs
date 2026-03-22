using System.Runtime.CompilerServices;
using Autodesk.BuildingConnected;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Contacts;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Contacts.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Contacts.ContactsRequestBuilder;

namespace Autodesk.BuildingConnected.Managers;

/// <summary>
/// Manager for BuildingConnected contact operations — lists contacts and retrieves a contact by id.
/// </summary>
public class ContactsManager
{
    private readonly BaseBuildingConnectedClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContactsManager"/> class.
    /// </summary>
    /// <param name="api">The BuildingConnected API client.</param>
    public ContactsManager(BaseBuildingConnectedClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists BuildingConnected contacts with automatic cursor-based pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/contacts
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-contacts-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request, including cursor and filter query parameters.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ContactsGetResponse_results"/>.</returns>
    /// <example>
    /// <code>
    /// await foreach (var contact in client.ContactsManager.ListContactsAsync())
    /// {
    ///     Console.WriteLine(contact);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ContactsGetResponse_results> ListContactsAsync(
        RequestConfiguration<ContactsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = requestConfiguration?.QueryParameters?.CursorState;

        while (true)
        {
            ContactsGetResponse? response = await _api.Construction.Buildingconnected.V2.Contacts
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.CursorState = cursor;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (ContactsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.CursorState))
                yield break;

            cursor = response.Pagination.CursorState;
        }
    }

    /// <summary>
    /// Gets a BuildingConnected contact by id.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/buildingconnected/v2/contacts/{contactId}
    /// APS docs: https://aps.autodesk.com/en/docs/buildingconnected/v2/reference/http/buildingconnected-contacts-contactId-GET
    /// </remarks>
    /// <param name="contactId">The contact id.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithContactGetResponse"/> instance, or <c>null</c> if the response is empty.</returns>
    /// <example>
    /// <code>
    /// WithContactGetResponse? contact = await client.ContactsManager.GetContactAsync("contact-id-here");
    /// </code>
    /// </example>
    public async Task<WithContactGetResponse?> GetContactAsync(
        string contactId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Buildingconnected.V2.Contacts[contactId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
