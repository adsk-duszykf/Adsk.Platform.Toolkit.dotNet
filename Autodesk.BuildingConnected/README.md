# Autodesk BuildingConnected SDK for .NET

> **⚠️ UNOFFICIAL PACKAGE ⚠️**

A .NET SDK providing a [Fluent API](https://dzone.com/articles/java-fluent-api) for the [BuildingConnected](https://aps.autodesk.com/en/docs/buildingconnected/v2/overview/) APIs, generated from the official OpenAPI specifications using [Microsoft Kiota](https://learn.microsoft.com/en-us/openapi/kiota/overview).

## Features

This SDK provides access to multiple BuildingConnected API endpoints through a unified client:

| API | Endpoint Path |
|-----|---------------|
| **BuildingConnected** | `/construction/buildingconnected/v2/*` |
| **TradeTapp** | `/construction/tradetapp/v2/*` |

## Installation

```bash
dotnet add package Adsk.Platform.BuildingConnected
```

## Quick Start

```csharp
using Autodesk.BuildingConnected;

// Provide a function that returns the access token
Func<Task<string>> getAccessToken = () => Task.FromResult("YOUR_ACCESS_TOKEN");

// Create the client
var client = new BuildingConnectedClient(getAccessToken);

// Use the fluent API to access BuildingConnected endpoints
var projects = await client.BuildingConnected.Projects.GetAsync();
```

## Custom HttpClient

You can optionally provide your own `HttpClient` instance:

```csharp
var httpClient = new HttpClient();
var client = new BuildingConnectedClient(getAccessToken, httpClient);
```
