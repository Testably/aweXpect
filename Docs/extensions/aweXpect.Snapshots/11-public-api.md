# Public API

The package [aweXpect.Snapshots.PublicApi](https://www.nuget.org/packages/aweXpect.Snapshots.PublicApi) verifies the
public API of an assembly against a reviewed snapshot, so that every change of the public API shows up in a pull
request:

```csharp
using aweXpect.Snapshots.PublicApi;

await Expect.That(typeof(Album).Assembly).MatchesPublicApiSnapshot().UniqueFor.TargetFramework();
```

The expectation and `PublicApiDeclarations.GenerateFrom(…)`, which generates the public API without an expectation, are
in the namespace `aweXpect.Snapshots.PublicApi`.

The public API is written as C# declarations, e.g.

```csharp
namespace MyLibrary
{
    public abstract class Album : System.IComparable<MyLibrary.Album>
    {
        protected Album(string title) { }
        public string Title { get; }
        public abstract System.Collections.Generic.IEnumerable<string> Tracks();
    }
}
```

It usually differs between the target frameworks of a library, so keep a snapshot per target framework of the test
project with `.UniqueFor.TargetFramework()`; each target framework of the test project loads the matching build of the
library. A build that the tests cannot run on, e.g. for `netstandard2.0`, or a reference assembly is read from its file
without loading it, and named after its target framework:

```csharp
await Expect.That(new FileInfo(path)).MatchesPublicApiSnapshot().Named("netstandard2.0");
```

The types in the signatures are resolved from the assemblies next to the assembly, in the output of the tests, of the
.NET version it targets and of the current runtime. A build for a newer .NET than the tests run on needs the .NET SDK of
that version, which installs its reference assemblies. When a referenced assembly cannot be resolved, or only in a lower
version, the expectation throws an `InvalidOperationException` that names it, instead of writing a public API that
differs from machine to machine.

The types that the assembly forwards to another assembly are written as
`[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(…))]`, as removing a forward breaks the consumers
that were compiled against the assembly. Options configure what is written, for the expectation and for
`PublicApiDeclarations.GenerateFrom(…)`:

```csharp
await Expect.That(typeof(Album).Assembly).MatchesPublicApiSnapshot(o => o.ExcludingForwardedTypes());
```

The assembly is read with Roslyn, so the package depends on a recent version of `Microsoft.CodeAnalysis.CSharp` that
reads all extension members correctly. A test project that needs an older Roslyn, e.g. for testing an analyzer, cannot
use the package, so verify the public API in a separate test project.

Otherwise it behaves like [`MatchesSnapshot()` for strings](01-matching-snapshots.md#strings), e.g. for the files and
[accepting changes](02-accepting-snapshots.md).
