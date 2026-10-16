namespace RA_MuniAnnouncements.Api.Models;

/// <summary>Contains the announcement display settings for an authority.</summary>
/// <param name="AuthorityCode">The authority code.</param>
/// <param name="AuthorityName">The authority display name.</param>
/// <param name="EnabledCategories">The category keys enabled for display.</param>
/// <param name="CategoryDisplayNames">Display names keyed by category.</param>
/// <param name="UrgentCategories">The category keys treated as urgent.</param>
/// <param name="MaxItems">The maximum number of announcements to return.</param>
public sealed record AuthoritySiteSettings(
    string AuthorityCode,
    string AuthorityName,
    IReadOnlyList<string> EnabledCategories,
    IReadOnlyDictionary<string, string> CategoryDisplayNames,
    IReadOnlyList<string> UrgentCategories,
    int MaxItems);