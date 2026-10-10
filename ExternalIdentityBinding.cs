namespace TheSingularityWorkshop.Profiles;

/// <summary>
/// Identifies an external authentication subject without carrying credentials or tokens.
/// </summary>
/// <remarks>
/// The issuer/provider and subject together identify the external account. The subject is
/// not assumed to be an email address. This value is a reference supplied by trusted
/// authentication integration; constructing it does not authenticate a caller or prove ownership.
/// </remarks>
public sealed record ExternalIdentityReference
{
    /// <summary>Creates a provider-qualified external identity reference.</summary>
    /// <param name="issuer">Stable issuer or provider identifier.</param>
    /// <param name="subject">Provider-issued subject identifier.</param>
    public ExternalIdentityReference(string issuer, string subject)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        Issuer = issuer;
        Subject = subject;
    }

    /// <summary>Stable issuer or provider identifier.</summary>
    public string Issuer { get; }

    /// <summary>Provider-issued subject identifier, not necessarily an email address.</summary>
    public string Subject { get; }
}

/// <summary>
/// Associates a domain profile with an external identity reference.
/// </summary>
/// <remarks>
/// This is a mapping contract, not an authentication result. Trusted integration code must
/// authenticate the external principal, decide whether the mapping may be created, and protect
/// the mapping in storage. Credentials, tokens, provider SDKs, and protocol handling stay outside
/// Profiles. A verified email or issuer claim does not by itself establish organization membership,
/// legal identity, authorization, or endorsement.
/// </remarks>
public sealed record ProfileExternalIdentityBinding
{
    /// <summary>Creates a mapping between a profile and an external identity.</summary>
    public ProfileExternalIdentityBinding(ProfileId profileId, ExternalIdentityReference externalIdentity)
    {
        if (profileId == default)
            throw new ArgumentException("A profile ID must be initialized.", nameof(profileId));

        ProfileId = profileId;
        ExternalIdentity = externalIdentity ?? throw new ArgumentNullException(nameof(externalIdentity));
    }

    /// <summary>Domain profile associated with the external identity.</summary>
    public ProfileId ProfileId { get; }

    /// <summary>Provider-qualified external identity reference.</summary>
    public ExternalIdentityReference ExternalIdentity { get; }
}
