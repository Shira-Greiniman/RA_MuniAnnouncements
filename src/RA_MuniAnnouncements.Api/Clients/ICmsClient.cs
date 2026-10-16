using RA_MuniAnnouncements.Api.Models;

namespace RA_MuniAnnouncements.Api.Clients;

/// <summary>Retrieves announcements from the CMS.</summary>
public interface ICmsClient
{
    /// <summary>Gets announcements for an authority.</summary>
    /// <param name="authorityCode">The authority code.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The announcements returned by the CMS.</returns>
    Task<IReadOnlyList<CmsAnnouncement>> GetAnnouncementsAsync(string authorityCode, CancellationToken ct);
}