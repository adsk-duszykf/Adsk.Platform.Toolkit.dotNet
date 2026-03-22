using System.Runtime.CompilerServices;
using Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.ClassificationSystems;
using Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.ClassificationSystems.Item;
using Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.ClassificationSystems.Item.Classifications;
using Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.ClassificationSystems.Item.ClassificationsImport;
using Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.ContentViews;
using Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.Packages;
using Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.Packages.Item;
using Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.Packages.Item.TakeoffItems;
using Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.Packages.Item.TakeoffItems.Item;
using Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.Packages.Item.TakeoffTypes;
using Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.Packages.Item.TakeoffTypes.Item;
using Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.Settings;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.ClassificationSystems.ClassificationSystemsRequestBuilder;
using static Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.ClassificationSystems.Item.Classifications.ClassificationsRequestBuilder;
using static Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.ContentViews.ContentViewsRequestBuilder;
using static Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.Packages.PackagesRequestBuilder;
using static Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.Packages.Item.TakeoffItems.TakeoffItemsRequestBuilder;
using static Autodesk.ACC.Construction.Takeoff.V1.Projects.Item.Packages.Item.TakeoffTypes.TakeoffTypesRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Takeoff operations — manages takeoff packages, items, types, and classification systems.
/// </summary>
public class TakeoffManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="TakeoffManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public TakeoffManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves the measurement system settings for a project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/takeoff/v1/projects/{projectId}/settings
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-settings-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SettingsGetResponse"/> containing the project measurement system settings</returns>
    /// <example>
    /// <code>
    /// SettingsGetResponse? settings = await client.TakeoffManager.GetSettingsAsync(projectId);
    /// </code>
    /// </example>
    public async Task<SettingsGetResponse?> GetSettingsAsync(
        Guid projectId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Takeoff.V1.Projects[projectId]
            .Settings
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Updates the measurement system settings for a project.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/takeoff/v1/projects/{projectId}/settings
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-settings-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The measurement system update body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="SettingsPatchResponse"/> containing the updated measurement system settings</returns>
    /// <example>
    /// <code>
    /// SettingsPatchRequestBody body = new SettingsPatchRequestBody { MeasurementSystem = "METRIC" };
    /// SettingsPatchResponse? settings = await client.TakeoffManager.UpdateSettingsAsync(projectId, body);
    /// </code>
    /// </example>
    public async Task<SettingsPatchResponse?> UpdateSettingsAsync(
        Guid projectId,
        SettingsPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Takeoff.V1.Projects[projectId]
            .Settings
            .PatchAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves classification systems for a project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/takeoff/v1/projects/{projectId}/classification-systems
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-classification-systems-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{ClassificationSystemsGetResponse_results}"/> of <see cref="ClassificationSystemsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var system in client.TakeoffManager.ListClassificationSystemsAsync(projectId))
    /// {
    ///     Console.WriteLine(system.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ClassificationSystemsGetResponse_results> ListClassificationSystemsAsync(
        Guid projectId,
        RequestConfiguration<ClassificationSystemsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Takeoff.V1.Projects[projectId]
                .ClassificationSystems
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
    /// Creates a classification system for a project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/takeoff/v1/projects/{projectId}/classification-systems
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-classification-systems-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The classification system creation body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ClassificationSystemsPostResponse"/> containing the created classification system</returns>
    /// <example>
    /// <code>
    /// ClassificationSystemsPostRequestBody body = new ClassificationSystemsPostRequestBody { Name = "My System", Type = "CLASSIFICATION_SYSTEM_1" };
    /// ClassificationSystemsPostResponse? system = await client.TakeoffManager.CreateClassificationSystemAsync(projectId, body);
    /// </code>
    /// </example>
    public async Task<ClassificationSystemsPostResponse?> CreateClassificationSystemAsync(
        Guid projectId,
        ClassificationSystemsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Takeoff.V1.Projects[projectId]
            .ClassificationSystems
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves details of a specified classification system.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/takeoff/v1/projects/{projectId}/classification-systems/{systemId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-classification-systems-system_id-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="systemId">The classification system ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithSystemGetResponse"/> containing the classification system details</returns>
    /// <example>
    /// <code>
    /// WithSystemGetResponse? system = await client.TakeoffManager.GetClassificationSystemAsync(projectId, systemId);
    /// </code>
    /// </example>
    public async Task<WithSystemGetResponse?> GetClassificationSystemAsync(
        Guid projectId,
        Guid systemId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Takeoff.V1.Projects[projectId]
            .ClassificationSystems[systemId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Deletes a classification system from a project.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /construction/takeoff/v1/projects/{projectId}/classification-systems/{systemId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-classification-systems-system_id-DELETE
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="systemId">The classification system ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.TakeoffManager.DeleteClassificationSystemAsync(projectId, systemId);
    /// </code>
    /// </example>
    public async Task DeleteClassificationSystemAsync(
        Guid projectId,
        Guid systemId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Takeoff.V1.Projects[projectId]
            .ClassificationSystems[systemId]
            .DeleteAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the classification hierarchy for a classification system with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/takeoff/v1/projects/{projectId}/classification-systems/{systemId}/classifications
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-classification-systems-system_id-classifications-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="systemId">The classification system ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{ClassificationsGetResponse_results}"/> of <see cref="ClassificationsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var classification in client.TakeoffManager.ListClassificationsAsync(projectId, systemId))
    /// {
    ///     Console.WriteLine(classification.Code);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ClassificationsGetResponse_results> ListClassificationsAsync(
        Guid projectId,
        Guid systemId,
        RequestConfiguration<ClassificationsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Takeoff.V1.Projects[projectId]
                .ClassificationSystems[systemId]
                .Classifications
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
    /// Updates a classification system by importing classifications.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/takeoff/v1/projects/{projectId}/classification-systems/{systemId}/classifications:import
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-classification-systems-system_id-classificationsimport-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="systemId">The classification system ID</param>
    /// <param name="body">The import body containing classifications</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="ClassificationsImportPostResponse"/> containing the import result</returns>
    /// <example>
    /// <code>
    /// ClassificationsImportPostRequestBody body = new ClassificationsImportPostRequestBody { Classifications = new List&lt;...&gt;() };
    /// ClassificationsImportPostResponse? result = await client.TakeoffManager.ImportClassificationsAsync(projectId, systemId, body);
    /// </code>
    /// </example>
    public async Task<ClassificationsImportPostResponse?> ImportClassificationsAsync(
        Guid projectId,
        Guid systemId,
        ClassificationsImportPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Takeoff.V1.Projects[projectId]
            .ClassificationSystems[systemId]
            .ClassificationsImport
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves takeoff packages for a project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/takeoff/v1/projects/{projectId}/packages
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-packages-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{PackagesGetResponse_results}"/> of <see cref="PackagesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var pkg in client.TakeoffManager.ListPackagesAsync(projectId))
    /// {
    ///     Console.WriteLine(pkg.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<PackagesGetResponse_results> ListPackagesAsync(
        Guid projectId,
        RequestConfiguration<PackagesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Takeoff.V1.Projects[projectId]
                .Packages
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
    /// Creates a takeoff package for a project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/takeoff/v1/projects/{projectId}/packages
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-packages-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The package creation body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="PackagesPostResponse"/> containing the created takeoff package</returns>
    /// <example>
    /// <code>
    /// PackagesPostRequestBody body = new PackagesPostRequestBody { Name = "My Package" };
    /// PackagesPostResponse? pkg = await client.TakeoffManager.CreatePackageAsync(projectId, body);
    /// </code>
    /// </example>
    public async Task<PackagesPostResponse?> CreatePackageAsync(
        Guid projectId,
        PackagesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Takeoff.V1.Projects[projectId]
            .Packages
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a specified takeoff package.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/takeoff/v1/projects/{projectId}/packages/{packageId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-packages-package_id-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="packageId">The takeoff package ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithPackageGetResponse"/> containing the takeoff package details</returns>
    /// <example>
    /// <code>
    /// WithPackageGetResponse? pkg = await client.TakeoffManager.GetPackageAsync(projectId, packageId);
    /// </code>
    /// </example>
    public async Task<WithPackageGetResponse?> GetPackageAsync(
        Guid projectId,
        Guid packageId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Takeoff.V1.Projects[projectId]
            .Packages[packageId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Updates the name of a takeoff package.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/takeoff/v1/projects/{projectId}/packages/{packageId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-packages-package_id-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="packageId">The takeoff package ID</param>
    /// <param name="body">The package update body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithPackagePatchResponse"/> containing the updated takeoff package</returns>
    /// <example>
    /// <code>
    /// WithPackagePatchRequestBody body = new WithPackagePatchRequestBody { Name = "Updated Name" };
    /// WithPackagePatchResponse? pkg = await client.TakeoffManager.UpdatePackageAsync(projectId, packageId, body);
    /// </code>
    /// </example>
    public async Task<WithPackagePatchResponse?> UpdatePackageAsync(
        Guid projectId,
        Guid packageId,
        WithPackagePatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Takeoff.V1.Projects[projectId]
            .Packages[packageId]
            .PatchAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves takeoff types for a package with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/takeoff/v1/projects/{projectId}/packages/{packageId}/takeoff-types
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-packages-package_id-takeoff-types-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="packageId">The takeoff package ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{TakeoffTypesGetResponse_results}"/> of <see cref="TakeoffTypesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var type in client.TakeoffManager.ListTakeoffTypesAsync(projectId, packageId))
    /// {
    ///     Console.WriteLine(type.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<TakeoffTypesGetResponse_results> ListTakeoffTypesAsync(
        Guid projectId,
        Guid packageId,
        RequestConfiguration<TakeoffTypesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Takeoff.V1.Projects[projectId]
                .Packages[packageId]
                .TakeoffTypes
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
    /// Retrieves a specified takeoff type for a package.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/takeoff/v1/projects/{projectId}/packages/{packageId}/takeoff-types/{takeoffTypeId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-packages-package_id-takeoff-types-takeoff_type_id-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="packageId">The takeoff package ID</param>
    /// <param name="takeoffTypeId">The takeoff type ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithTakeoffTypeGetResponse"/> containing the takeoff type details</returns>
    /// <example>
    /// <code>
    /// WithTakeoffTypeGetResponse? type = await client.TakeoffManager.GetTakeoffTypeAsync(projectId, packageId, takeoffTypeId);
    /// </code>
    /// </example>
    public async Task<WithTakeoffTypeGetResponse?> GetTakeoffTypeAsync(
        Guid projectId,
        Guid packageId,
        Guid takeoffTypeId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Takeoff.V1.Projects[projectId]
            .Packages[packageId]
            .TakeoffTypes[takeoffTypeId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves takeoff items for a package with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/takeoff/v1/projects/{projectId}/packages/{packageId}/takeoff-items
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-packages-package_id-takeoff-items-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="packageId">The takeoff package ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{TakeoffItemsGetResponse_results}"/> of <see cref="TakeoffItemsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var item in client.TakeoffManager.ListTakeoffItemsAsync(projectId, packageId))
    /// {
    ///     Console.WriteLine(item.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<TakeoffItemsGetResponse_results> ListTakeoffItemsAsync(
        Guid projectId,
        Guid packageId,
        RequestConfiguration<TakeoffItemsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Takeoff.V1.Projects[projectId]
                .Packages[packageId]
                .TakeoffItems
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
    /// Retrieves a specified takeoff item for a package.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/takeoff/v1/projects/{projectId}/packages/{packageId}/takeoff-items/{takeoffItemId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-packages-package_id-takeoff-items-takeoff_item_id-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="packageId">The takeoff package ID</param>
    /// <param name="takeoffItemId">The takeoff item ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithTakeoffItemGetResponse"/> containing the takeoff item details</returns>
    /// <example>
    /// <code>
    /// WithTakeoffItemGetResponse? item = await client.TakeoffManager.GetTakeoffItemAsync(projectId, packageId, takeoffItemId);
    /// </code>
    /// </example>
    public async Task<WithTakeoffItemGetResponse?> GetTakeoffItemAsync(
        Guid projectId,
        Guid packageId,
        Guid takeoffItemId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Takeoff.V1.Projects[projectId]
            .Packages[packageId]
            .TakeoffItems[takeoffItemId]
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves content views for a project with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/takeoff/v1/projects/{projectId}/content-views
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/takeoff-projects-project_id-content-views-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports Limit, Offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{ContentViewsGetResponse_results}"/> of <see cref="ContentViewsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var view in client.TakeoffManager.ListContentViewsAsync(projectId))
    /// {
    ///     Console.WriteLine(view.Id);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ContentViewsGetResponse_results> ListContentViewsAsync(
        Guid projectId,
        RequestConfiguration<ContentViewsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Takeoff.V1.Projects[projectId]
                .ContentViews
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
}
