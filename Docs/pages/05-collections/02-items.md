import PropertyComparisons from '../_property-comparisons.md';

# Items

Describes how to verify the number of items, how many items meet an expectation, a single item or the item at an
index.

| Expectation                         | Negated               | Summary                                                           |
|-------------------------------------|-----------------------|-------------------------------------------------------------------|
| [`HasCount`](#count)                | negated comparison    | has the expected number of items                                  |
| [`IsEmpty`](#count)                 | `IsNotEmpty`          | has no items                                                      |
| [`All()`, `Any()`, …](#quantifiers) | `None()`              | the selected number of items meet an expectation                  |
| [`HasSingle`](#single-item)         |                       | has exactly one (matching) item                                   |
| [`HasItem`](#item-at-index)         | `DoesNotHaveItem`     | has the expected item at an index or at any index                 |
| [`HasItemThat`](#item-at-index)     | `DoesNotHaveItemThat` | has an item that meets an expectation at an index or at any index |

## Count

You can verify the number of items in a collection:

```csharp
IEnumerable<string> songs = ["Come Together", "Something", "Let It Be"];

await Expect.That(songs).HasCount(3);
await Expect.That(songs).HasCount().GreaterThan(2);
await Expect.That(songs).HasCount().NotBetween(5).And(10);
```

<PropertyComparisons />

You can also verify that the collection is empty or not:

```csharp
await Expect.That(Array.Empty<Track>()).IsEmpty();
await Expect.That(new[] { "Let It Be" }).IsNotEmpty();
```

## Quantifiers

You can add expectations that a certain number of items must meet. A quantifier selects how many:

| Quantifier              | Succeeds when the expectation is met by             |
|-------------------------|-----------------------------------------------------|
| `All()`                 | every item                                          |
| `Any()`                 | at least one item (a shorthand for `AtLeast(1)`)    |
| `AtLeast(minimum)`      | at least `minimum` items                            |
| `AtMost(maximum)`       | at most `maximum` items                             |
| `Between(min).And(max)` | between `min` and `max` items, both bounds included |
| `Exactly(expected)`     | exactly `expected` items                            |
| `LessThan(maximum)`     | fewer than `maximum` items                          |
| `MoreThan(minimum)`     | more than `minimum` items                           |
| `None()`                | no item                                             |

and what the items must meet:

| Expectation                         | Succeeds for an item that                                             |
|-------------------------------------|-----------------------------------------------------------------------|
| [`ComplyWith`](#nested-expectation) | meets the nested expectation                                          |
| [`Satisfy`](#condition)             | satisfies the predicate                                               |
| [`AreEqualTo`](#equality)           | is equal to the expected value                                        |
| [`AreEquivalentTo`](#equality)      | is [equivalent](../04-values/13-equivalency.md) to the expected value |
| [`Are<T>`](#type)                   | is of type `T` or a derived type                                      |
| [`AreExactly<T>`](#type)            | is exactly of type `T`                                                |
| [`AreUnique`](#unique)              | occurs exactly once (`AreNotUnique`: more than once)                  |

An empty collection satisfies `All()`, like it does `Enumerable.All`, so
`Expect.That(new int[0]).All().Satisfy(x => false)` succeeds. In contrast,
[`WithRecursiveInnerExceptions`](../06-behaviour/01-delegates.md#recursive-inner-exceptions) and
`HasRecursiveInnerExceptions` fail for an exception without inner exceptions.

### Nested expectation

You can verify that items in a collection comply with an expectation on the individual items:

```csharp
await Expect.That([1, 2, 3]).All().ComplyWith(item => item.IsLessThan(4));
await Expect.That([1, 2, 3]).Any().ComplyWith(item => item.IsEqualTo(2));
await Expect.That([1, 2, 3]).AtLeast(2).ComplyWith(item => item.IsGreaterThanOrEqualTo(2));
await Expect.That([1, 2, 3]).AtMost(1).ComplyWith(item => item.IsNegative());
await Expect.That([1, 2, 3]).Between(2).And(3).ComplyWith(item => item.IsPositive());
await Expect.That([1, 2, 3]).Exactly(1).ComplyWith(item => item.IsEqualTo(2));
await Expect.That([1, 2, 3]).None().ComplyWith(item => item.IsNegative());
```

An item for which the nested expectation fails and its negation fails as well decides the result, because the
expectation could not answer it: the nested expectation threw (the exception becomes the `InnerException`), or the item
or an inspected member is `null`. So `None().ComplyWith(item => item.StartsWith("Let"))` fails for a `null` item, and so
does its negation. The same applies to [`HasItemThat`](#item-at-index) and to collections compared with item
expectations or predicates, where in any order such an item only decides when it matches no other expected item.

### Condition

You can verify that items in a collection satisfy a condition:

```csharp
Track[] tracks = [new() { Title = "Let It Be", PlayCount = 3 }, new() { Title = "Get Back", PlayCount = 0 }];

await Expect.That(tracks).Any().Satisfy(track => track.PlayCount > 0);
await Expect.That(tracks).None().Satisfy(track => track.Title == null);
```

### Equality

You can verify that the items in the collection are equal to the `expected` value:

```csharp
await Expect.That([1, 1, 1]).All().AreEqualTo(1);
```

The items are compared by their default equality, unless you specify a
[custom comparer](../04-values/12-object.md#custom-comparer), [equivalency](../04-values/13-equivalency.md), a
[tolerance](./index.md#tolerance) or, for strings, one of the [string options](../04-values/03-string.md#string-options):

```csharp
IEnumerable<Album> albums = //...
Album expected = //...

await Expect.That(albums).All().AreEqualTo(expected).Equivalent();
await Expect.That(albums).All().AreEqualTo(expected).Using(new AlbumComparer());
await Expect.That(albums).AtLeast(2).AreEquivalentTo(expected);
await Expect.That(["let it be", "LET IT BE"]).All().AreEqualTo("Let It Be").IgnoringCase();
await Expect.That([2.04, 2.02, 2.01]).All().AreEqualTo(2.0).Within(0.1);
```

### Type

You can verify that the items are of a given type, or exactly of that type:

```csharp
IEnumerable<INotification> notifications = //...

await Expect.That(notifications).All().Are<INotification>();
await Expect.That(notifications).AtLeast(1).AreExactly<UserCreatedNotification>();
await Expect.That(notifications).None().Are(typeof(UserDeletedNotification));
```

### Unique

You can verify how many items in a collection occur exactly once:

```csharp
await Expect.That([1, 2, 3]).All().AreUnique();
await Expect.That([1, 2, 3, 1]).AtLeast(1).AreNotUnique();
await Expect.That([1, 2, 1, 2]).None().AreUnique();
await Expect.That([1, 2, 3, 4, 5, 5]).AtLeast(4).AreUnique();
```

For objects, you can also verify the uniqueness of a member, and you can use a
[custom comparer](../04-values/12-object.md#custom-comparer) or ignore the case of strings:

```csharp
Album[] albums = //...

await Expect.That(albums).All().AreUnique(x => x.Title);
await Expect.That(albums).All().AreUnique().Using(new AlbumComparer());
await Expect.That(["Help!", "Revolver"]).All().AreUnique().IgnoringCase();
```

A [set](./index.md#sets) that was created with a custom comparer never holds two items that its comparer considers
equal, so its items are unique, unless a custom comparer or a string option such as `IgnoringCase()` changes the
comparison.

For dictionaries, verify the [values](./04-dictionaries.md#keys-and-values) instead, as the keys are unique by design.

## Single item

You can verify that the collection contains a single item, and continue with expectations on it with `Which`:

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

The awaited result is the single item:

```csharp
IEnumerable<string> songs = ["Let It Be"];

string song = await Expect.That(songs).HasSingle();
```

## Item at index

You can verify that the collection contains an item that satisfies the expectation on a given index (or any index):

```csharp
IEnumerable<string> songs = ["Two of Us", "Dig a Pony", "Across the Universe", "I Me Mine"];

await Expect.That(songs).HasItem("Dig a Pony").AtIndex(1); // at the zero-based index 1
await Expect.That(songs).HasItem("Across the Universe").AtIndexFromEnd(1); // at the zero-based index 1 from end
await Expect.That(songs).HasItem(it => it.StartsWith("I Me")); // at any index
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
IEnumerable<string> songs = ["Two of Us", "Dig a Pony", "Across the Universe", "I Me Mine"];

await Expect.That(songs).HasItemThat(it => it.IsEqualTo("Dig a Pony")).AtIndex(1);
await Expect.That(songs).HasItemThat(it => it.StartsWith("Across").And.EndsWith("Universe")); // at any index
```

Each of these expectations has a negated counterpart. It is satisfied when the index holds a different item, and also
when the collection is too short to have an item at that index:

```csharp
IEnumerable<string> songs = ["Two of Us", "Dig a Pony"];

await Expect.That(songs).DoesNotHaveItem("I Me Mine").AtIndex(1);
await Expect.That(songs).DoesNotHaveItem("Two of Us").AtIndex(4); // no item at index 4
await Expect.That(songs).DoesNotHaveItem().Matching(it => it.StartsWith("I Me")).AtIndex(1);
await Expect.That(songs).DoesNotHaveItemThat(it => it.StartsWith("I Me")).AtIndex(1);
```
