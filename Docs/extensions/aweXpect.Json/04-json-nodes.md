---
description: Use the JSON expectations on a JsonNode and its subtypes JsonObject, JsonArray and JsonValue.
---

# `JsonNode`

A [`JsonNode`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonnode) and its subtypes
`JsonObject`, `JsonArray` and `JsonValue`, nullable or not, have the same expectations as a `JsonElement`:
[`Matches`, `MatchesExactly`](./02-matching.md), `HasProperty`, `DoesNotHaveProperty`, `HasValueAt`,
`DoesNotHaveValueAt`, `HasElementAt`, `IsObject` (with `WhoseProperties`), `IsArray` (with `WhoseElements`), `IsString`, `IsNumber` and `IsBoolean`, with the
same options and failure messages:

```csharp
JsonNode? track = JsonNode.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""");

await That(track).Matches(new { title = "Let It Be" });
await That(track).HasProperty("artist", a => a.IsString("The Beatles"));

JsonArray titles = new("Let It Be", "Yesterday");

await That(titles).IsArray().WhoseElements.HasCount(2);
```

`MatchesExactly(new { title = "Let It Be" })` on the track would fail with:

```text title="Failure message"
Expected that track
matches new { title = "Let It Be" } exactly,
but it differed as $.artist had unexpected "The Beatles"
```

The expectations keep the type of the subject for chaining with `.And`. A call that names the number type, like
`IsNumber<int>()` or `IsNumber<double>(243)`, continues on a `JsonNode`, as C# cannot infer the node type then, except
on a `JsonValue`, which has `IsNumber<TNumber>` overloads of its own.

:::note
The node is verified in the state it has when the expectation is awaited, so changes made after `That(…)` are
included.
:::

A `null` node is the JSON value `null`, as `JsonNode.Parse("null")` returns it. Use the general `IsNull()` expectation
of aweXpect to verify that a node is the JSON value `null`, e.g. `HasProperty("label", l => l.IsNull())`, so there is
no JSON-specific `IsNull()` for a `JsonNode`.

<details>
<summary>Nested nodes and `null` nodes</summary>

The expectations on a property, a value at a path or an element receive the node itself, e.g. for `IsSameAs(…)`.
Likewise, `WhoseElements` provides the nodes of an array as `JsonNode?` items and `WhoseProperties` the properties of an
object as `KeyValuePair<string, JsonNode?>` items with the name and the node of the value, instead of `JsonElement` and
`JsonProperty` items:

```csharp
JsonArray playlist = new(new JsonObject { ["title"] = "Let It Be" });
JsonNode first = playlist[0]!;

await That(playlist).HasElementAt(0, e => e.IsSameAs(first));
await That(playlist).IsArray().WhoseElements.Contains(first);
await That(first).IsObject().WhoseProperties.All().Satisfy(p => p.Value is JsonValue);
```

`Matches(null)` succeeds for a `null` node, while `IsObject()` would fail with:

```text title="Failure message"
Expected that node
is an object,
but it was null
```

</details>
