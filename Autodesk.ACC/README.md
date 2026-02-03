# Autodesk.ACC - Autodesk Construction Cloud SDK

A .NET SDK providing a [Fluent API](https://dzone.com/articles/java-fluent-api) for the [Autodesk Construction Cloud (ACC)](https://aps.autodesk.com/en/docs/acc/v1/overview/) APIs, generated from the official OpenAPI specifications using [Microsoft Kiota](https://learn.microsoft.com/en-us/openapi/kiota/overview).

## Features

This SDK provides access to multiple ACC API endpoints through a unified client:

| API | Endpoint Path |
|-----|---------------|
| **Accounts** | `/hq/v1/accounts/*` |
| **Admin** | `/construction/admin/v1/*` |
| **AutoSpecs** | `/construction/autospecs/v1/*` |
| **Clash** | `/bim360/clash/v3/*` |
| **Cost** | `/cost/v1/*` |
| **Data Connector** | `/dataconnector/v1/*` |
| **Docs** | `/bim360/docs/v1/*` |
| **Files** | `/construction/files/v1/*` |
| **Forms** | `/construction/forms/v1/*` |
| **Index** | `/construction/index/v2/*` |
| **Issues** | `/construction/issues/v1/*` |
| **ModelSet** | `/bim360/modelset/v3/*` |
| **Relationships** | `/bim360/relationship/v2/*` |
| **RCM** | `/construction/rcm/v1/*` |
| **RFIs** | `/construction/rfis/v3/*` |
| **Sheets** | `/construction/sheets/v1/*` |
| **Submittals** | `/construction/submittals/v2/*` |

## Installation

```bash
dotnet add package Adsk.Platform.ACC
```

## Quick Start

```csharp
using Autodesk.ACC;

// Provide a function that returns the access token
Func<Task<string>> getAccessToken = () => Task.FromResult("YOUR_ACCESS_TOKEN");

// Initialize the ACC client
var accClient = new ACCclient(getAccessToken);
```

## Usage Examples

### Get Issues

```csharp
// Get issues for a project
var issues = await accClient.Issues.Projects[projectId].Issues.GetAsync();

foreach (var issue in issues?.Results ?? [])
{
    Console.WriteLine($"Issue: {issue.Title} - Status: {issue.Status}");
}
```

### Get Clash Results

```csharp
// Get clash test results
var clashTests = await accClient.Clash.Containers[containerId].Clash.Tests.GetAsync();
```

### Get Project Files

```csharp
// Get files in a folder
var files = await accClient.Files.Projects[projectId].Folders[folderId].Contents.GetAsync();
```

### Get RFIs

```csharp
// Get RFIs for a project
var rfis = await accClient.RFIs.Projects[projectId].Rfis.GetAsync();
```

### Using the Full API

For endpoints not available through shortcuts, use the `Api` property to access the full API structure:

```csharp
// Access the full API
var result = await accClient.Api.Construction.Issues.V1.Projects[projectId].Issues.GetAsync();
```

## API Structure

The SDK provides convenient shortcut properties for common endpoints:

| Property | Description |
|----------|-------------|
| `accClient.Accounts` | Account management APIs |
| `accClient.Projects` | Project relationship APIs |
| `accClient.Clash` | Clash detection APIs |
| `accClient.Docs` | Document management APIs |
| `accClient.ModelSet` | Model set management APIs |
| `accClient.AutoSpecs` | AutoSpecs APIs |
| `accClient.Admin` | Admin APIs |
| `accClient.Issues` | Issues management APIs |
| `accClient.Sheets` | Sheets APIs |
| `accClient.Forms` | Forms APIs |
| `accClient.Files` | Files management APIs |
| `accClient.Index` | Index/search APIs |
| `accClient.Cost` | Cost management APIs |
| `accClient.DataConnector` | Data Connector APIs |
| `accClient.Submittals` | Submittals APIs |
| `accClient.RFIs` | RFI management APIs |

## Custom HttpClient

You can provide your own `HttpClient` instance for advanced scenarios:

```csharp
var httpClient = new HttpClient();
// Configure your HttpClient...

var accClient = new ACCclient(getAccessToken, httpClient);
```

## Requirements

- .NET 8.0 or later
- Valid Autodesk Platform Services (APS) access token with appropriate scopes

## Documentation

- [ACC API Documentation](https://aps.autodesk.com/en/docs/acc/v1/overview/)
- [Autodesk Platform Services](https://aps.autodesk.com/)
- [Microsoft Kiota Documentation](https://learn.microsoft.com/en-us/openapi/kiota/overview)

## License

This project is licensed under the MIT License.
