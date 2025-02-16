# aweXpect.Json

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Json)](https://www.nuget.org/packages/aweXpect.Json)
[![Build](https://github.com/aweXpect/aweXpect.Json/actions/workflows/build.yml/badge.svg)](https://github.com/aweXpect/aweXpect.Json/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.Json&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=aweXpect_aweXpect.Json)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.Json&metric=coverage)](https://sonarcloud.io/summary/new_code?id=aweXpect_aweXpect.Json)

Extensions on System.Text.Json for [aweXpect](https://github.com/aweXpect/aweXpect).

## String comparison as JSON

You can compare two strings for JSON equivalency:

```csharp
string subject = "{\"foo\":{\"bar\":[1,2,3]}}";
string expected = """
                  {
                    "foo": {
                      "bar": [ 1, 2, 3 ]
                    }
                  }
                  """;

await Expect.That(subject).Is(expected).AsJson();
```

## Validation

You can verify, that a string is valid JSON.

```csharp
string subject = "{\"foo\": 2}";

await Expect.That(subject).IsValidJson();
```

This verifies that the string can be parsed by [
`JsonDocument.Parse`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocument.parse) without
exceptions.

You can also specify the [
`JsonDocumentOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions):

```csharp
string subject = "{\"foo\": 2}";

await Expect.That(subject).IsValidJson(o => o with {CommentHandling = JsonCommentHandling.Disallow});
```

You can also add additional expectations on the [
`JsonElement`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement) created when parsing the
subject:

```csharp
string subject = "{\"foo\": 2}";

await Expect.That(subject).IsValidJson().Which(j => j.Matches(new{foo = 2}));
```

## `JsonElement`

### Match

You can verify, that the `JsonElement` matches an expected object:

```csharp
JsonElement subject = JsonDocument.Parse("{\"foo\": 1, \"bar\": \"baz\"}").RootElement;

await Expect.That(subject).Matches(new{foo = 1});
await Expect.That(subject).MatchesExactly(new{foo = 1, bar = "baz"});
```

You can verify, that the `JsonElement` matches an expected array:

```csharp
JsonElement subject = JsonDocument.Parse("[1,2,3]").RootElement;

await Expect.That(subject).Matches([1, 2]);
await Expect.That(subject).MatchesExactly([1, 2, 3]);
```

You can also verify, that the `JsonElement` matches a primitive type:

```csharp
await Expect.That(JsonDocument.Parse("\"foo\"").RootElement).Matches("foo");
await Expect.That(JsonDocument.Parse("42.3").RootElement).Matches(42.3);
await Expect.That(JsonDocument.Parse("true").RootElement).Matches(true);
await Expect.That(JsonDocument.Parse("null").RootElement).Matches(null);
```

### Be object

You can verify that a `JsonElement` is a JSON object that satisfy some expectations:

```csharp
JsonElement subject = JsonDocument.Parse("{\"foo\": 1, \"bar\": \"baz\"}").RootElement;

await Expect.That(subject).IsObject(o => o
    .With("foo").Matching(1).And
    .With("bar").Matching("baz"));
```

You can also verify that a property is another object recursively:

```csharp
JsonElement subject = JsonDocument.Parse("{\"foo\": {\"bar\": \"baz\"}}").RootElement;

await Expect.That(subject).IsObject(o => o
    .With("foo").AnObject(i => i
        .With("bar").Matching("baz")));
```

You can also verify that a property is an array:

```csharp
JsonElement subject = JsonDocument.Parse("{\"foo\": [1, 2]}").RootElement;

await Expect.That(subject).IsObject(o => o
    .With("foo").AnArray(a => a.WithElements(1, 2)));
```

You can also verify the number of properties in a JSON object:

```csharp
JsonElement subject = JsonDocument.Parse("{\"foo\": 1, \"bar\": \"baz\"}").RootElement;

await Expect.That(subject).IsObject(o => o.With(2).Properties());
```

### Be array

You can verify that a `JsonElement` is a JSON array that satisfy some expectations:

```csharp
JsonElement subject = JsonDocument.Parse("[\"foo\",\"bar\"]").RootElement;

await Expect.That(subject).IsArray(a => a
    .At(0).Matching("foo").And
    .At(1).Matching("bar"));
```

You can also verify the number of elements in a JSON array:

```csharp
JsonElement subject = JsonDocument.Parse("[1, 2, 3]").RootElement;

await Expect.That(subject).IsArray(o => o.With(3).Elements());
```

You can also directly match the expected values of an array:

```csharp
JsonElement subject = JsonDocument.Parse("[\"foo\",\"bar\"]").RootElement;

await Expect.That(subject).IsArray(a => a
    .WithElements("foo", "bar"));
```

You can also match sub-arrays recursively (add `null` to skip an element):

```csharp
JsonElement subject = JsonDocument.Parse("[[0,1,2],[1,2,3],[2,3,4],[3,4,5,6]]").RootElement;

await Expect.That(subject).IsArray(a => a
    .WithArrays(
        i => i.WithElements(0,1,2),
        i => i.At(0).Matching(1).And.At(2).Matching(3),
        null,
        i => i.With(4).Elements()
    ));
```

You can also match objects recursively (add `null` to skip an element):

```csharp
JsonElement subject = JsonDocument.Parse(
	"""
	[
	  {"foo":1},
	  {"bar":2},
	  {"bar": null, "baz": true}
	]
	""").RootElement;
await Expect.That(subject).IsArray(a => a
	.WithObjects(
		i => i.With("foo").Matching(1),
		null,
		i => i.With(2).Properties()
	));
```

## JSON serializable

You can verify that an `object` is JSON serializable:

```csharp
MyClass subject = new MyClass();

await Expect.That(subject).IsJsonSerializable();
```

This validates, that the `MyClass` can be serialized and deserialized to/from JSON and that the result is equivalent to
the original subject.

You can specify both, the [
`JsonSerializerOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions) and the
equivalency options:

```csharp
MyClass subject = new MyClass();

await Expect.That(subject).IsJsonSerializable(
    new JsonSerializerOptions { IncludeFields = true },
    e => e.IgnoringMember("Foo"));
```

You can also specify an expected generic type that the subject should have:

```csharp
object subject = //...

await Expect.That(subject).IsJsonSerializable<MyClass>(
    new JsonSerializerOptions { IncludeFields = true },
    e => e.IgnoringMember("Foo"));
```
