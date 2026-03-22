using Autodesk.ACC.Managers;

namespace Autodesk.ACC;

/// <summary>
/// Main entry point for the creation and management of the ACC API client
/// </summary>
public class ACCclient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ACCclient"/> class.
    /// </summary>
    /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public ACCclient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
    {
        var adapter = Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);

        Api = new BaseACCclient(adapter);

        AccountAdminManager = new AccountAdminManager(Api);
        AssetsManager = new AssetsManager(Api);
        AutoSpecsManager = new AutoSpecsManager(Api);
        CostManager = new CostManager(Api);
        DataConnectorManager = new DataConnectorManager(Api);
        FileManagementManager = new FileManagementManager(Api);
        FormsManager = new FormsManager(Api);
        IssuesManager = new IssuesManager(Api);
        LocationsManager = new LocationsManager(Api);
        ModelCoordinationManager = new ModelCoordinationManager(Api);
        ModelPropertiesManager = new ModelPropertiesManager(Api);
        PhotosManager = new PhotosManager(Api);
        RelationshipsManager = new RelationshipsManager(Api);
        ReviewsManager = new ReviewsManager(Api);
        RFIsManager = new RFIsManager(Api);
        SheetsManager = new SheetsManager(Api);
        SubmittalsManager = new SubmittalsManager(Api);
        TakeoffManager = new TakeoffManager(Api);
        TransmittalsManager = new TransmittalsManager(Api);
    }

    /// <summary>
    /// ACC API client base path 'https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-users-me-GET/'
    /// </summary>
    public BaseACCclient Api { get; protected set; }

    // ── Managers ─────────────────────────────────────────────────────────

    /// <summary>
    /// Manager for Account Admin operations (accounts, projects, users, companies, business units)
    /// </summary>
    public AccountAdminManager AccountAdminManager { get; }

    /// <summary>
    /// Manager for Assets operations (asset records, categories, statuses, custom attributes)
    /// </summary>
    public AssetsManager AssetsManager { get; }

    /// <summary>
    /// Manager for AutoSpecs operations (metadata, smart register, requirements, submittals summary)
    /// </summary>
    public AutoSpecsManager AutoSpecsManager { get; }

    /// <summary>
    /// Manager for Cost Management operations (budgets, contracts, change orders, expenses)
    /// </summary>
    public CostManager CostManager { get; }

    /// <summary>
    /// Manager for Data Connector operations (data requests, jobs, data downloads)
    /// </summary>
    public DataConnectorManager DataConnectorManager { get; }

    /// <summary>
    /// Manager for File Management operations (PDF exports, permissions, custom attributes, naming standards, packages)
    /// </summary>
    public FileManagementManager FileManagementManager { get; }

    /// <summary>
    /// Manager for Forms operations (form templates, forms, form values)
    /// </summary>
    public FormsManager FormsManager { get; }

    /// <summary>
    /// Manager for Issues operations (issues, types, comments, attachments, root causes)
    /// </summary>
    public IssuesManager IssuesManager { get; }

    /// <summary>
    /// Manager for Locations operations (location nodes and trees)
    /// </summary>
    public LocationsManager LocationsManager { get; }

    /// <summary>
    /// Manager for Model Coordination operations (model sets, clash tests, clashes, screenshots)
    /// </summary>
    public ModelCoordinationManager ModelCoordinationManager { get; }

    /// <summary>
    /// Manager for Model Properties operations (property indexes, diffs, queries)
    /// </summary>
    public ModelPropertiesManager ModelPropertiesManager { get; }

    /// <summary>
    /// Manager for Photos operations (get and search photos)
    /// </summary>
    public PhotosManager PhotosManager { get; }

    /// <summary>
    /// Manager for Relationships operations (create, search, sync relationships)
    /// </summary>
    public RelationshipsManager RelationshipsManager { get; }

    /// <summary>
    /// Manager for Reviews operations (workflows, reviews, progress, approvals)
    /// </summary>
    public ReviewsManager ReviewsManager { get; }

    /// <summary>
    /// Manager for RFIs operations (RFIs, types, attributes, responses, comments)
    /// </summary>
    public RFIsManager RFIsManager { get; }

    /// <summary>
    /// Manager for Sheets operations (sheets, version sets, uploads, collections, exports)
    /// </summary>
    public SheetsManager SheetsManager { get; }

    /// <summary>
    /// Manager for Submittals operations (submittal items, packages, specs, reviews, tasks)
    /// </summary>
    public SubmittalsManager SubmittalsManager { get; }

    /// <summary>
    /// Manager for Takeoff operations (packages, items, types, classification systems)
    /// </summary>
    public TakeoffManager TakeoffManager { get; }

    /// <summary>
    /// Manager for Transmittals operations (transmittals, recipients, folders, documents)
    /// </summary>
    public TransmittalsManager TransmittalsManager { get; }
}