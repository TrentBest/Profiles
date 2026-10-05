# Profiles How-To Guide

## Create the common entity types

~~~csharp
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
var location = ProfileAttributeDefinition.Create(
    "location",
    typeof(string));

person.SetClaim(new ProfileClaim(location, "Tacoma"));

person.SetDisclosureRule(
    new DisclosureRule("location", DisclosureScope.Private));
~~~

A representation will not contain the claim unless a policy explicitly permits it.

## Publish a claim

~~~csharp
person.SetDisclosureRule(
    new DisclosureRule("language", DisclosureScope.Public));
~~~

Only the selected claim becomes public.

## Give one entity explicit access

~~~csharp
person.SetDisclosureRule(
    new DisclosureRule(
        "phone",
        DisclosureScope.Explicit,
        new HashSet<ProfileId> { trustedObserver.Id }));
~~~

## Give a group access

~~~csharp
var family = person.DefineGroup("Family");
family.Add(sibling.Id);

person.SetDisclosureRule(
    new DisclosureRule(
        "emergency-contact",
        DisclosureScope.Group,
        new HashSet<ProfileId> { family.Id }));

var representation = person.RepresentTo(
    sibling.Id,
    new HashSet<ProfileId> { family.Id });
~~~

The AllowedEntities values for Group rules are group IDs. The observer's membership is supplied through observerGroups.

## Give different observers different avatars

~~~csharp
person.SetPublicAvatar("simple-avatar");

var friends = person.DefineGroup("Friends");
friends.Add(friend.Id);

person.SetGroupAvatar("Friends", "cheshire-cat");
person.SetAvatar(vip.Id, "formal-portrait");
~~~

Resolution order is:

1. observer-specific avatar
2. first matching group avatar
3. public avatar
4. anonymous

## Exclude an observer

~~~csharp
person.Exclude(blockedObserver.Id);

var representation = person.RepresentTo(blockedObserver.Id);
~~~

The excluded representation contains the stable profile ID, no display name, anonymous avatar, and no claims.

## Ask an assumption instead of requesting raw data

~~~csharp
var adult = new AssumptionDefinition(
    "age-21-plus",
    "The profile represents someone aged 21 or older.",
    p => p.Claims.TryGetValue("age", out var claim)
         && claim.Value is int age
         && age >= 21);

var result = new ProfileAssumptionResolver()
    .Resolve(person, adult);
~~~

For production systems, place trusted verification and authorization around the assumption rather than treating self-declared values as legal proof.

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

The link is available to the trusted identity layer. It is not emitted by RepresentTo.

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

Profiles does not create the sale.

## Produce a public exposure report

~~~csharp
var report = ProfilePublicDataReporter.Create(person);

foreach (var key in report.PublicAttributes)
{
    Console.WriteLine(key);
}
~~~

The report contains metadata about exposure, not the private values themselves.

## Record access

~~~csharp
var record = new ProfileAccessRecord(
    person.Id,
    observer.Id,
    DateTimeOffset.UtcNow);

recorder.Record(record);
~~~

The consuming system owns the recorder implementation and persistence.

## Build a creator-defined business ecosystem

Profiles can describe the entities around a creator's software without becoming that software's business model.

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

Economy, Micro Bundles, Experiences, and other Workshop packages can reference these profiles rather than inventing competing identity models.

## Do not put application behavior into Profiles

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
