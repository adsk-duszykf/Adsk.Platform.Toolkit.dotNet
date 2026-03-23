using System.Text.Json.Serialization;

#pragma warning disable CS1591
namespace Autodesk.Vault.Models;

/// <summary>
/// Typed representation of the included folder dictionary from search results.
/// Maps folder IDs (as strings) to <see cref="FolderEntry"/> objects.
/// </summary>
public class EntityCollection_included_propertyDefinition_data : Dictionary<string, EntityCollection_included_propertyDefinition_data.FolderEntry>
{
    public class FolderEntry
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("systemName")]
        public string? SystemName { get; set; }

        [JsonPropertyName("dataType")]
        public EntityCollection_included_propertyDefinition_dataType? DataType { get; set; }

        [JsonPropertyName("active")]
        public bool? Active { get; set; }

        [JsonPropertyName("isSystem")]
        public bool? IsSystem { get; set; }

        [JsonPropertyName("initialValue")]
        public string? InitialValue { get; set; }
 
    }

    public enum EntityCollection_included_propertyDefinition_dataType
    {
        String,
        Numeric,
        Bool,
        DateTime,
        Image,

    }
}
#pragma warning restore CS1591
