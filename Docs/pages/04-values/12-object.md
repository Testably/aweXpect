---
description: Verify any object for equality, reference equality, type, null, a predicate, a nested expectation or garbage collection.
---

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
| [`Whose`](../02-how-it-works/03-combining.md#on-members-of-the-subject) |                     | a member meets a nested expectation                   |
| [`IsCollected`](#garbage-collection)                                    | `IsNotCollected`    | the target of a `WeakReference` is garbage collected  |

## Equality

You can verify that the `object` is equal to another one or not:

```csharp
record Album(string Title);
Album album = new("Abbey Road");

await That(album).IsEqualTo(new Album("Abbey Road"));
await That(album).IsNotEqualTo(new Album("Revolver"));
```

This uses the `object.Equals(object?, object?)` method.

<details>
<summary>Numbers typed as `object`</summary>

A number typed as `object` also equals a number of another numeric type with the same value (e.g. `1` and `1L`)
in equality (`IsEqualTo`, `IsOneOf`, `Contains`, `ContainsValue`), but not in
[equivalency](./13-equivalency.md). For `Half`, `Int128` and `UInt128` this only applies
[on .NET 8 or later](../03-setup/02-target-frameworks.md).

</details>

### Reference equality

You can verify that the `object` has the same reference as another one:

```csharp
record Album(string Title);
Album album = new("Abbey Road");

await That(album).IsSameAs(album);
await That(album).IsNotSameAs(new Album("Abbey Road"));
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

await That(album).IsEqualTo(new Album("Abbey Road")).Using(new AlbumComparer());
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

await That(trackId).IsEquatableTo(42L);
await That(trackId).IsNotEquatableTo(7L);
```

## Equivalency

You can verify that the `object` is structurally equivalent to another one. See the
[equivalency](./13-equivalency.md) page for details and configuration options:

```csharp
Album album = new("Abbey Road");

await That(album).IsEquivalentTo(new Album("Abbey Road"));
await That(album).IsNotEquivalentTo(new Album("Revolver"));
```

Use `IsEqualTo` when the type defines what equal means, e.g. a record or a type that overrides `Equals`. Use
`IsEquivalentTo` when it does not, e.g. for a class that only has reference equality, or when some members should be
ignored. `.Equivalent()` switches an expectation that compares with `Equals`, e.g. `Contains` or `All().AreEqualTo` on
a collection, to the same [equivalency](./13-equivalency.md#as-a-modifier-of-equality).

## One of

You can verify that the `object` is one of many alternatives:

```csharp
record Album(string Title);
Album album = new("Abbey Road");

await That(album).IsOneOf([new Album("Abbey Road"), new Album("Revolver")]);
await That(album).IsNotOneOf([new Album("Revolver"), new Album("Help!")]);
```

## Type check

You can verify that the `object` is of a given type or a derived type, or that it is not:

```csharp
object album = new Album("Abbey Road");

await That(album).Is<Album>();
await That(album).Is(typeof(Album));
await That(album).IsNot<Track>();
await That(album).IsNot(typeof(Track));
```

`IsExactly` and `IsNotExactly` do not accept a derived type:

```csharp
object album = new Album("Abbey Road");

await That(album).IsExactly<Album>();
await That(album).IsExactly(typeof(Album));
await That(album).IsNotExactly<Track>();
await That(album).IsNotExactly(typeof(Track));
```

## Null

You can verify that the `object` is `null` or not:

```csharp
object? album = null;

await That(album).IsNull();
await That(new Album("Abbey Road")).IsNotNull();
```

## Condition

You can verify that any object satisfies a given predicate:

```csharp
Track track = new() { Title = "Let It Be", PlayCount = 3 };

await That(track).Satisfies(x => x.PlayCount > 0);
await That(track).DoesNotSatisfy(x => x.IsPlayed);
```

When the object changes in the background, `Within(…)` waits until it satisfies the predicate, see
[waiting for a condition](../02-how-it-works/06-time-and-cancellation.md#a-condition).

## Nested expectation

You can verify that any object complies with an expectation:

```csharp
List<Track> tracks = new();

await That(tracks).CompliesWith(x => x.IsEmpty());
await That(tracks).DoesNotComplyWith(x => x.HasCount().GreaterThan(0));
```

`DoesNotComplyWith` succeeds when the nested expectation fails. Like `Satisfies`, `CompliesWith` can
[wait for the object](../02-how-it-works/06-time-and-cancellation.md#a-condition) with `Within(…)`.

Which one to use:

- `Satisfies` for a quick check with a predicate; a failure only shows the predicate and the object.
- `CompliesWith` to reuse other expectations, e.g. `IsEmpty()` or a string option; a failure explains what the nested
  expectation found.
- [`Whose`](../02-how-it-works/03-combining.md#on-members-of-the-subject) to verify a member of the object; a failure
  names the member and its value.

## Garbage collection

You can verify that the target of a `WeakReference` or `WeakReference<T>` is garbage collected or not:

```csharp
using System.Runtime.CompilerServices;

WeakReference released = CreateReleasedReference();
Album album = new("Let It Be");
WeakReference<Album> cached = new(album);

await That(released).IsCollected();
await That(cached).IsNotCollected()
  .Because("the album is still in use");
GC.KeepAlive(album);

[MethodImpl(MethodImplOptions.NoInlining)]
static WeakReference CreateReleasedReference()
  => new(new Album("Abbey Road"));
```

:::warning[A reference held by the test keeps the target alive]
A strong reference that the test itself still holds keeps the target alive, e.g. a local variable or the temporary
object in `new WeakReference(new Album(…))`, which a Debug build keeps until the end of the method, even when it is no
longer used. The example above therefore creates the released album in a separate method. For an object the test
creates itself, use
[`IsCollectedAfter`](../06-behavior/01-delegates.md#memory-leaks) instead, which creates the object where no
reference to it remains.
:::

<details>
<summary>How the garbage collection is triggered</summary>

Both force full blocking garbage collections before they check the target, and repeat them a few times, as
finalizers can release further objects.

</details>
