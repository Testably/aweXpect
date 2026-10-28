# Public API

The package [aweXpect.Snapshots.PublicApi](https://www.nuget.org/packages/aweXpect.Snapshots.PublicApi) verifies the
public API of an assembly against a reviewed snapshot, so that every change of the public API shows up in a pull
request:

```csharp
await Expect.That(typeof(Album).Assembly).MatchesPublicApiSnapshot()
    .UniqueFor.TargetFramework();
```

The expectation is in the namespace `aweXpect`, like `MatchesSnapshot()`. `PublicApiDeclarations.GenerateFrom(…)`, which
generates the public API without an expectation, and the options are in the namespace `aweXpect.Snapshots.PublicApi`.

The public API is written as C# declarations, e.g.

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

It behaves like [`MatchesSnapshot()` for strings](01-matching-snapshots.md#strings), e.g. for the files and
[accepting changes](02-accepting-snapshots.md). `.And` continues on the assembly or the file, and awaiting the
expectation returns it.

## Target frameworks and reference assemblies

It usually differs between the target frameworks of a library, so keep a snapshot per target framework of the test
project with `.UniqueFor.TargetFramework()`; each target framework of the test project loads the matching build of the
library. A build that the tests cannot run on, e.g. for `netstandard2.0`, or a reference assembly is read from its file
without loading it, and named after its target framework:

```csharp
await Expect.That(new FileInfo(path)).MatchesPublicApiSnapshot().Named("netstandard2.0");
```

A build for a newer .NET than the tests run on needs the .NET SDK of that version, which installs its reference
assemblies.

<details>
<summary>How the types in the signatures are resolved</summary>

The types in the signatures are resolved from the assemblies next to the assembly, in the output of the tests, of the
.NET version it targets and of the current runtime. The NuGet packages of the library, which its build output does not
contain, fill in the assemblies that are missing or only found in a lower version: they are read from the global packages
folder with the versions that the `deps.json` next to the assembly lists. When a referenced assembly cannot be
resolved, or only in a lower version, the expectation throws an `InvalidOperationException` that names it, instead of
writing a public API that differs from machine to machine.

</details>

## Options

Options configure what is written, for the expectation and for `PublicApiDeclarations.GenerateFrom(…)`.
A failure lists the options that differ from the defaults in the "Public API options" context, e.g.
`ignore forwarded types`.

### Forwarded types

The types that the assembly forwards to another assembly are written as
`[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(…))]`, as removing a forward breaks the consumers
that were compiled against the assembly. They can be ignored:

```csharp
await Expect.That(typeof(Album).Assembly).MatchesPublicApiSnapshot(o => o.Ignoring.ForwardedTypes());
```

### Attributes

An attribute is written when consumers can see its class, or when compilers or analyzers read it by its name, e.g.
`[NotNullWhen(true)]` or `[OverloadResolutionPriority(1)]`, so that also a polyfill that the library declares internally
is written. An attribute that is irrelevant for the public API can be ignored everywhere, by its type or, e.g. when the
tests cannot reference it, by its full name like `Type.FullName`:

```csharp
await Expect.That(typeof(Album).Assembly).MatchesPublicApiSnapshot(o => o
    .Ignoring.Attribute<EditorBrowsableAttribute>()
    .Ignoring.Attribute("MusicStore.Internal.MarkerAttribute"));
```

`[Serializable]`, `[DllImport(…)]` and `[Optional]` are written, although the compiler stores them as flags in the
metadata instead of as attributes, as removing them breaks consumers, e.g. of serialization or interop; they can be
ignored like any other attribute.

### Records

A record is written with the members that the compiler synthesizes for it, e.g. `Equals`, `GetHashCode`, `ToString`,
`Deconstruct`, the copy constructor and the equality operators. The members that are marked as `[CompilerGenerated]` can
be ignored, which keeps the primary constructor, the positional properties and the hand-written members of the record:

```csharp
await Expect.That(typeof(Album).Assembly).MatchesPublicApiSnapshot(o => o.Ignoring.CompilerGeneratedMembers());
```

### Namespaces and types

The public API can be restricted to namespaces, including their sub-namespaces, or to types, including their nested
types, and namespaces and types can be ignored. A namespace matches whole segments, so `MusicStore.Generated` neither
includes nor ignores `MusicStore.GeneratedTools`, and ignoring wins over a restriction:

```csharp
await Expect.That(typeof(Album).Assembly).MatchesPublicApiSnapshot(o => o
    .Within.Namespace("MusicStore")
    .Ignoring.Namespace("MusicStore.Generated")
    .Ignoring.Type<LegacyAdapter>());
```

A type is selected by its type or by its full name like `Type.FullName`, for a generic type with any type arguments.
A nested type is written with its containing type. The filters also apply to the forwarded types. An empty namespace
is the global namespace.

### Formatting

Every declaration is written on a single line by default. To see a change of a single parameter as a change of a single
line, write the parameters of methods, constructors, operators, delegates and indexers with more than one parameter on
separate lines:

```csharp
await Expect.That(typeof(Album).Assembly).MatchesPublicApiSnapshot(o => o.Formatting.ParametersOnSeparateLines());
```

So instead of

```csharp
public Album(string title, [System.Runtime.CompilerServices.CallerMemberName] string source = "") { }
```

the declaration is written as

```csharp
public Album(
    string title,
    [System.Runtime.CompilerServices.CallerMemberName] string source = "") { }
```

## Requirements

The assembly is read with Roslyn, so the package depends on a recent version of `Microsoft.CodeAnalysis.CSharp` that
reads all extension members correctly. A test project that needs an older Roslyn, e.g. for testing an analyzer, cannot
use the package, so verify the public API in a separate test project.
