# Adsk.Platform.Automation

> **⚠️UNOFFICIAL PACKAGE⚠️**

The package `Adsk.Platform.Automation` provides a set of APIs to interact with the [Autodesk Design Automation API](https://aps.autodesk.com/en/docs/design-automation/v3/developers_guide/overview/).

## Documentation

- [API Reference](xref:Autodesk.Automation): Strongly typed API

## Installation

```bash
dotnet add package Adsk.Platform.Automation
```

## Usage

See the [QuickStart Guide](../GetStarted/quickStart.md) for a general understanding.

The root object is [`AutomationClient`](xref:Autodesk.Automation.AutomationClient). This object provides shortcuts to the various Design Automation API endpoints.

```csharp
using Autodesk.Automation;

async Task<string> getAccessToken()
{
    //return access token with your logic
}

var automationClient = new AutomationClient(getAccessToken);

// Access Design Automation services through shortcut properties
// e.g. automationClient.Activities, automationClient.AppBundles, automationClient.WorkItems, etc.
```
