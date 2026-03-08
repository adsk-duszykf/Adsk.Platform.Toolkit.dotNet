namespace Autodesk.Automation;

/// <summary>
/// Client for Autodesk Automation API. Provides access to all API endpoints and operations.
/// </summary>
public class AutomationClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutomationClient"/> class.
    /// </summary>
    /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public AutomationClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
    {
        var adapter = Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);

        Api = new BaseAutomationClient(adapter);
    }

    /// <summary>
    /// Design Automation API client base path 'https://developer.api.autodesk.com/da/us-east/v3/'
    /// </summary>
    public BaseAutomationClient Api { get; protected set; }

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/da/us-east/v3/activities/*
    /// </summary>
    public Da.UsEast.V3.Activities.ActivitiesRequestBuilder Activities => Api.Da.UsEast.V3.Activities;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/da/us-east/v3/appbundles/*
    /// </summary>
    public Da.UsEast.V3.Appbundles.AppbundlesRequestBuilder AppBundles => Api.Da.UsEast.V3.Appbundles;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/da/us-east/v3/engines/*
    /// </summary>
    public Da.UsEast.V3.Engines.EnginesRequestBuilder Engines => Api.Da.UsEast.V3.Engines;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/da/us-east/v3/forgeapps/*
    /// </summary>
    public Da.UsEast.V3.Forgeapps.ForgeappsRequestBuilder ForgeApps => Api.Da.UsEast.V3.Forgeapps;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/da/us-east/v3/health/*
    /// </summary>
    public Da.UsEast.V3.Health.HealthRequestBuilder Health => Api.Da.UsEast.V3.Health;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/da/us-east/v3/servicelimits/*
    /// </summary>
    public Da.UsEast.V3.Servicelimits.ServicelimitsRequestBuilder ServiceLimits => Api.Da.UsEast.V3.Servicelimits;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/da/us-east/v3/shares/*
    /// </summary>
    public Da.UsEast.V3.Shares.SharesRequestBuilder Shares => Api.Da.UsEast.V3.Shares;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/da/us-east/v3/workitems/*
    /// </summary>
    public Da.UsEast.V3.Workitems.WorkitemsRequestBuilder WorkItems => Api.Da.UsEast.V3.Workitems;
}
