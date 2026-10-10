using TheSingularityWorkshop.Profiles;
using Xunit;

namespace TheSingularityWorkshop.Profiles.Tests;

public sealed class ProfileTests
{
    [Fact]
    public void AllSupportedEntityKindsCanBeRepresented()
    {
        foreach (var kind in Enum.GetValues<ProfileEntityKind>())
        {
            var profile = new Profile(kind, kind.ToString());

            Assert.Equal(kind, profile.Kind);
            Assert.NotEqual(default, profile.Id);
        }
    }

    [Fact]
    public void ProfileIdsAreOpaqueAndUnique()
    {
        var first = ProfileId.New();
        var second = ProfileId.New();

        Assert.NotEqual(first, second);
        Assert.False(string.IsNullOrWhiteSpace(first.ToString()));
    }

    [Fact]
    public void PublicCollectionViewsCannotMutateProfileInternals()
    {
        var profile = CreateProfile();
        var other = ProfileId.New();
        profile.SetClaim(new ProfileClaim(
            ProfileAttributeDefinition.Create("language", typeof(string)),
            "en-US"));
        profile.Exclude(other);

        Assert.Throws<NotSupportedException>(
            () => ((IDictionary<string, ProfileClaim>)profile.Claims).Clear());

        var exclusionSnapshot = Assert.IsType<HashSet<ProfileId>>(profile.ExcludedEntities);
        exclusionSnapshot.Clear();
        Assert.Contains(other, profile.ExcludedEntities);

        var group = profile.DefineGroup("Friends");
        var member = ProfileId.New();
        group.Add(member);
        var membershipSnapshot = Assert.IsType<HashSet<ProfileId>>(group.Members);
        membershipSnapshot.Clear();
        Assert.Contains(member, group.Members);
    }

    [Fact]
    public void ClaimsEnforceDeclaredValueType()
    {
        var definition = ProfileAttributeDefinition.Create("age", typeof(int));

        Assert.Throws<ArgumentException>(
            () => new ProfileClaim(definition, "thirty-seven"));
    }

    [Fact]
    public void ClaimsRetainVerificationAndProvenance()
    {
        var verifiedAt = DateTimeOffset.UtcNow;
        var definition = ProfileAttributeDefinition.Create("role", typeof(string));
        var claim = new ProfileClaim(definition, "architect", verifiedAt, "trusted-provider");

        Assert.True(claim.IsVerified);
        Assert.Equal(verifiedAt, claim.VerifiedAt);
        Assert.Equal("trusted-provider", claim.Provenance);
    }

    [Fact]
    public void ProfileConfigurationCanBeFluent()
    {
        var language = ProfileAttributeDefinition.Create("language", typeof(string));
        var profile = new Profile(ProfileEntityKind.Individual, "Ari")
            .SetClaim(new ProfileClaim(language, "en-US"))
            .SetDisclosureRule(new DisclosureRule("language", DisclosureScope.Public))
            .SetPublicAvatar("ari-avatar");

        var representation = profile.RepresentTo(ProfileId.New());

        Assert.Equal("en-US", representation.Claims["language"]);
        Assert.Equal("ari-avatar", representation.Avatar);
    }

    [Fact]
    public void PublicClaimIsIncluded()
    {
        var profile = CreateProfile();
        var language = ProfileAttributeDefinition.Create("language", typeof(string));
        profile.SetClaim(new ProfileClaim(language, "en-US"));
        profile.SetDisclosureRule(new DisclosureRule("language", DisclosureScope.Public));

        var representation = profile.RepresentTo(ProfileId.New());

        Assert.Equal("en-US", representation.Claims["language"]);
    }

    [Fact]
    public void PrivateClaimDoesNotLeak()
    {
        var profile = CreateProfile();
        var age = ProfileAttributeDefinition.Create("age", typeof(int));
        profile.SetClaim(new ProfileClaim(age, 37));
        profile.SetDisclosureRule(new DisclosureRule("age", DisclosureScope.Private));

        Assert.DoesNotContain("age", profile.RepresentTo(ProfileId.New()).Claims.Keys);
    }

    [Fact]
    public void ExplicitClaimIsVisibleOnlyToAllowedObserver()
    {
        var profile = CreateProfile();
        var allowed = ProfileId.New();
        var denied = ProfileId.New();
        var email = ProfileAttributeDefinition.Create("email", typeof(string));

        profile.SetClaim(new ProfileClaim(email, "owner@example.test"));
        var allowedEntities = new HashSet<ProfileId> { allowed };
        profile.SetDisclosureRule(new DisclosureRule(
            "email",
            DisclosureScope.Explicit,
            allowedEntities));
        allowedEntities.Clear();

        Assert.Equal("owner@example.test", profile.RepresentTo(allowed).Claims["email"]);
        Assert.DoesNotContain("email", profile.RepresentTo(denied).Claims.Keys);
    }

    [Fact]
    public void GroupDisclosureRevealsOnlyToGroupMembers()
    {
        var profile = CreateProfile();
        var friend = ProfileId.New();
        var stranger = ProfileId.New();
        var friends = profile.DefineGroup("Friends");
        friends.Add(friend);

        var language = ProfileAttributeDefinition.Create("language", typeof(string));
        profile.SetClaim(new ProfileClaim(language, "en-US"));
        profile.SetDisclosureRule(new DisclosureRule(
            "language",
            DisclosureScope.Group,
            new HashSet<ProfileId> { friends.Id }));

        Assert.Equal(
            "en-US",
            profile.RepresentTo(
                friend,
                new HashSet<ProfileId> { friends.Id }).Claims["language"]);

        Assert.DoesNotContain(
            "language",
            profile.RepresentTo(stranger).Claims.Keys);

        // Membership defined by this profile is sufficient; callers need not
        // redundantly pass the same group ID into RepresentTo.
        Assert.Equal("en-US", profile.RepresentTo(friend).Claims["language"]);
    }

    [Fact]
    public void PublicRepresentationCanReplaceSensitiveValue()
    {
        var profile = CreateProfile();
        var location = ProfileAttributeDefinition.Create("location", typeof(string));
        profile.SetClaim(new ProfileClaim(location, "private-address"));
        profile.SetDisclosureRule(new DisclosureRule(
            "location",
            DisclosureScope.Public,
            PublicRepresentation: "region-only"));

        Assert.Equal("region-only", profile.RepresentTo(ProfileId.New()).Claims["location"]);
    }

    [Fact]
    public void DisplayNameRequiresItsOwnDisclosureRule()
    {
        var profile = new Profile(ProfileEntityKind.Individual, "Ari");

        Assert.Null(profile.RepresentTo(ProfileId.New()).DisplayName);

        profile.SetDisclosureRule(new DisclosureRule(
            "display-name", DisclosureScope.Public));

        Assert.Equal("Ari", profile.RepresentTo(ProfileId.New()).DisplayName);
    }

    [Fact]
    public void ExcludedObserverReceivesOnlyAnonymousRepresentation()
    {
        var profile = CreateProfile();
        var observer = ProfileId.New();
        profile.Exclude(observer);

        var representation = profile.RepresentTo(observer);

        Assert.Equal(profile.Id, representation.Id);
        Assert.Null(representation.DisplayName);
        Assert.Equal("anonymous", representation.Avatar);
        Assert.Empty(representation.Claims);
    }

    [Fact]
    public void GroupAvatarOverridesPublicAvatar()
    {
        var profile = CreateProfile();
        var friend = ProfileId.New();
        var friends = profile.DefineGroup("Friends");
        friends.Add(friend);

        profile.SetPublicAvatar("public");
        profile.SetGroupAvatar("Friends", "friend-avatar");

        Assert.Equal(
            "friend-avatar",
            profile.RepresentTo(
                friend,
                new HashSet<ProfileId> { friends.Id }).Avatar);
    }

    [Fact]
    public void DirectAvatarOverridesGroupAndPublicAvatars()
    {
        var profile = CreateProfile();
        var friend = ProfileId.New();
        var friends = profile.DefineGroup("Friends");
        friends.Add(friend);

        profile.SetPublicAvatar("public");
        profile.SetGroupAvatar("Friends", "group");
        profile.SetAvatar(friend, "direct");

        Assert.Equal(
            "direct",
            profile.RepresentTo(
                friend,
                new HashSet<ProfileId> { friends.Id }).Avatar);
    }

    [Fact]
    public void RemovingExclusionRestoresNormalRepresentation()
    {
        var profile = CreateProfile();
        var observer = ProfileId.New();
        profile.Exclude(observer);

        Assert.Equal("anonymous", profile.RepresentTo(observer).Avatar);

        Assert.True(profile.RemoveExclusion(observer));
        Assert.Equal("default", profile.RepresentTo(observer).Avatar);
    }

    [Fact]
    public void DefiningAnExistingGroupPreservesItsMembership()
    {
        var profile = CreateProfile();
        var member = ProfileId.New();
        var first = profile.DefineGroup("Friends");
        first.Add(member);

        var second = profile.DefineGroup("Friends");

        Assert.Same(first, second);
        Assert.Contains(member, second.Members);
    }

    [Fact]
    public void GroupsManageMembership()
    {
        var profile = CreateProfile();
        var member = ProfileId.New();
        var group = profile.DefineGroup("Customers");

        Assert.True(group.Add(member));
        Assert.False(group.Add(member));
        Assert.True(group.Contains(member));
        Assert.True(group.Remove(member));
        Assert.False(group.Contains(member));
    }

    [Fact]
    public void RelationshipsMustBelongToTheirSubject()
    {
        var profile = CreateProfile();
        var other = ProfileId.New();

        Assert.Throws<ArgumentException>(
            () => profile.AddRelationship(
                new ProfileRelationship(other, profile.Id, "owns")));

        profile.AddRelationship(
            new ProfileRelationship(profile.Id, other, "owns"));

        Assert.Single(profile.Relationships);
    }

    [Fact]
    public void PublicDataReportContainsExposureMetadataOnly()
    {
        var profile = CreateProfile();
        var age = ProfileAttributeDefinition.Create("age", typeof(int));
        profile.SetClaim(new ProfileClaim(age, 37));
        profile.SetDisclosureRule(new DisclosureRule("age", DisclosureScope.Public));
        profile.SetPublicAvatar("fluffy-ape");

        var report = ProfilePublicDataReporter.Create(profile);

        Assert.Equal(profile.Id, report.ProfileId);
        Assert.Contains("age", report.PublicAttributes);
        Assert.True(report.DisplayNameIsPublic);
        Assert.True(report.HasPublicAvatar);
    }

    [Fact]
    public void PublicDataReportIgnoresRulesWithoutMatchingClaims()
    {
        var profile = CreateProfile();
        profile.SetDisclosureRule(new DisclosureRule("future-attribute", DisclosureScope.Public));

        var report = ProfilePublicDataReporter.Create(profile);

        Assert.DoesNotContain("future-attribute", report.PublicAttributes);
        Assert.DoesNotContain("display-name", report.PublicAttributes);
        Assert.True(report.DisplayNameIsPublic);
    }

    [Fact]
    public void AssumptionCanBeSatisfied()
    {
        var profile = CreateProfile();
        var age = ProfileAttributeDefinition.Create("age", typeof(int));
        profile.SetClaim(new ProfileClaim(age, 37));

        var assumption = new AssumptionDefinition(
            "age-21-plus",
            "The profile represents someone aged 21 or older.",
            p => (int)p.Claims["age"].Value >= 21);

        var result = new ProfileAssumptionResolver().Resolve(profile, assumption);

        Assert.Equal(AssumptionResultKind.Satisfied, result.Kind);
        Assert.True(result.IsSatisfied);
        Assert.Equal("profile-claim", result.Proof);
    }

    [Fact]
    public void AssumptionCanBeNotSatisfied()
    {
        var profile = CreateProfile();
        var age = ProfileAttributeDefinition.Create("age", typeof(int));
        profile.SetClaim(new ProfileClaim(age, 18));

        var assumption = new AssumptionDefinition(
            "age-21-plus",
            "The profile represents someone aged 21 or older.",
            p => (int)p.Claims["age"].Value >= 21);

        var result = new ProfileAssumptionResolver().Resolve(profile, assumption);

        Assert.Equal(AssumptionResultKind.NotSatisfied, result.Kind);
        Assert.False(result.IsSatisfied);
    }

    [Fact]
    public void UnknownAssumptionIsNotNotSatisfied()
    {
        var profile = CreateProfile();
        var assumption = new AssumptionDefinition(
            "age-21-plus",
            "The profile represents someone aged 21 or older.",
            p => (int)p.Claims["age"].Value >= 21);

        var result = new ProfileAssumptionResolver().Resolve(profile, assumption);

        Assert.Equal(AssumptionResultKind.Unknown, result.Kind);
    }

    [Fact]
    public void IdentityLinksRemainPrivateToTrustedInfrastructure()
    {
        var canonical = ProfileId.New();
        var publisher = ProfileId.New();
        var store = new ProfileIdentityLinkStore();

        store.Add(new ProfileIdentityLink(
            canonical,
            publisher,
            ProfileIdentityLinkKind.Publisher));

        Assert.True(store.IsLinked(canonical, publisher));
        Assert.Single(store.Links);
    }

    [Fact]
    public void IdentityLinkStoreRejectsSelfLink()
    {
        var id = ProfileId.New();
        var store = new ProfileIdentityLinkStore();

        Assert.Throws<ArgumentException>(
            () => store.Add(new ProfileIdentityLink(
                id,
                id,
                ProfileIdentityLinkKind.Custom)));
    }

    [Fact]
    public void IdentityLinkStoreDoesNotDuplicateLinks()
    {
        var canonical = ProfileId.New();
        var publisher = ProfileId.New();
        var link = new ProfileIdentityLink(
            canonical,
            publisher,
            ProfileIdentityLinkKind.Publisher);
        var store = new ProfileIdentityLinkStore();

        store.Add(link);
        store.Add(link);

        Assert.Single(store.Links);
    }

    [Fact]
    public void LicensingRequiresExplicitEnablementAndAttributeAllowList()
    {
        var policy = new ProfileDataLicensePolicy();

        policy.Allow("language");
        Assert.False(policy.CanLicense("language"));

        policy.Enabled = true;
        Assert.True(policy.CanLicense("language"));

        Assert.True(policy.Disallow("language"));
        Assert.False(policy.CanLicense("language"));
    }

    [Fact]
    public void AccessRecordCapturesOnlyProfileObserverAndTime()
    {
        var profile = CreateProfile();
        var observer = ProfileId.New();
        var time = DateTimeOffset.UtcNow;

        var record = new ProfileAccessRecord(profile.Id, observer, time);

        Assert.Equal(profile.Id, record.ProfileId);
        Assert.Equal(observer, record.Observer);
        Assert.Equal(time, record.AccessedAt);
    }

    [Fact]
    public void ExternalIdentityRequiresIssuerAndSubject()
    {
        Assert.Throws<ArgumentException>(() => new ExternalIdentityReference(" ", "subject"));
        Assert.Throws<ArgumentException>(() => new ExternalIdentityReference("issuer", " "));

        var reference = new ExternalIdentityReference("https://identity.example.test", "subject-123");

        Assert.Equal("https://identity.example.test", reference.Issuer);
        Assert.Equal("subject-123", reference.Subject);
    }

    [Fact]
    public void ExternalIdentityBindingRequiresInitializedProfileId()
    {
        var reference = new ExternalIdentityReference("issuer", "subject");

        Assert.Throws<ArgumentException>(
            () => new ProfileExternalIdentityBinding(default, reference));

        var profileId = ProfileId.New();
        var binding = new ProfileExternalIdentityBinding(profileId, reference);

        Assert.Equal(profileId, binding.ProfileId);
        Assert.Equal(reference, binding.ExternalIdentity);
    }

    private static Profile CreateProfile()
    {
        var profile = new Profile(ProfileEntityKind.Individual, "Example");
        profile.SetDisclosureRule(new DisclosureRule("display-name", DisclosureScope.Public));
        profile.SetPublicAvatar("default");
        return profile;
    }
}
