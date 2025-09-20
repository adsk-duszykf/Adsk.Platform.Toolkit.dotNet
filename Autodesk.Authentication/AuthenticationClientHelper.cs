using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Autodesk.Authentication.Authentication.V2.Token;
using Autodesk.Authentication.Helpers.Models;
using SDKmodels = Autodesk.Authentication.Models;

namespace Autodesk.Authentication.Helpers;
/// <summary>
/// Helper class for the Autodesk Authentication SDK
/// </summary>
public class AuthenticationClientHelper
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// The Authentication API client
    /// </summary>
    public BaseAuthenticationClient Api { get; init; }
    internal AuthenticationClientHelper(BaseAuthenticationClient api, HttpClient httpClient)
    {
        Api = api;
        this.httpClient = httpClient;
    }
    /// <summary>
    /// Create the url for reaching the Autodesk login page with PKCE authentication
    /// </summary>
    /// <param name="clientId">Autodesk App Id</param>
    /// <param name="redirectUri">Callback url</param>
    /// <param name="scope">Token scope</param>
    /// <param name="nonce">Optional, except if scope is 'OpenId'</param>
    /// <param name="state">Optional.</param>
    /// <param name="forceLogin">Default:False. If 'true' ignore the current session and force the login again</param>
    /// <returns>Url for Autodesk PKCE authentication</returns>
    public static string CreatePKCE_authenticationUrl(string clientId, string redirectUri, IEnumerable<AuthenticationScope> scope, string codeChallenge, string nonce = "", string state = "", bool forceLogin = false)
    {
        var url = new UriBuilder(CreateAuthenticationUrl(clientId, redirectUri, scope, nonce, state, forceLogin));

        url.Query += $"code_challenge={codeChallenge}&code_challenge_method=S256";

        return url.ToString();
    }
    /// <summary>
    /// Generates a random string between 43 and 128 characters containing only alphanumeric characters and the punctuation characters '-', '.', '_', '~'.
    /// </summary>
    /// <param name="length">The length of the string to generate (between 43 and 128).</param>
    /// <returns>A random string.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static string GenerateRandomCodeVerifier(int length = 64)
    {
        if (length < 43 || length > 128)
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be between 43 and 128.");

        const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~";
        var randomBytes = new byte[length];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = allowedChars[randomBytes[i] % allowedChars.Length];
        }
        return new string(chars);
    }
    
    /// <summary>
    /// Generates a PKCE code challenge from a code verifier using SHA256 and Base64Url encoding.
    /// </summary>
    /// <returns>The code challenge string, and its code verifier</returns>
    public static (string codeVerifier, string codeChallenge) CreatePkceCodeChallenge()
    {
        var codeVerifier = GenerateRandomCodeVerifier();
        var bytes = Encoding.ASCII.GetBytes(codeVerifier);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        var codeChallenge = Base64UrlEncode(hash);
        return (codeVerifier, codeChallenge);
    }

    /// <summary>
    /// Encodes a byte array using Base64Url encoding (RFC 7636).
    /// </summary>
    /// <param name="input">The byte array to encode.</param>
    /// <returns>The Base64Url encoded string.</returns>
    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>
    /// Get the User info. 
    /// </summary>
    /// <param name="threeLeggedToken">3L Access token</param>
    /// <returns>User info</returns>
    public async Task<UserInfo?> GetUserInfoAsync(string threeLeggedToken)
    {
        var userInfoUrl = @"https://api.userprofile.autodesk.com/userinfo";

        var req = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(userInfoUrl),

        };
        req.Headers.Add("Authorization", $"Bearer {threeLeggedToken}");

        var response = await httpClient.SendAsync(req);
        response.EnsureSuccessStatusCode();

        var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();

        return userInfo;

    }

    /// <summary>
    /// Create the url for reaching the Autodesk login page (3legged Auth)
    /// </summary>
    /// <param name="clientId">Autodesk App Id</param>
    /// <param name="redirectUri">Callback url</param>
    /// <param name="scope">Token scope</param>
    /// <param name="nonce">Optional, except if scope is 'OpenId'</param>
    /// <param name="state"></param>
    /// <param name="forceLogin"></param>
    /// <returns>Url for the Autodesk login page</returns>
    public static string CreateAuthenticationUrl(string clientId, string redirectUri, IEnumerable<AuthenticationScope> scope, string nonce = "", string state = "", bool forceLogin = false)
    {
        var scopeAsString = CreateScopeString(scope);
        var url = new UriBuilder("https://developer.api.autodesk.com/authentication/v2/authorize")
        {
            Query = $"response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope={scopeAsString}"
        };

        if (string.IsNullOrEmpty(nonce) == false)
        {
            url.Query += $"&nonce={nonce}";
        }

        if (string.IsNullOrEmpty(state))
        {
            url.Query += $"&state={state}";
        }

        if (forceLogin)
        {
            url.Query += $"&prompt=login";
        }
        return url.ToString();
    }
    /// <summary>
    /// Extract the code from the callback url
    /// </summary>
    /// <param name="url">Callback url from Autodesk Authentication</param>
    /// <returns>Code</returns>
    public static string? ExtractCodeFromUrl(string url)
    {
        var uri = new Uri(url);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var code = query["code"];
        return code;
    }
    /// <summary>
    /// Refresh a 3 legged token
    /// </summary>
    /// <param name="clientId">Autodesk App Id</param>
    /// <param name="clientSecret">Autodesk App secret</param>
    /// <param name="refreshToken">Refresh token returned by the previous authentication</param>
    /// <param name="scopes">New scope. You can only reduce the initial scope</param>
    /// <returns>Fresh 3 legged token</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<AuthTokenExtended> RefreshThreeLeggedToken(string clientId, string clientSecret, string refreshToken, IEnumerable<AuthenticationScope>? scopes = null)
    {

        var bodyReq = new TokenPostRequestBody()
        {
            GrantType = SDKmodels.Granttype.Refresh_token,
            RefreshToken = refreshToken,
            Scope = scopes is null ? null : CreateScopeString(scopes)
        };

        var result = await Api.Authentication.V2.Token.PostAsync(bodyReq, r =>
        {
            r.Headers.Add("Authorization", CreateAuthorizationString(clientId, clientSecret));
        });

        var newToken = new AuthTokenExtended(result);

        return newToken ?? throw new InvalidOperationException("Token is null");

    }
    /// <summary>
    /// Create an auto refreshing 2 legged token
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="scopes"></param>
    /// <param name="authTokenStore">Used for storing the generated token. The token will be reused until it expires. At that point it will be regenerated</param>
    /// <returns>Function returning a 2L AccessToken</returns>
    public Func<Task<string>> CreateTwoLeggedAutoRefreshToken(string clientId, string clientSecret, IEnumerable<string> scopes, ITokenStore authTokenStore)
    {
        return async () =>
        {
            var currentToken = authTokenStore.Get();

            var isExpired = currentToken is null || currentToken?.ExpiresAt.Subtract(DateTime.UtcNow).TotalSeconds < 10;

            if (isExpired)
            {
                var newToken = await GetTwoLeggedToken(clientId, clientSecret, scopes);
                currentToken = new AuthTokenExtended(newToken);
                authTokenStore.Set(currentToken);
            }

            return currentToken?.AccessToken ?? string.Empty;
        };
    }
    /// <summary>
    /// Create an auto refreshing 2 legged token
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="scopes"></param>
    /// <param name="authTokenStore">Used for storing the generated token. The token will be reused until it expires. At that point it will be regenerated</param>
    /// <returns>Function returning a 2L AccessToken</returns>
    [Obsolete("Use CreateTwoLeggedAutoRefreshToken(string clientId, string clientSecret, IEnumerable<string> scopes, ITokenStore authTokenStore) instead")]
    public Func<Task<string>> CreateTwoLeggedAutoRefreshToken(string clientId, string clientSecret, IEnumerable<AuthenticationScope> scopes, ITokenStore authTokenStore)
    {
        return async () =>
        {
            var currentToken = authTokenStore.Get();

            var isExpired = currentToken is null || currentToken?.ExpiresAt.Subtract(DateTime.UtcNow).TotalSeconds < 10;

            if (isExpired)
            {
                var newToken = await GetTwoLeggedToken(clientId, clientSecret, scopes);
                currentToken = new AuthTokenExtended(newToken);
                authTokenStore.Set(currentToken);
            }

            return currentToken?.AccessToken ?? string.Empty;
        };
    }
    /// <summary>
    /// Create a 2 legged token
    /// </summary>
    /// <param name="clientId">Autodesk App Id</param>
    /// <param name="clientSecret">Autodesk App Secret</param>
    /// <param name="scopes">List of scopes</param>
    /// <returns>Fresh 2 legged token with expiration date calculated</returns>
    public async Task<AuthTokenExtended> GetTwoLeggedToken(string clientId, string clientSecret, IEnumerable<string> scopes)
    {

        var body = new TokenPostRequestBody()
        {
            GrantType = SDKmodels.Granttype.Client_credentials,
            Scope = CreateScopeString(scopes)
        };

        var authString = CreateAuthorizationString(clientId, clientSecret);

        var result = await Api.Authentication.V2.Token.PostAsync(body, r =>
        {
            r.Headers.Add("Authorization", authString);
        });


        return new AuthTokenExtended(result);
    }
    /// <summary>
    /// Create a 2 legged token
    /// </summary>
    /// <param name="clientId">Autodesk App Id</param>
    /// <param name="clientSecret">Autodesk App Secret</param>
    /// <param name="scopes">List of scopes</param>
    /// <returns>Fresh 2 legged token with expiration date calculated</returns>
    [Obsolete("Use GetTwoLeggedToken(string clientId, string clientSecret, IEnumerable<string> scopes) instead")]
    public async Task<AuthTokenExtended> GetTwoLeggedToken(string clientId, string clientSecret, IEnumerable<AuthenticationScope> scopes)
    {

        var body = new TokenPostRequestBody()
        {
            GrantType = SDKmodels.Granttype.Client_credentials,
            Scope = CreateScopeString(scopes)
        };

        var authString = CreateAuthorizationString(clientId, clientSecret);

        var result = await Api.Authentication.V2.Token.PostAsync(body, r =>
        {
            r.Headers.Add("Authorization", authString);
        });


        return new AuthTokenExtended(result);
    }

    /// <summary>
    /// Combine client id and client secret to create a base64 encoded string
    /// </summary>
    /// <param name="clientId">Autodesk App Id</param>
    /// <param name="clientSecret">Autodesk App Secret</param>
    /// <returns>'{clientId}:{clientSecret}' in base64 encoded string</returns>
    public static string CreateAuthorizationString(string clientId, string clientSecret)
    {
        string authValue = $"{clientId}:{clientSecret}";
        return $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(authValue))}";
    }

    /// <summary>
    /// Check if a token is valid
    /// </summary>
    /// <param name="authToken">Token to check</param>
    /// <returns>'True' if valid token</returns>
    public static bool IsValidToken(AuthTokenExtended? authToken)
    {
        //Token is valid if it is not null and it expires in more than 10 seconds
        var isValid = authToken is not null && authToken?.ExpiresAt.Subtract(DateTime.UtcNow).TotalSeconds > 10;
        return isValid;
    }

    /// <summary>
    /// Convert a list of scopes to a string
    /// </summary>
    /// <param name="scopes">List of scopes</param>
    /// <returns>Scopes separated with spaces</returns>
    public static string CreateScopeString(IEnumerable<AuthenticationScope> scopes)
    {
        return string.Join(" ", scopes.Select(s =>
        {
            return s.GetEnumMemberValue();
        }));
    }

    public static string CreateScopeString(IEnumerable<string> scopes)
    {
        return string.Join(" ", scopes);
    }
}

internal static class EnumExtensions
{
    internal static string? GetEnumMemberValue<TEnum>(this TEnum enumValue) where TEnum : struct, IConvertible
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new ArgumentException("TEnum must be an enumerated type.");
        }

        // Get the member info for the provided enum value
        var memberInfos = typeof(TEnum).GetMember(enumValue.ToString() ?? "").FirstOrDefault();

        var enumMemberAttribute = memberInfos?.GetCustomAttribute<EnumMemberAttribute>();

        return enumMemberAttribute?.Value;

    }
}