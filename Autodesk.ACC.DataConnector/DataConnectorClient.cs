using Autodesk.ACC.DataConnector.DataConnector.V1;
using Autodesk.ACC.DataConnector.Helpers;
using Autodesk.ACC.DataConnector.Managers;

namespace Autodesk.ACC.DataConnector;

/// <summary>
/// Main entry point for Autodesk Data Connector SDK
/// </summary>
public class DataConnectorClient
{
    /// <summary>
    /// ACC Data Connector API client base path 'https://developer.api.autodesk.com/data-connector/v1'
    /// </summary>
    public ApiRequestBuilder Api { get; }

    /// <summary>
    /// Manager for Data Connector Requests operations
    /// </summary>
    public RequestsManager Requests { get; }

    /// <summary>
    /// Manager for Data Connector Jobs operations
    /// </summary>
    public JobsManager Jobs { get; }

    /// <summary>
    /// Manager for Data Connector Data extraction operations
    /// </summary>
    public DataManager Data { get; }

    /// <summary>
    /// High-level helper functions supporting common operations
    /// </summary>
    public DataConnectorClientHelper Helper { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataConnectorClient"/> class.
    /// </summary>
    /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public DataConnectorClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
    {
        var adapter = Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);

        var baseClient = new BaseDataConnectorClient(adapter);

        Api = new ApiRequestBuilder
        {
            DataConnector = baseClient.DataConnector
        };

        // Initialize managers
        Requests = new RequestsManager(Api);
        Jobs = new JobsManager(Api);
        Data = new DataManager(Api);

        // Keep legacy helper for backward compatibility
        Helper = new DataConnectorClientHelper(baseClient.DataConnector.V1);
    }
}

/// <summary>
/// Container for all API request builders
/// </summary>
public class ApiRequestBuilder
{
    /// <summary>
    /// ACC Data Connector API request builder
    /// </summary>
    public Autodesk.ACC.DataConnector.DataConnector.DataConnectorRequestBuilder? DataConnector { get; init; }
}
