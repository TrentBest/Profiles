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
