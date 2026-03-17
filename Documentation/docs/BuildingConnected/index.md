# Adsk.Platform.BuildingConnected

> **⚠️UNOFFICIAL PACKAGE⚠️**

The package `Adsk.Platform.BuildingConnected` provides a set of APIs to interact with the [Autodesk BuildingConnected Service](https://aps.autodesk.com/en/docs/buildingconnected/v2/overview/).

## Documentation

- [API Reference](xref:Autodesk.BuildingConnected): Strongly typed API

## Installation

```bash
dotnet add package Adsk.Platform.BuildingConnected
```

## Usage

See the [QuickStart Guide](../GetStarted/quickStart.md) for a general understanding.

The root object is [`BuildingConnectedClient`](xref:Autodesk.BuildingConnected.BuildingConnectedClient). This object provides shortcuts to the BuildingConnected API endpoints.

```csharp
using Autodesk.BuildingConnected;

async Task<string> getAccessToken()
{
    //return access token with your logic
}

var bcClient = new BuildingConnectedClient(getAccessToken);

// Access BuildingConnected services through shortcut properties
// e.g. bcClient.BuildingConnected
```
