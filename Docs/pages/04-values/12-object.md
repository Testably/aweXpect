# Object

Describes the possible expectations for any object.

| Expectation                                                             | Negated             | Summary                                               |
|-------------------------------------------------------------------------|---------------------|-------------------------------------------------------|
| [`IsEqualTo`](#equality)                                                | `IsNotEqualTo`      | equal to the expected object                          |
| [`IsSameAs`](#reference-equality)                                       | `IsNotSameAs`       | the same reference as the expected object             |
| [`IsEquatableTo`](#iequatablet)                                         | `IsNotEquatableTo`  | equal according to its `IEquatable<T>` implementation |
| [`IsEquivalentTo`](#equivalency)                                        | `IsNotEquivalentTo` | structurally equal to the expected object             |
| [`IsOneOf`](#one-of)                                                    | `IsNotOneOf`        | equal to one of the expected objects                  |
| [`Is<T>`](#type-check)                                                  | `IsNot<T>`          | of type `T` or a derived type                         |
| [`IsExactly<T>`](#type-check)                                           | `IsNotExactly<T>`   | exactly of type `T`                                   |
| [`IsNull`](#null)                                                       | `IsNotNull`         | `null`                                                |
| [`Satisfies`](#condition)                                               | `DoesNotSatisfy`    | satisfies a predicate                                 |
| [`CompliesWith`](#nested-expectation)                                   | `DoesNotComplyWith` | meets a nested expectation                            |
| [`Whose`](../03-how-it-works/03-combining.md#on-members-of-the-subject) |                     | a member meets a nested expectation                   |

## Equality

You can verify that the `object` is equal to another one or not:

```csharp
record Album(string Title);
Album album = new("Abbey Road");

await Expect.That(album).IsEqualTo(new Album("Abbey Road"));
await Expect.That(album).IsNotEqualTo(new Album("Revolver"));
```

This uses the `object.Equals(object?, object?)` method.

:::note
A number typed as `object` also equals a number of another numeric type with the same value (e.g. `1` and `1L`)
in equality (`IsEqualTo`, `IsOneOf`, `Contains`, `ContainsValue`), but not in
[equivalency](./13-equivalency.md). For `Half`, `Int128` and `UInt128` this only applies
[on .NET 8 or later](../02-getting-started.md#target-frameworks).
:::

### Reference equality

You can verify that the `object` has the same reference as another one:

```csharp
record Album(string Title);
Album album = new("Abbey Road");

await Expect.That(album).IsSameAs(album);
await Expect.That(album).IsNotSameAs(new Album("Abbey Road"));
```

This uses the `object.ReferenceEquals(object?, object?)` method.

### Custom comparer

You can verify that the `object` is equal to another one while using a custom `IEqualityComparer<object>`:

```csharp
class AlbumComparer : IEqualityComparer<object>
{
  public bool Equals(object? x, object? y)
    => (x as Album)?.Title == (y as Album)?.Title;
  public int GetHashCode(object obj)
    => obj.GetHashCode();
}
Album album = new("Abbey Road");

await Expect.That(album).IsEqualTo(new Album("Abbey Road")).Using(new AlbumComparer());
```

### `IEquatable<T>`

You can verify that the `object` is equal to a value by using its `IEquatable<T>` implementation, also when the value
has a different type:

```csharp
class TrackId(long value) : IEquatable<long>
{
  public bool Equals(long other) => value == other;
}
TrackId trackId = new(42);

await Expect.That(trackId).IsEquatableTo(42L);
await Expect.That(trackId).IsNotEquatableTo(7L);
```

:::note
This inspects the subject by calling its `IEquatable<T>.Equals(T)` method. Therefore, `IsEquatableTo` and
`IsNotEquatableTo` fail for a `null` subject, even `IsEquatableTo(null)`, whereas `IsEqualTo(null)` succeeds.
:::

## Equivalency

You can verify that the `object` is structurally equivalent to another one. See the
[equivalency](./13-equivalency.md) page for details and configuration options:

```csharp
Album album = new("Abbey Road");

await Expect.That(album).IsEquivalentTo(new Album("Abbey Road"));
await Expect.That(album).IsNotEquivalentTo(new Album("Revolver"));
```

## One of

You can verify that the `object` is one of many alternatives:

```csharp
record Album(string Title);
Album album = new("Abbey Road");

await Expect.That(album).IsOneOf([new Album("Abbey Road"), new Album("Revolver")]);
await Expect.That(album).IsNotOneOf([new Album("Revolver"), new Album("Help!")]);
```

## Type check

You can verify that the `object` is of a given type or a derived type, or that it is not:

```csharp
object album = new Album("Abbey Road");

await Expect.That(album).Is<Album>();
await Expect.That(album).Is(typeof(Album));
await Expect.That(album).IsNot<Track>();
await Expect.That(album).IsNot(typeof(Track));
```

`IsExactly` and `IsNotExactly` do not accept a derived type:

```csharp
object album = new Album("Abbey Road");

await Expect.That(album).IsExactly<Album>();
await Expect.That(album).IsExactly(typeof(Album));
await Expect.That(album).IsNotExactly<Track>();
await Expect.That(album).IsNotExactly(typeof(Track));
```

## Null

You can verify if the `object` is `null` or not:

```csharp
object? album = null;

await Expect.That(album).IsNull();
await Expect.That(new Album("Abbey Road")).IsNotNull();
```

## Condition

You can verify that any object satisfies a given predicate:

```csharp
Track track = new() { Title = "Let It Be", PlayCount = 3 };

await Expect.That(track).Satisfies(x => x.PlayCount > 0);
await Expect.That(track).DoesNotSatisfy(x => x.IsPlayed);
```

When the object changes in the background, `Within(…)` waits until it satisfies the predicate, see
[waiting for a condition](../03-how-it-works/06-time-and-cancellation.md#a-condition).

## Nested expectation

You can verify that any object complies with an expectation:

```csharp
List<Track> tracks = new();

await Expect.That(tracks).CompliesWith(x => x.IsEmpty());
await Expect.That(tracks).DoesNotComplyWith(x => x.HasCount().GreaterThan(0));
```

`DoesNotComplyWith` is the exact inverse of `CompliesWith`: it succeeds as soon as the nested expectation fails. The
exception is a `null` subject, which fails an expectation that inspects it in its negated form as well, see
[`null` subjects](../03-how-it-works/04-null-subjects.md). Like `Satisfies`, `CompliesWith` can
[wait for the object](../03-how-it-works/06-time-and-cancellation.md#a-condition) with `Within(…)`.
