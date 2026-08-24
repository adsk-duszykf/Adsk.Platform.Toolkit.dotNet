
using Autodesk.Datum.PIM360.Managers;

namespace Autodesk.Datum.PIM360
{
    /// <summary>
    /// Client for Autodesk Datum API. Provides access to all API endpoints and operations.
    /// </summary>
    public class DatumPIM360Client
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatumPIM360Client"/> class.
        /// </summary>
        /// <param name="tenantName">The PIM360 tenant name, used to build the base URL as https://{tenantName}.pim360.io/api</param>
        /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
        /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
        public DatumPIM360Client(string tenantName, Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
        {
            if (string.IsNullOrWhiteSpace(tenantName))
            {
                throw new ArgumentException("tenantName cannot be null or empty.", nameof(tenantName));
            }

            var adapter = Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);

            //Only override the BaseUrl if it is not set for testing purposes
            //Tests method could set the BaseUrl to a mock server
            adapter.BaseUrl ??= $"https://{tenantName}.pim360.io/api";

            Api = new BaseDatumPIM360Client(adapter);
            ItemManager = new ItemManager(Api);
            EicManager = new EicManager(Api);
            ViewManager = new ViewManager(Api);
            QueryManager = new QueryManager(Api);
            ActivityManager = new ActivityManager(Api);
            FileManager = new FileManager(Api);
            CommentManager = new CommentManager(Api);
            MetaManager = new MetaManager(Api);
            QueueManager = new QueueManager(Api);
            ExplorerManager = new ExplorerManager(Api);
            AllocatorManager = new AllocatorManager(Api);
            DashboardManager = new DashboardManager(Api);
            FacilityManager = new FacilityManager(Api);
        }

        /// <summary>Gets the underlying Kiota-generated API client.</summary>
        public BaseDatumPIM360Client Api { get; protected set; }

        /// <summary>Gets the manager for object and item operations.</summary>
        public ItemManager ItemManager { get; protected set; }

        /// <summary>Gets the manager for Engineering Information Change operations.</summary>
        public EicManager EicManager { get; protected set; }

        /// <summary>Gets the manager for saved view operations.</summary>
        public ViewManager ViewManager { get; protected set; }

        /// <summary>Gets the manager for query and query-result operations.</summary>
        public QueryManager QueryManager { get; protected set; }

        /// <summary>Gets the manager for activity and ETL queue operations.</summary>
        public ActivityManager ActivityManager { get; protected set; }

        /// <summary>Gets the manager for file operations.</summary>
        public FileManager FileManager { get; protected set; }

        /// <summary>Gets the manager for comments and associations.</summary>
        public CommentManager CommentManager { get; protected set; }

        /// <summary>Gets the manager for metadata and explorer operations.</summary>
        public MetaManager MetaManager { get; protected set; }

        /// <summary>Gets the manager for ETL queue operations.</summary>
        public QueueManager QueueManager { get; protected set; }

        /// <summary>Gets the manager for attribute explorer operations.</summary>
        public ExplorerManager ExplorerManager { get; protected set; }

        /// <summary>Gets the manager for tag number allocator operations.</summary>
        public AllocatorManager AllocatorManager { get; protected set; }

        /// <summary>Gets the manager for dashboard query operations.</summary>
        public DashboardManager DashboardManager { get; protected set; }

        /// <summary>Gets the manager for facility explorer operations.</summary>
        public FacilityManager FacilityManager { get; protected set; }
    }
}
