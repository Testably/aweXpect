# Customization

Describes the customization values of aweXpect.Json. They work like the
[customization of aweXpect](https://docs.testably.org/aweXpect/how-it-works/configuration).

| Value                          | Default                      | Used to                                                                                                                                 |
|--------------------------------|------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------|
| `DefaultJsonDocumentOptions`   | `AllowTrailingCommas = true` | parse JSON strings                                                                                                                      |
| `DefaultJsonSerializerOptions` | `AllowTrailingCommas = true` | format JSON values, serialize the subject of `IsJsonSerializedAs` and, once set, [expected values](./02-matching.md#serializer-options) |

You can change the default
[`JsonDocumentOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions) and the
default [`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions).
Each value is set on its own and restored when the returned lifetime is disposed:

```csharp
using (Customize.aweXpect.Json().DefaultJsonDocumentOptions.Set(new JsonDocumentOptions
       {
           AllowTrailingCommas = false,
       }))
{
    // parses JSON strings without allowing trailing commas
}
```

To change a value for all tests, set it on `Customize.aweXpect.Global.Json()` instead, see
[global defaults](https://docs.testably.org/aweXpect/how-it-works/configuration#global-defaults).

To use other `JsonDocumentOptions` for a single expectation, pass them to
[`IsValidJson(o => …)`](./01-strings.md#validation) or, wherever [`JsonOptions`](./02-matching.md#options) are
accepted, with `WithDocumentOptions(…)`:

```csharp
string track = """{"title": "Let It Be", /* 1970 */ "artist": "The Beatles"}""";

await Expect.That(track).IsValidJsonMatching(new { title = "Let It Be" },
    o => o.WithDocumentOptions(d => d with { CommentHandling = JsonCommentHandling.Skip }));
```
