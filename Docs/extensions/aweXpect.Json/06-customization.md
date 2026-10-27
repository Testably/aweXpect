# Customization

Set a value under `Customize.aweXpect.Json()` for the current
[async flow](https://docs.testably.org/aweXpect/how-it-works/configuration#lifetimes-and-async-flows), or under
`Customize.aweXpect.Global.Json()` for
[all tests](https://docs.testably.org/aweXpect/how-it-works/configuration#global-defaults).

| Value                          | Default                      | Summary                                                                                                                                                                         |
|--------------------------------|------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `DefaultJsonDocumentOptions`   | `AllowTrailingCommas = true` | the options to parse JSON strings                                                                                                                                               |
| `DefaultJsonSerializerOptions` | `AllowTrailingCommas = true` | the options to format JSON values, serialize the subject of `IsJsonSerializable` and `IsJsonSerializedAs` and, once set, [expected values](./02-matching.md#serializer-options) |

You can change the default
[`JsonDocumentOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions) and the
default [`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions).
Each value is set on its own:

```csharp
using (Customize.aweXpect.Json().DefaultJsonDocumentOptions.Set(new JsonDocumentOptions
       {
           AllowTrailingCommas = false,
       }))
{
    // parses JSON strings without allowing trailing commas
}
```

To use other `JsonDocumentOptions` for a single expectation, pass them with `WithDocumentOptions(…)` wherever
[`JsonOptions`](./02-matching.md#options) are accepted, also in [`IsValidJson(o => …)`](./01-strings.md#validation):

```csharp
string track = """{"title": "Let It Be", /* 1970 */ "artist": "The Beatles"}""";

await Expect.That(track).IsValidJsonMatching(new { title = "Let It Be" },
    o => o.WithDocumentOptions(d => d with { CommentHandling = JsonCommentHandling.Skip }));
```
