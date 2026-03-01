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
    }

    /// <summary>
    /// Tandem API client providing access to the full generated API surface.
    /// </summary>
    public BaseTandemClient Api { get; protected set; }

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/tandem/v1/groups/*
    /// </summary>
    public Tandem.V1.Groups.GroupsRequestBuilder Groups => Api.Tandem.V1.Groups;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/tandem/v1/modeldata/*
    /// </summary>
    public Tandem.V1.Modeldata.ModeldataRequestBuilder Modeldata => Api.Tandem.V1.Modeldata;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/tandem/v1/models/*
    /// </summary>
    public Tandem.V1.ModelsRequests.ModelsRequestBuilder Models => Api.Tandem.V1.Models;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/tandem/v1/timeseries/*
    /// </summary>
    public Tandem.V1.Timeseries.TimeseriesRequestBuilder Timeseries => Api.Tandem.V1.Timeseries;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/tandem/v1/twins/*
    /// </summary>
    public Tandem.V1.Twins.TwinsRequestBuilder Twins => Api.Tandem.V1.Twins;

}
