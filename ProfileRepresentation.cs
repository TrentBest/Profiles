namespace TheSingularityWorkshop.Profiles;

/// <summary>Safe observer-specific representation of a profile.</summary>
/// <remarks>A representation is deliberately distinct from the underlying profile.</remarks>
public sealed record ProfileRepresentation(
    ProfileId Id,
    string? DisplayName,
    string? Avatar,
    IReadOnlyDictionary<string, object?> Claims);
