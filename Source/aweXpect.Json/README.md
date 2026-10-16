# aweXpect.Json

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Json)](https://www.nuget.org/packages/aweXpect.Json)
[![Build](https://github.com/Testably/aweXpect.Json/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Json/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Json&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Json)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Json&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Json)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Json%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Json/main)

Expectations for JSON strings and `System.Text.Json` for [aweXpect](https://github.com/Testably/aweXpect).

## Overview

| Expectation                                             | Subject       | Summary                                                   |
|---------------------------------------------------------|---------------|-----------------------------------------------------------|
| [`IsEqualTo(…).AsJson()`](#string-comparison-as-json)   | `string`      | equal to the expected JSON, compared by structure         |
| [`IsValidJson`](#validation)                            | `string`      | can be parsed as JSON                                     |
| [`IsValidJsonMatching`](#validation)                    | `string`      | valid JSON that matches the expected value                |
| [`IsValidJsonMatchingExactly`](#validation)             | `string`      | valid JSON that matches the expected value exactly        |
| [`Matches`](#matches)                                   | `JsonElement` | matches the expected value, additional properties allowed |
| [`MatchesExactly`](#matches)                            | `JsonElement` | matches the expected value exactly                        |
| [`IsObject`](#isobject)                                 | `JsonElement` | a JSON object, optionally with expectations on it         |
| [`IsArray`](#isarray)                                   | `JsonElement` | a JSON array, optionally with expectations on it          |
| [`IsArray().WhoseElements`](#isarray)                   | `JsonElement` | collection expectations on the elements of a JSON array   |
| [`IsString`](#value-kinds)                              | `JsonElement` | a JSON string                                             |
| [`IsNumber`](#value-kinds)                              | `JsonElement` | a JSON number                                             |
| [`IsBoolean`](#value-kinds)                             | `JsonElement` | a JSON boolean, i.e. `true` or `false`                    |
| [`IsNull`](#value-kinds)                                | `JsonElement` | the JSON value `null`                                     |
| [`Matches`](#jsonnode)                                  | `JsonNode`    | matches the expected value, additional properties allowed |
| [`MatchesExactly`](#jsonnode)                           | `JsonNode`    | matches the expected value exactly                        |
| [`IsObject`](#jsonnode)                                 | `JsonNode`    | a JSON object, optionally with expectations on it         |
| [`IsArray`](#jsonnode)                                  | `JsonNode`    | a JSON array, optionally with expectations on it          |
| [`IsArray().WhoseElements`](#jsonnode)                  | `JsonNode`    | collection expectations on the elements of a JSON array   |
| [`IsString`](#jsonnode)                                 | `JsonNode`    | a JSON string                                             |
| [`IsNumber`](#jsonnode)                                 | `JsonNode`    | a JSON number                                             |
| [`IsBoolean`](#jsonnode)                                | `JsonNode`    | a JSON boolean, i.e. `true` or `false`                    |
| [`IsJsonSerializable`](#json-serializable)              | `object`      | survives a round trip through the JSON serializer         |

## String comparison as JSON

You can verify that a string is equal to the expected JSON, compared by its structure instead of its formatting:

```csharp
string track = """{"title":"Let It Be","artist":"The Beatles","durationSeconds":243}""";
string expected = """
                  {
                    "title": "Let It Be",
                    "artist": "The Beatles",
                    "durationSeconds": 243
                  }
                  """;

await Expect.That(track).IsEqualTo(expected).AsJson();
```

If the track had a duration of 241 seconds instead, the expectation would fail with:

```text title="Failure message"
Expected that track
is JSON equivalent to {
  "title": "Let It Be",
  "artist": "The Beatles",
  "durationSeconds": 243
},
but it differed as $.durationSeconds was 241 instead of 243

Actual:
{"title":"Let It Be","artist":"The Beatles","durationSeconds":241}

Expected:
{
  "title": "Let It Be",
  "artist": "The Beatles",
  "durationSeconds": 243
}
```

The expected string must be valid JSON, otherwise an `ArgumentException` is thrown. As JSON is compared by its
structure, `AsJson()` cannot be combined with `IgnoringCase()` or `Using(comparer)`.

`AsJson()` is not available on `Contains` or `DoesNotContain` for a string subject, as parts of a JSON string are
usually not valid JSON. Use `IsEqualTo(expected).AsJson()` or `IsValidJsonMatching(expected)` instead. On a collection
of strings, `Contains(expected).AsJson()` compares each item as JSON.

## Validation

You can verify that a string is valid JSON:

```csharp
string track = """{"title": "Let It Be"}""";

await Expect.That(track).IsValidJson();
```

If the closing brace was missing, the expectation would fail with:

```text title="Failure message"
Expected that track
is valid JSON,
but it could not be parsed: Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed. LineNumber: 0 | BytePositionInLine: 21.
```

This verifies that the string can be parsed by
[`JsonDocument.Parse`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocument.parse) without
exceptions. You can also specify the
[`JsonDocumentOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions):

```csharp
string track = """{"title": "Let It Be"}""";

await Expect.That(track).IsValidJson(o => o with { CommentHandling = JsonCommentHandling.Disallow });
```

You can add further expectations on the
[`JsonElement`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement) parsed from the subject:

```csharp
string track = """{"title": "Let It Be"}""";

await Expect.That(track).IsValidJson().Which(j => j.Matches(new { title = "Let It Be" }));
```

Or you can verify that the string is valid JSON and [matches](#matches) an expected value in one step:

```csharp
string track = """{"title": "Let It Be", "artist": "The Beatles"}""";

await Expect.That(track).IsValidJsonMatching(new { title = "Let It Be" });
await Expect.That(track).IsValidJsonMatchingExactly(new { title = "Let It Be", artist = "The Beatles" });
```

`IsValidJsonMatching(new { title = "Yesterday" })` would fail with:

```text title="Failure message"
Expected that track
is valid JSON which matches new { title = "Yesterday" },
but it differed as $.title was "Let It Be" instead of "Yesterday"

JSON options:
 - ignore additional properties
```

## `JsonElement`

### Matches

You can verify that a `JsonElement` matches an expected object. `Matches` ignores additional properties, while
`MatchesExactly` also fails for them:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""").RootElement;

await Expect.That(track).Matches(new { title = "Let It Be" });
await Expect.That(track).MatchesExactly(new { title = "Let It Be", artist = "The Beatles" });
```

`MatchesExactly(new { title = "Let It Be" })` would fail because of the additional `artist` property:

```text title="Failure message"
Expected that track
matches new { title = "Let It Be" } exactly,
but it differed as $.artist had unexpected "The Beatles"
```

By default, a property with the value `null` differs from a missing property. With `TreatingNullAsAbsent()`, a
property with the value `null` counts as absent, in both directions:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "album": null}""").RootElement;

await Expect.That(track).MatchesExactly(new { title = "Let It Be" },
    o => o.TreatingNullAsAbsent());
await Expect.That(track).MatchesExactly(new { title = "Let It Be", artist = (string?)null },
    o => o.TreatingNullAsAbsent());
```

Without the option, `MatchesExactly(new { title = "Let It Be" })` would fail with:

```text title="Failure message"
Expected that track
matches new { title = "Let It Be" } exactly,
but it differed as $.album had unexpected <null>
```

The option is available wherever `JsonOptions` are accepted, e.g. `IsEqualTo(expected).AsJson(o => o.TreatingNullAsAbsent())`,
`IsValidJsonMatching`, [`IsObject`](#isobject) and [`IsArray`](#isarray). Elements of an array with the value `null`
are not affected.

You can verify that a `JsonElement` matches an expected array:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday", "Hey Jude"]""").RootElement;

await Expect.That(titles).Matches(["Let It Be", "Yesterday", "Hey Jude"]);
await Expect.That(titles).MatchesExactly(["Let It Be", "Yesterday", "Hey Jude"]);
```

Unlike additional properties, additional array elements are not ignored by `Matches`.
`Matches(["Let It Be", "Yesterday"])` would fail with:

```text title="Failure message"
Expected that titles
matches ["Let It Be", "Yesterday"],
but it differed as $[2] had unexpected "Hey Jude"

JSON options:
 - ignore additional properties
```

With `IgnoringAdditionalArrayElements()`, the additional elements of all compared arrays, also nested ones, are
ignored. The option is available wherever `JsonOptions` are accepted, e.g.
`IsEqualTo(expected).AsJson(o => o.IgnoringAdditionalArrayElements())`:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday", "Hey Jude"]""").RootElement;

await Expect.That(titles).Matches(["Let It Be", "Yesterday"],
    o => o.IgnoringAdditionalArrayElements());
```

By default, arrays are compared by index. With `IgnoringArrayOrder()`, the elements of all compared arrays, also
nested ones, can be in any order. The option is available wherever `JsonOptions` are accepted, e.g.
`IsEqualTo(expected).AsJson(o => o.IgnoringArrayOrder())`:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday", "Hey Jude"]""").RootElement;

await Expect.That(titles).MatchesExactly(["Hey Jude", "Let It Be", "Yesterday"],
    o => o.IgnoringArrayOrder());
```

Each expected element is paired with the first matching element of the subject that is not paired yet. The expected
elements that could not be paired are reported as missing on the array, the remaining elements of the subject as
unexpected at their index. `MatchesExactly(["Hey Jude", "Something", "Help!"], o => o.IgnoringArrayOrder())` would
fail with:

```text title="Failure message"
Expected that titles
matches ["Hey Jude", "Something", "Help!"] exactly,
but it differed as
  $ had missing "Something" and
  $ had missing "Help!" and
  $[0] had unexpected "Let It Be" and
  $[1] had unexpected "Yesterday"

JSON options:
 - ignore array order
```

When exactly one element remains unpaired on each side, they are compared with each other instead, e.g.
`$[1] was "Yesterday" instead of "Something"`. Combined with `IgnoringAdditionalArrayElements()`, the expected
elements only have to be contained in the subject in any order. The pairing is greedy, so overlapping `It.Is…`
expectations can fail, although another pairing would succeed. The positional members of [`IsArray`](#isarray) (`At`,
`WithElements`, `WithArrays` and `WithObjects`) still verify the element at each index.

The failure message lists the active options in a "JSON options" context, like in the example above, and omits it when
no option is active. As `Matches`, `IsValidJsonMatching`, [`IsObject`](#isobject) and [`IsArray`](#isarray) ignore
additional properties by default, their failures list `ignore additional properties`.

Generated values, e.g. ids, timestamps or ETags, can be left out of the comparison with `IgnoringMember(name)` or
`Ignoring(path => …)`. An ignored value and everything below it is neither compared nor reported as missing or
unexpected. The options are available wherever `JsonOptions` are accepted, e.g.
`IsEqualTo(expected).AsJson(o => o.IgnoringMember("id"))`:

```csharp
JsonElement track = JsonDocument.Parse(
    """{"id": 42, "title": "Let It Be", "meta": {"createdAt": "2026-10-16", "etag": "abc"}}""").RootElement;

await Expect.That(track).MatchesExactly(new { id = 1, title = "Let It Be", meta = new { createdAt = "2026-01-01" } },
    o => o.IgnoringMember("id").IgnoringMember("createdAt").Ignoring(path => path.EndsWith(".etag")));
```

The name is matched against the JSON path, as it is shown in failure messages, and must cover whole trailing segments
of it, ignoring the casing: `"id"` ignores `$.id` and `$.items[0].id`, but not `$.parentId`, `"meta.createdAt"` only
ignores `createdAt` within `meta`, and `"$.id"` only ignores the property of the root. `"[0]"` ignores the first
element of every array; with `IgnoringArrayOrder()`, an index refers to the element of the subject, which is then left
out of the pairing. Explicit expectations of [`IsObject`](#isobject) and [`IsArray`](#isarray), e.g. `With("id")`,
`Without("id")` or `At(0)`, still verify an ignored value.

The members to ignore are listed in the "JSON options" context. When the comparison fails, each name that matched no
member is named below them, together with the member it most likely meant, as a misspelled name otherwise silently
ignores nothing. For a track `{"title": "Let It Be", "createdAt": "2026-10-16"}`,
`MatchesExactly(new { title = "Yesterday", createdAt = "2026-01-01" }, o => o.IgnoringMember("craetedAt"))` would fail
with:

```text title="Failure message"
Expected that track
matches new { title = "Yesterday", createdAt = "2026-01-01" } exactly,
but it differed as
  $.title was "Let It Be" instead of "Yesterday" and
  $.createdAt was "2026-10-16" instead of "2026-01-01"

JSON options:
 - ignore members: ["craetedAt"]
   "craetedAt" matched no member (did you mean "createdAt"?)
```

Calculated numbers rarely match an expected literal exactly. With `NumbersWithin(tolerance)`, a number matches when it
differs from the expected number by at most the tolerance, and with `NumbersWithin(tolerance).Percent()` by at most the
percentage of the expected number. The option is available wherever `JsonOptions` are accepted, e.g.
`IsEqualTo(expected).AsJson(o => o.NumbersWithin(0.001))`, and can be combined with the other options:

```csharp
JsonElement invoice = JsonDocument.Parse("""{"total": 0.30000000000000004, "tax": 0.0575}""").RootElement;

await Expect.That(invoice).MatchesExactly(new { total = 0.3, tax = 0.057m },
    o => o.NumbersWithin(0.001));
await Expect.That(invoice).MatchesExactly(new { tax = 0.057m },
    o => o.NumbersWithin(1).Percent().IgnoringAdditionalProperties());
```

The tolerance applies to every number in the compared JSON, also to integers such as ids or counts. Like in aweXpect,
a relative tolerance only matches an equal number when the expected number is `0`. The numbers are compared as
`double`, so numbers outside its range are compared by their text. For a product `{"name": "Guitar", "price": 110}`,
`MatchesExactly(new { name = "Guitar", price = 100 }, o => o.NumbersWithin(5).Percent())` would fail with:

```text title="Failure message"
Expected that product
matches new { name = "Guitar", price = 100 } exactly,
but it differed as $.price was 110 instead of 100 ± 5%

JSON options:
 - compare numbers within ± 5%
```

You can also verify that a `JsonElement` matches a primitive value:

```csharp
await Expect.That(JsonDocument.Parse("\"Let It Be\"").RootElement).Matches("Let It Be");
await Expect.That(JsonDocument.Parse("4.05").RootElement).Matches(4.05);
await Expect.That(JsonDocument.Parse("true").RootElement).Matches(true);
await Expect.That(JsonDocument.Parse("null").RootElement).Matches(null);
```

### IsObject

You can verify that a `JsonElement` is a JSON object that satisfies some expectations:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""").RootElement;

await Expect.That(track).IsObject(o => o
    .With("title").Matching("Let It Be").And
    .With("artist").Matching("The Beatles"));
```

With `.With("artist").Matching("The Rolling Stones")` instead, the expectation would fail with:

```text title="Failure message"
Expected that track
is an object and $.title matches "Let It Be" and $.artist matches "The Rolling Stones",
but it differed as $.artist was "The Beatles" instead of "The Rolling Stones"

JSON options:
 - ignore additional properties
```

You can verify that a property is another object, recursively:

```csharp
JsonElement track = JsonDocument.Parse(
    """{"title": "Let It Be", "album": {"title": "Let It Be", "year": 1970}}""").RootElement;

await Expect.That(track).IsObject(o => o
    .With("album").AnObject(album => album
        .With("year").Matching(1970)));
```

You can verify that a property is an array:

```csharp
JsonElement album = JsonDocument.Parse(
    """{"title": "Abbey Road", "tracks": ["Come Together", "Something"]}""").RootElement;

await Expect.That(album).IsObject(o => o
    .With("tracks").AnArray(tracks => tracks.WithElements("Come Together", "Something")));
```

You can address a nested value with a path relative to the object, instead of nesting `AnObject(…)` and
`AnArray(…)`. The path is a property name, followed by any number of `.name` or zero-based `[index]` segments, and
is written without a leading `$`. Wildcards, filters and recursive descent are not supported, and property names that
contain `.`, `[` or `]` can only be addressed with `With(…)`. The addressed value supports the same expectations as
`With(…)`:

```csharp
JsonElement album = JsonDocument.Parse(
    """{"artist": {"origin": {"city": "Liverpool"}}, "tracks": [{"title": "Come Together"}]}""").RootElement;

await Expect.That(album).IsObject(o => o
    .At("artist.origin.city").Matching("Liverpool").And
    .At("tracks[0].title").AString());
```

If the artist had no origin, the expectation would fail with the first segment that could not be reached:

```text title="Failure message"
Expected that album
is an object and $.artist.origin.city matches "Liverpool" and $.tracks[0].title is a string,
but it differed as property $.artist.origin did not exist

JSON options:
 - ignore additional properties
```

You can verify the kind of a property value without pinning the value itself with `AString()`, `ANumber()`,
`ABoolean()` (`true` or `false`) and `Null()`. They are also available for array elements, e.g. `.At(0).AString()`:

```csharp
JsonElement track = JsonDocument.Parse(
    """{"title": "Let It Be", "durationSeconds": 243, "explicit": false, "label": null}""").RootElement;

await Expect.That(track).IsObject(o => o
    .With("title").AString().And
    .With("durationSeconds").ANumber().And
    .With("explicit").ABoolean().And
    .With("label").Null());
```

If the duration was the string `"243"` instead, the expectation would fail with:

```text title="Failure message"
Expected that track
is an object and $.title is a string and $.durationSeconds is a number and $.explicit is a boolean and $.label is null,
but it differed as $.durationSeconds was a string

JSON options:
 - ignore additional properties
```

You can verify the number of properties in a JSON object:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""").RootElement;

await Expect.That(track).IsObject(o => o.With(2).Properties());
```

You can verify that a JSON object does not contain a property. A property with the value `null` exists and fails
the expectation, unless `TreatingNullAsAbsent()` is set:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""").RootElement;

await Expect.That(track).IsObject(o => o
    .With("title").Matching("Let It Be").And
    .Without("label"));
```

With `.Without("artist")` instead, the expectation would fail with:

```text title="Failure message"
Expected that track
is an object and $.title matches "Let It Be" and $.artist does not exist,
but it differed as $.artist was "The Beatles"

JSON options:
 - ignore additional properties
```

With `TreatingNullAsAbsent()`, a property with the value `null` counts as absent for the whole expectation:
`Without` accepts it, `With(…).Matching(null)` also accepts a missing property, every other expectation on the
property fails as if it did not exist, and `With(…).Properties()` does not count it:

```csharp
JsonElement user = JsonDocument.Parse("""{"name": "John", "passwordHash": null}""").RootElement;

await Expect.That(user).IsObject(o => o
    .With("name").Matching("John").And
    .Without("passwordHash"),
    o => o.TreatingNullAsAbsent());
```

With `.With("passwordHash").Matching("8f14e4")` instead of `.Without("passwordHash")`, the expectation would fail
with:

```text title="Failure message"
Expected that user
is an object and $.name matches "John" and $.passwordHash matches "8f14e4",
but it differed as property $.passwordHash did not exist

JSON options:
 - ignore additional properties
 - treat null as absent
```

### IsArray

You can verify that a `JsonElement` is a JSON array that satisfies some expectations:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday"]""").RootElement;

await Expect.That(titles).IsArray(a => a
    .At(0).Matching("Let It Be").And
    .At(1).Matching("Yesterday"));
```

You can verify the number of elements in a JSON array:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday"]""").RootElement;

await Expect.That(titles).IsArray(a => a.With(2).Elements());
```

You can match the expected elements of an array directly:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday"]""").RootElement;

await Expect.That(titles).IsArray(a => a.WithElements("Let It Be", "Yesterday"));
```

With `WithElements("Let It Be", "Hey Jude")` instead, the expectation would fail with:

```text title="Failure message"
Expected that titles
is an array and $[0] matches "Let It Be" and $[1] matches "Hey Jude",
but it differed as $[1] was "Yesterday" instead of "Hey Jude"

JSON options:
 - ignore additional properties
```

You can match nested arrays recursively (use `null` to skip an element):

```csharp
JsonElement sides = JsonDocument.Parse(
    """[["Come Together", "Something"], ["Here Comes the Sun"], ["The End", "Her Majesty"]]""").RootElement;

await Expect.That(sides).IsArray(a => a
    .WithArrays(
        side => side.WithElements("Come Together", "Something"),
        null,
        side => side.At(1).Matching("Her Majesty")));
```

You can match objects in an array recursively (use `null` to skip an element):

```csharp
JsonElement tracks = JsonDocument.Parse(
    """
    [
      {"title": "Let It Be"},
      {"title": "Yesterday"},
      {"title": "Hey Jude", "durationSeconds": 431}
    ]
    """).RootElement;

await Expect.That(tracks).IsArray(a => a
    .WithObjects(
        track => track.With("title").Matching("Let It Be"),
        null,
        track => track.With(2).Properties()));
```

You can verify the elements of an array regardless of their position with `WhoseElements`. It provides all
collection expectations of aweXpect, e.g. `All()`, `AtLeast(…)`, `AtMost(…)`, `Exactly(…)`, `Between(…).And(…)` and
`None()` with `ComplyWith(…)` or `Satisfy(…)`, as well as `Contains(…)`, `HasCount(…)` and `IsEmpty()`:

```csharp
JsonElement tracks = JsonDocument.Parse(
    """
    [
      {"title": "Let It Be", "artist": "The Beatles", "durationSeconds": 243},
      {"title": "Yesterday", "artist": "The Beatles", "durationSeconds": 125},
      {"title": "Hey Jude", "artist": "The Beatles", "durationSeconds": 431}
    ]
    """).RootElement;

await Expect.That(tracks).IsArray().WhoseElements
    .All().ComplyWith(t => t.Matches(new { artist = "The Beatles" }));

await Expect.That(tracks).IsArray().WhoseElements
    .AtLeast(2).ComplyWith(t => t.IsObject(o => o.With("durationSeconds").ANumber()));

await Expect.That(tracks).IsArray().WhoseElements
    .Contains([t => t.Matches(new { title = "Hey Jude" })]);

await Expect.That(tracks).IsArray().WhoseElements
    .Contains(t => t.GetProperty("durationSeconds").GetInt32() > 400);

await Expect.That(tracks).IsArray().WhoseElements.HasCount(3);
```

If "Yesterday" was by "Paul McCartney" instead, the first expectation would fail with:

```text title="Failure message"
Expected that tracks
is an array whose elements match new { artist = "The Beatles" } for all items,
but only 2 of 3 did

Not matching items (elements):
[
  {"title":"Yesterday","artist":"Paul McCartney","durationSeconds":125}
]

Collection (elements):
[
  {"title":"Let It Be","artist":"The Beatles","durationSeconds":243},
  {"title":"Yesterday","artist":"Paul McCartney","durationSeconds":125},
  {"title":"Hey Jude","artist":"The Beatles","durationSeconds":431}
]

JSON options (elements[1]):
 - ignore additional properties
```

A `JsonElement` has no value equality, so value-based expectations on the elements, like `Contains(someElement)` or
`AreEqualTo(…)`, are not useful. Use expectations like `ComplyWith(e => e.Matches(…))`, `Contains([e => e.Matches(…)])`
or predicates like `Contains(e => …)` instead. A single lambda in `Contains(e => …)` is such a predicate on the
`JsonElement`, so pass expectations on an element as a collection.

### Value kinds

You can verify that a `JsonElement` is a JSON string, number, boolean (`true` or `false`) or `null`:

```csharp
JsonElement title = JsonDocument.Parse("\"Let It Be\"").RootElement;

await Expect.That(title).IsString();
await Expect.That(JsonDocument.Parse("243").RootElement).IsNumber();
await Expect.That(JsonDocument.Parse("false").RootElement).IsBoolean();
await Expect.That(JsonDocument.Parse("null").RootElement).IsNull();
```

`IsNumber()` on the title would fail with:

```text title="Failure message"
Expected that title
is a number,
but it was a string
```

For a nullable `JsonElement?`, `IsNull()` is the general expectation of aweXpect that verifies that the subject itself
is `null`, so `IsString()`, `IsNumber()` and `IsBoolean()` are available, but not a JSON-specific `IsNull()`.

## `JsonNode`

A [`JsonNode`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonnode) and its subtypes
`JsonObject`, `JsonArray` and `JsonValue`, nullable or not, have the same expectations as a [`JsonElement`](#jsonelement):
`Matches`, `MatchesExactly`, `IsObject`, `IsArray` (with `WhoseElements`), `IsString`, `IsNumber` and `IsBoolean`, with
the same options and failure messages:

```csharp
JsonNode? track = JsonNode.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""");

await Expect.That(track).Matches(new { title = "Let It Be" });
await Expect.That(track).IsObject(o => o.With("artist").Matching("The Beatles"));

JsonArray titles = new("Let It Be", "Yesterday");

await Expect.That(titles).IsArray().WhoseElements.HasCount(2);
```

`MatchesExactly(new { title = "Let It Be" })` on the track would fail with:

```text title="Failure message"
Expected that track
matches new { title = "Let It Be" } exactly,
but it differed as $.artist had unexpected "The Beatles"
```

The node is verified in the state it has when the expectation is awaited, so changes made after `Expect.That(…)` are
included.

A `null` node is the JSON value `null`, as `JsonNode.Parse("null")` returns it. `Matches(null)` succeeds for it, while
`IsObject()` would fail with:

```text title="Failure message"
Expected that node
is an object,
but it was null
```

Use the general `IsNull()` expectation of aweXpect to verify that a node is the JSON value `null`, so there is no
JSON-specific `IsNull()` for a `JsonNode`.

## JSON serializable

You can verify that an object is JSON serializable:

```csharp
public record Track(string Title, string Artist);

Track track = new("Let It Be", "The Beatles");

await Expect.That(track).IsJsonSerializable();
```

This verifies that the object can be serialized to JSON and deserialized again, and that the result is equivalent to
the original object. For example, a property with a private setter loses its value:

```csharp
public class PlayCounter
{
    public string Title { get; set; } = "";
    public int PlayCount { get; private set; }
    public void Play() => PlayCount++;
}

PlayCounter counter = new() { Title = "Let It Be" };
counter.Play();

await Expect.That(counter).IsJsonSerializable();
```

```text title="Failure message"
Expected that counter
is serializable as JSON,
but it was not:
  Property PlayCount differed:
      Actual: 0
    Expected: 1
```

You can specify both the
[`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions) and the
equivalency options:

```csharp
await Expect.That(track).IsJsonSerializable(
    new JsonSerializerOptions { IncludeFields = true },
    e => e.IgnoringMember(nameof(Track.Artist)));
```

You can also specify the type that the subject is deserialized into:

```csharp
object track = new Track("Let It Be", "The Beatles");

await Expect.That(track).IsJsonSerializable<Track>(
    new JsonSerializerOptions { IncludeFields = true },
    e => e.IgnoringMember(nameof(Track.Artist)));
```

## Customization

You can change the default
[`JsonDocumentOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions), used to
parse JSON strings, and the default
[`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions), used to
format JSON values. Both allow trailing commas by default. Each value is set on its own and restored when the returned
lifetime is disposed:

```csharp
using (Customize.aweXpect.Json().DefaultJsonDocumentOptions.Set(new JsonDocumentOptions
       {
           AllowTrailingCommas = false,
       }))
{
    // parses JSON strings without allowing trailing commas
}
```

To change a value for all tests, set it on `Customize.aweXpect.Global.Json()` instead.
