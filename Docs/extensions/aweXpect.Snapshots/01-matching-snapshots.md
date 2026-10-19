# Matching snapshots

## Strings

You can verify that a string matches the snapshot that is stored next to the test:

```csharp
public class AlbumTests
{
    [Fact]
    public async Task Describes()
    {
        string description = Describe(new Album("Abbey Road"));

        await Expect.That(description).MatchesSnapshot();
    }
}
```

The snapshot is the file `AlbumTests.Describes.awexpect-snapshot.txt` in the directory of the test file: the first part
is the name of the test file without its extension, the second part the name of the test method. Both are determined at
compile time, so no reflection is needed and the expectation also works under Native AOT. The marker
`.awexpect-snapshot.` keeps the snapshots apart from other files, e.g. those of other snapshot tools.

Before the first run the snapshot does not exist yet, so the expectation fails with:

```text title="Failure message"
Expected that description
matches the snapshot "/src/Tests/AlbumTests.Describes.awexpect-snapshot.txt",
but the snapshot did not exist

Actual:
Abbey Road by The Beatles

Pending snapshot:
/src/Tests/AlbumTests.Describes.awexpect-snapshot.pending.txt
```

If the snapshot contained "Abbey Road by the Beatles" instead, the expectation would fail with:

```text title="Failure message"
Expected that description
matches the snapshot "/src/Tests/AlbumTests.Describes.awexpect-snapshot.txt",
but it was "Abbey Road by The Beatles", which differs at index 14:
            ↓ (actual)
  "…Road by The Beatles"
  "…Road by the Beatles"
            ↑ (expected)

Snapshot:
Abbey Road by the Beatles

Pending snapshot:
/src/Tests/AlbumTests.Describes.awexpect-snapshot.pending.txt
```

A snapshot with several lines lists every line that differs.

### File extension

A string that is JSON, HTML, XML or code can be stored with a matching file extension, so that editors and diff tools
highlight it:

```csharp
await Expect.That(json).MatchesSnapshot().WithExtension("json");
```

The extension replaces `txt` in both files, e.g. `AlbumTests.Describes.awexpect-snapshot.json` and
`AlbumTests.Describes.awexpect-snapshot.pending.json`, also together with a name or an environment, e.g.
`AlbumTests.Describes.Abbey Road@net8.0.awexpect-snapshot.html` for
`.Named("Abbey Road").UniqueFor.TargetFramework().WithExtension("html")`. It is kept as given and passed without the
leading dot: it cannot be empty, contain a `.`, whitespace, control characters or any of `< > : " / \ | ? * % @`, or be
`pending`; otherwise `WithExtension` throws an `ArgumentException`. Objects are always stored as `.txt`, so
`WithExtension` is only offered for strings and [binary data](#binary-data-and-files).

A snapshot that was already used with another extension in the same test run fails, as the two files would otherwise
drift apart:

```text title="Failure message"
Expected that json
matches the snapshot "/src/Tests/AlbumTests.Describes.awexpect-snapshot.json",
but the snapshot was already used with the extension "txt" in this test run, so use one extension or name each one with .Named(…) or .NamedAfter(…)
```

## Objects

Any other value is written by the aweXpect formatter, completely and indented, so that a change in a single member
shows up as a changed line:

```csharp
await Expect.That(album).MatchesSnapshot();
```

```text title="AlbumTests.Loads.awexpect-snapshot.txt"
Album {
  Artist = "The Beatles",
  Genre = Rock,
  Note = <null>,
  Released = 1969-09-26T00:00:00.0000000,
  Title = "Abbey Road",
  Tracks = [
    Track {
      Number = 1,
      Title = "Come Together"
    },
    Track {
      Number = 2,
      Title = "Something"
    }
  ]
}
```

- Members are written in alphabetical order, and nothing is shortened like in a failure message.
- Numbers and dates are written independently of the current culture.
- An object that contains itself is written as `*recursive*` where it repeats, and an object that is nested too deeply
  for the stack fails the expectation instead of being shortened.
- The static type decides: a `string` is compared as raw text, but a string in an `object` subject is written in quotes
  like any other value.
- A collection is written in the order it enumerates its items, except for the hash-based collections whose order can
  change between runs: the items of a `HashSet<T>`, `ImmutableHashSet<T>`, `FrozenSet<T>` or `ConcurrentBag<T>` are
  sorted by their written text, and the entries of a `ConcurrentDictionary`, `ImmutableDictionary`, `FrozenDictionary`
  or `Hashtable` by their key. A `Dictionary<TKey, TValue>` keeps its insertion order.
- Text rules apply to the written text, e.g. `.For.Guids().Mask()` for a `Guid` member; member and value rules, e.g.
  `.For.Members().WithName("Id").Ignore()`, apply while the object is written, see [rules](04-masking-values.md).

In a trimmed application or under Native AOT, the members of the subject's type are only known when the source
generator of aweXpect registers them for the call site, which it does for `MatchesSnapshot()`.

### Exceptions

An exception is written member-wise as well, e.g. the one that a record player throws for a negative play count:

```csharp
public void Play(Album album, int playCount)
{
    if (playCount < 0)
    {
        throw new ArgumentOutOfRangeException(nameof(playCount), playCount, "Play count must be positive.")
        {
            Data = { ["Album"] = album.Title },
        };
    }
}
```

```csharp
await Expect.That(() => player.Play(album, -1)).Throws<ArgumentOutOfRangeException>().Which.MatchesSnapshot();
```

```text title="PlayerTests.Rejects.awexpect-snapshot.txt"
ArgumentOutOfRangeException {
  ActualValue = -1,
  Data = {
    ["Album"] = "Abbey Road"
  },
  Message = "Play count must be positive. (Parameter 'playCount')\nActual value was -1.",
  ParamName = "playCount"
}
```

- The `Message`, the `Data` when it is not empty, the `InnerException` when it is set, and the other public members of
  the type are written, sorted like the members of any other object. An inner exception is written member-wise, too,
  and an `AggregateException` writes its `InnerExceptions` instead of its `InnerException`, which only repeats the first
  of them.
- The `StackTrace`, `TargetSite`, `Source`, `HelpLink` and `HResult`, and the `FusionLog` of the exceptions that load
  assemblies, are left out, as they differ between builds, runtimes, machines and Native AOT.
- The message of a framework exception can differ between the runtimes, operating systems and cultures: the message
  above names the actual value on a line of its own, with the line break of the operating system, and .NET Framework
  words it differently. Leave out or mask such members with the [member rules](04-masking-values.md#members-and-values),
  or keep a snapshot per target framework and operating system with
  [`.UniqueFor`](03-naming-snapshots.md#snapshots-per-environment):

  ```csharp
  await Expect.That(() => player.Play(album, -1)).Throws<ArgumentOutOfRangeException>().Which.MatchesSnapshot()
      .For.Members().WithName("Message").Ignore()
      .For.Members().WithName("ActualValue").Mask();
  ```

- In a trimmed application or under Native AOT, the own members of a custom exception, e.g. a `Side` property of a
  `RecordPlayerException`, are left out: the source generator of aweXpect registers no type derived from `Exception`,
  because its `TargetSite` requires unreferenced code. The message, data and inner exceptions are still written, and so
  are the `ParamName`, `ActualValue`, `ObjectName` and `FileName` of the framework exceptions, which are read without
  reflection.
- A failure message still writes an exception as its type and message.

## Binary data and files

Binary data, e.g. the cover art an album service renders, is compared byte by byte. A `byte[]` or a `Stream` does not
tell which kind of file it is, so it needs the extension:

```csharp
await Expect.That(coverArt).MatchesSnapshot().WithExtension("png");
await Expect.That(linerNotesStream).MatchesSnapshot().WithExtension("pdf");
```

The snapshot is `AlbumTests.Cover.awexpect-snapshot.png` and a mismatch is written to
`AlbumTests.Cover.awexpect-snapshot.pending.png`, so both open in an image viewer. Without `.WithExtension(…)` the
expectation cannot be awaited, so a missing extension does not compile. A `Stream` is read from its current position
to the end; it does not need to be seekable and is neither reset nor disposed.

A `FileInfo` is stored with the extension of the file, here `pdf`:

```csharp
await Expect.That(new FileInfo("liner-notes.pdf")).MatchesSnapshot();
```

`.WithExtension(…)` sets another one, e.g. for a file without an extension like `LICENSE`, which fails otherwise, as
does an extension that a snapshot cannot have, e.g. `pending`. A file that does not exist fails like a `null` subject.

The failure never shows the bytes, but the first byte that differs, counted from 0, and the lengths:

```text title="Failure message"
Expected that coverArt
matches the snapshot "/src/Tests/AlbumTests.Cover.awexpect-snapshot.png",
but it differed at byte 1024 (the snapshot has 4096 bytes, the subject 4100)

Pending snapshot:
/src/Tests/AlbumTests.Cover.awexpect-snapshot.pending.png
```

- The bytes are stored unchanged: no encoding, no additional final line break, and the newline style is not ignored.
- Rules do not apply to binary data: `.For` is not offered, and the [rules for all snapshots](04-masking-values.md) are
  ignored.
- Names, `.UniqueFor`, accepting changes and the [check for orphaned snapshots](05-orphaned-snapshots.md) work as for
  any other snapshot.
- A `byte[]` is compared as binary data, not written by the formatter like other collections. As for a string, the
  static type decides, so a `byte[]` in an `object` subject is still written by the formatter.
- A [comparer](08-comparers.md) can compare it instead, e.g. tolerating small differences of single pixels.
- Version control must not change the line breaks in a binary snapshot, so mark its extension as binary in
  `.gitattributes`, e.g. `*.png binary`, unless Git already detects it as binary.
- A type derived from `Stream`, e.g. a `MemoryStream`, needs C# 13 or later to choose the overload for streams; before,
  `MatchesSnapshot()` throws an `InvalidOperationException` that asks to declare the subject as a `Stream`, e.g.
  `Expect.That<Stream>(stream)`.

## Files

The text of a string or an object is stored encoded as UTF-8. The pending file ends with an additional line break, which
is removed again when the snapshot is read, so that an editor which adds a final line break does not change the
snapshot.

The newline style of a text snapshot is ignored when comparing, because version control can change it when the snapshot
is checked out. A [comparer](08-comparers.md) can compare it in another way.

A snapshot that cannot be read, e.g. because another process locks it, fails the expectation with the reason. A pending
file that cannot be written is named as such in the failure, and a leftover pending file that cannot be deleted does not
fail a matching snapshot.

A snapshot cannot be negated: `DoesNotComplyWith(it => it.MatchesSnapshot())` always fails, as accepting the pending
snapshot would turn any success into a failure. Nothing is written for a negated snapshot, also not while accepting
changes.
