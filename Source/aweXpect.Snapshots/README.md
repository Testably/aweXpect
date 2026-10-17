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
await Expect.That(playlist).MatchesSnapshot().Masking.Regex(@"plays=\d+", "plays=<count>");

// replace every Guid and ISO 8601 date with a numbered placeholder ({Guid_1}, {Date_1}, …)
await Expect.That(playlist).MatchesSnapshot().Masking.Guids().Masking.Dates();

// leave out or mask members of an object by their name or type
await Expect.That(album).MatchesSnapshot().Ignoring.Member("PlayCount").Masking.Member("Id");
```

The snapshot is stored as `<Class>.<Method>.snapshot.txt` next to the test file, where `<Class>` is the name of the
test file. A missing or different snapshot fails the test and writes the actual value to
`<Class>.<Method>.snapshot.pending.txt`; rename it to `<Class>.<Method>.snapshot.txt` to accept it, or set the
environment variable `AWEXPECT_ACCEPT_SNAPSHOTS` to accept all changed snapshots of a test run.
`OrphanedSnapshots.CheckAsync()` in the teardown of the test run reports the snapshots that no test uses any more on a
build server, and deletes them while accepting changes.

## Overview

| Expectation                                                                      | Subject  | Summary                                                 |
|----------------------------------------------------------------------------------|----------|---------------------------------------------------------|
| [`MatchesSnapshot`](../../Docs/extensions/aweXpect.Snapshots/01-matching-snapshots.md#strings)              | `string` | matches the snapshot stored next to the test            |
| [`MatchesSnapshot`](../../Docs/extensions/aweXpect.Snapshots/01-matching-snapshots.md#objects) | any `T`  | matches the snapshot of the object, written by aweXpect |

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Snapshots/).
