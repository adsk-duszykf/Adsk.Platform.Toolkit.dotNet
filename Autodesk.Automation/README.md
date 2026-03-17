# Autodesk Automation SDK for .NET

> **⚠️ UNOFFICIAL PACKAGE ⚠️**

A .NET SDK providing a [Fluent API](https://dzone.com/articles/java-fluent-api) for the [Autodesk  Automation API](https://aps.autodesk.com/en/docs/design-automation/v3/developers_guide/overview/), generated  with [Microsoft Kiota](https://learn.microsoft.com/en-us/openapi/kiota/overview).

## Features

This SDK provides access to `Automation API` endpoints through a unified client:

| API | Endpoint Path |
|-----|---------------|
| **Activities** | `/da/us-east/v3/activities/*` |
| **AppBundles** | `/da/us-east/v3/appbundles/*` |
| **Engines** | `/da/us-east/v3/engines/*` |
| **ForgeApps** | `/da/us-east/v3/forgeapps/*` |
| **Health** | `/da/us-east/v3/health/*` |
| **ServiceLimits** | `/da/us-east/v3/servicelimits/*` |
| **Shares** | `/da/us-east/v3/shares/*` |
| **WorkItems** | `/da/us-east/v3/workitems/*` |

## Installation

```bash
dotnet add package Adsk.Platform.Automation
```

## Quick Start

```csharp
using Autodesk.Automation;

// Provide a function that returns the access token
Func<Task<string>> getAccessToken = () => Task.FromResult("YOUR_ACCESS_TOKEN");

// Initialize the Automation client
var automationClient = new AutomationClient(getAccessToken);
```

### Using with 2-Legged Authentication

For server-to-server communication using client credentials (2-legged OAuth), use the `Autodesk.Authentication` package:

```csharp
using Autodesk.Automation;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;

// Your APS app credentials
var clientId = "YOUR_CLIENT_ID";
var clientSecret = "YOUR_CLIENT_SECRET";

// Define the required scopes
var scopes = new[] { "code:all" };

// Create authentication client
var authClient = new AuthenticationClient();

// Create an auto-refreshing token provider (handles token expiration automatically)
var tokenStore = new InMemoryTokenStore();
var getAccessToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId, 
    clientSecret, 
    scopes, 
    tokenStore);

// Initialize the Automation client with auto-refreshing token
var automationClient = new AutomationClient(getAccessToken);

// Now you can use the client - tokens are refreshed automatically when needed
var engines = await automationClient.Engines.GetAsync();
```

## Usage Examples

### List Engines

```csharp
// Get available engines
var engines = await automationClient.Engines.GetAsync();
```

### List Activities

```csharp
// Get all activities
var activities = await automationClient.Activities.GetAsync();
```

### List AppBundles

```csharp
// Get all app bundles
var appBundles = await automationClient.AppBundles.GetAsync();
```

### Create a WorkItem

```csharp
// Submit a work item
var workItem = await automationClient.WorkItems.PostAsync(new()
{
    // Configure your work item
});
```

### Check Service Health

```csharp
// Check Design Automation service health
var health = await automationClient.Health.GetAsync();
```

### Using the Full API

For endpoints not available through shortcuts, use the `Api` property to access the full API structure:

```csharp
// Access the full API
var result = await automationClient.Api.Da.UsEast.V3.Engines.GetAsync();
```

## API Structure

The SDK provides convenient shortcut properties for common endpoints:

| Property | Description |
|----------|-------------|
| `automationClient.Activities` | Activity management APIs |
| `automationClient.AppBundles` | AppBundle management APIs |
| `automationClient.Engines` | Engine listing APIs |
| `automationClient.ForgeApps` | ForgeApp/Nickname APIs |
| `automationClient.Health` | Service health check APIs |
| `automationClient.ServiceLimits` | Service limits APIs |
| `automationClient.Shares` | Shared resource APIs |
| `automationClient.WorkItems` | WorkItem management APIs |

## Custom HttpClient

You can provide your own `HttpClient` instance for advanced scenarios:

```csharp
var httpClient = new HttpClient();
// Configure your HttpClient...

var automationClient = new AutomationClient(getAccessToken, httpClient);
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
    var engines = await automationClient.Engines.GetAsync();
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

var engines = await automationClient.Engines.GetAsync(requestConfig);
```

## Requirements

- .NET 8.0 or later
- Valid Autodesk Platform Services (APS) access token with appropriate scopes

## Documentation

- [Design Automation API Documentation](https://aps.autodesk.com/en/docs/design-automation/v3/developers_guide/overview/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/en-us/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
