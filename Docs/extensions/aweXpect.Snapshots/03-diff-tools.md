# Diff tools

Reference the package [aweXpect.Snapshots.DiffEngine](https://www.nuget.org/packages/aweXpect.Snapshots.DiffEngine)
in the test project, and a missing or different snapshot opens a diff tool with
[DiffEngine](https://github.com/VerifyTests/DiffEngine), which finds the tools installed on the machine, e.g. Rider,
Visual Studio, VS Code, Beyond Compare or WinMerge:

```xml
<PackageReference Include="aweXpect.Snapshots.DiffEngine" Version="…"/>
```

The diff tool compares the pending file with the snapshot file, e.g.
`AlbumTests.Describes.awexpect-snapshot.pending.txt` with `AlbumTests.Describes.awexpect-snapshot.txt`, so taking over
a change in the diff tool changes the snapshot, and the next run of the test passes. DiffEngine creates an empty snapshot file for a snapshot that does not exist yet, as most diff
tools need two files. Once the snapshot matches, the diff tool is closed again.

With a [store of your own](advanced/01-storing-snapshots.md) that keeps no files, the value and the snapshot are written to
temporary files in `<temp>/aweXpect.Snapshots`, e.g. `Tests%2FAlbumTests.Describes.received.txt` and
`Tests%2FAlbumTests.Describes.snapshot.txt`, so that the change can be reviewed; edits in the diff tool are lost
there.

The diff tool is never opened on a build server, the same detection as for
[accepting changes](02-accepting-snapshots.md), nor when DiffEngine is turned off, e.g. with the environment variable
`DiffEngine_Disabled=true`; see the [DiffEngine documentation](https://github.com/VerifyTests/DiffEngine) for its
other options, e.g. the order of the diff tools.

## Registration

The package registers a [reporter](advanced/03-reporters.md) by calling `DiffEngineReporter.Register()` from a module
initializer that the source generator of aweXpect.Core adds to the test project. To register it yourself, e.g. only
for some test projects, turn this off in the project file:

```xml
<PropertyGroup>
    <AweXpectSnapshotsDiffEngine>false</AweXpectSnapshotsDiffEngine>
</PropertyGroup>
```

Then register it in an assembly-level setup of the test framework, e.g. in the constructor of an xunit fixture or a
`[SetUpFixture]` of NUnit:

```csharp
using aweXpect.Snapshots.DiffEngine;

DiffEngineReporter.Register();
```

`Register` adds the reporter to the reporters that are already set, unless one is added already.

<details>
<summary>Projects without module initializers</summary>

A module initializer needs C# 9 and the `ModuleInitializerAttribute`, which .NET 5 and later provide, and which a
project for .NET Framework can declare as an `internal` type itself. Without them, the reporter is not registered
automatically; register it yourself as shown above.

</details>
