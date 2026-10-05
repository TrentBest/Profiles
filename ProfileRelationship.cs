namespace TheSingularityWorkshop.Profiles;

/// <summary>Describes a semantic relationship between two profile entities.</summary>
/// <param name="Subject">The profile that owns or originates the relationship.</param>
/// <param name="Target">The profile that the relationship points to.</param>
/// <param name="RelationshipType">Application-defined relationship vocabulary.</param>
public sealed record ProfileRelationship(ProfileId Subject, ProfileId Target, string RelationshipType);
