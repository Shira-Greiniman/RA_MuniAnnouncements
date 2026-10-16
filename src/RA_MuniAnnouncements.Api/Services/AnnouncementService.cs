using Microsoft.Extensions.Logging;
using RA_MuniAnnouncements.Api.Clients;
using RA_MuniAnnouncements.Api.Models;
using RA_MuniAnnouncements.Api.Repositories;

namespace RA_MuniAnnouncements.Api.Services;

/// <summary>Coordinates CMS announcements, authority settings, and normalization.</summary>
public sealed class AnnouncementService(
    ICmsClient cmsClient,
    ISiteSettingsRepository settingsRepository,
    IAnnouncementNormalizer normalizer,
    ILogger<AnnouncementService> logger) : IAnnouncementService
{
    /// <inheritdoc />
    public async Task<AnnouncementsResponse> GetAsync(string authorityCode, CancellationToken ct)
    {
        var announcementsTask = cmsClient.GetAnnouncementsAsync(authorityCode, ct);
        var settingsTask = settingsRepository.GetAsync(authorityCode, ct);

        await Task.WhenAll(announcementsTask, settingsTask);

        var settings = await settingsTask;
        if (settings is null)
        {
            throw new AuthorityNotFoundException(authorityCode);
        }

        var announcements = await announcementsTask;
        var authorityAnnouncements = announcements
            .Where(item => string.Equals(item.AuthorityCode, authorityCode, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        var droppedCount = announcements.Count - authorityAnnouncements.Length;

        if (droppedCount > 0)
        {
            logger.LogWarning(
                "Filtered {DroppedCount} CMS announcement(s) with a mismatched authority code.",
                droppedCount);
        }

        return normalizer.Normalize(authorityAnnouncements, settings);
    }
}