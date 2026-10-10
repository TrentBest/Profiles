namespace TheSingularityWorkshop.Profiles;

/// <summary>Holds one micro-data claim owned by a profile.</summary>
/// <remarks>
/// Values are type-checked but not deep-cloned. Prefer immutable value objects; mutable values
/// must be treated as immutable while the claim is in use to keep profile state predictable.
/// </remarks>
public sealed class ProfileClaim
{
    /// <summary>Creates a claim.</summary>
    /// <param name="definition">The semantic definition and required value type.</param>
    /// <param name="value">A non-null value matching <paramref name="definition"/>. The value is retained by reference.</param>
    /// <param name="verifiedAt">Optional time of verification.</param>
    /// <param name="provenance">Optional source or verification authority.</param>
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
    /// <remarks>This object is not deep-cloned. Treat it as immutable after creating the claim.</remarks>
    public object Value { get; }

    /// <summary>When the claim was verified, if it was verified.</summary>
    public DateTimeOffset? VerifiedAt { get; }

    /// <summary>Optional source or verification authority.</summary>
    public string? Provenance { get; }

    /// <summary>Whether the claim has an explicit verification timestamp.</summary>
    public bool IsVerified => VerifiedAt.HasValue;
}
