# Dictionaries

Describes the possible expectations for dictionaries.

## Equality

You can verify that a dictionary is equal to another one. The entries are compared by key, so the order in which the
two dictionaries enumerate them does not matter:

```csharp
Dictionary<int, string> values = new() { { 42, "foo" }, { 43, "bar" } };

await Expect.That(values).IsEqualTo(new Dictionary<int, string> { { 43, "bar" }, { 42, "foo" } });
await Expect.That(values).IsNotEqualTo(new Dictionary<int, string> { { 42, "baz" } });
```

The keys are looked up through the dictionary, so its key comparer decides which keys are the same:

```csharp
Dictionary<string, int> values = new(StringComparer.OrdinalIgnoreCase) { { "foo", 42 } };

await Expect.That(values).IsEqualTo(new Dictionary<string, int> { { "FOO", 42 } });
```

To compare the entries in their enumeration order instead, compare them as a collection of
`KeyValuePair<TKey, TValue>`:

```csharp
SortedDictionary<int, string> values = new() { { 42, "foo" }, { 43, "bar" } };

await Expect.That(values.AsEnumerable())
    .IsEqualTo([new KeyValuePair<int, string>(42, "foo"), new KeyValuePair<int, string>(43, "bar")]);
```

## Entry

You can verify that a dictionary contains the `expected` entry, which is looked up by its key, too:

```csharp
Dictionary<int, string> values = new() { { 42, "foo" }, { 43, "bar" } };

await Expect.That(values).Contains(42, "foo");
await Expect.That(values).DoesNotContain(42, "bar");
```

The entry can also be given as a `KeyValuePair<TKey, TValue>`:

```csharp
Dictionary<int, string> values = new() { { 42, "foo" }, { 43, "bar" } };

await Expect.That(values).Contains(new KeyValuePair<int, string>(42, "foo"));
```

## Keys

You can verify that a dictionary contains the `expected` key(s):

```csharp
Dictionary<int, string> values = new() { { 42, "foo" }, { 43, "bar" } };

await Expect.That(values).ContainsKey(42);
await Expect.That(values).ContainsKeys(42, 43);
await Expect.That(values).DoesNotContainKey(44);
await Expect.That(values).DoesNotContainKeys(44, 45, 46);
```

You can add additional expectations on the corresponding value(s). `WhoseValues` is a collection of the values
for the expected keys, so all collection expectations are available:

```csharp
Dictionary<int, string> values = new() { { 42, "foo" }, { 43, "bar" }, { 44, "baz" } };

await Expect.That(values).ContainsKey(42).WhoseValue.IsEqualTo("foo");
await Expect.That(values).ContainsKeys(43, 44).WhoseValues.IsEqualTo(["bar", "baz"]);
await Expect.That(values).ContainsKeys(43, 44).WhoseValues.Contains("bar");
await Expect.That(values).ContainsKeys(43, 44).WhoseValues.All().ComplyWith(v => v.StartsWith("ba"));
```

## Values

You can verify that a dictionary contains the `expected` value(s):

```csharp
Dictionary<int, string> values = new() { { 42, "foo" }, { 43, "bar" } };

await Expect.That(values).ContainsValue("foo");
await Expect.That(values).ContainsValues("foo", "bar");
await Expect.That(values).DoesNotContainValue("something");
await Expect.That(values).DoesNotContainValues("something", "else");
```

The values are compared with the same equality options as the items of a collection, and the expected keys or
values can also be given as a collection:

```csharp
Dictionary<int, string> values = new() { { 42, "foo" }, { 43, "bar" } };
string[] expected = ["FOO", "BAR"];

await Expect.That(values).ContainsValue("FOO").IgnoringCase();
await Expect.That(values).ContainsValues(expected).IgnoringCase();
await Expect.That(values).ContainsKeys(new List<int> { 42, 43 });
```

`DoesNotContainKeys` and `DoesNotContainValues` mean "none of": they fail as soon as the dictionary contains one of
them. This is stricter than negating `ContainsKeys` or `ContainsValues` with `DoesNotComplyWith`, which is the exact
inverse ("not all") and only fails when the dictionary contains all of them:

```csharp
Dictionary<int, string> values = new() { { 42, "foo" }, { 43, "bar" } };

await Expect.That(values).DoesNotComplyWith(d => d.ContainsKeys(42, 44))
  .Because("the key 44 is missing, although 42 is contained");
await Expect.That(values).DoesNotContainKeys(44, 45)
  .Because("none of the keys is contained, whereas `DoesNotContainKeys(42, 44)` would fail");
```

## Keys and values

You can continue with expectations on the keys or on the values of a dictionary:

```csharp
Dictionary<int, string> values = new() { { 42, "foo" }, { 43, "bar" } };

await Expect.That(values).Keys.Contains(42);
await Expect.That(values).Values.All().AreUnique();
```

The keys keep the key comparer of the dictionary, so e.g. `Keys.Contains("FOO")` succeeds for a key `"foo"` in a
dictionary created with `StringComparer.OrdinalIgnoreCase`.

The keys of a dictionary are unique by design, so its uniqueness is decided by the values alone.
