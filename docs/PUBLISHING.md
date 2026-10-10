# Profiles Publishing Checklist

## Package identity

- Repository: TrentBest/Profiles
- Package ID: TheSingularityWorkshop.Profiles
- Current version: 0.1.0-alpha.1
- Target framework: net8.0
- License: MIT

## NuGet Trusted Publishing

The repository-scoped publisher should use:

~~~text
Repository:
TrentBest/Profiles

Glob asset filter:
TheSingularityWorkshop.Profiles
~~~

The glob is intentionally restricted to the package ID. Do not use a wildcard when this publisher only needs to publish Profiles.

## Before publishing

1. Review the development branch.
2. Confirm the package metadata.
3. Confirm the README renders correctly.
4. Confirm XML documentation builds without warnings.
5. Confirm the unit-test workflow passes.
6. Build the package locally.
7. Inspect the generated .nupkg.
8. Confirm the package README is present.
9. Confirm no credentials, provider secrets, private identity data, or environment-specific configuration are packaged.
10. Confirm the intended version.
11. Review the NuGet Trusted Publishing configuration.
12. Only then manually invoke the package workflow.

## Build and inspect locally

~~~bash
dotnet restore TheSingularityWorkshop.Profiles.csproj
dotnet build TheSingularityWorkshop.Profiles.csproj --configuration Release
dotnet test tests/TheSingularityWorkshop.Profiles.Tests/TheSingularityWorkshop.Profiles.Tests.csproj --configuration Release
dotnet pack TheSingularityWorkshop.Profiles.csproj --configuration Release --output ./artifacts
~~~

The package workflow is deliberately manual. Adding the workflow does not publish anything by itself.

## What publication means

NuGet publication makes the package available to external consumers. It does not mean the domain is complete in every future direction.

The current package is an intentionally small semantic foundation for:

- entity abstraction
- profile identity
- micro-data
- relationships
- disclosure
- representations
- assumptions
- access transparency
- sovereign personas
- voluntary data licensing

Host applications or dedicated security components must enforce authorization and protect access to the original profile data. Future adapters can add authentication, verification, persistence, jurisdiction policy, communications, Economy, and other infrastructure without changing the core into those systems.

SingularityWarehouse's set-based, mathematically bounded security theory may inform those integrations. Profiles does not mandate that implementation, and its observer-specific representation must not be presented as complete application-wide access control.

## Release boundary

Publication is an explicit release action.

Do not turn publication into an automatic side effect of ordinary development commits.
