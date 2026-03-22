using Autodesk.Tandem.Managers;

namespace Autodesk.Tandem;

/// <summary>
/// High-level client for the Autodesk Tandem Digital Twin APIs.
/// </summary>
public class TandemClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TandemClient"/> class.
    /// </summary>
    /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public TandemClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
    {
        var adapter = Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);

        Api = new BaseTandemClient(adapter);

        TwinsManager = new TwinsManager(Api);
        GroupsManager = new GroupsManager(Api);
        ModeldataManager = new ModeldataManager(Api);
        StreamsManager = new StreamsManager(Api);
        StreamConfigsManager = new StreamConfigsManager(Api);
        DocumentsManager = new DocumentsManager(Api);
        TemplatesManager = new TemplatesManager(Api);
        ViewsManager = new ViewsManager(Api);
    }

    /// <summary>
    /// Tandem API client providing access to the full generated API surface.
    /// </summary>
    public BaseTandemClient Api { get; protected set; }

    /// <summary>
    /// Manager for Twins operations (get, create, history, users, default model).
    /// </summary>
    public TwinsManager TwinsManager { get; }

    /// <summary>
    /// Manager for Groups operations (list, get, history, user management).
    /// </summary>
    public GroupsManager GroupsManager { get; }

    /// <summary>
    /// Manager for Modeldata operations (create, mutate, scan, schema, history).
    /// </summary>
    public ModeldataManager ModeldataManager { get; }

    /// <summary>
    /// Manager for Streams operations (time series data ingestion and retrieval).
    /// </summary>
    public StreamsManager StreamsManager { get; }

    /// <summary>
    /// Manager for Stream Configs operations (list, get, update configurations).
    /// </summary>
    public StreamConfigsManager StreamConfigsManager { get; }

    /// <summary>
    /// Manager for Documents operations (create, get, delete documents).
    /// </summary>
    public DocumentsManager DocumentsManager { get; }

    /// <summary>
    /// Manager for Templates operations (get twin templates).
    /// </summary>
    public TemplatesManager TemplatesManager { get; }

    /// <summary>
    /// Manager for Views operations (get saved views).
    /// </summary>
    public ViewsManager ViewsManager { get; }

}
