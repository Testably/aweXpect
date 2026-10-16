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
```

You can specify both the
[`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions) and the
[equivalency options](https://docs.testably.org/aweXpect/values/equivalency#configuration):

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

The expected value is either a JSON string, which must be valid JSON (otherwise an `ArgumentException` is thrown), or
an object that is compared like in [`Matches`](./02-matching.md), including `It.Is…` expectations. Without
`JsonSerializerOptions`, the subject is serialized with the default options of the
[customization](./05-customization.md). A `null` subject is serialized as `null`, and a subject that cannot be
serialized, e.g. because of a reference cycle, fails the expectation with the message of the exception.

The serialized JSON is compared exactly, like in `MatchesExactly`, so additional properties fail the expectation. All
[`JsonOptions`](./02-matching.md) are available to relax the comparison, e.g. `IgnoringAdditionalProperties()`,
`IgnoringArrayOrder()`, `TreatingNullAsAbsent()` or `IgnoringMember(name)`:

```csharp
await Expect.That(track).IsJsonSerializedAs(new { title = "Let It Be" }, serializerOptions,
    o => o.IgnoringAdditionalProperties());
```

:::note[Trimming and Native AOT]
Unless a `JsonTypeInfo<T>` is passed, the subject is serialized by reflection, which is switched off when publishing
with trimming or [Native AOT](https://docs.testably.org/aweXpect/how-it-works/native-aot). In such an application, pass
the source-generated `JsonTypeInfo<T>` of the subject together with the expected value as a JSON string:

```csharp
[JsonSerializable(typeof(Track))]
internal partial class TrackContext : JsonSerializerContext;

await Expect.That(track).IsJsonSerializedAs("""{"Title": "Let It Be", "Genre": 2}""", TrackContext.Default.Track,
    o => o.TreatingNullAsAbsent());
```
:::
