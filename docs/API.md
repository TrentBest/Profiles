# Profiles API Map

Use this page to orient yourself among the public types. It is a conceptual map, not a complete signature reference; XML documentation in the package remains authoritative for member-level details. Examples and exact behavior should be checked against the current source branch.

## Identity

### ProfileId

Opaque stable identifier for a profile entity.

### Profile

The primary entity/profile boundary.

It owns entity kind, display name, micro-data claims, disclosure rules, groups, relationships, exclusions, avatar policy, and observer-specific representation.

## Entity vocabulary

### ProfileEntityKind

The built-in broad entity categories:

- Individual
- Company
- Group
- Organization
- Experience
- Agent
- Custom

This is intentionally an abstraction rather than a database schema.

## Micro-data

### ProfileAttributeDefinition

Defines a named attribute and its expected CLR type.

### ProfileClaim

Stores one typed value together with optional verification timestamp and provenance. Values are type-checked but not deep-cloned; prefer immutable value objects or treat values as immutable after claim creation.

## Disclosure

### DisclosureScope

Controls the audience for a claim:

- Private
- Public
- Group
- Explicit

### DisclosureRule

Connects an attribute key to a disclosure policy. `AllowedObservers` contains observer IDs for Explicit rules; `AllowedGroups` contains group IDs for Group rules. `RepresentationOverride` substitutes a value for every observer permitted by the rule, so it can disclose a less precise value rather than the underlying claim.

## Groups and relationships

### ProfileGroup

User-controlled membership collection that can participate in disclosure and avatar selection.

### ProfileRelationship

Semantic relationship between two profiles.

The relationship vocabulary remains application-defined.

## Representations

### ProfileRepresentation

Observer-specific projection of a profile.

It is intentionally distinct from Profile. Its claims dictionary is copied and read-only, so callers cannot mutate the representation's structure through the exposed collection. Arbitrary claim values are not deep-cloned, however; immutable values are the safest choice.

### ProfilePublicDataReport

Owner-facing metadata describing which parts of a profile are currently public.

### ProfilePublicDataReporter

Creates public exposure reports without copying private values into the report.

## Assumptions

### AssumptionDefinition

Describes a proposition required by an Experience.

### AssumptionResult and AssumptionResultKind

`AssumptionResult` carries the assumption key, a `Kind`, and optional proof text. `AssumptionResultKind` is the outcome enum: `Satisfied`, `NotSatisfied`, or `Unknown`. The default in-process resolver maps missing dictionary keys and invalid operations to `Unknown`; applications should not treat self-declared claims as independent verification.

### IProfileAssumptionResolver

Integration boundary for resolving propositions. Applications can provide another resolver when they need trusted verification or domain-specific semantics.

### ProfileAssumptionResolver

Default in-process resolver. Its predicate is application-supplied; the resolver does not itself establish that claim values are trustworthy or legally authoritative.

## Sovereign identity

### ProfileIdentityLink

Private relationship between canonical and linked profiles.

### ProfileIdentityLinkStore

Trusted infrastructure boundary for maintaining private identity links.

### ProfileIdentityLinkKind

Built-in reasons for a private link:

- Persona
- Publisher
- Commercial
- Custom

### ExternalIdentityReference

Provider-qualified external identity reference containing issuer/provider and subject. It carries no credentials and does not authenticate the subject.

### ProfileExternalIdentityBinding

Maps a domain profile to an external identity reference. Trusted integration is responsible for authentication, mapping policy, storage, and protection. The mapping does not grant authorization.

## Data licensing

### ProfileDataLicensePolicy

Explicit opt-in allow-list for attributes eligible for licensing.

It does not perform transactions.

## Access transparency

### ProfileAccessRecord

Minimal access event containing profile, observer, and timestamp.

### IProfileAccessRecorder

Boundary for sending access events to application-owned audit/persistence infrastructure. The consuming application supplies the implementation and decides retention, storage, and access controls. Creating a representation does not automatically record an access event; the host must invoke its recorder at the appropriate policy boundary.

## Dependency philosophy

The Profiles domain intentionally has no required dependency on Entity Framework, SQL, HTTP, cloud storage, authentication SDKs, payment SDKs, or rendering frameworks.

This is deliberate. Consumers compose the package with the infrastructure they already use.


---

**The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.**
