
using Autodesk.ACC.AccountAdmin.BIM.Hq.V1.Accounts.Item.Business_units_structure;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.ACC.AccountAdmin.Managers;

/// <summary>
/// Manager for Business Units operations (BIM360)
/// </summary>
public class BusinessUnitsManager
{
    private readonly ApiRequestBuilder _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessUnitsManager"/> class.
    /// </summary>
    /// <param name="api">The API request builder</param>
    public BusinessUnitsManager(ApiRequestBuilder api)
    {
        _api = api;
    }

    /// <summary>
    /// Query all the business units in a specific BIM 360 account.
    /// </summary>
    /// <param name="accountId">The account ID of the business unit</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Business units structure</returns>
    public async Task<Business_units_structureGetResponse?> GetBusinessUnitsAsync(
        string accountId,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Hq!.V1.Accounts[accountId]
            .Business_units_structure
            .GetAsBusiness_units_structureGetResponseAsync(requestConfiguration, cancellationToken);

        return result;
    }

    /// <summary>
    /// Creates or redefines the business units of a specific BIM 360 account.
    /// </summary>
    /// <param name="accountId">The account ID of the business unit</param>
    /// <param name="businessUnitsData">The business units structure data</param>
    /// <param name="requestConfiguration">Optional configuration for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated business units structure</returns>
    public async Task<Business_units_structurePutResponse?> UpdateBusinessUnitsAsync(
        string accountId,
        Business_units_structurePutRequestBody businessUnitsData,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _api.Hq!.V1.Accounts[accountId]
            .Business_units_structure
            .PutAsBusiness_units_structurePutResponseAsync(businessUnitsData, requestConfiguration, cancellationToken);

        return result;
    }
}

