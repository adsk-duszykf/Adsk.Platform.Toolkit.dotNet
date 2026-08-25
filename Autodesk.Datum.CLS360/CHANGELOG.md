# Changelog - Adsk.Platform.Datum.CLS360

All notable changes to this package are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [1.1.0]

### Added

- Multi-targeting: package now also targets `net10.0` in addition to `net8.0`.

---

## [1.0.0]

### Added

- **`DatumCLS360Client`** - unified entry point for the CLS360 API. Accepts a tenant name, access-token provider, and optional `HttpClient`; configures requests for `https://{tenantName}.cls360.io/api`.
- **`DomainManager`** - list, get, seed, and retrieve snapshots of domains.
- **`ClassManager`** - list, create, get, and update classes, including attribute, output, and UoM mappings.
- **`TemplateManager`** - list, create, get, and update templates; retrieve essential attributes by item type.
- **`AttributeManager`** - retrieve information and measure attributes for a domain.
- **`FileManager`** - download files by file handle or dataset reference.
- **Kiota-generated fluent client** - direct access to the full CLS360 REST API through `client.Api`.

---