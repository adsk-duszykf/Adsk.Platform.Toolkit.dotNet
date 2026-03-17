# Adsk.Platform.VaultData

> **⚠️UNOFFICIAL PACKAGE⚠️**

The package `Adsk.Platform.VaultData` provides a set of APIs to interact with the [Autodesk Vault Server](https://aps.autodesk.com/en/docs/vault/v1/overview/).

## Documentation

- [API Reference](xref:Autodesk.Vault): Strongly typed API

## Installation

```bash
dotnet add package Adsk.Platform.VaultData
```

## Usage

See the [QuickStart Guide](../GetStarted/quickStart.md) for a general understanding.

The root object is [`VaultClient`](xref:Autodesk.Vault.VaultClient). This object provides high-level managers for different API areas (Authentication, Accounts, Files and Folders, Items, Search, Jobs, etc.).

```csharp
using Autodesk.Vault;

async Task<string> getAccessToken()
{
    //return access token with your logic
}

var vaultClient = new VaultClient(getAccessToken, "https://your-vault-server", "user@example.com");

// Access Vault services through manager properties
// e.g. vaultClient.FilesAndFolders, vaultClient.Items, vaultClient.Search, etc.
```
