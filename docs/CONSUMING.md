# Consuming Profiles

This guide helps application developers add Profiles to an existing .NET 8 application and choose the right boundary for each responsibility. The examples are checked against the current source shape, but have not been independently compiled as a single tutorial project.

## What this package gives you

**TheSingularityWorkshop.Profiles** gives an application a small, provider-neutral abstraction for describing **entities and the information those entities choose to expose**.

> A Profile is an abstraction of an entity, not a record from a particular database.

The entity can be:

| ProfileEntityKind | Meaning |
|---|---|
| Individual | A person or individual actor |
| Company | A company or commercial entity |
| Group | A social, functional, or community group |
| Organization | An institution or other organized body |
| Experience | A world, application, service, or interactive experience |
| Agent | A machine or autonomous actor |
| Custom | An application-defined entity |

Your application decides what persistence, authentication, networking, UI, or business rules sit around these abstractions.

## Install

Add the package to a .NET 8 project:

~~~bash
dotnet add package TheSingularityWorkshop.Profiles
~~~

The current source package version is `0.1.0-alpha.1`. Confirm package availability on NuGet before depending on that version; repository source and a published package are not the same thing.

## Create an entity profile

~~~csharp
using TheSingularityWorkshop.Profiles;

var company = new Profile(
    ProfileEntityKind.Company,
    "Dragonforge Industries");

var experience = new Profile(
    ProfileEntityKind.Experience,
    "Dragonforge Online");

var player = new Profile(
    ProfileEntityKind.Individual,
    "Ari");
~~~

All three use the same domain abstraction. Their surrounding application behavior can be completely different.

## Add micro-data

Claims are intentionally small and independently addressable.

~~~csharp
var language = ProfileAttributeDefinition.Create(
    "language",
    typeof(string),
    "Preferred language.");

player.SetClaim(new ProfileClaim(language, "en-US"));
~~~

The claim exists in the profile, but that does not make it public.

## Decide what an observer may see

Create or obtain an observer ID from your application's identity layer. The example uses a fresh ID only to demonstrate the API:

~~~csharp
var observerId = ProfileId.New();

player.SetDisclosureRule(
    new DisclosureRule("language", DisclosureScope.Public));

var representation = player.RepresentTo(observerId);
~~~

Available scopes are Private, Public, Group, and Explicit.

~~~text
Profile (owned domain information)
   |
   | disclosure policy + observer
   v
ProfileRepresentation
   |
   +-- observer-facing projection
~~~

Do not hand the complete Profile to an untrusted renderer, plugin, or remote application merely because it needs a representation. The representation is the intended handoff object; the consuming application remains responsible for enforcing its own trust and transport boundaries.

## Model relationships

~~~csharp
company.AddRelationship(
    new ProfileRelationship(
        company.Id,
        player.Id,
        "employs"));
~~~

The relationship subject must be the profile receiving the relationship. The package does not prescribe every relationship vocabulary; your application can define relationship types appropriate to its domain.

## Groups

Groups can participate in disclosure and avatar selection. The observer's group memberships must come from your application, not from an untrusted claim supplied by the observer.

~~~csharp
var friends = player.DefineGroup("Friends");
var friendId = ProfileId.New();
friends.Add(friendId);

player.SetGroupAvatar("Friends", "cheshire-cat");
~~~

When requesting a representation for a member, pass the relevant group IDs as the `observerGroups` argument to `RepresentTo`. Group rules use group IDs as their allowed audience.

## Assumptions

An Experience can ask whether a proposition is satisfied without making every consumer handle the underlying value directly. This example assumes the profile has an integer `age` claim and uses a guarded lookup:

~~~csharp
var adult = new AssumptionDefinition(
    "age-21-plus",
    "The profile represents someone aged 21 or older.",
    p => (int)p.Claims["age"].Value >= 21);

var result = new ProfileAssumptionResolver()
    .Resolve(player, adult);
~~~

Possible outcomes are Satisfied, NotSatisfied, and Unknown. In this example, a missing `age` key raises `KeyNotFoundException`, which the default resolver maps to Unknown. This example assumes that any present `age` claim has an integer value.

For sensitive or legally consequential propositions, put trusted verification above the Profiles domain. The core does not turn self-declared information into legal authority.

## Canonical identity and personas

One owner can maintain separate profiles for different contexts:

~~~text
Canonical Profile
       |
       +--> personal profile
       +--> publisher profile
       +--> commercial profile
       +--> experience identity
~~~

Use `ProfileIdentityLink` and `ProfileIdentityLinkStore` in trusted infrastructure that needs to know about those private relationships. Ordinary representations do not emit the link.

## Data licensing

Disclosure and licensing are deliberately separate concepts.

~~~csharp
var policy = new ProfileDataLicensePolicy
{
    Enabled = true
};

policy.Allow("language");
~~~

This says that the owner has opted into licensing that attribute. It does not create a sale, buyer, price, payment, or legal authorization. A marketplace or transaction layer must handle those concerns.

## Access records

Applications can create an access event without making Profiles responsible for persistence:

~~~csharp
var access = new ProfileAccessRecord(
    player.Id,
    observerId,
    DateTimeOffset.UtcNow);
~~~

Pass the record to your application's implementation of `IProfileAccessRecorder`, and persist it according to your application's retention and access policy. Profiles defines the boundary; it does not supply an audit database.

## Persistence

Profiles intentionally has no database dependency. You can put the domain objects behind an in-memory store, files, object storage, a service, a Warehouse adapter, or another persistence system. The choice belongs to the consuming application.

## Authentication

Profiles is not an authentication provider. It now provides a small contract for mapping a profile to a provider-qualified external identity, but the contract does not authenticate the external principal or validate ownership.

~~~csharp
var profile = new Profile(ProfileEntityKind.Individual, "Ari");
var externalIdentity = new ExternalIdentityReference(
    "https://identity.example.test",
    "provider-subject-123");

var binding = new ProfileExternalIdentityBinding(profile.Id, externalIdentity);
~~~

Create a binding only after trusted authentication integration has established the external principal and authorized the mapping. Store and protect it in infrastructure you control. Do not use an email address as a universal identity key, and do not treat a binding as permission to access a profile or perform an action.

~~~text
Authentication credentials
        |
        v
Authenticated principal
        |
        v
Profiles identity boundary
        |
        v
Profile / representation / policy
~~~

Credentials, tokens, provider SDKs, and authentication protocols belong outside this package. A Profile ID is a domain identifier, not proof that a caller owns that identity.

## Privacy boundary

Treat Profile as the owner's domain object and ProfileRepresentation as the observer-facing projection. This separation makes it possible for an application to render, transmit, or cache a representation without automatically giving that consumer the complete underlying profile. It does not replace authentication, authorization, secure transport, or application-level threat modelling.

## What Profiles does not do

Profiles is intentionally not a user database, authentication provider, identity provider, payment processor, marketplace, accounting system, renderer, communications transport, legal compliance engine, jurisdiction engine, or cloud persistence provider. Those are integration boundaries above or beside the domain package.

## Recommended integration pattern

~~~text
             Authentication
                    |
                    v
              Identity layer
                    |
                    v
             +--------------+
             |   Profiles   |
             +--------------+
              /     |      \
             /      |       \
        Storage   Policy   Verification
           |        |          |
           +--------+----------+
                    |
                    v
                Experience
                    |
          +---------+---------+
          |                   |
     Representation       Assumption
          |                   |
       Renderer         Experience logic
~~~

Keep the Profiles package small. Add application-specific adapters rather than turning the domain package into the application.

## Composing with HeadlessAi

A host may use [TheSingularityWorkshop.HeadlessAi](https://github.com/TrentBest/TheSingularityWorkshop.HeadlessAi) for headless agent execution and Profiles for domain identity or observer-specific persona representation. Keep the packages independently usable: HeadlessAi core need not depend on Profiles. The host can pass only the profile representation needed for a task, then separately enforce tool access, data access, and other permissions. Profile claims are descriptive input, not authorization grants.

## Further reading

- [How-to guide](HOW_TO.md) — task-oriented recipes for common profile operations.
- [Theory](THEORY.md) — the mental model behind entity abstraction, micro-data, and observer-specific disclosure.
- [Architecture](ARCHITECTURE.md) — ownership, dependency direction, and integration seams.
- [Privacy model](PRIVACY.md) — the privacy principles represented by the domain, not a legal-compliance guarantee.
- [API map](API.md) — a conceptual map of public types; XML documentation remains authoritative for member details.
- [Publishing checklist](PUBLISHING.md) — package metadata and the explicit release boundary.

---

**The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.**
