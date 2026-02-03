namespace Autodesk.ACC;

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
    }

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/hq/v1/accounts/*
    /// </summary>
    public Hq.V1.Accounts.AccountsRequestBuilder Accounts => Api.Hq.V1.Accounts;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/bim360/relationship/v2/*
    /// </summary>
    public Bim360.Relationship.V2.V2RequestBuilder Projects => Api.Bim360.Relationship.V2;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/bim360/clash/v3/*
    /// </summary>
    public Bim360.Clash.V3.V3RequestBuilder Clash => Api.Bim360.Clash.V3;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/bim360/docs/v1/*
    /// </summary>
    public Bim360.Docs.V1.V1RequestBuilder Docs => Api.Bim360.Docs.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/bim360/modelset/v3/*
    /// </summary>
    public Bim360.Modelset.V3.V3RequestBuilder ModelSet => Api.Bim360.Modelset.V3;

    /// <summary>
    /// ACC API client base path 'https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-users-me-GET/'
    /// </summary>
    public BaseACCclient Api { get; protected set; }

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/autospecs/v1/*
    /// </summary>
    public Construction.Autospecs.V1.V1RequestBuilder AutoSpecs => Api.Construction.Autospecs.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/admin/v1/*
    /// </summary>
    public Construction.Admin.V1.V1RequestBuilder Admin => Api.Construction.Admin.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/issues/v1/*
    /// </summary>
    public Construction.Issues.V1.V1RequestBuilder Issues => Api.Construction.Issues.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/sheets/v1/*
    /// </summary>
    public Construction.Sheets.V1.V1RequestBuilder Sheets => Api.Construction.Sheets.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/forms/v1/*
    /// </summary>
    public Construction.Forms.V1.V1RequestBuilder Forms => Api.Construction.Forms.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/files/v1/*
    /// </summary>
    public Construction.Files.V1.V1RequestBuilder Files => Api.Construction.Files.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/index/v2/*
    /// </summary>
    public Construction.IndexNamespace.V2.V2RequestBuilder Index => Api.Construction.Index.V2;
    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/cost/v1/*
    /// </summary>
    public Cost.V1.V1RequestBuilder Cost => Api.Cost.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/dataconnector/v1/*
    /// </summary>
    public DataConnector.V1.V1RequestBuilder DataConnector => Api.DataConnector.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/submittals/v1/*
    /// </summary>
    public Construction.Submittals.V2.V2RequestBuilder Submittals => Api.Construction.Submittals.V2;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/rfis/v3/*
    /// </summary>
    public Construction.Rfis.V3.V3RequestBuilder RFIs => Api.Construction.Rfis.V3;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/locations/v2/*
    /// </summary>
    public Construction.Locations.V2.V2RequestBuilder Locations => Api.Construction.Locations.V2;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/packages/v1/*
    /// </summary>
    public Construction.Packages.V1.V1RequestBuilder Packages => Api.Construction.Packages.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/reviews/v1/*
    /// </summary>
    public Construction.Reviews.V1.V1RequestBuilder Reviews => Api.Construction.Reviews.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/photos/v1/*
    /// </summary>
    public Construction.Photos.V1.V1RequestBuilder Photos => Api.Construction.Photos.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/takeoff/v1/*
    /// </summary>
    public Construction.Takeoff.V1.V1RequestBuilder Takeoff => Api.Construction.Takeoff.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/transmittals/v1/*
    /// </summary>
    public Construction.Transmittals.V1.V1RequestBuilder Transmittals => Api.Construction.Transmittals.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/rcm/v1/*
    /// </summary>
    public Construction.Rcm.V1.V1RequestBuilder RCM => Api.Construction.Rcm.V1;
}