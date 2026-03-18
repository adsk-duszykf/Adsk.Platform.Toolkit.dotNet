using Autodesk.DataManagement.Helpers;
using Autodesk.DataManagement.Managers;
using Autodesk.DataManagement.OSS;

namespace Autodesk.DataManagement;
/// <summary>
/// Data Management client.
/// </summary>
public class DataManagementClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataManagementClient"/> class.
    /// </summary>
    /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public DataManagementClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
    {
        var adapter = Autodesk.Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);
        DataMgtApi = new BaseDataManagementClient(adapter);
        OssApi = new BaseOSSClient(adapter);
        Helper = new DataManagementClientHelper(DataMgtApi, OssApi);

        Hubs = new HubsManager(DataMgtApi);
        Projects = new ProjectsManager(DataMgtApi);
        Folders = new FoldersManager(DataMgtApi);
        Items = new ItemsManager(DataMgtApi);
        Versions = new VersionsManager(DataMgtApi);
        Commands = new CommandsManager(DataMgtApi);
        Buckets = new BucketsManager(OssApi);
        Objects = new ObjectsManager(OssApi);
    }

    /// <summary>
    /// Data Management API client
    /// </summary>
    public BaseDataManagementClient DataMgtApi { get; private set; }

    /// <summary>
    /// OSS API client
    /// </summary>
    public BaseOSSClient OssApi { get; private set; }

    /// <summary>
    /// High-level order functions supporting common operations
    /// </summary>
    public DataManagementClientHelper Helper { get; private set; }

    /// <summary>
    /// Manager for Hub operations (list and get hubs)
    /// </summary>
    public HubsManager Hubs { get; }

    /// <summary>
    /// Manager for Project operations (list, get, top folders, storage, downloads, jobs)
    /// </summary>
    public ProjectsManager Projects { get; }

    /// <summary>
    /// Manager for Folder operations (get, create, update, contents, search, refs, relationships)
    /// </summary>
    public FoldersManager Folders { get; }

    /// <summary>
    /// Manager for Item operations (get, create, update, tip, versions, refs, relationships)
    /// </summary>
    public ItemsManager Items { get; }

    /// <summary>
    /// Manager for Version operations (get, create, update, downloads, refs, relationships)
    /// </summary>
    public VersionsManager Versions { get; }

    /// <summary>
    /// Manager for Command operations (execute bulk commands on project resources)
    /// </summary>
    public CommandsManager Commands { get; }

    /// <summary>
    /// Manager for OSS Bucket operations (list, create, delete, details)
    /// </summary>
    public BucketsManager Buckets { get; }

    /// <summary>
    /// Manager for OSS Object operations (list, delete, copy, signed URLs, S3 uploads/downloads)
    /// </summary>
    public ObjectsManager Objects { get; }
}
