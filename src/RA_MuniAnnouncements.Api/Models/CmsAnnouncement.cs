namespace RA_MuniAnnouncements.Api.Models;

/// <summary>Represents an announcement received from the CMS.</summary>
/// <param name="Id">The CMS announcement identifier.</param>
/// <param name="AuthorityCode">The authority that owns the announcement.</param>
/// <param name="Title">The announcement title.</param>
/// <param name="Body">The announcement body.</param>
/// <param name="Category">The CMS category key.</param>
/// <param name="Status">The CMS publication status.</param>
/// <param name="PublishDate">The announcement publication date.</param>
/// <param name="ExpiryDate">The optional announcement expiry date.</param>
public sealed record CmsAnnouncement(
    string Id,
    string AuthorityCode,
    string Title,
    string Body,
    string Category,
    string Status,
    DateTimeOffset PublishDate,
    DateTimeOffset? ExpiryDate);