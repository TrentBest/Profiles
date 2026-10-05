namespace TheSingularityWorkshop.Profiles;

/// <summary>Holds one micro-data claim owned by a profile.</summary>
public sealed class ProfileClaim
{
    /// <summary>Creates a claim.</summary>
    public ProfileClaim(ProfileAttributeDefinition definition, object value, DateTimeOffset? verifiedAt = null, string? provenance = null)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        ArgumentNullException.ThrowIfNull(value);
        if (!definition.ValueType.IsInstanceOfType(value))
            throw new ArgumentException($"Value for '{definition.Key}' must be assignable to {definition.ValueType}.", nameof(value));

        Value = value;
        VerifiedAt = verifiedAt;
        Provenance = provenance;
    }

    /// <summary>The semantic definition of the claim.</summary>
    public ProfileAttributeDefinition Definition { get; }

    /// <summary>The raw value; consumers should not expose it without authorization.</summary>
    public object Value { get; }

    /// <summary>When the claim was verified, if it was verified.</summary>
    public DateTimeOffset? VerifiedAt { get; }

    /// <summary>Optional source or verification authority.</summary>
    public string? Provenance { get; }

    /// <summary>Whether the claim has an explicit verification timestamp.</summary>
    public bool IsVerified => VerifiedAt.HasValue;
}
