namespace RA_MuniAnnouncements.Api.Models;

/// <summary>Contains the announcements returned for an authority.</summary>
/// <param name="AuthorityCode">The authority code.</param>
/// <param name="AuthorityName">The authority display name.</param>
/// <param name="Announcements">The formatted announcements.</param>
public sealed record AnnouncementsResponse(
    string AuthorityCode,
    string AuthorityName,
    IReadOnlyList<AnnouncementDto> Announcements);