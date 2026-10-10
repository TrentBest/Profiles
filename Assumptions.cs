namespace TheSingularityWorkshop.Profiles;

/// <summary>Describes a proposition an experience needs resolved.</summary>
public sealed record AssumptionDefinition
{
    /// <summary>Creates a proposition with a stable key, human-readable meaning, and resolver.</summary>
    public AssumptionDefinition(string key, string description, Func<Profile, bool> resolver)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Key = key;
        Description = description;
        Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
    }

    /// <summary>Stable key identifying the assumption.</summary>
    public string Key { get; }

    /// <summary>Human-readable explanation of the proposition.</summary>
    public string Description { get; }

    /// <summary>Evaluates the proposition against a profile.</summary>
    public Func<Profile, bool> Resolver { get; }

    /// <summary>Deconstructs the definition into its constructor values.</summary>
    public void Deconstruct(out string key, out string description, out Func<Profile, bool> resolver)
    {
        key = Key;
        description = Description;
        resolver = Resolver;
    }
}

/// <summary>Outcome of resolving an assumption.</summary>
public enum AssumptionResultKind
{
    /// <summary>The proposition was established.</summary>
    Satisfied,
    /// <summary>The proposition was established as false.</summary>
    NotSatisfied,
    /// <summary>There is insufficient information to decide.</summary>
    Unknown
}

/// <summary>Result of resolving an assumption without exposing unrelated data.</summary>
public sealed record AssumptionResult(string AssumptionKey, AssumptionResultKind Kind, string? Proof = null)
{
    /// <summary>Whether the assumption is satisfied.</summary>
    public bool IsSatisfied => Kind == AssumptionResultKind.Satisfied;
}

/// <summary>Resolves experience assumptions against a profile.</summary>
public interface IProfileAssumptionResolver
{
    /// <summary>Resolves an assumption and returns only its outcome/proof.</summary>
    AssumptionResult Resolve(Profile profile, AssumptionDefinition assumption);
}

/// <summary>Default in-process assumption resolver.</summary>
public sealed class ProfileAssumptionResolver : IProfileAssumptionResolver
{
    /// <inheritdoc />
    public AssumptionResult Resolve(Profile profile, AssumptionDefinition assumption)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(assumption);

        try
        {
            return assumption.Resolver(profile)
                ? new AssumptionResult(assumption.Key, AssumptionResultKind.Satisfied, "profile-claim")
                : new AssumptionResult(assumption.Key, AssumptionResultKind.NotSatisfied);
        }
        catch (KeyNotFoundException)
        {
            return new AssumptionResult(assumption.Key, AssumptionResultKind.Unknown);
        }
        catch (InvalidOperationException)
        {
            return new AssumptionResult(assumption.Key, AssumptionResultKind.Unknown);
        }
    }
}
