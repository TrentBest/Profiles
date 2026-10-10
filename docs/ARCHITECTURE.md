# Profiles Architecture

## Boundary

Profiles owns the semantic boundary between an entity's information and an observer's permitted representation.

~~~text
Entity
  |
  v
Profile
  |
  +-- Claims
  +-- Disclosure
  +-- Groups
  +-- Relationships
  +-- Exclusions
  +-- Representations
  |
  v
Observer
~~~

The core does not own persistence, transport, rendering, communications, or jurisdiction enforcement.

## Micro-data

A ProfileClaim is the smallest current storage unit exposed by the domain API.

~~~text
Profile
  |
  +-- age
  +-- language
  +-- role
  +-- interests
  +-- ...
~~~

A disclosure rule is attached to a claim key, allowing each piece of information to have a separate visibility policy.

## Representation

Profile.RepresentTo produces an observer-specific ProfileRepresentation.

~~~text
Profile != Representation
~~~

The profile contains the owner's information.

The representation contains only what the observer is permitted to receive.

An excluded observer receives the stable profile ID and an anonymous representation, with no profile claims.

## Groups

Groups are profile-owned membership boundaries.

A group can control claim disclosure, avatar selection, and future interaction permissions.

## Assumptions

An experience can provide an AssumptionDefinition.

~~~text
Experience
    |
    | "Age >= 21?"
    v
Profiles
    |
    +--> Satisfied
    +--> NotSatisfied
    +--> Unknown
~~~

The domain deliberately distinguishes Unknown from NotSatisfied.

A missing claim is not proof of a failed proposition.

## Access history

ProfileAccessRecord identifies profile, observer, and time.

The record intentionally does not contain the observer's private reason.

The owner can therefore answer:

> Who accessed my data?

without the Profiles core becoming a surveillance system about the accessor.

## Public-data reporting

ProfilePublicDataReporter returns exposure metadata rather than raw values.

The first report answers which claim keys are public, whether display name is public, and whether a public avatar exists.

## Persistence

The core is persistence-agnostic.

~~~text
Profiles
   |
   +-- in-memory
   +-- file
   +-- object storage
   +-- Warehouse
   +-- distributed service
~~~

No persistence implementation is required by the domain package.

## Security direction

The existing Warehouse security work provides useful architectural inspiration: authorization should be based on membership, capability, intersection, and least privilege rather than broad ownership of an entire data record.

Profiles deliberately starts cleaner.

No hard-coded privileged identifiers belong here.
No bypass flag belongs here.
No application-specific security exception belongs here.

Authorization policy should be explicit, composable, and testable.

## Jurisdiction

Jurisdiction is a policy boundary above the core.

~~~text
Profiles domain
      |
      v
Jurisdiction policy
      |
      v
Experience / service
~~~

## Future communication boundary

~~~text
Profile authorization
       |
       +-- visual
       +-- audio
       +-- text
       +-- presence
       +-- haptics
~~~

Spatial LOD and renderer behavior remain outside Profiles.


## Sovereign identity separation

Identity and presentation are separate layers.

~~~text
                 canonical identity
                        |
             +----------+----------+
             |                     |
       personal profile       publisher profile
             |                     |
       private persona       public persona
             |                     |
             +----------+----------+
                        |
              trusted identity layer
~~~

ProfileIdentityLink exists for trusted infrastructure. It is not emitted by Profile.RepresentTo.

This allows a creator to publish work under a chosen publisher identity without requiring the audience to learn the canonical identity behind it.

Legal disclosure is an authority boundary, not an ordinary observer capability. Any disclosure mechanism must validate the applicable legal process and jurisdiction outside the neutral domain model.

## Data licensing

Licensing is opt-in and distinct from disclosure.

ProfileDataLicensePolicy says whether the owner has enabled licensing and which micro-data keys are eligible. It does not sell data, set prices, identify buyers, settle transactions, or encode a platform fee.

Those concerns belong to a separate marketplace boundary.

## Mathematical model versus enforcement

The set-based, mathematically bounded security work in SingularityWarehouse is a source of architectural theory and inspiration. Profiles should preserve the ability to reason precisely about information, observers, and permitted representations without making one security implementation mandatory for every consumer.

Keep these responsibilities distinct:

- **Domain model:** describes claims, disclosure rules, exclusions, and observer-specific representations.
- **Authorization and enforcement:** the host application or a dedicated security component decides whether an operation is permitted and enforces that decision at every relevant boundary.
- **Mathematical security model:** can inform explicit, composable, testable policies, but does not by itself enforce those policies in a host application.

A representation is a useful information boundary, not a substitute for endpoint authorization, persistence access control, transport protection, identity verification, or operational security. A caller that can access the original `Profile` object can still access its stored claims unless the surrounding system protects that object.

Profiles should not require consumers to adopt a particular authorization framework. Integrations may use set-based or capability-based models where appropriate, while the core remains provider-neutral.

The current package implements profile disclosure and representation behavior; it does not implement the complete SingularityWarehouse security model or claim that mathematical bounds alone guarantee system-wide enforcement.

