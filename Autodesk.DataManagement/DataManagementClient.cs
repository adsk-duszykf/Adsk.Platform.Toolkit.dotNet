using Autodesk.DataManagement.Helpers;
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


}

