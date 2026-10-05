# Profiles API Map

This page is a conceptual map of the public API. XML documentation in the package remains the authoritative member-level reference.

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

Stores one typed value together with optional verification timestamp and provenance.

## Disclosure

### DisclosureScope

Controls the audience for a claim:

- Private
- Public
- Group
- Explicit

### DisclosureRule

Connects an attribute key to a disclosure policy and optional public representation.

## Groups and relationships

### ProfileGroup

User-controlled membership collection that can participate in disclosure and avatar selection.

### ProfileRelationship

Semantic relationship between two profiles.

The relationship vocabulary remains application-defined.

## Representations

### ProfileRepresentation

Observer-specific projection of a profile.

It is intentionally distinct from Profile.

### ProfilePublicDataReport

Owner-facing metadata describing which parts of a profile are currently public.

### ProfilePublicDataReporter

Creates public exposure reports without copying private values into the report.

## Assumptions

### AssumptionDefinition

Describes a proposition required by an Experience.

### AssumptionResult

Returns Satisfied, NotSatisfied, or Unknown.

### IProfileAssumptionResolver

Integration boundary for resolving propositions.

### ProfileAssumptionResolver

Default in-process resolver.

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

## Data licensing

### ProfileDataLicensePolicy

Explicit opt-in allow-list for attributes eligible for licensing.

It does not perform transactions.

## Access transparency

### ProfileAccessRecord

Minimal access event containing profile, observer, and timestamp.

### IProfileAccessRecorder

Boundary for sending access events to application-owned audit/persistence infrastructure.

## Dependency philosophy

The Profiles domain intentionally has no required dependency on Entity Framework, SQL, HTTP, cloud storage, authentication SDKs, payment SDKs, or rendering frameworks.

This is deliberate. Consumers compose the package with the infrastructure they already use.
