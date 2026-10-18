# Objects, arrays and value kinds

Describes the possible expectations for the kind and the content of a `JsonElement` or a [`JsonNode`](#jsonnode).

| Expectation                           | Negated | Summary                                                 |
|---------------------------------------|---------|---------------------------------------------------------|
| [`IsObject`](#isobject)               |         | a JSON object, optionally with expectations on it       |
| [`IsArray`](#isarray)                 |         | a JSON array, optionally with expectations on it        |
| [`IsArray().WhoseElements`](#isarray) |         | collection expectations on the elements of a JSON array |
| [`IsString`](#value-kinds)            |         | a JSON string                                           |
| [`IsNumber`](#value-kinds)            |         | a JSON number                                           |
| [`IsBoolean`](#value-kinds)           |         | a JSON boolean, i.e. `true` or `false`                  |
| [`IsNull`](#value-kinds)              |         | the JSON value `null`, only for a `JsonElement`         |

## IsObject

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

## IsArray

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
[collection expectations](https://docs.testably.org/aweXpect/collections/items) of aweXpect, e.g. the
[quantifiers](https://docs.testably.org/aweXpect/collections/items#quantifiers) `All()`, `AtLeast(…)`, `AtMost(…)`,
`Exactly(…)`, `Between(…).And(…)` and `None()` with `ComplyWith(…)` or `Satisfy(…)`, as well as `Contains(…)`,
`HasCount(…)` and `IsEmpty()`:

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

:::warning
A `JsonElement` has no value equality, so value-based expectations on the elements, like `Contains(someElement)` or
`AreEqualTo(…)`, are not useful. Use expectations like `ComplyWith(e => e.Matches(…))`, `Contains([e => e.Matches(…)])`
or predicates like `Contains(e => …)` instead. A single lambda in `Contains(e => …)` is such a predicate on the
`JsonElement`, so pass expectations on an element as a collection.
:::

## Value kinds

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

:::note
For a nullable `JsonElement?`, `IsNull()` is the general expectation of aweXpect that verifies that the subject itself
is `null`, so `IsString()`, `IsNumber()` and `IsBoolean()` are available, but not a JSON-specific `IsNull()`.
:::

## `JsonNode`

A [`JsonNode`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonnode) and its subtypes
`JsonObject`, `JsonArray` and `JsonValue`, nullable or not, have the same expectations as a `JsonElement`:
[`Matches`, `MatchesExactly`](./02-matching.md), `IsObject`, `IsArray` (with `WhoseElements`), `IsString`, `IsNumber`
and `IsBoolean`, with the same options and failure messages:

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

:::note
The node is verified in the state it has when the expectation is awaited, so changes made after `Expect.That(…)` are
included.
:::

A `null` node is the JSON value `null`, as `JsonNode.Parse("null")` returns it. `Matches(null)` succeeds for it, while
`IsObject()` would fail with:

```text title="Failure message"
Expected that node
is an object,
but it was null
```

Use the general `IsNull()` expectation of aweXpect to verify that a node is the JSON value `null`, so there is no
JSON-specific `IsNull()` for a `JsonNode`.

## Newtonsoft.Json

There are no expectations for a `JToken` of Newtonsoft.Json. Convert it to a `JsonElement` through its JSON text to
use all expectations of a `JsonElement`:

```csharp
JObject token = JObject.Parse("""{"title": "Let It Be"}""");
JsonElement track = JsonDocument.Parse(token.ToString()).RootElement;

await Expect.That(track).Matches(new { title = "Let It Be" });
```

The values are compared as their JSON text, so e.g. a date token is a JSON string.
