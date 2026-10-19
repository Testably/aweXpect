# aweXpect.Snapshots

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Snapshots)](https://www.nuget.org/packages/aweXpect.Snapshots)
[![Build](https://github.com/Testably/aweXpect/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect/actions/workflows/build.yml)

Snapshot testing for [aweXpect](https://github.com/Testably/aweXpect).

## At a glance

```csharp
// compare a string with the snapshot that is stored next to the test
await Expect.That(description).MatchesSnapshot();

// compare an object, written completely and indented by the aweXpect formatter
await Expect.That(album).MatchesSnapshot();

// compare an exception member-wise, without the members that differ between builds and machines, like the stack trace
await Expect.That(() => player.Play(album, -1)).Throws<ArgumentOutOfRangeException>().Which.MatchesSnapshot();

// replace values that change from run to run before comparing
await Expect.That(playlist).MatchesSnapshot().For.Text(@"plays=\d+").AsRegex().Mask("plays=<count>");

// replace every Guid and ISO 8601 date with a numbered placeholder ({Guid_1}, {Date_1}, …)
await Expect.That(playlist).MatchesSnapshot().For.Guids().Mask().For.Dates().Mask();

// leave out, mask or convert members of an object, selected by their name, type or declaring type
await Expect.That(album).MatchesSnapshot()
    .For.Members().WithName("PlayCount").Ignore()
    .For.Members().WithName("Id").Mask()
    .For.Members().DeclaredIn<Album>().WithName("Released").OfType<DateTime>().Convert(released => released.Year);

// store a JSON string as AlbumTests.Describes.awexpect-snapshot.json, so that editors and diff tools highlight it
await Expect.That(json).MatchesSnapshot().WithExtension("json");

// keep a snapshot per target framework and operating system (AlbumTests.Describes@net8.0@windows.awexpect-snapshot.txt)
await Expect.That(exception.Message).MatchesSnapshot().UniqueFor.TargetFramework().UniqueFor.OperatingSystem();

// verify the public API of an assembly, written as C# declarations
// (package and namespace aweXpect.Snapshots.PublicApi)
await Expect.That(typeof(Album).Assembly).MatchesPublicApiSnapshot().UniqueFor.TargetFramework();
```

The snapshot is stored as `<Class>.<Method>.awexpect-snapshot.txt` next to the test file, where `<Class>` is the name
of the test file. A missing or different snapshot fails the test and writes the actual value to
`<Class>.<Method>.awexpect-snapshot.pending.txt`; rename it to `<Class>.<Method>.awexpect-snapshot.txt` to accept it,
or set the environment variable `AWEXPECT_ACCEPT_SNAPSHOTS` to `all` to accept all changed snapshots of a test run (or
to `new` to only add the missing ones). Snapshots of version 0.1 are named `.snapshot.` instead, see
[Migrating from 0.1](../../Docs/extensions/aweXpect.Snapshots/03-naming-snapshots.md#migrating-from-01).
`OrphanedSnapshots.CheckAsync()` in the teardown of the test run reports the snapshots that no test uses any more on a
build server, and deletes them while accepting all changes.
[Comparers](../../Docs/extensions/aweXpect.Snapshots/08-comparers.md) decide whether a value matches its snapshot instead of the built-in
comparison, e.g. an image comparer that tolerates small differences of single pixels.
[Reporters](../../Docs/extensions/aweXpect.Snapshots/09-reporters.md) are told about every missing, different or matching snapshot, e.g. to open a
diff tool.

Reference [aweXpect.Snapshots.DiffEngine](https://www.nuget.org/packages/aweXpect.Snapshots.DiffEngine) as well, and a
missing or different snapshot opens a diff tool with [DiffEngine](https://github.com/VerifyTests/DiffEngine), in which
the change can be taken over into the snapshot; see [Diff tools](../../Docs/extensions/aweXpect.Snapshots/10-diff-tools.md).

## Overview

| Expectation                                                                      | Subject  | Summary                                                 |
|----------------------------------------------------------------------------------|----------|---------------------------------------------------------|
| [`MatchesSnapshot`](../../Docs/extensions/aweXpect.Snapshots/01-matching-snapshots.md#strings)              | `string` | matches the snapshot stored next to the test            |
| [`MatchesSnapshot`](../../Docs/extensions/aweXpect.Snapshots/01-matching-snapshots.md#objects) | any `T`  | matches the snapshot of the object, written by aweXpect |
| [`MatchesPublicApiSnapshot`](../../Docs/extensions/aweXpect.Snapshots/11-public-api.md) | `Assembly` | matches the snapshot of the public API of the assembly |

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Snapshots/).
