# aweXpect.Json

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Json)](https://www.nuget.org/packages/aweXpect.Json)

[aweXpect.Json](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Json) contains expectations for JSON strings, `JsonElement`,
`JsonNode` and the JSON serialization of objects with `System.Text.Json`.

## Installation

```shell
dotnet add package aweXpect.Json --prerelease
```

The package brings `Expect.That` with it, but not the expectations for strings, numbers or collections, so reference
[`aweXpect`](https://www.nuget.org/packages/aweXpect) as well, e.g. for the `IsEqualTo` of `IsEqualTo(…).AsJson()`.
The expectations live in the namespace `aweXpect`, so `using aweXpect;` is all you need; only the
[customization](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/customization) lives in `aweXpect.Json`.

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

```text
Expected that json
is valid JSON which matches new { title = "Let It Be" },
but it differed as $.title was "Yesterday" instead of "Let It Be"

JSON options:
 - ignore additional properties
```

## Overview

Expectations for [`string`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/strings):

| Expectation                                                                                                     | Negated                    | Summary                                                         |
|-----------------------------------------------------------------------------------------------------------------|----------------------------|-----------------------------------------------------------------|
| [`IsEqualTo(…).AsJson()`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/strings#string-comparison-as-json) | `IsNotEqualTo(…).AsJson()` | equal to the expected JSON, compared by structure               |
| [`IsValidJson`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/strings#validation)                 | `IsNotValidJson`           | can be parsed as JSON, optionally with expectations on its root |
| [`IsValidJsonMatching`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/strings#validation)         |                            | valid JSON that matches the expected value                      |
| [`IsValidJsonMatchingExactly`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/strings#validation)  |                            | valid JSON that matches the expected value exactly              |

Expectations for `JsonElement` and [`JsonNode`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/json-nodes):

| Expectation                                                                                                              | Negated               | Summary                                                                                                      |
|--------------------------------------------------------------------------------------------------------------------------|-----------------------|--------------------------------------------------------------------------------------------------------------|
| [`Matches`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/matching)                                        |                       | matches the expected value, additional properties allowed                                                    |
| [`MatchesExactly`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/matching)                                 |                       | matches the expected value exactly                                                                           |
| [`HasProperty`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#properties)               | `DoesNotHaveProperty` | a JSON object with the property, optionally with expectations                                                |
| [`HasValueAt`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#values-at-a-path)          | `DoesNotHaveValueAt`  | a value at a path, optionally with expectations                                                              |
| [`HasElementAt`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#elements)                |                       | a JSON array with an element at the index, optionally with expectations                                      |
| [`IsObject`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#isobject)                    |                       | a JSON object                                                                                                |
| [`IsObject().WhoseProperties`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#isobject)  |                       | collection expectations on the properties of a JSON object                                                   |
| [`IsArray`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#isarray)                      |                       | a JSON array                                                                                                 |
| [`IsArray().WhoseElements`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#isarray)      |                       | collection expectations on the elements of a JSON array                                                      |
| [`IsString`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#value-kinds)                 |                       | a JSON string, optionally equal to a value or with expectations                                              |
| [`IsNumber`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#value-kinds)                 |                       | a JSON number, optionally with expectations on its value                                                     |
| [`IsString().StartingWith(…)`, …](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#value-kinds) |                  | a JSON string compared with a string operator, e.g. `EqualTo`, `EndingWith` or `Containing(…).AtLeast(2)`    |
| [`IsNumber().GreaterThan(…)`, …](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#value-kinds) |                   | a JSON number compared as `double` with a number operator, e.g. `Between(…).And(…)` or `EqualTo(…).Within(…)` |
| [`IsNumber<TNumber>`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#numbers)            |                       | a JSON number that fits into the number type, e.g. an `int`, optionally equal to a value                     |
| [`IsBoolean`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#value-kinds)                |                       | a JSON boolean, i.e. `true` or `false`, optionally equal to a value or with expectations                     |
| [`IsNull`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays#value-kinds)                   |                       | the JSON value `null`, only for a `JsonElement`                                                              |

Expectations for any `object`, serialized with `System.Text.Json`:

| Expectation                                                                                                     | Negated | Summary                                                |
|-----------------------------------------------------------------------------------------------------------------|---------|--------------------------------------------------------|
| [`IsJsonSerializable`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/serialization#json-serializable)  |         | survives a round trip through the JSON serializer      |
| [`IsJsonSerializedAs`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/serialization#serialized-as-json) |         | serialized by the JSON serializer as the expected JSON |

The comparisons can be relaxed with options like `TreatingNullAsAbsent()`, `IgnoringArrayOrder()`,
`IgnoringMember(name)` or `ComparingNumbersWithin(tolerance)`, see
[Matching JSON](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/matching).

## Customization

Set a value under `Customize.aweXpect.Json()` for the current
[async flow](https://docs.testably.org/aweXpect/how-it-works/configuration#lifetimes-and-async-flows), or under
`Customize.aweXpect.Global.Json()` for
[all tests](https://docs.testably.org/aweXpect/how-it-works/configuration#global-defaults).

| Value                                                                                                | Default                      | Summary                                         |
|------------------------------------------------------------------------------------------------------|------------------------------|-------------------------------------------------|
| [`DefaultJsonDocumentOptions`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/customization)   | `AllowTrailingCommas = true` | the options to parse JSON strings               |
| [`DefaultJsonSerializerOptions`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/customization) | `AllowTrailingCommas = true` | the options to format and serialize JSON values |

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/):

- [JSON strings](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/strings): comparing strings as JSON and
  validating them
- [Matching JSON](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/matching): `Matches` and
  `MatchesExactly`, and the options to relax the comparison
- [Objects, arrays and value kinds](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/objects-and-arrays):
  `HasProperty`, `HasValueAt`, `HasElementAt`, `IsObject`, `WhoseProperties`, `IsArray`, `WhoseElements` and the
  value kinds
- [`JsonNode`](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/json-nodes): the same expectations for a
  `JsonNode` and its subtypes
- [Serialization](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/serialization): `IsJsonSerializable`
  and `IsJsonSerializedAs`
- [Customization](https://docs.testably.org/aweXpect/extensions/aweXpect.Json/customization): the default
  `JsonDocumentOptions` and `JsonSerializerOptions`
