namespace Autodesk.BIM360;

public class BIM360client
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BIM360client"/> class.
    /// </summary>
    /// <param name="getAccessToken">Function for getting the access token used for the following calls</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public BIM360client(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
    {
        var adapter = Common.HttpClientLibrary.HttpClientFactory.CreateAdapter(getAccessToken, httpClient);

        Api = new BaseBIM360client(adapter);
    }

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/hq/v1/accounts/*
    /// </summary>
    public Hq.V1.Accounts.AccountsRequestBuilder Accounts => Api.Hq.V1.Accounts;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/hq/v1/regions/eu/accounts/*
    /// </summary>
    public Hq.V1.Regions.Eu.Accounts.AccountsRequestBuilder AccountsEU_V1 => Api.Hq.V1.Regions.Eu.Accounts;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/hq/v2/regions/eu/accounts/*
    /// </summary>
    public Hq.V2.Regions.Eu.Accounts.AccountsRequestBuilder AccountsEU_V2 => Api.Hq.V2.Regions.Eu.Accounts;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/admin/v1/*
    /// </summary>
    public Construction.Admin.V1.V1RequestBuilder Admin => Api.Construction.Admin.V1;

    /// <summary>
    /// BIM360 API client base path 'https://aps.autodesk.com/en/docs/acc/v1/reference/http/issues-users-me-GET/'
    /// </summary>
    public BaseBIM360client Api { get; protected set; }

    /// <summary>

    /// Shortcut to endpoints https://developer.api.autodesk.com/bim360/assets/v1/*
    /// </summary>
    public Bim360.Assets.V1.V1RequestBuilder Assets => Api.Bim360.Assets.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/bim360/checklists/v1/*
    /// </summary>
    public Bim360.Checklists.V1.V1RequestBuilder Checklists => Api.Bim360.Checklists.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/bim360/clash/v3/*
    /// </summary>
    public Bim360.Clash.V3.V3RequestBuilder Clash => Api.Bim360.Clash.V3;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/cost/v1/*
    /// </summary>
    public Cost.V1.V1RequestBuilder Cost => Api.Cost.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/dataconnector/v1/*
    /// </summary>
    public DataConnector.V1.V1RequestBuilder DataConnector => Api.DataConnector.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/bim360/docs/v1/*
    /// </summary>
    public Bim360.Docs.V1.V1RequestBuilder Docs => Api.Bim360.Docs.V1;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/index/v2/*
    /// </summary>
    public Construction.IndexNamespace.V2.V2RequestBuilder Index => Api.Construction.Index.V2;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/issues/v2/*
    /// </summary>
    public Issues.V2.V2RequestBuilder Issues => Api.Issues.V2;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/bim360/modelset/v3/*
    /// </summary>
    public Bim360.Modelset.V3.V3RequestBuilder ModelSet => Api.Bim360.Modelset.V3;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/bim360/relationship/v2/*
    /// </summary>
    public Bim360.Relationship.V2.V2RequestBuilder Projects => Api.Bim360.Relationship.V2;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/bim360/relationship/v2/*
    /// </summary>
    public Bim360.Relationship.V2.V2RequestBuilder Relationships => Api.Bim360.Relationship.V2;

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/bim360/rfis/v2/*
    /// </summary>
    public Bim360.Rfis.V2.V2RequestBuilder RFIs => Api.Bim360.Rfis.V2;

}
