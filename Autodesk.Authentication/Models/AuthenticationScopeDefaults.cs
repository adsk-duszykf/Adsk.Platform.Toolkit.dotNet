namespace Autodesk.Authentication.Helpers.Models;

/// <summary>
/// Provides default scope string constants for use with <c>IEnumerable&lt;string&gt;</c> overloads.
/// </summary>
public static class AuthenticationScopeDefaults
{
    /// <summary>Read data from the platform.</summary>
    public static readonly string DataRead = "data:read";
    /// <summary>Write data to the platform.</summary>
    public static readonly string DataWrite = "data:write";
    /// <summary>Create data on the platform.</summary>
    public static readonly string DataCreate = "data:create";
    /// <summary>Search data on the platform.</summary>
    public static readonly string DataSearch = "data:search";

    /// <summary>Create OSS buckets.</summary>
    public static readonly string BucketCreate = "bucket:create";
    /// <summary>Read OSS bucket metadata and list objects.</summary>
    public static readonly string BucketRead = "bucket:read";
    /// <summary>Update OSS bucket policies.</summary>
    public static readonly string BucketUpdate = "bucket:update";
    /// <summary>Delete OSS buckets.</summary>
    public static readonly string BucketDelete = "bucket:delete";

    /// <summary>Author and execute code on Design Automation.</summary>
    public static readonly string CodeAll = "code:all";

    /// <summary>Read BIM 360 / ACC account data.</summary>
    public static readonly string AccountRead = "account:read";
    /// <summary>Write BIM 360 / ACC account data.</summary>
    public static readonly string AccountWrite = "account:write";

    /// <summary>OpenID Connect scope for ID tokens.</summary>
    public static readonly string OpenId = "openid";

    /// <summary>Read the end user's profile data (excluding email).</summary>
    public static readonly string UserProfileRead = "user-profile:read";

    /// <summary>Read the end user's profile data (including email).</summary>
    public static readonly string UserRead = "user:read";
    /// <summary>Update the end user's profile data.</summary>
    public static readonly string UserWrite = "user:write";

    /// <summary>Access viewable resources such as thumbnails.</summary>
    public static readonly string ViewablesRead = "viewables:read";


}
