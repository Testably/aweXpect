# aweXpect.Snapshots.PublicApi

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Snapshots.PublicApi)](https://www.nuget.org/packages/aweXpect.Snapshots.PublicApi)

[aweXpect.Snapshots.PublicApi](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Snapshots.PublicApi) verifies the public API of an assembly against a reviewed snapshot of [aweXpect.Snapshots](https://www.nuget.org/packages/aweXpect.Snapshots), so that every change of the public API shows up in a pull request.

## Installation

```shell
dotnet add package aweXpect.Snapshots.PublicApi
```

The package brings aweXpect.Snapshots with it. `MatchesPublicApiSnapshot` lives in the namespace `aweXpect`; `PublicApiDeclarations.GenerateFrom(…)` and the options need `using aweXpect.Snapshots.PublicApi;`.

The examples write `That(...)` instead of `Expect.That(...)`, which needs `using static aweXpect.Expect;` (see [Getting started](https://docs.testably.org/aweXpect/getting-started#install-the-package)).

## At a glance

```csharp
// verify the public API of an assembly, with a snapshot per target framework
await That(typeof(Album).Assembly).MatchesPublicApiSnapshot()
    .UniqueFor.TargetFramework();

// verify a build that the tests cannot load, e.g. for netstandard2.0, or a reference assembly
await That(new FileInfo(path)).MatchesPublicApiSnapshot().Named("netstandard2.0");

// restrict or filter what is written
await That(typeof(Album).Assembly).MatchesPublicApiSnapshot(o => o
    .Within.Namespace("MusicStore")
    .Ignoring.Namespace("MusicStore.Generated")
    .Ignoring.Attribute<EditorBrowsableAttribute>()
    .Formatting.ParametersOnSeparateLines());
```

The public API is written as C# declarations and stored with the extension `cs`, e.g. `AlbumTests.PublicApi@net8.0.awexpect-snapshot.cs`:

```csharp
namespace MusicStore
{
    public abstract class Album : System.IComparable<MusicStore.Album>
    {
        protected Album(string title) { }
        public string Title { get; }
        public abstract System.Collections.Generic.IEnumerable<string> Tracks();
    }
}
```

The assembly is read with Roslyn, so the package depends on a recent version of `Microsoft.CodeAnalysis.CSharp`. A test project that needs an older Roslyn, e.g. for testing an analyzer, verifies the public API in a separate test project.

## Documentation

The full documentation is available at [docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Snapshots/public-api):

- [Public API](https://docs.testably.org/aweXpect/extensions/aweXpect.Snapshots/public-api): target frameworks and reference assemblies, forwarded types, attributes, records, namespaces and types, formatting
- [aweXpect.Snapshots](https://docs.testably.org/aweXpect/extensions/aweXpect.Snapshots/): matching, accepting and naming snapshots
