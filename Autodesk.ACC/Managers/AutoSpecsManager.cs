using Autodesk.ACC.Construction.Autospecs.V1.Projects.Item.Metadata;
using Autodesk.ACC.Construction.Autospecs.V1.Projects.Item.VersionNamespace.Item.Requirements;
using Autodesk.ACC.Construction.Autospecs.V1.Projects.Item.VersionNamespace.Item.Smartregister;
using Autodesk.ACC.Construction.Autospecs.V1.Projects.Item.VersionNamespace.Item.SubmittalsSummary;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for AutoSpecs operations — retrieves specification metadata, smart registers,
/// requirements, and submittal summaries for ACC projects.
/// </summary>
public class AutoSpecsManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoSpecsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public AutoSpecsManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves AutoSpecs-related information about the specified project, including details
    /// about the project versions and the region.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/autospecs/v1/projects/{projectId}/metadata
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/autospecs-getprojectmetadata-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="MetadataGetResponse"/> containing the AutoSpecs metadata for the project</returns>
    /// <example>
    /// <code>
    /// MetadataGetResponse? metadata = await client.AutoSpecsManager.GetMetadataAsync("projectId");
    /// </code>
    /// </example>
    public async Task<MetadataGetResponse?> GetMetadataAsync(
        string projectId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Autospecs.V1.Projects[projectId]
            .Metadata
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves the submittal logs (Smart Register) from the specification PDFs
    /// imported into AutoSpecs for a specific project version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/autospecs/v1/projects/{projectId}/version/{versionId}/smartregister
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/autospecs-getversionsmartregister-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="versionId">The ID of the version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SmartregisterGetResponse"/> containing the smart register data</returns>
    /// <example>
    /// <code>
    /// SmartregisterGetResponse? register = await client.AutoSpecsManager.GetSmartRegisterAsync("projectId", "versionId");
    /// </code>
    /// </example>
    public async Task<SmartregisterGetResponse?> GetSmartRegisterAsync(
        string projectId,
        string versionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Autospecs.V1.Projects[projectId]
            .Version[versionId]
            .Smartregister
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves the number of submittals for the submittal groups in each submittal section
    /// for a specific project version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/autospecs/v1/projects/{projectId}/version/{versionId}/requirements
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/autospecs-getversionrequirements-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="versionId">The ID of the version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="RequirementsGetResponse"/> containing the requirements data</returns>
    /// <example>
    /// <code>
    /// RequirementsGetResponse? requirements = await client.AutoSpecsManager.GetRequirementsAsync("projectId", "versionId");
    /// </code>
    /// </example>
    public async Task<RequirementsGetResponse?> GetRequirementsAsync(
        string projectId,
        string versionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Autospecs.V1.Projects[projectId]
            .Version[versionId]
            .Requirements
            .GetAsync(requestConfiguration, cancellationToken);
    }

    /// <summary>
    /// Retrieves the number of submittals for each submittal group and each submittal type
    /// for a specific project version.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/autospecs/v1/projects/{projectId}/version/{versionId}/submittalsSummary
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/autospecs-getversionsummary-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="versionId">The ID of the version</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SubmittalsSummaryGetResponse"/> containing the submittals summary data</returns>
    /// <example>
    /// <code>
    /// SubmittalsSummaryGetResponse? summary = await client.AutoSpecsManager.GetSubmittalsSummaryAsync("projectId", "versionId");
    /// </code>
    /// </example>
    public async Task<SubmittalsSummaryGetResponse?> GetSubmittalsSummaryAsync(
        string projectId,
        string versionId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Autospecs.V1.Projects[projectId]
            .Version[versionId]
            .SubmittalsSummary
            .GetAsync(requestConfiguration, cancellationToken);
    }
}
