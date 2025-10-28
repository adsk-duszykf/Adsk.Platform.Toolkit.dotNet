# Autodesk Construction Cloud - Data Connector

The package `Adsk.Platform.ACC.DataConnector` provides a set of APIs to interact with the [Autodesk Construction Cloud - Data Connector Service](https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-introduction/).

## Documentation

- [API Reference](xref:Autodesk.ACC.DataConnector): Strongly typed API
- [Managers](xref:Autodesk.ACC.DataConnector.Managers): Organized manager classes for common operations
  - [RequestsManager](xref:Autodesk.ACC.DataConnector.Managers.RequestsManager): Manage data extraction requests
  - [JobsManager](xref:Autodesk.ACC.DataConnector.Managers.JobsManager): Manage extraction jobs
  - [DataManager](xref:Autodesk.ACC.DataConnector.Managers.DataManager): Download extracted data files
- [Helpers](xref:Autodesk.ACC.DataConnector.Helpers.DataConnectorClientHelper): Set of helper methods

## Installation

```bash
dotnet add package Adsk.Platform.ACC.DataConnector
```

## Usage

See the [QuickStart Guide](../GetStarted/quickStart.md) for a general understanding.

The root object is [`DataConnectorClient`](xref:Autodesk.ACC.DataConnector.DataConnectorClient). This object provides access to:

- **Manager classes** for organized, high-level operations (recommended)
- **Direct API access** for low-level control
- **Helper methods** for common workflows (legacy)

### Recommended: Use Manager Classes

Manager classes provide an organized, intuitive interface for working with Data Connector APIs:

```csharp
using Autodesk.ACC.DataConnector;

async Task<string> GetAccessToken()
{
    // Return access token with your logic
}

var client = new DataConnectorClient(GetAccessToken);
var accountId = Guid.Parse("your-account-id");

// Create a data request using RequestsManager
var requestData = new RequestsPostRequestBody
{
    Description = "Monthly issues and RFIs extract",
    ScheduleInterval = "ONE_TIME",
    EffectiveFrom = DateTime.UtcNow,
    ServiceGroups = new[] { "issues", "rfis" },
    SendEmail = true
};

var request = await client.Requests.CreateRequestAsync(accountId, requestData);

// List all jobs using JobsManager
var jobs = await client.Jobs.ListJobsAsync(accountId, config => 
{
    config.QueryParameters.Sort = "desc";
    config.QueryParameters.Limit = 20;
});

// Get job data listing
var jobId = Guid.Parse("job-id");
var dataListing = await client.Jobs.GetJobDataListingAsync(accountId, jobId);

// Download a data file using DataManager
var fileName = Guid.Parse("file-id");
var fileUrl = await client.Data.GetJobDataFileAsync(accountId, jobId, fileName);
```

### Alternative: Direct API Access

For advanced scenarios, you can use the low-level API directly:

```csharp
using Autodesk.ACC.DataConnector;

var client = new DataConnectorClient(GetAccessToken);
var accountId = Guid.Parse("your-account-id");

// Direct API access for fine-grained control
var jobs = await client.Api.DataConnector.V1.Accounts[accountId].Jobs
    .GetAsJobsGetResponseAsync(config => 
    {
        config.QueryParameters.Sort = "asc";
    });
```

## Manager Classes Overview

### RequestsManager

Manages data connector requests (extraction definitions):

- `CreateRequestAsync()` - Create a new data extraction request
- `ListRequestsAsync()` - List all requests with filtering and pagination
- `GetRequestAsync()` - Get details of a specific request
- `UpdateRequestAsync()` - Update request configuration (e.g., schedule, active status)
- `DeleteRequestAsync()` - Delete a request
- `GetRequestJobsAsync()` - Get all jobs spawned by a specific request

### JobsManager

Manages data connector jobs (actual extraction executions):

- `ListJobsAsync()` - List all jobs with filtering and pagination
- `GetJobAsync()` - Get details of a specific job
- `CancelJobAsync()` - Cancel a running job
- `GetJobDataListingAsync()` - List all data files in a completed job

### DataManager

Manages data extraction file operations:

- `GetJobDataFileAsync()` - Get a signed URL to download a specific data extract file

## Key Features

✅ **Type-Safe** - Uses `Guid` types for IDs to prevent errors  
✅ **Async/Await** - Full async support with cancellation tokens  
✅ **IntelliSense** - Comprehensive XML documentation  
✅ **Organized** - APIs grouped by functionality for easy discovery  
✅ **Flexible** - Use managers for simplicity or direct API for control
