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


## Canonical identity versus public persona

A user may deliberately maintain multiple representations of themselves:

- a canonical profile used for trusted account ownership
- a personal social profile
- a publisher/creator profile
- a commercial identity
- an experience-specific identity

These profiles can be privately linked by trusted infrastructure while remaining separate to ordinary observers.

The privacy goal is not merely to hide a field. It is to avoid revealing the relationship itself.

Only an authorized identity-resolution process should be able to connect the profiles. Legal disclosure, where required, belongs to applicable law and validated legal process; it is not a normal profile permission.

## Data licensing is a user choice

Profiles distinguishes three different actions:

1. **Disclosure** — showing information to an authorized observer.
2. **Verification** — proving a proposition without necessarily revealing the underlying fact.
3. **Licensing** — explicitly offering selected information for an economic purpose.

One does not imply another.

A user who never enables licensing has not implicitly offered their data for sale merely because an attribute is public.

When licensing is enabled, the owner controls the micro-data allow-list. A future marketplace can negotiate the actual transaction and settlement independently.

The platform's economic share is therefore a sustainability mechanism, not a transfer of ownership. The core deliberately does not prescribe a percentage.
