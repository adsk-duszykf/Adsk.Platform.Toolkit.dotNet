# Autodesk.BIM360 - BIM 360 SDK

A .NET SDK providing a [Fluent API](https://dzone.com/articles/java-fluent-api) for the [Autodesk BIM 360](https://aps.autodesk.com/en/docs/bim360/v1/overview/) APIs, generated from the official OpenAPI specifications using [Microsoft Kiota](https://learn.microsoft.com/en-us/openapi/kiota/overview).

## Features

This SDK provides access to multiple BIM 360 API endpoints through a unified client:

| API | Endpoint Path |
|-----|---------------|
| **Accounts** | `/hq/v1/accounts/*` |
| **Accounts EU (v1)** | `/hq/v1/regions/eu/accounts/*` |
| **Accounts EU (v2)** | `/hq/v2/regions/eu/accounts/*` |
| **Admin** | `/construction/admin/v1/*` |
| **Assets** | `/bim360/assets/v1/*` |
| **Checklists** | `/bim360/checklists/v1/*` |
| **Clash** | `/bim360/clash/v3/*` |
| **Cost** | `/cost/v1/*` |
| **Data Connector** | `/dataconnector/v1/*` |
| **Docs** | `/bim360/docs/v1/*` |
| **Index** | `/construction/index/v2/*` |
| **Issues** | `/issues/v2/*` |
| **ModelSet** | `/bim360/modelset/v3/*` |
| **Projects** | `/bim360/relationship/v2/*` |
| **Relationships** | `/bim360/relationship/v2/*` |
| **RFIs** | `/bim360/rfis/v2/*` |

## Installation

```bash
dotnet add package Adsk.Platform.BIM360
```

## Quick Start

```csharp
using Autodesk.BIM360;

// Provide a function that returns the access token
Func<Task<string>> getAccessToken = () => Task.FromResult("YOUR_ACCESS_TOKEN");

// Initialize the BIM 360 client
var bim360Client = new BIM360client(getAccessToken);
```

### Using with 2-Legged Authentication

For server-to-server communication using client credentials (2-legged OAuth), use the `Autodesk.Authentication` package:

```csharp
using Autodesk.BIM360;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;

// Your APS app credentials
var clientId = "YOUR_CLIENT_ID";
var clientSecret = "YOUR_CLIENT_SECRET";

// Define the required scopes
var scopes = new[] { "data:read", "data:write", "account:read" };

// Create authentication client
var authClient = new AuthenticationClient();

// Create an auto-refreshing token provider (handles token expiration automatically)
var tokenStore = new InMemoryTokenStore();
var getAccessToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId, 
    clientSecret, 
    scopes, 
    tokenStore);

// Initialize the BIM 360 client with auto-refreshing token
var bim360Client = new BIM360client(getAccessToken);

// Now you can use the client - tokens are refreshed automatically when needed
var issues = await bim360Client.Issues.Containers[containerId].QualityIssues.GetAsync();
```

## Usage Examples

### Get Issues

```csharp
// Get quality issues for a container
var issues = await bim360Client.Issues.Containers[containerId].QualityIssues.GetAsync();

foreach (var issue in issues?.Data ?? [])
{
    Console.WriteLine($"Issue: {issue.Attributes?.Title} - Status: {issue.Attributes?.Status}");
}
```

### Get Clash Results

```csharp
// Get clash test results
var clashTests = await bim360Client.Clash.Containers[containerId].Clash.Tests.GetAsync();
```

### Get RFIs

```csharp
// Get RFIs for a container
var rfis = await bim360Client.RFIs.Containers[containerId].Rfis.GetAsync();
```

### Get Checklists

```csharp
// Get checklists for a container
var checklists = await bim360Client.Checklists.Containers[containerId].Checklists.GetAsync();
```

### Using the Full API

For endpoints not available through shortcuts, use the `Api` property to access the full API structure:

```csharp
// Access the full API
var result = await bim360Client.Api.Issues.V2.Containers[containerId].QualityIssues.GetAsync();
```

## API Structure

The SDK provides convenient shortcut properties for common endpoints:

| Property | Description |
|----------|-------------|
| `bim360Client.Accounts` | Account management APIs |
| `bim360Client.AccountsEU_V1` | EU region account APIs (v1) |
| `bim360Client.AccountsEU_V2` | EU region account APIs (v2) |
| `bim360Client.Admin` | Admin APIs |
| `bim360Client.Assets` | Assets management APIs |
| `bim360Client.Checklists` | Checklists APIs |
| `bim360Client.Clash` | Clash detection APIs |
| `bim360Client.Cost` | Cost management APIs |
| `bim360Client.DataConnector` | Data Connector APIs |
| `bim360Client.Docs` | Document management APIs |
| `bim360Client.Index` | Index/search APIs |
| `bim360Client.Issues` | Issues management APIs |
| `bim360Client.ModelSet` | Model set management APIs |
| `bim360Client.Projects` | Project relationship APIs |
| `bim360Client.Relationships` | Relationships APIs |
| `bim360Client.RFIs` | RFI management APIs |

## Custom HttpClient

You can provide your own `HttpClient` instance for advanced scenarios:

```csharp
var httpClient = new HttpClient();
// Configure your HttpClient...

var bim360Client = new BIM360client(getAccessToken, httpClient);
```

## Rate Limiting

The SDK handles API rate limits automatically thanks to the built-in retry handler provided by the [Kiota HTTP client](https://learn.microsoft.com/en-us/openapi/kiota/middleware). When the API returns a `429 Too Many Requests` response, the SDK will:

- Automatically retry the request with exponential backoff
- Respect the `Retry-After` header returned by the API
- Retry up to a configurable number of times before failing

This means you don't need to implement custom retry logic in your application — the SDK handles transient failures and rate limiting transparently.

## Error Handling

By default, the SDK throws an `HttpRequestException` for any non-successful HTTP response (4xx or 5xx status codes). This differs from Kiota's default behavior, which requires you to check the response status manually.

The exception includes:
- The request URI
- The HTTP status code
- The full `HttpResponseMessage` in the `Data["context"]` property, allowing you to inspect the request, headers, response body, and other details for debugging

```csharp
try
{
    var issues = await bim360Client.Issues.Containers[containerId].QualityIssues.GetAsync();
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Request failed: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");

    // Access the full response for more details
    if (ex.Data["context"] is HttpResponseMessage response)
    {
        // Get request details
        Console.WriteLine($"Request URI: {response.RequestMessage?.RequestUri}");
        Console.WriteLine($"Request Method: {response.RequestMessage?.Method}");

        // Get response body
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response body: {body}");
    }
}
```

If you prefer Kiota's default behavior (no automatic exception throwing), you can disable the error handler:

```csharp
using Autodesk.Common.HttpClientLibrary.Middleware.Options;

// Disable error handling for a specific request
var requestConfig = new Action<RequestConfiguration<DefaultQueryParameters>>(config =>
{
    config.Options.Add(new ErrorHandlerOption { Enabled = false });
});

var issues = await bim360Client.Issues.Containers[containerId].QualityIssues.GetAsync(requestConfig);
```

## Requirements

- .NET 8.0 or later
- Valid Autodesk Platform Services (APS) access token with appropriate scopes

## Documentation

- [BIM 360 API Documentation](https://aps.autodesk.com/en/docs/bim360/v1/overview/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/en-us/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
