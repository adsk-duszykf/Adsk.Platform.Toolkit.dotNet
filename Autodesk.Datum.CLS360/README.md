# Autodesk Datum CLS360 SDK for .NET

[![NuGet](https://img.shields.io/nuget/v/Adsk.Platform.Datum.CLS360)](https://www.nuget.org/packages/Adsk.Platform.Datum.CLS360)

> **Unofficial package** - not affiliated with or endorsed by Autodesk.
>
> **Namespace:** `Autodesk.Datum.CLS360` | **Targets:** `net8.0`, `net10.0` | **License:** MIT
> Generated from OpenAPI specifications with [Microsoft Kiota](https://learn.microsoft.com/openapi/kiota/overview).

A type-safe C# SDK for the Autodesk Datum CLS360 REST API. It provides managers for domains, classes, templates, attributes, and files, plus a fluent API for direct endpoint access.

## Installation

```bash
dotnet add package Adsk.Platform.Datum.CLS360
```

## Quick Start

Create a client with your CLS360 tenant name and a function that returns an access token. The tenant name is used to form the base URL `https://{tenantName}.cls360.io/api`.

```csharp
using Autodesk.Datum.CLS360;

var client = new DatumCLS360Client(
    tenantName: "your-tenant",
    getAccessToken: () => Task.FromResult("YOUR_ACCESS_TOKEN"));

await foreach (var domain in client.DomainManager.ListDomainsAsync())
{
    Console.WriteLine(domain.Hdl);
}
```

## Authentication With 2-Legged OAuth

CLS360 authentication uses the tenant-specific ACL360 token endpoint. Request a client-credentials token from `https://{tenant}.acl360.io/oauth2/token`, then pass the returned access token to the client:

```csharp
using System.Net.Http.Json;
using Autodesk.Datum.CLS360;

var tenantName = "autodesk-demo";
var clientId = "YOUR_CLIENT_ID";
var clientSecret = "YOUR_CLIENT_SECRET";

using var httpClient = new HttpClient();
var tokenResponse = await httpClient.PostAsJsonAsync(
    $"https://{tenantName}.acl360.io/oauth2/token",
    new
    {
        grant_type = "client_credentials",
        client_id = clientId,
        client_secret = clientSecret,
        scope = $"{tenantName}-cls360"
    });

tokenResponse.EnsureSuccessStatusCode();
var token = await tokenResponse.Content.ReadFromJsonAsync<AccessToken>();
if (token?.access_token is null)
    throw new InvalidOperationException("The token response did not contain an access token.");

var client = new DatumCLS360Client(
    tenantName,
    () => Task.FromResult(token.access_token));

public sealed class AccessToken
{
    public string? access_token { get; set; }
    public string? token_type { get; set; }
}
```

The scope normally follows the `{tenant}-cls360` convention and must match the service configured in ACL360. Store client credentials securely and do not hard-code them in production applications.

## Managers

| Manager | Operations |
| --- | --- |
| `AttributeManager` | Get information and measure attributes for a domain |
| `ClassManager` | List, create, get, and update classes and their mappings |
| `DomainManager` | List, get, seed, and retrieve snapshots of domains |
| `FileManager` | Download files by file handle or dataset reference |
| `TemplateManager` | List, create, get, and update templates and essential attributes |

## Usage Examples

### Retrieve a Domain and Its Attributes

```csharp
var domain = await client.DomainManager.GetDomainAsync("domain-handle");
var attributes = await client.AttributeManager.GetAttributesForDomainAsync("domain-handle");
```

### List Classes and Templates

```csharp
await foreach (var classItem in client.ClassManager.ListClassesAsync("domain-handle"))
{
    Console.WriteLine(classItem.Hdl);
}

await foreach (var template in client.TemplateManager.ListTemplatesAsync("domain-handle"))
{
    Console.WriteLine(template.Hdl);
}
```

### Download a File

```csharp
var content = await client.FileManager.GetFileByHandleAsync("file-handle");
```

## Automatic Pagination

Paginated manager methods return `IAsyncEnumerable<T>` and fetch subsequent pages automatically. Use `break` or LINQ's `.Take(n)` to stop early.

```csharp
await foreach (var domain in client.DomainManager.ListDomainsAsync())
{
    Console.WriteLine(domain.Hdl);
}
```

Most paged methods accept Kiota `RequestConfiguration<T>` for query filters, headers, options, and cancellation tokens.

## Fluent API

Use `Api` when an endpoint is not exposed by a manager, or when you need direct access to the generated request builders:

```csharp
var domains = await client.Api.Domains.GetAsync();
var domain = await client.Api.Domains["domain-handle"].GetAsync();
```

## Custom HttpClient

Pass an `HttpClient` instance to use your application's configured client:

```csharp
var httpClient = new HttpClient();
var client = new DatumCLS360Client("your-tenant", getAccessToken, httpClient);
```

## Rate Limiting and Error Handling

The default Kiota HTTP pipeline retries transient `429`, `503`, and `504` responses and respects `Retry-After` headers. The shared HTTP client also provides configurable rate limiting and converts non-success responses to `ApiException` by default. See the [shared HTTP client documentation](../Autodesk.Common.HttpClient/README.md) for configuration details.

## Constructor

```csharp
public DatumCLS360Client(
    string tenantName,
    Func<Task<string>> getAccessToken,
    HttpClient? httpClient = null)
```

| Parameter | Type | Description |
| --- | --- | --- |
| `tenantName` | `string` | CLS360 tenant name used to construct the API base URL |
| `getAccessToken` | `Func<Task<string>>` | Async function returning a valid OAuth bearer token |
| `httpClient` | `HttpClient?` | Optional custom HTTP client |

## Conventions

- Async manager methods use the `Async` suffix.
- Paginated endpoints return `IAsyncEnumerable<T>`.
- Non-paginated endpoints return `Task<T?>`.
- Request configuration uses Kiota `RequestConfiguration<T>` objects.
- Request and response models are generated types in the `Autodesk.Datum.CLS360` namespace tree.

## Requirements

- .NET 8.0 or later
- A valid ACL360 access token with the CLS360 scope configured for your tenant

## License

This project is licensed under the MIT License.