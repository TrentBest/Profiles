namespace TheSingularityWorkshop.Profiles;

/// <summary>Describes the profile data currently marked publicly disclosable.</summary>
public sealed record ProfilePublicDataReport(
    ProfileId ProfileId,
    IReadOnlyList<string> PublicAttributes,
    bool DisplayNameIsPublic,
    bool HasPublicAvatar);

/// <summary>Builds an owner-facing report of public profile exposure.</summary>
public static class ProfilePublicDataReporter
{
    /// <summary>Creates a report without returning private claim values.</summary>
    public static ProfilePublicDataReport Create(Profile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        var publicAttributes = profile.PublicAttributeKeys.ToArray();
        return new ProfilePublicDataReport(
            profile.Id,
            publicAttributes,
            profile.IsDisplayNamePublic,
            profile.HasPublicAvatar);
    }
}
