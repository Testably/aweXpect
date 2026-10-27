# Serialization

Describes the possible expectations for the JSON serialization of objects with `System.Text.Json`.

| Expectation                                 | Negated | Summary                                                |
|---------------------------------------------|---------|--------------------------------------------------------|
| [`IsJsonSerializable`](#json-serializable)  |         | survives a round trip through the JSON serializer      |
| [`IsJsonSerializedAs`](#serialized-as-json) |         | serialized by the JSON serializer as the expected JSON |

## JSON serializable

You can verify that an object is JSON serializable:

```csharp
public record Track(string Title, string Artist);

Track track = new("Let It Be", "The Beatles");

await Expect.That(track).IsJsonSerializable();
```

This verifies that the object can be serialized to JSON and deserialized again, and that the result is
[equivalent](https://docs.testably.org/aweXpect/values/equivalency) to the original object. For example, a property
with a private setter loses its value:

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

Equivalency options:
 - include public fields and properties
```

As with `IsEquivalentTo`, the failure lists the equivalency options, and a member to ignore that matched no member,
e.g. a misspelled `IgnoringMember("PlayCont")`, gets a hint with the closest member.

You can specify both the
[`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions) and the
[equivalency options](https://docs.testably.org/aweXpect/values/equivalency#configuration):

```csharp
await Expect.That(track).IsJsonSerializable(
    new JsonSerializerOptions { IncludeFields = true },
    e => e.IgnoringMember(nameof(Track.Artist)));
```

Without `JsonSerializerOptions`, the subject is serialized with the default options of the
[customization](./06-customization.md).

<details>
<summary>Runtime type and `null` subjects</summary>

The subject is serialized and deserialized as its runtime type, so that the members of a derived type survive the
round trip as well, and keeps its declared type for further expectations after `.And`. A `null` subject fails the
expectation.

</details>

## Serialized as JSON

You can verify the JSON that an object is serialized to by `System.Text.Json`, e.g. to test the contract of a public
API:

```csharp
public enum Genre { Pop = 1, Rock = 2 }
public record Track(string Title, Genre Genre, string? Album = null);

Track track = new("Let It Be", Genre.Rock);
JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web)
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
};

await Expect.That(track).IsJsonSerializedAs("""{"title": "Let It Be", "genre": "rock"}""", serializerOptions);
await Expect.That(track).IsJsonSerializedAs(new { title = "Let It Be", genre = "rock" }, serializerOptions);
```

With `new JsonSerializerOptions(JsonSerializerDefaults.Web)` instead, the first expectation would fail with:

```text title="Failure message"
Expected that track
is serialized as JSON {"title": "Let It Be", "genre": "rock"},
but it differed as
  $.genre was number 2 instead of "rock" and
  $.album had unexpected <null>

Serialized:
{"title":"Let It Be","genre":2,"album":null}
```

Unlike [`IsJsonSerializable`](#json-serializable), which verifies that an object survives a round trip through the
serializer, `IsJsonSerializedAs` verifies the JSON itself: the property names and their casing, the representation of
enums, omitted `null` values and the effect of converters.

The expected value is either JSON text or an object that is compared like in [`Matches`](./02-matching.md),
including `It.Is…` expectations. Without `JsonSerializerOptions`, the subject is serialized with the default options of
the [customization](./06-customization.md). A `null` subject is serialized as `null`, and a subject that cannot be
serialized, e.g. because of a reference cycle, fails the expectation with the message of the exception.

The subject and an expected object are serialized on two separate paths:

| Value              | Serialized with                                                          | Without them                              |
|--------------------|--------------------------------------------------------------------------|-------------------------------------------|
| the subject        | the `JsonSerializerOptions` or `JsonTypeInfo<T>` parameter               | the default options of the customization  |
| an expected object | [`o => o.WithSerializerOptions(…)`](./02-matching.md#serializer-options) | the default options of `System.Text.Json` |

So the `JsonSerializerOptions` parameter only serializes the subject, and an expected object is serialized neither
with this parameter nor with the customized default options, so that a naming policy cannot hide how the subject is
actually serialized. To serialize the expected object with other options, pass them with `WithSerializerOptions(…)`:

```csharp
JsonSerializerOptions camelCase = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

await Expect.That(track).IsJsonSerializedAs(new { Title = "Let It Be", Genre = 2, Album = (string?)null },
    camelCase, o => o.WithSerializerOptions(camelCase));
```

The serialized JSON is compared exactly, like in `MatchesExactly`, so additional properties fail the expectation. All
[`JsonOptions`](./02-matching.md) are available to relax the comparison, e.g. `IgnoringAdditionalProperties()`,
`IgnoringArrayOrder()`, `TreatingNullAsAbsent()` or `IgnoringMember(name)`:

```csharp
await Expect.That(track).IsJsonSerializedAs(new { title = "Let It Be" }, serializerOptions,
    o => o.IgnoringAdditionalProperties());
```

:::note[Trimming and Native AOT]
Unless a `JsonTypeInfo<T>` is passed, the subject is serialized by reflection, which is switched off when publishing
with trimming or [Native AOT](https://docs.testably.org/aweXpect/setup/native-aot). In such an application, pass
the source-generated `JsonTypeInfo<T>` of the subject together with the expected value as a JSON string:

```csharp
[JsonSerializable(typeof(Track))]
internal partial class TrackContext : JsonSerializerContext;

await Expect.That(track).IsJsonSerializedAs("""{"Title": "Let It Be", "Genre": 2}""",
    TrackContext.Default.Track, o => o.TreatingNullAsAbsent());
```
:::

<details>
<summary>More on trimming and Native AOT</summary>

`IsJsonSerializable` takes the `JsonTypeInfo<T>` as well, and then serializes and deserializes the subject as `T`. It
also compares the deserialized object with the subject by
[equivalency](https://docs.testably.org/aweXpect/setup/native-aot#equivalency), which needs the members of the
subject type registered. The source generator of aweXpect registers them at the call site, but it cannot see the
properties that the `System.Text.Json` source generator adds to the context. Pass the type info from a variable of type
`JsonTypeInfo<T>` instead, or name the type in `[assembly: GenerateMetadata(typeof(Track))]`:

```csharp
JsonTypeInfo<Track> typeInfo = TrackContext.Default.Track;

await Expect.That(track).IsJsonSerializable(typeInfo);
```

Expected values in any expectation, e.g. in `Matches` or `IsValidJsonMatching`, are serialized by reflection as
well, so an anonymous object always needs it. Only values that consist of `null`, strings, characters, booleans,
numbers, `JsonElement`, `JsonDocument`, `JsonNode` (unless a `JsonValue` wraps another type), `It.Is…` expectations
and collections of them are written without reflection and work in such an application, e.g. `Matches(2)` or
`Matches(new[] { 1, 2 })`. The expectations on properties and values, like `HasProperty("count", c => c.IsNumber<int>())`,
need no serialization at all. A named type works when it is registered in a source-generated
context, whose options are passed with [`WithSerializerOptions(…)`](./02-matching.md#serializer-options):

```csharp
await Expect.That(element).Matches(new Track("Let It Be", Genre.Rock),
    o => o.WithSerializerOptions(TrackContext.Default.Options));
```

</details>
