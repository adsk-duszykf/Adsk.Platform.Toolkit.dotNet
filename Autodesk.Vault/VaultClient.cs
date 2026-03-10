using Autodesk.Vault.Managers;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Autodesk.Vault;

/// <summary>
/// Main entry point for interacting with the Vault API. It initializes the API client and provides high-level managers for different API areas (Authentication, Accounts, Options, Informational, Properties, Files and Folders, Items, Change Orders, Links, Search and Jobs).
/// </summary>
public class VaultClient
{
    /// <summary>
    /// Vault server URL
    /// </summary>
    public string VaultServerUrl { get; private set; } = string.Empty;

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
    /// <param name="vaultServerUrl">Vault server URL. Like "http://10.148.0.1", "https://windowsmachine1"</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public VaultClient(Func<Task<string>> get2LeggedAccessToken, string vaultServerUrl, string userId, HttpClient? httpClient = null)
    {
        Api = CreateClient(get2LeggedAccessToken, vaultServerUrl, httpClient);
        VaultServerUrl = vaultServerUrl;

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
    /// <param name="vaultServerUrl">Vault server URL. Like "http://10.148.0.1", "https://windowsmachine1"</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    public VaultClient(Func<Task<string>> get3LeggedAccessToken, string vaultServerUrl, HttpClient? httpClient = null)
    {
        Api = CreateClient(get3LeggedAccessToken, vaultServerUrl, httpClient);
        VaultServerUrl = vaultServerUrl;

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
    /// Initializes the Vault API client with the provided access token function, vault server and optional HttpClient. The access token function can be for either 2 legged or 3 legged authentication as both are supported by the same authentication provider. This method is used internally by the constructors to create the API client instance used for all calls to the Vault API.
    /// </summary>
    /// <param name="getAccessToken">Function for getting the APS access token used for the following calls</param>
    /// <param name="vaultServerUrl">Vault server URL. Like "http://10.148.0.1", "https://windowsmachine1"</param>
    /// <param name="httpClient">Optional: Override the default HttpClient used for performing API calls</param>
    /// <returns>Instance of BaseVaultClient</returns>
    /// <exception cref="ArgumentNullException"></exception>
    private static BaseVaultClient CreateClient(Func<Task<string>> getAccessToken, string vaultServerUrl, HttpClient? httpClient = null)
    {
        if (!Uri.TryCreate(vaultServerUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("Must be a valid absolute HTTP or HTTPS URL.", nameof(vaultServerUrl));

        var baseUrl = new Uri(uri, "/AutodeskDM/Services/api/vault/v2").ToString();

        var auth = new BaseBearerTokenAuthenticationProvider(new Common.HttpClientLibrary.AccessTokenProvider(getAccessToken));
        var adapter = new Microsoft.Kiota.Bundle.DefaultRequestAdapter(auth, null, null, httpClient)
        {
            BaseUrl = baseUrl
        };

        return new BaseVaultClient(adapter);
    }

}

