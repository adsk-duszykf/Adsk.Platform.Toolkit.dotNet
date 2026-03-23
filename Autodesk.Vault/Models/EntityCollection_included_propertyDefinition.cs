using System.Text.Json;
using Microsoft.Kiota.Serialization;

namespace Autodesk.Vault.Models;

public partial class EntityCollection_included_propertyDefinition
{
    /// <summary>
    /// Parses the included property definition data from search results into a strongly-typed <see cref="EntityCollection_included_propertyDefinition_data"/> object.
    /// This method serializes the raw <see cref="EntityCollection_included_propertyDefinition"/>
    /// </summary>
    /// <returns>A <see cref="EntityCollection_included_propertyDefinition_data"/> object.</returns>
    public async Task<EntityCollection_included_propertyDefinition_data?> ParseAsync()
    {

            var json = await this.SerializeAsJsonStringAsync();
            return JsonSerializer.Deserialize<EntityCollection_included_propertyDefinition_data>(json);

    }
}
