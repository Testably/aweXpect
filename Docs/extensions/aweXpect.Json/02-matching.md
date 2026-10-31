---
description: Match a JsonElement or JsonNode with an expected value and relax the comparison with the JSON options.
---

# Matching JSON

Describes the possible expectations for matching a `JsonElement` or a
[`JsonNode`](./04-json-nodes.md) with an expected value, and the [options](#options) to relax the
comparison.

| Expectation                  | Negated | Summary                                                   |
|------------------------------|---------|-----------------------------------------------------------|
| [`Matches`](#objects)        |         | matches the expected value, additional properties allowed |
| [`MatchesExactly`](#objects) |         | matches the expected value exactly                        |

## Objects

You can verify that a `JsonElement` matches an expected object. `Matches` ignores additional properties, while
`MatchesExactly` also fails for them:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""").RootElement;

await That(track).Matches(new { title = "Let It Be" });
await That(track).MatchesExactly(new { title = "Let It Be", artist = "The Beatles" });
```

`Matches` accepts the same values as `MatchesExactly` with the option `IgnoringAdditionalProperties()`.
`MatchesExactly(new { title = "Let It Be" })` would fail because of the additional `artist` property:

```text title="Failure message"
Expected that track
matches new { title = "Let It Be" } exactly,
but it differed as $.artist had unexpected "The Beatles"
```

## Arrays

You can verify that a `JsonElement` matches an expected array:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday", "Hey Jude"]""").RootElement;

await That(titles).Matches(["Let It Be", "Yesterday", "Hey Jude"]);
await That(titles).MatchesExactly(["Let It Be", "Yesterday", "Hey Jude"]);
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

## Primitive values

You can also verify that a `JsonElement` matches a primitive value:

```csharp
await That(JsonDocument.Parse("4.05").RootElement).Matches(4.05);
await That(JsonDocument.Parse("true").RootElement).Matches(true);
await That(JsonDocument.Parse("null").RootElement).Matches(null);
```

An expected `string` is [JSON text](#expected-json), so `Matches("42")` expects the number `42`. To match a JSON
string, write it with quotes or as an `It.Is<string>()` expectation:

```csharp
JsonElement title = JsonDocument.Parse("\"Let It Be\"").RootElement;

await That(title).Matches("\"Let It Be\"");
await That(title).Matches(It.Is<string>().That.IsEqualTo("Let It Be"));
```

A string inside an expected object or array, e.g. `new { title = "Let It Be" }`, and a string typed as `object`, e.g.
`(object)"Let It Be"`, stay JSON strings.

Any expected value, also a nested one, can be an `It.Is<T>()` expectation instead, e.g.
`Matches(new { duration = It.Is<long>().That.IsGreaterThan(200) })`.

<details>
<summary>Numbers in `It.Is<T>()`</summary>

A JSON number meets `It.Is<T>()` as a `T` when System.Text.Json can read it as that numeric type, so `It.Is<long>()`,
`It.Is<double>()` and `It.Is<decimal>()` all accept `243`, while an integer type does not accept a number with a
fraction or an exponent, e.g. `2.5` or `1e3`.

</details>

## Expected JSON

An expected `string` is parsed as JSON text:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""").RootElement;

await That(track).Matches("""{"title": "Let It Be"}""");
```

`MatchesExactly("""{"title": "Let It Be"}""")` would fail because of the additional `artist` property:

```text title="Failure message"
Expected that track
matches """{"title": "Let It Be"}""" exactly,
but it differed as $.artist had unexpected "The Beatles"
```

The JSON text is parsed with the `JsonDocumentOptions` of [`WithDocumentOptions(…)`](./06-customization.md), and
`WithSerializerOptions(…)` does not apply to it. Invalid JSON throws an `ArgumentException` when the expectation is
written, so that also a negated expectation, e.g. within `DoesNotComplyWith(…)`, cannot pass.

The expected value can also be a `JsonElement`, a `JsonDocument` or a `JsonNode`, whose JSON is used as it is.

## Options

The options are available wherever `JsonOptions` are accepted, e.g. `Matches`, `MatchesExactly`,
`IsEqualTo(expected).AsJson(o => …)`, `IsValidJsonMatching` and
[`IsJsonSerializedAs`](./05-serialization.md#serialized-as-json).
They work like the [options of aweXpect](/aweXpect/how-it-works/options):

| Option                                                                    | Configures                                             |
|---------------------------------------------------------------------------|--------------------------------------------------------|
| [`IgnoringAdditionalProperties()`](#objects)                              | ignores additional properties of objects               |
| [`TreatingNullAsAbsent()`](#treating-null-as-absent)                      | a property with the value `null` counts as absent      |
| [`IgnoringAdditionalArrayElements()`](#additional-array-elements)         | ignores additional elements of arrays                  |
| [`IgnoringArrayOrder()`](#array-order)                                    | the elements of arrays can be in any order             |
| [`IgnoringMember(name)`, `IgnoringMembers(path => …)`](#ignoring-members) | leaves values out of the comparison                    |
| [`ComparingNumbersWithin(tolerance)`](#numbers-within-a-tolerance)        | numbers match within an absolute or relative tolerance |
| [`WithDocumentOptions(…)`](./06-customization.md)                         | the `JsonDocumentOptions` used to parse JSON strings   |
| [`WithSerializerOptions(…)`](#serializer-options)                         | the `JsonSerializerOptions` for the expected value     |

### Treating `null` as absent

By default, a property with the value `null` differs from a missing property. With `TreatingNullAsAbsent()`, a
property with the value `null` counts as absent, in both directions:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "album": null}""").RootElement;

await That(track).MatchesExactly(new { title = "Let It Be" },
    o => o.TreatingNullAsAbsent());
await That(track).MatchesExactly(new { title = "Let It Be", artist = (string?)null },
    o => o.TreatingNullAsAbsent());
```

Without the option, `MatchesExactly(new { title = "Let It Be" })` would fail with:

```text title="Failure message"
Expected that track
matches new { title = "Let It Be" } exactly,
but it differed as $.album had unexpected <null>
```

Elements of an array with the value `null` are not affected.

### Additional array elements

With `IgnoringAdditionalArrayElements()`, the additional elements of all compared arrays, also nested ones, are
ignored:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday", "Hey Jude"]""").RootElement;

await That(titles).Matches(["Let It Be", "Yesterday"],
    o => o.IgnoringAdditionalArrayElements());
```

### Array order

By default, arrays are compared by index. With `IgnoringArrayOrder()`, the elements of all compared arrays, also
nested ones, can be in any order:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday", "Hey Jude"]""").RootElement;

await That(titles).MatchesExactly(["Hey Jude", "Let It Be", "Yesterday"],
    o => o.IgnoringArrayOrder());
```

Combined with `IgnoringAdditionalArrayElements()`, the expected elements only have to be contained in the subject in
any order. `MatchesExactly(["Hey Jude", "Something", "Help!"], o => o.IgnoringArrayOrder())` would fail with:

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

<details>
<summary>How the elements are paired</summary>

Each expected element is paired with the first matching element of the subject that is not paired yet. The expected
elements that could not be paired are reported as missing on the array, the remaining elements of the subject as
unexpected at their index. When exactly one element remains unpaired on each side, they are compared with each other
instead, e.g. `$[1] was "Yesterday" instead of "Something"`.
[`HasElementAt`](./03-objects-and-arrays.md#elements) still verifies the element at its index.

The pairing is greedy, so overlapping `It.Is…` expectations can fail, although another pairing would succeed.

</details>

### Ignoring members

Generated values, e.g. ids, timestamps or ETags, can be left out of the comparison with `IgnoringMember(name)` or
`IgnoringMembers(path => …)`. An ignored value and everything below it is neither compared nor reported as missing or
unexpected:

```csharp
JsonElement track = JsonDocument.Parse(
    """{"id": 42, "title": "Let It Be", "meta": {"createdAt": "2026-10-16", "etag": "abc"}}""").RootElement;

await That(track).MatchesExactly(
    new { id = 1, title = "Let It Be", meta = new { createdAt = "2026-01-01" } },
    o => o.IgnoringMember("id").IgnoringMember("createdAt").IgnoringMembers(path => path.EndsWith(".etag")));
```

The name is matched against the JSON path, as it is shown in failure messages, and must cover whole trailing segments
of it, with the same casing, as JSON property names are case-sensitive: `"id"` ignores `$.id` and `$.items[0].id`, but
not `$.parentId` or `$.ID`, `"meta.createdAt"` only ignores `createdAt` within `meta`, and `"$.id"` only ignores the
property of the root. `"[0]"` ignores the first element of every array.

The members to ignore are listed in the "JSON options" context. When the comparison fails, each name that matched no
member is named below them, together with the member it most likely meant, e.g. one that differs only in casing, as a
misspelled name otherwise silently ignores nothing. For a track `{"title": "Let It Be", "createdAt": "2026-10-16"}`,
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

<details>
<summary>Ignored indices and explicit expectations</summary>

With `IgnoringArrayOrder()`, an index refers to the element of the subject, which is then left out of the pairing.
Explicit expectations like [`HasProperty("id")`](./03-objects-and-arrays.md#properties), `DoesNotHaveProperty("id")`
or [`HasElementAt(0, …)`](./03-objects-and-arrays.md#elements) still verify an ignored value.

</details>

### Numbers within a tolerance

Calculated numbers rarely match an expected literal exactly. With `ComparingNumbersWithin(tolerance)`, a number matches
when it differs from the expected number by at most the tolerance, and with `ComparingNumbersWithin(tolerance).Percent()`
by at most the percentage of the expected number. The option can be combined with the other options:

```csharp
JsonElement album = JsonDocument.Parse(
    """{"durationMinutes": 47.050000000000004, "rating": 4.6005}""").RootElement;

await That(album).MatchesExactly(new { durationMinutes = 47.05, rating = 4.6m },
    o => o.ComparingNumbersWithin(0.001));
await That(album).MatchesExactly(new { rating = 4.6m },
    o => o.ComparingNumbersWithin(1).Percent().IgnoringAdditionalProperties());
```

The tolerance applies to every number in the compared JSON, also to integers such as ids or counts. For a track
`{"title": "Hey Jude", "durationSeconds": 431}`,
`MatchesExactly(new { title = "Hey Jude", durationSeconds = 400 }, o => o.ComparingNumbersWithin(5).Percent())` would
fail with:

```text title="Failure message"
Expected that track
matches new { title = "Hey Jude", durationSeconds = 400 } exactly,
but it differed as $.durationSeconds was 431 instead of 400 ± 5%

JSON options:
 - compare numbers within ± 5%
```

<details>
<summary>Zero and very large numbers</summary>

Like the [tolerance in aweXpect](/aweXpect/values/number#tolerance), a relative tolerance
only matches an equal number when the expected number is `0`. The numbers are compared as `double`, so numbers outside
its range are compared by their text.

</details>

### Serializer options

The expected value is serialized to JSON before the comparison. By default, the property names of an anonymous object
are kept as they are, so a subject with camelCase names needs `new { title = "Let It Be" }`. With
`WithSerializerOptions(…)`, the expected value is serialized with the given
[`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions) instead,
e.g. with a naming policy:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""").RootElement;
JsonSerializerOptions camelCase = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

await That(track).MatchesExactly(new { Title = "Let It Be", Artist = "The Beatles" },
    o => o.WithSerializerOptions(camelCase));
```

The comparison itself stays strict: only the JSON of the expected value changes. Without the option, the expected value
is serialized with the `DefaultJsonSerializerOptions` of the [customization](./06-customization.md) when they are
customized, except in [`IsJsonSerializedAs`](./05-serialization.md#serialized-as-json). `It.Is…` expectations keep
working with any options.

Pass the options of a source-generated `JsonSerializerContext` to use it with trimming or
[Native AOT](./05-serialization.md#serialized-as-json).

<details>
<summary>Source-generated serializer contexts</summary>

Options with a `TypeInfoResolver`, e.g. those of a source-generated `JsonSerializerContext`, serialize the expected
value without reflection. Each type of the expected value, also the runtime type of a value in an `object` property,
must then be registered in the context; otherwise a `NotSupportedException` names the missing
`[JsonSerializable(typeof(…))]` attribute.

</details>

### Options in failure messages

The failure message lists the active options in a "JSON options" context, like in the examples above, and omits it
when no option is active. The options of `WithDocumentOptions(…)` and `WithSerializerOptions(…)` are not listed. As `Matches` and `IsValidJsonMatching` ignore additional properties by default, their failures list
`ignore additional properties`.
