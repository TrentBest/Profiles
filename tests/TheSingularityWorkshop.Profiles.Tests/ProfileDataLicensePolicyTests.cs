using TheSingularityWorkshop.Profiles;
using Xunit;

namespace TheSingularityWorkshop.Profiles.Tests;

public sealed class ProfileDataLicensePolicyTests
{
    [Fact]
    public void AttributeKeysReturnsSnapshotRatherThanMutableInternalState()
    {
        var policy = new ProfileDataLicensePolicy { Enabled = true };
        policy.Allow("language");

        var snapshot = Assert.IsType<HashSet<string>>(policy.AttributeKeys);
        snapshot.Clear();

        Assert.True(policy.CanLicense("language"));
        Assert.Contains("language", policy.AttributeKeys);
    }
}
