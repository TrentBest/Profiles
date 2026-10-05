# Profiles Privacy Model

## Ownership

The default design stance is:

> **Your data is yours.**

The Workshop provides infrastructure for describing, controlling, presenting, and—when explicitly authorized—licensing that data.

Profiles does not transfer ownership merely because a claim exists in a runtime object or persistence adapter.

## Minimum disclosure

An experience should ask for the smallest information necessary.

~~~text
Bad:
    Give the experience the entire user record.

Preferred:
    Resolve the exact proposition the experience needs.
~~~

For example, Age >= 21 can produce a proof/result without handing the experience the user's date of birth.

## Exclusion

An exclusion is an explicit owner decision.

The excluded observer receives:

~~~text
stable ProfileId
anonymous avatar
no display name
no claims
~~~

This is intentionally stronger than an application-level block button that only hides a UI element.

## Observer-specific appearance

Appearance is not identity.

A user can choose different representations for different observers or groups.

~~~text
Public       -> simple avatar
Friends      -> Cheshire Cat
Game guild   -> fantasy creature
Excluded     -> anonymous representation
~~~

The renderer is responsible for rendering the representation. Profiles is responsible for the information boundary.

## Access transparency

A profile owner should be able to inspect who has accessed their data.

The core access record contains:

~~~text
ProfileId
ObserverId
Timestamp
~~~

It does not automatically expose the observer's private reason, unrelated profile information, private activity history, or other users' data.

> Transparency about access is not a blanket authorization to inspect the accessor.

## Public availability

Public exposure should be reportable from policy itself.

~~~text
What of my profile is currently public?
~~~

This should not require copying every profile value into a second public database.

## Retention

The domain does not choose retention periods.

Retention depends on purpose, jurisdiction, consent, security requirements, contractual obligations, and operational needs.

A persistence adapter should make retention explicit.

## Verification

A profile claim may eventually be self-declared, imported, verified, expired, disputed, or revoked.

The initial package only carries an optional verification timestamp and provenance.

A future verification layer should define trusted authorities and proof semantics.

## Jurisdiction

Profiles is privacy-oriented infrastructure, not a legal compliance certificate.

Policy layers should enforce jurisdiction-specific requirements without contaminating the neutral domain API.

## Economic use

If a user explicitly chooses to license data, that should be an explicit transaction.

The platform should not silently reinterpret ordinary disclosure as permission to sell.

A future marketplace can represent purpose, buyer, requested claims, permitted representation, price, duration, consent, revocation, settlement, and platform allocation.
