# Profiles Theory

## 1. Entity before application

The central idea of Profiles is that an application should not have to invent a different identity abstraction for every kind of thing it can represent.

A person, company, guild, game world, autonomous agent, or custom business entity can all participate in the same ecosystem through a common semantic boundary.

~~~text
                    Entity
                      |
        +-------------+-------------+
        |             |             |
     person        company       experience
        |             |             |
        +-------------+-------------+
                      |
                    Profile
~~~

ProfileEntityKind identifies the broad kind of entity. The rest of the application can remain free to specialize behavior.

This is deliberately not a database inheritance hierarchy.

## 2. Identity is not presentation

A canonical identity answers:

> Which entity is this?

A representation answers:

> What may this observer receive about that entity?

Those questions must remain separate.

~~~text
Canonical Profile
       |
       | disclosure policy
       v
Observer-specific Representation
~~~

Two observers can therefore receive different representations of the same profile without creating two canonical identities.

## 3. Micro-data

Profiles treats individual claims as small units of information.

Instead of giving an application the entire user record, the model encourages giving it the one proposition or attribute it actually needs.

This is the package's micro-data principle.

A claim such as language, age, role, or capability can have an independent disclosure rule.

That makes least-privilege behavior a natural consequence of the domain model.

## 4. Representation is a security boundary

ProfileRepresentation is not merely a view-model convenience.

It is the semantic boundary between owned information and observer-visible information.

~~~text
Profile
  = owned domain information

Representation
  = authorized projection
~~~

An application should be able to reason about the difference explicitly.

An excluded observer receives an anonymous representation rather than the profile's claims.

## 5. Unknown is a first-class result

Experiences frequently need propositions rather than raw facts.

For example:

~~~text
Does this participant satisfy Age >= 21?
~~~

The useful result is:

~~~text
Satisfied
NotSatisfied
Unknown
~~~

Unknown prevents missing information from being silently interpreted as false.

It also allows an experience to ask for additional verification without requiring the underlying claim to be disclosed.

## 6. Verification is separate from assertion

Profiles carries optional provenance and verification time because a claim may have a source.

It does not attempt to declare which sources are legally authoritative.

A mature system can layer:

~~~text
Profile claim
     |
     v
Verification provider
     |
     v
Proof / provenance
     |
     v
Experience assumption
~~~

This keeps the neutral domain useful across jurisdictions and applications.

## 7. Identity sovereignty

A canonical identity does not need to be the same identity a person presents publicly.

A creator may need a canonical account identity, publisher identity, commercial identity, and experience-specific identity.

Private identity links allow trusted infrastructure to maintain those relationships without forcing ordinary observers to learn them.

The linkage itself is sensitive information.

## 8. Disclosure is not licensing

Profiles deliberately distinguishes:

~~~text
Disclosure
    = an observer may receive information.

Verification
    = a proposition can be established.

Licensing
    = the owner explicitly permits an economic use.
~~~

None of these should silently imply another.

A public attribute is not automatically for sale.

A verified proposition does not require revealing the underlying value.

A licensing policy does not execute a transaction.

## 9. Access transparency without surveillance

An owner should be able to know who accessed their information.

That does not imply that the owner should automatically receive all private information about the accessor.

The access boundary is intentionally narrow:

~~~text
Profile
  |
  +-- who accessed me?
  +-- when?
~~~

Transparency about access is not a blanket authorization to inspect the accessor.

## 10. Groups are policy vocabulary

Groups are not only social constructs.

They provide a reusable vocabulary for friends, family, coworkers, customers, guild members, trusted creators, and application-specific audiences.

The same group can eventually influence several forms of interaction without requiring Profiles to become a renderer or communications engine.

## 11. Provider neutrality

Profiles is deliberately independent of authentication vendors, databases, cloud providers, payment providers, rendering frameworks, web frameworks, and jurisdiction-specific services.

Adapters can be composed around it.

## 12. Ecosystem role

Profiles belongs above raw persistence and below application experiences.

~~~text
Persistence / external systems
            |
            v
      Integration layer
            |
            v
         Profiles
            |
            v
   Authorization / policy
            |
            v
       Experiences
~~~

The Workshop can therefore use Profiles as a common language while different Experiences remain free to build their own worlds.

## 13. The deeper abstraction

The most important abstraction is not Profile.

It is the relationship:

~~~text
Entity
   +
Information
   +
Policy
   =
Observer-specific representation
~~~

That relationship is what allows a shared ecosystem to contain many kinds of software without requiring every application to own or understand every user's entire identity record.

> The profile owns the boundary. The observer receives the representation.
