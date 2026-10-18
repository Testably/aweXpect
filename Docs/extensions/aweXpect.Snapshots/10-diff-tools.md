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

With a [store of your own](07-storing-snapshots.md) that keeps no files, the value and the snapshot are written to
temporary files in `<temp>/aweXpect.Snapshots`, e.g. `Tests%2FAlbumTests.Describes.received.txt` and
`Tests%2FAlbumTests.Describes.snapshot.txt`, so that the change can be reviewed; edits in the diff tool are lost
there.

The diff tool is never opened on a build server, the same detection as for
[accepting changes](02-accepting-snapshots.md), nor when DiffEngine is turned off, e.g. with the environment variable
`DiffEngine_Disabled=true`; see the [DiffEngine documentation](https://github.com/VerifyTests/DiffEngine) for its
other options, e.g. the order of the diff tools.

## Registration

The package registers a [reporter](09-reporters.md) by adding a small file with a module initializer to the test
project. To register it yourself, e.g. only for some test projects, turn this off in the project file:

```xml
<PropertyGroup>
    <AweXpectSnapshotsDiffEngine>false</AweXpectSnapshotsDiffEngine>
</PropertyGroup>
```

A module initializer needs C# 9 and .NET 5 or later, so on .NET Framework and with an older language version the
reporter is not registered automatically. Register it in an assembly-level setup of the test framework instead, e.g.
in the constructor of an xunit fixture or a `[SetUpFixture]` of NUnit:

```csharp
Customize.aweXpect.Global.Snapshots().Reporters.Set([
    .. Customize.aweXpect.Global.Snapshots().Reporters.Get(),
    new DiffEngineReporter(),
]);
```
