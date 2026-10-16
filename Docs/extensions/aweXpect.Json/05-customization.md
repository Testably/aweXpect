# Customization

You can change the default
[`JsonDocumentOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions), used to
parse JSON strings, and the default
[`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions), used to
format JSON values and to serialize the subject of [`IsJsonSerializedAs`](./04-serialization.md#serialized-as-json).
Both allow trailing commas by default. Each value is set on its own and restored when the returned lifetime is
disposed:

```csharp
using (Customize.aweXpect.Json().DefaultJsonDocumentOptions.Set(new JsonDocumentOptions
       {
           AllowTrailingCommas = false,
       }))
{
    // parses JSON strings without allowing trailing commas
}
```

To change a value for all tests, set it on `Customize.aweXpect.Global.Json()` instead.
