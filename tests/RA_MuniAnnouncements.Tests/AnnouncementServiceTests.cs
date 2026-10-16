using Microsoft.Extensions.Logging.Abstractions;
using RA_MuniAnnouncements.Api.Clients;
using RA_MuniAnnouncements.Api.Models;
using RA_MuniAnnouncements.Api.Repositories;
using RA_MuniAnnouncements.Api.Services;
using Xunit;

namespace RA_MuniAnnouncements.Tests;

public sealed class AnnouncementServiceTests
{
    [Fact]
    public async Task GetAsync_UnknownAuthority_ThrowsAuthorityNotFoundException()
    {
        // Arrange
        var cmsClient = new FakeCmsClient([]);
        var settingsRepository = new FakeSiteSettingsRepository(null);
        var service = CreateService(cmsClient, settingsRepository, new FakeAnnouncementNormalizer());

        // Act
        var exception = await Assert.ThrowsAsync<AuthorityNotFoundException>(
            () => service.GetAsync("9999", CancellationToken.None));

        // Assert
        Assert.Equal("Authority '9999' was not found.", exception.Message);
    }

    [Fact]
    public async Task GetAsync_FiltersForeignTenantItemsBeforeNormalization()
    {
        // Arrange
        var cmsClient = new FakeCmsClient(
        [
            BuildAnnouncement("local", "6500"),
            BuildAnnouncement("foreign", "5000")
        ]);
        var settingsRepository = new FakeSiteSettingsRepository(BuildSettings());
        var normalizer = new FakeAnnouncementNormalizer();
        var service = CreateService(cmsClient, settingsRepository, normalizer);

        // Act
        var response = await service.GetAsync("6500", CancellationToken.None);

        // Assert
        Assert.Equal(new[] { "local" }, normalizer.ReceivedItems!.Select(item => item.Id));
        Assert.Equal("6500", response.AuthorityCode);
    }

    private static AnnouncementService CreateService(
        ICmsClient cmsClient,
        ISiteSettingsRepository settingsRepository,
        IAnnouncementNormalizer normalizer) =>
        new(cmsClient, settingsRepository, normalizer, NullLogger<AnnouncementService>.Instance);

    private static CmsAnnouncement BuildAnnouncement(string id, string authorityCode) =>
        new(id, authorityCode, $"Title {id}", "Body", "roads", "Published", DateTimeOffset.UtcNow, null);

    private static AuthoritySiteSettings BuildSettings() =>
        new(
            "6500",
            "Hadera",
            ["roads"],
            new Dictionary<string, string> { ["roads"] = "Roads" },
            [],
            5);

    private sealed class FakeCmsClient(IReadOnlyList<CmsAnnouncement> announcements) : ICmsClient
    {
        public Task<IReadOnlyList<CmsAnnouncement>> GetAnnouncementsAsync(string authorityCode, CancellationToken ct) =>
            Task.FromResult(announcements);
    }

    private sealed class FakeSiteSettingsRepository(AuthoritySiteSettings? settings) : ISiteSettingsRepository
    {
        public Task<AuthoritySiteSettings?> GetAsync(string authorityCode, CancellationToken ct) =>
            Task.FromResult(settings);
    }

    private sealed class FakeAnnouncementNormalizer : IAnnouncementNormalizer
    {
        public IReadOnlyList<CmsAnnouncement>? ReceivedItems { get; private set; }

        public AnnouncementsResponse Normalize(IEnumerable<CmsAnnouncement> items, AuthoritySiteSettings settings)
        {
            ReceivedItems = items.ToArray();
            return new AnnouncementsResponse(settings.AuthorityCode, settings.AuthorityName, []);
        }
    }
}