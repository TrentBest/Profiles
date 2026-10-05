using TheSingularityWorkshop.Profiles;

namespace TheSingularityWorkshop.Profiles.Tests;

public sealed class ProfileTests
{
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
    public void GroupsCanSelectDifferentAvatars()
    {
        var profile = CreateProfile();
        var friend = ProfileId.New();
        var stranger = ProfileId.New();

        var friends = profile.DefineGroup("Friends");
        friends.Add(friend);
        profile.SetGroupAvatar("Friends", "cheshire-cat");
        profile.SetAvatar(stranger, "fluffy-ape");

        Assert.Equal("cheshire-cat", profile.RepresentTo(friend).Avatar);
        Assert.Equal("fluffy-ape", profile.RepresentTo(stranger).Avatar);
    }

    [Fact]
    public void PrivateClaimDoesNotLeak()
    {
        var profile = CreateProfile();
        var age = ProfileAttributeDefinition.Create("age", typeof(int));
        profile.SetClaim(new ProfileClaim(age, 37));
        profile.SetDisclosureRule(new DisclosureRule("age", DisclosureScope.Private));

        var representation = profile.RepresentTo(ProfileId.New());

        Assert.DoesNotContain("age", representation.Claims.Keys);
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
            "language", DisclosureScope.Group, new HashSet<ProfileId> { friends.Id }));

        Assert.Equal("en-US", profile.RepresentTo(friend, new HashSet<ProfileId> { friends.Id }).Claims["language"]);
        Assert.DoesNotContain("language", profile.RepresentTo(stranger).Claims.Keys);
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
    public void AccessRecordIdentifiesObserverWithoutReason()
    {
        var profile = CreateProfile();
        var observer = ProfileId.New();
        var record = new ProfileAccessRecord(profile.Id, observer, DateTimeOffset.UtcNow);

        Assert.Equal(profile.Id, record.ProfileId);
        Assert.Equal(observer, record.Observer);
    }

    private static Profile CreateProfile()
    {
        var profile = new Profile(ProfileEntityKind.Individual, "Example");
        profile.SetDisclosureRule(new DisclosureRule("display-name", DisclosureScope.Public));
        profile.SetAvatar(profile.Id, "default");
        return profile;
    }
}
