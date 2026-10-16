using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using RA_MuniAnnouncements.Api.Models;

namespace RA_MuniAnnouncements.Api.Services;

/// <summary>Filters and formats CMS announcements for public display.</summary>
public sealed class AnnouncementNormalizer(
    TimeProvider timeProvider,
    ILogger<AnnouncementNormalizer> logger) : IAnnouncementNormalizer
{
    private const int DefaultMaxItems = 10;
    private const int MaxSummaryLength = 150;
    private const string JerusalemTimeZoneId = "Asia/Jerusalem";
    private const string WindowsJerusalemTimeZoneId = "Israel Standard Time";

    /// <inheritdoc />
    public AnnouncementsResponse Normalize(IEnumerable<CmsAnnouncement> items, AuthoritySiteSettings settings)
    {
        var now = timeProvider.GetUtcNow();
        var enabledCategories = new HashSet<string>(settings.EnabledCategories, StringComparer.OrdinalIgnoreCase);
        var urgentCategories = new HashSet<string>(settings.UrgentCategories, StringComparer.OrdinalIgnoreCase);
        var timeZone = GetJerusalemTimeZone();
        var maxItems = settings.MaxItems > 0 ? settings.MaxItems : DefaultMaxItems;

        // Spec 4.1: Include only published, currently valid announcements in enabled categories.
        var normalized = items
            .Where(item =>
                string.Equals(item.Status, "Published", StringComparison.OrdinalIgnoreCase)
                && item.PublishDate <= now
                && (item.ExpiryDate is null || item.ExpiryDate > now)
                && enabledCategories.Contains(item.Category))
            // Spec 4.2: Keep urgency as the primary sort, then publication date and ID; apply the configured limit.
            .Select(item => new
            {
                Dto = MapToDto(item, settings, urgentCategories, timeZone),
                item.PublishDate
            })
            .OrderByDescending(item => item.Dto.IsUrgent)
            .ThenByDescending(item => item.PublishDate)
            .ThenBy(item => item.Dto.Id, StringComparer.Ordinal)
            .Take(maxItems)
            .Select(item => item.Dto)
            .ToArray();

        return new AnnouncementsResponse(settings.AuthorityCode, settings.AuthorityName, normalized);
    }

    private AnnouncementDto MapToDto(
        CmsAnnouncement item,
        AuthoritySiteSettings settings,
        HashSet<string> urgentCategories,
        TimeZoneInfo timeZone)
    {
        // Spec 4.3: Use the category code when no display name is configured and log the fallback.
        if (!settings.CategoryDisplayNames.TryGetValue(item.Category, out var categoryName))
        {
            categoryName = settings.CategoryDisplayNames
                .FirstOrDefault(entry => string.Equals(entry.Key, item.Category, StringComparison.OrdinalIgnoreCase))
                .Value;

            if (categoryName is null)
            {
                categoryName = item.Category;
                logger.LogWarning(
                    "No display name configured for announcement category {Category}; using the category code.",
                    item.Category);
            }
        }

        return new AnnouncementDto(
            item.Id,
            item.Title,
            CreateSummary(item.Body),
            categoryName,
            urgentCategories.Contains(item.Category),
            TimeZoneInfo.ConvertTime(item.PublishDate, timeZone)
                .ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
    }

    private static string CreateSummary(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return string.Empty;
        }

        // Spec 4.4: Extract readable HTML text, discard scripts/styles, decode entities and collapse whitespace.
        var document = new HtmlDocument();
        document.LoadHtml(body);

        foreach (var node in document.DocumentNode.SelectNodes("//script|//style") ?? Enumerable.Empty<HtmlNode>())
        {
            node.Remove();
        }

        var textNodes = document.DocumentNode.SelectNodes("//text()");
        var text = textNodes is null
            ? string.Empty
            : string.Join(" ", textNodes.Select(node => node.InnerText));
        text = WebUtility.HtmlDecode(text).Replace('\u00A0', ' ');
        text = Regex.Replace(text, @"\s+", " ").Trim();

        if (text.Length <= MaxSummaryLength)
        {
            return text;
        }

        var maxTextLength = MaxSummaryLength - 1;
        var lastSpace = text.LastIndexOf(' ', maxTextLength);
        var truncateAt = lastSpace > 0 ? lastSpace : maxTextLength;
        return string.Concat(text.AsSpan(0, truncateAt).TrimEnd(), "…");
    }

    private static TimeZoneInfo GetJerusalemTimeZone()
    {
        // Spec 4.5: Format dates in Jerusalem time with a Windows zone fallback.
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(JerusalemTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(WindowsJerusalemTimeZoneId);
        }
    }
}