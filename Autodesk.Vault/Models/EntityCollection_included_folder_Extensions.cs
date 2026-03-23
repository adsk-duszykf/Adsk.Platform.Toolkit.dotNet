using System.Text.Json;
using Microsoft.Kiota.Serialization;

namespace Autodesk.Vault.Models;
/// <summary>
/// Extension methods for parsing the included folder data from search results into a strongly-typed <see cref="EntityCollection_included_folder_data"/> object.
/// </summary>
public partial class EntityCollection_included_folder
{
    /// <summary>
    /// Parses the included folder data from search results into a strongly-typed <see cref="EntityCollection_included_folder_data"/> object.
    /// This method serializes the raw <see cref="EntityCollection_included_folder"/> node
    /// </summary>
    /// <returns>A <see cref="EntityCollection_included_folder_data"/> object</returns>
    public async Task<EntityCollection_included_folder_data?> ParseAsync()
    {

            var json = await this.SerializeAsJsonStringAsync();
            return JsonSerializer.Deserialize<EntityCollection_included_folder_data>(json);
    }

}