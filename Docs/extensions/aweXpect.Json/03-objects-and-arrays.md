---
description: Verify the properties, values at a path, array elements and value kinds of a JsonElement or JsonNode.
---

# Objects, arrays and value kinds

Describes the possible expectations for the kind and the content of a `JsonElement` or a [`JsonNode`](./04-json-nodes.md).

| Expectation                                         | Negated               | Summary                                                          |
|-----------------------------------------------------|-----------------------|------------------------------------------------------------------|
| [`HasProperty`](#properties)                        | `DoesNotHaveProperty` | a JSON object with the property, optionally with expectations    |
| [`HasValueAt`](#values-at-a-path)                   | `DoesNotHaveValueAt`  | a value at a path, optionally with expectations                  |
| [`HasElementAt`](#elements)                         |                       | a JSON array with an element at the index, optionally with expectations |
| [`IsObject`](#isobject)                             |                       | a JSON object                                                    |
| [`IsObject().WhoseProperties`](#isobject)           |                       | collection expectations on the properties of a JSON object       |
| [`IsArray`](#isarray)                               |                       | a JSON array                                                     |
| [`IsArray().WhoseElements`](#isarray)               |                       | collection expectations on the elements of a JSON array          |
| [`IsString`](#value-kinds)                          |                       | a JSON string, optionally equal to a value, compared with an operator or with expectations |
| [`IsNumber`](#value-kinds)                          |                       | a JSON number, optionally compared with an operator or with expectations on its value |
| [`IsNumber<TNumber>`](#numbers)                     |                       | a JSON number that fits into the number type, optionally equal to a value |
| [`IsBoolean`](#value-kinds)                         |                       | a JSON boolean, i.e. `true` or `false`, optionally equal to a value or with expectations |
| [`IsNull`](#value-kinds)                            |                       | the JSON value `null`, only for a `JsonElement`                  |

## Properties

You can verify that a `JsonElement` is a JSON object with or without a property. The expectations on the value of a
property are the ones of a `JsonElement`, so all expectations on this page are available for it, and the
[typed values](#value-kinds) give access to all expectations of aweXpect:

```csharp
JsonElement track = JsonDocument.Parse(
    """{"title": "Let It Be", "artist": "The Beatles", "durationSeconds": 243}""").RootElement;

await That(track)
    .HasProperty("title", t => t.IsString("Let It Be")).And
    .HasProperty("durationSeconds", d => d.IsNumber<int>(n => n.IsBetween(120).And(600))).And
    .DoesNotHaveProperty("password");
```

The failure names the value by its JSON path. If the title was "Yesterday", the expectation would fail with:

```text title="Failure message"
Expected that track
has property "title" which is a string equal to "Let It Be" and has property "durationSeconds" which is an int which is between 120 and 600 and does not have property "password",
but $.title was "Yesterday", which differs at index 0:
   ↓ (actual)
  "Yesterday"
  "Let It Be"
   ↑ (expected)

Actual ($.title):
"Yesterday"
```

A failed lookup or value adds the complete JSON that it verified, indented, as `Actual` context, as the failure itself shows a value only in a single line and shortened. A nested expectation adds the value it verified, labeled with its JSON path, e.g. `Actual ($.title):`, instead of the whole document.

Every failing part of a combined expectation is reported. The expectations on a property are only verified when the
property exists, otherwise the failure says that it did not exist, e.g. `but property $.title did not exist`. With
`.DoesNotHaveProperty("artist")`, the track would fail with:

```text title="Failure message"
Expected that track
has property "title" and does not have property "artist",
but it had property "artist" with "The Beatles"

Actual:
{
  "title": "Let It Be",
  "artist": "The Beatles",
  "durationSeconds": 243
}
```

The name is taken literally, so that names like `"@odata.context"` or `"user.name"` can be addressed.

The expectations can be nested, and the JSON path in the failure continues with each level:

```csharp
JsonElement album = JsonDocument.Parse(
    """{"artist": {"name": "The Beatles", "origin": {"city": "Liverpool"}}}""").RootElement;

await That(album).HasProperty("artist", a => a
    .HasProperty("origin", o => o.Matches(new { city = "Liverpool" })));
```

If the city was "London" instead, the expectation would fail with:

```text title="Failure message"
Expected that album
has property "artist" which has property "origin" which matches new { city = "Liverpool" },
but $.artist.origin differed as $.artist.origin.city was "London" instead of "Liverpool"

JSON options ($.artist.origin):
 - ignore additional properties
```

<details>
<summary>Paths, other kinds and negation</summary>

The paths of a nested `Matches` continue the path in the same way, also for the members to ignore, e.g.
`IgnoringMembers(path => path == "$.artist.origin.city")`. In the JSON path of a failure, a name that is no plain identifier
is written in brackets, e.g. `$['@odata.context']`, so that it cannot be read as several segments.

A subject that is no JSON object fails `HasProperty` and `DoesNotHaveProperty` alike, e.g. with
`but it was an array`.

Negated, e.g. within `DoesNotComplyWith(…)`, the expectations on a property are an alternative: the negation is met
when the property does not exist or its value does not satisfy them. Negating
`HasProperty("title", t => t.IsString("Let It Be"))` would fail for the track with:

```text title="Failure message"
Expected that track
does not have property "title" or $.title is not a string equal to "Let It Be",
but $.title was "Let It Be"

Actual ($.title):
"Let It Be"
```

The same applies to [`HasValueAt`](#values-at-a-path) and [`HasElementAt`](#elements).

</details>

### `null` and missing properties

A property with the value `null` exists, so `HasProperty` accepts it and `DoesNotHaveProperty` fails for it. Verify the
value with `IsNull()`:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "label": null}""").RootElement;

await That(track).HasProperty("label", l => l.IsNull()).And.DoesNotHaveProperty("legacyId");
```

With `TreatingNullAsAbsent()`, a property with the value `null` counts as absent, so `DoesNotHaveProperty` accepts it
and `HasProperty` fails for it, also on the way to a [value at a path](#values-at-a-path):

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "label": null}""").RootElement;

await That(track).DoesNotHaveProperty("label").TreatingNullAsAbsent();
```

Without the option, the expectation would fail with:

```text title="Failure message"
Expected that track
does not have property "label",
but it had property "label" with <null>

Actual:
{
  "title": "Let It Be",
  "label": null
}
```

## Values at a path

You can verify a nested value with a path relative to the subject, instead of nesting `HasProperty` and
`HasElementAt`. The path is a property name, followed by any number of `.name` or zero-based `[index]` segments, and
is written without a leading `$`. Wildcards, filters and recursive descent are not supported, and property names that
contain `.`, `[` or `]` can only be addressed with `HasProperty`:

```csharp
JsonElement album = JsonDocument.Parse(
    """{"artist": {"origin": {"city": "Liverpool"}}, "tracks": [{"title": "Come Together"}]}""").RootElement;

await That(album)
    .HasValueAt("artist.origin.city", c => c.IsString("Liverpool")).And
    .HasValueAt("tracks[0].title", t => t.IsString());
```

If the artist had no origin, the expectation would fail with the first segment that could not be reached:

```text title="Failure message"
Expected that album
has value at "artist.origin.city" which is a string equal to "Liverpool" and has value at "tracks[0].title" which is a string,
but property $.artist.origin did not exist

Actual:
{
  "artist": {},
  "tracks": [
    {
      "title": "Come Together"
    }
  ]
}
```

`TreatingNullAsAbsent()` is available like for [`HasProperty`](#null-and-missing-properties).

Without expectations, `HasValueAt` only verifies that the value exists, and `DoesNotHaveValueAt` that it does not:

```csharp
JsonElement album = JsonDocument.Parse(
    """{"artist": {"origin": {"city": "Liverpool"}}, "tracks": [{"title": "Come Together"}]}""").RootElement;

await That(album)
    .HasValueAt("tracks[0].title").And
    .DoesNotHaveValueAt("artist.origin.country").And
    .DoesNotHaveValueAt("tracks[1].title");
```

<details>
<summary>Values of another kind on the way</summary>

A value on the way that has another kind than the next segment requires fails with e.g.
`but $.artist was a string instead of an object`. A missing property or index on the way satisfies
`DoesNotHaveValueAt`, while a value of another kind on the way fails it like `HasValueAt`, as the path cannot be looked
up in it.

</details>

## Elements

You can verify the element at a zero-based index of a JSON array:

```csharp
JsonElement tracks = JsonDocument.Parse("""[{"title": "Let It Be"}, {"title": "Yesterday"}]""").RootElement;

await That(tracks).HasElementAt(0, t => t.HasProperty("title", p => p.IsString("Let").AsPrefix()));
```

If the first track was "Yesterday", the expectation would fail with:

```text title="Failure message"
Expected that tracks
has element at index 0 which has property "title" which is a string starting with "Let",
but $[0].title was "Yesterday", which differs at index 0:
   ↓ (actual)
  "Yesterday"
  "Let"
   ↑ (expected prefix)

Actual ($[0].title):
"Yesterday"
```

An index beyond the end fails with e.g. `but index $[2] did not exist`, without verifying the expectations.
`HasElementAt(2)` without expectations only verifies that the element exists. To verify
the number of elements or the elements regardless of their position, use [`WhoseElements`](#isarray), and to match
the elements directly, use [`Matches`](./02-matching.md) with an expected array, e.g.
`Matches(new[] { "Let It Be", "Yesterday" })`.

## IsObject

You can verify that a `JsonElement` is a JSON object:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""").RootElement;

await That(track).IsObject();
```

You can verify the properties of an object regardless of their name with `WhoseProperties`. Like
[`WhoseElements`](#isarray) for arrays, it provides all
[collection expectations](/aweXpect/collections/items) of aweXpect, here on the
`JsonProperty` items in the order of the document, e.g. `HasCount(2)` for the number of properties:

```csharp
JsonElement track = JsonDocument.Parse(
    """{"title": "Let It Be", "artist": "The Beatles", "album": "Let It Be"}""").RootElement;

await That(track).IsObject().WhoseProperties.HasCount(3);

await That(track).IsObject().WhoseProperties
    .All().Satisfy(p => p.Value.ValueKind == JsonValueKind.String);

await That(track).IsObject().WhoseProperties
    .All().ComplyWith(p => p.Whose(x => x.Value, v => v.IsString()));

await That(track).IsObject().WhoseProperties
    .None().Satisfy(p => p.Name.StartsWith("_"));
```

If the track also had `"durationSeconds": 243`, the second expectation would fail with:

```text title="Failure message"
Expected that track
is an object whose properties satisfy p => p.Value.ValueKind == JsonValueKind.String for all items,
but only 3 of 4 did

Not matching items (properties):
[
  "durationSeconds":243
]

Collection (properties):
[
  "title":"Let It Be",
  "artist":"The Beatles",
  "album":"Let It Be",
  "durationSeconds":243
]
```

:::warning
A `JsonProperty` has no value equality, so verify the properties with predicates on their `Name` and `Value`, like
`Satisfy(p => …)` or `Contains(p => …)`, or with expectations on their value, like
`ComplyWith(p => p.Whose(x => x.Value, v => …))`.
:::

<details>
<summary>Duplicate property names</summary>

A name that occurs more than once in the object is kept in `WhoseProperties` as often as it occurs.

</details>

## IsArray

You can verify that a `JsonElement` is a JSON array:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday"]""").RootElement;

await That(titles).IsArray();
```

You can verify the elements of an array regardless of their position with `WhoseElements`. It provides all
[collection expectations](/aweXpect/collections/items) of aweXpect, e.g. the
[quantifiers](/aweXpect/collections/items#quantifiers) `All()`, `AtLeast(…)`, `AtMost(…)`,
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

await That(tracks).IsArray().WhoseElements
    .All().ComplyWith(t => t.Matches(new { artist = "The Beatles" }));

await That(tracks).IsArray().WhoseElements
    .AtLeast(2).ComplyWith(t => t.HasProperty("durationSeconds", d => d.IsNumber()));

await That(tracks).IsArray().WhoseElements
    .Contains([t => t.Matches(new { title = "Hey Jude" })]);

await That(tracks).IsArray().WhoseElements
    .Contains(t => t.GetProperty("durationSeconds").GetInt32() > 400);

await That(tracks).IsArray().WhoseElements.HasCount(3);
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

await That(title).IsString();
await That(JsonDocument.Parse("243").RootElement).IsNumber();
await That(JsonDocument.Parse("false").RootElement).IsBoolean();
await That(JsonDocument.Parse("null").RootElement).IsNull();
```

`IsNumber()` on the title would fail with:

```text title="Failure message"
Expected that title
is a number,
but it was a string
```

`IsString`, `IsNumber` and `IsBoolean` also accept an expected value. `IsString(…)` compares the string exactly and
takes the string options of aweXpect, e.g. `AsPrefix()`, `AsWildcard()`, `AsRegex()` or `IgnoringCase()`, and
`IsNumber(…)` takes a tolerance of the type of the expected value with `Within(…)`, also relative to the expected value
with `Within(…).Percent()`:

```csharp
JsonElement track = JsonDocument.Parse(
    """{"title": "Let It Be", "rating": 4.5, "explicit": false}""").RootElement;

await That(track)
    .HasProperty("title", t => t.IsString("let").AsPrefix().IgnoringCase()).And
    .HasProperty("rating", r => r.IsNumber(4.5).Within(0.1)).And
    .HasProperty("explicit", e => e.IsBoolean(false));
```

If the rating was 4.7, the expectation would fail with:

```text title="Failure message"
Expected that track
has property "title" which is a string starting with "let" ignoring case and has property "rating" which is a double equal to 4.5 ± 0.1 and has property "explicit" which is a boolean equal to False,
but $.rating was 4.7

Actual ($.rating):
4.7
```

Like `IsString()`, a value of another kind fails the expectation, e.g. `IsString("4.5")` on the rating fails with
`but $.rating was a number`.

`IsString()` and `IsNumber()` also offer operators on their result, which compare the value in the same sentence. A
string offers `EqualTo`, `StartingWith`, `EndingWith` and `Containing`, their negations and `NotContaining`, with the
string options of aweXpect, e.g. `IgnoringCase()` or `AsWildcard()`, and with a quantifier for `Containing`, e.g.
`Containing("e").AtLeast(2)`. A number is compared as `double` with `EqualTo`, `GreaterThan`,
`GreaterThanOrEqualTo`, `LessThan`, `LessThanOrEqualTo`, `Between(…).And(…)` and their negations, and `EqualTo` and
`NotEqualTo` take a tolerance with `Within(…)`, also relative with `Within(…).Percent()`:

```csharp
JsonElement track = JsonDocument.Parse(
    """{"title": "Let It Be", "rating": 4.5}""").RootElement;

await That(track)
    .HasProperty("title", t => t.IsString().StartingWith("let").IgnoringCase()).And
    .HasProperty("rating", r => r.IsNumber().Between(1).And(5));
```

If the rating was 6, the expectation would fail with:

```text title="Failure message"
Expected that track
has property "title" which is a string starting with "let" ignoring case and has property "rating" which is a number between 1.0 and 5.0,
but $.rating was 6

Actual ($.rating):
6
```

Only one operator can follow `IsString()` or `IsNumber()`; chain further checks with `.And`, e.g.
`IsString().StartingWith("Let").And.IsString().EndingWith("Be")`.

For richer checks, `IsString`, `IsNumber` and `IsBoolean` accept expectations on the value as a `string`, a `double`
or a `bool`, so that all expectations of aweXpect apply to it:

```csharp
JsonElement track = JsonDocument.Parse(
    """{"title": "Let It Be", "rating": 4.5, "explicit": false}""").RootElement;

await That(track)
    .HasProperty("title", t => t.IsString(s => s.StartsWith("Let").And.HasLength(9))).And
    .HasProperty("rating", r => r.IsNumber(n => n.IsBetween(1).And(5))).And
    .HasProperty("explicit", e => e.IsBoolean(b => b.IsFalse()));
```

:::note
For a nullable `JsonElement?`, `IsNull()` is the general expectation of aweXpect that verifies that the subject itself
is `null`, so `IsString()`, `IsNumber()` and `IsBoolean()` are available, but not a JSON-specific `IsNull()`.
:::

<details>
<summary>Negation, operators and very large numbers</summary>

A value of another kind satisfies the negation of `IsString`, `IsNumber` and `IsBoolean`, so it succeeds within
`DoesNotComplyWith(…)`. A second operator on the same result of `IsString()` or `IsNumber()` throws an
`InvalidOperationException`. A number that does not fit into a `double`, which `IsNumber()` alone accepts, fails both
an operator and its negation, as whether it is e.g. greater than 3 cannot be answered; the same holds for
`IsNumber(n => …)`. An invalid pattern, e.g. of `AsRegex()`, throws whichever value the subject has.

</details>

### Numbers

`IsNumber<TNumber>()` verifies that a number fits into the number type, e.g. that it is an integer within the range of
an `int`, and `IsNumber<TNumber>(n => …)` verifies the value as this type. The supported types are `byte`, `sbyte`,
`short`, `ushort`, `int`, `uint`, `long`, `ulong`, `BigInteger`, `float`, `double` and `decimal`, and on .NET 8 or
later also `nint`, `nuint`, `Int128`, `UInt128` and `Half`. `IsNumber(n => …)` without a type argument verifies the
number as `double`:

```csharp
JsonElement track = JsonDocument.Parse("""{"durationSeconds": 243, "price": 9.99}""").RootElement;

await That(track)
    .HasProperty("durationSeconds", d => d.IsNumber<long>(n => n.IsBetween(120).And(600))).And
    .HasProperty("price", p => p.IsNumber<decimal>(n => n.IsEqualTo(9.99m)));
```

If the duration was `243.5`, `HasProperty("durationSeconds", d => d.IsNumber<int>())` would fail with:

```text title="Failure message"
Expected that track
has property "durationSeconds" which is an int,
but $.durationSeconds was 243.5, which is not an int

Actual ($.durationSeconds):
243.5
```

`IsNumber(expected)` compares the number as the type of the expected value, so `IsNumber(243)` reads it as `int`,
`IsNumber(9.99m)` as `decimal` and `IsNumber(4.5)` as `double`. A `long` or a `decimal` is thereby compared exactly,
without the rounding of a `double`:

```csharp
JsonElement track = JsonDocument.Parse("""{"durationSeconds": 243, "price": 9.99}""").RootElement;

await That(track)
    .HasProperty("durationSeconds", d => d.IsNumber(243)).And
    .HasProperty("price", p => p.IsNumber(9.99m));
```

:::warning
A number that does not fit into the type of the expected value fails the expectation, like for `IsNumber<TNumber>()`.
If the duration was `243.0`, the expectation above would fail with:

```text title="Failure message"
Expected that track
has property "durationSeconds" which is an int equal to 243 and has property "price" which is a decimal equal to 9.99,
but $.durationSeconds was 243.0, which is not an int

Actual ($.durationSeconds):
243.0
```

Compare such a number as `double` with `IsNumber(243.0)` or `IsNumber<double>(243)`.
:::
