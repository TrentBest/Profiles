using TheSingularityWorkshop.Profiles;
using Xunit;

namespace TheSingularityWorkshop.Profiles.Tests;

public sealed class SovereigntyTests
{
    [Fact]
    public void PublisherPersonaCanRemainPrivatelyLinked()
    {
        var canonical = ProfileId.New();
        var publisher = ProfileId.New();
        var store = new ProfileIdentityLinkStore();

        store.Add(new ProfileIdentityLink(canonical, publisher, ProfileIdentityLinkKind.Publisher));

        Assert.True(store.IsLinked(canonical, publisher));
        Assert.Single(store.Links);
    }

    [Fact]
    public void IdentityLinkIsNotPartOfObserverRepresentation()
    {
        var canonical = new Profile(ProfileEntityKind.Individual, "Private Person");
        var publisher = new Profile(ProfileEntityKind.Organization, "Public Publisher");
        var observer = ProfileId.New();
        var store = new ProfileIdentityLinkStore();

        store.Add(new ProfileIdentityLink(canonical.Id, publisher.Id, ProfileIdentityLinkKind.Publisher));

        var representation = publisher.RepresentTo(observer);

        Assert.DoesNotContain(canonical.Id.ToString(), representation.Claims.Values.Select(value => value?.ToString()));
        Assert.DoesNotContain("identity-link", representation.Claims.Keys);
    }

    [Fact]
    public void DataLicensingMustBeExplicitlyEnabled()
    {
        var policy = new ProfileDataLicensePolicy();
        policy.Allow("interests");

        Assert.False(policy.CanLicense("interests"));

        policy.Enabled = true;

        Assert.True(policy.CanLicense("interests"));
        Assert.False(policy.CanLicense("age"));
    }
}
