import PropertyComparisons from '../_property-comparisons.md';

# Items

Describes how to verify the number of items, how many items meet an expectation, a single item or the item at an
index.

## Count

You can verify the number of items in a collection:

```csharp
IEnumerable<int> values = Enumerable.Range(1, 10);

await Expect.That(values).HasCount(10);
// or more explicit
await Expect.That(values).HasCount().EqualTo(10);

await Expect.That(values).HasCount().GreaterThan(8);
await Expect.That(values).HasCount().Between(8).And(12);
await Expect.That(values).HasCount().NotBetween(1).And(5);
```

<PropertyComparisons />

You can also verify that the collection is empty or not:

```csharp
await Expect.That(Array.Empty<int>()).IsEmpty();
await Expect.That(Enumerable.Range(1, 10)).IsNotEmpty();
```

## Elements

You can add expectations that a certain number of elements must meet. A quantifier selects how many:

| Quantifier              | Succeeds when the expectation is met by               |
|-------------------------|-------------------------------------------------------|
| `All()`                 | every item                                            |
| `Any()`                 | at least one item (a shorthand for `AtLeast(1)`)      |
| `AtLeast(minimum)`      | at least `minimum` items                              |
| `AtMost(maximum)`       | at most `maximum` items                               |
| `Between(min).And(max)` | between `min` and `max` items, both bounds included   |
| `Exactly(expected)`     | exactly `expected` items                              |
| `LessThan(maximum)`     | fewer than `maximum` items                            |
| `MoreThan(minimum)`     | more than `minimum` items                             |
| `None()`                | no item                                               |

An empty collection satisfies `All()`, like it does `Enumerable.All`, so
`Expect.That(new int[0]).All().Satisfy(x => false)` succeeds. In contrast,
[`WithRecursiveInnerExceptions`](../04-delegates.md#recursive-inner-exceptions) and
`HasRecursiveInnerExceptions` fail for an exception without inner exceptions.

### Nested expectation

You can verify that items in a collection comply with an expectation on the individual elements:

```csharp
await Expect.That([1, 2, 3]).All().ComplyWith(item => item.IsLessThan(4));
await Expect.That([1, 2, 3]).Any().ComplyWith(item => item.IsEqualTo(2));
await Expect.That([1, 2, 3]).AtLeast(2).ComplyWith(item => item.IsGreaterThanOrEqualTo(2));
await Expect.That([1, 2, 3]).AtMost(1).ComplyWith(item => item.IsNegative());
await Expect.That([1, 2, 3]).Between(2).And(3).ComplyWith(item => item.IsPositive());
await Expect.That([1, 2, 3]).Exactly(1).ComplyWith(item => item.IsEqualTo(2));
await Expect.That([1, 2, 3]).None().ComplyWith(item => item.IsNegative());
```

### Condition

You can verify that items in a collection satisfy a condition:

```csharp
await Expect.That([1, 2, 3]).All().Satisfy(item => item < 4);
await Expect.That([1, 2, 3]).Any().Satisfy(item => item == 2);
await Expect.That([1, 2, 3]).AtLeast(2).Satisfy(item => item >= 2);
await Expect.That([1, 2, 3]).AtMost(1).Satisfy(item => item < 0);
await Expect.That([1, 2, 3]).Between(2).And(3).Satisfy(item => item > 0);
await Expect.That([1, 2, 3]).Exactly(1).Satisfy(item => item == 2);
await Expect.That([1, 2, 3]).None().Satisfy(item => item < 0);
```

### Equality

You can verify that the items in the collection are equal to the `expected` value:

```csharp
await Expect.That([1, 1, 1]).All().AreEqualTo(1);
```

You can also use a [custom comparer](../common-types/06-object.md#custom-comparer) or
configure [equivalency](../06-equivalency.md):

```csharp
IEnumerable<Album> albums = //...
Album expected = //...

await Expect.That(albums).All().AreEqualTo(expected).Equivalent();
await Expect.That(albums).All().AreEqualTo(expected).Using(new AlbumComparer());
```

For strings, you can configure this expectation to ignore case, ignore newline style, ignore the indentation, ignoring
leading or trailing white-space, or use a custom `IEqualityComparer<string>`:

```csharp
await Expect.That(["foo", "FOO", "Foo"]).All().AreEqualTo("foo").IgnoringCase();
```

For certain types you can also specify a [tolerance](./index.md#tolerance):

```csharp
IEnumerable<double> values = [2.04, 2.02, 2.01];

await Expect.That(values).All().AreEqualTo(2.0).Within(0.1);
```

### Unique

You can verify how many items in a collection occur exactly once:

```csharp
await Expect.That([1, 2, 3]).All().AreUnique();
await Expect.That([1, 2, 3, 1]).AtLeast(1).AreNotUnique();
await Expect.That([1, 2, 1, 2]).None().AreUnique();
await Expect.That([1, 2, 3, 4, 5, 5]).AtLeast(4).AreUnique();
```

For objects, you can also verify the uniqueness of a member:

```csharp
Album[] albums = //...

await Expect.That(albums).All().AreUnique(x => x.Title);
```

You can also use a [custom comparer](../common-types/06-object.md#custom-comparer), or ignore the case of
strings:

```csharp
await Expect.That(albums).All().AreUnique().Using(new AlbumComparer());
await Expect.That(["a", "b"]).All().AreUnique().IgnoringCase();
```

A [set](./index.md#sets) that was created with a custom comparer never holds two items that its comparer considers
equal, so its items are unique, unless a custom comparer or a string option such as `IgnoringCase()` changes the
comparison.

For dictionaries, verify the [values](./04-dictionaries.md#keys-and-values) instead, as the keys are unique by design.

## Single item

You can verify that the collection contains a single element that satisfies an expectation:

```csharp
IEnumerable<int> values = [42];

await Expect.That(values).HasSingle();
await Expect.That(values).HasSingle().Which.IsGreaterThan(41);
```

You can also apply filters for the items:

```csharp
IEnumerable<int> values = [1, 2, 3,];

await Expect.That(values).HasSingle().Matching(it => it > 2);

// Or with generic type:
IEnumerable<Person> persons = //...

await Expect.That(persons).HasSingle().Matching<Student>();
await Expect.That(persons).HasSingle().Matching<Student>(student => student.Courses.Count == 0);

// Similar to the expectations above, but verify the type exactly:
await Expect.That(persons).HasSingle().MatchingExactly<Student>();
await Expect.That(persons).HasSingle().MatchingExactly<Student>(student => student.Courses.Count == 0);
```

The awaited result is the single element:

```csharp
IEnumerable<int> values = [42];

int result = await Expect.That(values).HasSingle();
await Expect.That(result).IsGreaterThan(41);
```

## Item at index

You can verify that the collection contains an item that satisfies the expectation on a given index (or any index):

```csharp
IEnumerable<string> values = ["0th item", "1st item", "2nd item", "3rd item"];

await Expect.That(values).HasItem("1st item").AtIndex(1); // at the zero-based index 1
await Expect.That(values).HasItem("2nd item").AtIndex(1).FromEnd(); // at the zero-based index 1 from end
await Expect.That(values).HasItem(it => it.StartsWith("2nd")); // at any index
```

You can also check that the item matches a specific type:

```csharp
IEnumerable<INotification> values = //...

// Verify that the item at index 1 is of type `UserCreatedNotification`
await Expect.That(values).HasItem().Matching<UserCreatedNotification>().AtIndex(1);
// Verify that an item of type `UserDeletedNotification` for user with ID 3 exists
await Expect.That(values).HasItem().Matching<UserDeletedNotification>(x => x.UserId == 3);

// Similar to the expectations above, but verify the type exactly:
await Expect.That(values).HasItem().MatchingExactly<UserCreatedNotification>().AtIndex(1);
await Expect.That(values).HasItem().MatchingExactly<UserDeletedNotification>(x => x.UserId == 3);
```

You can also use expectations on the individual items:

```csharp
IEnumerable<string> values = ["0th item", "1st item", "2nd item", "3rd item"];

await Expect.That(values).HasItemThat(it => it.IsEqualTo("1st item")).AtIndex(1);
await Expect.That(values).HasItemThat(it => it.StartsWith("2nd").And.EndsWith("item")); // at any index
```

Each of these expectations has a negated counterpart. It is satisfied when the index holds a different item, and also
when the collection is too short to have an item at that index:

```csharp
IEnumerable<string> values = ["0th item", "1st item"];

await Expect.That(values).DoesNotHaveItem("2nd item").AtIndex(1);
await Expect.That(values).DoesNotHaveItem("0th item").AtIndex(4); // no item at index 4
await Expect.That(values).DoesNotHaveItem().Matching(it => it.StartsWith("2nd")).AtIndex(1);
await Expect.That(values).DoesNotHaveItemThat(it => it.StartsWith("2nd")).AtIndex(1);
```
