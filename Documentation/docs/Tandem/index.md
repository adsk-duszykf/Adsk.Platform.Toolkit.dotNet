# Adsk.Platform.Tandem

> **⚠️UNOFFICIAL PACKAGE⚠️**

The package `Adsk.Platform.Tandem` provides a set of APIs to interact with the [Autodesk Tandem Digital Twin Service](https://aps.autodesk.com/en/docs/tandem/v1/overview/).

## Documentation

- [API Reference](xref:Autodesk.Tandem): Strongly typed API

## Installation

```bash
dotnet add package Adsk.Platform.Tandem
```

## Usage

See the [QuickStart Guide](../GetStarted/quickStart.md) for a general understanding.

The root object is [`TandemClient`](xref:Autodesk.Tandem.TandemClient). This object provides shortcuts to the various Tandem API endpoints.

```csharp
using Autodesk.Tandem;

async Task<string> getAccessToken()
{
    //return access token with your logic
}

var tandemClient = new TandemClient(getAccessToken);

// Access Tandem services through shortcut properties
// e.g. tandemClient.Twins, tandemClient.Models, tandemClient.Groups, etc.
```
