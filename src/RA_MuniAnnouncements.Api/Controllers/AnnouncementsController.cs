using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using RA_MuniAnnouncements.Api.Models;
using RA_MuniAnnouncements.Api.Services;

namespace RA_MuniAnnouncements.Api.Controllers;

[ApiController]
[Route("api/announcements")]
[Produces("application/json")]
public sealed class AnnouncementsController(IAnnouncementService announcementService) : ControllerBase
{
    [HttpGet("{authorityCode}")]
    [ProducesResponseType(typeof(AnnouncementsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AnnouncementsResponse>> Get(string authorityCode, CancellationToken ct)
    {
        if (!Regex.IsMatch(authorityCode, @"^\d{1,10}$"))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid authority code",
                detail: "The authority code must contain between 1 and 10 digits.");
        }

        try
        {
            return Ok(await announcementService.GetAsync(authorityCode, ct));
        }
        catch (AuthorityNotFoundException)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Authority not found");
        }
    }
}