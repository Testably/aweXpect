# aweXpect.Json

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Json)](https://www.nuget.org/packages/aweXpect.Json)
[![Build](https://github.com/Testably/aweXpect.Json/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Json/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Json&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Json)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Json&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Json)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Json%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Json/main)

Expectations for JSON strings and `System.Text.Json` for [aweXpect](https://github.com/Testably/aweXpect).

## Overview

| Expectation                                             | Subject       | Summary                                                   |
|---------------------------------------------------------|---------------|-----------------------------------------------------------|
| [`IsEqualTo(…).AsJson()`](#string-comparison-as-json)   | `string`      | equal to the expected JSON, compared by structure         |
| [`IsValidJson`](#validation)                            | `string`      | can be parsed as JSON                                     |
| [`IsValidJsonMatching`](#validation)                    | `string`      | valid JSON that matches the expected value                |
| [`IsValidJsonMatchingExactly`](#validation)             | `string`      | valid JSON that matches the expected value exactly        |
| [`Matches`](#matches)                                   | `JsonElement` | matches the expected value, additional properties allowed |
| [`MatchesExactly`](#matches)                            | `JsonElement` | matches the expected value exactly                        |
| [`IsObject`](#isobject)                                 | `JsonElement` | a JSON object, optionally with expectations on it         |
| [`IsArray`](#isarray)                                   | `JsonElement` | a JSON array, optionally with expectations on it          |
| [`IsJsonSerializable`](#json-serializable)              | `object`      | survives a round trip through the JSON serializer         |

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
structure, `AsJson()` cannot be combined with `IgnoringCase()` or `Using(comparer)`.

`AsJson()` is not available on `Contains` or `DoesNotContain` for a string subject, as parts of a JSON string are
usually not valid JSON. Use `IsEqualTo(expected).AsJson()` or `IsValidJsonMatching(expected)` instead. On a collection
of strings, `Contains(expected).AsJson()` compares each item as JSON.

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

await Expect.That(track).IsValidJson(o => o with { CommentHandling = JsonCommentHandling.Disallow });
```

You can add further expectations on the
[`JsonElement`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement) parsed from the subject:

```csharp
string track = """{"title": "Let It Be"}""";

await Expect.That(track).IsValidJson().Which(j => j.Matches(new { title = "Let It Be" }));
```

Or you can verify that the string is valid JSON and [matches](#matches) an expected value in one step:

```csharp
string track = """{"title": "Let It Be", "artist": "The Beatles"}""";

await Expect.That(track).IsValidJsonMatching(new { title = "Let It Be" });
await Expect.That(track).IsValidJsonMatchingExactly(new { title = "Let It Be", artist = "The Beatles" });
```

`IsValidJsonMatching(new { title = "Yesterday" })` would fail with:

```text title="Failure message"
Expected that track
is valid JSON which matches new { title = "Yesterday" },
but it differed as $.title was "Let It Be" instead of "Yesterday"
```

## `JsonElement`

### Matches

You can verify that a `JsonElement` matches an expected object. `Matches` ignores additional properties, while
`MatchesExactly` also fails for them:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""").RootElement;

await Expect.That(track).Matches(new { title = "Let It Be" });
await Expect.That(track).MatchesExactly(new { title = "Let It Be", artist = "The Beatles" });
```

`MatchesExactly(new { title = "Let It Be" })` would fail because of the additional `artist` property:

```text title="Failure message"
Expected that track
matches new { title = "Let It Be" } exactly,
but it differed as $.artist had unexpected "The Beatles"
```

You can verify that a `JsonElement` matches an expected array:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday", "Hey Jude"]""").RootElement;

await Expect.That(titles).Matches(["Let It Be", "Yesterday"]);
await Expect.That(titles).MatchesExactly(["Let It Be", "Yesterday", "Hey Jude"]);
```

You can also verify that a `JsonElement` matches a primitive value:

```csharp
await Expect.That(JsonDocument.Parse("\"Let It Be\"").RootElement).Matches("Let It Be");
await Expect.That(JsonDocument.Parse("4.05").RootElement).Matches(4.05);
await Expect.That(JsonDocument.Parse("true").RootElement).Matches(true);
await Expect.That(JsonDocument.Parse("null").RootElement).Matches(null);
```

### IsObject

You can verify that a `JsonElement` is a JSON object that satisfies some expectations:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""").RootElement;

await Expect.That(track).IsObject(o => o
    .With("title").Matching("Let It Be").And
    .With("artist").Matching("The Beatles"));
```

With `.With("artist").Matching("The Rolling Stones")` instead, the expectation would fail with:

```text title="Failure message"
Expected that track
is an object and $.title matches "Let It Be" and $.artist matches "The Rolling Stones",
but it differed as $.artist was "The Beatles" instead of "The Rolling Stones"
```

You can verify that a property is another object, recursively:

```csharp
JsonElement track = JsonDocument.Parse(
    """{"title": "Let It Be", "album": {"title": "Let It Be", "year": 1970}}""").RootElement;

await Expect.That(track).IsObject(o => o
    .With("album").AnObject(album => album
        .With("year").Matching(1970)));
```

You can verify that a property is an array:

```csharp
JsonElement album = JsonDocument.Parse(
    """{"title": "Abbey Road", "tracks": ["Come Together", "Something"]}""").RootElement;

await Expect.That(album).IsObject(o => o
    .With("tracks").AnArray(tracks => tracks.WithElements("Come Together", "Something")));
```

You can verify the number of properties in a JSON object:

```csharp
JsonElement track = JsonDocument.Parse("""{"title": "Let It Be", "artist": "The Beatles"}""").RootElement;

await Expect.That(track).IsObject(o => o.With(2).Properties());
```

### IsArray

You can verify that a `JsonElement` is a JSON array that satisfies some expectations:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday"]""").RootElement;

await Expect.That(titles).IsArray(a => a
    .At(0).Matching("Let It Be").And
    .At(1).Matching("Yesterday"));
```

You can verify the number of elements in a JSON array:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday"]""").RootElement;

await Expect.That(titles).IsArray(a => a.With(2).Elements());
```

You can match the expected elements of an array directly:

```csharp
JsonElement titles = JsonDocument.Parse("""["Let It Be", "Yesterday"]""").RootElement;

await Expect.That(titles).IsArray(a => a.WithElements("Let It Be", "Yesterday"));
```

With `WithElements("Let It Be", "Hey Jude")` instead, the expectation would fail with:

```text title="Failure message"
Expected that titles
is an array and $[0] matches "Let It Be" and $[1] matches "Hey Jude",
but it differed as $[1] was "Yesterday" instead of "Hey Jude"
```

You can match nested arrays recursively (use `null` to skip an element):

```csharp
JsonElement sides = JsonDocument.Parse(
    """[["Come Together", "Something"], ["Here Comes the Sun"], ["The End", "Her Majesty"]]""").RootElement;

await Expect.That(sides).IsArray(a => a
    .WithArrays(
        side => side.WithElements("Come Together", "Something"),
        null,
        side => side.At(1).Matching("Her Majesty")));
```

You can match objects in an array recursively (use `null` to skip an element):

```csharp
JsonElement tracks = JsonDocument.Parse(
    """
    [
      {"title": "Let It Be"},
      {"title": "Yesterday"},
      {"title": "Hey Jude", "durationSeconds": 431}
    ]
    """).RootElement;

await Expect.That(tracks).IsArray(a => a
    .WithObjects(
        track => track.With("title").Matching("Let It Be"),
        null,
        track => track.With(2).Properties()));
```

## JSON serializable

You can verify that an object is JSON serializable:

```csharp
public record Track(string Title, string Artist);

Track track = new("Let It Be", "The Beatles");

await Expect.That(track).IsJsonSerializable();
```

This verifies that the object can be serialized to JSON and deserialized again, and that the result is equivalent to
the original object. For example, a property with a private setter loses its value:

```csharp
public class PlayCounter
{
    public string Title { get; set; } = "";
    public int PlayCount { get; private set; }
    public void Play() => PlayCount++;
}

PlayCounter counter = new() { Title = "Let It Be" };
counter.Play();

await Expect.That(counter).IsJsonSerializable();
```

```text title="Failure message"
Expected that counter
is serializable as JSON,
but it was not:
  Property PlayCount differed:
      Actual: 0
    Expected: 1
```

You can specify both the
[`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions) and the
equivalency options:

```csharp
await Expect.That(track).IsJsonSerializable(
    new JsonSerializerOptions { IncludeFields = true },
    e => e.IgnoringMember(nameof(Track.Artist)));
```

You can also specify the type that the subject is deserialized into:

```csharp
object track = new Track("Let It Be", "The Beatles");

await Expect.That(track).IsJsonSerializable<Track>(
    new JsonSerializerOptions { IncludeFields = true },
    e => e.IgnoringMember(nameof(Track.Artist)));
```

## Customization

You can change the default
[`JsonDocumentOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions), used to
parse JSON strings, and the default
[`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions), used to
format JSON values. Both allow trailing commas by default. Each value is set on its own and restored when the returned
lifetime is disposed:

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
