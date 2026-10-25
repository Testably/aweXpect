# aweXpect.Json

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Json)](https://www.nuget.org/packages/aweXpect.Json)

[aweXpect.Json](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Json) contains expectations for JSON strings, `JsonElement`,
`JsonNode` and the JSON serialization of objects with `System.Text.Json`.

## At a glance

```csharp
string json = """{"title": "Let It Be", "artist": "The Beatles", "durationSeconds": 243}""";

// compare JSON strings by their structure instead of their formatting
string expected = """{ "artist": "The Beatles", "title": "Let It Be", "durationSeconds": 243 }""";
await Expect.That(json).IsEqualTo(expected).AsJson();

// verify that a string is valid JSON that matches an expected value
await Expect.That(json).IsValidJsonMatching(new { title = "Let It Be" });

// verify single values of a JsonElement or JsonNode
JsonElement track = JsonDocument.Parse(json).RootElement;
await Expect.That(track)
    .HasProperty("title", t => t.IsString("Let It Be")).And
    .HasProperty("durationSeconds", d => d.IsNumber<int>()).And
    .DoesNotHaveProperty("password");

// verify the JSON contract of a type, e.g. `record Track(string Title, string Artist)`
await Expect.That(new Track("Let It Be", "The Beatles")).IsJsonSerializedAs(
    """{"Title": "Let It Be", "Artist": "The Beatles"}""");
```

A failing comparison names every difference by its JSON path. If the title was "Yesterday", `IsValidJsonMatching`
would fail with:

```text title="Failure message"
Expected that json
is valid JSON which matches new { title = "Let It Be" },
but it differed as $.title was "Yesterday" instead of "Let It Be"

JSON options:
 - ignore additional properties
```

## Overview

| Expectation                                                                 | Subject                   | Summary                                                         |
|-----------------------------------------------------------------------------|---------------------------|-----------------------------------------------------------------|
| [`IsEqualTo(…).AsJson()`](./01-strings.md#string-comparison-as-json) | `string`                  | equal to the expected JSON, compared by structure               |
| [`IsValidJson`](./01-strings.md#validation)                        | `string`                  | can be parsed as JSON, optionally with expectations on its root |
| [`IsNotValidJson`](./01-strings.md#validation)                     | `string`                  | cannot be parsed as JSON                                        |
| [`IsValidJsonMatching`](./01-strings.md#validation)                | `string`                  | valid JSON that matches the expected value                      |
| [`IsValidJsonMatchingExactly`](./01-strings.md#validation)         | `string`                  | valid JSON that matches the expected value exactly              |
| [`Matches`](./02-matching.md)                                      | `JsonElement`, `JsonNode` | matches the expected value, additional properties allowed       |
| [`MatchesExactly`](./02-matching.md)                               | `JsonElement`, `JsonNode` | matches the expected value exactly                              |
| [`HasProperty`](./03-objects-and-arrays.md#properties)             | `JsonElement`, `JsonNode` | a JSON object with the property, optionally with expectations   |
| [`DoesNotHaveProperty`](./03-objects-and-arrays.md#properties)     | `JsonElement`, `JsonNode` | a JSON object without the property                              |
| [`HasValueAt`](./03-objects-and-arrays.md#values-at-a-path)        | `JsonElement`, `JsonNode` | a value at a path, optionally with expectations                 |
| [`DoesNotHaveValueAt`](./03-objects-and-arrays.md#values-at-a-path) | `JsonElement`, `JsonNode` | no value at a path                                             |
| [`HasElementAt`](./03-objects-and-arrays.md#elements)              | `JsonElement`, `JsonNode` | a JSON array with an element at the index, optionally with expectations |
| [`IsObject`](./03-objects-and-arrays.md#isobject)                  | `JsonElement`, `JsonNode` | a JSON object                                                   |
| [`IsObject().WhoseProperties`](./03-objects-and-arrays.md#isobject) | `JsonElement`, `JsonNode` | collection expectations on the properties of a JSON object      |
| [`IsArray`](./03-objects-and-arrays.md#isarray)                    | `JsonElement`, `JsonNode` | a JSON array                                                    |
| [`IsArray().WhoseElements`](./03-objects-and-arrays.md#isarray)    | `JsonElement`, `JsonNode` | collection expectations on the elements of a JSON array         |
| [`IsString`](./03-objects-and-arrays.md#value-kinds)               | `JsonElement`, `JsonNode` | a JSON string, optionally equal to a value or with expectations |
| [`IsNumber`](./03-objects-and-arrays.md#value-kinds)               | `JsonElement`, `JsonNode` | a JSON number, optionally with expectations on its value        |
| [`IsString().StartingWith(…)`, …](./03-objects-and-arrays.md#value-kinds) | `JsonElement`, `JsonNode` | a JSON string compared with a string operator, e.g. `EqualTo`, `EndingWith` or `Containing(…).AtLeast(2)` |
| [`IsNumber().GreaterThan(…)`, …](./03-objects-and-arrays.md#value-kinds) | `JsonElement`, `JsonNode` | a JSON number compared as `double` with a number operator, e.g. `Between(…).And(…)` or `EqualTo(…).Within(…)` |
| [`IsNumber<TNumber>`](./03-objects-and-arrays.md#numbers)          | `JsonElement`, `JsonNode` | a JSON number that fits into the number type, e.g. an `int`, optionally equal to a value |
| [`IsBoolean`](./03-objects-and-arrays.md#value-kinds)              | `JsonElement`, `JsonNode` | a JSON boolean, i.e. `true` or `false`, optionally equal to a value or with expectations |
| [`IsNull`](./03-objects-and-arrays.md#value-kinds)                 | `JsonElement`             | the JSON value `null`                                           |
| [`IsJsonSerializable`](./04-serialization.md#json-serializable)    | `object`                  | survives a round trip through the JSON serializer               |
| [`IsJsonSerializedAs`](./04-serialization.md#serialized-as-json)   | `object`                  | serialized by the JSON serializer as the expected JSON          |

The comparisons can be relaxed with options like `TreatingNullAsAbsent()`, `IgnoringArrayOrder()`,
`IgnoringMember(name)` or `ComparingNumbersWithin(tolerance)`, see [Matching JSON](./02-matching.md).

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/):

- [JSON strings](./01-strings.md): comparing strings as JSON and validating them
- [Matching JSON](./02-matching.md): `Matches` and `MatchesExactly`, and the options to relax the comparison
- [Objects, arrays and value kinds](./03-objects-and-arrays.md): `HasProperty`, `HasValueAt`, `HasElementAt`,
  `IsObject`, `WhoseProperties`, `IsArray`, `WhoseElements`, the value kinds and `JsonNode` subjects
- [Serialization](./04-serialization.md): `IsJsonSerializable` and `IsJsonSerializedAs`
- [Customization](./05-customization.md): the default `JsonDocumentOptions` and `JsonSerializerOptions`
