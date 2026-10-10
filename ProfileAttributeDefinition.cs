namespace TheSingularityWorkshop.Profiles;

/// <summary>Defines one micro-data attribute that may be held by a profile.</summary>
public sealed record ProfileAttributeDefinition
{
    /// <summary>Creates an attribute definition with a non-empty key and a valid value type.</summary>
    public ProfileAttributeDefinition(string key, Type valueType, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(valueType);

        Key = key;
        ValueType = valueType;
        Description = description;
    }

    /// <summary>The stable key used to identify this attribute.</summary>
    public string Key { get; }

    /// <summary>The CLR type accepted for values of this attribute.</summary>
    public Type ValueType { get; }

    /// <summary>Optional human-readable explanation of the attribute.</summary>
    public string? Description { get; }

    /// <summary>Creates an attribute definition.</summary>
    public static ProfileAttributeDefinition Create(string key, Type valueType, string? description = null)
        => new(key, valueType, description);

    /// <summary>Deconstructs the definition into its constructor values.</summary>
    public void Deconstruct(out string key, out Type valueType, out string? description)
    {
        key = Key;
        valueType = ValueType;
        description = Description;
    }
}
