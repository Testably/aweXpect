# Dictionaries

Describes the possible expectations for dictionaries.

| Expectation                          | Negated                | Summary                                              |
|--------------------------------------|------------------------|------------------------------------------------------|
| [`IsEqualTo`](#equality)             | `IsNotEqualTo`         | has the same entries as the expected dictionary      |
| [`Contains`](#entry)                 | `DoesNotContain`       | contains the expected entry                          |
| [`ContainsKey`](#keys)               | `DoesNotContainKey`    | contains the expected key                            |
| [`ContainsKeys`](#keys)              | `DoesNotContainKeys`   | contains all expected keys (negated: none of them)   |
| [`ContainsValue`](#values)           | `DoesNotContainValue`  | contains the expected value                          |
| [`ContainsValues`](#values)          | `DoesNotContainValues` | contains all expected values (negated: none of them) |
| [`Keys`, `Values`](#keys-and-values) |                        | continue with the keys or values as a collection     |

The samples on this page use the following dictionary:

```csharp
Dictionary<string, int> playCounts = new() { { "Let It Be", 42 }, { "Yesterday", 7 } };
```

## Equality

You can verify that a dictionary is equal to another one. The entries are compared by key, so the order in which the
two dictionaries enumerate them does not matter:

```csharp
await Expect.That(playCounts).IsEqualTo(new Dictionary<string, int> { { "Yesterday", 7 }, { "Let It Be", 42 } });
await Expect.That(playCounts).IsNotEqualTo(new Dictionary<string, int> { { "Let It Be", 41 } });
```

The keys are looked up through the dictionary, so its key comparer decides which keys are the same:

```csharp
Dictionary<string, int> ratings = new(StringComparer.OrdinalIgnoreCase) { { "Let It Be", 5 } };

await Expect.That(ratings).IsEqualTo(new Dictionary<string, int> { { "LET IT BE", 5 } });
```

The comparer is read from the dictionary types of the framework and from the dictionary that a
`ReadOnlyDictionary<TKey, TValue>` wraps, not from a custom dictionary or a `ConcurrentDictionary<TKey, TValue>` on
.NET Framework. Before .NET 10, the wrapped dictionary is only reached by reflection, so it is not read in a project
that enables trimming or Native AOT. For the others, a key of the dictionary that equals no expected key fails the
expectation, even when the comparer considers it the same as one.

To compare the entries in their enumeration order instead, compare them as a collection of
`KeyValuePair<TKey, TValue>`:

```csharp
SortedDictionary<string, int> sorted = new() { { "Let It Be", 42 }, { "Yesterday", 7 } };

await Expect.That(sorted.AsEnumerable())
    .IsEqualTo([new KeyValuePair<string, int>("Let It Be", 42), new KeyValuePair<string, int>("Yesterday", 7)]);
```

## Entry

You can verify that a dictionary contains the `expected` entry, which is looked up by its key, too:

```csharp
await Expect.That(playCounts).Contains("Let It Be", 42);
await Expect.That(playCounts).Contains(new KeyValuePair<string, int>("Yesterday", 7));
await Expect.That(playCounts).DoesNotContain("Let It Be", 7);
```

## Keys

You can verify that a dictionary contains the `expected` key(s):

```csharp
await Expect.That(playCounts).ContainsKey("Let It Be");
await Expect.That(playCounts).ContainsKeys("Let It Be", "Yesterday");
await Expect.That(playCounts).DoesNotContainKey("Help!");
await Expect.That(playCounts).DoesNotContainKeys("Help!", "Something");
```

You can add additional expectations on the corresponding value(s). `WhoseValues` is a collection of the values
for the expected keys, so all collection expectations are available:

```csharp
await Expect.That(playCounts).ContainsKey("Let It Be").WhoseValue.IsEqualTo(42);
await Expect.That(playCounts).ContainsKeys("Let It Be", "Yesterday").WhoseValues.IsEqualTo([42, 7]);
await Expect.That(playCounts).ContainsKeys("Let It Be", "Yesterday").WhoseValues.All().ComplyWith(v => v.IsPositive());
```

## Values

You can verify that a dictionary contains the `expected` value(s):

```csharp
await Expect.That(playCounts).ContainsValue(42);
await Expect.That(playCounts).ContainsValues(42, 7);
await Expect.That(playCounts).DoesNotContainValue(0);
await Expect.That(playCounts).DoesNotContainValues(0, 1);
```

The values are compared with the same equality options as the items of a collection, e.g. a
[tolerance](./index.md#tolerance) with `Within` or `IgnoringCase()` for strings, in `Contains`, `ContainsValue`,
`ContainsValues` and `IsEqualTo` and in their negations. The expected keys or values can also be given as a
collection:

```csharp
Dictionary<int, string> titles = new() { { 1, "Let It Be" }, { 2, "Yesterday" } };
Dictionary<string, double> durations = new() { { "Let It Be", 3.85 }, { "Yesterday", 2.07 } };
string[] expected = ["LET IT BE", "YESTERDAY"];

await Expect.That(titles).ContainsValue("let it be").IgnoringCase();
await Expect.That(titles).ContainsValues(expected).IgnoringCase();
await Expect.That(titles).Contains(1, "LET IT BE").IgnoringCase();
await Expect.That(durations).Contains("Yesterday", 2.0).Within(0.1);
await Expect.That(titles).ContainsKeys(new List<int> { 1, 2 });
```

`DoesNotContainKeys` and `DoesNotContainValues` mean "none of": they fail as soon as the dictionary contains one of
them. This is stricter than negating `ContainsKeys` or `ContainsValues` with `DoesNotComplyWith`, which is the exact
inverse ("not all") and only fails when the dictionary contains all of them:

```csharp
await Expect.That(playCounts).DoesNotComplyWith(d => d.ContainsKeys("Let It Be", "Help!"))
  .Because("the key \"Help!\" is missing, although \"Let It Be\" is contained");
await Expect.That(playCounts).DoesNotContainKeys("Help!", "Something")
  .Because("none of the keys is contained, whereas `DoesNotContainKeys(\"Let It Be\", \"Help!\")` would fail");
```

## Keys and values

You can continue with expectations on the keys or on the values of a dictionary:

```csharp
await Expect.That(playCounts).Keys.Contains("Let It Be");
await Expect.That(playCounts).Values.All().AreUnique();
```

The keys keep the key comparer of the dictionary, so e.g. `Keys.Contains("LET IT BE")` succeeds for a key
`"Let It Be"` in a dictionary created with `StringComparer.OrdinalIgnoreCase` (for a
`ConcurrentDictionary<TKey, TValue>` not [on .NET Framework](../02-getting-started.md#target-frameworks)).

The keys of a dictionary are unique by design, so its entries are unique as well. To verify that its values are unique,
use `Values.All().AreUnique()`.
