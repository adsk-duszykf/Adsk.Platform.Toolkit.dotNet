using ACCapi=Autodesk.ACC.AccountAdmin.ACC;
using BIMapi=Autodesk.ACC.AccountAdmin.BIM;
using Autodesk.ACC.AccountAdmin.Managers;
using Autodesk.ACC.AccountAdmin.ACC.Construction;
using Autodesk.ACC.AccountAdmin.BIM.Hq;

namespace Autodesk.ACC.AccountAdmin;

/// <summary>
/// Main entry point for Autodesk Account Admin SDK
/// </summary>
public class AccountAdminClient
{
    /// <summary>
    /// All ACC and BIM360 API request builders
    /// </summary>
    public ApiRequestBuilder Api { get; }

    /// <summary>
    /// Manager for ACC Projects operations
    /// </summary>
    public ProjectsManager Projects { get; }

    /// <summary>
    /// Manager for ACC Project Users operations
    /// </summary>
    public ProjectUsersManager ProjectUsers { get; }

    /// <summary>
    /// Manager for BIM360 Companies operations
    /// </summary>
    public CompaniesManager Companies { get; }

    /// <summary>
    /// Manager for BIM360 Account Users operations
    /// </summary>
    public AccountUsersManager AccountUsers { get; }

    /// <summary>
    /// Manager for BIM360 Business Units operations
    /// </summary>
    public BusinessUnitsManager BusinessUnits { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountAdminClient"/> class.
    /// </summary>
    /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public AccountAdminClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
    {
        var adapter = Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);

        // Create base clients for ACC and BIM360
        var accBaseClient = new ACCapi.BaseAccountAdminClient(adapter);
        var bimBaseClient = new BIMapi.BaseAccountAdminClient(adapter);

        Api = new ApiRequestBuilder
        {
            Construction = accBaseClient.Construction,
            Hq = bimBaseClient.Hq
        };

        // Initialize managers
        Projects = new ProjectsManager(Api);
        ProjectUsers = new ProjectUsersManager(Api);
        Companies = new CompaniesManager(Api);
        AccountUsers = new AccountUsersManager(Api);
        BusinessUnits = new BusinessUnitsManager(Api);
    }
}

/// <summary>
/// Container for all API request builders
/// </summary>
public class ApiRequestBuilder
{
    /// <summary>
    /// ACC Construction API request builder
    /// </summary>
    public ConstructionRequestBuilder? Construction { get; init; }

    /// <summary>
    /// BIM360 HQ API request builder
    /// </summary>
    public HqRequestBuilder? Hq { get; init; }
}

