# aweXpect.Snapshots.DiffEngine

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Snapshots.DiffEngine)](https://www.nuget.org/packages/aweXpect.Snapshots.DiffEngine)

[aweXpect.Snapshots.DiffEngine](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Snapshots.DiffEngine) opens a diff tool for a missing or different snapshot of [aweXpect.Snapshots](https://www.nuget.org/packages/aweXpect.Snapshots) with [DiffEngine](https://github.com/VerifyTests/DiffEngine), which finds the tools installed on the machine, e.g. Rider, Visual Studio, VS Code, Beyond Compare or WinMerge.

## Installation

```shell
dotnet add package aweXpect.Snapshots.DiffEngine
```

The package brings aweXpect.Snapshots with it and registers itself, so no further setup is needed.

## At a glance

The diff tool compares the pending file with the snapshot file, e.g. `AlbumTests.Describes.awexpect-snapshot.pending.txt` with `AlbumTests.Describes.awexpect-snapshot.txt`, so taking over a change in the diff tool changes the snapshot, and the next run of the test passes. Once the snapshot matches, the diff tool is closed again.

The diff tool is never opened on a build server, nor when DiffEngine is turned off, e.g. with the environment variable `DiffEngine_Disabled=true`.

To register the reporter yourself, e.g. only for some test projects, turn off the automatic registration in the project file:

```xml
<PropertyGroup>
    <AweXpectSnapshotsDiffEngine>false</AweXpectSnapshotsDiffEngine>
</PropertyGroup>
```

and call `DiffEngineReporter.Register()` from the namespace `aweXpect.Snapshots.DiffEngine` in an assembly-level setup of the test framework.

## Documentation

The full documentation is available at [docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Snapshots/diff-tools):

- [Diff tools](https://docs.testably.org/aweXpect/extensions/aweXpect.Snapshots/diff-tools): the files that are compared, stores without files and the registration
- [aweXpect.Snapshots](https://docs.testably.org/aweXpect/extensions/aweXpect.Snapshots/): matching, accepting and naming snapshots
