using RA_MuniAnnouncements.Api.Models;

namespace RA_MuniAnnouncements.Api.Repositories;

/// <summary>Loads site settings for an authority.</summary>
public interface ISiteSettingsRepository
{
    /// <summary>Gets the settings for an authority.</summary>
    /// <param name="authorityCode">The authority code.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The settings, or null when they are not found.</returns>
    Task<AuthoritySiteSettings?> GetAsync(string authorityCode, CancellationToken ct);
}