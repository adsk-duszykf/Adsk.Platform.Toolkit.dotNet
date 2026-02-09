# Autodesk.Vault - Vault Data SDK

A .NET SDK providing a [Fluent API](https://dzone.com/articles/java-fluent-api) for the [Autodesk Vault](https://www.autodesk.com/products/vault/overview) Data APIs, generated from the official OpenAPI specifications using [Microsoft Kiota](https://learn.microsoft.com/en-us/openapi/kiota/overview).

## Features

This SDK provides access to Vault Data API endpoints through a unified client:

| API | Description |
|-----|-------------|
| **Sessions** | Authentication and session management |
| **Vaults** | Vault operations and management |
| **Files & Folders** | File versions, folder contents, and file associations |
| **Items** | Item management, versions, and bill of materials |
| **Change Orders** | Engineering Change Order (ECO) management |
| **Users** | User account management |
| **Groups** | Group management |
| **Roles** | Role management |
| **Server Info** | Server information retrieval |
| **System Options** | System-level options |
| **Profile Attributes** | User profile attribute definitions |
| **Search** | Basic and advanced search operations |
| **Jobs** | Job management and monitoring |
| **Links** | Entity link management |
| **Properties** | Property definitions |

## Installation

```bash
dotnet add package Adsk.Platform.VaultData
```

## Quick Start

### Using with 2-Legged Authentication (Server-to-Server)

For server-to-server communication using client credentials (2-legged OAuth) with user impersonation:

```csharp
using Autodesk.Vault;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;

// Your APS app credentials
var clientId = "YOUR_CLIENT_ID";
var clientSecret = "YOUR_CLIENT_SECRET";

// Define the required scopes
var scopes = new[] { "data:read", "data:write" };

// Create authentication client
var authClient = new AuthenticationClient();

// Create an auto-refreshing token provider
var tokenStore = new InMemoryTokenStore();
var getAccessToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId, 
    clientSecret, 
    scopes, 
    tokenStore);

// Initialize the Vault client with 2-legged auth
// The userId parameter specifies the Vault user to impersonate
var vaultClient = new VaultClient(
    getAccessToken, 
    vaultServer: "your-vault-server.autodesk.com", 
    userId: "user@company.com");
```

### Using with 3-Legged Authentication (User Context)

For user-context operations using 3-legged OAuth:

```csharp
using Autodesk.Vault;

// Function that returns the 3-legged access token
Func<Task<string>> getAccessToken = () => Task.FromResult("YOUR_3LEGGED_ACCESS_TOKEN");

// Initialize the Vault client with 3-legged auth
var vaultClient = new VaultClient(
    getAccessToken, 
    vaultServer: "your-vault-server.autodesk.com");
```

## Usage Examples

### Create a Session

```csharp
using Autodesk.Vault.Sessions;

// Create session with username/password
var sessionData = new SessionsPostRequestBody
{
    VaultName = "MyVault",
    UserName = "myuser",
    Password = "mypassword"
};

var session = await vaultClient.Auth.CreateSessionAsync(sessionData);
Console.WriteLine($"Session ID: {session?.Id}");
```

### Get Server Information

```csharp
// Get server information
var serverInfo = await vaultClient.Api.ServerInfo.GetAsync();
Console.WriteLine($"Server Version: {serverInfo?.ServerVersion}");
```

### List Vaults

```csharp
// Get all vaults
var vaults = await vaultClient.Api.Vaults.GetAsync();

foreach (var vault in vaults?.Data ?? [])
{
    Console.WriteLine($"Vault: {vault.Name} - ID: {vault.Id}");
}
```

### Get Files and Folders

```csharp
// Get folder contents
var contents = await vaultClient.FilesAndFolders.GetFolderContentsAsync(vaultId, folderId);

foreach (var item in contents?.Data ?? [])
{
    Console.WriteLine($"Item: {item.Name}");
}

// Download file content
var fileStream = await vaultClient.FilesAndFolders.GetFileVersionContentAsync(
    vaultId, 
    fileVersionId);
```

### Work with Items

```csharp
// Get items in a vault
var items = await vaultClient.Items.GetItemsAsync(vaultId);

foreach (var item in items?.Data ?? [])
{
    Console.WriteLine($"Item: {item.Number} - {item.Title}");
}

// Get item versions
var versions = await vaultClient.Items.GetItemVersionsAsync(vaultId, itemId);

// Get bill of materials for an item version
var bom = await vaultClient.Items.GetBillOfMaterialsAsync(vaultId, itemVersionId);
```

### Search Operations

```csharp
using Autodesk.Vault.Vaults.WithVaultIdAdvancedSearch;

// Perform advanced search
var searchBody = new WithVaultIdAdvancedSearchPostRequestBody
{
    // Configure your search criteria
};

var results = await vaultClient.Search.PerformAdvancedSearchAsync(vaultId, searchBody);

foreach (var entity in results?.Data ?? [])
{
    Console.WriteLine($"Found: {entity.Name}");
}
```

### Manage Users and Groups

```csharp
// Get all users
var users = await vaultClient.Accounts.GetUsersAsync();

// Get all groups
var groups = await vaultClient.Accounts.GetGroupsAsync();

// Get user by ID
var user = await vaultClient.Accounts.GetUserByIdAsync(userId);
```

### Using the Full API

For endpoints not available through manager shortcuts, use the `Api` property to access the full API structure:

```csharp
// Access the full API for low-level operations
var result = await vaultClient.Api.Vaults[vaultId].Files[fileId].GetAsync();
```

## Manager Reference

The SDK provides convenient manager properties for organized access to API operations:

| Property | Description |
|----------|-------------|
| `vaultClient.Auth` | Authentication and session management |
| `vaultClient.Accounts` | Users, Groups, Roles, Profile Attributes |
| `vaultClient.Options` | System Options, Vault Options |
| `vaultClient.Informational` | Server Info, Vaults |
| `vaultClient.Properties` | Property definitions |
| `vaultClient.FilesAndFolders` | Files, File Versions, Folders, Associations |
| `vaultClient.Items` | Items, Item Versions, Bill of Materials |
| `vaultClient.ChangeOrders` | Engineering Change Orders (ECOs) |
| `vaultClient.Links` | Entity links and relationships |
| `vaultClient.Search` | Basic and advanced search |
| `vaultClient.Jobs` | Job management and status |

## API Structure

The `Api` property provides direct access to the generated Kiota client:

| Property | Description |
|----------|-------------|
| `vaultClient.Api.Sessions` | Session management endpoints |
| `vaultClient.Api.Vaults` | Vault and vault content endpoints |
| `vaultClient.Api.Users` | User management endpoints |
| `vaultClient.Api.Groups` | Group management endpoints |
| `vaultClient.Api.Roles` | Role management endpoints |
| `vaultClient.Api.ServerInfo` | Server information endpoint |
| `vaultClient.Api.SystemOptions` | System options endpoints |
| `vaultClient.Api.ProfileAttributeDefinitions` | Profile attribute endpoints |

## Custom HttpClient

You can provide your own `HttpClient` instance for advanced scenarios:

```csharp
var httpClient = new HttpClient();
// Configure your HttpClient (timeouts, handlers, etc.)

var vaultClient = new VaultClient(getAccessToken, vaultServer, userId, httpClient);
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
    var items = await vaultClient.Items.GetItemsAsync(vaultId);
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

var items = await vaultClient.Items.GetItemsAsync(vaultId, requestConfig);
```

## Requirements

- .NET 8.0 or later
- Valid Autodesk Platform Services (APS) access token with appropriate scopes
- Access to an Autodesk Vault server

## Documentation

- [Autodesk Vault](https://www.autodesk.com/products/vault/overview)
- [Vault REST API Documentation](https://www.autodeskapis.com/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/en-us/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
