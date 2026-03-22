using System.Runtime.CompilerServices;
using Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Companies;
using Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Projects;
using Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Users.Item.Products;
using Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Users.Item.Roles;
using Autodesk.ACC.Construction.Admin.V1.Projects.Item.Users;
using Autodesk.ACC.Construction.Admin.V1.Projects.Item.Users.Item;
using Autodesk.ACC.Hq.V1.Accounts.Item.Business_units_structure;
using Autodesk.ACC.Hq.V1.Accounts.Item.Companies.Item;
using Autodesk.ACC.Hq.V1.Accounts.Item.Projects.Item.Image;
using Autodesk.ACC.Hq.V1.Accounts.Item.Users.Item;
using Microsoft.Kiota.Abstractions;
using static Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Projects.ProjectsRequestBuilder;
using static Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Companies.CompaniesRequestBuilder;
using static Autodesk.ACC.Construction.Admin.V1.Projects.Item.Users.UsersRequestBuilder;

namespace Autodesk.ACC.Managers;

/// <summary>
/// Manager for Account Admin operations — manages accounts, projects, users, companies, and business units.
/// </summary>
public class AccountAdminManager
{
    private readonly BaseACCclient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountAdminManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public AccountAdminManager(BaseACCclient api)
    {
        _api = api;
    }

    /// <summary>
    /// Retrieves a paginated list of projects in an account with automatic page fetching.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/accounts/{accountId}/projects
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/admin-accounts-accountidprojects-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account (hub ID without the b. prefix)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Projects.ProjectsGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var project in client.AccountAdminManager.ListProjectsAsync(accountId))
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
            var response = await _api.Construction.Admin.V1.Accounts[accountId]
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

            foreach (var item in response.Results)
            {
                yield return item;
            }

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Creates a new project in an account.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/admin/v1/accounts/{accountId}/projects
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/admin-accounts-accountidprojects-POST
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="body">The project creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Projects.ProjectsPostResponse"/> containing the created project</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Projects.ProjectsPostResponse? response = await client.AccountAdminManager.CreateProjectAsync(accountId, new ProjectsPostRequestBody());
    /// </code>
    /// </example>
    public async Task<ProjectsPostResponse?> CreateProjectAsync(
        Guid accountId,
        ProjectsPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Admin.V1.Accounts[accountId]
            .Projects
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves detailed information about a project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/projects/{projectId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/admin-projectsprojectId-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Admin.V1.Projects.Item.WithProjectGetResponse"/> containing the project details</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Admin.V1.Projects.Item.WithProjectGetResponse? project = await client.AccountAdminManager.GetProjectAsync(projectId);
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
    /// Updates a project image (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /hq/v1/accounts/{account_id}/projects/{project_id}/image
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/projects-:project_id-image-PATCH
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Projects.Item.Image.ImagePatchResponse"/> containing the updated image</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Projects.Item.Image.ImagePatchResponse? result = await client.AccountAdminManager.UpdateProjectImageAsync(accountId, projectId);
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
    /// Retrieves a paginated list of companies in an account (Construction Admin).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/accounts/{accountId}/companies
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/companies-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Companies.CompaniesGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var company in client.AccountAdminManager.ListCompaniesAsync(accountId))
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
            var response = await _api.Construction.Admin.V1.Accounts[accountId]
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

            foreach (var item in response.Results)
            {
                yield return item;
            }

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Creates a new partner company (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /hq/v1/accounts/{account_id}/companies
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/companies-POST
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="body">The company creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Companies.CompaniesPostResponse"/> containing the created company</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Companies.CompaniesPostResponse? response = await client.AccountAdminManager.CreateCompanyAsync(accountId, new CompaniesPostRequestBody());
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
    /// Imports companies into an account.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /hq/v1/accounts/{account_id}/companies/import
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/companies-import-POST
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="body">The import data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Companies.Import.ImportPostResponse"/> containing the import results</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Companies.Import.ImportPostResponse? response = await client.AccountAdminManager.ImportCompaniesAsync(accountId, new ImportPostRequestBody());
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
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/companies-:company_id-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="companyId">The ID of the company</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Companies.Item.WithCompany_GetResponse"/> containing the company details</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Companies.Item.WithCompany_GetResponse? company = await client.AccountAdminManager.GetCompanyAsync(accountId, companyId);
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
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/companies-:company_id-PATCH
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="companyId">The ID of the company</param>
    /// <param name="body">The company update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Companies.Item.WithCompany_PatchResponse"/> containing the updated company</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Companies.Item.WithCompany_PatchResponse? updated = await client.AccountAdminManager.UpdateCompanyAsync(accountId, companyId, new WithCompany_PatchRequestBody());
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
    /// Searches for companies in an account.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/companies/search
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/companies-search-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request (supports filter parameters)</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Companies.Search.SearchGetResponse"/> containing the search results</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Companies.Search.SearchGetResponse? results = await client.AccountAdminManager.SearchCompaniesAsync(accountId);
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
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/projects-:project_id-companies-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Projects.Item.Companies.CompaniesGetResponse"/> containing the project companies</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Projects.Item.Companies.CompaniesGetResponse? companies = await client.AccountAdminManager.GetProjectCompaniesAsync(accountId, projectId);
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
    /// Updates a company image (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /hq/v1/accounts/{account_id}/companies/{company_id}/image
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/companies-:company_id-image-PATCH
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="companyId">The ID of the company</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Companies.Item.Image.ImagePatchResponse"/> containing the updated image</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Companies.Item.Image.ImagePatchResponse? result = await client.AccountAdminManager.UpdateCompanyImageAsync(accountId, companyId);
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
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/users-POST
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="body">The user creation data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Users.UsersPostResponse"/> containing the created user</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Users.UsersPostResponse? response = await client.AccountAdminManager.CreateUserAsync(accountId, new UsersPostRequestBody());
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
    /// Retrieves a page of users in an account (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/users
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/users-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Users.UsersGetResponse"/> containing a single page of users</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Users.UsersGetResponse? users = await client.AccountAdminManager.GetUsersAsync(accountId);
    /// </code>
    /// </example>
    public async Task<Hq.V1.Accounts.Item.Users.UsersGetResponse?> GetUsersAsync(
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
    /// Imports users into an account.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /hq/v1/accounts/{account_id}/users/import
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/users-import-POST
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="body">The import data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Users.Import.ImportPostResponse"/> containing the import results</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Users.Import.ImportPostResponse? response = await client.AccountAdminManager.ImportUsersAsync(accountId, new Users.Import.ImportPostRequestBody());
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
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/users-:user_id-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="userId">The ID of the user</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Users.Item.WithUser_GetResponse"/> containing the user details</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Users.Item.WithUser_GetResponse? user = await client.AccountAdminManager.GetUserAsync(accountId, userId);
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
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/users-:user_id-PATCH
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="userId">The ID of the user</param>
    /// <param name="body">The user update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Users.Item.WithUser_PatchResponse"/> containing the updated user</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Users.Item.WithUser_PatchResponse? updated = await client.AccountAdminManager.UpdateUserAsync(accountId, userId, new WithUser_PatchRequestBody());
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
    /// Retrieves projects assigned to a user.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/accounts/{accountId}/users/{userId}/projects
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/admin-usersuseridprojects-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="userId">The user ID (ACC ID or Autodesk ID)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Users.Item.Projects.ProjectsGetResponse"/> containing the user's projects</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Users.Item.Projects.ProjectsGetResponse? projects = await client.AccountAdminManager.GetUserProjectsAsync(accountId, userId);
    /// </code>
    /// </example>
    public async Task<Construction.Admin.V1.Accounts.Item.Users.Item.Projects.ProjectsGetResponse?> GetUserProjectsAsync(
        Guid accountId,
        string userId,
        RequestConfiguration<Construction.Admin.V1.Accounts.Item.Users.Item.Projects.ProjectsRequestBuilder.ProjectsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Admin.V1.Accounts[accountId]
            .Users[userId]
            .Projects
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves products assigned to a user.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/accounts/{accountId}/users/{userId}/products
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/admin-usersuseridproducts-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="userId">The user ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Users.Item.Products.ProductsGetResponse"/> containing the user's products</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Users.Item.Products.ProductsGetResponse? products = await client.AccountAdminManager.GetUserProductsAsync(accountId, userId);
    /// </code>
    /// </example>
    public async Task<ProductsGetResponse?> GetUserProductsAsync(
        Guid accountId,
        string userId,
        RequestConfiguration<ProductsRequestBuilder.ProductsRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Admin.V1.Accounts[accountId]
            .Users[userId]
            .Products
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves roles assigned to a user.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/accounts/{accountId}/users/{userId}/roles
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/admin-usersuseridroles-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="userId">The user ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Users.Item.Roles.RolesGetResponse"/> containing the user's roles</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Admin.V1.Accounts.Item.Users.Item.Roles.RolesGetResponse? roles = await client.AccountAdminManager.GetUserRolesAsync(accountId, userId);
    /// </code>
    /// </example>
    public async Task<RolesGetResponse?> GetUserRolesAsync(
        Guid accountId,
        string userId,
        RequestConfiguration<RolesRequestBuilder.RolesRequestBuilderGetQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Admin.V1.Accounts[accountId]
            .Users[userId]
            .Roles
            .GetAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.QueryParameters = requestConfiguration?.QueryParameters ?? r.QueryParameters;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Searches for users in an account (HQ API).
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/users/search
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/users-search-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Users.Search.SearchGetResponse"/> containing the search results</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Users.Search.SearchGetResponse? results = await client.AccountAdminManager.SearchUsersAsync(accountId);
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
    /// Retrieves a paginated list of project users with automatic page fetching.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/projects/{projectId}/users
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/admin-projectsprojectId-users-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Autodesk.ACC.Construction.Admin.V1.Projects.Item.Users.UsersGetResponse_results"/> items, automatically paginated</returns>
    /// <example>
    /// <code>
    /// await foreach (var user in client.AccountAdminManager.ListProjectUsersAsync(projectId))
    /// {
    ///     Console.WriteLine(user.Name);
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<UsersGetResponse_results> ListProjectUsersAsync(
        Guid projectId,
        RequestConfiguration<UsersRequestBuilderGetQueryParameters>? requestConfiguration = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int offset = requestConfiguration?.QueryParameters?.Offset ?? 0;

        while (true)
        {
            var response = await _api.Construction.Admin.V1.Projects[projectId]
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

            foreach (var item in response.Results)
            {
                yield return item;
            }

            var totalResults = response.Pagination?.TotalResults ?? 0;
            if (totalResults > 0 && offset + response.Results.Count >= totalResults)
                yield break;

            offset += response.Results.Count;
        }
    }

    /// <summary>
    /// Assigns a user to a project.
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/admin/v1/projects/{projectId}/users
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/admin-projects-project-Id-users-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The user assignment data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Admin.V1.Projects.Item.Users.UsersPostResponse"/> containing the created project user</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Admin.V1.Projects.Item.Users.UsersPostResponse? user = await client.AccountAdminManager.AddProjectUserAsync(projectId, new UsersPostRequestBody());
    /// </code>
    /// </example>
    public async Task<UsersPostResponse?> AddProjectUserAsync(
        Guid projectId,
        UsersPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Admin.V1.Projects[projectId]
            .Users
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Imports multiple users into a project (up to 200 per request).
    /// </summary>
    /// <remarks>
    /// Wraps: POST /construction/admin/v2/projects/{projectId}/users:import
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/admin-v2-projects-project-Id-users-import-POST
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="body">The import data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Admin.V2.Projects.Item.UsersImport.UsersImportPostResponse"/> containing the import results</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Admin.V2.Projects.Item.UsersImport.UsersImportPostResponse? response = await client.AccountAdminManager.ImportProjectUsersAsync(projectId, new UsersImportPostRequestBody());
    /// </code>
    /// </example>
    public async Task<Construction.Admin.V2.Projects.Item.UsersImport.UsersImportPostResponse?> ImportProjectUsersAsync(
        Guid projectId,
        Construction.Admin.V2.Projects.Item.UsersImport.UsersImportPostRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Admin.V2.Projects[projectId]
            .UsersImport
            .PostAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves detailed information about a user in a project.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /construction/admin/v1/projects/{projectId}/users/{userId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/admin-projectsprojectId-users-userId-GET
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="userId">The user ID (ACC ID or Autodesk ID)</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Admin.V1.Projects.Item.Users.Item.WithUserGetResponse"/> containing the project user details</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Admin.V1.Projects.Item.Users.Item.WithUserGetResponse? user = await client.AccountAdminManager.GetProjectUserAsync(projectId, userId);
    /// </code>
    /// </example>
    public async Task<WithUserGetResponse?> GetProjectUserAsync(
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
    /// Updates a user in a project.
    /// </summary>
    /// <remarks>
    /// Wraps: PATCH /construction/admin/v1/projects/{projectId}/users/{userId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/admin-projects-project-Id-users-userId-PATCH
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="userId">The user ID</param>
    /// <param name="body">The user update data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Construction.Admin.V1.Projects.Item.Users.Item.WithUserPatchResponse"/> containing the updated project user</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Construction.Admin.V1.Projects.Item.Users.Item.WithUserPatchResponse? updated = await client.AccountAdminManager.UpdateProjectUserAsync(projectId, userId, new WithUserPatchRequestBody());
    /// </code>
    /// </example>
    public async Task<WithUserPatchResponse?> UpdateProjectUserAsync(
        Guid projectId,
        string userId,
        WithUserPatchRequestBody body,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Construction.Admin.V1.Projects[projectId]
            .Users[userId]
            .PatchAsync(body, r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Removes a user from a project.
    /// </summary>
    /// <remarks>
    /// Wraps: DELETE /construction/admin/v1/projects/{projectId}/users/{userId}
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/admin-projects-project-Id-users-userId-DELETE
    /// </remarks>
    /// <param name="projectId">The ID of the project</param>
    /// <param name="userId">The user ID</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
    /// <example>
    /// <code>
    /// await client.AccountAdminManager.RemoveProjectUserAsync(projectId, userId);
    /// </code>
    /// </example>
    public async Task RemoveProjectUserAsync(
        Guid projectId,
        string userId,
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        await _api.Construction.Admin.V1.Projects[projectId]
            .Users[userId]
            .DeleteAsync(r =>
                {
                    r.Headers = requestConfiguration?.Headers ?? r.Headers;
                    r.Options = requestConfiguration?.Options ?? r.Options;
                }, cancellationToken);
    }

    /// <summary>
    /// Retrieves the business units structure for an account.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /hq/v1/accounts/{account_id}/business_units_structure
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/business_units_structure-GET
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Business_units_structure.Business_units_structureGetResponse"/> containing the business units structure</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Business_units_structure.Business_units_structureGetResponse? structure = await client.AccountAdminManager.GetBusinessUnitsStructureAsync(accountId);
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
    /// Creates or redefines the business units structure for an account.
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /hq/v1/accounts/{account_id}/business_units_structure
    /// APS docs: https://aps.autodesk.com/en/docs/acc/v1/reference/http/business_units_structure-PUT
    /// </remarks>
    /// <param name="accountId">The ID of the account</param>
    /// <param name="body">The business units structure data</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="Autodesk.ACC.Hq.V1.Accounts.Item.Business_units_structure.Business_units_structurePutResponse"/> containing the updated business units structure</returns>
    /// <example>
    /// <code>
    /// Autodesk.ACC.Hq.V1.Accounts.Item.Business_units_structure.Business_units_structurePutResponse? result = await client.AccountAdminManager.UpdateBusinessUnitsStructureAsync(accountId, new Business_units_structurePutRequestBody());
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
}
