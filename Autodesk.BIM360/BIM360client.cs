using Autodesk.BIM360.Managers;

namespace Autodesk.BIM360;

/// <summary>
/// Main entry point for the creation and management of the BIM360 API client
/// </summary>
public class BIM360client
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BIM360client"/> class.
    /// </summary>
    /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public BIM360client(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
    {
        var adapter = Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);

        Api = new BaseBIM360client(adapter);

        AccountAdminManager = new AccountAdminManager(Api);
        DataConnectorManager = new DataConnectorManager(Api);
        DocumentManagementManager = new DocumentManagementManager(Api);
        IssuesManager = new IssuesManager(Api);
        RFIsManager = new RFIsManager(Api);
        RelationshipsManager = new RelationshipsManager(Api);
        AssetsManager = new AssetsManager(Api);
        ChecklistsManager = new ChecklistsManager(Api);
        CostManager = new CostManager(Api);
        LocationsManager = new LocationsManager(Api);
        ModelCoordinationManager = new ModelCoordinationManager(Api);
        ModelPropertiesManager = new ModelPropertiesManager(Api);
    }

    /// <summary>
    /// BIM 360 API root client (base URL <c>https://developer.api.autodesk.com</c>). See also
    /// <see href="https://aps.autodesk.com/en/docs/bim360/v1/overview/">BIM 360 API overview</see>.
    /// </summary>
    public BaseBIM360client Api { get; protected set; }

    // ── Managers ─────────────────────────────────────────────────────────

    /// <summary>
    /// Manager for Account Admin operations (accounts, projects, users, companies, business units)
    /// </summary>
    public AccountAdminManager AccountAdminManager { get; }

    /// <summary>
    /// Manager for BIM 360 Assets v1/v2 (categories, statuses, custom attributes, asset records).
    /// </summary>
    public AssetsManager AssetsManager { get; }

    /// <summary>
    /// Manager for BIM 360 Checklists (templates and instances).
    /// </summary>
    public ChecklistsManager ChecklistsManager { get; }

    /// <summary>
    /// Manager for Data Connector operations (data requests, jobs, data downloads)
    /// </summary>
    public DataConnectorManager DataConnectorManager { get; }

    /// <summary>
    /// Manager for Cost Management operations (budgets, contracts, change orders, expenses, payments, main contracts, and related settings).
    /// </summary>
    public CostManager CostManager { get; }

    /// <summary>
    /// Manager for BIM 360 Document Management (permissions, custom attributes, versions batch, naming standards, exports)
    /// </summary>
    public DocumentManagementManager DocumentManagementManager { get; }

    /// <summary>
    /// Manager for Issues v2 (containers, issues, comments, attachments, types, attributes).
    /// </summary>
    public IssuesManager IssuesManager { get; }

    /// <summary>
    /// Manager for Locations operations (location nodes and trees)
    /// </summary>
    public LocationsManager LocationsManager { get; }

    /// <summary>
    /// Manager for Model Coordination (model sets, views, clash tests, clashes, screenshots).
    /// </summary>
    public ModelCoordinationManager ModelCoordinationManager { get; }

    /// <summary>
    /// Manager for Model Properties / Construction Index (indexes, diffs, queries, fields, manifests).
    /// </summary>
    public ModelPropertiesManager ModelPropertiesManager { get; }

    /// <summary>
    /// Manager for BIM 360 RFIs v2 (RFIs, comments, attachments, current user).
    /// </summary>
    public RFIsManager RFIsManager { get; }

    /// <summary>
    /// Manager for Relationship service v2 (create, search, batch, sync, delete).
    /// </summary>
    public RelationshipsManager RelationshipsManager { get; }
}
