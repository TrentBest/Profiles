namespace TheSingularityWorkshop.Profiles;

/// <summary>Compact opaque identity for a profile entity.</summary>
public readonly record struct ProfileId(Guid Value)
{
    /// <summary>Creates a new profile identifier.</summary>
    public static ProfileId New() => new(Guid.NewGuid());

    /// <summary>Returns the identifier text.</summary>
    public override string ToString() => Value.ToString("D");
}
