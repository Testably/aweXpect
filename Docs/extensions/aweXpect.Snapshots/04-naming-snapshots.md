---
description: Name snapshots per test case, per argument or per environment, and nest them under their test file in the IDE.
---

# Naming snapshots

The cases of a parameterized test share the name of the test method, so give each case its own snapshot with
`.Named(…)`:

```csharp
[Test]
[Arguments("Abbey Road")]
[Arguments("Let It Be")]
public async Task Describes(string title)
{
    string description = Describe(new Album(title));

    await That(description).MatchesSnapshot().Named(title);
}
```

The name is appended to the file names, e.g. `AlbumTests.Describes.Abbey Road.awexpect-snapshot.txt`. The same
applies to several snapshots in one test method.

<details>
<summary>Escaped characters and long names</summary>

Characters that a file name cannot hold on every operating system (`< > : " / \ | ? *` and control characters), as well
as `%` and `.`, are escaped as `%XX` in the file name, e.g. `.Named("AC/DC")` is stored in
`ArtistTests.Describes.AC%2FDC.awexpect-snapshot.txt`. The same holds for `@`, which separates the environment (see
below).

A name longer than 64 characters in the file name, where each escaped character takes three, is shortened to 55
characters, followed by `~` and a hash of the whole name, e.g.
`Sgt. Pepper's Lonely Hearts Club Band (Remastered Sup~e0c652ac`, so that the path of the snapshot stays short enough
for every file system. This applies to every name, also to one from `.Named(…)`.

</details>

A snapshot that was already used for another value in the same test run fails, so that the cases cannot overwrite each
other's pending snapshot:

```text title="Failure message"
Expected that description
matches the snapshot "/src/Tests/AlbumTests.Describes.awexpect-snapshot.txt",
but the snapshot was already used for another value in this test run, so name each one with .Named(…) or .NamedAfter(…)
```

The same value again is allowed, so that a retried or repeated test still compares with its snapshot.

<details>
<summary>Retried tests and names that only differ in case</summary>

When the test framework reports retries (NUnit and TUnit), a retry may also use another value than its failed attempt.
NUnit also counts the runs of `[Repeat]` as retries, so while accepting, a repeated test whose value changes keeps the
value of its last run instead of failing.

Names that only differ in case, e.g. `.Named("Live")` and `.Named("live")`, fail as well, also for the same value, as
they are the same file on Windows and macOS but two files on Linux:

```text title="Failure message"
Expected that description
matches the snapshot "/src/Tests/AlbumTests.Describes.live.awexpect-snapshot.txt",
but the snapshot "/src/Tests/AlbumTests.Describes.Live.awexpect-snapshot.txt", whose id only differs in case, was already used in this test run, so give them names that differ in more than the case with .Named(…) or .NamedAfter(…)
```

</details>

## Naming after the arguments

`.NamedAfter(…)` names the snapshot after any number of arguments of the test, which works with every test framework
and under Native AOT:

```csharp
[Test]
[Arguments("Abbey Road", 1969)]
[Arguments("Let It Be", 1970)]
public async Task Describes(string title, int year)
{
    string description = Describe(new Album(title, year));

    await That(description).MatchesSnapshot().NamedAfter(title, year);
}
```

The values are written in the invariant culture and separated by `_`, e.g.
`AlbumTests.Describes.Abbey Road_1969.awexpect-snapshot.txt`. The name holds only the values, so that renaming a
parameter keeps the snapshot, and it is escaped and shortened like any other name.

<details>
<summary>How the values are written</summary>

- `null` is written as `null` and an empty string as `""`.
- A `double` or `float` is written with the fewest digits that round-trip, the same on .NET Framework and .NET, e.g.
  `0.3333333333333333` for `1.0 / 3`.
- A date or time is written in ISO 8601, e.g. `1969-09-26T00:00:00.0000000`.
- A collection, tuple, record or other object is written like in a snapshot, e.g. `[1, 2, 3]` or
  `(4.5, "Something")`, with the items of a set sorted. An object with its own `ToString()` is written by it, any other
  one with its members.

</details>

## Naming after the test case

:::note[Experimental]
This setting is a proposal and off by default, as it changes the snapshot of every parameterized test whose cases
shared one so far.
:::

With `NamedAfterTestArguments`, each snapshot of a parameterized test is named after the arguments of the running test
case, so that its cases are kept apart without `.Named(…)` or `.NamedAfter(…)`:

```csharp
Customize.aweXpect.Snapshots().NamedAfterTestArguments.Set(true); // for this async flow
```

```csharp
[Test]
[Arguments("Abbey Road", 1969)]
[Arguments("Let It Be", 1970)]
public async Task Describes(string title, int year)
{
    string description = Describe(new Album(title, year));

    await That(description).MatchesSnapshot();
}
```

The arguments are written like those of `.NamedAfter(…)`, e.g.
`AlbumTests.Describes.Abbey Road_1969.awexpect-snapshot.txt`. A test without parameters keeps its snapshot.
`.Named(…)` is appended to the arguments, e.g. `.Named("cover")` gives `Abbey Road_1969_cover`, so that each case can
hold several snapshots, while `.NamedAfter(…)` replaces them.

The name has to be the same in every run, so name a test with `.Named(…)` or `.NamedAfter(…)` instead when one of its
arguments is written differently each time, e.g. a `Guid.NewGuid()`, a `DateTime.Now`, a mock or any other object whose
`ToString()` contains a random value or a hash code. Otherwise each run looks for another snapshot and fails, as it
does not exist yet.

<details>
<summary>Arguments of the test class, injected objects and cases written alike</summary>

The arguments of a parameterized test class come first, e.g. `de-DE_Abbey Road` for NUnit `[TestFixture("de-DE")]` or
TUnit `[Arguments("de-DE")]` on the class, so that its instances are kept apart as well. Only values are included, e.g.
strings, numbers, dates or collections of them, so that an object that the test framework injects, e.g. a shared
fixture, is skipped. xUnit v3 and MSTest have no arguments of the test class, and NUnit 3 does not expose them.

The arguments of the test method are all written, except an injected `CancellationToken`, so name a test with
`.Named(…)` or `.NamedAfter(…)` when the test framework injects an object into one of its parameters, e.g. with TUnit
`[ClassDataSource]`.

Two cases whose arguments are written alike, e.g. two objects whose own `ToString()` returns the same text, share a
snapshot and fail like any snapshot that is used for another value. When the `ToString()` of an argument throws, the
name holds the exception like a snapshot would.

</details>

The running test is only known when the test framework exposes it, see
[the running test](/aweXpect/setup/test-frameworks#the-running-test) for the
supported test frameworks.

MSTest 4 marks `TestContext.Current` as experimental, so it is only used when the test project opts in, which also
suppresses the diagnostic `MSTESTEXP` in the generated code:

```xml
<PropertyGroup>
  <AweXpectExperimentalMSTest>true</AweXpectExperimentalMSTest>
</PropertyGroup>
```

Without it, the snapshot keeps the name of the test method, as without the setting.

## Snapshots per environment

Some output legitimately differs between environments, e.g. an exception message on .NET Framework and .NET, or a
path on Windows and Linux. Keep a snapshot for each of them with `.UniqueFor`, and combine several by repeating it:

```csharp
await That(exception.Message).MatchesSnapshot()
    .UniqueFor.TargetFramework()
    .UniqueFor.OperatingSystem();
```

The environment is appended to the file names after an `@`, e.g.
`AlbumTests.Describes@net8.0@windows.awexpect-snapshot.txt`, or
`AlbumTests.Describes.Abbey Road@net8.0@windows@x64.awexpect-snapshot.txt` together with `.Named("Abbey Road")`:

| Option                         | Values                                                           |
|--------------------------------|------------------------------------------------------------------|
| `.UniqueFor.TargetFramework()` | the target framework of the test project, e.g. `net8.0`, `net48` |
| `.UniqueFor.OperatingSystem()` | `windows`, `linux`, `osx` or `freebsd`                           |
| `.UniqueFor.Architecture()`    | the architecture of the process, e.g. `x64`, `arm64`             |

The values always keep this order, whatever the order of the calls. A missing snapshot of the environment fails like
any other missing snapshot; it does not fall back to the snapshot that every environment shares.

A [custom store](08-advanced/01-storing-snapshots.md) receives the environment as `SnapshotId.Variant`, e.g. `net8.0@windows`.

<details>
<summary>How the environment is determined</summary>

The target framework is the one the build knows (`$(TargetFramework)`), which the build targets of aweXpect.Snapshots
record in the test assembly, so `.UniqueFor.TargetFramework()` throws an `InvalidOperationException` when they are
missing. On another operating system, `.UniqueFor.OperatingSystem()` throws a `PlatformNotSupportedException`.

</details>

## Nesting in the IDE

In a C# project, the build targets of aweXpect.Snapshots nest each snapshot and pending file under its test file, so
that Rider and Visual Studio show them below it in the solution explorer.

<details>
<summary>How the test file is found</summary>

As the names of test files and snapshots can both contain dots, the test file is the longest prefix of the file name,
of up to eight parts separated by a dot, for which a `.cs` file exists in the same directory, e.g.
`AlbumTests.Describes.Abbey Road.awexpect-snapshot.txt` is nested under `AlbumTests.Describes.cs` if it exists, and
under `AlbumTests.cs` otherwise.

</details>

A `DependentUpon` that you set yourself is kept, e.g. to nest the snapshots under another file:

```xml
<ItemGroup>
  <None Update="ApiApprovalTests.*.awexpect-snapshot.txt" DependentUpon="ApiApprovalTests.cs"/>
</ItemGroup>
```

To turn the nesting off, set the property `AweXpectSnapshotsNestFiles` to `false` in the test project:

```xml
<PropertyGroup>
  <AweXpectSnapshotsNestFiles>false</AweXpectSnapshotsNestFiles>
</PropertyGroup>
```
