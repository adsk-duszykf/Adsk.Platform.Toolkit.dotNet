using System.Text.RegularExpressions;
using Autodesk.DataManagement.Models;

namespace Autodesk.DataManagement.Helpers.Models;

/// <summary>
/// File version data
/// </summary>
/// <param name="FolderId">The ID of the folder containing the file.</param>
/// <param name="FileName">The name of the file.</param>
/// <param name="Id">The ID of the file.</param>
public partial record class FileVersion(string? FolderId, string FileName, string Id) : IHasFileName
{
    /// <summary>
    /// The version of the file.
    /// </summary>
    public int Version => int.Parse(GetVersion().Match(Id).Groups[1].Value);

    /// <summary>
    /// Regex to get the version of the file.
    /// </summary>
    [GeneratedRegex(@"version=(\d+)")]
    private static partial Regex GetVersion();
}

/// <summary>
/// File item data
/// </summary>
/// <param name="Data">File Item data</param>
/// <param name="Included">Tip version of the file item</param>
public record FileItem(FolderContents_data Data, FolderContents_included Included) { }

/// <summary>
/// Sub folder list data
/// </summary>
public record SubFolderList
{
    /// <summary>
    /// The parent folder of the sub folders.
    /// </summary>
    public FolderItem ParentFolder { get; set; } = new();
    /// <summary>
    /// The sub folders of the parent folder.
    /// </summary>
    public List<FolderItem> SubFolders { get; set; } = [];
}
/// <summary>
/// Folder item data
/// </summary>
public record FolderItem
{
    /// <summary>
    /// The hub of the folder item.
    /// </summary>
    public IdNameMap Hub { get; set; } = new();
    /// <summary>
    /// The project of the folder item.
    /// </summary>
    public IdNameMap Project { get; set; } = new();
    /// <summary>
    /// The folder of the folder item.
    /// </summary>
    public IdNameMap Folder { get; set; } = new();
    /// <summary>
    /// The path of the folder item.
    /// </summary>
    public string Path { get; set; } = "";
}

/// <summary>
/// Folder path data
/// </summary>
public record FolderPath
{
    /// <summary>
    /// The hub of the folder path.
    /// </summary>
    public IdNameMap Hub { get; set; } = new();
    /// <summary>
    /// The project of the folder path.
    /// </summary>
    public IdNameMap Project { get; set; } = new();
    /// <summary>
    /// List of folders in the path
    /// </summary>
    public List<IdNameMap> Folders { get; set; } = [];
    /// <summary>
    /// The path of the folder path.
    /// </summary>
    public string Path => string.Join("\\", Folders.Select(f => f.Name));
}
/// <summary>
/// ID and name map data
/// </summary>
/// <param name="Id">The ID of the map.</param>
/// <param name="Name">The name of the map.</param>
public record IdNameMap(string Id = "", string Name = "");

/// <summary>
/// Interface for objects that have a file name
/// </summary>
public interface IHasFileName
{
    /// <summary>
    /// The name of the file.
    /// </summary>
    string FileName { get; }
}