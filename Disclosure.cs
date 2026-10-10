namespace TheSingularityWorkshop.Profiles;

/// <summary>Describes who may receive a representation of profile data.</summary>
public enum DisclosureScope
{
    /// <summary>No disclosure.</summary>
    Private,
    /// <summary>Anyone may receive the representation.</summary>
    Public,
    /// <summary>Members of selected groups may receive the representation.</summary>
    Group,
    /// <summary>Specifically listed observers may receive the representation.</summary>
    Explicit
}

/// <summary>Defines the audience and optional value substitution for one attribute.</summary>
/// <remarks>
/// <para>Audience identifiers are intentionally separated: <see cref="AllowedObservers"/> contains observer IDs,
/// while <see cref="AllowedGroups"/> contains group IDs. They are never interchangeable.</para>
/// <para><see cref="RepresentationOverride"/> is used for every authorized audience, not only public disclosure.
/// It can therefore provide a less precise or otherwise audience-safe value.</para>
/// </remarks>
public sealed record DisclosureRule
{
    private readonly HashSet<ProfileId> _allowedObservers;
    private readonly HashSet<ProfileId> _allowedGroups;

    /// <summary>Creates a disclosure rule with explicitly typed audience lists.</summary>
    public DisclosureRule(
        string attributeKey,
        DisclosureScope scope,
        IReadOnlySet<ProfileId>? allowedObservers = null,
        IReadOnlySet<ProfileId>? allowedGroups = null,
        string? representationOverride = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(attributeKey);
        if (!Enum.IsDefined(scope))
            throw new ArgumentOutOfRangeException(nameof(scope), scope, "Unknown disclosure scope.");

        if (scope != DisclosureScope.Explicit && allowedObservers is { Count: > 0 })
            throw new ArgumentException("Observer IDs are only valid for Explicit disclosure.", nameof(allowedObservers));
        if (scope != DisclosureScope.Group && allowedGroups is { Count: > 0 })
            throw new ArgumentException("Group IDs are only valid for Group disclosure.", nameof(allowedGroups));

        AttributeKey = attributeKey;
        Scope = scope;
        _allowedObservers = allowedObservers is null ? [] : new HashSet<ProfileId>(allowedObservers);
        _allowedGroups = allowedGroups is null ? [] : new HashSet<ProfileId>(allowedGroups);
        RepresentationOverride = representationOverride;
    }

    /// <summary>Attribute key governed by this rule.</summary>
    public string AttributeKey { get; }

    /// <summary>Audience scope.</summary>
    public DisclosureScope Scope { get; }

    /// <summary>Observer IDs permitted by an Explicit rule.</summary>
    public IReadOnlySet<ProfileId> AllowedObservers => new HashSet<ProfileId>(_allowedObservers);

    /// <summary>Group IDs permitted by a Group rule.</summary>
    public IReadOnlySet<ProfileId> AllowedGroups => new HashSet<ProfileId>(_allowedGroups);

    /// <summary>Optional value used for any observer allowed by this rule.</summary>
    public string? RepresentationOverride { get; }

    /// <summary>Determines whether the requester is permitted.</summary>
    public bool Allows(ProfileId requester, IReadOnlySet<ProfileId> requesterGroups) => Scope switch
    {
        DisclosureScope.Private => false,
        DisclosureScope.Public => true,
        DisclosureScope.Explicit => _allowedObservers.Contains(requester),
        DisclosureScope.Group => _allowedGroups.Overlaps(requesterGroups),
        _ => false
    };
}
