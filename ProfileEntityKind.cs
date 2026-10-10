namespace TheSingularityWorkshop.Profiles;

/// <summary>Identifies the broad kind of entity represented by a profile.</summary>
public enum ProfileEntityKind
{
    /// <summary>An individual person or agent.</summary>
    Individual,
    /// <summary>A company or commercial entity.</summary>
    Company,
    /// <summary>A social or functional group.</summary>
    Group,
    /// <summary>An organization or institution.</summary>
    Organization,
    /// <summary>An experience or world.</summary>
    Experience,
    /// <summary>A machine or autonomous agent.</summary>
    Agent,
    /// <summary>A custom entity kind.</summary>
    Custom
}
