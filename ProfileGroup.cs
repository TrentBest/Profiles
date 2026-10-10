namespace TheSingularityWorkshop.Profiles;

/// <summary>A user-controlled group whose membership can control disclosure.</summary>
public sealed class ProfileGroup
{
    private readonly HashSet<ProfileId> _members = [];

    /// <summary>Creates a group.</summary>
    public ProfileGroup(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = ProfileId.New();
        Name = name;
    }

    /// <summary>Stable group identifier.</summary>
    public ProfileId Id { get; }

    /// <summary>User-facing group name.</summary>
    public string Name { get; }

    /// <summary>Current members.</summary>
    public IReadOnlySet<ProfileId> Members => new HashSet<ProfileId>(_members);

    /// <summary>Adds an entity.</summary>
    public bool Add(ProfileId entity) => _members.Add(entity);

    /// <summary>Removes an entity.</summary>
    public bool Remove(ProfileId entity) => _members.Remove(entity);

    /// <summary>Tests membership.</summary>
    public bool Contains(ProfileId entity) => _members.Contains(entity);
}
