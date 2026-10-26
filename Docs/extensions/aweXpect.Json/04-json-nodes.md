# `JsonNode`

A [`JsonNode`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonnode) and its subtypes
`JsonObject`, `JsonArray` and `JsonValue`, nullable or not, have the same expectations as a `JsonElement`:
[`Matches`, `MatchesExactly`](./02-matching.md), `HasProperty`, `DoesNotHaveProperty`, `HasValueAt`,
`DoesNotHaveValueAt`, `HasElementAt`, `IsObject` (with `WhoseProperties`), `IsArray` (with `WhoseElements`), `IsString`, `IsNumber` and `IsBoolean`, with the
same options and failure messages:

```csharp
JsonNode? track = JsonNode.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""");

await Expect.That(track).Matches(new { title = "Let It Be" });
await Expect.That(track).HasProperty("artist", a => a.IsString("The Beatles"));

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

A `null` node is the JSON value `null`, as `JsonNode.Parse("null")` returns it. Use the general `IsNull()` expectation
of aweXpect to verify that a node is the JSON value `null`, e.g. `HasProperty("label", l => l.IsNull())`, so there is
no JSON-specific `IsNull()` for a `JsonNode`.

<details>
<summary>Nested nodes and `null` nodes</summary>

The expectations on a property, a value at a path or an element receive the node itself, e.g. for `IsSameAs(…)`.
`Matches(null)` succeeds for a `null` node, while `IsObject()` would fail with:

```text title="Failure message"
Expected that node
is an object,
but it was null
```

</details>
