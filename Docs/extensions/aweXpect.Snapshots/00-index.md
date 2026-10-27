# aweXpect.Snapshots

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Snapshots)](https://www.nuget.org/packages/aweXpect.Snapshots)

[aweXpect.Snapshots](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Snapshots) compares strings and objects with reviewed
snapshots, which are stored next to the tests or in a store of your own, e.g. to verify generated text or a complex
result without writing out the expected value.

## Installation

```ps
dotnet add package aweXpect.Snapshots
```

- [aweXpect.Snapshots](https://www.nuget.org/packages/aweXpect.Snapshots): `MatchesSnapshot` for strings, objects,
  binary data and files. The other two packages bring it with them.
- [aweXpect.Snapshots.DiffEngine](https://www.nuget.org/packages/aweXpect.Snapshots.DiffEngine): add it to open a diff
  tool for a missing or different snapshot when the tests run locally; it registers itself, see
  [Diff tools](./03-diff-tools.md).
- [aweXpect.Snapshots.PublicApi](https://www.nuget.org/packages/aweXpect.Snapshots.PublicApi): add it to verify the
  public API of an assembly with `MatchesPublicApiSnapshot`, see [Public API](./07-public-api.md).

The packages bring `Expect.That` with them, but not the expectations for strings, numbers or collections, so reference
[`aweXpect`](https://www.nuget.org/packages/aweXpect) as well. `MatchesSnapshot` and `MatchesPublicApiSnapshot` live in
the namespace `aweXpect`; `OrphanedSnapshots`, `SnapshotAcceptance`, `SnapshotRule` and the
[customization](#customization) need `using aweXpect.Snapshots;`.

## At a glance

```csharp
// compare a string with the snapshot that is stored next to the test
await Expect.That(description).MatchesSnapshot();

// compare an object, written completely and indented by the aweXpect formatter
await Expect.That(album).MatchesSnapshot();

// compare an exception member-wise, without members that vary per build or machine, like the stack trace
await Expect.That(() => player.Play(album, -1)).Throws<ArgumentOutOfRangeException>().Which.MatchesSnapshot();

// replace values that change from run to run before comparing
await Expect.That(playlist).MatchesSnapshot().For.Text(@"plays=\d+").AsRegex().Mask("plays=<count>");

// replace every Guid and ISO 8601 date with a numbered placeholder ({Guid_1}, {Date_1}, …)
await Expect.That(playlist).MatchesSnapshot().For.Guids().Mask().For.Dates().Mask();

// leave out, mask or convert members of an object, selected by their name, type or declaring type
await Expect.That(album).MatchesSnapshot()
    .For.Members().WithName("PlayCount").Ignore()
    .For.Members().WithName("Id").Mask()
    .For.Members().DeclaredIn<Album>().WithName("Released").OfType<DateTime>().Convert(date => date.Year);

// store a JSON string as AlbumTests.Describes.awexpect-snapshot.json for syntax highlighting
await Expect.That(json).MatchesSnapshot().WithExtension("json");

// keep a snapshot per target framework and operating system
// (AlbumTests.Describes@net8.0@windows.awexpect-snapshot.txt)
await Expect.That(exception.Message).MatchesSnapshot()
    .UniqueFor.TargetFramework().UniqueFor.OperatingSystem();

// verify the public API of an assembly, written as C# declarations
// (package aweXpect.Snapshots.PublicApi)
await Expect.That(typeof(Album).Assembly).MatchesPublicApiSnapshot()
    .UniqueFor.TargetFramework();
```

The snapshot is stored as `<TestFile>.<Method>.awexpect-snapshot.txt` next to the test file, where `<TestFile>` is the
name of the test file without its extension. A missing or different snapshot fails the test and writes the actual
value to `<TestFile>.<Method>.awexpect-snapshot.pending.txt`; rename it to `<TestFile>.<Method>.awexpect-snapshot.txt`
to accept it, or set the environment variable `AWEXPECT_ACCEPT_SNAPSHOTS` to `all` to accept all changed snapshots of a
test run (or to `new` to only add the missing ones).

Before the first run, the snapshot of `description` does not exist yet, so the expectation fails with:

```text title="Failure message"
Expected that description
matches the snapshot "/src/Tests/AlbumTests.Describes.awexpect-snapshot.txt",
but the snapshot did not exist

Actual:
Abbey Road by The Beatles

Pending snapshot:
/src/Tests/AlbumTests.Describes.awexpect-snapshot.pending.txt
```

## Overview

Expectations for `string` and any other object:

| Expectation                                             | Negated | Summary                                                                                  |
|---------------------------------------------------------|---------|------------------------------------------------------------------------------------------|
| [`MatchesSnapshot`](./01-matching-snapshots.md#strings) |         | a string matches the snapshot stored next to the test, with rules for its lines and text |
| [`MatchesSnapshot`](./01-matching-snapshots.md#objects) |         | an object matches its snapshot, written by aweXpect, with rules for its members and text |

Expectations for `Assembly` and `FileInfo`, from aweXpect.Snapshots.PublicApi:

| Expectation                                      | Negated | Summary                                                                              |
|--------------------------------------------------|---------|--------------------------------------------------------------------------------------|
| [`MatchesPublicApiSnapshot`](./07-public-api.md) |         | the public API of the assembly, or of the assembly in the file, matches the snapshot |

## Customization

Set a value under `Customize.aweXpect.Snapshots()` for the current
[async flow](https://docs.testably.org/aweXpect/how-it-works/configuration#lifetimes-and-async-flows), or under
`Customize.aweXpect.Global.Snapshots()` for
[all tests](https://docs.testably.org/aweXpect/how-it-works/configuration#global-defaults).

| Value                                                                            | Default                                                  | Summary                                                      |
|----------------------------------------------------------------------------------|----------------------------------------------------------|--------------------------------------------------------------|
| [`Acceptance`](./02-accepting-snapshots.md)                                      | `null`, left to the variable `AWEXPECT_ACCEPT_SNAPSHOTS` | which missing or changed snapshots are accepted              |
| [`Rules`](./05-masking-values.md#for-all-snapshots)                              | none                                                     | the rules to mask or ignore values in every snapshot         |
| [`Store`](./advanced/01-storing-snapshots.md)                                    | a file next to the test file                             | where the snapshots are kept                                 |
| [`Comparers`](./advanced/02-comparers.md)                                        | none                                                     | decide whether a value matches its snapshot, e.g. for images |
| [`Reporters`](./advanced/03-reporters.md)                                        | none                                                     | are told about every missing, different or matching snapshot |
| [`NamedAfterTestArguments`](./04-naming-snapshots.md#naming-after-the-test-case) | `false`                                                  | names each snapshot after the arguments of the test case     |
| [`AttachPendingFiles`](./02-accepting-snapshots.md#on-a-build-server)            | `false`                                                  | attaches the pending file of a mismatch to the test result   |

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Snapshots/):

- [Matching snapshots](./01-matching-snapshots.md): strings, objects, exceptions, binary data and files, the file
  extension, encoding and line breaks
- [Accepting snapshots](./02-accepting-snapshots.md): reviewing the pending file, accepting all changes of a run and
  attaching the pending files on a build server
- [Diff tools](./03-diff-tools.md): open a diff tool for a missing or different snapshot with
  [DiffEngine](https://github.com/VerifyTests/DiffEngine), in which the change can be taken over into the snapshot
- [Naming snapshots](./04-naming-snapshots.md): a snapshot per test case or environment, and
  [nested under their test file](./04-naming-snapshots.md#nesting-in-the-ide) in Rider and Visual Studio
- [Masking and ignoring values](./05-masking-values.md): mask, convert or leave out values that change from run to run
- [Orphaned snapshots](./06-orphaned-snapshots.md): `OrphanedSnapshots.CheckAsync()` in the teardown of the test run
  reports the snapshots that no test uses any more on a build server, and deletes them while accepting all changes
- [Public API](./07-public-api.md): `MatchesPublicApiSnapshot` from aweXpect.Snapshots.PublicApi
- Advanced:
  - [Storing snapshots elsewhere](./advanced/01-storing-snapshots.md): a `SnapshotStore` instead of files next to the
    tests, e.g. a shared folder or a database
  - [Comparers](./advanced/02-comparers.md): decide whether a value matches its snapshot instead of the built-in
    comparison, e.g. an image comparer that tolerates small differences of single pixels
  - [Reporters](./advanced/03-reporters.md): are told about every missing, different or matching snapshot, e.g. to
    open a diff tool
  - [Deterministic builds](./advanced/04-deterministic-builds.md): how the snapshots are found when the build replaces
    the path of the repository with `/_/`
