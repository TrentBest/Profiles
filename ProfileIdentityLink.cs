namespace TheSingularityWorkshop.Profiles;

/// <summary>
/// Represents a private relationship between two profiles whose shared identity is not part of ordinary representations.
/// </summary>
/// <remarks>
/// This boundary allows one person or organization to maintain separate public-facing personas,
/// such as a personal profile and a publisher profile. Resolution of the link belongs to trusted
/// identity infrastructure and is not exposed to ordinary observers.
/// </remarks>
public sealed record ProfileIdentityLink
{
    /// <summary>Creates a private link between two distinct, initialized profiles.</summary>
    public ProfileIdentityLink(
        ProfileId canonicalProfileId,
        ProfileId linkedProfileId,
        ProfileIdentityLinkKind kind)
    {
        if (canonicalProfileId == default)
            throw new ArgumentException("The canonical profile ID must be initialized.", nameof(canonicalProfileId));
        if (linkedProfileId == default)
            throw new ArgumentException("The linked profile ID must be initialized.", nameof(linkedProfileId));
        if (canonicalProfileId == linkedProfileId)
            throw new ArgumentException("A profile cannot be linked to itself.", nameof(linkedProfileId));
        if (!Enum.IsDefined(kind))
            throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown identity-link kind.");

        CanonicalProfileId = canonicalProfileId;
        LinkedProfileId = linkedProfileId;
        Kind = kind;
    }

    /// <summary>Canonical profile that owns the private identity relationship.</summary>
    public ProfileId CanonicalProfileId { get; }

    /// <summary>Other profile privately associated with the canonical profile.</summary>
    public ProfileId LinkedProfileId { get; }

    /// <summary>Reason or category for the private link.</summary>
    public ProfileIdentityLinkKind Kind { get; }

    /// <summary>Deconstructs the link into its constructor values.</summary>
    public void Deconstruct(
        out ProfileId canonicalProfileId,
        out ProfileId linkedProfileId,
        out ProfileIdentityLinkKind kind)
    {
        canonicalProfileId = CanonicalProfileId;
        linkedProfileId = LinkedProfileId;
        kind = Kind;
    }
}

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
