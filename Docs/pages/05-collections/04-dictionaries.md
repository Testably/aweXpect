---
description: Verify dictionaries by their entries, keys and values, and compare them with other dictionaries.
---

# Dictionaries

Describes the possible expectations for dictionaries.

| Expectation                           | Negated                | Summary                                              |
|---------------------------------------|------------------------|------------------------------------------------------|
| [`IsEqualTo`](#equality)              | `IsNotEqualTo`         | has the same entries as the expected dictionary      |
| [`Contains`](#entry)                  | `DoesNotContain`       | contains the expected entry                          |
| [`ContainsKey`](#contained)           | `DoesNotContainKey`    | contains the expected key                            |
| [`ContainsKeys`](#contained)          | `DoesNotContainKeys`   | contains all expected keys (negated: none of them)   |
| [`ContainsValue`](#contained)         | `DoesNotContainValue`  | contains the expected value                          |
| [`ContainsValues`](#contained)        | `DoesNotContainValues` | contains all expected values (negated: none of them) |
| [`Keys`, `Values`](#as-collections)   |                        | continue with the keys or values as a collection     |

The samples on this page use the following dictionary:

```csharp
Dictionary<string, int> playCounts = new() { { "Let It Be", 42 }, { "Yesterday", 7 } };
```

## Equality

You can verify that a dictionary is equal to another one. The entries are compared by key, so the order in which the
two dictionaries enumerate them does not matter:

```csharp
Dictionary<string, int> reordered = new() { { "Yesterday", 7 }, { "Let It Be", 42 } };

await That(playCounts).IsEqualTo(reordered);
await That(playCounts).IsNotEqualTo(new Dictionary<string, int> { { "Let It Be", 41 } });
```

The keys are looked up through the dictionary, so its key comparer decides which keys are the same:

```csharp
Dictionary<string, int> ratings = new(StringComparer.OrdinalIgnoreCase) { { "Let It Be", 5 } };

await That(ratings).IsEqualTo(new Dictionary<string, int> { { "LET IT BE", 5 } });
```

To compare the entries in their enumeration order instead, compare them as a collection of
`KeyValuePair<TKey, TValue>`:

```csharp
SortedDictionary<string, int> sorted = new() { { "Let It Be", 42 }, { "Yesterday", 7 } };
KeyValuePair<string, int>[] expected = [new("Let It Be", 42), new("Yesterday", 7)];

await That(sorted.AsEnumerable()).IsEqualTo(expected);
```

<details>
<summary>Which key comparers are read</summary>

The comparer is read from the dictionary types of the framework and from the dictionary that a
`ReadOnlyDictionary<TKey, TValue>` wraps, not from a custom dictionary or a `ConcurrentDictionary<TKey, TValue>` on
.NET Framework. Before .NET 10, the wrapped dictionary is only reached by reflection, so it is not read in a project
that enables trimming or Native AOT. For the others, a key of the dictionary that equals no expected key fails the
expectation, even when the comparer considers it the same as one.

</details>

## Entry

You can verify that a dictionary contains the `expected` entry, which is looked up by its key, too:

```csharp
await That(playCounts).Contains("Let It Be", 42);
await That(playCounts).Contains(new KeyValuePair<string, int>("Yesterday", 7));
await That(playCounts).DoesNotContain("Let It Be", 7);
```

## Keys and values

### Contained

You can verify that a dictionary contains the `expected` key(s) or value(s). Several keys or values can also be given
as a collection:

```csharp
await That(playCounts).ContainsKey("Let It Be");
await That(playCounts).ContainsKeys("Let It Be", "Yesterday");
await That(playCounts).DoesNotContainKey("Help!");
await That(playCounts).DoesNotContainKeys("Help!", "Something");
await That(playCounts).ContainsValue(42);
await That(playCounts).ContainsValues(42, 7);
await That(playCounts).DoesNotContainValue(0);
await That(playCounts).DoesNotContainValues(0, 1);
await That(playCounts).ContainsKeys(new List<string> { "Let It Be", "Yesterday" });
```

After `ContainsKey` or `ContainsKeys`, you can add expectations on the corresponding value(s). `WhoseValues` is a
collection of the values for the expected keys, so all collection expectations are available:

```csharp
await That(playCounts).ContainsKey("Let It Be").WhoseValue.IsEqualTo(42);
await That(playCounts).ContainsKeys("Let It Be", "Yesterday").WhoseValues.IsEqualTo([42, 7]);
await That(playCounts).ContainsKeys("Let It Be", "Yesterday")
  .WhoseValues.All().ComplyWith(v => v.IsPositive());
```

The values, also the value of an [entry](#entry), are compared with the same options as the items of a collection,
e.g. `IgnoringCase()` for strings or a [tolerance](./01-equality.md#tolerance) with `Within`:

```csharp
Dictionary<int, string> titles = new() { { 1, "Let It Be" }, { 2, "Yesterday" } };
Dictionary<string, double> durations = new() { { "Let It Be", 3.85 }, { "Yesterday", 2.07 } };

await That(titles).ContainsValue("let it be").IgnoringCase();
await That(titles).ContainsValues("LET IT BE", "YESTERDAY").IgnoringCase();
await That(titles).Contains(1, "LET IT BE").IgnoringCase();
await That(durations).Contains("Yesterday", 2.0).Within(0.1);
```

`DoesNotContainKeys` and `DoesNotContainValues` mean "none of": they fail as soon as the dictionary contains one of
them. This is stricter than negating `ContainsKeys` or `ContainsValues` with `DoesNotComplyWith`, which is the exact
inverse ("not all") and only fails when the dictionary contains all of them:

```csharp
await That(playCounts).DoesNotComplyWith(d => d.ContainsKeys("Let It Be", "Help!"))
  .Because("the key \"Help!\" is missing, although \"Let It Be\" is contained");
await That(playCounts).DoesNotContainKeys("Help!", "Something")
  .Because("none of the keys is contained, while adding \"Let It Be\" would fail");
```

### As collections

:::info[C# 14 or later]
`Keys` and `Values` are extension properties, which need C# 14 or later, the default only for .NET 10 and later. With
an older language version, pass the keys or values themselves, e.g. `That(playCounts.Values).All().AreUnique()`;
the keys then no longer use the key comparer of the dictionary.
:::

You can continue with expectations on the keys or on the values of a dictionary:

```csharp
await That(playCounts).Keys.Contains("Let It Be");
await That(playCounts).Values.All().AreUnique();
```

The keys keep the key comparer of the dictionary, so e.g. `Keys.Contains("LET IT BE")` succeeds for a key
`"Let It Be"` in a dictionary created with `StringComparer.OrdinalIgnoreCase`.

The keys of a dictionary are unique by design, so its entries are unique as well. To verify that its values are unique,
use `Values.All().AreUnique()`.

<details>
<summary>`ConcurrentDictionary` on .NET Framework</summary>

On [.NET Framework](../03-setup/02-target-frameworks.md), the comparer of a
`ConcurrentDictionary<TKey, TValue>` is not read.

</details>
