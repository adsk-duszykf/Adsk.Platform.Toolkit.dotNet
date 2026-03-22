using Autodesk.Automation.Managers;

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

        ActivitiesManager = new ActivitiesManager(Api);
        AppBundlesManager = new AppBundlesManager(Api);
        EnginesManager = new EnginesManager(Api);
        ForgeAppsManager = new ForgeAppsManager(Api);
        HealthManager = new HealthManager(Api);
        ServiceLimitsManager = new ServiceLimitsManager(Api);
        SharesManager = new SharesManager(Api);
        WorkItemsManager = new WorkItemsManager(Api);
    }

    /// <summary>
    /// Design Automation API client base path 'https://developer.api.autodesk.com/da/us-east/v3/'
    /// </summary>
    public BaseAutomationClient Api { get; protected set; }

    /// <summary>
    /// Manager for Activity operations — manages activities, aliases, and versions.
    /// </summary>
    public ActivitiesManager ActivitiesManager { get; }

    /// <summary>
    /// Manager for AppBundle operations — manages app bundles, aliases, and versions.
    /// </summary>
    public AppBundlesManager AppBundlesManager { get; }

    /// <summary>
    /// Manager for Engine operations — lists and retrieves engine details.
    /// </summary>
    public EnginesManager EnginesManager { get; }

    /// <summary>
    /// Manager for ForgeApps (nickname) operations — manages app nicknames.
    /// </summary>
    public ForgeAppsManager ForgeAppsManager { get; }

    /// <summary>
    /// Manager for Health operations — checks engine health status.
    /// </summary>
    public HealthManager HealthManager { get; }

    /// <summary>
    /// Manager for Service Limits operations — manages service limit configurations.
    /// </summary>
    public ServiceLimitsManager ServiceLimitsManager { get; }

    /// <summary>
    /// Manager for Shares operations — lists shared AppBundles and Activities.
    /// </summary>
    public SharesManager SharesManager { get; }

    /// <summary>
    /// Manager for WorkItem operations — creates and monitors work items.
    /// </summary>
    public WorkItemsManager WorkItemsManager { get; }
}
