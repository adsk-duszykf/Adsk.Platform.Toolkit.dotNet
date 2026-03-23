using System.Runtime.CompilerServices;
using Autodesk.Parameters.Parameters.V1.Classifications.Categories;
using Autodesk.Parameters.Parameters.V1.Disciplines;
using Autodesk.Parameters.Parameters.V1.Units;
using Microsoft.Kiota.Abstractions;
using ClassificationGroupsRequestBuilder = Autodesk.Parameters.Parameters.V1.Classifications.Groups.GroupsRequestBuilder;
using ClassificationGroupsGetResponse_results = Autodesk.Parameters.Parameters.V1.Classifications.Groups.GroupsGetResponse_results;

namespace Autodesk.Parameters.Managers;

/// <summary>
/// Manager for Classification operations including groups, categories, disciplines, and units
/// </summary>
public class ClassificationsManager
{
    private readonly BaseParametersClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassificationsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ClassificationsManager(BaseParametersClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lists classification groups with automatic pagination. Classification groups organize a set of parameters applied to an object by further grouping together as a subset.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/classifications/groups
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-listclassificationgroupsv1-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filter[bindable], ids, limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="ClassificationGroupsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var group in client.ClassificationsManager.ListClassificationGroupsAsync())
    /// {
    ///     Console.WriteLine($"{group.Id}: {group.Name}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<ClassificationGroupsGetResponse_results> ListClassificationGroupsAsync(
        RequestConfiguration<ClassificationGroupsRequestBuilder.GroupsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Parameters.V1.Classifications.Groups
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
    /// Lists categories defined in the system with automatic pagination. Categories can be found in various contexts.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/classifications/categories
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-listcategoriesv1-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filter[bindable], ids, limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="CategoriesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var category in client.ClassificationsManager.ListCategoriesAsync())
    /// {
    ///     Console.WriteLine($"{category.Id}: {category.Name}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CategoriesGetResponse_results> ListCategoriesAsync(
        RequestConfiguration<CategoriesRequestBuilder.CategoriesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Parameters.V1.Classifications.Categories
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
    /// Lists disciplines used in the system with automatic pagination. Currently, disciplines used in Revit are included.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/disciplines
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-listdisciplinesv1-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports ids, limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="DisciplinesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var discipline in client.ClassificationsManager.ListDisciplinesAsync())
    /// {
    ///     Console.WriteLine($"{discipline.Id}: {discipline.Name}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<DisciplinesGetResponse_results> ListDisciplinesAsync(
        RequestConfiguration<DisciplinesRequestBuilder.DisciplinesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Parameters.V1.Disciplines
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
    /// Lists units defined in the system with automatic pagination.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /parameters/v1/units
    /// APS docs: https://aps.autodesk.com/en/docs/parameters/v1/reference/http/parameters-listunitsv1-GET
    /// </remarks>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports ids, limit, offset)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="UnitsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var unit in client.ClassificationsManager.ListUnitsAsync())
    /// {
    ///     Console.WriteLine($"{unit.Id}: {unit.Name}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<UnitsGetResponse_results> ListUnitsAsync(
        RequestConfiguration<UnitsRequestBuilder.UnitsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Parameters.V1.Units
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
