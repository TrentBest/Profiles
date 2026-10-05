namespace TheSingularityWorkshop.Profiles;

/// <summary>
/// Describes an owner's explicit willingness to license selected profile data.
/// </summary>
/// <remarks>
/// Ordinary disclosure is not consent to sell. A separate marketplace or transaction layer
/// should create the actual offer, obtain the user's authorization, settle payment, and record
/// revocation or expiry. No platform fee is embedded in this policy.
/// </remarks>
public sealed class ProfileDataLicensePolicy
{
    private readonly HashSet<string> _attributeKeys = new(StringComparer.Ordinal);

    /// <summary>Whether the owner has enabled voluntary data licensing.</summary>
    public bool Enabled { get; set; }

    /// <summary>Data attributes the owner has made eligible for licensing.</summary>
    public IReadOnlySet<string> AttributeKeys => _attributeKeys;

    /// <summary>Adds one attribute to the licensing allow-list.</summary>
    public void Allow(string attributeKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(attributeKey);
        _attributeKeys.Add(attributeKey);
    }

    /// <summary>Removes one attribute from the licensing allow-list.</summary>
    public bool Disallow(string attributeKey) => _attributeKeys.Remove(attributeKey);

    /// <summary>Tests whether one attribute is eligible for licensing.</summary>
    public bool CanLicense(string attributeKey) => Enabled && _attributeKeys.Contains(attributeKey);
}
