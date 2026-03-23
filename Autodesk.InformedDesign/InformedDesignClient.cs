using Autodesk.InformedDesign.Managers;

namespace Autodesk.InformedDesign;

/// <summary>
/// Main entry point for the creation and management of the Informed Design API client
/// </summary>
public class InformedDesignClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InformedDesignClient"/> class.
    /// </summary>
    /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public InformedDesignClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
    {
        var adapter = Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);

        Api = new BaseInformedDesignClient(adapter);

        ProductsManager = new ProductsManager(Api);
        ReleasesManager = new ReleasesManager(Api);
        VariantsManager = new VariantsManager(Api);
        OutputsManager = new OutputsManager(Api);
        UploadsManager = new UploadsManager(Api);
        DownloadsManager = new DownloadsManager(Api);
        RulesManager = new RulesManager(Api);
    }

    /// <summary>
    /// Informed Design API client — provides low-level fluent access to all endpoints
    /// </summary>
    /// <remarks>
    /// Usage: <c>Api.IndustrializedConstruction.InformedDesign.V1.Products</c>
    /// </remarks>
    public BaseInformedDesignClient Api { get; protected set; }

    // ── Managers ─────────────────────────────────────────────────────────

    /// <summary>
    /// Manager for Products operations (CRUD, upload, download)
    /// </summary>
    public ProductsManager ProductsManager { get; }

    /// <summary>
    /// Manager for Releases operations (CRUD for product releases)
    /// </summary>
    public ReleasesManager ReleasesManager { get; }

    /// <summary>
    /// Manager for Variants operations (CRUD for release variants)
    /// </summary>
    public VariantsManager VariantsManager { get; }

    /// <summary>
    /// Manager for Outputs operations (CRUD for variant outputs)
    /// </summary>
    public OutputsManager OutputsManager { get; }

    /// <summary>
    /// Manager for Uploads operations (upload requests)
    /// </summary>
    public UploadsManager UploadsManager { get; }

    /// <summary>
    /// Manager for Downloads operations (download requests)
    /// </summary>
    public DownloadsManager DownloadsManager { get; }

    /// <summary>
    /// Manager for Rules operations (evaluate, validate, get rules)
    /// </summary>
    public RulesManager RulesManager { get; }
}
