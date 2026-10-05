# Consuming Profiles

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

~~~bash
dotnet add package TheSingularityWorkshop.Profiles
~~~

Or:

~~~xml
<PackageReference Include="TheSingularityWorkshop.Profiles" Version="0.1.0-alpha.1" />
~~~

The package currently targets .NET 8.

## Create an entity profile

~~~csharp
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

All three are the same domain abstraction. Their surrounding application behavior can be completely different.

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

~~~csharp
player.SetDisclosureRule(
    new DisclosureRule("language", DisclosureScope.Public));

var representation = player.RepresentTo(observerId);
~~~

Available scopes are Private, Public, Group, and Explicit.

The important boundary is:

~~~text
Profile
  |
  | policy
  v
ProfileRepresentation
  |
  +-- only information the observer may receive
~~~

Do not hand the complete Profile to an untrusted renderer, plugin, or remote application merely because it needs a representation.

## Model relationships

~~~csharp
company.AddRelationship(
    new ProfileRelationship(
        company.Id,
        player.Id,
        "employs"));
~~~

The package does not prescribe every relationship vocabulary. Your application can define the relationship types appropriate to its domain.

## Groups

Groups can control disclosure and representation.

~~~csharp
var friends = player.DefineGroup("Friends");
friends.Add(friendId);

player.SetGroupAvatar("Friends", "cheshire-cat");
~~~

The same entity can therefore have different representations for different audiences.

## Assumptions

An Experience should ask for the smallest proposition it needs.

~~~csharp
var adult = new AssumptionDefinition(
    "age-21-plus",
    "The profile represents someone aged 21 or older.",
    p => (int)p.Claims["age"].Value >= 21);

var result = new ProfileAssumptionResolver()
    .Resolve(player, adult);
~~~

Possible outcomes are Satisfied, NotSatisfied, and Unknown.

Unknown matters. Missing information is not automatically evidence that a proposition is false.

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

Use ProfileIdentityLink and ProfileIdentityLinkStore for trusted infrastructure that needs to know about those private relationships.

Ordinary representations do not emit the link.

## Data licensing

Disclosure and licensing are deliberately separate concepts.

~~~csharp
var policy = new ProfileDataLicensePolicy
{
    Enabled = true
};

policy.Allow("language");
~~~

This says that the owner has opted into licensing that attribute. It does not create a sale, buyer, price, payment, or legal authorization.

A marketplace or transaction layer must handle those concerns.

## Access records

Applications can record access without making Profiles responsible for persistence:

~~~csharp
var access = new ProfileAccessRecord(
    player.Id,
    observerId,
    DateTimeOffset.UtcNow);

recorder.Record(access);
~~~

IProfileAccessRecorder is an integration boundary. Store the event wherever the owning application requires.

## Persistence

Profiles intentionally has no database dependency.

You can put the domain objects behind:

- an in-memory store
- files
- object storage
- a service
- a Warehouse adapter
- another persistence system

The choice belongs to the consuming application.

## Authentication

Profiles is not an authentication provider.

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

Credentials, tokens, provider SDKs, and authentication protocols belong outside this package.

## Privacy boundary

Treat Profile as the owner's domain object and ProfileRepresentation as the observer-facing projection.

That separation makes it possible for an application to render, transmit, or cache a representation without automatically giving that consumer the complete underlying profile.

## What Profiles does not do

Profiles is intentionally not:

- a user database
- an authentication provider
- an identity provider
- a payment processor
- a marketplace
- an accounting system
- a renderer
- a communications transport
- a legal compliance engine
- a jurisdiction engine
- a cloud persistence provider

Those are integration boundaries above or beside the domain package.

## Recommended integration pattern

~~~text
             Authentication
                    |
                    v
              Identity layer
                    |
                    v
             +-------------+
             |   Profiles  |
             +-------------+
              /     |     \
             /      |      \
       Storage   Policy   Verification
          |         |          |
          +---------+----------+
                    |
                    v
              Experience
                    |
          +---------+---------+
          |                   |
      Representation       Assumption
          |                   |
        Renderer          Experience logic
~~~

Keep the Profiles package small. Add application-specific adapters rather than turning the domain package into the application.

## Further reading

- Architecture
- Privacy model
- Theory
- How-to guide
- API map
