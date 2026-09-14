# Changelog - Adsk.Platform.Datum.CLS360

All notable changes to this package are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [2.0.1]

### Fixed

- Re-packaged to fix invalid `Adsk.Platform.HttpClient` dependency version metadata generated during CI workflow.

---

## [2.0.0]

### Fixed

- Corrected several CLS360 OpenAPI response schemas that did not match the actual API
  (verified against the [official CLS360 API docs](https://github.com/datum360/Datum360-API-User-Guides/tree/main/CLS360)):
  - `ClassManager.ListClassesAsync` — the `value` field was previously modeled as a single
    object instead of an array, causing deserialization failures.
  - `TemplateManager.ListTemplatesAsync` — the response actually returns `rows`/`header`
    (like `OutputMappings`/`AttributeMappings`), not `value`/`@nextLink`.
  - `AttributeManager.GetAttributesForDomainAsync` — response is returned under HTTP `200`, not `201`.
  - `DomainManager` — `Company`, `Logo`, `Lock` fields were missing from domain objects.
  - `ClassManager.GetClassAsync`/`GetUoMGroupAsync` and `TemplateManager.GetTemplateAsync` —
    added missing `detail.key`, `options` (on metadata/associations entries), and top-level
    `usedby`/`history` fields.

### Changed (breaking)

- **`ClassManager.CreateClassAsync`** and **`ClassManager.UpdateClassAsync`** now return
  `Task<string?>` instead of `Task<ClassesPostResponse?>`/`Task<WithClsHdlPostResponse?>` —
  the API actually returns the new handle (create) or `"OK"`/an error message (update) as a
  plain string, not the full class object.
- **`TemplateManager.CreateTemplateAsync`** and **`TemplateManager.UpdateTemplateAsync`**
  now return `Task<string?>` for the same reason.
- **`TemplateManager.ListTemplatesAsync`** now yields `TemplatesGetResponse_rows` instead of
  `TemplatesGetResponse_value`, and performs a single request — the endpoint has no
  `page`/`pageSize` parameters, so it was never actually paginated.

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