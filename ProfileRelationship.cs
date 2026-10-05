namespace TheSingularityWorkshop.Profiles;

/// <summary>Describes a relationship between two profile entities.</summary>
public sealed record ProfileRelationship(ProfileId Subject, ProfileId Object, string RelationshipType);
