# Autodesk Tandem SDK for .NET

> **⚠️ UNOFFICIAL PACKAGE ⚠️**

A .NET SDK providing a [Fluent API](https://dzone.com/articles/java-fluent-api) for the [Autodesk Tandem](https://aps.autodesk.com/en/docs/tandem/v1/overview/) APIs, generated from the official OpenAPI specifications using [Microsoft Kiota](https://learn.microsoft.com/en-us/openapi/kiota/overview).

## Features

This SDK provides access to Tandem API endpoints through a unified client:

| API | Endpoint Path |
|-----|---------------|
| **Groups** | `/tandem/v1/groups/*` |
| **Modeldata** | `/tandem/v1/modeldata/*` |
| **Models** | `/tandem/v1/models/*` |
| **Timeseries** | `/tandem/v1/timeseries/*` |
| **Twins** | `/tandem/v1/twins/*` |

## Installation

```bash
dotnet add package Adsk.Platform.Tandem
```

## Quick Start

```csharp
using Autodesk.Tandem;

// Provide a function that returns the access token
Func<Task<string>> getAccessToken = () => Task.FromResult("YOUR_ACCESS_TOKEN");

// Initialize the Tandem client
var tandemClient = new TandemClient(getAccessToken);
```

### Using with 2-Legged Authentication

For server-to-server communication using client credentials (2-legged OAuth), use the `Autodesk.Authentication` package:

```csharp
using Autodesk.Tandem;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;

// Your APS app credentials
var clientId = "YOUR_CLIENT_ID";
var clientSecret = "YOUR_CLIENT_SECRET";

// Define the required scopes
var scopes = new[] { "data:read", "data:write" };

// Create authentication client
var authClient = new AuthenticationClient();

// Create an auto-refreshing token provider (handles token expiration automatically)
var tokenStore = new InMemoryTokenStore();
var getAccessToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId, 
    clientSecret, 
    scopes, 
    tokenStore);

// Initialize the Tandem client with auto-refreshing token
var tandemClient = new TandemClient(getAccessToken);
```

## Usage Examples

### Get Twins

```csharp
// Get all twins (facilities) accessible to the user
var twins = await tandemClient.Twins.GetAsync();
```

### Get Models

```csharp
// Get models for a twin
var models = await tandemClient.Models[twinId].GetAsync();
```

### Using the Full API

For endpoints not available through shortcuts, use the `Api` property to access the full API structure:

```csharp
// Access the full API
var result = await tandemClient.Api.Tandem.V1.Twins.GetAsync();
```

## API Structure

The SDK provides convenient shortcut properties for common endpoints:

| Property | Description |
|----------|-------------|
| `tandemClient.Groups` | Facility groups management APIs |
| `tandemClient.Modeldata` | Model data query APIs |
| `tandemClient.Models` | Models management APIs |
| `tandemClient.Timeseries` | Time series data APIs |
| `tandemClient.Twins` | Digital twin (facility) management APIs |

## Custom HttpClient

You can provide your own `HttpClient` instance for advanced scenarios:

```csharp
var httpClient = new HttpClient();
// Configure your HttpClient...

var tandemClient = new TandemClient(getAccessToken, httpClient);
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
    var twins = await tandemClient.Twins.GetAsync();
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

var twins = await tandemClient.Twins.GetAsync(requestConfig);
```

## Requirements

- .NET 8.0 or later
- Valid Autodesk Platform Services (APS) access token with appropriate scopes

## Documentation

- [Tandem API Documentation](https://aps.autodesk.com/en/docs/tandem/v1/developers_guide/overview/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/en-us/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
