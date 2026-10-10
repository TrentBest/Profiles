using System.Collections.ObjectModel;

namespace TheSingularityWorkshop.Profiles;

/// <summary>Runtime profile sovereignty boundary for one ecosystem entity.</summary>
/// <remarks>Persistence, transport, rendering, and jurisdiction policy remain outside this type.</remarks>
public sealed class Profile
{
    private readonly Dictionary<string, ProfileClaim> _claims = new(StringComparer.Ordinal);
    private readonly Dictionary<string, DisclosureRule> _rules = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _avatars = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ProfileGroup> _groups = new(StringComparer.Ordinal);
    private readonly List<ProfileRelationship> _relationships = [];
    private readonly HashSet<ProfileId> _excluded = [];
    private readonly ReadOnlyDictionary<string, ProfileClaim> _claimsView;
    private readonly ReadOnlyDictionary<string, ProfileGroup> _groupsView;
    private readonly ReadOnlyCollection<ProfileRelationship> _relationshipsView;

    /// <summary>Creates a profile.</summary>
    public Profile(ProfileEntityKind kind, string? displayName = null)
    {
        if (!Enum.IsDefined(kind))
            throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown profile entity kind.");

        Id = ProfileId.New();
        Kind = kind;
        DisplayName = displayName;
        _claimsView = new ReadOnlyDictionary<string, ProfileClaim>(_claims);
        _groupsView = new ReadOnlyDictionary<string, ProfileGroup>(_groups);
        _relationshipsView = _relationships.AsReadOnly();
    }

    /// <summary>Stable identity.</summary>
    public ProfileId Id { get; }

    /// <summary>Entity kind.</summary>
    public ProfileEntityKind Kind { get; }

    /// <summary>Optional display name.</summary>
    public string? DisplayName { get; set; }

    /// <summary>Micro-data claims held by this profile.</summary>
    public IReadOnlyDictionary<string, ProfileClaim> Claims => _claimsView;

    /// <summary>User-defined disclosure groups.</summary>
    public IReadOnlyDictionary<string, ProfileGroup> Groups => _groupsView;

    /// <summary>Relationships owned by this profile.</summary>
    public IReadOnlyList<ProfileRelationship> Relationships => _relationshipsView;

    /// <summary>Profiles excluded by this profile.</summary>
    public IReadOnlySet<ProfileId> ExcludedEntities => new HashSet<ProfileId>(_excluded);

    /// <summary>Attribute keys currently marked public.</summary>
    public IEnumerable<string> PublicAttributeKeys => _claims.Keys
        .Where(key => _rules.TryGetValue(key, out var rule) && rule.Scope == DisclosureScope.Public);

    /// <summary>Whether the display name is public.</summary>
    public bool IsDisplayNamePublic =>
        _rules.TryGetValue("display-name", out var rule) && rule.Scope == DisclosureScope.Public;

    /// <summary>Whether a default public avatar has been selected.</summary>
    public bool HasPublicAvatar => _avatars.ContainsKey(ProfileAvatarKey);

    /// <summary>Defines or replaces a micro-data claim.</summary>
    public Profile SetClaim(ProfileClaim claim)
    {
        ArgumentNullException.ThrowIfNull(claim);
        _claims[claim.Definition.Key] = claim;
        return this;
    }

    /// <summary>Defines or replaces disclosure for one micro-data attribute.</summary>
    public Profile SetDisclosureRule(DisclosureRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        _rules[rule.AttributeKey] = new DisclosureRule(
            rule.AttributeKey,
            rule.Scope,
            rule.AllowedObservers,
            rule.AllowedGroups,
            rule.RepresentationOverride);
        return this;
    }

    /// <summary>Creates a user-controlled group.</summary>
    public ProfileGroup DefineGroup(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (_groups.TryGetValue(name, out var existing))
            return existing;

        var group = new ProfileGroup(name);
        _groups.Add(name, group);
        return group;
    }

    /// <summary>Adds a relationship owned by this profile.</summary>
    public Profile AddRelationship(ProfileRelationship relationship)
    {
        ArgumentNullException.ThrowIfNull(relationship);
        if (relationship.Subject != Id)
            throw new ArgumentException("The relationship subject must be this profile.", nameof(relationship));

        _relationships.Add(relationship);
        return this;
    }

    /// <summary>Excludes an observer from detailed representations.</summary>
    public Profile Exclude(ProfileId observer)
    {
        _excluded.Add(observer);
        return this;
    }

    /// <summary>Removes an observer from the exclusion list.</summary>
    public bool RemoveExclusion(ProfileId observer) => _excluded.Remove(observer);

    /// <summary>Selects an avatar for one observer.</summary>
    public Profile SetAvatar(ProfileId observer, string avatar)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(avatar);
        _avatars[ObserverAvatarKey(observer)] = avatar;
        return this;
    }

    /// <summary>Selects the public/default avatar.</summary>
    public Profile SetPublicAvatar(string avatar)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(avatar);
        _avatars[ProfileAvatarKey] = avatar;
        return this;
    }

    /// <summary>Selects an avatar for members of a group.</summary>
    public Profile SetGroupAvatar(string groupName, string avatar)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(groupName);
        ArgumentException.ThrowIfNullOrWhiteSpace(avatar);
        if (!_groups.ContainsKey(groupName))
            throw new KeyNotFoundException($"Group '{groupName}' does not exist.");

        _avatars[GroupAvatarKey(groupName)] = avatar;
        return this;
    }

    /// <summary>Creates the observer-specific representation.</summary>
    public ProfileRepresentation RepresentTo(ProfileId observer, IReadOnlySet<ProfileId>? observerGroups = null)
    {
        var effectiveGroups = observerGroups is null
            ? new HashSet<ProfileId>()
            : new HashSet<ProfileId>(observerGroups);

        // Membership recorded on this profile is authoritative for its own groups.
        // Hosts may add trusted external group memberships through observerGroups.
        foreach (var group in _groups.Values)
        {
            if (group.Contains(observer))
                effectiveGroups.Add(group.Id);
        }

        if (_excluded.Contains(observer))
            return new ProfileRepresentation(Id, null, "anonymous", new Dictionary<string, object?>());

        var claims = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var (key, claim) in _claims)
        {
            if (_rules.TryGetValue(key, out var rule) && rule.Allows(observer, effectiveGroups))
                claims[key] = rule.RepresentationOverride ?? claim.Value;
        }

        return new ProfileRepresentation(
            Id,
            IsDisplayNameVisible(observer, effectiveGroups) ? DisplayName : null,
            ResolveAvatar(observer, effectiveGroups),
            claims);
    }

    private string ResolveAvatar(ProfileId observer, HashSet<ProfileId> observerGroups)
    {
        if (_avatars.TryGetValue(ObserverAvatarKey(observer), out var direct)) return direct;

        foreach (var group in _groups.Values)
        {
            if (observerGroups.Contains(group.Id) && _avatars.TryGetValue(GroupAvatarKey(group.Name), out var groupAvatar))
                return groupAvatar;
        }

        return _avatars.TryGetValue(ProfileAvatarKey, out var fallback) ? fallback : "anonymous";
    }

    private bool IsDisplayNameVisible(ProfileId observer, IReadOnlySet<ProfileId> observerGroups)
        => _rules.TryGetValue("display-name", out var rule) && rule.Allows(observer, observerGroups);

    private static string ObserverAvatarKey(ProfileId observer) => $"observer:{observer.Value:D}";
    private static string GroupAvatarKey(string groupName) => $"group:{groupName}";
    private const string ProfileAvatarKey = "profile";
}
