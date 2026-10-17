# aweXpect.Snapshots

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Snapshots)](https://www.nuget.org/packages/aweXpect.Snapshots)
[![Build](https://github.com/Testably/aweXpect.Snapshots/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Snapshots/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Snapshots&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Snapshots)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Snapshots&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Snapshots)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Snapshots%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Snapshots/main)

Snapshot testing for [aweXpect](https://github.com/Testably/aweXpect).

## At a glance

```csharp
// compare a string with the snapshot that is stored next to the test
await Expect.That(description).MatchesSnapshot();

// compare an object, written completely and indented by the aweXpect formatter
await Expect.That(album).MatchesSnapshot();

// replace values that change from run to run before comparing
await Expect.That(playlist).MatchesSnapshot().For.Text(@"plays=\d+").AsRegex().Mask("plays=<count>");

// replace every Guid and ISO 8601 date with a numbered placeholder ({Guid_1}, {Date_1}, …)
await Expect.That(playlist).MatchesSnapshot().For.Guids().Mask().For.Dates().Mask();

// leave out, mask or convert members of an object, selected by their name, type or declaring type
await Expect.That(album).MatchesSnapshot()
    .For.Members().WithName("PlayCount").Ignore()
    .For.Members().WithName("Id").Mask()
    .For.Members().DeclaredIn<Album>().WithName("Released").OfType<DateTime>().Convert(released => released.Year);

// store a JSON string as AlbumTests.Describes.snapshot.json, so that editors and diff tools highlight it
await Expect.That(json).MatchesSnapshot().WithExtension("json");

// keep a snapshot per target framework and operating system (AlbumTests.Describes@net8.0@windows.snapshot.txt)
await Expect.That(exception.Message).MatchesSnapshot().UniqueFor.TargetFramework().UniqueFor.OperatingSystem();
```

The snapshot is stored as `<Class>.<Method>.snapshot.txt` next to the test file, where `<Class>` is the name of the
test file. A missing or different snapshot fails the test and writes the actual value to
`<Class>.<Method>.snapshot.pending.txt`; rename it to `<Class>.<Method>.snapshot.txt` to accept it, or set the
environment variable `AWEXPECT_ACCEPT_SNAPSHOTS` to `all` to accept all changed snapshots of a test run (or to `new` to
only add the missing ones).
`OrphanedSnapshots.CheckAsync()` in the teardown of the test run reports the snapshots that no test uses any more on a
build server, and deletes them while accepting all changes.
[Reporters](../../Docs/extensions/aweXpect.Snapshots/08-reporters.md) are told about every missing, different or matching snapshot, e.g. to open a
diff tool.

## Overview

| Expectation                                                                      | Subject  | Summary                                                 |
|----------------------------------------------------------------------------------|----------|---------------------------------------------------------|
| [`MatchesSnapshot`](../../Docs/extensions/aweXpect.Snapshots/01-matching-snapshots.md#strings)              | `string` | matches the snapshot stored next to the test            |
| [`MatchesSnapshot`](../../Docs/extensions/aweXpect.Snapshots/01-matching-snapshots.md#objects) | any `T`  | matches the snapshot of the object, written by aweXpect |

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Snapshots/).
