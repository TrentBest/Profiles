namespace TheSingularityWorkshop.Profiles;

/// <summary>Records that an entity accessed a profile.</summary>
/// <remarks>The core records who and when, not the observer's private reason.</remarks>
public sealed record ProfileAccessRecord(ProfileId ProfileId, ProfileId Observer, DateTimeOffset AccessedAt);

/// <summary>Receives access events for an owning system to persist or process.</summary>
public interface IProfileAccessRecorder
{
    /// <summary>Records an access event.</summary>
    void Record(ProfileAccessRecord access);
}
