# Contributor guide

## Pull requests

**Pull requests are welcome!**  
Please include a clear description of the changes you have made with your request; the title should follow
the [conventional commits](https://www.conventionalcommits.org/en/v1.0.0/) guideline.
All code should be covered by unit tests and comply with the coding guideline in this project.

### Technical expectations

As a framework for supporting unit testing, this project has a high standard for testing itself.  
In order to support this, static code analysis is performed
using [SonarCloud](https://sonarcloud.io/project/overview?id=Testably_aweXpect) with quality gate requiring to

- solve all issues reported by SonarCloud
- have a code coverage of > 90%

Additionally a nightly run on `main` checks the quality of the unit tests of `aweXpect.Core` and the extension
packages with mutation tests
using [Stryker.NET](https://stryker-mutator.io/docs/stryker-net/introduction/). The mutation tests of `aweXpect` are
disabled for now, because Stryker cannot capture the coverage of its tests within the time limit
([stryker-net#3871](https://github.com/stryker-mutator/stryker-net/issues/3871)).

## Building and testing

`dotnet build aweXpect.slnx` builds everything; `./build.ps1 UnitTests` (or `./build.sh UnitTests`) runs the tests
like the CI.

The unit tests run on [TUnit](https://tunit.dev) as Microsoft.Testing.Platform executables, so use `dotnet run`
instead of `dotnet test`:

```shell
dotnet run --project Tests/aweXpect.Tests -f net8.0
dotnet run --project Tests/aweXpect.Tests -f net8.0 -- --treenode-filter "/*/*/ThatString+IsEqualTo+Tests/*"
```

Slow tests are marked with both `[Explicit]` and `[Category(TestCategories.Slow)]` and only run through a filter, e.g.
`--treenode-filter "/*/*/*/*[Category=Slow]"`. The first slow test of a project requires adding the project to
`ProjectsWithSlowTests` in `Pipeline/Build.UnitTest.cs`.

The public API of `aweXpect` and `aweXpect.Core` is compared with the snapshots in `Tests/aweXpect.Api.Tests`. After an
intended API change, accept the new surface with the explicit test `AcceptApiChanges` and commit the updated snapshots:

```shell
dotnet run --project Tests/aweXpect.Api.Tests -- --treenode-filter "/*/*/ApiApprovalTests/AcceptApiChanges"
```

The MSTest, NUnit and xUnit v2 projects under `Tests/Frameworks` still run with `dotnet test`; the TUnit and xUnit v3
projects there run with `dotnet run`.
