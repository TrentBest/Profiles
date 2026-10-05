namespace TheSingularityWorkshop.Profiles;

/// <summary>
/// Represents a private relationship between two profiles whose shared identity is not part of ordinary representations.
/// </summary>
/// <remarks>
/// This boundary allows one person or organization to maintain separate public-facing personas,
/// such as a personal profile and a publisher profile. Resolution of the link belongs to trusted
/// identity infrastructure and is not exposed to ordinary observers.
/// </remarks>
public sealed record ProfileIdentityLink(
    ProfileId CanonicalProfileId,
    ProfileId LinkedProfileId,
    ProfileIdentityLinkKind Kind);

/// <summary>Describes why two profiles are privately linked.</summary>
public enum ProfileIdentityLinkKind
{
    /// <summary>A public-facing persona is controlled by the same canonical profile.</summary>
    Persona,

    /// <summary>A publisher-facing identity is controlled by the same canonical profile.</summary>
    Publisher,

    /// <summary>A commercial or organizational identity is controlled by the same canonical profile.</summary>
    Commercial,

    /// <summary>A custom private identity relationship.</summary>
    Custom
}
