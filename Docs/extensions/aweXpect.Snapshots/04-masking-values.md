# Masking values

Values that change from run to run, such as ids or timestamps, can be replaced with a placeholder before the subject is
compared, so that the snapshot stays stable:

```csharp
await Expect.That(json).MatchesSnapshot()
    .Masking.Text(album.Id.ToString(), "<album-id>");
```

`.Masking.Text(…)` replaces literal text. `.Masking.Regex(…)` takes a regex pattern instead, and the replacement can
use the substitutions of `Regex.Replace`, e.g. `$1` for the first group, to keep part of the match (`$$` inserts a `$`):

```csharp
await Expect.That(log).MatchesSnapshot()
    .Masking.Regex(@"(\w+\.fm):\d+", "$1:<port>");   // "radio.fm:8000" → "radio.fm:<port>"
```

A pattern passed as a string, and the built-in maskings below, use the regex timeout of aweXpect,
`Customize.aweXpect.Settings().DefaultRegexTimeout` (one second by default). Options can be set inline, e.g. `(?i)`; for
a timeout of its own, pass a `Regex` instead:
`.Masking.Regex(new Regex(@"token=\w+", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5)), "token=<token>")`. A pattern
that times out fails the expectation, and an invalid pattern throws an `ArgumentException` where it is written.

The maskings are applied in the order in which they are written, so put a specific masking before a general one:

```csharp
await Expect.That(playlist).MatchesSnapshot()
    .Masking.Text(DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), "<today>")
    .Masking.Regex(@"\d{4}-\d{2}-\d{2}", "<date>");
```

In the other order, the general masking would already have replaced today's date with `<date>`.

The snapshot, the pending and the accepted snapshot and the failure contain the masked value, as it is what is compared.
Two values that only differ in a masked part count as the same value for the check of [named
snapshots](03-naming-snapshots.md).

## Guids and dates

`.Masking.Guids()` and `.Masking.Dates()` replace every `Guid` and date with a numbered placeholder in braces, so that it
stands out from the surrounding text. The same value keeps the same number, so relations stay visible in the snapshot:

```csharp
await Expect.That(log).MatchesSnapshot().Masking.Guids().Masking.Dates();
```

```text title="Snapshot"
album {Guid_1} added {Date_1} by {Guid_2}
album {Guid_1} played {Date_2}
```

- A `Guid` is matched in the format `"D"` (`3f2504e0-4f89-11d3-9a0c-0305e82c3301`) in upper or lower case, also inside
  braces or parentheses, which are kept. The format `"N"` without hyphens is not matched, as it cannot be told apart
  from a hash.
- A date is matched in ISO 8601: a date (`2026-10-17`) or a date with a time (`2026-10-17T09:30`), with optional seconds,
  a fraction of up to seven digits and an offset or `Z`. This covers how `DateTime`, `DateTimeOffset` and `DateOnly`
  write themselves with the round-trip format `"o"`, which the aweXpect formatter uses. Culture-specific formats, a space
  instead of the `T` and a time on its own are not matched; mask them with a regex.

The numbers are counted per subject in the order of appearance, and only after the maskings that come before, so
`.Masking.Text(album.Id.ToString(), "<album-id>").Masking.Guids()` numbers only the other `Guid`s.

## Members and types of an object

In a [snapshot of an object](01-matching-snapshots.md#objects), members can be left out or masked by their name or
their type, while the object is written, so that they don't depend on the written text:

```csharp
await Expect.That(album).MatchesSnapshot()
    .Ignoring.Member("PlayCount")                     // left out with its name
    .Ignoring.Member<DateTime>()                      // every DateTime member left out
    .Masking.Member("Id")                             // numbered: Id = {Id_1}
    .Masking.Member("Artist.StreamToken", "{token}")  // fixed replacement
    .Masking.Type<Guid>();                            // numbered: {Guid_1}
```

```text title="Snapshot"
Album {
  Artist = Artist {
    CatalogId = {Guid_1},
    Id = {Id_1},
    StreamToken = {token}
  },
  CatalogId = {Guid_2},
  Id = {Id_2},
  Title = "Abbey Road"
}
```

- A member name matches whole segments of the member path, ignoring the case, at any depth, like an ignored member of
  an equivalency comparison: `Id` matches `Id`, `Artist.Id` and `Tracks[0].Id`, but not `TrackId`; `Artist.Id` only
  matches the `Id` of an `Artist`.
- A type matches the declared or the runtime type of a value, including `Nullable<T>` and derived types:
  `.Ignoring.Member<DateTime>()` also leaves out a `DateTime?` member and a `DateTime` in an `object` member.
- `.Ignoring.Member<T>()` only leaves out members: the items of a collection, the values of a dictionary and the items
  of a tuple are kept, so that a `List<DateTime>` is not written as an empty list. `.Masking.Type<T>()` masks every value
  of the type, also these items, so use it for them, or leave out a single item by its path, e.g.
  `.Ignoring.Member("Releases[0]")`.
- A numbered placeholder is named after the last segment of the member name or after the type, and shares its numbers
  with the other maskings of that name, e.g. `.Masking.Type<Guid>()` with `.Masking.Guids()`. A `null` value is not
  masked.
- Ignoring wins over masking, the first masking that matches a value replaces it, and the rules of the expectation come
  before those for all snapshots. They apply before the maskings of the text.

A `string` is compared as raw text and has no members, so a member or type rule on a `string` subject throws an
`InvalidOperationException`.

## For all snapshots

Maskings that apply to every snapshot are set with `Customize.aweXpect.Snapshots().Maskings`, e.g. in an assembly-level
setup, from the factories of `SnapshotMasking` in the namespace `aweXpect.Snapshots`. They have the same names and
semantics as the maskings of an expectation:

```csharp
Customize.aweXpect.Global.Snapshots().Maskings.Set([
    SnapshotMasking.Guids(),
    SnapshotMasking.Dates(),
    SnapshotMasking.Text(Environment.MachineName, "<machine>"),
    SnapshotMasking.Regex(new Regex(@"localhost:\d+"), "localhost:<port>"),
]);
```

They are applied after the maskings of the expectation, in the order of the list, so a specific masking of the
expectation goes before a general one for all snapshots. Members and types are left out for all snapshots of objects
with `Customize.aweXpect.Snapshots().Ignorings` and the factories of `SnapshotIgnoring`, e.g.
`Ignorings.Set([SnapshotIgnoring.Member<DateTime>()])`. Member and type rules for all snapshots are ignored for a `string`
subject. Setting a value replaces the list; to extend an outer setting, e.g. for one async flow, include it:

```csharp
using (Customize.aweXpect.Snapshots().Maskings.Set([
           .. Customize.aweXpect.Snapshots().Maskings.Get(),
           SnapshotMasking.Text(library.Owner, "<owner>"),
       ]))
{
    await Expect.That(playlist).MatchesSnapshot();
}
```
