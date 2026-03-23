# Parameters SDK for .NET

[![NuGet](https://img.shields.io/nuget/v/Adsk.Platform.Parameters)](https://www.nuget.org/packages/Adsk.Platform.Parameters)

> **Unofficial package** — not affiliated with or endorsed by Autodesk.
>
> **Namespace:** `Autodesk.Parameters` | **Target:** `net8.0` | **License:** MIT
> Generated from OpenAPI specs via [Microsoft Kiota](https://learn.microsoft.com/openapi/kiota/overview).

A type-safe C# SDK for the [Autodesk Parameters](https://aps.autodesk.com/en/docs/parameters/v1/overview/) REST API. Covers parameter groups, collections, parameter definitions, enumerations, specs, labels, and classification data — **35 methods across 7 service managers** through a single unified client.

The SDK provides two access patterns:

1. **Manager API** (recommended) — high-level methods with automatic pagination, strongly-typed parameters, and XML doc comments linking to official APS docs.
2. **Fluent URL API** — mirrors the REST endpoint structure directly for full control over requests.

## Installation

```bash
dotnet add package Adsk.Platform.Parameters
dotnet add package Adsk.Platform.Authentication
```

## Quick Start

```csharp
using Autodesk.Parameters;

var client = new ParametersClient(() => Task.FromResult("YOUR_ACCESS_TOKEN"));

// Manager approach (recommended) — auto-paginates all pages
await foreach (var collection in client.CollectionsManager.ListCollectionsAsync(accountId, groupId))
{
    Console.WriteLine($"{collection.Id}: {collection.Title}");
}

// Fluent URL approach — mirrors the REST path directly
var response = await client.Api.Parameters.V1.Accounts[accountId].Groups[groupId].Collections.GetAsync();
```

### Authentication with 2-Legged OAuth

For server-to-server communication, use the [`Adsk.Platform.Authentication`](https://www.nuget.org/packages/Adsk.Platform.Authentication) package:

```csharp
using Autodesk.Parameters;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;

var authClient = new AuthenticationClient();
var tokenStore = new InMemoryTokenStore();

var getAccessToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId: "YOUR_CLIENT_ID",
    clientSecret: "YOUR_CLIENT_SECRET",
    scopes: new[] { "data:read", "data:write" },
    tokenStore);

var client = new ParametersClient(getAccessToken);
```

### Dependency Injection

```csharp
using Autodesk.Common.HttpClientLibrary;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddAdskToolkitHttpClient("ApsClient");

// In your service:
public class MyService(IHttpClientFactory httpClientFactory)
{
    public ParametersClient CreateClient(Func<Task<string>> getAccessToken)
    {
        var httpClient = httpClientFactory.CreateClient("ApsClient");
        return new ParametersClient(getAccessToken, httpClient);
    }
}
```

## Available Managers

Every manager is a property on `ParametersClient`. Paginated endpoints return `IAsyncEnumerable<T>` (auto-fetches all pages); non-paginated endpoints return `Task<T?>`.

| Manager | Description | Methods |
| ------- | ----------- | ------: |
| `GroupsManager` | List, get, and update parameter groups | 3 |
| `CollectionsManager` | List, get, create, and update parameter collections | 4 |
| `ParametersManager` | CRUD, sharing, searching, rendering of parameters | 9 |
| `EnumerationsManager` | List, create, and update enumerations | 4 |
| `SpecsManager` | List, create, and update specs | 4 |
| `LabelsManager` | CRUD labels, attach/detach labels to parameters | 7 |
| `ClassificationsManager` | Classification groups, categories, disciplines, units | 4 |

## Automatic Pagination

Paginated Manager methods return `IAsyncEnumerable<T>`, transparently fetching every page. Use `break` or LINQ's `.Take(n)` to stop early without fetching unnecessary pages.

```csharp
// Iterate all collections across all pages
await foreach (var collection in client.CollectionsManager.ListCollectionsAsync(accountId, groupId))
{
    Console.WriteLine(collection.Title);
}

// Stop after first 10 items
int count = 0;
await foreach (var label in client.LabelsManager.ListLabelsAsync(accountId))
{
    if (++count >= 10) break;
}

// Apply query parameter filters
await foreach (var collection in client.CollectionsManager.ListCollectionsAsync(accountId, groupId,
    new() { QueryParameters = { Limit = 50 } }))
{
    Console.WriteLine(collection.Title);
}
```

## Usage Examples

### List Groups and Collections

```csharp
using Autodesk.Parameters;

var groups = await client.GroupsManager.ListGroupsAsync(accountId);
foreach (var group in groups?.Results ?? [])
{
    Console.WriteLine($"Group: {group.Id} — {group.Title}");

    await foreach (var collection in client.CollectionsManager.ListCollectionsAsync(accountId, group.Id))
    {
        Console.WriteLine($"  Collection: {collection.Id} — {collection.Title}");
    }
}
```

### Create Parameters

```csharp
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections.Item.Parameters;

var created = await client.ParametersManager.CreateParametersAsync(accountId, groupId, collectionId,
    new ParametersPostRequestBody
    {
        // Set parameter definition properties
    });
```

### Search Parameters

```csharp
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections.Item.ParametersSearch;

await foreach (var param in client.ParametersManager.SearchParametersAsync(accountId, groupId, collectionId,
    new ParametersSearchPostRequestBody()))
{
    Console.WriteLine($"{param.Id}: {param.Name}");
}
```

### Manage Labels

```csharp
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Labels;
using Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections.Item.LabelsAttach;

// Create labels
var newLabels = await client.LabelsManager.CreateLabelsAsync(accountId, new LabelsPostRequestBody
{
    // Set label properties
});

// Attach labels to parameters in a collection
var attached = await client.LabelsManager.AttachLabelsAsync(accountId, groupId, collectionId,
    new LabelsAttachPostRequestBody
    {
        // Set label IDs and parameter IDs
    });

// Delete a label (only if not attached to any parameter)
await client.LabelsManager.DeleteLabelAsync(accountId, "label-id");
```

### List Classification Data

```csharp
// List all disciplines
await foreach (var discipline in client.ClassificationsManager.ListDisciplinesAsync())
{
    Console.WriteLine($"{discipline.Id}: {discipline.Name}");
}

// List all units
await foreach (var unit in client.ClassificationsManager.ListUnitsAsync())
{
    Console.WriteLine($"{unit.Id}: {unit.Name}");
}
```

### Using the Fluent URL API

For full control or endpoints not covered by a Manager, use the `Api` property which mirrors the REST path structure:

```csharp
// GET /parameters/v1/accounts/{accountId}/groups
var groups = await client.Api.Parameters.V1.Accounts[accountId].Groups.GetAsync();

// GET /parameters/v1/specs
var specs = await client.Api.Parameters.V1.Specs.GetAsync();

// GET /parameters/v1/classifications/categories
var categories = await client.Api.Parameters.V1.Classifications.Categories.GetAsync();
```

## Fluent URL Shortcut Properties

These shortcut properties skip the version prefix for common endpoints:

| Property | Base Path |
| -------- | --------- |
| `client.Accounts` | `/parameters/v1/accounts/*` |
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
    var groups = await client.GroupsManager.ListGroupsAsync(accountId);
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

To disable the error handler for a specific request:

```csharp
using Autodesk.Common.HttpClientLibrary.Middleware.Options;

// Fluent URL API — uses Action<> lambda (Kiota-generated)
var specs = await client.Api.Parameters.V1.Specs.GetAsync(config =>
{
    config.Options.Add(new ErrorHandlerOption { Enabled = false });
});
```

## Custom HttpClient

You can provide your own `HttpClient` for advanced scenarios (proxies, custom handlers, etc.):

```csharp
var httpClient = new HttpClient();
var client = new ParametersClient(getAccessToken, httpClient);
```

## Constructor

```csharp
public ParametersClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
```

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| `getAccessToken` | `Func<Task<string>>` | Async function returning a valid OAuth bearer token |
| `httpClient` | `HttpClient?` | Optional custom HttpClient (default includes retry, rate-limit, and error handling middleware) |

## Conventions

These patterns are consistent across all 35 methods and are useful for AI code generation:

- All async methods use the `*Async` suffix
- Paginated endpoints return `IAsyncEnumerable<T>` — auto-fetches all pages; use `break` or `.Take(n)` to stop early
- Non-paginated endpoints return `Task<T?>`; void operations return `Task`
- Every method accepts optional `RequestConfiguration<T>? requestConfiguration` (object, not `Action<>`) and `CancellationToken cancellationToken` parameters
- `accountId` is `Guid` across all managers
- `groupId`, `collectionId`, `parameterId`, `labelId`, `enumerationId`, `specId` are all `string`
- Request body types are Kiota-generated classes in sub-namespaces (e.g. `Autodesk.Parameters.Parameters.V1.Accounts.Item.Groups.Item.Collections.Item.Parameters.ParametersPostRequestBody`)

## Related Packages

| Package | NuGet | Purpose |
| ------- | ----- | ------- |
| `Adsk.Platform.Authentication` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.Authentication) | OAuth 2-legged/3-legged token management |
| `Adsk.Platform.HttpClient` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.HttpClient) | Shared HTTP client with retry, rate limiting, error handling |
| `Adsk.Platform.ACC` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.ACC) | Autodesk Construction Cloud APIs |

## For AI Assistants

A machine-readable API reference with all 35 method signatures, return types, and REST endpoint mappings is available at [`llm.txt`](./llm.txt).

## Requirements

- .NET 8.0 or later
- Valid [Autodesk Platform Services (APS)](https://aps.autodesk.com/) access token with appropriate scopes

## Documentation

- [Parameters API Documentation](https://aps.autodesk.com/en/docs/parameters/v1/overview/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
