namespace TheSingularityWorkshop.Profiles;

/// <summary>Describes who may receive a representation of profile data.</summary>
public enum DisclosureScope
{
    /// <summary>No disclosure.</summary>
    Private,
    /// <summary>Anyone may receive the representation.</summary>
    Public,
    /// <summary>Members of a selected group may receive the representation.</summary>
    Group,
    /// <summary>A specifically authorized entity may receive the representation.</summary>
    Explicit
}

/// <summary>Defines the representation permitted for one attribute.</summary>
public sealed record DisclosureRule(
    string AttributeKey,
    DisclosureScope Scope,
    IReadOnlySet<ProfileId>? AllowedEntities = null,
    string? PublicRepresentation = null)
{
    /// <summary>Determines whether the requester is permitted.</summary>
    public bool Allows(ProfileId requester, IReadOnlySet<ProfileId> requesterGroups) => Scope switch
    {
        DisclosureScope.Private => false,
        DisclosureScope.Public => true,
        DisclosureScope.Explicit => AllowedEntities?.Contains(requester) == true,
        DisclosureScope.Group => AllowedEntities is not null && AllowedEntities.Overlaps(requesterGroups),
        _ => false
    };
}
