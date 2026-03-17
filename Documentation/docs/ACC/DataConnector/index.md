# Adsk.Platform.ACC.DataConnector [Deprecated]

> **⚠️UNOFFICIAL PACKAGE⚠️**
>
> **Note:** This package is deprecated. Use [Adsk.Platform.ACC](https://www.nuget.org/packages/Adsk.Platform.ACC) instead.

The package `Adsk.Platform.ACC.DataConnector` provides a set of APIs to interact with the [Autodesk Construction Cloud - Data Connector Service](https://aps.autodesk.com/en/docs/acc/v1/reference/http/data-connector-api/).

## Documentation

- [API Reference](xref:Autodesk.ACC.DataConnector): Strongly typed API
- [Helpers](xref:Autodesk.ACC.DataConnector.Helpers.DataConnectorClientHelper): Set of helper methods

## Installation

```bash
dotnet add package Adsk.Platform.ACC.DataConnector
```

## Usage

See the  [QuickStart Guide](../../GetStarted/quickStart.md) for a general understanding.

The root object is [`DataConnectorClient`](xref:Autodesk.ACC.DataConnector.DataConnectorClient). This object provides access to the `Authentication` API and the `Helpers` method.


### Use the [API](xref:Autodesk.ACC.DataConnector)

```csharp
using Autodesk.ACC.DataConnector;
using Autodesk.ACC.DataConnector.Models;

public async Task<Jobs> GetJobs()
{

    async Task<string> getAccessToken()
    {
        //return access token with your logic
    }

    var DataConnectorClient = new DataConnectorClient(getAccessToken);

    var accountId = ""; // Replace with your account id

    // The `DataConnectorClient.Api` object refers to the base url `https://developer.api.autodesk.com/data-connector/v1/`
    // Returns the jobs for the specified account in ascending order
    var jobs = await Api.Accounts[accountId].Jobs.GetAsync(r => r.QueryParameters.Sort="asc");

    return jobs;
}
```
