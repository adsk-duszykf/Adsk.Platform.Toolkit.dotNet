# Autodesk Datum PIM360 SDK for .NET

[![NuGet](https://img.shields.io/nuget/v/Adsk.Platform.Datum.PIM360)](https://www.nuget.org/packages/Adsk.Platform.Datum.PIM360)

> Unofficial package - not affiliated with or endorsed by Autodesk.

A type-safe .NET 8 client for the Autodesk Datum PIM360 REST API, generated with Microsoft Kiota.

## Installation

```bash
dotnet add package Adsk.Platform.Datum.PIM360
```

## Quick start

```csharp
using Autodesk.Datum.PIM360;

DatumPIM360Client client = new(
    tenantName: "your-tenant",
    getAccessToken: () => Task.FromResult("YOUR_ACCESS_TOKEN"));

await foreach (string handle in client.ItemManager.ListHandlesAsync("Equipment", "Status", "Active"))
{
    Console.WriteLine(handle);
}
```

## Managers

The client exposes managers for the main PIM360 API areas:

- `ActivityManager` for activity logs and ETL queue operations
- `AllocatorManager` for tag number allocation and parsing
- `CommentManager` reserved for future comment operations
- `DashboardManager` for dashboard queries
- `EicManager` for Engineering Information Changes
- `ExplorerManager` for attribute explorer queries
- `FacilityManager` for facility explorer queries
- `FileManager` for file downloads
- `ItemManager` for object handles, items, and changes
- `MetaManager` for snapshot metadata and attributes
- `QueryManager` for queries and paged query results
- `QueueManager` for ETL queue status and control
- `ViewManager` for saved views

Paged methods return `IAsyncEnumerable<T>` and fetch subsequent `skip`/`limit` pages automatically. The complete generated Kiota surface remains available through `client.Api`.

## Direct API access

```csharp
Snapshotversion.SnapshotversionGetResponse? snapshot =
    await client.Api.Snapshotversion.GetAsync();
```

Use a custom `HttpClient` when your application supplies its own handler, proxy, or test server:

```csharp
HttpClient httpClient = new();
DatumPIM360Client client = new("your-tenant", getAccessToken, httpClient);
```
