# Naming snapshots

The cases of a parameterized test share the name of the test method, so give each case its own snapshot with
`.Named(…)`:

```csharp
[Theory]
[InlineData("Abbey Road")]
[InlineData("Let It Be")]
public async Task Describes(string title)
{
    string description = Describe(new Album(title));

    await Expect.That(description).MatchesSnapshot().Named(title);
}
```

The name is appended to the file names, e.g. `AlbumTests.Describes.Abbey Road.awexpect-snapshot.txt`. The same
applies to several snapshots in one test method. Characters that a file name cannot hold on every operating system
(`< > : " / \ | ? *` and control characters), as well as `%` and `.`, are escaped as `%XX` in the file name, e.g.
`.Named("AC/DC")` is stored in `ArtistTests.Describes.AC%2FDC.awexpect-snapshot.txt`. The same holds for `@`, which
separates the environment (see below).

A snapshot that was already used for another value in the same test run fails, so that the cases cannot overwrite each
other's pending snapshot:

```text title="Failure message"
Expected that description
matches the snapshot "/src/Tests/AlbumTests.Describes.awexpect-snapshot.txt",
but the snapshot was already used for another value in this test run, so name each one with .Named(…) or .NamedAfter(…)
```

The same value again is allowed, so that a retried or repeated test still compares with its snapshot.

Names that only differ in case, e.g. `.Named("Live")` and `.Named("live")`, fail as well, also for the same value, as
they are the same file on Windows and macOS but two files on Linux:

```text title="Failure message"
Expected that description
matches the snapshot "/src/Tests/AlbumTests.Describes.live.awexpect-snapshot.txt",
but the snapshot "/src/Tests/AlbumTests.Describes.Live.awexpect-snapshot.txt", whose id only differs in case, was already used in this test run, so give them names that differ in more than the case with .Named(…) or .NamedAfter(…)
```

The test file and method are set by the compiler through optional parameters, so a name must not be passed as an
argument: `MatchesSnapshot("login")` throws an `ArgumentException` that points to `.Named(…)`.

## Naming after the arguments

`.NamedAfter(…)` names the snapshot after up to four arguments of the test. The compiler passes the name of each
argument, so it works with every test framework and under Native AOT:

```csharp
[Theory]
[InlineData("Abbey Road", 1969)]
[InlineData("Let It Be", 1970)]
public async Task Describes(string title, int year)
{
    string description = Describe(new Album(title, year));

    await Expect.That(description).MatchesSnapshot().NamedAfter(title, year);
}
```

Each argument is written as `<name>=<value>`, separated by `_`, e.g.
`AlbumTests.Describes.title=Abbey Road_year=1969.awexpect-snapshot.txt`. A member access is named like `nameof`, so
`.NamedAfter(album.Title)` gives `Title=Abbey Road`; any other expression is used as written. Values are written in
the invariant culture, `null` as `null`, and escaped like any other name.

## Snapshots per environment

Some output legitimately differs between environments, e.g. an exception message on .NET Framework and .NET, or a
path on Windows and Linux. Keep a snapshot for each of them with `.UniqueFor`, and combine several by repeating it:

```csharp
await Expect.That(exception.Message).MatchesSnapshot()
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

The target framework is the one the build knows (`$(TargetFramework)`), which the build targets of aweXpect.Snapshots
record in the test assembly, so `.UniqueFor.TargetFramework()` throws an `InvalidOperationException` when they are
missing. On another operating system, `.UniqueFor.OperatingSystem()` throws a `PlatformNotSupportedException`.

A [custom store](07-storing-snapshots.md) receives the environment as `SnapshotId.Variant`, e.g. `net8.0@windows`.

## Nesting in the IDE

In a C# project, the build targets of aweXpect.Snapshots nest each snapshot and pending file under its test file, so
that Rider and Visual Studio show them below it in the solution explorer. As the names of test files and snapshots can
both contain dots, the test file is the longest prefix of the file name, of up to eight parts separated by a dot, for
which a `.cs` file exists in the same directory, e.g. `AlbumTests.Describes.Abbey Road.awexpect-snapshot.txt` is nested
under `AlbumTests.Describes.cs` if it exists, and under `AlbumTests.cs` otherwise.

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
