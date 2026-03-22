# Autodesk Construction Cloud SDK for .NET

[![NuGet](https://img.shields.io/nuget/v/Adsk.Platform.ACC)](https://www.nuget.org/packages/Adsk.Platform.ACC)

> **Unofficial package** — not affiliated with or endorsed by Autodesk.
>
> **Namespace:** `Autodesk.ACC` | **Target:** `net8.0` | **License:** MIT
> Generated from OpenAPI specs via [Microsoft Kiota](https://learn.microsoft.com/openapi/kiota/overview).

A type-safe C# SDK for the [Autodesk Construction Cloud (ACC)](https://aps.autodesk.com/en/docs/acc/v1/overview/) REST APIs. Covers account admin, issues, cost management, RFIs, submittals, model coordination, sheets, and more — **341 methods across 19 service managers** through a single unified client.

The SDK provides two access patterns:

1. **Manager API** (recommended) — high-level methods with automatic pagination, strongly-typed parameters, and XML doc comments linking to official APS docs.
2. **Fluent URL API** — mirrors the REST endpoint structure directly for full control over requests.

## Installation

```bash
dotnet add package Adsk.Platform.ACC
dotnet add package Adsk.Platform.Authentication
```

## Quick Start

```csharp
using Autodesk.ACC;

var client = new ACCclient(() => Task.FromResult("YOUR_ACCESS_TOKEN"));

// Manager approach (recommended) — auto-paginates all pages
await foreach (var issue in client.IssuesManager.ListIssuesAsync(projectId))
{
    Console.WriteLine($"{issue.Title} — {issue.Status}");
}

// Fluent URL approach — mirrors the REST path directly
var response = await client.Api.Construction.Issues.V1.Projects[projectId].Issues.GetAsync();
```

### Authentication with 2-Legged OAuth

For server-to-server communication, use the [`Adsk.Platform.Authentication`](https://www.nuget.org/packages/Adsk.Platform.Authentication) package:

```csharp
using Autodesk.ACC;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;

var authClient = new AuthenticationClient();
var tokenStore = new InMemoryTokenStore();

var getAccessToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId: "YOUR_CLIENT_ID",
    clientSecret: "YOUR_CLIENT_SECRET",
    scopes: new[] { "data:read", "data:write", "account:read" },
    tokenStore);

var client = new ACCclient(getAccessToken);
```

### Dependency Injection

```csharp
using Autodesk.Common.HttpClientLibrary;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddAdskToolkitHttpClient("ApsClient");

// In your service:
public class MyService(IHttpClientFactory httpClientFactory)
{
    public ACCclient CreateClient(Func<Task<string>> getAccessToken)
    {
        var httpClient = httpClientFactory.CreateClient("ApsClient");
        return new ACCclient(getAccessToken, httpClient);
    }
}
```

## Available Managers

Every manager is a property on `ACCclient`. Paginated endpoints return `IAsyncEnumerable<T>` (auto-fetches all pages); non-paginated endpoints return `Task<T?>`.

| Manager | Description | Methods |
| ------- | ----------- | ------: |
| `AccountAdminManager` | Accounts, projects, users, companies, business units | 29 |
| `AssetsManager` | Asset records, categories, statuses, custom attributes | 24 |
| `AutoSpecsManager` | Metadata, smart register, requirements, submittals summary | 4 |
| `CostManager` | Budgets, contracts, change orders, expenses, payments | 63 |
| `DataConnectorManager` | Data extraction requests, jobs, downloads | 11 |
| `FileManagementManager` | PDF exports, permissions, custom attributes, naming standards | 14 |
| `FormsManager` | Form templates, forms, form values | 5 |
| `IssuesManager` | Issues, types, comments, attachments, root causes | 14 |
| `LocationsManager` | Location nodes and trees | 4 |
| `ModelCoordinationManager` | Model sets, clash tests, clashes, screenshots | 43 |
| `ModelPropertiesManager` | Property indexes, diffs, queries | 16 |
| `PhotosManager` | Get and search photos | 2 |
| `RelationshipsManager` | Create, search, sync relationships | 9 |
| `ReviewsManager` | Workflows, reviews, progress, approvals | 10 |
| `RFIsManager` | RFIs, types, attributes, responses, comments | 16 |
| `SheetsManager` | Sheets, version sets, uploads, collections, exports | 26 |
| `SubmittalsManager` | Submittal items, packages, specs, reviews, tasks | 29 |
| `TakeoffManager` | Packages, items, types, classification systems | 17 |
| `TransmittalsManager` | Transmittals, recipients, folders, documents | 5 |

## Automatic Pagination

Paginated Manager methods return `IAsyncEnumerable<T>`, transparently fetching every page. Use `break` or LINQ's `.Take(n)` to stop early without fetching unnecessary pages.

```csharp
// Iterate all issues across all pages
await foreach (var issue in client.IssuesManager.ListIssuesAsync(projectId))
{
    Console.WriteLine(issue.Title);
}

// Stop after first 10 items
int count = 0;
await foreach (var item in client.SubmittalsManager.ListItemsAsync(projectId))
{
    if (++count >= 10) break;
}

// Apply query parameter filters
await foreach (var user in client.AccountAdminManager.ListProjectUsersAsync(projectId,
    new() { QueryParameters = { Sort = ["name"] } }))
{
    Console.WriteLine(user.Name);
}
```

## Usage Examples

### List Projects

```csharp
await foreach (var project in client.AccountAdminManager.ListProjectsAsync(accountId))
{
    Console.WriteLine($"{project.Name} — {project.Status}");
}
```

### Create an Issue

```csharp
using Autodesk.ACC.Construction.Issues.V1.Projects.Item.Issues;

var newIssue = await client.IssuesManager.CreateIssueAsync(projectId, new IssuesPostRequestBody
{
    Title = "Missing fire extinguisher on Level 3",
    Description = "Fire safety requirement not met",
});
```

### Search RFIs and Add a Response

```csharp
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.SearchRfis;
using Autodesk.ACC.Construction.Rfis.V3.Projects.Item.Rfis.Item.Responses;

var rfis = await client.RFIsManager.SearchRfisAsync(projectId, new SearchRfisPostRequestBody());

await client.RFIsManager.CreateRfiResponseAsync(projectId, rfiId, new ResponsesPostRequestBody
{
    Content = "The specified material has been approved.",
});
```

### Cost Management

```csharp
await foreach (var budget in client.CostManager.ListBudgetsAsync(containerId))
{
    Console.WriteLine($"{budget.Name}: {budget.OriginalAmount}");
}

await foreach (var contract in client.CostManager.ListContractsAsync(containerId))
{
    Console.WriteLine($"{contract.Name} — {contract.Status}");
}
```

### Model Coordination and Clash Detection

```csharp
await foreach (var modelSet in client.ModelCoordinationManager.ListModelSetsAsync(containerId))
{
    Console.WriteLine($"{modelSet.Name} — {modelSet.Status}");
}

var clashTests = await client.ModelCoordinationManager
    .GetModelSetClashTestsAsync(containerId, modelSetId);
```

### Using the Fluent URL API

For full control or endpoints not covered by a Manager, use the `Api` property which mirrors the REST path structure:

```csharp
// GET /construction/issues/v1/projects/{id}/issues
var issues = await client.Api.Construction.Issues.V1.Projects[projectId].Issues.GetAsync();

// GET /construction/files/v1/projects/{id}/folders/{id}/contents
var files = await client.Api.Construction.Files.V1.Projects[projectId]
    .Folders[folderId].Contents.GetAsync();
```

## Fluent URL Shortcut Properties

These shortcut properties skip the version prefix for common endpoints:

| Property | Base Path |
| -------- | --------- |
| `client.Accounts` | `/hq/v1/accounts/*` |
| `client.Admin` | `/construction/admin/v1/*` |
| `client.Assets` | `/construction/assets/*` |
| `client.AutoSpecs` | `/construction/autospecs/v1/*` |
| `client.Clash` | `/bim360/clash/v3/*` |
| `client.Cost` | `/cost/v1/*` |
| `client.DataConnector` | `/dataconnector/v1/*` |
| `client.Docs` | `/bim360/docs/v1/*` |
| `client.Files` | `/construction/files/v1/*` |
| `client.Forms` | `/construction/forms/v1/*` |
| `client.Index` | `/construction/index/v2/*` |
| `client.Issues` | `/construction/issues/v1/*` |
| `client.Locations` | `/construction/locations/v2/*` |
| `client.ModelSet` | `/bim360/modelset/v3/*` |
| `client.Packages` | `/construction/packages/v1/*` |
| `client.Photos` | `/construction/photos/v1/*` |
| `client.RCM` | `/construction/rcm/v1/*` |
| `client.Relationships` | `/bim360/relationship/v2/*` |
| `client.Reviews` | `/construction/reviews/v1/*` |
| `client.RFIs` | `/construction/rfis/v3/*` |
| `client.Sheets` | `/construction/sheets/v1/*` |
| `client.Submittals` | `/construction/submittals/v2/*` |
| `client.Takeoff` | `/construction/takeoff/v1/*` |
| `client.Transmittals` | `/construction/transmittals/v1/*` |
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
    var issues = await client.IssuesManager.ListIssuesAsync(projectId).ToListAsync();
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

var issues = await client.Issues.Projects[projectId].Issues.GetAsync(config =>
{
    config.Options.Add(new ErrorHandlerOption { Enabled = false });
});
```

## Custom HttpClient

You can provide your own `HttpClient` for advanced scenarios (proxies, custom handlers, etc.):

```csharp
var httpClient = new HttpClient();
var client = new ACCclient(getAccessToken, httpClient);
```

## Constructor

```csharp
public ACCclient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
```

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| `getAccessToken` | `Func<Task<string>>` | Async function returning a valid OAuth bearer token |
| `httpClient` | `HttpClient?` | Optional custom HttpClient (default includes retry, rate-limit, and error handling middleware) |

## Conventions

These patterns are consistent across all 341 methods and are useful for AI code generation:

- All async methods use the `*Async` suffix
- Paginated endpoints return `IAsyncEnumerable<T>` — auto-fetches all pages; use `break` or `.Take(n)` to stop early
- Non-paginated endpoints return `Task<T?>`
- Every method accepts optional `RequestConfiguration<T>? requestConfiguration` (object, not `Action<>`) and `CancellationToken cancellationToken` parameters
- `projectId` is `string` in most managers; `Guid` in `AccountAdminManager`, `ReviewsManager`, `ModelCoordinationManager`
- `containerId` is `string` in `CostManager`; `Guid` in `ModelCoordinationManager` and `RelationshipsManager`
- Request body types are Kiota-generated classes in sub-namespaces (e.g. `Autodesk.ACC.Construction.Issues.V1.Projects.Item.Issues.IssuesPostRequestBody`)

## Related Packages

| Package | NuGet | Purpose |
| ------- | ----- | ------- |
| `Adsk.Platform.Authentication` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.Authentication) | OAuth 2-legged/3-legged token management |
| `Adsk.Platform.HttpClient` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.HttpClient) | Shared HTTP client with retry, rate limiting, error handling |
| `Adsk.Platform.DataManagement` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.DataManagement) | Hubs, projects, folders, items, versions |

## For AI Assistants

A machine-readable API reference with all 341 method signatures, return types, and REST endpoint mappings is available at [`llm.txt`](./llm.txt) — optimized for use with AI coding tools (Copilot, Cursor, ChatGPT, etc.).

## Requirements

- .NET 8.0 or later
- Valid [Autodesk Platform Services (APS)](https://aps.autodesk.com/) access token with appropriate scopes

## Documentation

- [ACC API Documentation](https://aps.autodesk.com/en/docs/acc/v1/overview/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
