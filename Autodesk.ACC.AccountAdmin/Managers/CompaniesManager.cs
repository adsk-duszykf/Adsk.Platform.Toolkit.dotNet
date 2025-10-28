
using Autodesk.ACC.AccountAdmin.Hq.V1.Accounts.Item.Companies;
using Autodesk.ACC.AccountAdmin.Hq.V1.Accounts.Item.Companies.Import;
using static Autodesk.ACC.AccountAdmin.Hq.V1.Accounts.Item.Companies.CompaniesRequestBuilder;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.ACC.AccountAdmin.Managers;

/// <summary>
/// Manager for Companies operations (BIM360)
/// </summary>
public class CompaniesManager
{
    private readonly ApiRequestBuilder _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompaniesManager"/> class.
    /// </summary>
    /// <param name="api">The API request builder</param>
    public CompaniesManager(ApiRequestBuilder api)
    {
        _api = api;
    }

    /// <summary>
    /// Query all the partner companies in a specific BIM 360 account.
    /// </summary>
    /// <param name="accountId">The account ID of the company</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Company data</returns>
    public async Task<CompaniesGetResponse?> ListCompaniesAsync(
        string accountId,
        Action<RequestConfiguration<CompaniesRequestBuilderGetQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Hq!.V1.Accounts[accountId]
            .Companies
            .GetAsCompaniesGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Create a new partner company.
    /// </summary>
    /// <param name="accountId">The account ID</param>
    /// <param name="companyData">The company creation data</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created company information</returns>
    public async Task<CompaniesPostResponse?> CreateCompanyAsync(
        string accountId,
        CompaniesPostRequestBody companyData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Hq!.V1.Accounts[accountId]
            .Companies
            .PostAsCompaniesPostResponseAsync(companyData, requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Bulk import partner companies to the company directory in a specific BIM 360 account.
    /// Maximum 50 companies per call.
    /// </summary>
    /// <param name="accountId">The account ID</param>
    /// <param name="companies">Array of companies to import (max 50)</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Import result with success and failure counts</returns>
    public async Task<ImportPostResponse?> ImportCompaniesAsync(
        string accountId,
        ImportPostRequestBody companies,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Hq!.V1.Accounts[accountId]
            .Companies
            .Import
            .PostAsImportPostResponseAsync(companies, requestConfiguration, cancellationToken);

        return result;
    }
}

