using Autodesk.BuildingConnected.Managers;

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

        ProjectsManager = new ProjectsManager(Api);
        ProjectTeamMembersManager = new ProjectTeamMembersManager(Api);
        BidPackagesManager = new BidPackagesManager(Api);
        InvitesManager = new InvitesManager(Api);
        BidsManager = new BidsManager(Api);
        ProjectBidFormsManager = new ProjectBidFormsManager(Api);
        ScopeSpecificBidFormsManager = new ScopeSpecificBidFormsManager(Api);
        OpportunitiesManager = new OpportunitiesManager(Api);
        ContactsManager = new ContactsManager(Api);
        UsersManager = new UsersManager(Api);
        CertificationManager = new CertificationManager(Api);
        OfficesManager = new OfficesManager(Api);
        OpportunityProjectPairsManager = new OpportunityProjectPairsManager(Api);
    }

    /// <summary>
    /// BuildingConnected API client base path
    /// </summary>
    public BaseBuildingConnectedClient Api { get; protected set; }

    /// <summary>
    /// Shortcut to endpoints https://developer.api.autodesk.com/construction/buildingconnected/v2/*
    /// </summary>
    public Construction.Buildingconnected.V2.V2RequestBuilder BuildingConnected => Api.Construction.Buildingconnected.V2;

    /// <summary>
    /// Manager for Projects and Costs operations
    /// </summary>
    public ProjectsManager ProjectsManager { get; }

    /// <summary>
    /// Manager for Project Team Members operations
    /// </summary>
    public ProjectTeamMembersManager ProjectTeamMembersManager { get; }

    /// <summary>
    /// Manager for Bid Packages operations (including batch create/patch/delete)
    /// </summary>
    public BidPackagesManager BidPackagesManager { get; }

    /// <summary>
    /// Manager for Invites operations (including email imports)
    /// </summary>
    public InvitesManager InvitesManager { get; }

    /// <summary>
    /// Manager for Bids operations (including attachments, line items, and plugs)
    /// </summary>
    public BidsManager BidsManager { get; }

    /// <summary>
    /// Manager for Project Bid Forms and Line Items operations
    /// </summary>
    public ProjectBidFormsManager ProjectBidFormsManager { get; }

    /// <summary>
    /// Manager for Scope-Specific Bid Forms and Line Items operations
    /// </summary>
    public ScopeSpecificBidFormsManager ScopeSpecificBidFormsManager { get; }

    /// <summary>
    /// Manager for Opportunities and Comments operations
    /// </summary>
    public OpportunitiesManager OpportunitiesManager { get; }

    /// <summary>
    /// Manager for Contacts operations
    /// </summary>
    public ContactsManager ContactsManager { get; }

    /// <summary>
    /// Manager for Users operations (including current user)
    /// </summary>
    public UsersManager UsersManager { get; }

    /// <summary>
    /// Manager for Certification operations (certificate types and agencies)
    /// </summary>
    public CertificationManager CertificationManager { get; }

    /// <summary>
    /// Manager for Offices operations
    /// </summary>
    public OfficesManager OfficesManager { get; }

    /// <summary>
    /// Manager for Opportunity-Project Pairs operations
    /// </summary>
    public OpportunityProjectPairsManager OpportunityProjectPairsManager { get; }
}
