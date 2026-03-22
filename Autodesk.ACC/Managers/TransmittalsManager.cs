using System.Runtime.CompilerServices;
using Autodesk.ACC.Construction.Transmittals.V1.Projects.Item.Transmittals;
using Autodesk.ACC.Construction.Transmittals.V1.Projects.Item.Transmittals.Item;
using Autodesk.ACC.Construction.Transmittals.V1.Projects.Item.Transmittals.Item.Documents;
using Autodesk.ACC.Construction.Transmittals.V1.Projects.Item.Transmittals.Item.Folders;
using Autodesk.ACC.Construction.Transmittals.V1.Projects.Item.Transmittals.Item.Recipients;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.Construction.Transmittals.V1.Projects.Item.Transmittals.TransmittalsRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Transmittals operations — retrieves transmittal records, recipients,
/// folders, and documents for ACC projects.
/// </summary>
public class TransmittalsManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransmittalsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public TransmittalsManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves all transmittals created in the specified project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/transmittals/v1/projects/{projectId}/transmittals
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/transmittals-listtransmittals-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Sort, Limit, Offset query parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{TransmittalsGetResponse_results}"/> of <see cref="TransmittalsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var transmittal in client.TransmittalsManager.ListTransmittalsAsync(projectId))
    /// {
    ///     Console.WriteLine(transmittal.Title);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<TransmittalsGetResponse_results> ListTransmittalsAsync(
        Guid projectId,
        RequestConfiguration<TransmittalsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Transmittals.V1.Projects[projectId]
                .Transmittals
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (var item in response.Results)
            {
                yield return item;
            }

            if (string.IsNullOrEmpty(response.Pagination?.NextUrl))
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves a transmittal by ID within the specified project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/transmittals/v1/projects/{projectId}/transmittals/{transmittalId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/transmittals-gettransmittal-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="transmittalId">The ID of the transmittal</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithTransmittalGetResponse"/> containing the transmittal details</returns>
    /// <example>
    /// <code>
    /// WithTransmittalGetResponse? transmittal = await client.TransmittalsManager.GetTransmittalAsync(projectId, transmittalId);
    /// </code>
    /// </example>
    public async Task<WithTransmittalGetResponse?> GetTransmittalAsync(
        Guid projectId,
        Guid transmittalId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Transmittals.V1.Projects[projectId]
            .Transmittals[transmittalId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves all recipients of a specific transmittal, including project members and external members.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/transmittals/v1/projects/{projectId}/transmittals/{transmittalId}/recipients
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/transmittals-listtransmittalrecipients-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="transmittalId">The ID of the transmittal</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RecipientsGetResponse"/> containing the recipients of the transmittal</returns>
    /// <example>
    /// <code>
    /// RecipientsGetResponse? recipients = await client.TransmittalsManager.GetRecipientsAsync(projectId, transmittalId);
    /// </code>
    /// </example>
    public async Task<RecipientsGetResponse?> GetRecipientsAsync(
        Guid projectId,
        Guid transmittalId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Transmittals.V1.Projects[projectId]
            .Transmittals[transmittalId]
            .Recipients
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves all folders associated with the documents included in a specific transmittal.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/transmittals/v1/projects/{projectId}/transmittals/{transmittalId}/folders
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/transmittals-listtransmittalfolders-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="transmittalId">The ID of the transmittal</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Sort, Limit, Offset query parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="FoldersGetResponse"/> containing the folders response</returns>
    /// <example>
    /// <code>
    /// FoldersGetResponse? folders = await client.TransmittalsManager.GetFoldersAsync(projectId, transmittalId);
    /// </code>
    /// </example>
    public async Task<FoldersGetResponse?> GetFoldersAsync(
        Guid projectId,
        Guid transmittalId,
        RequestConfiguration<FoldersRequestBuilder.FoldersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Transmittals.V1.Projects[projectId]
            .Transmittals[transmittalId]
            .Folders
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the documents that were included in a specific transmittal.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/transmittals/v1/projects/{projectId}/transmittals/{transmittalId}/documents
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/transmittals-listtransmittaldocuments-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="transmittalId">The ID of the transmittal</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Sort, Limit, Offset query parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="DocumentsGetResponse"/> containing the documents response</returns>
    /// <example>
    /// <code>
    /// DocumentsGetResponse? documents = await client.TransmittalsManager.GetDocumentsAsync(projectId, transmittalId);
    /// </code>
    /// </example>
    public async Task<DocumentsGetResponse?> GetDocumentsAsync(
        Guid projectId,
        Guid transmittalId,
        RequestConfiguration<DocumentsRequestBuilder.DocumentsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Transmittals.V1.Projects[projectId]
            .Transmittals[transmittalId]
            .Documents
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }
}
