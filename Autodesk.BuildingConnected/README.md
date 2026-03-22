# BuildingConnected SDK for .NET

[![NuGet](https://img.shields.io/nuget/v/Adsk.Platform.BuildingConnected)](https://www.nuget.org/packages/Adsk.Platform.BuildingConnected)

> **Unofficial package** — not affiliated with or endorsed by Autodesk.
>
> **Namespace:** `Autodesk.BuildingConnected` | **Target:** `net8.0` | **License:** MIT
> Generated from OpenAPI specs via [Microsoft Kiota](https://learn.microsoft.com/openapi/kiota/overview).

A type-safe C# SDK for the [Autodesk BuildingConnected](https://aps.autodesk.com/en/docs/buildingconnected/v2/overview/) REST APIs. Covers projects, bid packages, bids, invites, opportunities, contacts, users, offices, certifications, and bid forms — **76 methods across 13 service managers** through a single unified client.

The SDK provides two access patterns:

1. **Manager API** (recommended) — high-level methods with automatic cursor-based pagination, strongly-typed parameters, and XML doc comments linking to official APS docs.
2. **Fluent URL API** — mirrors the REST endpoint structure directly for full control over requests.

## Installation

```bash
dotnet add package Adsk.Platform.BuildingConnected
dotnet add package Adsk.Platform.Authentication
```

## Quick Start

```csharp
using Autodesk.BuildingConnected;

var client = new BuildingConnectedClient(() => Task.FromResult("YOUR_ACCESS_TOKEN"));

// Manager approach (recommended) — auto-paginates all pages
await foreach (var project in client.ProjectsManager.ListProjectsAsync())
{
    Console.WriteLine($"{project.Name} — {project.State}");
}

// Fluent URL approach — mirrors the REST path directly
var response = await client.Api.Construction.Buildingconnected.V2.Projects.GetAsync();
```

### Authentication with 2-Legged OAuth

For server-to-server communication, use the [`Adsk.Platform.Authentication`](https://www.nuget.org/packages/Adsk.Platform.Authentication) package:

```csharp
using Autodesk.BuildingConnected;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;

var authClient = new AuthenticationClient();
var tokenStore = new InMemoryTokenStore();

var getAccessToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId: "YOUR_CLIENT_ID",
    clientSecret: "YOUR_CLIENT_SECRET",
    scopes: new[] { "data:read", "data:write" },
    tokenStore);

var client = new BuildingConnectedClient(getAccessToken);
```

### Dependency Injection

```csharp
using Autodesk.Common.HttpClientLibrary;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddAdskToolkitHttpClient("ApsClient");

// In your service:
public class MyService(IHttpClientFactory httpClientFactory)
{
    public BuildingConnectedClient CreateClient(Func<Task<string>> getAccessToken)
    {
        var httpClient = httpClientFactory.CreateClient("ApsClient");
        return new BuildingConnectedClient(getAccessToken, httpClient);
    }
}
```

## Available Managers

Every manager is a property on `BuildingConnectedClient`. Paginated endpoints return `IAsyncEnumerable<T>` (auto-fetches all pages); non-paginated endpoints return `Task<T?>`.

| Manager | Description | Methods |
| ------- | ----------- | ------: |
| `ProjectsManager` | Projects, indirect costs, batch cost operations | 11 |
| `ProjectTeamMembersManager` | Project team member CRUD | 5 |
| `BidPackagesManager` | Bid packages CRUD and batch operations | 8 |
| `InvitesManager` | Invites, email imports | 4 |
| `BidsManager` | Bids, attachments, line items, plugs | 8 |
| `ProjectBidFormsManager` | Project bid forms and line items CRUD/batch | 10 |
| `ScopeSpecificBidFormsManager` | Scope-specific bid forms and line items CRUD/batch | 10 |
| `OpportunitiesManager` | Opportunities CRUD, comments | 7 |
| `ContactsManager` | Contact listing and retrieval | 2 |
| `UsersManager` | Users listing, retrieval, current user | 3 |
| `CertificationManager` | Certificate types and certifying agencies | 2 |
| `OfficesManager` | Office listing and retrieval | 2 |
| `OpportunityProjectPairsManager` | Opportunity-project pair CRUD | 4 |

## Automatic Pagination

Paginated Manager methods return `IAsyncEnumerable<T>`, transparently fetching every page using cursor-based pagination. Use `break` or LINQ's `.Take(n)` to stop early without fetching unnecessary pages.

```csharp
// Iterate all projects across all pages
await foreach (var project in client.ProjectsManager.ListProjectsAsync())
{
    Console.WriteLine(project.Name);
}

// Stop after first 10 items
int count = 0;
await foreach (var bid in client.BidsManager.ListBidsAsync())
{
    if (++count >= 10) break;
}

// Apply query parameter filters
await foreach (var project in client.ProjectsManager.ListProjectsAsync(
    new() { QueryParameters = { SearchText = "Office Tower", IncludeClosed = true } }))
{
    Console.WriteLine(project.Name);
}
```

## Usage Examples

### List Projects and Their Costs

```csharp
await foreach (var project in client.ProjectsManager.ListProjectsAsync())
{
    Console.WriteLine($"{project.Name} — {project.State}");

    await foreach (var cost in client.ProjectsManager.ListCostsAsync(project.Id))
    {
        Console.WriteLine($"  Cost: {cost.Name}");
    }
}
```

### Create a Project and Bid Package

```csharp
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Projects;
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.BidPackages;

var newProject = await client.ProjectsManager.CreateProjectAsync(new ProjectsPostRequestBody
{
    Name = "New Office Building",
});

var bidPackage = await client.BidPackagesManager.CreateBidPackageAsync(new BidPackagesPostRequestBody
{
    ProjectId = newProject?.Id,
    Name = "Electrical Package",
});
```

### Work with Bids and Attachments

```csharp
// List bids with filters
await foreach (var bid in client.BidsManager.ListBidsAsync(
    new() { QueryParameters = { FilterprojectId = projectId } }))
{
    Console.WriteLine($"Bid: {bid.Id}");

    // List line items for each bid
    await foreach (var lineItem in client.BidsManager.ListBidLineItemsAsync(bid.Id))
    {
        Console.WriteLine($"  Line item: {lineItem.Id}");
    }
}
```

### Manage Opportunities

```csharp
using Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Opportunities;

var opportunity = await client.OpportunitiesManager.CreateOpportunityAsync(
    new OpportunitiesPostRequestBody
    {
        Name = "Highway Bridge Project",
    });

// List comments on an opportunity
await foreach (var comment in client.OpportunitiesManager
    .ListOpportunityCommentsAsync(opportunity?.Id))
{
    Console.WriteLine(comment.Id);
}
```

### Using the Fluent URL API

For full control or endpoints not covered by a Manager, use the `Api` property which mirrors the REST path structure:

```csharp
// GET /construction/buildingconnected/v2/projects
var projects = await client.BuildingConnected.Projects.GetAsync();

// GET /construction/buildingconnected/v2/users/me
var me = await client.BuildingConnected.Users.Me.GetAsync();

// GET /construction/buildingconnected/v2/bids/{bidId}
var bid = await client.BuildingConnected.Bids[bidId].GetAsync();
```

## Fluent URL Shortcut Properties

| Property | Base Path |
| -------- | --------- |
| `client.BuildingConnected` | `/construction/buildingconnected/v2/*` |
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
    await foreach (var project in client.ProjectsManager.ListProjectsAsync())
    {
        Console.WriteLine(project.Name);
    }
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

// Fluent URL API uses Action<> lambda (Kiota-generated)
var projects = await client.BuildingConnected.Projects.GetAsync(config =>
{
    config.Options.Add(new ErrorHandlerOption { Enabled = false });
});
```

## Custom HTTP Client

```csharp
var httpClient = new HttpClient();
var client = new BuildingConnectedClient(getAccessToken, httpClient);
```

## Constructor

```csharp
public BuildingConnectedClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
```

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| `getAccessToken` | `Func<Task<string>>` | Async function returning a valid OAuth bearer token |
| `httpClient` | `HttpClient?` | Optional custom HttpClient (default includes retry, rate-limit, and error handling middleware) |

## Conventions

These patterns are consistent across all 76 methods and are useful for AI code generation:

- All async methods use the `*Async` suffix
- Paginated endpoints return `IAsyncEnumerable<T>` — auto-fetches all pages using cursor-based pagination; use `break` or `.Take(n)` to stop early
- Non-paginated endpoints return `Task<T?>`
- Every method accepts optional `RequestConfiguration<T>? requestConfiguration` (object, not `Action<>`) and `CancellationToken cancellationToken` parameters
- All ID parameters (`projectId`, `bidId`, `formId`, etc.) are `string` — no `Guid` types in this SDK
- Request body types are Kiota-generated classes in sub-namespaces (e.g. `Autodesk.BuildingConnected.Construction.Buildingconnected.V2.Projects.ProjectsPostRequestBody`)

## Related Packages

| Package | NuGet | Purpose |
| ------- | ----- | ------- |
| `Adsk.Platform.Authentication` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.Authentication) | OAuth 2-legged/3-legged token management |
| `Adsk.Platform.HttpClient` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.HttpClient) | Shared HTTP client with retry, rate limiting, error handling |
| `Adsk.Platform.ACC` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.ACC) | Autodesk Construction Cloud (ACC) SDK |
| `Adsk.Platform.DataManagement` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.DataManagement) | Hubs, projects, folders, items, versions |

## For AI Assistants

A machine-readable API reference with all 76 method signatures, return types, and REST endpoint mappings is available at [`llm.txt`](./llm.txt).

## Requirements

- .NET 8.0 or later
- Valid [Autodesk Platform Services (APS)](https://aps.autodesk.com/) access token with appropriate scopes

## Documentation

- [BuildingConnected API Documentation](https://aps.autodesk.com/en/docs/buildingconnected/v2/overview/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
