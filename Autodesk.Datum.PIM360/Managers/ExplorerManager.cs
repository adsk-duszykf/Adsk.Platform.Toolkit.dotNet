using Autodesk.Datum.PIM360.Explorer;

namespace Autodesk.Datum.PIM360.Managers;

/// <summary>Manager for attribute explorer operations.</summary>
public sealed class ExplorerManager : Pim360ManagerBase
{
    private readonly BaseDatumPIM360Client _api;

    /// <summary>Initializes a new instance of the <see cref="ExplorerManager"/> class.</summary>
    /// <param name="api">The generated PIM360 API client.</param>
    public ExplorerManager(BaseDatumPIM360Client api) => _api = api;

    /// <summary>Provides access to the generated explorer request builders.</summary>
    public ExplorerRequestBuilder Api => _api.Explorer;
}
