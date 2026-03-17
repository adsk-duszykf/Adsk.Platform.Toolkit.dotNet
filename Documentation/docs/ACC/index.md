# Adsk.Platform.ACC

> **⚠️UNOFFICIAL PACKAGE⚠️**

The package `Adsk.Platform.ACC` provides a set of APIs to interact with the [Autodesk Construction Cloud (ACC) Services](https://aps.autodesk.com/en/docs/acc/v1/overview/).

## Documentation

- [API Reference](xref:Autodesk.ACC): Strongly typed API

## Installation

```bash
dotnet add package Adsk.Platform.ACC
```

## Usage

See the [QuickStart Guide](../GetStarted/quickStart.md) for a general understanding.

The root object is [`ACCclient`](xref:Autodesk.ACC.ACCclient). This object provides shortcuts to the various ACC API endpoints.

```csharp
using Autodesk.ACC;

async Task<string> getAccessToken()
{
    //return access token with your logic
}

var accClient = new ACCclient(getAccessToken);

// Access ACC services through shortcut properties
// e.g. accClient.Issues, accClient.Cost, accClient.Files, etc.
```
