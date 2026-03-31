# Adsk.Platform.InformedDesign

> **⚠️UNOFFICIAL PACKAGE⚠️**

The package `Adsk.Platform.InformedDesign` provides a set of APIs to interact with the [Autodesk Informed Design](https://aps.autodesk.com/en/docs/informed-design/v1/overview/) REST APIs.

## Documentation

- [API Reference](xref:Autodesk.InformedDesign): Strongly typed API

## Installation

```bash
dotnet add package Adsk.Platform.InformedDesign
```

## Usage

See the [QuickStart Guide](../GetStarted/quickStart.md) for a general understanding.

The root object is [`InformedDesignClient`](xref:Autodesk.InformedDesign.InformedDesignClient). This object exposes manager APIs for products, releases, variants, outputs, uploads, downloads, and rules, plus the generated Fluent API on `Api`.

```csharp
using Autodesk.InformedDesign;

async Task<string> getAccessToken()
{
    //return access token with your logic
}

var client = new InformedDesignClient(getAccessToken);

// e.g. client.ProductsManager, client.Api, etc.
```
