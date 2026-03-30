using Autodesk.Authentication.Helpers;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace Autodesk.Authentication;
/// <summary>
/// Client for the Autodesk Authentication API providing both low-level API access and high-level helpers.
/// </summary>
public class AuthenticationClient
{
    /// <summary>
    /// Initializes a new instance of <see cref="AuthenticationClient"/>.
    /// </summary>
    /// <param name="httpClient">Optional pre-configured <see cref="HttpClient"/>. When <c>null</c>, a default client is created.</param>
    public AuthenticationClient(HttpClient? httpClient = null)
    {
        httpClient ??= Autodesk.Common.HttpClientLibrary.HttpClientFactory.Create();

        var adapter = new HttpClientRequestAdapter(
            authenticationProvider: new AnonymousAuthenticationProvider(),
            parseNodeFactory: null,
            serializationWriterFactory: null,
            httpClient: httpClient);

        Api = new BaseAuthenticationClient(adapter);
        Helper = new AuthenticationClientHelper(Api, httpClient);
    }

    /// <summary>
    /// Low-level, Kiota-generated request builders for the Authentication API.
    /// </summary>
    public BaseAuthenticationClient Api { get; private set; }
    /// <summary>
    /// High-level helper methods for common authentication workflows.
    /// </summary>
    public AuthenticationClientHelper Helper { get; private set; }

}
