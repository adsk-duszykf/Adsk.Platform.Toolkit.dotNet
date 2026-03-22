using Autodesk.Automation.Da.UsEast.V3.Servicelimits.Item;
using Microsoft.Kiota.Abstractions;

namespace Autodesk.Automation.Managers;

/// <summary>
/// Manager for Service Limits operations — manages Design Automation service limit configurations.
/// </summary>
public class ServiceLimitsManager
{
    private readonly BaseAutomationClient _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLimitsManager"/> class.
    /// </summary>
    /// <param name="api">The API client</param>
    public ServiceLimitsManager(BaseAutomationClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Gets a user's service limit configuration.
    /// </summary>
    /// <remarks>
    /// Wraps: GET /da/us-east/v3/servicelimits/{owner}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/servicelimits-owner-GET
    /// </remarks>
    /// <param name="owner">The user to fetch the service limit configuration for (should be "me")</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithOwnerGetResponse"/> containing the service limits configuration</returns>
    /// <example>
    /// <code>
    /// WithOwnerGetResponse? limits = await client.ServiceLimitsManager.GetServiceLimitsAsync("me");
    /// </code>
    /// </example>
    public async Task<WithOwnerGetResponse?> GetServiceLimitsAsync(
        string owner = "me",
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Servicelimits[owner]
            .GetAsync(r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }

    /// <summary>
    /// Updates a user's service limit configuration.
    /// </summary>
    /// <remarks>
    /// Wraps: PUT /da/us-east/v3/servicelimits/{owner}
    /// APS docs: https://aps.autodesk.com/en/docs/design-automation/v3/reference/http/servicelimits-owner-PUT
    /// </remarks>
    /// <param name="body">The service limits update data</param>
    /// <param name="owner">The user to associate the configuration to (should be "me")</param>
    /// <param name="requestConfiguration">(Optional) Configuration for the request</param>
    /// <param name="cancellationToken">(Optional) Cancellation token</param>
    /// <returns>A <see cref="WithOwnerPutResponse"/> containing the updated service limits</returns>
    /// <example>
    /// <code>
    /// WithOwnerPutResponse? limits = await client.ServiceLimitsManager.UpdateServiceLimitsAsync(new WithOwnerPutRequestBody
    /// {
    ///     FrontendLimits = new WithOwnerPutRequestBody_frontendLimits { LimitMonthlyProcessingTimeInHours = 10 }
    /// });
    /// </code>
    /// </example>
    public async Task<WithOwnerPutResponse?> UpdateServiceLimitsAsync(
        WithOwnerPutRequestBody body,
        string owner = "me",
        RequestConfiguration<DefaultQueryParameters>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        return await _api.Da.UsEast.V3.Servicelimits[owner]
            .PutAsync(body, r =>
            {
                r.Headers = requestConfiguration?.Headers ?? r.Headers;
                r.Options = requestConfiguration?.Options ?? r.Options;
            }, cancellationToken);
    }
}
