# Autodesk Data Management SDK for .NET

> **⚠️ UNOFFICIAL PACKAGE ⚠️**

The `Adsk.Platform.DataManagement` toolkit provides a set of APIs to interact with the [Autodesk Data Management Service](https://aps.autodesk.com/developer/overview/data-management-api) and the [Object Storage Service (OSS)](https://aps.autodesk.com/en/docs/data/v2/developers_guide/basics/#object-storage-service-oss).

## Getting Started

Install the NuGet package:

```bash
dotnet add package Adsk.Platform.DataManagement
dotnet add package Adsk.Platform.HttpClient
```

Create a client by providing a function that returns a valid access token:

```csharp
var client = new DataManagementClient(() => Task.FromResult(accessToken));
```

The token provider is called before each request, so you can handle token refresh transparently.

## Two Ways to Access the API

The SDK provides two complementary approaches to interact with the Autodesk APIs:

| Approach | Access via | Best for |
|----------|-----------|----------|
| **Managers** | `client.Hubs`, `client.Folders`, ... | Most use cases -- simplified methods, automatic pagination, discoverability |
| **Fluent URL builders** | `client.DataMgtApi`, `client.OssApi` | Advanced scenarios -- full control, custom filters, any endpoint |

Both approaches use the same underlying HTTP client, authentication, and middleware (retry, rate-limiting). You can mix and match them freely in the same application.

### Managers (recommended)

Manager classes wrap the most common operations into simple, self-documented methods that show up in IntelliSense. Paginated endpoints return `IAsyncEnumerable<T>` so you never need to handle page tokens manually.

### Fluent URL Builders

The Kiota-generated fluent API mirrors the REST URL structure. Each path segment is a property, and path parameters are indexers. This gives you access to every endpoint and query parameter, including those not yet covered by a manager.

```csharp
// The fluent path mirrors the REST URL:
// GET /data/v1/projects/{project_id}/folders/{folder_id}/contents
await client.DataMgtApi.Data.V1.Projects[projectId].Folders[folderId].Contents.GetAsync();

// GET /oss/v2/buckets/{bucketKey}/details
await client.OssApi.Oss.V2.Buckets["my-bucket"].Details.GetAsync();
```

## Managers

The SDK organizes all operations into manager classes accessible from the client:

| Manager | Property | Description |
|---------|----------|-------------|
| **Hubs** | `client.Hubs` | List and get hubs (BIM 360, ACC, Fusion Team) |
| **Projects** | `client.Projects` | List/get projects, top folders, storage, downloads, jobs |
| **Folders** | `client.Folders` | Get/create/update folders, list contents, search, refs |
| **Items** | `client.Items` | Get/create/update items, tip version, list versions, refs |
| **Versions** | `client.Versions` | Get/create/update versions, downloads, refs |
| **Commands** | `client.Commands` | Execute bulk commands on project resources |
| **Buckets** | `client.Buckets` | List/create/delete OSS buckets |
| **Objects** | `client.Objects` | List/delete/copy objects, signed URLs, S3 uploads/downloads |

## Usage Examples

### List Hubs and Projects

```csharp
var hubs = await client.Hubs.GetHubsAsync();

await foreach (var project in client.Projects.ListProjectsAsync("b.my-account-id"))
{
    Console.WriteLine($"{project.Id} - {project.Attributes?.Name}");
}
```

### Browse Folder Contents

```csharp
var topFolders = await client.Projects.GetTopFoldersAsync("b.my-account-id", "b.my-project-id");
var rootFolderId = topFolders?.Data?.FirstOrDefault()?.Id;

await foreach (var item in client.Folders.ListFolderContentsAsync("b.my-project-id", rootFolderId!))
{
    Console.WriteLine($"{item.Type}: {item.Id}");
}
```

### Filter Folder Contents

```csharp
await foreach (var item in client.Folders.ListFolderContentsAsync("b.my-project-id", folderId, config =>
{
    config.QueryParameters.FiltertypeAsGetFilterTypeQueryParameterType =
        new[] { GetFilterTypeQueryParameterType.Items };
    config.QueryParameters.Pagelimit = 50;
}))
{
    Console.WriteLine(item.Id);
}
```

### Get Item Tip Version

```csharp
var tip = await client.Items.GetItemTipAsync("b.my-project-id", itemId);
Console.WriteLine($"Latest version: {tip?.Data?.Id}");
```

### List All Versions of an Item

```csharp
await foreach (var version in client.Items.ListItemVersionsAsync("b.my-project-id", itemId))
{
    Console.WriteLine($"v{version.Attributes?.VersionNumber}: {version.Id}");
}
```

### Upload a File (OSS)

```csharp
// 1. Get a signed upload URL
var upload = await client.Objects.GetS3SignedUploadUrlAsync("my-bucket", "my-file.txt");

// 2. Upload the file to S3 using the signed URL (use HttpClient or similar)
// ...

// 3. Complete the upload
await client.Objects.CompleteS3UploadAsync("my-bucket", "my-file.txt",
    new Completes3upload_body { UploadKey = upload?.UploadKey, ETags = new List<string> { eTag } });
```

### List Buckets

```csharp
await foreach (var bucket in client.Buckets.ListBucketsAsync())
{
    Console.WriteLine($"{bucket.BucketKey} ({bucket.PolicyKey})");
}
```

## Pagination

Paginated endpoints return `IAsyncEnumerable<T>` and automatically fetch all pages. You can stop early with `break` or LINQ:

```csharp
// Get only the first 10 projects
var first10 = await client.Projects.ListProjectsAsync("b.my-account-id")
    .Take(10)
    .ToListAsync();
```

## Urn vs Id

The SDK uses `urn` and `id` interchangeably. The `id` is a unique identifier for a folder, item, or version -- it is a string starting with the `urn` prefix.

## Advanced Filtering

Some endpoints support filters as query parameters (see [Filtering](https://aps.autodesk.com/en/docs/data/v2/developers_guide/filtering/) in the APS documentation).

For complex filters not exposed as typed query parameters, override the default URL:

```csharp
var filters = new List<(string, string)>
{
    ("filter[extension.type]", "items:autodesk.bim360:File"),
    ("filter[type]", "item"),
    ("filter[attributes.fileName]-contains", "Floor")
};

var requestInfo = client.DataMgtApi.Data.V1.Projects[projectId].Folders[folderId].Search
    .ToGetRequestInformation();

var searchUri = client.Helper.CreateRequestWithFilters(requestInfo, filters);

var result = await client.DataMgtApi.Data.V1.Projects[projectId].Folders[folderId].Search
    .WithUrl(searchUri).GetAsync();
```

## Fluent URL Builders -- More Examples

When a manager method doesn't cover your use case, drop down to the fluent URL builders. The path mirrors the REST endpoint, so the [APS documentation](https://aps.autodesk.com/en/docs/data/v2/reference/http/) maps directly:

```csharp
// GET /data/v1/projects/{project_id}/folders/{folder_id}/contents  (with query params)
var contents = await client.DataMgtApi.Data.V1.Projects[projectId].Folders[folderId].Contents
    .GetAsync(config =>
    {
        config.QueryParameters.Filtertype = new[] { "items" };
        config.QueryParameters.FilterextensionType = new[] { "items:autodesk.bim360:File" };
    });

// POST /data/v1/projects/{project_id}/commands
var command = await client.DataMgtApi.Data.V1.Projects[projectId].Commands
    .PostAsync(new CreateCommand { /* ... */ });

// GET /oss/v2/buckets/{bucketKey}/objects/{objectKey}/signeds3download
var download = await client.OssApi.Oss.V2.Buckets["my-bucket"].Objects["file.dwg"].Signeds3download
    .GetAsync();
```
