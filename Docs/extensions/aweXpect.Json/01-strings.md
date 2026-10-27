# JSON strings

Describes the possible expectations for strings that contain JSON.

| Expectation                                           | Negated                    | Summary                                                         |
|-------------------------------------------------------|----------------------------|-----------------------------------------------------------------|
| [`IsEqualTo(…).AsJson()`](#string-comparison-as-json) | `IsNotEqualTo(…).AsJson()` | equal to the expected JSON, compared by structure               |
| [`IsValidJson`](#validation)                          | `IsNotValidJson`           | can be parsed as JSON, optionally with expectations on its root |
| [`IsValidJsonMatching`](#validation)                  |                            | valid JSON that matches the expected value                      |
| [`IsValidJsonMatchingExactly`](#validation)           |                            | valid JSON that matches the expected value exactly              |

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

As JSON is compared by its structure, `AsJson()` cannot be combined with the
[string options](https://docs.testably.org/aweXpect/values/string#string-options) `IgnoringCase()` or
`Using(comparer)`. Instead, `AsJson(o => …)` accepts the [JSON options](./02-matching.md#options) to relax the
comparison.

:::note
`AsJson()` is not available on `Contains` or `DoesNotContain` for a string subject, as parts of a JSON string are
usually not valid JSON. Use `IsEqualTo(expected).AsJson()` or `IsValidJsonMatching(expected)` instead. On a collection
of strings, `Contains(expected).AsJson()` compares each item as JSON.
:::

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
[`JsonDocumentOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions) with
`WithDocumentOptions(…)`; the other [JSON options](./02-matching.md#options) are ignored, as there is nothing to
compare:

```csharp
string track = """{"title": "Let It Be"}""";

await Expect.That(track).IsValidJson(
    o => o.WithDocumentOptions(d => d with { CommentHandling = JsonCommentHandling.Disallow }));
```

You can also verify that a string is not valid JSON:

```csharp
string track = "Let It Be";

await Expect.That(track).IsNotValidJson();
```

The `JsonDocumentOptions` are available here as well, e.g. to reject a trailing comma, which is allowed by default:

```csharp
string track = """{"title": "Let It Be",}""";

await Expect.That(track).IsNotValidJson(
    o => o.WithDocumentOptions(d => d with { AllowTrailingCommas = false }));
```

If the string is valid JSON, e.g. `"[1, 2]"`, the expectation fails with:

```text title="Failure message"
Expected that track
is not valid JSON,
but it was "[1, 2]"
```

You can add further expectations on the root
[`JsonElement`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement) parsed from the subject,
optionally followed by the document options:

```csharp
string track = """{"title": "Let It Be"}""";

await Expect.That(track).IsValidJson(j => j.Matches(new { title = "Let It Be" }));
await Expect.That(track).IsValidJson(j => j.IsObject(),
    o => o.WithDocumentOptions(d => d with { AllowTrailingCommas = true }));
```

Or you can verify that the string is valid JSON and [matches](./02-matching.md) an expected value in one step:

```csharp
string track = """{"title": "Let It Be", "artist": "The Beatles"}""";

await Expect.That(track).IsValidJsonMatching(new { title = "Let It Be" });
await Expect.That(track).IsValidJsonMatchingExactly(new { title = "Let It Be", artist = "The Beatles" });
await Expect.That(track).IsValidJsonMatching("""{"title": "Let It Be"}""");
```

An expected `string` is [JSON text](./02-matching.md#expected-json), like in `Matches`.

`IsValidJsonMatching(new { title = "Yesterday" })` would fail with:

```text title="Failure message"
Expected that track
is valid JSON which matches new { title = "Yesterday" },
but it differed as $.title was "Let It Be" instead of "Yesterday"

JSON options:
 - ignore additional properties
```

<details>
<summary>`null` subjects and negation</summary>

A `null` subject fails `IsValidJson` and `IsNotValidJson` alike.

With expectations on the root element and with `IsValidJsonMatching` or `IsValidJsonMatchingExactly`, a string that
is not valid JSON fails also when the expectation is negated, e.g. within `DoesNotComplyWith(…)`, as there is no root
element to verify. Only the negation of `IsValidJson()` without expectations is met by a string that is not valid JSON.

</details>
