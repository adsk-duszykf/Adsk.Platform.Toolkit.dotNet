# Autodesk Datum CLS360 SDK for .NET

[![NuGet](https://img.shields.io/nuget/v/Adsk.Platform.Datum.CLS360)](https://www.nuget.org/packages/Adsk.Platform.Datum.CLS360)

> **Unofficial package** - not affiliated with or endorsed by Autodesk.
>
> **Namespace:** `Autodesk.Datum.CLS360` | **Target:** `net8.0` | **License:** MIT
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

## Managers

| Manager | Operations |
| --- | --- |
| `DomainManager` | List, get, seed, and retrieve snapshots of domains |
| `ClassManager` | List, create, get, and update classes and their mappings |
| `TemplateManager` | List, create, get, and update templates and essential attributes |
| `AttributeManager` | Get information and measure attributes for a domain |
| `FileManager` | Download files by file handle or dataset reference |

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