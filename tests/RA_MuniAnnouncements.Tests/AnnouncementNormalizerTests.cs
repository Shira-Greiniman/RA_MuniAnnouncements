using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using RA_MuniAnnouncements.Api.Models;
using RA_MuniAnnouncements.Api.Services;
using Xunit;

namespace RA_MuniAnnouncements.Tests;

public sealed class AnnouncementNormalizerTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 15, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Normalize_ExcludesNonPublishedItemsCaseInsensitively()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var items = new[]
        {
            BuildAnnouncement("draft", status: "Draft"),
            BuildAnnouncement("published", status: "pUbLiShEd")
        };

        // Act
        var result = normalizer.Normalize(items, BuildSettings());

        // Assert
        Assert.Equal(new[] { "published" }, result.Announcements.Select(item => item.Id));
    }

    [Fact]
    public void Normalize_ExcludesFutureItems()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var items = new[]
        {
            BuildAnnouncement("past", publishDate: Now.AddMinutes(-1)),
            BuildAnnouncement("future", publishDate: Now.AddTicks(1))
        };

        // Act
        var result = normalizer.Normalize(items, BuildSettings());

        // Assert
        Assert.Equal(new[] { "past" }, result.Announcements.Select(item => item.Id));
    }

    [Fact]
    public void Normalize_ExcludesExpiredItemsAndKeepsNoExpiryItems()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var items = new[]
        {
            BuildAnnouncement("expired", expiryDate: Now.AddTicks(-1)),
            BuildAnnouncement("expires-now", expiryDate: Now),
            BuildAnnouncement("no-expiry")
        };

        // Act
        var result = normalizer.Normalize(items, BuildSettings());

        // Assert
        Assert.Equal(new[] { "no-expiry" }, result.Announcements.Select(item => item.Id));
    }

    [Fact]
    public void Normalize_ExcludesCategoriesNotEnabled()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var settings = BuildSettings(enabledCategories: ["roads"]);
        var items = new[]
        {
            BuildAnnouncement("enabled", category: "roads"),
            BuildAnnouncement("disabled", category: "events")
        };

        // Act
        var result = normalizer.Normalize(items, settings);

        // Assert
        Assert.Equal(new[] { "enabled" }, result.Announcements.Select(item => item.Id));
    }

    [Fact]
    public void Normalize_MapsCategoryNameAndFallsBackToCode()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var settings = BuildSettings(
            enabledCategories: ["roads", "events"],
            categoryDisplayNames: new Dictionary<string, string> { ["roads"] = "Roads and traffic" });
        var items = new[]
        {
            BuildAnnouncement("mapped", category: "roads"),
            BuildAnnouncement("fallback", category: "events")
        };

        // Act
        var result = normalizer.Normalize(items, settings);

        // Assert
        Assert.Equal("Roads and traffic", result.Announcements.Single(item => item.Id == "mapped").CategoryName);
        Assert.Equal("events", result.Announcements.Single(item => item.Id == "fallback").CategoryName);
    }

    [Fact]
    public void Normalize_MarksOnlyUrgentCategories()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var settings = BuildSettings(urgentCategories: ["water"]);
        var items = new[]
        {
            BuildAnnouncement("urgent", category: "WATER"),
            BuildAnnouncement("regular", category: "roads")
        };

        // Act
        var result = normalizer.Normalize(items, settings);

        // Assert
        Assert.True(result.Announcements.Single(item => item.Id == "urgent").IsUrgent);
        Assert.False(result.Announcements.Single(item => item.Id == "regular").IsUrgent);
    }

    [Fact]
    public void Normalize_SortsUrgentThenPublishDateThenId()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var settings = BuildSettings(urgentCategories: ["water"], maxItems: 10);
        var items = new[]
        {
            BuildAnnouncement("regular-newest", category: "roads", publishDate: Now.AddDays(-1)),
            BuildAnnouncement("urgent-older", category: "water", publishDate: Now.AddDays(-4)),
            BuildAnnouncement("urgent-z", category: "water", publishDate: Now.AddDays(-3)),
            BuildAnnouncement("urgent-a", category: "water", publishDate: Now.AddDays(-3))
        };

        // Act
        var result = normalizer.Normalize(items, settings);

        // Assert
        Assert.Equal(new[] { "urgent-a", "urgent-z", "urgent-older", "regular-newest" },
            result.Announcements.Select(item => item.Id));
    }

    [Fact]
    public void Normalize_AppliesMaxItemsAfterSorting()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var settings = BuildSettings(urgentCategories: ["water"], maxItems: 2);
        var items = new[]
        {
            BuildAnnouncement("old-urgent", category: "water", publishDate: Now.AddDays(-20)),
            BuildAnnouncement("newer-regular", category: "roads", publishDate: Now.AddDays(-1)),
            BuildAnnouncement("newest-regular", category: "roads", publishDate: Now.AddDays(-2))
        };

        // Act
        var result = normalizer.Normalize(items, settings);

        // Assert
        Assert.Equal(new[] { "old-urgent", "newer-regular" }, result.Announcements.Select(item => item.Id));
    }

    [Fact]
    public void Normalize_UsesDefaultMaxItemsWhenLimitIsNonPositive()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var settings = BuildSettings(maxItems: 0);
        var items = Enumerable.Range(1, 12)
            .Select(index => BuildAnnouncement($"item-{index:D2}", publishDate: Now.AddDays(-index)))
            .ToArray();

        // Act
        var result = normalizer.Normalize(items, settings);

        // Assert
        Assert.Equal(10, result.Announcements.Count);
        Assert.Equal("item-01", result.Announcements[0].Id);
        Assert.Equal("item-10", result.Announcements[^1].Id);
    }

    [Fact]
    public void Normalize_StripsHtmlScriptsStylesDecodesEntitiesAndCollapsesWhitespace()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var item = BuildAnnouncement(
            "html",
            body: "<p> One   &amp; <b>two</b>&nbsp;three\n four</p><script>bad()</script><style>hidden</style>");

        // Act
        var result = normalizer.Normalize([item], BuildSettings());

        // Assert
        Assert.Equal("One & two three four", Assert.Single(result.Announcements).Summary);
    }

    [Fact]
    public void Normalize_SeparatesTextFromAdjacentHtmlElements()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var item = BuildAnnouncement("adjacent-elements", body: "<h2>A</h2><p>B</p><div>C</div>");

        // Act
        var result = normalizer.Normalize([item], BuildSettings());

        // Assert
        Assert.Equal("A B C", Assert.Single(result.Announcements).Summary);
    }

    [Fact]
    public void Normalize_LimitsSummaryTo150CharactersAndLeavesShortBodyUnchanged()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var wordBody = string.Join(' ', Enumerable.Repeat("word", 40));
        var noSpaceBody = new string('x', 200);
        var items = new[]
        {
            BuildAnnouncement("words", body: wordBody),
            BuildAnnouncement("no-spaces", body: noSpaceBody),
            BuildAnnouncement("short", body: "A short body.")
        };

        // Act
        var result = normalizer.Normalize(items, BuildSettings());

        // Assert
        var wordSummary = result.Announcements.Single(item => item.Id == "words").Summary;
        var noSpaceSummary = result.Announcements.Single(item => item.Id == "no-spaces").Summary;
        Assert.Equal(string.Concat(string.Join(' ', Enumerable.Repeat("word", 30)), "…"), wordSummary);
        Assert.Equal(150, wordSummary.Length);
        Assert.Equal(string.Concat(new string('x', 149), "…"), noSpaceSummary);
        Assert.Equal(150, noSpaceSummary.Length);
        Assert.Equal("A short body.", result.Announcements.Single(item => item.Id == "short").Summary);
    }

    [Fact]
    public void Normalize_ReturnsEmptySummaryForEmptyOrNullBody()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var items = new[]
        {
            BuildAnnouncement("empty", body: string.Empty),
            BuildAnnouncement("null", body: null)
        };

        // Act
        var result = normalizer.Normalize(items, BuildSettings());

        // Assert
        Assert.Equal(2, result.Announcements.Count);
        Assert.All(result.Announcements, item => Assert.Empty(item.Summary));
    }

    [Fact]
    public void Normalize_FormatsPublishDateInJerusalemTime()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var item = BuildAnnouncement("jerusalem-date", publishDate: new DateTimeOffset(2026, 10, 14, 22, 30, 0, TimeSpan.Zero));

        // Act
        var result = normalizer.Normalize([item], BuildSettings());

        // Assert
        Assert.Equal("15/10/2026", Assert.Single(result.Announcements).PublishDate);
    }

    [Fact]
    public void Normalize_EmptyInputCopiesAuthorityDetails()
    {
        // Arrange
        var normalizer = CreateNormalizer(CreateClock());
        var settings = BuildSettings();

        // Act
        var result = normalizer.Normalize([], settings);

        // Assert
        Assert.Empty(result.Announcements);
        Assert.Equal(settings.AuthorityCode, result.AuthorityCode);
        Assert.Equal(settings.AuthorityName, result.AuthorityName);
    }

    private static FakeTimeProvider CreateClock() => new(Now);

    private static AnnouncementNormalizer CreateNormalizer(FakeTimeProvider clock) =>
        new(clock, NullLogger<AnnouncementNormalizer>.Instance);

    private static CmsAnnouncement BuildAnnouncement(
        string id,
        string category = "roads",
        DateTimeOffset? publishDate = null,
        DateTimeOffset? expiryDate = null,
        string status = "Published",
        string? body = "Announcement body") =>
        new(id, "6500", $"Title {id}", body!, category, status, publishDate ?? Now.AddDays(-1), expiryDate);

    private static AuthoritySiteSettings BuildSettings(
        IReadOnlyList<string>? enabledCategories = null,
        IReadOnlyDictionary<string, string>? categoryDisplayNames = null,
        IReadOnlyList<string>? urgentCategories = null,
        int maxItems = 5) =>
        new(
            "6500",
            "Hadera",
            enabledCategories ?? new[] { "roads", "water", "events" },
            categoryDisplayNames ?? new Dictionary<string, string>
            {
                ["roads"] = "Roads",
                ["water"] = "Water",
                ["events"] = "Events"
            },
            urgentCategories ?? new[] { "water" },
            maxItems);
}