using RA_MuniAnnouncements.Api.Models;

namespace RA_MuniAnnouncements.Api.Repositories;

/// <summary>Provides hard-coded site settings for supported authorities.</summary>
public sealed class MockSiteSettingsRepository : ISiteSettingsRepository
{
    /// <inheritdoc />
    public async Task<AuthoritySiteSettings?> GetAsync(string authorityCode, CancellationToken ct)
    {
        await Task.Delay(10, ct);

        return authorityCode switch
        {
            "6500" => new AuthoritySiteSettings(
                "6500",
                "עיריית חדרה",
                ["roads", "water", "events"],
                new Dictionary<string, string>
                {
                    ["roads"] = "תחבורה וכבישים",
                    ["water"] = "מים וביוב",
                    ["events"] = "אירועים"
                },
                ["water"],
                5),
            "5000" => new AuthoritySiteSettings(
                "5000",
                "עיריית תל אביב-יפו",
                ["roads"],
                new Dictionary<string, string>
                {
                    ["roads"] = "תחבורה וכבישים"
                },
                [],
                3),
            _ => null
        };
    }
}