namespace RA_MuniAnnouncements.Api.Models;

/// <summary>Represents an announcement formatted for the public API.</summary>
/// <param name="Id">The announcement identifier.</param>
/// <param name="Title">The announcement title.</param>
/// <param name="Summary">A short announcement summary.</param>
/// <param name="CategoryName">The category display name.</param>
/// <param name="IsUrgent">Whether the announcement is urgent.</param>
/// <param name="PublishDate">The publication date formatted as dd/MM/yyyy.</param>
public sealed record AnnouncementDto(
    string Id,
    string Title,
    string Summary,
    string CategoryName,
    bool IsUrgent,
    string PublishDate);