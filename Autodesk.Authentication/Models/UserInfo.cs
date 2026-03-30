namespace Autodesk.Authentication.Helpers.Models;


/// <summary>
/// Represents the authenticated user's profile information returned by the Autodesk userinfo endpoint.
/// </summary>
public class UserInfo
{
    /// <summary>The email property</summary>
    public string? Email { get; set; }

    /// <summary>The email_verified property</summary>
    public bool? EmailVerified { get; set; }
    /// <summary>The family_name property</summary>

    public string? FamilyName { get; set; }

    /// <summary>The given_name property</summary>

    public string? GivenName { get; set; }
    /// <summary>The locale property</summary>
    public string? Locale { get; set; }
    /// <summary>The name property</summary>
    public string? Name { get; set; }
    /// <summary>The picture property</summary>
    public string? Picture { get; set; }
    /// <summary>The preferred_username property</summary>
    public string? PreferredUsername { get; set; }
    /// <summary>The profile property</summary>
    public string? Profile { get; set; }
    /// <summary>The sub property</summary>
    public string? Sub { get; set; }
    /// <summary>The updated_at property</summary>
    public int? UpdatedAt { get; set; }
}

