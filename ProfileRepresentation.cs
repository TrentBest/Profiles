using System.Collections.ObjectModel;

namespace TheSingularityWorkshop.Profiles;

/// <summary>Safe observer-specific representation of a profile.</summary>
/// <remarks>
/// The claims collection is copied and exposed as a read-only dictionary. This protects the
/// representation's structure, but does not deep-clone arbitrary claim values. Claim values
/// should therefore be immutable value objects or treated as immutable after publication.
/// </remarks>
public sealed record ProfileRepresentation
{
    /// <summary>Creates a representation with an isolated, read-only claims collection.</summary>
    public ProfileRepresentation(
        ProfileId id,
        string? displayName,
        string? avatar,
        IReadOnlyDictionary<string, object?> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);

        Id = id;
        DisplayName = displayName;
        Avatar = avatar;
        Claims = new ReadOnlyDictionary<string, object?>(
            new Dictionary<string, object?>(claims, StringComparer.Ordinal));
    }

    /// <summary>Identity of the represented profile.</summary>
    public ProfileId Id { get; }

    /// <summary>Display name allowed for this observer.</summary>
    public string? DisplayName { get; }

    /// <summary>Avatar allowed for this observer.</summary>
    public string? Avatar { get; }

    /// <summary>Authorized claims; the collection cannot be modified through this representation.</summary>
    public IReadOnlyDictionary<string, object?> Claims { get; }

    /// <summary>Deconstructs the representation into its four public components.</summary>
    public void Deconstruct(
        out ProfileId id,
        out string? displayName,
        out string? avatar,
        out IReadOnlyDictionary<string, object?> claims)
    {
        id = Id;
        displayName = DisplayName;
        avatar = Avatar;
        claims = Claims;
    }
}
