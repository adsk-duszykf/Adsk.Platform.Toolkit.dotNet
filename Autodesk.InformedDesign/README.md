# Informed Design SDK for .NET

[![NuGet](https://img.shields.io/nuget/v/Adsk.Platform.InformedDesign)](https://www.nuget.org/packages/Adsk.Platform.InformedDesign)

> **Unofficial package** — not affiliated with or endorsed by Autodesk.
>
> **Namespace:** `Autodesk.InformedDesign` | **Target:** `net8.0` | **License:** MIT
> Generated from OpenAPI specs via [Microsoft Kiota](https://learn.microsoft.com/openapi/kiota/overview).

A type-safe C# SDK for the [Autodesk Informed Design (Beta)](https://aps.autodesk.com/en/docs/informed-design/v1/overview/) REST APIs — also known as Dynamic Content / Industrialized Construction. Manage products, releases, variants, outputs, uploads, downloads, and rules — **31 methods across 7 managers** through a single unified client.

The SDK provides two access patterns:

1. **Manager API** (recommended) — high-level methods with automatic pagination, strongly-typed parameters, and XML doc comments linking to official APS docs.
2. **Fluent URL API** — mirrors the REST endpoint structure directly for full control over requests.

## Installation

```bash
dotnet add package Adsk.Platform.InformedDesign
dotnet add package Adsk.Platform.Authentication
```

## Quick Start

```csharp
using Autodesk.InformedDesign;

var client = new InformedDesignClient(() => Task.FromResult("YOUR_ACCESS_TOKEN"));

// Manager approach (recommended) — auto-paginates all pages
await foreach (var product in client.ProductsManager.ListProductsAsync())
{
    Console.WriteLine(product);
}

// Create a product
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Products;

ProductsPostRequestBody body = new() { /* set properties */ };
ProductsPostResponse? created = await client.ProductsManager.CreateProductAsync(body);
```

### Authentication with 2-Legged OAuth

For server-to-server communication, use the [`Adsk.Platform.Authentication`](https://www.nuget.org/packages/Adsk.Platform.Authentication) package:

```csharp
using Autodesk.InformedDesign;
using Autodesk.Authentication;
using Autodesk.Authentication.Helpers.Models;

var authClient = new AuthenticationClient();
var tokenStore = new InMemoryTokenStore();

var getAccessToken = authClient.Helper.CreateTwoLeggedAutoRefreshToken(
    clientId: "YOUR_CLIENT_ID",
    clientSecret: "YOUR_CLIENT_SECRET",
    scopes: new[] { "data:read", "data:write" },
    tokenStore);

var client = new InformedDesignClient(getAccessToken);
```

### Dependency Injection

```csharp
using Autodesk.Common.HttpClientLibrary;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddAdskToolkitHttpClient("ApsClient");

// In your service:
public class MyService(IHttpClientFactory httpClientFactory)
{
    public InformedDesignClient CreateClient(Func<Task<string>> getAccessToken)
    {
        var httpClient = httpClientFactory.CreateClient("ApsClient");
        return new InformedDesignClient(getAccessToken, httpClient);
    }
}
```

## Available Managers

| Manager | Description | Methods |
| --- | --- | --- |
| `ProductsManager` | Product CRUD, upload URLs, complete upload, download URL, delete upload | 9 |
| `ReleasesManager` | Release CRUD for products | 5 |
| `VariantsManager` | Variant CRUD for releases | 4 |
| `OutputsManager` | Output CRUD and outputs request status | 5 |
| `UploadsManager` | Upload request listing, creation, and details | 3 |
| `DownloadsManager` | Download request creation and details | 2 |
| `RulesManager` | Evaluate, validate, and retrieve rules | 3 |

## Automatic Pagination

Paginated endpoints return `IAsyncEnumerable<T>` and automatically fetch all pages. Use `break` or LINQ's `.Take(n)` to stop early.

```csharp
// Iterate all products
await foreach (var product in client.ProductsManager.ListProductsAsync())
{
    Console.WriteLine(product);
}

// Stop after first 10
int count = 0;
await foreach (var product in client.ProductsManager.ListProductsAsync())
{
    if (count++ >= 10) break;
}

// With query parameters — use object initializer syntax
await foreach (var release in client.ReleasesManager.ListReleasesAsync(
    new() { QueryParameters = { Limit = 25 } }))
{
    Console.WriteLine(release);
}
```

## Usage Examples

### Get a Product by ID

```csharp
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Products.Item;

Guid productId = Guid.Parse("your-product-id");
WithProductGetResponse? product = await client.ProductsManager.GetProductAsync(productId);
```

### Create a Release

```csharp
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Releases;

ReleasesPostRequestBody body = new() { /* set properties */ };
ReleasesPostResponse? release = await client.ReleasesManager.CreateReleaseAsync(body);
```

### Evaluate Rules

```csharp
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.RulesEvaluate;

RulesEvaluatePostRequestBody body = new() { /* set properties */ };
RulesEvaluatePostResponse? result = await client.RulesManager.EvaluateRulesAsync(body);
```

### Upload Product Content

```csharp
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Products.Item.UploadUrls;
using Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Products.Item.CompleteUpload;

Guid productId = Guid.Parse("your-product-id");

// Step 1: Get upload URLs
UploadUrlsPostRequestBody urlsBody = new() { /* set properties */ };
UploadUrlsPostResponse? urls = await client.ProductsManager.GetUploadUrlsAsync(productId, urlsBody);

// Step 2: Upload file content to the signed URLs (use HttpClient directly)

// Step 3: Mark upload as complete
CompleteUploadPostRequestBody completeBody = new() { /* set properties */ };
CompleteUploadPostResponse? completed = await client.ProductsManager.CompleteUploadAsync(productId, completeBody);
```

### Using the Fluent URL API

```csharp
// List products directly via fluent API
var response = await client.Api.IndustrializedConstruction.InformedDesign.V1.Products
    .GetAsync(config =>
    {
        config.QueryParameters.AccessType = GetAccessTypeQueryParameterType.ACC;
        config.QueryParameters.AccessId = "your-access-id";
    });

// Get a single variant
Guid variantId = Guid.Parse("your-variant-id");
var variant = await client.Api.IndustrializedConstruction.InformedDesign.V1.Variants[variantId]
    .GetAsync();
```

## Rate Limiting

The SDK includes automatic retry middleware for HTTP 429 (Too Many Requests) responses. It reads the `Retry-After` header and falls back to exponential backoff. No configuration is needed — this is built into the shared HTTP client provided by [`Adsk.Platform.HttpClient`](https://www.nuget.org/packages/Adsk.Platform.HttpClient).

## Error Handling

The SDK's error handler middleware throws `HttpRequestException` for non-2xx responses, with the full response available in `ex.Data["context"]`:

```csharp
try
{
    await client.ProductsManager.DeleteProductAsync(productId);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Status: {ex.StatusCode} — {ex.Message}");
    if (ex.Data["context"] is HttpResponseMessage resp)
    {
        string body = await resp.Content.ReadAsStringAsync();
        Console.WriteLine($"Response body: {body}");
    }
}
```

To disable the error handler for a specific request (e.g. to handle 404 yourself):

```csharp
using Autodesk.Common.HttpClientLibrary.Middleware.Options;

WithProductGetResponse? product = await client.ProductsManager.GetProductAsync(productId,
    new() { Options = { new ErrorHandlerOption { Enabled = false } } });
```

## Custom HTTP Client

```csharp
var httpClient = new HttpClient();
var client = new InformedDesignClient(() => Task.FromResult("TOKEN"), httpClient);
```

## Constructor

```csharp
public InformedDesignClient(Func<Task<string>> getAccessToken, HttpClient? httpClient = null)
```

| Parameter | Type | Description |
| --- | --- | --- |
| `getAccessToken` | `Func<Task<string>>` | Async function returning a valid OAuth bearer token |
| `httpClient` | `HttpClient?` | (Optional) Custom HTTP client. Default includes retry and rate-limit middleware |

## Conventions

- **Method naming**: `List*Async` (paginated), `Get*Async` (single item), `Create*Async` (POST), `Update*Async` (PATCH), `Delete*Async` (DELETE)
- **Return types**: `IAsyncEnumerable<T>` for paginated endpoints, `Task<T?>` for single-item reads, `Task` for deletes
- **RequestConfiguration**: All manager methods accept `RequestConfiguration<T>?` — **never `Action<>`**. Configure via object initializer: `new() { QueryParameters = { Limit = 50 } }`
- **Parameter types**: All resource IDs are `Guid` except `rulesKey` which is `string`
- **Request body types**: Located in the generated namespace matching the endpoint path (e.g. `Autodesk.InformedDesign.IndustrializedConstruction.InformedDesign.V1.Products.ProductsPostRequestBody`)

## Related Packages

| Package | NuGet | Purpose |
| --- | --- | --- |
| Adsk.Platform.Authentication | [NuGet](https://www.nuget.org/packages/Adsk.Platform.Authentication) | OAuth token management |
| Adsk.Platform.HttpClient | [NuGet](https://www.nuget.org/packages/Adsk.Platform.HttpClient) | Shared HTTP client with retry, rate limiting |
| Adsk.Platform.ACC | [NuGet](https://www.nuget.org/packages/Adsk.Platform.ACC) | Autodesk Construction Cloud SDK |
| Adsk.Platform.DataManagement | [NuGet](https://www.nuget.org/packages/Adsk.Platform.DataManagement) | Data Management / OSS SDK |

## For AI Assistants

A machine-readable API reference with all method signatures, return types, and REST endpoint mappings is available at [`llm.txt`](./llm.txt).

## Requirements

- .NET 8.0 or later
- Valid [Autodesk Platform Services (APS)](https://aps.autodesk.com/) access token with appropriate scopes

## Documentation

- [Informed Design API Documentation](https://aps.autodesk.com/en/docs/informed-design/v1/overview/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
