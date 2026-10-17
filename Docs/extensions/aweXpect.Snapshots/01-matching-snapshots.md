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

The snapshot is the file `AlbumTests.Describes.snapshot.txt` in the directory of the test file: the first part is the
name of the test file without its extension, the second part the name of the test method. Both are determined at
compile time, so no reflection is needed and the expectation also works under Native AOT.

Before the first run the snapshot does not exist yet, so the expectation fails with:

```text title="Failure message"
Expected that description
matches the snapshot "/src/Tests/AlbumTests.Describes.snapshot.txt",
but the snapshot did not exist

Actual:
Abbey Road by The Beatles

Pending snapshot:
/src/Tests/AlbumTests.Describes.snapshot.pending.txt
```

If the snapshot contained "Abbey Road by the Beatles" instead, the expectation would fail with:

```text title="Failure message"
Expected that description
matches the snapshot "/src/Tests/AlbumTests.Describes.snapshot.txt",
but it was "Abbey Road by The Beatles", which differs at index 14:
            ↓ (actual)
  "…Road by The Beatles"
  "…Road by the Beatles"
            ↑ (expected)

Snapshot:
Abbey Road by the Beatles

Pending snapshot:
/src/Tests/AlbumTests.Describes.snapshot.pending.txt
```

A snapshot with several lines lists every line that differs.

## Objects

Any other value is written by the aweXpect formatter, completely and indented, so that a change in a single member
shows up as a changed line:

```csharp
await Expect.That(album).MatchesSnapshot();
```

```text title="AlbumTests.Loads.snapshot.txt"
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
- Maskings apply to the written text, e.g. `.Masking.Guids()` for a `Guid` member.

In a trimmed application or under Native AOT, the members of the subject's type are only known when the source
generator of aweXpect registers them for the call site, which it does for `MatchesSnapshot()`.

## Files

The snapshot files are encoded as UTF-8. The pending file ends with an additional line break, which is removed again
when the snapshot is read, so that an editor which adds a final line break does not change the snapshot.

The newline style is ignored when comparing, because version control can change it when the snapshot is checked out.

A snapshot that cannot be read, e.g. because another process locks it, fails the expectation with the reason. A pending
file that cannot be written is named as such in the failure, and a leftover pending file that cannot be deleted does not
fail a matching snapshot.

A snapshot cannot be negated: `DoesNotComplyWith(it => it.MatchesSnapshot())` always fails, as accepting the pending
snapshot would turn any success into a failure. Nothing is written for a negated snapshot, also not while accepting
changes.
