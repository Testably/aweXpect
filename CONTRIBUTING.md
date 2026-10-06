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

Additionally each push to the `main` branch checks the quality of the unit tests
using [Stryker.NET](https://stryker-mutator.io/docs/stryker-net/introduction/).

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

`aweXpect.Api.Tests` and the projects under `Tests/Frameworks` still run with `dotnet test`.
