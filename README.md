# The Singularity Workshop — Profiles

[![Code Coverage](https://codecov.io/gh/TrentBest/Profiles/graph/badge.svg)](https://codecov.io/gh/TrentBest/Profiles)

![Profiles architecture](docs/images/profiles-architecture.svg)

**Profiles is the Workshop's provider-neutral entity, identity, representation, and data-sovereignty abstraction.**

> A Profile is an abstraction of an entity — not a database record.

That distinction matters. When you consume **TheSingularityWorkshop.Profiles**, you get one common semantic vocabulary for the kinds of entities that participate in an ecosystem:

| Entity | ProfileEntityKind | Example |
|---|---|---|
| Individual | Individual | a person, creator, participant |
| Company | Company | a business, studio, publisher |
| Group | Group | guild, team, community |
| Organization | Organization | school, institution, nonprofit |
| Experience | Experience | game, world, application, service |
| Agent | Agent | autonomous software or machine actor |
| Custom | Custom | an application-defined entity |

The consuming application owns the behavior around those entities. Profiles does not become your database, login provider, renderer, payment processor, or application framework.

## The central idea

~~~text
                         ENTITY
                            |
                            v
                         PROFILE
                            |
             +--------------+--------------+
             |              |              |
          Claims       Relationships     Groups
             |              |              |
        disclosure       semantics       policy
             +--------------+--------------+
                            |
                            v
                OBSERVER-SPECIFIC
                  REPRESENTATION
~~~

The same underlying Profile can produce different representations for different observers.

## Identity is not appearance

A canonical identity answers **which entity is this?**

A representation answers **what may this observer receive?**

~~~text
Canonical Profile
       |
       | disclosure / audience policy
       v
ProfileRepresentation
       |
       +-- display name
       +-- avatar
       +-- permitted claims
~~~

This makes privacy a domain boundary rather than a collection of UI tricks.

## Micro-data is the unit of control

Profiles encourages applications to request the smallest useful piece of information.

A profile may contain claims such as:

- age
- language
- role
- location
- interests
- capabilities
- memberships
- application-defined attributes

Each claim can have its own disclosure rule.

A consumer does not have to receive the entire profile merely because it needs one fact.

## Disclosure

The core provides four disclosure scopes:

- **Private** — not disclosed by the rule.
- **Public** — available to observers.
- **Group** — available to members of a selected group.
- **Explicit** — available to selected entities.

~~~csharp
var language = ProfileAttributeDefinition.Create("language", typeof(string));
profile.SetClaim(new ProfileClaim(language, "en-US"));
profile.SetDisclosureRule(
    new DisclosureRule("language", DisclosureScope.Public));
~~~

Then:

~~~csharp
var representation = profile.RepresentTo(observerId);
~~~

The representation contains only what policy allows.

## Groups, avatars, and exclusion

A Profile can define groups such as Friends, Family, Customers, Guild, or Trusted Creators.

The same entity can appear differently to different observers:

~~~text
Public observer   -> public avatar
Friend             -> group avatar
Trusted observer   -> explicit avatar
Excluded observer  -> anonymous representation
~~~

An exclusion is stronger than hiding a UI element. The excluded representation contains no display name and no claims.

**Identity is not appearance.**

## Relationships

Relationships are first-class semantic links between Profiles.

~~~csharp
company.AddRelationship(
    new ProfileRelationship(
        company.Id,
        person.Id,
        "employs"));
~~~

The package does not dictate the complete relationship vocabulary. Your application can define relationships appropriate to its domain.

This means a consumer can describe:

~~~text
Company
  |
  +-- employs --> Individual
  +-- owns -----> Experience
  +-- operates --> Group
  +-- publishes -> products / services
~~~

Profiles supplies the entity boundary. Other Workshop packages can build their own domain models on top of it.

## Assumptions instead of unnecessary disclosure

An Experience should often ask for a proposition rather than raw personal data.

~~~text
Experience asks:

    Age >= 21?

Profiles returns:

    Satisfied
    NotSatisfied
    Unknown
~~~

Unknown is deliberately distinct from NotSatisfied.

Missing information is not automatically evidence that a proposition is false.

The default resolver is intentionally small. Trusted verification providers can be layered above the domain for claims that require stronger authority.

## Canonical identity and public personas

Profiles supports private links between profiles.

~~~text
Canonical identity
       |
       +---- personal profile
       +---- publisher profile
       +---- commercial profile
       +---- experience identity
~~~

A creator can therefore publish under a chosen persona without requiring ordinary observers to learn the canonical identity behind it.

ProfileIdentityLinkStore is for trusted identity infrastructure. The link is not emitted by Profile.RepresentTo.

## Access transparency

The domain includes a deliberately small access event:

~~~text
Profile
Observer
Timestamp
~~~

An owning application can send that event to its own audit or persistence system through IProfileAccessRecorder.

Transparency about who accessed a profile does not automatically grant permission to inspect the accessor's private life.

## Public exposure reporting

ProfilePublicDataReporter lets an owner inspect exposure metadata without copying private values into the report.

It answers questions such as:

- Which attribute keys are public?
- Is the display name public?
- Is a public avatar selected?

## Data licensing is separate from disclosure

Profiles deliberately distinguishes:

~~~text
Disclosure
    -> an observer may receive information.

Verification
    -> a proposition can be established.

Licensing
    -> the owner explicitly permits an economic use.
~~~

ProfileDataLicensePolicy is an opt-in allow-list for attributes.

It does **not**:

- create a sale
- identify a buyer
- set a price
- execute payment
- settle revenue
- determine legal consent
- encode a platform fee

Those belong to marketplace, transaction, policy, and legal layers outside this package.

## Provider-neutral by design

Profiles has no required dependency on:

- Entity Framework
- SQL
- HTTP
- cloud storage
- authentication SDKs
- payment SDKs
- rendering frameworks

A consuming application can persist Profiles in memory, files, object storage, a service, Warehouse adapters, or another system.

~~~text
                 Authentication
                        |
                        v
                  Identity layer
                        |
                        v
                   PROFILES
                  /    |    \
                 /     |     \
           Storage   Policy  Verification
                 \     |     /
                  \    |    /
                        v
                   EXPERIENCE
                        |
              +---------+---------+
              |                   |
       Representation        Assumption
              |                   |
          Renderer          App logic
~~~

## How to consume it

~~~bash
dotnet add package TheSingularityWorkshop.Profiles
~~~

Start with:

- [Consuming Profiles](docs/CONSUMING.md)
- [How-to guide](docs/HOW_TO.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Theory](docs/THEORY.md)
- [Privacy model](docs/PRIVACY.md)
- [API map](docs/API.md)

## Package quality boundary

The package is intentionally domain-focused.

It is **not** a legal compliance certificate, identity provider, authentication service, financial service, or guarantee that a consuming application's policies satisfy any jurisdiction.

Those decisions belong to the application and its qualified professional and infrastructure layers.

## NuGet readiness

- Target framework: .NET 8
- XML documentation enabled
- Warnings treated as errors
- Package README included
- MIT package license metadata
- NuGet Trusted Publishing prepared for TrentBest/Profiles
- Package ID: TheSingularityWorkshop.Profiles
- Current version: 0.1.0-alpha.1

Publication remains an explicit release action.

---

![Trent Best](https://avatars.githubusercontent.com/u/16405167?v=4&size=200)

[GitHub — Profiles](https://github.com/TrentBest/Profiles)

**The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.**
