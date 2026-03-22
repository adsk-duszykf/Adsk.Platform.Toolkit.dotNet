# Autodesk Tandem Digital Twin SDK for .NET

[![NuGet](https://img.shields.io/nuget/v/Adsk.Platform.Tandem)](https://www.nuget.org/packages/Adsk.Platform.Tandem)

> **Unofficial package** — not affiliated with or endorsed by Autodesk.
>
> **Namespace:** `Autodesk.Tandem` | **Target:** `net8.0` | **License:** MIT
> Generated from OpenAPI specs via [Microsoft Kiota](https://learn.microsoft.com/openapi/kiota/overview).

A type-safe C# SDK for the [Autodesk Tandem](https://aps.autodesk.com/en/docs/tandem/v1/developers_guide/overview/) Digital Twin REST APIs. Covers groups, twins (facilities), model data, streams/time series, documents, templates, and views — **31 methods across 8 service managers** through a single unified client.

The SDK provides two access patterns:

1. **Manager API** (recommended) — high-level methods with strongly-typed parameters and XML doc comments linking to official APS docs.
2. **Fluent URL API** — mirrors the REST endpoint structure directly for full control over requests.

## Installation

```bash
dotnet add package Adsk.Platform.Tandem
dotnet add package Adsk.Platform.Authentication
```

## Quick Start

```csharp
using Autodesk.Tandem;

var client = new TandemClient(() => Task.FromResult("YOUR_ACCESS_TOKEN"));

// Manager approach (recommended)
var groups = await client.GroupsManager.GetGroupsAsync();
var twin = await client.TwinsManager.GetTwinAsync("urn:adsk.dtt:...");

// Fluent URL approach — mirrors the REST path directly
var response = await client.Api.Tandem.V1.Groups.GetAsync();
```

### Authentication with 2-Legged OAuth

For server-to-server communication, use the [`Adsk.Platform.Authentication`](https://www.nuget.org/packages/Adsk.Platform.Authentication) package:

```csharp
using Autodesk.Tandem;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;

var authClient = new AuthenticationClient();
var tokenStore = new InMemoryTokenStore();

var getAccessToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId: "YOUR_CLIENT_ID",
    clientSecret: "YOUR_CLIENT_SECRET",
    scopes: new[] { "data:read", "data:write" },
    tokenStore);

var client = new TandemClient(getAccessToken);
```

### Dependency Injection

```csharp
using Autodesk.Common.HttpClientLibrary;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddAdskToolkitHttpClient("ApsClient");

// In your service:
public class MyService(IHttpClientFactory httpClientFactory)
{
    public TandemClient CreateClient(Func<Task<string>> getAccessToken)
    {
        var httpClient = httpClientFactory.CreateClient("ApsClient");
        return new TandemClient(getAccessToken, httpClient);
    }
}
```

## Available Managers

Every manager is a property on `TandemClient`. All endpoints return `Task<T?>` or `Task` (this API has no paginated endpoints).

| Manager | Description | Methods |
| ------- | ----------- | ------: |
| `TwinsManager` | Get, create, history, users, default model for twins (facilities) | 6 |
| `GroupsManager` | List, get, history, and user management for groups | 4 |
| `ModeldataManager` | Create, mutate, scan elements, get schema and history | 5 |
| `StreamsManager` | Time series data ingestion, retrieval, secrets, webhooks | 7 |
| `StreamConfigsManager` | List, get, update, and save stream configurations | 4 |
| `DocumentsManager` | Create, get, and delete twin documents | 3 |
| `TemplatesManager` | Get twin templates including classification | 1 |
| `ViewsManager` | Get saved views for a twin | 1 |

## Usage Examples

### List Groups and Get Twins

```csharp
using Autodesk.Tandem;

var client = new TandemClient(getAccessToken);

var groups = await client.GroupsManager.GetGroupsAsync();
var twins = await client.TwinsManager.GetTwinsByGroupAsync("urn:adsk.dtg:...");
```

### Scan Model Elements

```csharp
using Autodesk.Tandem.Tandem.V1.Modeldata.Item.Scan;

var elements = await client.ModeldataManager.ScanElementsAsync(
    "urn:adsk.dtm:...",
    new ScanPostRequestBody { Families = new List<string> { "std" } });
```

### Mutate Element Properties

```csharp
using Autodesk.Tandem.Tandem.V1.Modeldata.Item.Mutate;

var result = await client.ModeldataManager.MutateElementsAsync(
    "urn:adsk.dtm:...",
    new MutatePostRequestBody());
```

### Work with Time Series Data

```csharp
// Get latest stream data
using Autodesk.Tandem.Tandem.V1.Timeseries.ModelsRequests.Item.Streams;

var latest = await client.StreamsManager.GetLatestStreamsDataAsync(
    "urn:adsk.dtm:...",
    new StreamsPostRequestBody { Keys = new List<string> { "streamKey1" } });

// Get historical data for a specific stream element
var history = await client.StreamsManager.GetStreamDataAsync(
    "urn:adsk.dtm:...",
    "elementId");
```

### Manage Documents

```csharp
using Autodesk.Tandem.Tandem.V1.Twins.Item.Documents;

var doc = await client.DocumentsManager.CreateDocumentAsync(
    "urn:adsk.dtt:...",
    new DocumentsPostRequestBody { Name = "floorplan.pdf" });

await client.DocumentsManager.DeleteDocumentAsync("urn:adsk.dtt:...", "docId");
```

### Using the Fluent URL API

For full control or endpoints not covered by a Manager, use the `Api` property which mirrors the REST path structure:

```csharp
// GET /tandem/v1/groups
var groups = await client.Api.Tandem.V1.Groups.GetAsync();

// GET /tandem/v1/twins/{twinID}
var twin = await client.Api.Tandem.V1.Twins["urn:adsk.dtt:..."].GetAsync();

// GET /tandem/v1/modeldata/{modelID}/schema
var schema = await client.Api.Tandem.V1.Modeldata["urn:adsk.dtm:..."].Schema.GetAsync();
```

## Fluent URL Shortcut Properties

These shortcut properties skip the version prefix for common endpoints:

| Property | Base Path |
| -------- | --------- |
| `client.Groups` | `/tandem/v1/groups/*` |
| `client.Modeldata` | `/tandem/v1/modeldata/*` |
| `client.Models` | `/tandem/v1/models/*` |
| `client.Timeseries` | `/tandem/v1/timeseries/*` |
| `client.Twins` | `/tandem/v1/twins/*` |
| `client.Api` | Full base client (all paths) |

## Rate Limiting

The SDK handles API rate limits automatically. When the API returns a `429 Too Many Requests` response, the SDK will:

- Automatically retry the request with exponential backoff
- Respect the `Retry-After` header returned by the API
- Retry up to a configurable number of times before failing

No custom retry logic needed — transient failures and rate limiting are handled transparently by the built-in [Kiota HTTP middleware](https://learn.microsoft.com/openapi/kiota/middleware).

## Error Handling

By default, the SDK throws `HttpRequestException` for any non-success HTTP response (4xx/5xx). This is enabled by default — unlike Kiota's default behavior which requires manual status checking.

The exception includes the status code and the full `HttpResponseMessage` in `ex.Data["context"]`:

```csharp
try
{
    var twin = await client.TwinsManager.GetTwinAsync("urn:adsk.dtt:...");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Status: {ex.StatusCode} — {ex.Message}");

    if (ex.Data["context"] is HttpResponseMessage response)
    {
        Console.WriteLine($"URI: {response.RequestMessage?.RequestUri}");
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Body: {body}");
    }
}
```

To disable the error handler for a specific Manager method request:

```csharp
using Autodesk.Common.HttpClientLibrary.Middleware.Options;

var twin = await client.TwinsManager.GetTwinAsync("urn:adsk.dtt:...",
    new() { Options = { new ErrorHandlerOption { Enabled = false } } });
```

To disable for a Fluent URL API request:

```csharp
using Autodesk.Common.HttpClientLibrary.Middleware.Options;

var twin = await client.Twins["urn:adsk.dtt:..."].GetAsync(config =>
{
    config.Options.Add(new ErrorHandlerOption { Enabled = false });
});
```

## Custom HTTP Client

You can provide your own `HttpClient` for advanced scenarios (proxies, custom handlers, etc.):

```csharp
var httpClient = new HttpClient();
var client = new TandemClient(getAccessToken, httpClient);
```

## Constructor

```csharp
public TandemClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
```

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| `getAccessToken` | `Func<Task<string>>` | Async function returning a valid OAuth bearer token |
| `httpClient` | `HttpClient?` | Optional custom HttpClient (default includes retry, rate-limit, and error handling middleware) |

## Conventions

These patterns are consistent across all 31 methods and are useful for AI code generation:

- All async methods use the `*Async` suffix
- All endpoints return `Task<T?>` or `Task` (no paginated endpoints in this API)
- Every method accepts optional `RequestConfiguration<T>? requestConfiguration` (object, not `Action<>`) and `CancellationToken cancellationToken` parameters
- `groupId`, `twinId`, `modelId`, `elementId`, `userId`, `documentId` are all `string`
- Request body types are Kiota-generated classes in sub-namespaces (e.g. `Autodesk.Tandem.Tandem.V1.Modeldata.Item.Scan.ScanPostRequestBody`)

## Related Packages

| Package | NuGet | Purpose |
| ------- | ----- | ------- |
| `Adsk.Platform.Authentication` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.Authentication) | OAuth 2-legged/3-legged token management |
| `Adsk.Platform.HttpClient` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.HttpClient) | Shared HTTP client with retry, rate limiting, error handling |
| `Adsk.Platform.DataManagement` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.DataManagement) | Hubs, projects, folders, items, versions |

## For AI Assistants

A machine-readable API reference with all 31 method signatures, return types, and REST endpoint mappings is available at [`llm.txt`](./llm.txt).

## Requirements

- .NET 8.0 or later
- Valid [Autodesk Platform Services (APS)](https://aps.autodesk.com/) access token with appropriate scopes

## Documentation

- [Tandem API Documentation](https://aps.autodesk.com/en/docs/tandem/v1/developers_guide/overview/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
