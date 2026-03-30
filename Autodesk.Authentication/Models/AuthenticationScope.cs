using System.Runtime.Serialization;

namespace Autodesk.Authentication.Helpers.Models
{
    /// <summary>
    /// Defines the available OAuth 2.0 scopes for Autodesk Platform Services.
    /// </summary>
    public enum AuthenticationScope
    {
        /// <summary>Read the end user's profile data (excluding email).</summary>
        [EnumMember(Value = "user-profile:read")]
        UserProfileRead,
        /// <summary>Read the end user's profile data (including email).</summary>
        [EnumMember(Value = "user:read")]
        UserRead,
        /// <summary>Update the end user's profile data.</summary>
        [EnumMember(Value = "user:write")]
        UserWrite,
        /// <summary>Access viewable resources such as thumbnails.</summary>
        [EnumMember(Value = "viewables:read")]
        ViewablesRead,
        /// <summary>Read data from the platform (OSS, Data Management).</summary>
        [EnumMember(Value = "data:read")]
        DataRead,
        /// <summary>Write data to the platform.</summary>
        [EnumMember(Value = "data:write")]
        DataWrite,
        /// <summary>Create data on the platform.</summary>
        [EnumMember(Value = "data:create")]
        DataCreate,
        /// <summary>Search data on the platform.</summary>
        [EnumMember(Value = "data:search")]
        DataSearch,
        /// <summary>Create OSS buckets.</summary>
        [EnumMember(Value = "bucket:create")]
        BucketCreate,
        /// <summary>Read OSS bucket metadata and list objects.</summary>
        [EnumMember(Value = "bucket:read")]
        BucketRead,
        /// <summary>Update OSS bucket policies.</summary>
        [EnumMember(Value = "bucket:update")]
        BucketUpdate,
        /// <summary>Delete OSS buckets.</summary>
        [EnumMember(Value = "bucket:delete")]
        BucketDelete,
        /// <summary>Author and execute code on Design Automation.</summary>
        [EnumMember(Value = "code:all")]
        CodeAll,
        /// <summary>Read BIM 360 / ACC account data.</summary>
        [EnumMember(Value = "account:read")]
        AccountRead,
        /// <summary>Write BIM 360 / ACC account data.</summary>
        [EnumMember(Value = "account:write")]
        AccountWrite,
        /// <summary>OpenID Connect scope for ID tokens.</summary>
        [EnumMember(Value = "openid")]
        OpenId,
    }
}
