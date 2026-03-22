using System.Runtime.CompilerServices;
using Autodesk.BIM360.Construction.Admin.V1.Accounts.Item.Companies;
using Autodesk.BIM360.Construction.Admin.V1.Accounts.Item.Projects;
using Autodesk.BIM360.Construction.Admin.V1.Projects.Item.Users.Item;
using Autodesk.BIM360.Hq.V1.Accounts.Item.Business_units_structure;
using Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.Item;
using Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.Item.Image;
using Autodesk.BIM360.Hq.V1.Accounts.Item.Users.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.BIM360.Construction.Admin.V1.Accounts.Item.Companies.CompaniesRequestBuilder;
using static Autodesk.BIM360.Construction.Admin.V1.Accounts.Item.Projects.ProjectsRequestBuilder;

namespace Autodesk.BIM360.Managers;

/// <summary>
/// Manager for Account Admin operations — manages accounts, projects, users, companies, and business units (BIM 360 HQ, Construction Admin, and BIM 360 Admin APIs).
/// </summary>
public class AccountAdminManager
{
    private readonly BaseBIM360client _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountAdminManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public AccountAdminManager(BaseBIM360client api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves a paginated list of projects in an account with automatic page fetching (Construction Admin API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/accounts/{accountId}/projects
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/admin-accounts-accountidprojects-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account (hub ID without the b. prefix)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Autodesk.BIM360.Construction.Admin.V1.Accounts.Item.Projects.ProjectsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (Autodesk.BIM360.Construction.Admin.V1.Accounts.Item.Projects.ProjectsGetResponse_results project in client.AccountAdminManager.ListProjectsAsync(accountId))
    /// {
    ///     Console.WriteLine(project.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Construction.Admin.V1.Accounts.Item.Projects.ProjectsGetResponse_results> ListProjectsAsync(
        Guid accountId,
        RequestConfiguration<ProjectsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            Construction.Admin.V1.Accounts.Item.Projects.ProjectsGetResponse? response = await _api.Construction.Admin.V1.Accounts[accountId]
                .Projects
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (Construction.Admin.V1.Accounts.Item.Projects.ProjectsGetResponse_results item in response.Results)
            {
                yield return item;
            }

            int totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves one HTTP response for GET /hq/v1/accounts/{account_id}/projects. The OpenAPI model is a single object; use query parameters limit and offset for paging at the HTTP level.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/projects
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/projects-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>The deserialized response body as modeled in the generated client</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.ProjectsGetResponse? page = await client.AccountAdminManager.GetHqProjectsPageAsync(accountId);
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Projects.ProjectsGetResponse?> GetHqProjectsPageAsync(
        Guid accountId,
        RequestConfiguration<Hq.V1.Accounts.Item.Projects.ProjectsRequestBuilder.ProjectsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Projects
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a new BIM 360 project in an account (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /hq/v1/accounts/{account_id}/projects
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/projects-POST
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="body">The project creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.ProjectsPostResponse"/> containing the created project</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.ProjectsPostResponse? response = await client.AccountAdminManager.CreateProjectAsync(accountId, new Hq.V1.Accounts.Item.Projects.ProjectsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Projects.ProjectsPostResponse?> CreateProjectAsync(
        Guid accountId,
        Hq.V1.Accounts.Item.Projects.ProjectsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Projects
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves detailed information about a project (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/projects/{project_id}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/projects-:project_id-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.Item.WithProject_GetResponse"/> containing the project details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.Item.WithProject_GetResponse? project = await client.AccountAdminManager.GetHqProjectAsync(accountId, projectId);
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Projects.Item.WithProject_GetResponse?> GetHqProjectAsync(
        Guid accountId,
        Guid projectId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Projects[projectId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a BIM 360 project (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /hq/v1/accounts/{account_id}/projects/{project_id}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/projects-:project_id-PATCH
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The project update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.Item.WithProject_PatchResponse"/> containing the updated project</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.Item.WithProject_PatchResponse? updated = await client.AccountAdminManager.UpdateHqProjectAsync(accountId, projectId, new Hq.V1.Accounts.Item.Projects.Item.WithProject_PatchRequestBody());
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Projects.Item.WithProject_PatchResponse?> UpdateHqProjectAsync(
        Guid accountId,
        Guid projectId,
        Hq.V1.Accounts.Item.Projects.Item.WithProject_PatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Projects[projectId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves detailed information about a project (Construction Admin API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/projects/{projectId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/admin-projectsprojectId-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.Admin.V1.Projects.Item.WithProjectGetResponse"/> containing the project details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.Admin.V1.Projects.Item.WithProjectGetResponse? project = await client.AccountAdminManager.GetProjectAsync(projectId);
    /// </code>
    /// </example>
    public async Task<Construction.Admin.V1.Projects.Item.WithProjectGetResponse?> GetProjectAsync(
        Guid projectId,
        RequestConfiguration<Construction.Admin.V1.Projects.Item.WithProjectItemRequestBuilder.WithProjectItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Admin.V1.Projects[projectId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates or updates a project image (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /hq/v1/accounts/{account_id}/projects/{project_id}/image
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/projects-:project_id-image-PATCH
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.Item.Image.ImagePatchResponse"/> containing the updated image</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.Item.Image.ImagePatchResponse? result = await client.AccountAdminManager.UpdateProjectImageAsync(accountId, projectId);
    /// </code>
    /// </example>
    public async Task<ImagePatchResponse?> UpdateProjectImageAsync(
        Guid accountId,
        Guid projectId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Projects[projectId]
            .Image
            .PatchAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a paginated list of companies in an account (Construction Admin API) with automatic page fetching.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/accounts/{accountId}/companies
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/companies-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Autodesk.BIM360.Construction.Admin.V1.Accounts.Item.Companies.CompaniesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (Autodesk.BIM360.Construction.Admin.V1.Accounts.Item.Companies.CompaniesGetResponse_results company in client.AccountAdminManager.ListCompaniesAsync(accountId))
    /// {
    ///     Console.WriteLine(company.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<CompaniesGetResponse_results> ListCompaniesAsync(
        Guid accountId,
        RequestConfiguration<CompaniesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            Construction.Admin.V1.Accounts.Item.Companies.CompaniesGetResponse? response = await _api.Construction.Admin.V1.Accounts[accountId]
                .Companies
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (CompaniesGetResponse_results item in response.Results)
            {
                yield return item;
            }

            int totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves one HTTP response for GET /hq/v1/accounts/{account_id}/companies. Use query parameters limit and offset for paging at the HTTP level.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/companies
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/companies-GET-legacy
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>The deserialized response body as modeled in the generated client</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.CompaniesGetResponse? page = await client.AccountAdminManager.GetHqCompaniesPageAsync(accountId);
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Companies.CompaniesGetResponse?> GetHqCompaniesPageAsync(
        Guid accountId,
        RequestConfiguration<Hq.V1.Accounts.Item.Companies.CompaniesRequestBuilder.CompaniesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Companies
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a new partner company (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /hq/v1/accounts/{account_id}/companies
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/companies-POST
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="body">The company creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.CompaniesPostResponse"/> containing the created company</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.CompaniesPostResponse? response = await client.AccountAdminManager.CreateCompanyAsync(accountId, new Hq.V1.Accounts.Item.Companies.CompaniesPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Companies.CompaniesPostResponse?> CreateCompanyAsync(
        Guid accountId,
        Hq.V1.Accounts.Item.Companies.CompaniesPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Companies
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Imports companies into an account (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /hq/v1/accounts/{account_id}/companies/import
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/companies-import-POST
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="body">The import data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.Import.ImportPostResponse"/> containing the import results</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.Import.ImportPostResponse? response = await client.AccountAdminManager.ImportCompaniesAsync(accountId, new Hq.V1.Accounts.Item.Companies.Import.ImportPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Companies.Import.ImportPostResponse?> ImportCompaniesAsync(
        Guid accountId,
        Hq.V1.Accounts.Item.Companies.Import.ImportPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Companies
            .Import
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a single company by ID (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/companies/{company_id}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/companies-:company_id-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="companyId">The ID of the company</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.Item.WithCompany_GetResponse"/> containing the company details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.Item.WithCompany_GetResponse? company = await client.AccountAdminManager.GetCompanyAsync(accountId, companyId);
    /// </code>
    /// </example>
    public async Task<WithCompany_GetResponse?> GetCompanyAsync(
        Guid accountId,
        Guid companyId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Companies[companyId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a company (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /hq/v1/accounts/{account_id}/companies/{company_id}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/companies-:company_id-PATCH
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="companyId">The ID of the company</param>
    /// <param name="body">The company update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.Item.WithCompany_PatchResponse"/> containing the updated company</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.Item.WithCompany_PatchResponse? updated = await client.AccountAdminManager.UpdateCompanyAsync(accountId, companyId, new WithCompany_PatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithCompany_PatchResponse?> UpdateCompanyAsync(
        Guid accountId,
        Guid companyId,
        WithCompany_PatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Companies[companyId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Searches for companies in an account (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/companies/search
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/companies-search-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filter parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.Search.SearchGetResponse"/> containing the search results</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.Search.SearchGetResponse? results = await client.AccountAdminManager.SearchCompaniesAsync(accountId);
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Companies.Search.SearchGetResponse?> SearchCompaniesAsync(
        Guid accountId,
        RequestConfiguration<Hq.V1.Accounts.Item.Companies.Search.SearchRequestBuilder.SearchRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Companies
            .Search
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves companies in a project (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/projects/{project_id}/companies
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/projects-:project_id-companies-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.Item.Companies.CompaniesGetResponse"/> containing the project companies</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.Item.Companies.CompaniesGetResponse? companies = await client.AccountAdminManager.GetProjectCompaniesAsync(accountId, projectId);
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Projects.Item.Companies.CompaniesGetResponse?> GetProjectCompaniesAsync(
        Guid accountId,
        Guid projectId,
        RequestConfiguration<Hq.V1.Accounts.Item.Projects.Item.Companies.CompaniesRequestBuilder.CompaniesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Projects[projectId]
            .Companies
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates or updates a company image (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /hq/v1/accounts/{account_id}/companies/{company_id}/image
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/companies-:company_id-image-PATCH
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="companyId">The ID of the company</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.Item.Image.ImagePatchResponse"/> containing the updated image</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Companies.Item.Image.ImagePatchResponse? result = await client.AccountAdminManager.UpdateCompanyImageAsync(accountId, companyId);
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Companies.Item.Image.ImagePatchResponse?> UpdateCompanyImageAsync(
        Guid accountId,
        Guid companyId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Companies[companyId]
            .Image
            .PatchAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a new user in an account (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /hq/v1/accounts/{account_id}/users
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/users-POST
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="body">The user creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Users.UsersPostResponse"/> containing the created user</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Users.UsersPostResponse? response = await client.AccountAdminManager.CreateUserAsync(accountId, new Hq.V1.Accounts.Item.Users.UsersPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Users.UsersPostResponse?> CreateUserAsync(
        Guid accountId,
        Hq.V1.Accounts.Item.Users.UsersPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Users
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves one HTTP response for GET /hq/v1/accounts/{account_id}/users. Use query parameters limit and offset for paging at the HTTP level.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/users
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/users-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>The deserialized response body as modeled in the generated client</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Users.UsersGetResponse? users = await client.AccountAdminManager.GetHqUsersPageAsync(accountId);
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Users.UsersGetResponse?> GetHqUsersPageAsync(
        Guid accountId,
        RequestConfiguration<Hq.V1.Accounts.Item.Users.UsersRequestBuilder.UsersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Users
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Imports users into an account (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /hq/v1/accounts/{account_id}/users/import
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/users-import-POST
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="body">The import data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Users.Import.ImportPostResponse"/> containing the import results</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Users.Import.ImportPostResponse? response = await client.AccountAdminManager.ImportUsersAsync(accountId, new Hq.V1.Accounts.Item.Users.Import.ImportPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Users.Import.ImportPostResponse?> ImportUsersAsync(
        Guid accountId,
        Hq.V1.Accounts.Item.Users.Import.ImportPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Users
            .Import
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a single user by ID (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/users/{user_id}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/users-:user_id-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="userId">The ID of the user</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Users.Item.WithUser_GetResponse"/> containing the user details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Users.Item.WithUser_GetResponse? user = await client.AccountAdminManager.GetUserAsync(accountId, userId);
    /// </code>
    /// </example>
    public async Task<WithUser_GetResponse?> GetUserAsync(
        Guid accountId,
        Guid userId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Users[userId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a user (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /hq/v1/accounts/{account_id}/users/{user_id}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/users-:user_id-PATCH
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="userId">The ID of the user</param>
    /// <param name="body">The user update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Users.Item.WithUser_PatchResponse"/> containing the updated user</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Users.Item.WithUser_PatchResponse? updated = await client.AccountAdminManager.UpdateUserAsync(accountId, userId, new WithUser_PatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithUser_PatchResponse?> UpdateUserAsync(
        Guid accountId,
        Guid userId,
        WithUser_PatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Users[userId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Searches for users in an account (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/users/search
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/users-search-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Users.Search.SearchGetResponse"/> containing the search results</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Users.Search.SearchGetResponse? results = await client.AccountAdminManager.SearchUsersAsync(accountId);
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Users.Search.SearchGetResponse?> SearchUsersAsync(
        Guid accountId,
        RequestConfiguration<Hq.V1.Accounts.Item.Users.Search.SearchRequestBuilder.SearchRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Users
            .Search
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a paginated list of project users with automatic page fetching (Construction Admin API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/projects/{projectId}/users
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/admin-projectsprojectId-users-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Autodesk.BIM360.Construction.Admin.V1.Projects.Item.Users.UsersGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (Autodesk.BIM360.Construction.Admin.V1.Projects.Item.Users.UsersGetResponse_results user in client.AccountAdminManager.ListConstructionProjectUsersAsync(projectId))
    /// {
    ///     Console.WriteLine(user.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Construction.Admin.V1.Projects.Item.Users.UsersGetResponse_results> ListConstructionProjectUsersAsync(
        Guid projectId,
        RequestConfiguration<Construction.Admin.V1.Projects.Item.Users.UsersRequestBuilder.UsersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            Construction.Admin.V1.Projects.Item.Users.UsersGetResponse? response = await _api.Construction.Admin.V1.Projects[projectId]
                .Users
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (Construction.Admin.V1.Projects.Item.Users.UsersGetResponse_results item in response.Results)
            {
                yield return item;
            }

            int totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves a paginated list of project users with automatic page fetching (BIM 360 Admin API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/admin/v1/projects/{projectId}/users
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/admin-v1-projects-projectId-users-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Autodesk.BIM360.Bim360.Admin.V1.Projects.Item.Users.UsersGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (Autodesk.BIM360.Bim360.Admin.V1.Projects.Item.Users.UsersGetResponse_results user in client.AccountAdminManager.ListBim360ProjectUsersAsync(projectId))
    /// {
    ///     Console.WriteLine(user.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<Bim360.Admin.V1.Projects.Item.Users.UsersGetResponse_results> ListBim360ProjectUsersAsync(
        Guid projectId,
        RequestConfiguration<Bim360.Admin.V1.Projects.Item.Users.UsersRequestBuilder.UsersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            Bim360.Admin.V1.Projects.Item.Users.UsersGetResponse? response = await _api.Bim360.Admin.V1.Projects[projectId]
                .Users
                .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                    r.QueryParameters.Offset = offset;
                }, cancellationToken);

            if (response?.Results is not { Count: > 0 })
                yield break;

            foreach (Bim360.Admin.V1.Projects.Item.Users.UsersGetResponse_results item in response.Results)
            {
                yield return item;
            }

            int totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Retrieves detailed information about a user in a project (Construction Admin API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/projects/{projectId}/users/{userId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/admin-projectsprojectId-users-userId-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="userId">The user ID (BIM 360 ID or Autodesk ID)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Construction.Admin.V1.Projects.Item.Users.Item.WithUserGetResponse"/> containing the project user details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Construction.Admin.V1.Projects.Item.Users.Item.WithUserGetResponse? user = await client.AccountAdminManager.GetConstructionProjectUserAsync(projectId, userId);
    /// </code>
    /// </example>
    public async Task<Construction.Admin.V1.Projects.Item.Users.Item.WithUserGetResponse?> GetConstructionProjectUserAsync(
        Guid projectId,
        string userId,
        RequestConfiguration<WithUserItemRequestBuilder.WithUserItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Admin.V1.Projects[projectId]
            .Users[userId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves detailed information about a user in a project (BIM 360 Admin API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /bim360/admin/v1/projects/{projectId}/users/{userId}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/admin-v1-projects-projectId-users-userId-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="userId">The user ID (BIM 360 ID or Autodesk ID)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Bim360.Admin.V1.Projects.Item.Users.Item.WithUserGetResponse"/> containing the project user details</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Bim360.Admin.V1.Projects.Item.Users.Item.WithUserGetResponse? user = await client.AccountAdminManager.GetBim360ProjectUserAsync(projectId, userId);
    /// </code>
    /// </example>
    public async Task<Bim360.Admin.V1.Projects.Item.Users.Item.WithUserGetResponse?> GetBim360ProjectUserAsync(
        Guid projectId,
        string userId,
        RequestConfiguration<Bim360.Admin.V1.Projects.Item.Users.Item.WithUserItemRequestBuilder.WithUserItemRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Bim360.Admin.V1.Projects[projectId]
            .Users[userId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Adds a project admin to a project (HQ v1 API).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /hq/v1/accounts/{account_id}/projects/{project_id}/users
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/projects-project_id-users-POST
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The request body</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.Item.Users.UsersPostResponse"/> containing the result</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Projects.Item.Users.UsersPostResponse? response = await client.AccountAdminManager.AddHqProjectUserAsync(accountId, projectId, new Hq.V1.Accounts.Item.Projects.Item.Users.UsersPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Projects.Item.Users.UsersPostResponse?> AddHqProjectUserAsync(
        Guid accountId,
        Guid projectId,
        Hq.V1.Accounts.Item.Projects.Item.Users.UsersPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Projects[projectId]
            .Users
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Imports users into a project (HQ v2 API).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /hq/v2/accounts/{account_id}/projects/{project_id}/users/import
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/projects-project_id-users-import-POST
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The import data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V2.Accounts.Item.Projects.Item.Users.Import.ImportPostResponse"/> containing the import results</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V2.Accounts.Item.Projects.Item.Users.Import.ImportPostResponse? response = await client.AccountAdminManager.ImportProjectUsersAsync(accountId, projectId, new Hq.V2.Accounts.Item.Projects.Item.Users.Import.ImportPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Hq.V2.Accounts.Item.Projects.Item.Users.Import.ImportPostResponse?> ImportProjectUsersAsync(
        Guid accountId,
        Guid projectId,
        Hq.V2.Accounts.Item.Projects.Item.Users.Import.ImportPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V2.Accounts[accountId.ToString()]
            .Projects[projectId.ToString()]
            .Users
            .Import
            .PostAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a user in a project (HQ v2 API).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /hq/v2/accounts/{account_id}/projects/{project_id}/users/{user_id}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/projects-project_id-users-user_id-PATCH
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="userId">The user&apos;s BIM 360 user ID</param>
    /// <param name="body">The user update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V2.Accounts.Item.Projects.Item.Users.Item.WithUser_PatchResponse"/> containing the updated project user</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V2.Accounts.Item.Projects.Item.Users.Item.WithUser_PatchResponse? updated = await client.AccountAdminManager.UpdateHqV2ProjectUserAsync(accountId, projectId, userId, new Hq.V2.Accounts.Item.Projects.Item.Users.Item.WithUser_PatchRequestBody());
    /// </code>
    /// </example>
    public async Task<Hq.V2.Accounts.Item.Projects.Item.Users.Item.WithUser_PatchResponse?> UpdateHqV2ProjectUserAsync(
        Guid accountId,
        Guid projectId,
        string userId,
        Hq.V2.Accounts.Item.Projects.Item.Users.Item.WithUser_PatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V2.Accounts[accountId.ToString()]
            .Projects[projectId.ToString()]
            .Users[userId]
            .PatchAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves industry roles for a project (HQ v2 API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v2/accounts/{account_id}/projects/{project_id}/industry_roles
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/projects-project_id-industry_roles-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V2.Accounts.Item.Projects.Item.Industry_roles.Industry_rolesGetResponse"/> containing industry roles</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V2.Accounts.Item.Projects.Item.Industry_roles.Industry_rolesGetResponse? roles = await client.AccountAdminManager.GetIndustryRolesAsync(accountId, projectId);
    /// </code>
    /// </example>
    public async Task<Hq.V2.Accounts.Item.Projects.Item.Industry_roles.Industry_rolesGetResponse?> GetIndustryRolesAsync(
        Guid accountId,
        Guid projectId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V2.Accounts[accountId.ToString()]
            .Projects[projectId.ToString()]
            .Industry_roles
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the business units structure for an account (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/business_units_structure
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/business_units_structure-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Business_units_structure.Business_units_structureGetResponse"/> containing the business units structure</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Business_units_structure.Business_units_structureGetResponse? structure = await client.AccountAdminManager.GetBusinessUnitsStructureAsync(accountId);
    /// </code>
    /// </example>
    public async Task<Business_units_structureGetResponse?> GetBusinessUnitsStructureAsync(
        Guid accountId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Business_units_structure
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Creates or redefines the business units structure for an account (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /hq/v1/accounts/{account_id}/business_units_structure
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/business_units_structure-PUT
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="body">The business units structure data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Business_units_structure.Business_units_structurePutResponse"/> containing the updated business units structure</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Business_units_structure.Business_units_structurePutResponse? result = await client.AccountAdminManager.UpdateBusinessUnitsStructureAsync(accountId, new Business_units_structurePutRequestBody());
    /// </code>
    /// </example>
    public async Task<Business_units_structurePutResponse?> UpdateBusinessUnitsStructureAsync(
        Guid accountId,
        Business_units_structurePutRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Business_units_structure
            .PutAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Returns the status of an asynchronous job (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/jobs/{job_id}
    /// APS docs: https://aps.autodesk.com/en/docs/bim360/v1/reference/http/jobs-:job_id-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="jobId">The job ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.BIM360.Hq.V1.Accounts.Item.Jobs.Item.WithJob_GetResponse"/> containing the job status</returns>
    /// <example>
    /// <code>
    /// Autodesk.BIM360.Hq.V1.Accounts.Item.Jobs.Item.WithJob_GetResponse? job = await client.AccountAdminManager.GetJobAsync(accountId, jobId);
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Jobs.Item.WithJob_GetResponse?> GetJobAsync(
        Guid accountId,
        Guid jobId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Hq.V1.Accounts[accountId]
            .Jobs[jobId]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
