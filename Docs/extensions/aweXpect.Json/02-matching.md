# Matching JSON

`Matches` and `MatchesExactly` compare a `JsonElement` or a [`JsonNode`](./03-objects-and-arrays.md#jsonnode) with an
expected value. The options on this page are available wherever `JsonOptions` are accepted, e.g.
`IsEqualTo(expected).AsJson(o => …)`, `IsValidJsonMatching`, [`IsObject`](./03-objects-and-arrays.md#isobject),
[`IsArray`](./03-objects-and-arrays.md#isarray) and [`IsJsonSerializedAs`](./04-serialization.md#serialized-as-json).

## Objects

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

## Arrays

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

## Primitive values

You can also verify that a `JsonElement` matches a primitive value:

```csharp
await Expect.That(JsonDocument.Parse("\"Let It Be\"").RootElement).Matches("Let It Be");
await Expect.That(JsonDocument.Parse("4.05").RootElement).Matches(4.05);
await Expect.That(JsonDocument.Parse("true").RootElement).Matches(true);
await Expect.That(JsonDocument.Parse("null").RootElement).Matches(null);
```

## Treating `null` as absent

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

Elements of an array with the value `null` are not affected.

## Additional array elements

With `IgnoringAdditionalArrayElements()`, the additional elements of all compared arrays, also nested ones, are
ignored:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday", "Hey Jude"]""").RootElement;

await Expect.That(titles).Matches(["Let It Be", "Yesterday"],
    o => o.IgnoringAdditionalArrayElements());
```

## Array order

By default, arrays are compared by index. With `IgnoringArrayOrder()`, the elements of all compared arrays, also
nested ones, can be in any order:

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
expectations can fail, although another pairing would succeed. The positional members of
[`IsArray`](./03-objects-and-arrays.md#isarray) (`At`, `WithElements`, `WithArrays` and `WithObjects`) still verify the
element at each index.

## Ignoring members

Generated values, e.g. ids, timestamps or ETags, can be left out of the comparison with `IgnoringMember(name)` or
`Ignoring(path => …)`. An ignored value and everything below it is neither compared nor reported as missing or
unexpected:

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
out of the pairing. Explicit expectations of [`IsObject`](./03-objects-and-arrays.md#isobject) and
[`IsArray`](./03-objects-and-arrays.md#isarray), e.g. `With("id")`, `Without("id")` or `At(0)`, still verify an
ignored value.

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

## Numbers within a tolerance

Calculated numbers rarely match an expected literal exactly. With `NumbersWithin(tolerance)`, a number matches when it
differs from the expected number by at most the tolerance, and with `NumbersWithin(tolerance).Percent()` by at most the
percentage of the expected number. The option can be combined with the other options:

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

## Options in failure messages

The failure message lists the active options in a "JSON options" context, like in the examples above, and omits it
when no option is active. As `Matches`, `IsValidJsonMatching`, [`IsObject`](./03-objects-and-arrays.md#isobject) and
[`IsArray`](./03-objects-and-arrays.md#isarray) ignore additional properties by default, their failures list
`ignore additional properties`.
