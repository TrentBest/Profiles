namespace TheSingularityWorkshop.Profiles;

/// <summary>Describes a proposition an experience needs resolved.</summary>
public sealed record AssumptionDefinition(string Key, string Description, Func<Profile, bool> Resolver);

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
