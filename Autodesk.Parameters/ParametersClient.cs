using Autodesk.Parameters.Managers;

namespace Autodesk.Parameters;

/// <summary>
/// Client for the Autodesk Parameters API. Provides access to all API endpoints and operations.
/// </summary>
public class ParametersClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParametersClient"/> class.
    /// </summary>
    /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public ParametersClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
    {
        var adapter = Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);

        Api = new BaseParametersClient(adapter);

        GroupsManager = new GroupsManager(Api);
        CollectionsManager = new CollectionsManager(Api);
        ParametersManager = new ParametersManager(Api);
        EnumerationsManager = new EnumerationsManager(Api);
        SpecsManager = new SpecsManager(Api);
        LabelsManager = new LabelsManager(Api);
        ClassificationsManager = new ClassificationsManager(Api);
    }

    /// <summary>
    /// Parameters API client providing access to the full generated API surface.
    /// </summary>
    public BaseParametersClient Api { get; protected set; }

    /// <summary>
    /// Manager for Parameter group operations (list, get, update groups)
    /// </summary>
    public GroupsManager GroupsManager { get; }

    /// <summary>
    /// Manager for Parameter collection operations (list, get, create, update collections)
    /// </summary>
    public CollectionsManager CollectionsManager { get; }

    /// <summary>
    /// Manager for Parameter operations (CRUD, sharing, searching, rendering)
    /// </summary>
    public ParametersManager ParametersManager { get; }

    /// <summary>
    /// Manager for Enumeration operations (list, create, update enumerations)
    /// </summary>
    public EnumerationsManager EnumerationsManager { get; }

    /// <summary>
    /// Manager for Spec operations (list, create, update specs)
    /// </summary>
    public SpecsManager SpecsManager { get; }

    /// <summary>
    /// Manager for Label operations (list, get, create, update, delete, attach, detach labels)
    /// </summary>
    public LabelsManager LabelsManager { get; }

    /// <summary>
    /// Manager for Classification operations (groups, categories, disciplines, units)
    /// </summary>
    public ClassificationsManager ClassificationsManager { get; }

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/parameters/v1/accounts/*
    /// </summary>
    public Parameters.V1.Accounts.AccountsRequestBuilder Accounts
    {
        get
        {
            return Api.Parameters.V1.Accounts;
        }
    }

}
