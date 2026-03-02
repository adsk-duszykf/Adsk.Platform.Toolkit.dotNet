# Adsk.Platform.Authentication

> **⚠️UNOFFICIAL PACKAGE⚠️**

The `Adsk.Platform.DataManagement` toolkit provides a set of APIs to interact with the [Autodesk Authentication Service](https://aps.autodesk.com/developer/overview/authentication-api).

## Documentation

More information can be found [here](https://adsk-duszykf.github.io/Adsk.Platform.Toolkit/docs/Authentication/README.html).


## Installation

```bash
dotnet add package Adsk.Platform.Authentication
```

## Usage

The root object is [`AuthenticationClient`](xref:Autodesk.Authentication.AuthenticationClient). This object provides access to the `Authentication` API and the `Helpers` method.

### 2 legged authentication

Getting a 2-legged access token is a common scenario. The SDK provides a helper class to simplify:

````csharp
// Create a new instance of the AuthenticationClient
var authClient = new AuthenticationClient();

// Use helpers to get a 2-legged access token
 AuthTokenExtended authToken = await authClient.Helper.GetTwoLeggedToken(
	APS_CLIENT_ID, 
	APS_CLIENT_SECRET, 
	[AuthenticationScopeDefaults.DataWrite, AuthenticationScopeDefaults.DataRead]);

````

Here is an example of how to get the hub list:

````csharp
using Autodesk.DataManagement;
using Autodesk.Authentication;

public async Task<Hubs> GetHub()
{

    var APS_CLIENT_ID="abcd"; // Replace with your client id
    var APS_CLIENT_SECRET="1234"; // Replace with your client secret

    AuthenticationClient authClient = new();

    async Task<string> getAccessToken()
    {
        var token = await authClient.Helper.GetTwoLeggedToken(APS_CLIENT_ID, APS_CLIENT_SECRET, [ AuthenticationScopeDefaults.DataWrite]);

        return token?.AccessToken is null ? throw new InvalidOperationException() : token.AccessToken;
    }

    var DMclient = new DataManagementClient(getAccessToken);

    var hubs = await DMclient.DataMgtApi.Project.V1.Hubs.GetAsync();

    return hubs;
}
````

### 3 legged authentication

This SDK depends on `AspNet.Security.OAuth.Autodesk` that leverages the ASP.NET Core authentication middleware to authenticate users with Autodesk.

1. In the `program.cs` file, add the authentication middleware:

    ````csharp
    using Microsoft.AspNetCore.Authentication.Cookies;

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();

    builder.Services.AddControllers();

    //Define the default authentication scheme
    // The cookie contains the paths to your sign in and sign out endpoints
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/signin";
        options.LogoutPath = "/signout";
    });

    // Add Autodesk authentication
    builder.Services.AddAuthentication().AddAutodesk(options =>
    {
        options.ClientId = builder.Configuration["AUTODESK_CLIENT_ID"] ?? throw new ArgumentException("'AUTODESK_CLIENT_ID' is undefined");
        options.ClientSecret = builder.Configuration["AUTODESK_CLIENT_SECRET"] ?? throw new ArgumentException("'AUTODESK_CLIENT_SECRET' is undefined");
        options.Scope.Add("data:read");
        options.CallbackPath = "/signin-autodesk";
        options.SaveTokens = true; // Save the access and refresh tokens that will accessible in the HttpContext 

    });

    var app = builder.Build();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    // Configure the HTTP request pipeline.
    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapFallbackToFile("/index.html");

    app.Run();

    ````

2. Create a controller, handling `SignIn` and `SignOut`. In your controller `test.cs`:

    ````csharp
    [ApiController]
    [Route("api/[controller]")]
    public class Test() : ControllerBase
    {
        private HttpClient _client = new();

        [HttpGet("~/signin")]
        public IActionResult SignIn()
        {
            // Instruct the Autodesk authentication middleware to redirect the user agent to the Autodesk login page
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, AutodeskAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("~/signout")]
        public IActionResult SignOutCurrentUser()
        {
            // Instruct the cookies middleware to delete the local cookie created
            // when the user agent is redirected from the external identity provider
            // after a successful authentication flow.
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("authToken")]
        [Authorize]
        //This endpoint is protected by the Autodesk authentication middleware and requires a valid access token (`Authorize` attribute)
        public async Task<IActionResult> GetTokenAsync()
        {
            // The middleware stores the access token in the HttpContext
            // Ensure that  `options.SaveTokens = true` is set in the `AddAutodesk`method in the `program.cs`file (see step 1)
            var accessToken = await HttpContext.GetTokenAsync(AutodeskAuthenticationDefaults.AuthenticationScheme, "access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("No access token available.");
            }

            return Ok(accessToken);
        }
    }

    ````

    The `{root}/signin` endpoint will redirect the user to the Autodesk login page. Once the user is authenticated, the user will be redirected to the `{root}/` endpoint.

    Then you can use the `{root}/api/test/authToken` endpoint to get the access token.

## Limitations

### Get User info

The endpoint to get user info is only available in the `Helper`. Example:

````csharp
var authClient = new AuthenticationClient();

var userInfo=authClient.Helper.GetUserInfoAsync("your three legged token");

````

## Advanced

### 2 legged authentication

The code below shows how to get a 2-legged access token without using the `Helper` class.

````csharp

var body = new TokenPostRequestBody()
{
    GrantType = Granttype.Client_credentials,
    Scope = "data:read data:write"
};

var authString = AuthenticationClientHelper.CreateAuthorizationString(APS_CLIENT_ID, APS_CLIENT_SECRET);

AuthToken authToken2 = await authClient.Authentication.V2.Token.PostAsync(body, r =>
{
    r.Headers.Add("Authorization", authString);
});

````
