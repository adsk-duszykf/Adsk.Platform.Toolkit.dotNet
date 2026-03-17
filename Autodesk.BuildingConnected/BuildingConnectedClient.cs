namespace Autodesk.BuildingConnected;

/// <summary>
/// Entry point for the BuildingConnected SDK, exposes the different API clients for each service.
/// </summary>
public class BuildingConnectedClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingConnectedClient"/> class.
    /// </summary>
    /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public BuildingConnectedClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
    {
        var adapter = Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);

        Api = new BaseBuildingConnectedClient(adapter);
    }

    /// <summary>
    /// BuildingConnected API client base path
    /// </summary>
    public BaseBuildingConnectedClient Api { get; protected set; }

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/buildingconnected/v2/*
    /// </summary>
    public Construction.Buildingconnected.V2.V2RequestBuilder BuildingConnected => Api.Construction.Buildingconnected.V2;

}
