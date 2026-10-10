namespace TheSingularityWorkshop.Profiles;

/// <summary>Owns private profile identity links for trusted identity infrastructure.</summary>
/// <remarks>
/// Ordinary profile representations must not expose these links. Legal disclosure is an
/// external policy decision and must be backed by whatever valid authority the applicable
/// jurisdiction requires; this core does not attempt to determine legal sufficiency.
/// </remarks>
public sealed class ProfileIdentityLinkStore
{
    private readonly List<ProfileIdentityLink> _links = [];

    /// <summary>Adds a private identity link.</summary>
    public void Add(ProfileIdentityLink link)
    {
        ArgumentNullException.ThrowIfNull(link);

        if (link.CanonicalProfileId == link.LinkedProfileId)
            throw new ArgumentException("A profile cannot be linked to itself.", nameof(link));

        if (_links.Contains(link))
            return;

        _links.Add(link);
    }

    /// <summary>Returns links known to trusted identity infrastructure.</summary>
    public IReadOnlyList<ProfileIdentityLink> Links => _links.AsReadOnly();

    /// <summary>
    /// Resolves whether two profiles are privately linked.
    /// </summary>
    public bool IsLinked(ProfileId first, ProfileId second)
        => _links.Any(link =>
            (link.CanonicalProfileId == first && link.LinkedProfileId == second) ||
            (link.CanonicalProfileId == second && link.LinkedProfileId == first));
}
