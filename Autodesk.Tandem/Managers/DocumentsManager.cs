using Autodesk.Tandem.Tandem.V1.Twins.Item.Documents;
using Autodesk.Tandem.Tandem.V1.Twins.Item.Documents.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Tandem.Managers;

/// <summary>
/// Manager for Documents operations
/// </summary>
public class DocumentsManager
{
    private readonly BaseTandemClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public DocumentsManager(BaseTandemClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Create Tandem documents from ACC files. File contents are copied to Tandem-managed storage system.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /tandem/v1/twins/{twinID}/documents
    /// </remarks>
    /// <param name="twinId">Twin URN</param>
    /// <param name="body">The document creation request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="DocumentsPostResponse"/> containing the created document details</returns>
    /// <example>
    /// <code>
    /// DocumentsPostResponse? result = await client.DocumentsManager.CreateDocumentAsync("urn:adsk.dtt:...", new DocumentsPostRequestBody { Name = "document.pdf" });
    /// </code>
    /// </example>
    public async Task<DocumentsPostResponse?> CreateDocumentAsync(
        string twinId,
        DocumentsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Twins[twinId].Documents
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns Tandem document definition.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /tandem/v1/twins/{twinID}/documents/{documentID}
    /// </remarks>
    /// <param name="twinId">Twin URN</param>
    /// <param name="documentId">Document ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithDocumentGetResponse"/> containing the document definition</returns>
    /// <example>
    /// <code>
    /// WithDocumentGetResponse? doc = await client.DocumentsManager.GetDocumentAsync("urn:adsk.dtt:...", "docId");
    /// </code>
    /// </example>
    public async Task<WithDocumentGetResponse?> GetDocumentAsync(
        string twinId,
        string documentId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Tandem.V1.Twins[twinId].Documents[documentId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Deletes Tandem document definition and its content.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /tandem/v1/twins/{twinID}/documents/{documentID}
    /// </remarks>
    /// <param name="twinId">Twin URN</param>
    /// <param name="documentId">Document ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.DocumentsManager.DeleteDocumentAsync("urn:adsk.dtt:...", "docId");
    /// </code>
    /// </example>
    public async Task DeleteDocumentAsync(
        string twinId,
        string documentId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Tandem.V1.Twins[twinId].Documents[documentId]
            .DeleteAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
