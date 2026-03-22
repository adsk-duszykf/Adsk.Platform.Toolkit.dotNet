# BIM 360 SDK for .NET

[![NuGet](https://img.shields.io/nuget/v/Adsk.Platform.BIM360)](https://www.nuget.org/packages/Adsk.Platform.BIM360)

> **Unofficial package** — not affiliated with or endorsed by Autodesk.
>
> **Namespace:** `Autodesk.BIM360` | **Target:** `net8.0` | **License:** MIT
> Generated from OpenAPI specs via [Microsoft Kiota](https://learn.microsoft.com/openapi/kiota/overview).

A type-safe C# SDK for the [Autodesk BIM 360](https://aps.autodesk.com/en/docs/bim360/v1/overview/) REST APIs. Covers account admin, issues, cost management, RFIs, model coordination, assets, checklists, document management, and more — **266 methods across 12 service managers** through a single unified client.

The SDK provides two access patterns:

1. **Manager API** (recommended) — high-level methods with automatic pagination, strongly-typed parameters, and XML doc comments linking to official APS docs.
2. **Fluent URL API** — mirrors the REST endpoint structure directly for full control over requests.

## Installation

```bash
dotnet add package Adsk.Platform.BIM360
dotnet add package Adsk.Platform.Authentication
```

## Quick Start

```csharp
using Autodesk.BIM360;

var client = new BIM360client(() => Task.FromResult("YOUR_ACCESS_TOKEN"));

// Manager approach (recommended) — auto-paginates all pages
await foreach (var issue in client.IssuesManager.ListIssuesAsync(containerId))
{
    Console.WriteLine($"{issue.Title} — {issue.Status}");
}

// Fluent URL approach — mirrors the REST path directly
var response = await client.Api.Issues.V2.Containers[containerId].QualityIssues.GetAsync();
```

### Authentication with 2-Legged OAuth

For server-to-server communication, use the [`Adsk.Platform.Authentication`](https://www.nuget.org/packages/Adsk.Platform.Authentication) package:

```csharp
using Autodesk.BIM360;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;

var authClient = new AuthenticationClient();
var tokenStore = new InMemoryTokenStore();

var getAccessToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId: "YOUR_CLIENT_ID",
    clientSecret: "YOUR_CLIENT_SECRET",
    scopes: new[] { "data:read", "data:write", "account:read" },
    tokenStore);

var client = new BIM360client(getAccessToken);
```

### Dependency Injection

```csharp
using Autodesk.Common.HttpClientLibrary;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddAdskToolkitHttpClient("ApsClient");

// In your service:
public class MyService(IHttpClientFactory httpClientFactory)
{
    public BIM360client CreateClient(Func<Task<string>> getAccessToken)
    {
        var httpClient = httpClientFactory.CreateClient("ApsClient");
        return new BIM360client(getAccessToken, httpClient);
    }
}
```

## Available Managers

Every manager is a property on `BIM360client`. Paginated endpoints return `IAsyncEnumerable<T>` (auto-fetches all pages); non-paginated endpoints return `Task<T?>`.

| Manager | Description | Methods |
| ------- | ----------- | ------: |
| `AccountAdminManager` | Accounts, projects, users, companies, business units, roles | 33 |
| `AssetsManager` | Asset records, categories, statuses, custom attributes | 27 |
| `ChecklistsManager` | Checklist templates and instances | 6 |
| `CostManager` | Budgets, contracts, change orders, expenses, payments, main contracts | 84 |
| `DataConnectorManager` | Data extraction requests, jobs, downloads | 11 |
| `DocumentManagementManager` | Permissions, custom attributes, versions batch, exports | 11 |
| `IssuesManager` | Issues, types, comments, attachments, root causes | 14 |
| `LocationsManager` | Location nodes and trees | 2 |
| `ModelCoordinationManager` | Model sets, clash tests, clashes, screenshots | 43 |
| `ModelPropertiesManager` | Property indexes, diffs, queries, fields, manifests | 16 |
| `RFIsManager` | RFIs, comments, attachments, current user | 10 |
| `RelationshipsManager` | Create, search, sync relationships | 9 |

## Automatic Pagination

Paginated Manager methods return `IAsyncEnumerable<T>`, transparently fetching every page. Use `break` or LINQ's `.Take(n)` to stop early without fetching unnecessary pages.

```csharp
// Iterate all issues across all pages
await foreach (var issue in client.IssuesManager.ListIssuesAsync(containerId))
{
    Console.WriteLine(issue.Title);
}

// Stop after first 10 items
int count = 0;
await foreach (var budget in client.CostManager.ListBudgetsAsync(containerId))
{
    if (++count >= 10) break;
}

// Apply query parameter filters
await foreach (var project in client.AccountAdminManager.ListProjectsAsync(accountId,
    new() { QueryParameters = { Sort = ["name"] } }))
{
    Console.WriteLine(project.Name);
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
using Autodesk.BIM360.Issues.V2.Containers.Item.QualityIssues;

IssuesPostResponse? newIssue = await client.IssuesManager.CreateIssueAsync(containerId, new IssuesPostRequestBody
{
    Title = "Missing fire extinguisher on Level 3",
    Description = "Fire safety requirement not met",
});
```

### List RFIs

```csharp
await foreach (var rfi in client.RFIsManager.ListRfisAsync(containerId))
{
    Console.WriteLine($"{rfi.Title} — {rfi.Status}");
}
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
// GET /issues/v2/containers/{containerId}/quality-issues
var issues = await client.Api.Issues.V2.Containers[containerId].QualityIssues.GetAsync();

// GET /bim360/assets/v2/projects/{projectId}/assets
var assets = await client.Api.Bim360.Assets.V2.Projects[projectId].Assets.GetAsync();

// GET /cost/v1/containers/{containerId}/budgets
var budgets = await client.Api.Cost.V1.Containers[containerId].Budgets.GetAsync();
```

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
    var issues = await client.IssuesManager.ListIssuesAsync(containerId).ToListAsync();
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

To disable the error handler for a specific Manager call:

```csharp
using Autodesk.Common.HttpClientLibrary.Middleware.Options;

var issues = await client.IssuesManager.ListIssuesAsync(containerId,
    new() { Options = { new ErrorHandlerOption { Enabled = false } } }).ToListAsync();
```

For Fluent URL API calls, the `Action<>` lambda is used (Kiota-generated):

```csharp
var issues = await client.Api.Issues.V2.Containers[containerId].QualityIssues.GetAsync(config =>
{
    config.Options.Add(new ErrorHandlerOption { Enabled = false });
});
```

## Custom HttpClient

You can provide your own `HttpClient` for advanced scenarios (proxies, custom handlers, etc.):

```csharp
var httpClient = new HttpClient();
var client = new BIM360client(getAccessToken, httpClient);
```

## Constructor

```csharp
public BIM360client(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
```

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| `getAccessToken` | `Func<Task<string>>` | Async function returning a valid OAuth bearer token |
| `httpClient` | `HttpClient?` | Optional custom HttpClient (default includes retry, rate-limit, and error handling middleware) |

## Conventions

These patterns are consistent across all 266 methods and are useful for AI code generation:

- All async methods use the `*Async` suffix
- Paginated endpoints return `IAsyncEnumerable<T>` — auto-fetches all pages; use `break` or `.Take(n)` to stop early
- Non-paginated endpoints return `Task<T?>`
- Every method accepts optional `RequestConfiguration<T>? requestConfiguration` (object, not `Action<>`) and `CancellationToken cancellationToken` parameters
- `accountId` is `Guid` in `AccountAdminManager`
- `containerId` is `string` in `IssuesManager`, `RFIsManager`, `ChecklistsManager`; `Guid` in `CostManager`, `ModelCoordinationManager`, `RelationshipsManager`, `DataConnectorManager`
- `projectId` is `string` in most managers; `Guid` in some `AccountAdminManager` methods
- Request body types are Kiota-generated classes in sub-namespaces (e.g. `Autodesk.BIM360.Issues.V2.Containers.Item.QualityIssues.IssuesPostRequestBody`)

## Related Packages

| Package | NuGet | Purpose |
| ------- | ----- | ------- |
| `Adsk.Platform.Authentication` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.Authentication) | OAuth 2-legged/3-legged token management |
| `Adsk.Platform.HttpClient` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.HttpClient) | Shared HTTP client with retry, rate limiting, error handling |
| `Adsk.Platform.ACC` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.ACC) | Autodesk Construction Cloud (successor to BIM 360) |
| `Adsk.Platform.DataManagement` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.DataManagement) | Hubs, projects, folders, items, versions |

## For AI Assistants

A machine-readable API reference with all 266 method signatures, return types, and REST endpoint mappings is available at [`llm.txt`](./llm.txt) — optimized for use with AI coding tools (Copilot, Cursor, ChatGPT, etc.).

## Requirements

- .NET 8.0 or later
- Valid [Autodesk Platform Services (APS)](https://aps.autodesk.com/) access token with appropriate scopes

## Documentation

- [BIM 360 API Documentation](https://aps.autodesk.com/en/docs/bim360/v1/overview/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
