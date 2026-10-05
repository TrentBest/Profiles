# The Singularity Workshop — Profiles

**Profiles are not a user database.**

TheSingularityWorkshop.Profiles is the privacy and representation boundary for entities in the Singularity Workshop ecosystem.

> **Your data belongs to you. The Workshop provides the machinery for deciding what another entity may know, see, prove, or experience.**

## The core model

~~~text
                         PROFILE
                            |
          +-----------------+------------------+
          |                 |                  |
       Micro-data       Relationships       Groups
          |                 |                  |
      disclosure        membership         permissions
          |                 |                  |
          +-----------------+------------------+
                            |
                     REPRESENTATION
                            |
             +--------------+--------------+
             |              |              |
          Friend          Stranger       Experience
             |              |              |
        avatar A       anonymous      avatar B
~~~

The same underlying profile can produce different representations for different observers.

## Micro-data is the unit of control

A profile can hold arbitrary small claims: age, display name, language, location, interests, capabilities, memberships, avatar preferences, and application-defined data.

Each claim can have its own disclosure rule. A profile never needs to become one giant record that every application receives.

## Exclusion

An exclusion is stronger than hiding an avatar.

~~~text
Permitted observer:
    chosen identity + permitted claims + permitted avatar

Excluded observer:
    stable profile ID + anonymous representation
~~~

No age, gender, username, display name, or unrelated profile claims are exposed by the core representation.

## Groups and multiple appearances

Users can define groups such as Friends, Family, Work, Game Guild, Customers, and Trusted Creators.

A person can therefore appear as a fluffy ape to one observer, a Cheshire Cat to friends, and an anonymous representation to someone they have excluded.

**Identity is not appearance.**

An avatar is a representation selected by policy.

## Assumptions: ask for propositions

Experiences should request the smallest proposition they need.

~~~text
Assumption:
    Age >= 21

Result:
    Satisfied
    NotSatisfied
    Unknown
~~~

Unknown is deliberately different from NotSatisfied. A user withholding a claim is not automatically a failed requirement.

The core also does not pretend that an arbitrary self-entered claim is authoritative for legally consequential requirements. Verification providers can be added above this domain.

## Public-data reporting

ProfilePublicDataReporter lets an owner inspect what is currently public without returning private claim values.

The initial report exposes:

- public attribute keys
- whether display name is public
- whether a public/default avatar exists

## Who accessed my data?

ProfileAccessRecord identifies:

~~~text
Profile ID
Observer ID
Access timestamp
~~~

This lets an owning system maintain an owner-visible access history.

The core intentionally does not add the observer's private reason or unrelated information about that observer.

**The profile owner can know who accessed their data without turning that access record into permission to inspect the accessor's private life.**

## Communication and spatial interaction

The same authorization boundary can eventually govern:

~~~text
Profile relationship
       |
       +-- visual representation
       +-- audio communication
       +-- messaging
       +-- presence
       +-- interaction
~~~

An experience can then use proximity and level-of-detail rules to connect sound streams or other interaction channels.

Profiles does not become a renderer or communications transport. It answers the more fundamental question:

> **Is this observer permitted to receive this representation or interaction?**

## Privacy by design

Profiles is designed to make privacy-preserving behavior natural:

- data minimization
- least privilege
- micro-data disclosure
- user-controlled groups
- explicit exclusion
- observer-specific representations
- proposition-level assumptions
- auditable access events
- configurable retention
- jurisdiction-aware policy layers

The package does not claim to make an application legally compliant. California, EU, and other requirements belong in policy and enforcement layers.

## No database dependency

There is deliberately no Entity Framework, SQL, HTTP, or cloud dependency in the core.

~~~text
Profile
 ├─ micro-claims
 ├─ disclosure rules
 ├─ groups
 ├─ relationships
 ├─ exclusions
 └─ observer-specific representations
~~~

A future persistence layer may use memory, files, object storage, a Warehouse adapter, or another system. Profiles does not dictate that choice.

## Economic boundary

Profiles does not hard-code a platform fee.

A future marketplace can separately model requests, purpose, price, consent, settlement, revocation, and revenue allocation.

The user's explicit decision to license information remains distinct from ordinary profile disclosure.

## Architecture

~~~text
                         FSM_API
                            |
                         FSM_COS
                            |
              +-------------+-------------+
              |                           |
          Profiles                    Warehouse
              |                           |
       identity + policy             resources/data
              |                           |
              +-------------+-------------+
                            |
                       Authorization
                            |
                       Experiences
                            |
             +--------------+--------------+
             |              |              |
            Web           Mobile       Immersive
~~~

Profiles is the identity/data sovereignty boundary. Warehouse can be a persistence/resource boundary. Authorization connects the two.

## Initial API

- Profile
- ProfileId
- ProfileEntityKind
- ProfileAttributeDefinition
- ProfileClaim
- DisclosureRule
- DisclosureScope
- ProfileGroup
- ProfileRelationship
- ProfileRepresentation
- ProfilePublicDataReport
- AssumptionDefinition
- AssumptionResult
- IProfileAssumptionResolver
- ProfileAccessRecord
- IProfileAccessRecorder

## What comes next

1. Richer public-data reports.
2. Persistent owner-visible access history.
3. Relationship-aware disclosure rules.
4. Communication authorization.
5. Named representation policies.
6. Trusted verification providers.
7. Jurisdiction policy packages.
8. Explicit data licensing and marketplace settlement.
9. Warehouse persistence/authorization adapter.
10. FSM integration where stateful profile workflows benefit from FSM_API.

> **The profile owns the data boundary. The observer receives only the representation they are permitted to receive.**

---

**Package:** TheSingularityWorkshop.Profiles  
**Version:** 0.1.0-alpha.1  
**Target:** .NET 8  
**Release:** source only; NuGet publication requires explicit review.

![Trent Best](https://avatars.githubusercontent.com/u/16405167?v=4&size=200)

[GitHub — Profiles](https://github.com/TrentBest/Profiles)

**The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.**


## Sovereign identity and publisher personas

A person does not have to publish their canonical identity merely because they participate in the ecosystem.

The same underlying owner can maintain separate profiles:

~~~text
Canonical profile
      |
      +---- personal representation
      +---- publisher profile
      +---- commercial persona
      +---- game identity
~~~

ProfileIdentityLink records these relationships for trusted identity infrastructure. Ordinary observers do not receive the link, and a representation does not reveal that two profiles belong to the same underlying owner.

A valid legal process may require disclosure of an otherwise protected identity. That is an external authority and jurisdiction boundary; the core does not decide whether a request is legally sufficient.

This separation lets a publisher create work under the identity they choose while the Workshop can retain the private linkage needed for account integrity, safety, and lawful obligations.

## Economic boundary

Profiles does not assume that a user wants to sell anything.

**Ordinary disclosure is never treated as permission to sell.**

ProfileDataLicensePolicy can explicitly enable licensing and identify which micro-data attributes are eligible. A separate marketplace/transaction layer must handle buyer, purpose, price, consent, settlement, expiry, and revocation.

The Workshop's economic model is a service-sustainability concern rather than ownership of the user's data. The platform allocation is not hard-coded into Profiles.

> **If you choose to license your data, you decide what is offered. The Workshop takes only the platform share agreed as the cost of keeping the machinery available.**

The user's data remains theirs.
