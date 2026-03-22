# Design Automation SDK for .NET

[![NuGet](https://img.shields.io/nuget/v/Adsk.Platform.Automation)](https://www.nuget.org/packages/Adsk.Platform.Automation)

> **Unofficial package** — not affiliated with or endorsed by Autodesk.
>
> **Namespace:** `Autodesk.Automation` | **Target:** `net8.0` | **License:** MIT
> Generated from OpenAPI specs via [Microsoft Kiota](https://learn.microsoft.com/openapi/kiota/overview).

A .NET SDK for the [Autodesk Design Automation API](https://aps.autodesk.com/en/docs/design-automation/v3/developers_guide/overview/) (v3). Provides **42 methods** across **8 managers** covering Activities, AppBundles, Engines, ForgeApps (nicknames), Health, Service Limits, Shares, and WorkItems. Paginated endpoints return `IAsyncEnumerable<T>` for seamless iteration with automatic page fetching.

The SDK provides two access patterns:

1. **Manager API** (recommended) — strongly-typed methods with auto-pagination, XML docs, and `RequestConfiguration<T>` support
2. **Fluent URL API** — mirrors the REST endpoint structure directly via `client.Api.Da.UsEast.V3.*`

## Installation

```bash
dotnet add package Adsk.Platform.Automation
dotnet add package Adsk.Platform.Authentication
```

## Quick Start

```csharp
using Autodesk.Automation;

var client = new AutomationClient(() => Task.FromResult("YOUR_ACCESS_TOKEN"));

// List all engines (auto-paginated)
await foreach (var engineId in client.EnginesManager.ListEnginesAsync())
{
    Console.WriteLine(engineId);
}

// Create a WorkItem
using Autodesk.Automation.Da.UsEast.V3.Workitems;

WorkitemsPostResponse? workItem = await client.WorkItemsManager.CreateWorkItemAsync(new WorkitemsPostRequestBody
{
    ActivityId = "MyNickname.MyActivity+MyAlias"
});
Console.WriteLine($"WorkItem created: {workItem?.Id} — Status: {workItem?.Status}");
```

### Authentication with 2-Legged OAuth

```csharp
using Autodesk.Automation;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;

var authClient = new AuthenticationClient();
var tokenStore = new InMemoryTokenStore();

Func<Task<string>> getAccessToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    "YOUR_CLIENT_ID",
    "YOUR_CLIENT_SECRET",
    new[] { "code:all" },
    tokenStore);

var client = new AutomationClient(getAccessToken);
```

### Dependency Injection

```csharp
using Autodesk.Common.HttpClientLibrary;

builder.Services.AddAdskToolkitHttpClient(
    "Automation",
    () => Task.FromResult("YOUR_ACCESS_TOKEN"),
    (getToken, httpClient) => new AutomationClient(getToken, httpClient));
```

## Available Managers

| Manager | Description | Methods |
| --- | --- | --- |
| `ActivitiesManager` | Activities, aliases, and versions | 13 |
| `AppBundlesManager` | AppBundles, aliases, and versions | 13 |
| `EnginesManager` | List and retrieve engine details | 2 |
| `ForgeAppsManager` | App nicknames and data deletion | 3 |
| `HealthManager` | Engine health status | 1 |
| `ServiceLimitsManager` | Service limit configuration | 2 |
| `SharesManager` | Shared AppBundles and Activities | 1 |
| `WorkItemsManager` | Create and monitor work items | 7 |

## Automatic Pagination

All list endpoints use token-based pagination. Manager methods that return `IAsyncEnumerable<T>` fetch all pages automatically — just iterate:

```csharp
// Iterate all activities across all pages
await foreach (var activityId in client.ActivitiesManager.ListActivitiesAsync())
{
    Console.WriteLine(activityId);
}

// Stop after the first 5 results
int count = 0;
await foreach (var engineId in client.EnginesManager.ListEnginesAsync())
{
    Console.WriteLine(engineId);
    if (++count >= 5) break;
}
```

## Usage Examples

### Create an Activity

```csharp
using Autodesk.Automation.Da.UsEast.V3.Activities;

ActivitiesPostResponse? activity = await client.ActivitiesManager.CreateActivityAsync(new ActivitiesPostRequestBody
{
    Id = "MyActivity",
    Engine = "Autodesk.AutoCAD+24",
    CommandLine = ["$(engine.path)\\accoreconsole.exe /i $(args[input].path) /s $(settings[script].path)"]
});
Console.WriteLine($"Activity created: {activity?.Id}");
```

### Create an Activity Alias

```csharp
using Autodesk.Automation.Da.UsEast.V3.Activities.Item.Aliases;

AliasesPostResponse? alias = await client.ActivitiesManager.CreateActivityAliasAsync("MyActivity", new AliasesPostRequestBody
{
    Id = "prod",
    Version = 1
});
```

### Create an AppBundle and Upload

```csharp
using Autodesk.Automation.Da.UsEast.V3.Appbundles;

AppbundlesPostResponse? bundle = await client.AppBundlesManager.CreateAppBundleAsync(new AppbundlesPostRequestBody
{
    Id = "MyAppBundle",
    Engine = "Autodesk.AutoCAD+24"
});

// Use bundle.UploadParameters to upload the package
string? uploadUrl = bundle?.UploadParameters?.EndpointURL;
```

### Get WorkItem Status

```csharp
using Autodesk.Automation.Da.UsEast.V3.Workitems.Item;

WorkitemsGetResponse? status = await client.WorkItemsManager.GetWorkItemStatusAsync("workitem-guid");
Console.WriteLine($"Status: {status?.Status}, Progress: {status?.Progress}");
```

### Update App Nickname

```csharp
using Autodesk.Automation.Da.UsEast.V3.Forgeapps.Item;

Stream? result = await client.ForgeAppsManager.UpdateNicknameAsync(new ForgeappsPatchRequestBody
{
    Nickname = "MyNickname"
});
```

### Using the Fluent URL API

```csharp
// List engines via fluent API
var engines = await client.Api.Da.UsEast.V3.Engines.GetAsync();

// Get a specific activity
var activity = await client.Api.Da.UsEast.V3.Activities["MyNickname.MyActivity+prod"].GetAsync();

// Create a WorkItem via fluent API
var workItem = await client.Api.Da.UsEast.V3.Workitems.PostAsync(new WorkitemsPostRequestBody
{
    ActivityId = "MyNickname.MyActivity+prod"
});
```

## Fluent URL Shortcut Properties

| Property | Base Path |
| --- | --- |
| `client.Activities` | `/da/us-east/v3/activities` |
| `client.AppBundles` | `/da/us-east/v3/appbundles` |
| `client.Engines` | `/da/us-east/v3/engines` |
| `client.ForgeApps` | `/da/us-east/v3/forgeapps` |
| `client.Health` | `/da/us-east/v3/health` |
| `client.ServiceLimits` | `/da/us-east/v3/servicelimits` |
| `client.Shares` | `/da/us-east/v3/shares` |
| `client.WorkItems` | `/da/us-east/v3/workitems` |

## Rate Limiting

The SDK handles API rate limits automatically via the built-in Kiota retry handler. When the API returns `429 Too Many Requests`, the SDK:

- Retries with exponential backoff
- Respects the `Retry-After` header
- Retries up to a configurable number of times before failing

No custom retry logic is needed in your application.

## Error Handling

By default, the SDK throws `HttpRequestException` for any non-2xx HTTP response. The exception includes the full response context for debugging:

```csharp
try
{
    var activity = await client.ActivitiesManager.GetActivityAsync("NonExistent.Activity+alias");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Request failed: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");

    if (ex.Data["context"] is HttpResponseMessage response)
    {
        Console.WriteLine($"Request URI: {response.RequestMessage?.RequestUri}");
        string body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response body: {body}");
    }
}
```

To disable the error handler for a specific Manager call, use `RequestConfiguration`:

```csharp
using Autodesk.Common.HttpClientLibrary.Middleware.Options;

var activity = await client.ActivitiesManager.GetActivityAsync("MyActivity+alias",
    new() { Options = { new ErrorHandlerOption { Enabled = false } } });
```

For Fluent URL API calls, the `Action<>` lambda is used (Kiota-generated):

```csharp
var engines = await client.Engines.GetAsync(config =>
{
    config.Options.Add(new ErrorHandlerOption { Enabled = false });
});
```

## Custom HTTP Client

```csharp
var httpClient = new HttpClient();
var client = new AutomationClient(() => Task.FromResult("TOKEN"), httpClient);
```

## Constructor

```csharp
public AutomationClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
```

| Parameter | Type | Description |
| --- | --- | --- |
| `getAccessToken` | `Func<Task<string>>` | Function that returns the access token |
| `httpClient` | `HttpClient?` | (Optional) Custom HTTP client instance |

## Conventions

These patterns apply to all Manager methods and are useful for AI code generation:

- **Naming**: `List*Async` (paginated), `Get*Async` (single), `Create*Async`, `Update*Async`, `Delete*Async`, `Cancel*Async`
- **Paginated methods** return `IAsyncEnumerable<T>` and iterate all pages automatically via token-based pagination
- **Non-paginated methods** return `Task<T?>` or `Task` (void)
- **All Manager methods** accept `RequestConfiguration<T>?` as an **object** (not `Action<>`). Configure via object initializer: `new() { QueryParameters = { ... } }`
- **Path parameters** are `string` for IDs and `int` for version numbers
- **Request body types** live in the namespace matching the endpoint path (e.g. `Autodesk.Automation.Da.UsEast.V3.Activities` for `ActivitiesPostRequestBody`)

## Related Packages

| Package | NuGet | Purpose |
| --- | --- | --- |
| `Adsk.Platform.Authentication` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.Authentication) | OAuth token management |
| `Adsk.Platform.HttpClient` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.HttpClient) | Shared HTTP client with retry and rate limiting |
| `Adsk.Platform.DataManagement` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.DataManagement) | Data Management API (OSS, DM) |
| `Adsk.Platform.ACC` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.ACC) | Autodesk Construction Cloud APIs |

## For AI Assistants

A machine-readable API reference with all method signatures, return types, and REST endpoint mappings is available at [`llm.txt`](./llm.txt).

## Requirements

- .NET 8.0 or later
- Valid [Autodesk Platform Services (APS)](https://aps.autodesk.com/) access token with `code:all` scope

## Documentation

- [Design Automation API Documentation](https://aps.autodesk.com/en/docs/design-automation/v3/developers_guide/overview/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
