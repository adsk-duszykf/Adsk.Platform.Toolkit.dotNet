using Autodesk.Vault.Managers;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Autodesk.Vault;

public class VaultClient
{
    /// <summary>
    /// Vault server name
    /// </summary>
    public string VaultServer { get; private set; } = string.Empty;

    /// <summary>
    /// Data Management API client
    /// </summary>
    public BaseVaultClient Api { get; private set; }

    /// <summary>
    /// High-level functions supporting authentication operations
    /// </summary>
    public AuthManager Auth { get; private set; }

    /// <summary>
    /// High-level functions supporting accounts operations (Users, Groups, Roles, Profile Attributes)
    /// </summary>
    public AccountsManager Accounts { get; private set; }

    /// <summary>
    /// High-level functions supporting options operations (System Options, Vault Options)
    /// </summary>
    public OptionsManager Options { get; private set; }

    /// <summary>
    /// High-level functions supporting informational operations (Server Info, Vaults)
    /// </summary>
    public InformationalManager Informational { get; private set; }

    /// <summary>
    /// High-level functions supporting property definition operations
    /// </summary>
    public PropertyManager Properties { get; private set; }

    /// <summary>
    /// High-level functions supporting files and folders operations
    /// </summary>
    public FilesAndFoldersManager FilesAndFolders { get; private set; }

    /// <summary>
    /// High-level functions supporting items operations
    /// </summary>
    public ItemsManager Items { get; private set; }

    /// <summary>
    /// High-level functions supporting change orders operations
    /// </summary>
    public ChangeOrdersManager ChangeOrders { get; private set; }

    /// <summary>
    /// High-level functions supporting links operations
    /// </summary>
    public LinksManager Links { get; private set; }

    /// <summary>
    /// High-level functions supporting search operations
    /// </summary>
    public SearchManager Search { get; private set; }

    /// <summary>
    /// High-level functions supporting jobs operations
    /// </summary>
    public JobsManager Jobs { get; private set; }

    /// <summary>
    /// Create a new instance for the Vault API http client.
    /// </summary>
    /// <param name="get2LeggedAccessToken">Function for getting the APS 2 legged access token used for the following calls</param>
    /// <param name="userId">A valid, active user email address. This user is going to be impersonated in all calls using the access token</param>
    /// <param name="vaultServer">Vault server name</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public VaultClient(Func<Task<string>> get2LeggedAccessToken, string vaultServer, string userId, HttpClient? httpClient = null)
    {
        Api = CreateClient(get2LeggedAccessToken, vaultServer, httpClient);
        VaultServer = vaultServer;

        // Initialize managers
        Auth = new AuthManager(Api);
        Accounts = new AccountsManager(Api);
        Options = new OptionsManager(Api);
        Informational = new InformationalManager(Api);
        Properties = new PropertyManager(Api);
        FilesAndFolders = new FilesAndFoldersManager(Api);
        Items = new ItemsManager(Api);
        ChangeOrders = new ChangeOrdersManager(Api);
        Links = new LinksManager(Api);
        Search = new SearchManager(Api);
        Jobs = new JobsManager(Api);
    }

    /// <summary>
    /// Create a new instance for the Vault API http client.
    /// </summary>
    /// <param name="get3LeggedAccessToken">Function for getting the APS 3 legged access token used for the following calls</param>
    /// <param name="vaultServer">Vault server name</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public VaultClient(Func<Task<string>> get3LeggedAccessToken, string vaultServer, HttpClient? httpClient = null)
    {
        Api = CreateClient(get3LeggedAccessToken, vaultServer, httpClient);
        VaultServer = vaultServer;

        // Initialize managers
        Auth = new AuthManager(Api);
        Accounts = new AccountsManager(Api);
        Options = new OptionsManager(Api);
        Informational = new InformationalManager(Api);
        Properties = new PropertyManager(Api);
        FilesAndFolders = new FilesAndFoldersManager(Api);
        Items = new ItemsManager(Api);
        ChangeOrders = new ChangeOrdersManager(Api);
        Links = new LinksManager(Api);
        Search = new SearchManager(Api);
        Jobs = new JobsManager(Api);
    }

    private static BaseVaultClient CreateClient(Func<Task<string>> getAccessToken, string vaultServer, HttpClient? httpClient = null)
    {
        if (string.IsNullOrEmpty(vaultServer))
            throw new ArgumentNullException(nameof(vaultServer), "Cannot be null or empty.");


        var auth = new BaseBearerTokenAuthenticationProvider(new Common.HttpClientLibrary.AccessTokenProvider(getAccessToken));
        var adapter = new Microsoft.Kiota.Bundle.DefaultRequestAdapter(auth, null, null, httpClient)
        {
            BaseUrl = $"https://{vaultServer}/AutodeskDM/Services/api/vault/v2"
        };

        return new BaseVaultClient(adapter);
    }

}

