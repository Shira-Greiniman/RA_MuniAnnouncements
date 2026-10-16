using RA_MuniAnnouncements.Api.Models;

namespace RA_MuniAnnouncements.Api.Services;

/// <summary>Retrieves formatted announcements for an authority.</summary>
public interface IAnnouncementService
{
    /// <summary>Gets the announcements for an authority.</summary>
    /// <param name="authorityCode">The authority code.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The authority and its formatted announcements.</returns>
    Task<AnnouncementsResponse> GetAsync(string authorityCode, CancellationToken ct);
}