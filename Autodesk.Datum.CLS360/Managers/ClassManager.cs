using System.Runtime.CompilerServices;
using Autodesk.Datum.CLS360.Domains.Item.Classes;
using Autodesk.Datum.CLS360.Domains.Item.Classes.Item;
using Autodesk.Datum.CLS360.Domains.Item.Classes.Item.AttributeMappings;
using Autodesk.Datum.CLS360.Domains.Item.Classes.Item.OutputMappings;
using Autodesk.Datum.CLS360.Domains.Item.Classes.Item.UomGroup;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Datum.CLS360.Managers;

/// <summary>
/// Manager for class operations.
/// </summary>
public class ClassManager
{
    private readonly BaseDatumCLS360Client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassManager"/> class.
    /// </summary>
    /// <param name="api">The API client.</param>
    public ClassManager(BaseDatumCLS360Client api)
    {
        _api = api;
    }

    /// <summary>
    /// Returns class objects for the given domain.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /domains/{domHdl}/classes
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ClassesGetResponse_value"/> items, automatically paginated.</returns>
    /// <example>
    /// <code>
    /// await foreach (ClassesGetResponse_value classItem in client.ClassManager.ListClassesAsync("domain-handle"))
    /// {
    ///     Console.WriteLine(classItem.Hdl);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ClassesGetResponse_value> ListClassesAsync(
        string domHdl,
        RequestConfiguration<ClassesRequestBuilder.ClassesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var requestBuilder = _api.Domains[domHdl].Classes;
        while (true)
        {
            ClassesGetResponse? response = await requestBuilder.GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);

            if (response?.Value is not null)
                yield return response.Value;

            if (string.IsNullOrWhiteSpace(response?.NextLink))
                yield break;

            requestBuilder = requestBuilder.WithUrl(response.NextLink);
        }
    }

    /// <summary>
    /// Creates a new class in the given domain.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /domains/{domHdl}/classes
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="body">The create request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="ClassesPostResponse"/> containing the created class.</returns>
    /// <example>
    /// <code>
    /// ClassesPostResponse? response = await client.ClassManager.CreateClassAsync("domain-handle", new ClassesPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ClassesPostResponse?> CreateClassAsync(
        string domHdl,
        ClassesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].Classes.PostAsync(body, r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }

    /// <summary>
    /// Returns a class object selected by handle.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /domains/{domHdl}/classes/{clsHdl}
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="clsHdl">Class handle.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithClsHdlGetResponse"/> containing the class object.</returns>
    /// <example>
    /// <code>
    /// WithClsHdlGetResponse? response = await client.ClassManager.GetClassAsync("domain-handle", "class-handle");
    /// </code>
    /// </example>
    public async Task<WithClsHdlGetResponse?> GetClassAsync(
        string domHdl,
        string clsHdl,
        RequestConfiguration<WithClsHdlItemRequestBuilder.WithClsHdlItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].Classes[clsHdl].GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }

    /// <summary>
    /// Updates an existing class.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /domains/{domHdl}/classes/{clsHdl}
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="clsHdl">Class handle.</param>
    /// <param name="body">The update request body.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="WithClsHdlPostResponse"/> containing the updated class.</returns>
    /// <example>
    /// <code>
    /// WithClsHdlPostResponse? response = await client.ClassManager.UpdateClassAsync("domain-handle", "class-handle", new WithClsHdlPostRequestBody());
    /// </code>
    /// </example>
    public async Task<WithClsHdlPostResponse?> UpdateClassAsync(
        string domHdl,
        string clsHdl,
        WithClsHdlPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].Classes[clsHdl].PostAsync(body, r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }

    /// <summary>
    /// Returns output mappings for an ETL target class.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /domains/{domHdl}/classes/{clsHdl}/OutputMappings
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="clsHdl">ETL target handle.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="OutputMappingsGetResponse"/> containing output mappings.</returns>
    /// <example>
    /// <code>
    /// OutputMappingsGetResponse? response = await client.ClassManager.GetOutputMappingListAsync("domain-handle", "class-handle");
    /// </code>
    /// </example>
    public async Task<OutputMappingsGetResponse?> GetOutputMappingListAsync(
        string domHdl,
        string clsHdl,
        RequestConfiguration<OutputMappingsRequestBuilder.OutputMappingsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].Classes[clsHdl].OutputMappings.GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }

    /// <summary>
    /// Returns attribute mappings for an ETL source class.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /domains/{domHdl}/classes/{clsHdl}/AttributeMappings
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="clsHdl">ETL source handle.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="AttributeMappingsGetResponse"/> containing attribute mappings.</returns>
    /// <example>
    /// <code>
    /// AttributeMappingsGetResponse? response = await client.ClassManager.GetAttributeMappingListAsync("domain-handle", "class-handle");
    /// </code>
    /// </example>
    public async Task<AttributeMappingsGetResponse?> GetAttributeMappingListAsync(
        string domHdl,
        string clsHdl,
        RequestConfiguration<AttributeMappingsRequestBuilder.AttributeMappingsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].Classes[clsHdl].AttributeMappings.GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }

    /// <summary>
    /// Returns the specified UoM group for a class.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /domains/{domHdl}/classes/{clsHdl}/uom-group
    /// </remarks>
    /// <param name="domHdl">Domain handle.</param>
    /// <param name="clsHdl">UoM group handle.</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request.</param>
    /// <param name="cancellationToken">(Optional) Cancellation token.</param>
    /// <returns>A <see cref="UomGroupGetResponse"/> containing the UoM group details.</returns>
    /// <example>
    /// <code>
    /// UomGroupGetResponse? response = await client.ClassManager.GetUoMGroupAsync("domain-handle", "class-handle");
    /// </code>
    /// </example>
    public async Task<UomGroupGetResponse?> GetUoMGroupAsync(
        string domHdl,
        string clsHdl,
        RequestConfiguration<UomGroupRequestBuilder.UomGroupRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Domains[domHdl].Classes[clsHdl].UomGroup.GetAsync(r =>
        {
            r.Headers = requestConfiguration?.Headers ?? r.Headers;
            r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
            r.Options = requestConfiguration?.Options ?? r.Options;
        }, cancellationToken);
    }
}
