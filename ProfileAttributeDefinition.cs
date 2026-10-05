namespace TheSingularityWorkshop.Profiles;

/// <summary>Defines one micro-data attribute that may be held by a profile.</summary>
public sealed record ProfileAttributeDefinition(string Key, Type ValueType, string? Description = null)
{
    /// <summary>Creates an attribute definition.</summary>
    public static ProfileAttributeDefinition Create(string key, Type valueType, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(valueType);
        return new ProfileAttributeDefinition(key, valueType, description);
    }
}
