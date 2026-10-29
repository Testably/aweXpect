# Assemblies

Assemblies are usually used as a [source](../01-sources.md#sources-the-in-helper), but you can also filter
and verify them directly.
[Filters and expectations](./index.md#how-to-read-the-tables) explains how to read the tables.

|                     | Filter                       | Expect (single)          | Expect (many)            | Negated                                                                   |
|---------------------|------------------------------|--------------------------|--------------------------|---------------------------------------------------------------------------|
| by name             | `.WithName("x")`             | `.HasName("x")`          | `.HaveName("x")`         | `WithoutName`, `DoesNotHaveName`, `DoNotHaveName`                         |
| by target framework | `.WhichTarget("net8.0")`     | `.Targets("net8.0")`     | `.Target("net8.0")`      | `WhichDoNotTarget`, `DoesNotTarget`, `DoNotTarget`                        |
| by version          | `.WithVersion(…)`            | `.HasVersion(…)`         | `.HaveVersion(…)`        |                                                                           |
| strong named        | `.WhichAreStrongNamed()`     | `.IsStrongNamed()`       | `.AreStrongNamed()`      | `WhichAreNotStrongNamed`, `IsNotStrongNamed`, `AreNotStrongNamed`         |
| has attribute       | `.With<TAttribute>()`        | `.Has<TAttribute>()`     | `.Have<TAttribute>()`    | `Without<TAttribute>`, `DoesNotHave<TAttribute>`, `DoNotHave<TAttribute>` |
| depends on assembly | `.WhichDependOn("x")`        | `.DependsOn("x")`        | `.DependOn("x")`         | `WhichDoNotDependOn`, `DoesNotDependOn`, `DoNotDependOn`                  |
| depends only on set | `.WhichDependOnlyOn("x", …)` | `.DependsOnlyOn("x", …)` | `.DependOnlyOn("x", …)`  |                                                                           |
| custom predicate    | `.Which(a => …)`             | `.Satisfies(a => …)`     | `.All().Satisfy(a => …)` | `Except(a => …)`                                                          |

```csharp
Assembly subject = Assembly.GetEntryAssembly();
Assembly[] subjects = AppDomain.CurrentDomain.GetAssemblies();

await Expect.That(subject).HasName("MusicStore").AsPrefix();
await Expect.That(subject).DependsOn("System.Core");
await Expect.That(subject).DoesNotDependOn("UnwantedDependency");
await Expect.That(subject).DependsOnlyOn("MusicStore.Domain");
await Expect.That(subjects).Have<AssemblyTitleAttribute>();
await Expect.That(subject).Targets("net8.0");
```

The target framework is matched against the short moniker form (e.g. `net8.0`, `netstandard2.0`, `net48`),
derived from the assembly's `[TargetFramework]` attribute. The negations `WhichDoNotTarget` / `DoesNotTarget` / `DoNotTarget`
accept the same [string matching options](./01-common-filters.md#string-matching-options), and `DoNotTarget` verifies that *none* of
the assemblies targets the framework.

An assembly is considered strong named when its name carries a non-empty public key token.

The version filter and expectations come in two forms. Pass a `Func<Version, bool>` predicate to match the whole
version, or omit it to compare individual components (`WithMajor`, `WithMinor`, `WithBuild`, `WithRevision`).
The filter and the expectations compare a component with the same operators as the `int` properties of core aweXpect,
e.g. `HasLength()`: `EqualTo`, `GreaterThan`, `GreaterThanOrEqualTo`, `LessThan`, `LessThanOrEqualTo`,
`Between(…).And(…)` and their negations `NotEqualTo`, `NotGreaterThan`, `NotBetween(…).And(…)`, and so on. Component
comparisons chain (all must hold).

```csharp
// Predicate form
In.AllLoadedAssemblies().WithVersion(version => version.Major >= 2)
await Expect.That(subject).HasVersion(version => version.Major >= 2);

// Component form
In.AllLoadedAssemblies().WithVersion().WithMajor.GreaterThanOrEqualTo(2).WithMinor.EqualTo(0)
In.AllLoadedAssemblies().WithVersion().WithMajor.Between(2).And(4).WithMinor.NotLessThan(1)
await Expect.That(subject).HasVersion().WithMajor.GreaterThanOrEqualTo(2);
await Expect.That(subjects).HaveVersion().WithMajor.EqualTo(1);
await Expect.That(subject).HasVersion().WithMajor.Between(2).And(4).WithMinor.NotEqualTo(0);
```

On a collection of assemblies, `HaveVersion` and `DependOnlyOn` also follow aweXpect's quantifiers:

```csharp
await Expect.That(subjects).AtLeast(2).HaveVersion().WithMajor.EqualTo(1);
await Expect.That(subjects).AtLeast(2).DependOnlyOn("MusicStore.Domain");
```

<details>
<summary>Assemblies without a target framework or version</summary>

Assemblies without a `[TargetFramework]` attribute are treated as having no target framework and never match. An
assembly without a version never matches a version filter or expectation. The `Build` and `Revision` components are
`-1` when absent from the version.

</details>
