---
description: Verify JSON strings, JsonElement and JsonNode values and the System.Text.Json serialization of objects with aweXpect.
---

import PackageInstall from '@site/src/components/PackageInstall';

# aweXpect.Json

[aweXpect.Json](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Json) extends [aweXpect](/aweXpect) with expectations for JSON strings, `JsonElement`, `JsonNode` and the JSON serialization of objects with `System.Text.Json`.

## Installation

<PackageInstall package="aweXpect.Json" prerelease />

The package brings `Expect.That` with it, but not the expectations for strings, numbers or collections, so reference
[`aweXpect`](https://www.nuget.org/packages/aweXpect) as well, e.g. for the `IsEqualTo` of `IsEqualTo(…).AsJson()`.
The expectations live in the namespace `aweXpect`, so `using aweXpect;` is all you need; only the
[customization](./06-customization.md) lives in `aweXpect.Json`.

The examples write `That(...)` instead of `Expect.That(...)`, which needs `using static aweXpect.Expect;` (see [Getting started](/aweXpect/getting-started#install-the-package)).

## At a glance

```csharp
string json = """{"title": "Let It Be", "artist": "The Beatles", "durationSeconds": 243}""";

// compare JSON strings by their structure instead of their formatting
string expected = """{ "artist": "The Beatles", "title": "Let It Be", "durationSeconds": 243 }""";
await That(json).IsEqualTo(expected).AsJson();

// verify that a string is valid JSON that matches an expected value
await That(json).IsValidJsonMatching(new { title = "Let It Be" });

// verify single values of a JsonElement or JsonNode
JsonElement track = JsonDocument.Parse(json).RootElement;
await That(track)
    .HasProperty("title", t => t.IsString("Let It Be")).And
    .HasProperty("durationSeconds", d => d.IsNumber<int>()).And
    .DoesNotHaveProperty("password");

// verify the JSON contract of a type, e.g. `record Track(string Title, string Artist)`
await That(new Track("Let It Be", "The Beatles")).IsJsonSerializedAs(
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

Expectations for [`string`](./01-strings.md):

| Expectation                                                          | Negated                    | Summary                                                         |
|----------------------------------------------------------------------|----------------------------|-----------------------------------------------------------------|
| [`IsEqualTo(…).AsJson()`](./01-strings.md#string-comparison-as-json) | `IsNotEqualTo(…).AsJson()` | equal to the expected JSON, compared by structure               |
| [`IsValidJson`](./01-strings.md#validation)                          | `IsNotValidJson`           | can be parsed as JSON, optionally with expectations on its root |
| [`IsValidJsonMatching`](./01-strings.md#validation)                  |                            | valid JSON that matches the expected value                      |
| [`IsValidJsonMatchingExactly`](./01-strings.md#validation)           |                            | valid JSON that matches the expected value exactly              |

Expectations for `JsonElement` and [`JsonNode`](./04-json-nodes.md):

| Expectation                                                               | Negated               | Summary                                                                                                      |
|---------------------------------------------------------------------------|-----------------------|--------------------------------------------------------------------------------------------------------------|
| [`Matches`](./02-matching.md)                                             |                       | matches the expected value, additional properties allowed                                                    |
| [`MatchesExactly`](./02-matching.md)                                      |                       | matches the expected value exactly                                                                           |
| [`HasProperty`](./03-objects-and-arrays.md#properties)                    | `DoesNotHaveProperty` | a JSON object with the property, optionally with expectations                                                |
| [`HasValueAt`](./03-objects-and-arrays.md#values-at-a-path)               | `DoesNotHaveValueAt`  | a value at a path, optionally with expectations                                                              |
| [`HasElementAt`](./03-objects-and-arrays.md#elements)                     |                       | a JSON array with an element at the index, optionally with expectations                                      |
| [`IsObject`](./03-objects-and-arrays.md#isobject)                         |                       | a JSON object                                                                                                |
| [`IsObject().WhoseProperties`](./03-objects-and-arrays.md#isobject)       |                       | collection expectations on the properties of a JSON object                                                   |
| [`IsArray`](./03-objects-and-arrays.md#isarray)                           |                       | a JSON array                                                                                                 |
| [`IsArray().WhoseElements`](./03-objects-and-arrays.md#isarray)           |                       | collection expectations on the elements of a JSON array                                                      |
| [`IsString`](./03-objects-and-arrays.md#value-kinds)                      |                       | a JSON string, optionally equal to a value or with expectations                                              |
| [`IsNumber`](./03-objects-and-arrays.md#value-kinds)                      |                       | a JSON number, optionally with expectations on its value                                                     |
| [`IsString().StartingWith(…)`, …](./03-objects-and-arrays.md#value-kinds) |                       | a JSON string compared with a string operator, e.g. `EqualTo`, `EndingWith` or `Containing(…).AtLeast(2)`    |
| [`IsNumber().GreaterThan(…)`, …](./03-objects-and-arrays.md#value-kinds)  |                       | a JSON number compared as `double` with a number operator, e.g. `Between(…).And(…)` or `EqualTo(…).Within(…)` |
| [`IsNumber<TNumber>`](./03-objects-and-arrays.md#numbers)                 |                       | a JSON number that fits into the number type, e.g. an `int`, optionally equal to a value                     |
| [`IsBoolean`](./03-objects-and-arrays.md#value-kinds)                     |                       | a JSON boolean, i.e. `true` or `false`, optionally equal to a value or with expectations                     |
| [`IsNull`](./03-objects-and-arrays.md#value-kinds)                        |                       | the JSON value `null`, only for a `JsonElement`                                                              |

Expectations for any `object`, serialized with `System.Text.Json`:

| Expectation                                                      | Negated | Summary                                                |
|------------------------------------------------------------------|---------|--------------------------------------------------------|
| [`IsJsonSerializable`](./05-serialization.md#json-serializable)  |         | survives a round trip through the JSON serializer      |
| [`IsJsonSerializedAs`](./05-serialization.md#serialized-as-json) |         | serialized by the JSON serializer as the expected JSON |

The comparisons can be relaxed with options like `TreatingNullAsAbsent()`, `IgnoringArrayOrder()`,
`IgnoringMember(name)` or `ComparingNumbersWithin(tolerance)`, see [Matching JSON](./02-matching.md).

## Customization

Set a value under `Customize.aweXpect.Json()` for the current
[async flow](/aweXpect/how-it-works/configuration#lifetimes-and-async-flows), or under
`Customize.aweXpect.Global.Json()` for
[all tests](/aweXpect/how-it-works/configuration#global-defaults).

| Value                                                   | Default                      | Summary                                         |
|---------------------------------------------------------|------------------------------|-------------------------------------------------|
| [`DefaultJsonDocumentOptions`](./06-customization.md)   | `AllowTrailingCommas = true` | the options to parse JSON strings               |
| [`DefaultJsonSerializerOptions`](./06-customization.md) | `AllowTrailingCommas = true` | the options to format and serialize JSON values |

## Documentation

- [JSON strings](./01-strings.md): comparing strings as JSON and validating them
- [Matching JSON](./02-matching.md): `Matches` and `MatchesExactly`, and the options to relax the comparison
- [Objects, arrays and value kinds](./03-objects-and-arrays.md): `HasProperty`, `HasValueAt`, `HasElementAt`,
  `IsObject`, `WhoseProperties`, `IsArray`, `WhoseElements` and the value kinds
- [`JsonNode`](./04-json-nodes.md): the same expectations for a `JsonNode` and its subtypes
- [Serialization](./05-serialization.md): `IsJsonSerializable` and `IsJsonSerializedAs`
- [Customization](./06-customization.md): the default `JsonDocumentOptions` and `JsonSerializerOptions`
