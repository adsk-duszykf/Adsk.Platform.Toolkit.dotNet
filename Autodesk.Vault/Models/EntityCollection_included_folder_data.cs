using System.Text.Json.Serialization;

#pragma warning disable CS1591
namespace Autodesk.Vault.Models;

/// <summary>
/// Typed representation of the included folder dictionary from search results.
/// Maps folder IDs (as strings) to <see cref="FolderEntry"/> objects.
/// </summary>
public class EntityCollection_included_folder_data : Dictionary<string, EntityCollection_included_folder_data.FolderEntry>
{
    public class FolderEntry
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("createDate")]
        public string? CreateDate { get; set; }

        [JsonPropertyName("createUserName")]
        public string? CreateUserName { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("categoryColor")]
        public double? CategoryColor { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("stateColor")]
        public double? StateColor { get; set; }

        [JsonPropertyName("subfolderCount")]
        public double? SubfolderCount { get; set; }

        [JsonPropertyName("children")]
        public string? Children { get; set; }

        [JsonPropertyName("isLibrary")]
        public bool? IsLibrary { get; set; }

        [JsonPropertyName("isReadOnly")]
        public bool? IsReadOnly { get; set; }

        [JsonPropertyName("isCloaked")]
        public bool? IsCloaked { get; set; }

        [JsonPropertyName("properties")]
        public List<PropertyEntry>? Properties { get; set; }
    }

    public class PropertyEntry
    {
        [JsonPropertyName("propertyDefinitionId")]
        public string? PropertyDefinitionId { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}
#pragma warning restore CS1591
