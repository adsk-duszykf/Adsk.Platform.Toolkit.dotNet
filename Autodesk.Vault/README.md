# Autodesk Vault Data SDK for .NET

[![NuGet](https://img.shields.io/nuget/v/Adsk.Platform.VaultData)](https://www.nuget.org/packages/Adsk.Platform.VaultData)

> **Unofficial package** — not affiliated with or endorsed by Autodesk.
>
> **Namespace:** `Autodesk.Vault` | **Target:** `net8.0` | **License:** MIT
> Generated from OpenAPI specs via [Microsoft Kiota](https://learn.microsoft.com/openapi/kiota/overview).

A type-safe C# SDK for the [Autodesk Vault](https://www.autodesk.com/products/vault/overview) Data REST API (v2). Covers sessions, vaults, files, folders, items, change orders, users, groups, roles, search, jobs, links, properties, and options — **60 methods across 11 service managers** through a single unified client.

The SDK provides two access patterns:

1. **Manager API** (recommended) — high-level methods with strongly-typed parameters and XML doc comments.
2. **Fluent URL API** — mirrors the REST endpoint structure directly for full control over requests.

## Installation

```bash
dotnet add package Adsk.Platform.VaultData
dotnet add package Adsk.Platform.Authentication
```

## Quick Start

```csharp
using Autodesk.Vault;

var client = new VaultClient(
    () => Task.FromResult("YOUR_ACCESS_TOKEN"),
    vaultServerUrl: "https://your-vault-server");

// Manager approach (recommended)
var vaults = await client.Informational.GetVaultsAsync();
foreach (var vault in vaults?.Data ?? [])
{
    Console.WriteLine($"Vault: {vault.Name} (ID: {vault.Id})");
}

// Fluent URL approach — mirrors the REST path directly
var serverInfo = await client.Api.ServerInfo.GetAsync();
Console.WriteLine($"Server Version: {serverInfo?.ServerVersion}");
```

### Authentication with 2-Legged OAuth

For server-to-server communication with user impersonation, use the [`Adsk.Platform.Authentication`](https://www.nuget.org/packages/Adsk.Platform.Authentication) package:

```csharp
using Autodesk.Vault;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;

var authClient = new AuthenticationClient();
var tokenStore = new InMemoryTokenStore();

var getAccessToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId: "YOUR_CLIENT_ID",
    clientSecret: "YOUR_CLIENT_SECRET",
    scopes: new[] { "data:read", "data:write" },
    tokenStore);

// 2-legged auth requires a userId for impersonation
var client = new VaultClient(
    getAccessToken,
    vaultServerUrl: "https://your-vault-server",
    userId: "user@company.com");
```

### Authentication with 3-Legged OAuth

For user-context operations:

```csharp
using Autodesk.Vault;

var client = new VaultClient(
    () => Task.FromResult("YOUR_3LEGGED_ACCESS_TOKEN"),
    vaultServerUrl: "https://your-vault-server");
```

## Available Managers

Every manager is a property on `VaultClient`. All endpoints return `Task<T?>` — the Vault API uses server-side filtering and sorting via query parameters.

| Manager | Description | Methods |
| --- | --- | ---: |
| `Auth` | Session creation (standard and Windows auth), retrieval, deletion | 4 |
| `Accounts` | Users, groups, roles, profile attributes, account info | 11 |
| `Options` | System-wide options and per-vault options (CRUD) | 10 |
| `Informational` | Server info, vault listing and details | 3 |
| `Properties` | Property definitions for a vault | 2 |
| `FilesAndFolders` | File versions, file details, folder contents, sub-folders, downloads, thumbnails | 9 |
| `Items` | Items, item versions, associated files, bill of materials, thumbnails | 8 |
| `ChangeOrders` | Change orders (ECOs), related files, associated entities, comments, attachments | 6 |
| `Links` | Entity links and relationships | 2 |
| `Search` | Basic search results and advanced search | 2 |
| `Jobs` | Job creation, status, and queue management | 3 |

## Usage Examples

### Create a Session

```csharp
using Autodesk.Vault.Sessions;

var sessionData = new SessionsPostRequestBody
{
    VaultName = "MyVault",
    UserName = "myuser",
    Password = "mypassword"
};

var session = await client.Auth.CreateSessionAsync(sessionData);
Console.WriteLine($"Session ID: {session?.Id}");
```

### Browse Files and Folders

```csharp
var contents = await client.FilesAndFolders.GetFolderContentsAsync(vaultId, folderId);
foreach (var item in contents?.Data ?? [])
{
    Console.WriteLine($"Item: {item.Name}");
}

// Download file content
var fileStream = await client.FilesAndFolders.GetFileVersionContentAsync(vaultId, fileVersionId);
```

### Work with Items and Bill of Materials

```csharp
var items = await client.Items.GetItemsAsync(vaultId);
foreach (var item in items?.Data ?? [])
{
    Console.WriteLine($"Item: {item.Number} — {item.Title}");
}

var bom = await client.Items.GetItemVersionBillOfMaterialsAsync(vaultId, itemVersionId);
```

### Manage Change Orders (ECOs)

```csharp
var changeOrders = await client.ChangeOrders.GetChangeOrdersAsync(vaultId);
foreach (var eco in changeOrders?.Data ?? [])
{
    Console.WriteLine($"ECO: {eco.Number} — {eco.Title}");
}

var relatedFiles = await client.ChangeOrders.GetChangeOrderRelatedFilesAsync(vaultId, changeOrderId);
var comments = await client.ChangeOrders.GetChangeOrderCommentsAsync(vaultId, changeOrderId);
```

### Advanced Search

```csharp
using Autodesk.Vault.Vaults.WithVaultIdAdvancedSearch;

var searchBody = new WithVaultIdAdvancedSearchPostRequestBody
{
    // Configure search criteria
};

var results = await client.Search.PerformAdvancedSearchAsync(vaultId, searchBody);
foreach (var entity in results?.Data ?? [])
{
    Console.WriteLine($"Found: {entity.Name}");
}
```

### Using the Fluent URL API

```csharp
// Get server information
var serverInfo = await client.Api.ServerInfo.GetAsync();

// Get all users
var users = await client.Api.Users.GetAsync();

// Get file by ID through the full path
var file = await client.Api.Vaults[vaultId].Files[fileId].GetAsync();
```

## Rate Limiting

The SDK handles API rate limits automatically via built-in Kiota middleware. When the API returns a `429 Too Many Requests` response, the SDK will:

- Automatically retry with exponential backoff
- Respect the `Retry-After` header
- Retry up to a configurable number of times before failing

No custom retry logic is needed in your application.

## Error Handling

By default, the SDK throws an `HttpRequestException` for any non-successful HTTP response (4xx or 5xx). The exception includes the request URI, HTTP status code, and the full `HttpResponseMessage` in the `Data["context"]` property.

```csharp
try
{
    var items = await client.Items.GetItemsAsync(vaultId);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Request failed: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");

    if (ex.Data["context"] is HttpResponseMessage response)
    {
        Console.WriteLine($"Request URI: {response.RequestMessage?.RequestUri}");
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response body: {body}");
    }
}
```

To disable the error handler for a specific request:

```csharp
using Autodesk.Common.HttpClientLibrary.Middleware.Options;

var items = await client.Items.GetItemsAsync(vaultId,
    new() { Options = { new ErrorHandlerOption { Enabled = false } } });
```

## Custom HTTP Client

```csharp
var httpClient = new HttpClient();
// Configure your HttpClient (timeouts, handlers, etc.)

var client = new VaultClient(getAccessToken, "https://your-vault-server", userId, httpClient);
```

## Constructors

```csharp
// 2-legged: server-to-server with user impersonation
public VaultClient(Func<Task<string>> get2LeggedAccessToken, string vaultServerUrl, string userId, HttpClient? httpClient = null)

// 3-legged: user context
public VaultClient(Func<Task<string>> get3LeggedAccessToken, string vaultServerUrl, HttpClient? httpClient = null)
```

| Parameter | Type | Description |
| --- | --- | --- |
| `get2LeggedAccessToken` / `get3LeggedAccessToken` | `Func<Task<string>>` | Async function returning a valid OAuth bearer token |
| `vaultServerUrl` | `string` | Valid absolute HTTP/HTTPS URL to the Vault server |
| `userId` | `string` | (2-legged only) Email address of the Vault user to impersonate |
| `httpClient` | `HttpClient?` | Optional custom HTTP client (default includes retry + rate limiting) |

The `vaultServerUrl` must be a valid absolute HTTP or HTTPS URL (e.g. `"https://vaultserver.example.com"` or `"http://10.148.0.1"`). An `ArgumentException` is thrown for invalid, relative, or non-HTTP URLs.

## Conventions

Patterns useful for AI code generation:

- **Method naming**: `Get{Entity}Async`, `Get{Entity}ByIdAsync`, `Create{Entity}Async`, `Update{Entity}ByIdAsync`, `Delete{Entity}ByIdAsync`, `Get{Entity}{SubResource}Async`
- **Return types**: all methods return `Task<T?>` or `Task` (no `IAsyncEnumerable<T>` — Vault API does not use cursor-based pagination through manager methods)
- **RequestConfiguration parameter**: all Manager methods accept `RequestConfiguration<T>? requestConfiguration` as an object — **never `Action<>`**. Configure via object initializer: `new() { QueryParameters = { ... } }`
- **Parameter types**: all identifiers (`vaultId`, `fileId`, `itemId`, etc.) are `string`
- **Body types**: request body types live in Kiota-generated namespaces (e.g. `Autodesk.Vault.Sessions.SessionsPostRequestBody`, `Autodesk.Vault.Vaults.WithVaultIdAdvancedSearch.WithVaultIdAdvancedSearchPostRequestBody`)
- **Model types**: response model types are in `Autodesk.Vault.Models` (e.g. `Session`, `FileObject`, `Item`, `ChangeOrder`, `Job`)

## Fluent URL Shortcut Properties

| Property | Base Path |
| --- | --- |
| `client.Api.Sessions` | `/sessions/*` |
| `client.Api.Groups` | `/groups/*` |
| `client.Api.Users` | `/users/*` |
| `client.Api.Roles` | `/roles/*` |
| `client.Api.ServerInfo` | `/serverInfo` |
| `client.Api.SystemOptions` | `/systemOptions/*` |
| `client.Api.ProfileAttributeDefinitions` | `/profileAttributeDefinitions/*` |
| `client.Api.Vaults` | `/vaults/*` |

## Related Packages

| Package | NuGet | Purpose |
| --- | --- | --- |
| `Adsk.Platform.Authentication` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.Authentication) | OAuth token management (2-legged and 3-legged) |
| `Adsk.Platform.HttpClient` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.HttpClient) | Shared HTTP client with retry, rate limiting, DI support |
| `Adsk.Platform.DataManagement` | [NuGet](https://www.nuget.org/packages/Adsk.Platform.DataManagement) | Cloud-based data management (hubs, projects, folders, files) |

## For AI Assistants

A machine-readable API reference with all method signatures, return types, and REST endpoint mappings is available at [`llm.txt`](./llm.txt).

## Requirements

- .NET 8.0 or later
- Valid [Autodesk Platform Services (APS)](https://aps.autodesk.com/) access token with appropriate scopes
- Access to an Autodesk Vault server

## Documentation

- [Autodesk Vault](https://www.autodesk.com/products/vault/overview)
- [Vault REST API Documentation](https://www.autodeskapis.com/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
