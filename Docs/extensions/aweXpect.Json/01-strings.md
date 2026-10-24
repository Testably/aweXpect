# JSON strings

Describes the possible expectations for strings that contain JSON.

| Expectation                                           | Negated                    | Summary                                                         |
|-------------------------------------------------------|----------------------------|-----------------------------------------------------------------|
| [`IsEqualTo(…).AsJson()`](#string-comparison-as-json) | `IsNotEqualTo(…).AsJson()` | equal to the expected JSON, compared by structure               |
| [`IsValidJson`](#validation)                          |                            | can be parsed as JSON, optionally with expectations on its root |
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

The expected string must be valid JSON, otherwise an `ArgumentException` is thrown. As JSON is compared by its
structure, `AsJson()` cannot be combined with the
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
[`JsonDocumentOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions):

```csharp
string track = """{"title": "Let It Be"}""";

await Expect.That(track).IsValidJson(
    o => o with { CommentHandling = JsonCommentHandling.Disallow });
```

You can add further expectations on the root
[`JsonElement`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement) parsed from the subject,
optionally followed by the `JsonDocumentOptions`:

```csharp
string track = """{"title": "Let It Be"}""";

await Expect.That(track).IsValidJson(j => j.Matches(new { title = "Let It Be" }));
await Expect.That(track).IsValidJson(j => j.IsObject(),
    o => o with { AllowTrailingCommas = true });
```

Or you can verify that the string is valid JSON and [matches](./02-matching.md) an expected value in one step:

```csharp
string track = """{"title": "Let It Be", "artist": "The Beatles"}""";

await Expect.That(track).IsValidJsonMatching(new { title = "Let It Be" });
await Expect.That(track).IsValidJsonMatchingExactly(
    new { title = "Let It Be", artist = "The Beatles" });
```

`IsValidJsonMatching(new { title = "Yesterday" })` would fail with:

```text title="Failure message"
Expected that track
is valid JSON which matches new { title = "Yesterday" },
but it differed as $.title was "Let It Be" instead of "Yesterday"

JSON options:
 - ignore additional properties
```
