# Autodesk Platform Services SDK for .NET

[![NuGet](https://img.shields.io/nuget/v/Adsk.Platform.DataManagement)](https://www.nuget.org/packages?q=Adsk.Platform)

> **Unofficial packages** — not affiliated with or endorsed by Autodesk.
>
> **Target:** `net8.0` | **License:** MIT | Generated from OpenAPI specs via [Microsoft Kiota](https://learn.microsoft.com/openapi/kiota/overview).

A collection of type-safe .NET libraries for [Autodesk Platform Services (APS)](https://aps.autodesk.com/) REST APIs — covering ACC, BIM 360, Data Management, Model Derivative, Authentication, Vault, Tandem, Automation, BuildingConnected, and Parameters. Each service ships as an independent NuGet package under the `Adsk.Platform.*` namespace.

## Quick Start

```csharp
using Autodesk.Authentication;
using Autodesk.DataManagement;

// 1. Create an auto-refreshing 2-legged token
var authClient = new AuthenticationClient();
var tokenStore = new InMemoryTokenStore();

var getToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId: "YOUR_CLIENT_ID",
    clientSecret: "YOUR_CLIENT_SECRET",
    scopes: new[] { "data:read", "data:write" },
    tokenStore);

// 2. Use any service client
var dmClient = new DataManagementClient(getToken);

// Manager approach (recommended) — auto-paginates all pages
await foreach (var project in dmClient.Projects.ListProjectsAsync("b.my-account-id"))
{
    Console.WriteLine($"{project.Id} — {project.Attributes?.Name}");
}

// Fluent URL approach — mirrors the REST endpoint directly
var hubs = await dmClient.DataMgtApi.Project.V1.Hubs.GetAsync();
```

## Installation

Install only the packages you need:

```bash
dotnet add package Adsk.Platform.Authentication
dotnet add package Adsk.Platform.DataManagement
dotnet add package Adsk.Platform.ACC
```

All SDK packages automatically include the shared HTTP client (`Adsk.Platform.HttpClient`) with retry, rate limiting, and error handling middleware.

## Available Packages

### Platform Services

| Package | NuGet | Client Class | Managers | Helper | README |
| ------- | ----- | ------------ | :------: | :----: | :----: |
| Authentication | [Adsk.Platform.Authentication](https://www.nuget.org/packages/Adsk.Platform.Authentication) | `AuthenticationClient` | — | Yes | [README](Autodesk.Authentication/README.md) |
| Data Management | [Adsk.Platform.DataManagement](https://www.nuget.org/packages/Adsk.Platform.DataManagement) | `DataManagementClient` | Yes | Yes | [README](Autodesk.DataManagement/README.md) |
| Model Derivative | [Adsk.Platform.ModelDerivative](https://www.nuget.org/packages/Adsk.Platform.ModelDerivative) | `ModelDerivativeClient` | Yes | Yes | [README](Autodesk.ModelDerivative/README.md) |

### Autodesk Construction Cloud (ACC)

| Package | NuGet | Client Class | Managers | Helper | README |
| ------- | ----- | ------------ | :------: | :----: | :----: |
| ACC (all-in-one) | [Adsk.Platform.ACC](https://www.nuget.org/packages/Adsk.Platform.ACC) | `ACCclient` | Yes | — | [README](Autodesk.ACC/README.md) |

> The individual ACC packages (`Adsk.Platform.ACC.AccountAdmin`, `ACC.Issues`, `ACC.RFIs`, `ACC.CostManagement`, `ACC.DataConnector`, `ACC.FileManagement`, `ACC.ModelProperties`) are **deprecated** on NuGet. Use the all-in-one `Adsk.Platform.ACC` package instead — it includes all ACC services through a single unified client with 19 service managers.

### Other Services

| Package | NuGet | Client Class | Managers | Helper | README |
| ------- | ----- | ------------ | :------: | :----: | :----: |
| BIM 360 | [Adsk.Platform.BIM360](https://www.nuget.org/packages/Adsk.Platform.BIM360) | `BIM360client` | Yes | — | [README](Autodesk.BIM360/README.md) |
| Automation | [Adsk.Platform.Automation](https://www.nuget.org/packages/Adsk.Platform.Automation) | `AutomationClient` | Yes | — | [README](Autodesk.Automation/README.md) |
| BuildingConnected | [Adsk.Platform.BuildingConnected](https://www.nuget.org/packages/Adsk.Platform.BuildingConnected) | `BuildingConnectedClient` | Yes | — | [README](Autodesk.BuildingConnected/README.md) |
| Parameters | Adsk.Platform.Parameters (preview) | `ParametersClient` | Yes | — | [README](Autodesk.Parameters/README.md) |
| Tandem | [Adsk.Platform.Tandem](https://www.nuget.org/packages/Adsk.Platform.Tandem) | `TandemClient` | Yes | — | [README](Autodesk.Tandem/README.md) |
| Vault | [Adsk.Platform.VaultData](https://www.nuget.org/packages/Adsk.Platform.VaultData) | `VaultClient` | Yes | — | [README](Autodesk.Vault/README.md) |

### Infrastructure

| Package | NuGet | Description | README |
| ------- | ----- | ----------- | :----: |
| HTTP Client | [Adsk.Platform.HttpClient](https://www.nuget.org/packages/Adsk.Platform.HttpClient) | Shared HTTP client with retry, rate limiting, and error handling middleware | [README](Autodesk.Common.HttpClient/README.md) |

## Architecture

Every SDK package follows the same structure. The entry point is always a `{Service}Client` class with up to three access patterns:

```text
{Service}Client
├── .Api        → Fluent URL API (Kiota-generated, mirrors REST endpoints)
├── .Helper     → Convenience methods for multi-step workflows
└── .{Manager}  → High-level manager classes with pagination and typed parameters
```

### Fluent URL API

The Kiota-generated fluent API maps directly to REST endpoint paths — making it easy to translate any APS documentation example into code:

```csharp
// REST:  GET https://developer.api.autodesk.com/project/v1/hubs
// SDK:       client.DataMgtApi.Project.V1.Hubs.GetAsync()

// REST:  GET https://developer.api.autodesk.com/construction/issues/v1/projects/{id}/issues
// SDK:       client.Api.Construction.Issues.V1.Projects[projectId].Issues.GetAsync()
```

### Managers (where available)

Manager classes wrap common operations into strongly-typed methods with automatic pagination via `IAsyncEnumerable<T>`:

```csharp
// Auto-fetches all pages — stop early with break or .Take(n)
await foreach (var issue in accClient.IssuesManager.ListIssuesAsync(projectId))
{
    Console.WriteLine($"{issue.Title} — {issue.Status}");
}
```

### Helper Methods (where available)

Helper methods simplify multi-step workflows into single calls:

```csharp
// Single call to navigate the folder hierarchy by path
var file = await dmClient.Helper.GetFileItemByPathAsync(
    "MyAccount/MyProject/Folder/SubFolder/FileName.ext");
```

## Authentication

The `AuthenticationClient` does not require an access token itself — it is used to obtain tokens for other services.

### 2-Legged OAuth (server-to-server)

```csharp
using Autodesk.Authentication;

var authClient = new AuthenticationClient();
var tokenStore = new InMemoryTokenStore();

var getToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId: "YOUR_CLIENT_ID",
    clientSecret: "YOUR_CLIENT_SECRET",
    scopes: new[] { "data:read", "data:write", "account:read" },
    tokenStore);

// Pass getToken to any service client
var client = new DataManagementClient(getToken);
```

### Simple Token (for testing)

```csharp
Func<Task<string>> getToken = () => Task.FromResult("YOUR_ACCESS_TOKEN");
var client = new DataManagementClient(getToken);
```

## Dependency Injection

All clients support the shared HTTP client factory for ASP.NET Core applications:

```csharp
using Autodesk.Common.HttpClientLibrary;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddAdskToolkitHttpClient("ApsClient");

// In your service:
public class MyService(IHttpClientFactory httpClientFactory)
{
    public DataManagementClient CreateClient(Func<Task<string>> getToken)
    {
        var httpClient = httpClientFactory.CreateClient("ApsClient");
        return new DataManagementClient(getToken, httpClient);
    }
}
```

## Error Handling

By default, all SDK clients throw `HttpRequestException` for non-success HTTP responses (4xx/5xx). The exception includes the full response context:

```csharp
try
{
    var hubs = await dmClient.DataMgtApi.Project.V1.Hubs.GetAsync();
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Status: {ex.StatusCode} — {ex.Message}");

    if (ex.Data["context"] is HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"URI: {response.RequestMessage?.RequestUri}");
        Console.WriteLine($"Body: {body}");
    }
}
```

## Rate Limiting

All SDK clients handle API rate limits automatically via built-in [Kiota HTTP middleware](https://learn.microsoft.com/openapi/kiota/middleware):

- Automatically retries on `429 Too Many Requests`
- Respects the `Retry-After` header
- Configurable retry count with exponential backoff

No custom retry logic needed.

## Why Kiota?

There are several OpenAPI SDK generators ([OpenAPI Generator](https://github.com/OpenAPITools/openapi-generator), [AutoRest](https://github.com/Azure/autorest), [NSwag](https://github.com/RicoSuter/NSwag), etc.). [Kiota](https://learn.microsoft.com/en-us/openapi/kiota/overview) was chosen because it:

- Generates **fully typed** request/response bodies, headers, query parameters, and path parameters
- Maps directly to API documentation — the fluent URL structure matches the REST paths
- Logs errors in reports and continues during generation (robust)
- Supports excluding endpoints from generation (customizable)
- Is open source, developed by Microsoft, and used for the Microsoft Graph SDK (proven at scale)

Introduction by the authors: [Introducing project Kiota | .NET Conf 2023](https://www.youtube.com/watch?v=sQ9Pv-rQ1s8)

## Running Tests

1. Create an app on [Autodesk Platform Services](https://aps.autodesk.com/) and get the client ID and secret.
1. Create `Sdk_Tests/appsettings.json`:

```json
{
    "APS_CLIENT_ID": "YOUR_CLIENT_ID",
    "APS_CLIENT_SECRET": "YOUR_CLIENT_SECRET"
}
```

1. Run the tests:

```bash
dotnet test
```

## For AI Assistants

Each package directory contains a `llm.txt` file with machine-readable method signatures, return types, and REST endpoint mappings — optimized for AI coding tools.

| Package | Reference |
| ------- | --------- |
| ACC | [`Autodesk.ACC/llm.txt`](Autodesk.ACC/llm.txt) |
| Automation | [`Autodesk.Automation/llm.txt`](Autodesk.Automation/llm.txt) |
| BIM 360 | [`Autodesk.BIM360/llm.txt`](Autodesk.BIM360/llm.txt) |
| BuildingConnected | [`Autodesk.BuildingConnected/llm.txt`](Autodesk.BuildingConnected/llm.txt) |
| Parameters | [`Autodesk.Parameters/llm.txt`](Autodesk.Parameters/llm.txt) |
| Tandem | [`Autodesk.Tandem/llm.txt`](Autodesk.Tandem/llm.txt) |

### SDK Conventions

These patterns are consistent across all packages:

- Entry point is always `new {Service}Client(getAccessToken)` (except `AuthenticationClient` which takes no token, and `VaultClient` which takes additional server URL)
- `.Api` property provides the Kiota-generated fluent URL builder
- `.Helper` property (when available) provides convenience methods for multi-step workflows
- Named Manager properties (when available) provide high-level typed operations
- All async methods use the `*Async` suffix
- Paginated Manager methods return `IAsyncEnumerable<T>` — auto-fetches all pages
- Non-paginated methods return `Task<T?>`
- Optional `HttpClient` parameter on all constructors for DI/custom configuration
- Request body types are Kiota-generated classes in sub-namespaces matching the URL path

## Documentation

- [Autodesk Platform Services](https://aps.autodesk.com/)
- [APS API Documentation](https://aps.autodesk.com/developer/documentation)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
