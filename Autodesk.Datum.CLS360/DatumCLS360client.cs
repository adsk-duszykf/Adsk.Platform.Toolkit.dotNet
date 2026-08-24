
using Autodesk.Datum.CLS360.Managers;

namespace Autodesk.Datum.CLS360
{
    /// <summary>
    /// Client for Autodesk Datum API. Provides access to all API endpoints and operations.
    /// </summary>
    public class DatumCLS360Client
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatumCLS360Client"/> class.
        /// </summary>
        /// <param name="tenantName">The CLS360 tenant name, used to build the base URL as https://{tenantName}.cls360.io/api</param>
        /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
        /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
        public DatumCLS360Client(string tenantName, Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
        {
            if (string.IsNullOrWhiteSpace(tenantName))
            {
                throw new ArgumentException("tenantName cannot be null or empty.", nameof(tenantName));
            }

            var adapter = Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);

            //Only override the BaseUrl if it is not set for testing purposes
            //Tests method could set the BaseUrl to a mock server
            if (adapter.BaseUrl is null)
            {
                adapter.BaseUrl = $"https://{tenantName}.cls360.io/api";
            }

            Api = new BaseDatumCLS360Client(adapter);
            DomainManager = new DomainManager(Api);
            ClassManager = new ClassManager(Api);
            TemplateManager = new TemplateManager(Api);
            AttributeManager = new AttributeManager(Api);
            FileManager = new FileManager(Api);
        }

        /// <summary>
        /// Gets the underlying Kiota-generated API client.
        /// </summary>
        public BaseDatumCLS360Client Api { get; protected set; }

        /// <summary>
        /// Gets the domain manager for domain-related operations.
        /// </summary>
        public DomainManager DomainManager { get; protected set; }

        /// <summary>
        /// Gets the class manager for class-related operations.
        /// </summary>
        public ClassManager ClassManager { get; protected set; }

        /// <summary>
        /// Gets the template manager for template-related operations.
        /// </summary>
        public TemplateManager TemplateManager { get; protected set; }

        /// <summary>
        /// Gets the attribute manager for attribute-related operations.
        /// </summary>
        public AttributeManager AttributeManager { get; protected set; }

        /// <summary>
        /// Gets the file manager for file download operations.
        /// </summary>
        public FileManager FileManager { get; protected set; }
    }
}
