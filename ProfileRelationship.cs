namespace TheSingularityWorkshop.Profiles;

/// <summary>Describes a semantic relationship between two profile entities.</summary>
public sealed record ProfileRelationship
{
    /// <summary>Creates a relationship between two initialized profile IDs.</summary>
    /// <param name="subject">The profile that owns or originates the relationship.</param>
    /// <param name="target">The profile that the relationship points to.</param>
    /// <param name="relationshipType">Application-defined relationship vocabulary.</param>
    public ProfileRelationship(ProfileId subject, ProfileId target, string relationshipType)
    {
        if (subject == default)
            throw new ArgumentException("The relationship subject must be initialized.", nameof(subject));
        if (target == default)
            throw new ArgumentException("The relationship target must be initialized.", nameof(target));
        ArgumentException.ThrowIfNullOrWhiteSpace(relationshipType);

        Subject = subject;
        Target = target;
        RelationshipType = relationshipType;
    }

    /// <summary>The profile that owns or originates the relationship.</summary>
    public ProfileId Subject { get; }

    /// <summary>The profile that the relationship points to.</summary>
    public ProfileId Target { get; }

    /// <summary>Application-defined relationship vocabulary.</summary>
    public string RelationshipType { get; }

    /// <summary>Deconstructs the relationship into its constructor values.</summary>
    public void Deconstruct(out ProfileId subject, out ProfileId target, out string relationshipType)
    {
        subject = Subject;
        target = Target;
        relationshipType = RelationshipType;
    }
}
