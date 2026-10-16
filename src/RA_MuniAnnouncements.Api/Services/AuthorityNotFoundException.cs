namespace RA_MuniAnnouncements.Api.Services;

/// <summary>Indicates that an authority could not be found.</summary>
public sealed class AuthorityNotFoundException : Exception
{
    /// <summary>Creates an exception for a missing authority.</summary>
    /// <param name="authorityCode">The code of the authority that was not found.</param>
    public AuthorityNotFoundException(string authorityCode)
        : base($"Authority '{authorityCode}' was not found.")
    {
    }
}