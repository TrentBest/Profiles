# Profiles How-To Guide

This is a task-oriented recipe collection for the current Profiles source. Snippets are checked against the current source shape, but have not been compiled together as one application. Each recipe is independent unless it says otherwise.

Start with the [consuming guide](CONSUMING.md) for installation and the main identity/disclosure boundary. See [API map](API.md) for the public type vocabulary and [architecture](ARCHITECTURE.md) for responsibility ownership.

## Create common entity types

~~~csharp
using TheSingularityWorkshop.Profiles;

var person = new Profile(ProfileEntityKind.Individual, "Ari");
var company = new Profile(ProfileEntityKind.Company, "Dragonforge Industries");
var guild = new Profile(ProfileEntityKind.Group, "Dragonforge Guild");
var university = new Profile(ProfileEntityKind.Organization, "Northwind Institute");
var world = new Profile(ProfileEntityKind.Experience, "Dragonforge Online");
var bot = new Profile(ProfileEntityKind.Agent, "Forge Assistant");
~~~

All of these are Profiles. The consuming application decides what each entity means operationally.

## Make a claim private

~~~csharp
var person = new Profile(ProfileEntityKind.Individual, "Ari");
var location = ProfileAttributeDefinition.Create("location", typeof(string));

person.SetClaim(new ProfileClaim(location, "Tacoma"));
person.SetDisclosureRule(
    new DisclosureRule("location", DisclosureScope.Private));

var observerId = ProfileId.New();
var representation = person.RepresentTo(observerId);
// The location claim is not included by this Private rule.
~~~

A claim and its disclosure rule are separate: adding a value does not automatically make it public.

## Publish a claim

~~~csharp
var person = new Profile(ProfileEntityKind.Individual, "Ari");
var language = ProfileAttributeDefinition.Create("language", typeof(string));

person.SetClaim(new ProfileClaim(language, "en-US"));
person.SetDisclosureRule(
    new DisclosureRule("language", DisclosureScope.Public));

var representation = person.RepresentTo(ProfileId.New());
// The representation may include the language claim.
~~~

Only the selected claim becomes public under this rule; other claims do not become public by association.

## Give one entity explicit access

~~~csharp
var person = new Profile(ProfileEntityKind.Individual, "Ari");
var trustedObserver = ProfileId.New();
var phone = ProfileAttributeDefinition.Create("phone", typeof(string));

person.SetClaim(new ProfileClaim(phone, "+1-555-0100"));
person.SetDisclosureRule(
    new DisclosureRule(
        "phone",
        DisclosureScope.Explicit,
        new HashSet<ProfileId> { trustedObserver }));

var allowed = person.RepresentTo(trustedObserver);
var other = person.RepresentTo(ProfileId.New());
// The phone claim is only intended for the trustedObserver.
~~~

Treat the example phone number as fictional sample data. In a real application, the observer ID must come from your trusted identity/authentication boundary.

## Give a group access

~~~csharp
var person = new Profile(ProfileEntityKind.Individual, "Ari");
var siblingId = ProfileId.New();
var sibling = person.DefineGroup("Family");
sibling.Add(siblingId);

var emergencyContact = ProfileAttributeDefinition.Create(
    "emergency-contact",
    typeof(string));
person.SetClaim(new ProfileClaim(emergencyContact, "Contact the family representative"));
person.SetDisclosureRule(
    new DisclosureRule(
        "emergency-contact",
        DisclosureScope.Group,
        new HashSet<ProfileId> { sibling.Id }));

var representation = person.RepresentTo(
    siblingId,
    new HashSet<ProfileId> { sibling.Id });
~~~

The allowed IDs for a Group rule are group IDs. The observer's membership is supplied through `observerGroups`; it must be established by the consuming application.

## Give different observers different avatars

~~~csharp
var person = new Profile(ProfileEntityKind.Individual, "Ari");
var friendId = ProfileId.New();
var vipId = ProfileId.New();

person.SetPublicAvatar("simple-avatar");

var friends = person.DefineGroup("Friends");
friends.Add(friendId);

person.SetGroupAvatar("Friends", "cheshire-cat");
person.SetAvatar(vipId, "formal-portrait");

var publicView = person.RepresentTo(ProfileId.New());
var friendView = person.RepresentTo(
    friendId,
    new HashSet<ProfileId> { friends.Id });
var vipView = person.RepresentTo(vipId);
~~~

Avatar resolution follows the current policy order:

1. observer-specific avatar
2. first matching group avatar
3. public avatar
4. anonymous avatar

The strings are avatar identifiers/values for the consuming renderer; Profiles does not render images.

## Exclude an observer

~~~csharp
var person = new Profile(ProfileEntityKind.Individual, "Ari");
var blockedObserver = ProfileId.New();

person.Exclude(blockedObserver);
var representation = person.RepresentTo(blockedObserver);
~~~

The excluded representation contains the stable profile ID, no display name, the anonymous avatar, and no claims. This is a domain-level representation rule, not a replacement for network authorization or account security.

## Ask an assumption instead of requesting raw data

~~~csharp
var person = new Profile(ProfileEntityKind.Individual, "Ari");
var age = ProfileAttributeDefinition.Create("age", typeof(int));
person.SetClaim(new ProfileClaim(age, 24));

var adult = new AssumptionDefinition(
    "age-21-plus",
    "The profile represents someone aged 21 or older.",
    p => p.Claims.TryGetValue("age", out var claim)
         && claim.Value is int value
         && value >= 21);

var result = new ProfileAssumptionResolver().Resolve(person, adult);
~~~

For production systems, place trusted verification and authorization around the assumption rather than treating self-declared values as legal proof. A claim's presence or a successful predicate is not itself independent verification.

## Create a private publisher identity

~~~csharp
var canonical = new Profile(
    ProfileEntityKind.Individual,
    "Canonical owner");

var publisher = new Profile(
    ProfileEntityKind.Company,
    "Dragonforge Publishing");

var links = new ProfileIdentityLinkStore();
links.Add(new ProfileIdentityLink(
    canonical.Id,
    publisher.Id,
    ProfileIdentityLinkKind.Publisher));
~~~

The link is available to the trusted identity layer. It is not emitted by `RepresentTo`. Protect the link store as sensitive identity-resolution data.

## Opt into data licensing

~~~csharp
var policy = new ProfileDataLicensePolicy
{
    Enabled = true
};

policy.Allow("language");

if (policy.CanLicense("language"))
{
    // Hand the explicit licensing decision to a marketplace/transaction layer.
}
~~~

Profiles does not create or settle the sale. A separate system must define the buyer, purpose, terms, price, consent record, revocation behavior, and transaction lifecycle.

## Produce a public exposure report

~~~csharp
var person = new Profile(ProfileEntityKind.Individual, "Ari");
var language = ProfileAttributeDefinition.Create("language", typeof(string));
person.SetClaim(new ProfileClaim(language, "en-US"));
person.SetDisclosureRule(new DisclosureRule("language", DisclosureScope.Public));

var report = ProfilePublicDataReporter.Create(person);
foreach (var key in report.PublicAttributes)
{
    Console.WriteLine(key);
}
~~~

The report contains metadata about exposure, not the private values themselves.

## Record access

~~~csharp
var person = new Profile(ProfileEntityKind.Individual, "Ari");
var observerId = ProfileId.New();
var record = new ProfileAccessRecord(
    person.Id,
    observerId,
    DateTimeOffset.UtcNow);

// Send record to your application's IProfileAccessRecorder implementation.
~~~

The consuming system owns the recorder implementation, retention policy, and persistence. Profiles does not create an audit database.

## Build a creator-defined business ecosystem

Profiles can describe entities around a creator's software without becoming that software's business model.

~~~text
Company
   |
   +-- owns --> Experience
   |
   +-- employs --> Individuals
   |
   +-- operates --> Groups
   |
   +-- publishes --> Products / services
~~~

Economy, MicroBundles, Experiences, and other Workshop packages may reference Profiles rather than invent competing identity models. Those integrations should be treated as design opportunities unless a specific adapter or working integration is documented.

## Keep application behavior outside Profiles

Prefer:

~~~text
Profiles
  -> entity identity
  -> claims
  -> relationships
  -> disclosure
  -> representation
~~~

over:

~~~text
Profiles
  -> database
  -> login
  -> payments
  -> rendering
  -> application-specific workflow
~~~

The package stays useful because the boundary stays small.

---

**The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.**
