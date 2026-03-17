# Adsk.Platform.BIM360

> **⚠️UNOFFICIAL PACKAGE⚠️**

The package `Adsk.Platform.BIM360` provides a set of APIs to interact with the [Autodesk BIM 360 Services](https://aps.autodesk.com/en/docs/bim360/v1/overview/).

## Documentation

- [API Reference](xref:Autodesk.BIM360): Strongly typed API

## Installation

```bash
dotnet add package Adsk.Platform.BIM360
```

## Usage

See the [QuickStart Guide](../GetStarted/quickStart.md) for a general understanding.

The root object is [`BIM360client`](xref:Autodesk.BIM360.BIM360client). This object provides shortcuts to the various BIM 360 API endpoints.

```csharp
using Autodesk.BIM360;

async Task<string> getAccessToken()
{
    //return access token with your logic
}

var bim360Client = new BIM360client(getAccessToken);

// Access BIM 360 services through shortcut properties
// e.g. bim360Client.Issues, bim360Client.Cost, bim360Client.Docs, etc.
```
