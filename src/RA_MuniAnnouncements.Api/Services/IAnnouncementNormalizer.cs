using RA_MuniAnnouncements.Api.Models;

namespace RA_MuniAnnouncements.Api.Services;

/// <summary>Defines formatting of CMS announcements for the public API.</summary>
public interface IAnnouncementNormalizer
{
    /// <summary>Normalizes announcements using the authority's settings.</summary>
    /// <param name="items">The announcements to normalize.</param>
    /// <param name="settings">The authority's display settings.</param>
    /// <returns>The normalized announcements response.</returns>
    AnnouncementsResponse Normalize(IEnumerable<CmsAnnouncement> items, AuthoritySiteSettings settings);
}