# Masking and ignoring values

Values that change from run to run, such as ids or timestamps, and values that are irrelevant for the test can be
masked, converted or left out before the subject is compared, so that the snapshot stays stable. A rule starts with
`.For`, selects what it applies to, narrows it down with filters, and ends with exactly one action:

```csharp
await Expect.That(album).MatchesSnapshot()
    .For.Members().WithName("PlayCount").Ignore()                       // left out with its name
    .For.Members().WithName("Id").Mask()                                // numbered: Id = {Id_1}
    .For.Members().WithPath("Artist.StreamToken").Mask("{token}")       // fixed text
    .For.Values().OfType<Guid>().Mask()                                 // numbered: {Guid_1}
    .For.Members().DeclaredIn<Album>().WithName(nameof(Album.Released)).OfType<DateTime>()
        .Convert(released => released.Year);                            // Released = 1969

await Expect.That(log).MatchesSnapshot()
    .For.Lines().Containing("elapsed").IgnoringCase().Ignore()
    .For.Text(album.Id.ToString()).Mask("<album-id>")
    .For.Guids().Mask()
    .For.Dates().Mask();
```

## Overview

The selections and their filters:

| Selection                                                | Filters                                                                                 |
|----------------------------------------------------------|-----------------------------------------------------------------------------------------|
| `.For.Members()`                                         | `WithName`, `WithPath`, `OfType<T>`, `OfExactType<T>`, `DeclaredIn<T>`, `Which`, `Except` |
| `.For.Members().WhichAreEmptyCollections()`              | the filters of `Members()` before it                                                    |
| `.For.Values()`                                          | like `Members()`, without `WhichAreEmptyCollections`                                    |
| `.For.Lines()`                                           | `Containing(text)`, `Which`, `Except`                                                   |
| `.For.Text(text)`                                        | `IgnoringCase()`, `AsWildcard()`, `AsRegex()`                                           |
| `.For.Regex(regex)`, `.For.Guids()`, `.For.Dates()`      | –                                                                                       |
| `.For.MachineName()`, `.For.UserName()`, `.For.Paths()`  | –                                                                                       |

The actions that each selection offers:

| Selection                       | `Ignore()` | `Mask()`                         | `Mask("…")` | `Convert(…)`            | `Keep()` |
|---------------------------------|:----------:|----------------------------------|:-----------:|-------------------------|:--------:|
| `Members()`                     |     ✅      | `{Id_1}`, `{Guid_1}`             |      ✅      | ✅ typed by `OfType<T>`  |    –     |
| `WhichAreEmptyCollections()`    |     ✅      | –                                |      –      | –                       |    –     |
| `Values()`                      |     ✅      | `{Id_1}`, `{Guid_1}`, `{Item_1}` |      ✅      | ✅ typed by `OfType<T>`  |    –     |
| `Lines()`                       |     ✅      | –                                | ✅ the line  | ✅ `string → string?`    |    –     |
| `Text(text)`, `Regex(regex)`    |     –      | –                                |      ✅      | –                       |    –     |
| `Guids()`, `Dates()`            |     –      | `{Guid_1}`, `{Date_1}`           |      ✅      | –                       |    –     |
| `MachineName()`, `UserName()`   |     –      | `{MachineName}`, `{UserName}`    |      ✅      | –                       |    –     |
| `Paths()`                       |     –      | –                                |      –      | –                       |    ✅     |

A selection only offers the actions that fit it, so e.g. `.For.Paths().Mask()` or `.For.Text("…").Ignore()` does not
compile. The filters are combined, so a value has to match all of them. After the action, the next rule starts with
`.For` again, or the expectation continues with `.Named(…)`, `.UniqueFor` or `.AcceptingChanges()`.

`.For.Members()` and `.For.Values()` only apply to a [snapshot of an object](01-matching-snapshots.md#objects), and
`.For.Lines()` only to a snapshot of a `string`; written on the other kind of subject, the action throws an
`InvalidOperationException`. The text selections apply to every snapshot.

### Mask texts are templates

The text of `Mask("…")` is written as it is, without quotes, unless it contains `{0}`: then `{0}` is replaced with the
number of the value. Every value masked with the same template shares one counter, across all rules, and the same value
keeps the same number, so relations stay visible in the snapshot:

```csharp
await Expect.That(order).MatchesSnapshot()
    .For.Members().WithName("CustomerId").Mask("{Customer_{0}}");   // CustomerId = {Customer_1}
```

`Mask()` is the shorthand for the template named after the selection: `Mask("{Id_{0}}")` for a member named `Id`,
`Mask("{Guid_{0}}")` for `.For.Guids()` and `.For.Values().OfType<Guid>()`, `Mask("{Item_{0}}")` for an item of a
collection. That is why a `Guid` member masked by its type and the same `Guid` in a string get the same number.

`{0}` is replaced as plain text, not with `string.Format`, so braces need no escaping: `Mask("{{Customer_{0}}}")` writes
`{{Customer_1}}`, not `{Customer_1}`.

For members and values, the template can also depend on the value, e.g. to link ids that belong together:

```csharp
await Expect.That(shop).MatchesSnapshot()
    .For.Members().OfType<int>().WithName("Id").AsSuffix()
    .Mask(value => "{" + (value.Name == "Id"
        ? value.DeclaringType.Name
        : value.Name.Substring(0, value.Name.Length - 2)) + "_{0}}");
```

```text title="Snapshot"
Shop {
  Customers = [
    Customer {
      Id = {Customer_1},
      Name = "Ringo"
    }
  ],
  Orders = [
    Order {
      CustomerId = {Customer_1},
      Id = {Order_1}
    }
  ]
}
```

A value for which the function returns `null` is kept.

## Members and values

`.For.Members()` selects the members of the objects in the snapshot, at any depth. `.For.Values()` selects the members
too, and also the items of tuples, collections and dictionaries:

- `WithName("Id")` compares the name of the member itself, exactly and case-sensitive, so it matches `Id`, `Artist.Id`
  and `Tracks[0].Id`, but not `TrackId` or `id`. The options of aweXpect's string comparison that follow apply to the
  name only: `.IgnoringCase()`, `.AsPrefix()`, `.AsSuffix()`, `.AsWildcard()` or `.AsRegex()`, e.g.
  `.WithName("Url").AsSuffix()`. An invalid pattern throws an `ArgumentException` at the action. For a value, the name
  of a tuple item is `Item1`, and that of a collection item or dictionary value its index or key in brackets, e.g. `[0]`.
- `WithPath("Artist.Id")` matches the end of the path on whole segments, compared ordinally: it matches the `Id` of an
  `Artist` at any depth, e.g. `Albums[0].Artist.Id`, but not `Artist.CatalogId`. The path has the format of
  `FormattedValue.Path`, e.g. `Tracks[0].Id` or `Releases[Abbey Road]`. To match the whole path from the subject, use
  `.Which(value => value.Path == "Id")`.
- `OfType<T>()` matches the declared or the runtime type, including `Nullable<T>` and derived types:
  `.For.Members().OfType<DateTime>()` also matches a `DateTime?` member and a `DateTime` in an `object` member.
  `OfExactType<T>()` only matches a value whose runtime type is `T`, or a `null` value declared as `T` or `T?`.
- `DeclaredIn<T>()` matches the values in an object of the type `T` or of a derived type, e.g.
  `.For.Members().DeclaredIn<Album>().WithName(nameof(Album.Price))` for the price of each album, but not of its tracks.
- `Which(predicate)` and `Except(predicate)` decide for each value, which they get as a `FormattedValue` with its
  `Kind`, `Name`, `Path`, declared `Type` and `DeclaringType`; the value itself is only read when the predicate calls
  `GetValue()`. An exception of the predicate is thrown.

```csharp
await Expect.That(album).MatchesSnapshot()
    .For.Members().DeclaredIn<Album>().WithName(nameof(Album.Price)).Ignore()
    .For.Members().WithName("Url").AsSuffix().IgnoringCase().Ignore()
    .For.Members().DeclaredIn<Track>().WithName(nameof(Track.Id)).Mask()
    .For.Values().Which(value => value.Path.StartsWith("Artist.", StringComparison.Ordinal)).Mask("<artist>");
```

`Ignore()` leaves out a member together with its name. On `.For.Values()`, it also leaves out the items of tuples,
collections and dictionaries, so `.For.Values().OfType<DateTime>().Ignore()` writes a `List<DateTime>` as an empty
list. Select members only with `.For.Members()`, mask the items instead, or leave out a single item by its path, e.g.
`.For.Values().WithPath("Dates[0]").Ignore()`.

`Mask()` and `Mask("…")` replace the value, see [the templates](#mask-texts-are-templates). The placeholder of `Mask()`
is named after the type when the selection is filtered by `OfType<T>()` or `OfExactType<T>()` but not by
`WithName` or `WithPath`, e.g. `{Guid_1}`, and otherwise after the member or tuple item, or `Item` for an item of a collection or a dictionary value. A `null`
value is neither masked nor converted, as it does not change from run to run.

### Empty collections

Members that are often empty, e.g. the bonus tracks of an album, can be left out while they are empty:

```csharp
await Expect.That(album).MatchesSnapshot().For.Members().WhichAreEmptyCollections().Ignore();
```

```text title="Snapshot"
Album {
  Title = "Let It Be",
  Tracks = [
    "Two of Us",
    "Dig a Pony"
  ]
}
```

- It leaves out each member whose value is an empty array, list, set, dictionary or other collection that knows its
  count, here `BonusTracks = []`. The filters of `.For.Members()` before it narrow it down, e.g.
  `.For.Members().DeclaredIn<Album>().WhichAreEmptyCollections().Ignore()`.
- A `null` value and an empty `string` are kept, and so are empty items of a collection or values of a dictionary,
  which keep their place.
- A sequence that does not know its count, like a LINQ query or an iterator, is kept as well and only enumerated once
  to be written, as it might only be enumerable once.

### Converting values

A volatile value often has a meaningful and stable part, e.g. the year of a release date or the rounded length of a
track. `Convert(…)` replaces the value with the result of the converter:

```csharp
await Expect.That(album).MatchesSnapshot()
    .For.Members().DeclaredIn<Album>().WithName(nameof(Album.Released)).OfType<DateTime>()
        .Convert(released => released.Year)
    .For.Values().OfType<TimeSpan>().Convert(duration => Math.Round(duration.TotalMinutes));
```

```text title="Snapshot"
Album {
  Released = 1969,
  Title = "Abbey Road",
  Tracks = [
    Track {
      Duration = 4.0,
      Title = "Come Together"
    }
  ]
}
```

- `OfType<T>()` types the selection, so the converter gets a `T` and its result type is inferred. Without it, the
  converter gets an `object`.
- The converted value is written by the formatter like any other value, e.g. a `string` in quotes, a number in the
  invariant culture and an object member-wise. The member and value rules do not apply to the values nested in it, but
  the text rules do. A converted `null` is written as `<null>`.
- A `null` value, or one that is no `T`, is not converted, and an exception of the converter is thrown.
- Under Native AOT, the source generator of aweXpect registers the members of the result type, so that e.g. an
  anonymous type is also written member-wise.

## Lines of a string

Lines that are only noise, such as durations, timestamps or banners, can be left out of a `string` snapshot, or replaced
as a whole:

```csharp
await Expect.That(log).MatchesSnapshot()
    .For.Lines().Containing("elapsed").IgnoringCase().Ignore()
    .For.Lines().Containing("Track ?: *Live*").AsWildcard().Ignore()
    .For.Lines().Containing(@"^\d{2}:\d{2} ").AsRegex().Ignore()
    .For.Lines().Which(line => line.Length == 0).Ignore()
    .For.Lines().Containing("Now playing").Mask("Now playing: <track>")
    .For.Lines().Containing("Volume").Convert(line => line.Substring(0, 6));
```

- `Containing(text)` selects each line that contains the text anywhere, compared ordinally like aweXpect's `Contains`.
  `.IgnoringCase()`, `.AsWildcard()` and `.AsRegex()` directly after it apply to this filter only, not to the comparison
  with the snapshot. A pattern uses the regex timeout of aweXpect; a pattern that times out fails the expectation, and an
  invalid pattern throws an `ArgumentException` at the action.
- `Which(predicate)` and `Except(predicate)` decide for each line.
- `Ignore()` leaves out the line, `Mask("…")` replaces the whole line with a [template](#mask-texts-are-templates), and
  `Convert(…)` with the text that the converter returns for it, or leaves it out for `null`.
- A line ends with `\r\n`, `\n` or `\r` and is checked without it. A replaced line keeps its line break. A removed line
  takes its line break with it, and removing the last line also removes the line break before it; the other lines keep
  their newline style.
- The lines are handled before the text rules, so a removed line does not use up a numbered placeholder like
  `{Guid_1}`.

An object is written by the formatter, so a line rule on any other subject than a `string` throws an
`InvalidOperationException`; select its members instead.

## Text

`.For.Text(…)` selects literal text in the written snapshot, e.g. a value known at test time, and `Mask("…")`
replaces it:

```csharp
await Expect.That(json).MatchesSnapshot()
    .For.Text(album.Id.ToString()).Mask("<album-id>")
    .For.Text("abbey road").IgnoringCase().Mask("<title>")
    .For.Text("Track ?:*").AsWildcard().Mask("<track>")
    .For.Text(@"(\w+\.fm):\d+").AsRegex().Mask("$1:<port>");   // "radio.fm:8000" → "radio.fm:<port>"
```

- The text is compared ordinally, unless `.IgnoringCase()` follows.
- `.AsWildcard()` interprets it as a wildcard pattern, where `*` matches zero or more characters and `?` exactly one
  character. Unlike in aweXpect's string comparison, a wildcard does not match a line break, so that `*` cannot mask the
  rest of the snapshot.
- `.AsRegex()` interprets it as a regex pattern, and the mask can use the substitutions of `Regex.Replace`, e.g. `$1` for
  the first group (`$$` inserts a `$`). The substitutions are applied first, then `{0}`, so `Mask("$1:{0}")` counts per
  value of the group. Any other mask is inserted literally.
- A pattern uses the regex timeout of aweXpect, `Customize.aweXpect.Settings().DefaultRegexTimeout` (one second by
  default); a pattern that times out fails the expectation, and an invalid pattern throws an `ArgumentException` at the
  action. An empty mask removes the text.

`.For.Regex(regex)` takes a `Regex` instead, for `RegexOptions`, a timeout of its own or a reused `static` or
`[GeneratedRegex]` instance, e.g.
`.For.Regex(new Regex(@"token=\w+", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5))).Mask("token=<token>")`.

The text rules are applied in the order in which they are written, so put a specific rule before a general one:

```csharp
await Expect.That(playlist).MatchesSnapshot()
    .For.Text(DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)).Mask("<today>")
    .For.Text(@"\d{4}-\d{2}-\d{2}").AsRegex().Mask("<date>");
```

In the other order, the general rule would already have replaced today's date with `<date>`.

The snapshot, the pending and the accepted snapshot and the failure contain the masked value, as it is what is compared.
Two values that only differ in a masked part count as the same value for the check of [named
snapshots](03-naming-snapshots.md).

## Guids and dates

`.For.Guids().Mask()` and `.For.Dates().Mask()` replace every `Guid` and date with a numbered placeholder in braces, so
that it stands out from the surrounding text. The same value keeps the same number:

```csharp
await Expect.That(log).MatchesSnapshot().For.Guids().Mask().For.Dates().Mask();
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
  instead of the `T` and a time on its own are not matched; select them with a regex.
- `Mask("…")` replaces them with a [template](#mask-texts-are-templates) instead, e.g. `Mask("<guid>")`.

The numbers are counted per subject in the order of appearance, and only after the rules that come before, so
`.For.Text(album.Id.ToString()).Mask("<album-id>").For.Guids().Mask()` numbers only the other `Guid`s.

## Environment

Paths of the environment differ between machines, so they are masked by default:

```text
// value                                               // snapshot
Loaded C:\Music\Discography\Tests\albums.json          Loaded {ProjectDirectory}\albums.json
Cache: C:\Users\paul\AppData\Local\Temp\abbey.tmp      Cache: {TempPath}\abbey.tmp
```

| Placeholder          | Path                                                                                     |
|----------------------|------------------------------------------------------------------------------------------|
| `{ProjectDirectory}` | the directory of the test project that contains the test file                            |
| `{SourceRoot}`       | the source root, see [deterministic builds](06-deterministic-builds.md)                  |
| `{OutputDirectory}`  | `AppContext.BaseDirectory`                                                               |
| `{TempPath}`         | `Path.GetTempPath()`                                                                     |
| `{UserProfile}`      | `Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)`, on every system      |

- The longest path is masked first, as they are nested in each other, e.g. the output directory in the project directory
  in the source root.
- On Windows, a path matches with `\` or `/` as separator, also with escaped backslashes (`C:\\Music`), as in JSON or in
  a string of a [snapshot of an object](01-matching-snapshots.md#objects); elsewhere only with `/`, as a backslash is part
  of a file name there. The rest of the path keeps its separators.
- Only whole segments match, so `C:\Music\Album` does not match inside `C:\Music\Albums`.
- The case is ignored on Windows and macOS, but not on Linux.
- A root of the file system, such as `C:\` or `/`, is never masked.

The paths are masked before all other text rules, so these see the placeholders. Keep them for one expectation with
`.For.Paths().Keep()`, or for all snapshots with `SnapshotRule.For.Paths().Keep()` in the
[rules for all snapshots](#for-all-snapshots). An expectation cannot mask them again while a rule for all snapshots keeps
them; set the rules without it in a scope instead.

The name of the machine and of the user are only masked when asked for, as whole words, with `{MachineName}` and
`{UserName}`. The machine name ignores the case, the user name only on Windows and macOS. They are masked right after
the paths, also by a rule for all snapshots, so a path in the user profile becomes `{UserProfile}`:

```csharp
await Expect.That(log).MatchesSnapshot().For.MachineName().Mask().For.UserName().Mask();
```

## For all snapshots

Rules that apply to every snapshot are set with `Customize.aweXpect.Snapshots().Rules`, e.g. in an assembly-level setup.
They start with `SnapshotRule.For` from the namespace `aweXpect.Snapshots` and offer the same selections, filters and
actions as the rules of an expectation:

```csharp
Customize.aweXpect.Global.Snapshots().Rules.Set([
    SnapshotRule.For.Guids().Mask(),
    SnapshotRule.For.Dates().Mask(),
    SnapshotRule.For.MachineName().Mask(),
    SnapshotRule.For.Regex(new Regex(@"localhost:\d+")).Mask("localhost:<port>"),
    SnapshotRule.For.Members().WhichAreEmptyCollections().Ignore(),
    SnapshotRule.For.Values().OfType<DateTime>().Convert(date => date.Year),
    SnapshotRule.For.Lines().Containing("Build date").IgnoringCase().Ignore(),
    SnapshotRule.For.Paths().Keep(),
]);
```

They are applied after the rules of the expectation, in the order of the list, so a specific rule of the expectation
goes before a general one for all snapshots. Member and value rules for all snapshots are ignored for a `string`
subject, and line rules for any other subject. Setting a value replaces the list; to extend an outer setting, e.g. for
one async flow, include it:

```csharp
using (Customize.aweXpect.Snapshots().Rules.Set([
           .. Customize.aweXpect.Snapshots().Rules.Get(),
           SnapshotRule.For.Text(library.Owner).Mask("<owner>"),
       ]))
{
    await Expect.That(playlist).MatchesSnapshot();
}
```

## Order

The rules are applied in this order:

1. The member and value rules, while an object is written: a rule that leaves a value out wins over every masking and
   converter, then the first masking or converter that matches the value replaces it, the rules of the expectation
   before those for all snapshots.
2. The line rules of a `string` subject: a rule that leaves a line out wins, then the first matching rule replaces it.
3. The paths of the environment, unless kept, then the machine and the user name.
4. The other text rules in the order in which they are written, those of the expectation before those for all
   snapshots.

Numbered placeholders share their counters across all these steps, so e.g. `.For.Values().OfType<Guid>().Mask()` and
`.For.Guids().Mask()` give the same `Guid` the same number.
