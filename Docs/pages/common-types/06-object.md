# Object

Describes the possible expectations for objects.

## Equality

You can verify that the `object` is equal to another one or not:

```csharp
record Album(string Title);
Album subject = new("Abbey Road");

await Expect.That(subject).IsEqualTo(new Album("Abbey Road"));
await Expect.That(subject).IsNotEqualTo(new Album("Revolver"));
```

*Note: this uses the underlying `object.Equals(object?, object?)` method*

### Reference equality

You can verify that the `object` has the same reference as another one:

```csharp
record Album(string Title);
Album subject = new("Abbey Road");

await Expect.That(subject).IsSameAs(subject);
await Expect.That(subject).IsNotSameAs(new Album("Abbey Road"));
```

*Note: this uses the underlying `object.ReferenceEquals(object?, object?)` method*

### Custom comparer

You can verify that the `object` is equal to another one while using a custom `IEqualityComparer<object>`:

```csharp
class AlbumComparer : IEqualityComparer<object>
{
  public bool Equals(object? x, object? y)
    => x != null && y != null;
  public int GetHashCode(object obj)
    => obj.GetHashCode();
}
Album subject = new("Abbey Road");

await Expect.That(subject).IsEqualTo(new Album("Revolver")).Using(new AlbumComparer());
```

## Equivalency

You can verify that the `object` is structurally equivalent to another one. See the
[equivalency](/docs/expectations/equivalency) page for details and configuration options.

```csharp
class Album(string title)
{
  public string Title { get; } = title;
}
Album subject = new("Abbey Road");

await Expect.That(subject).IsEquivalentTo(new Album("Abbey Road"));
await Expect.That(subject).IsNotEquivalentTo(new Album("Revolver"));
```

## One of

You can verify that the `object` is one of many alternatives:

```csharp
record Album(string Title);
Album subject = new("Abbey Road");

await Expect.That(subject).IsOneOf([new Album("Abbey Road"), new Album("Revolver")]);
await Expect.That(subject).IsNotOneOf([new Album("Revolver"), new Album("Help!")]);
```

## Type check

You can verify that the `object` is of a given type or not:

```csharp
object subject = new Album("Abbey Road");

await Expect.That(subject).Is<Album>();
await Expect.That(subject).Is(typeof(Album));
await Expect.That(subject).IsNot<Single>();
await Expect.That(subject).IsNot(typeof(Single));
```

This verifies, if the subject is of the given type or a derived type.

You can also verify that the `object` is only of the given type and not of a derived type:

```csharp
object subject = new Album("Abbey Road");

await Expect.That(subject).IsExactly<Album>();
await Expect.That(subject).IsExactly(typeof(Album));
await Expect.That(subject).IsNotExactly<Single>();
await Expect.That(subject).IsNotExactly(typeof(Single));
```

## Null

You can verify, if the `object` is `null` or not:

```csharp
object? subject = null;

await Expect.That(subject).IsNull();
await Expect.That(new object()).IsNotNull();
```

## Satisfy

You can verify that any object satisfies a given predicate:

```csharp
object? subject = null;

await Expect.That(subject).Satisfies(x => x == null);
```

When the object changes in the background, you can also verify that it satisfies a condition within a given time
period:

```csharp
Track subject = new() {
	IsPlayed = false
};
// Start a background task that sets `IsPlayed` to true

await Expect.That(subject).Satisfies(x => x.IsPlayed == true).Within(2.Seconds());
// using aweXpect.Chronology
```

## Comply with

You can verify that any object complies with an expectation:

```csharp
List<Track> tracks = new();

await Expect.That(tracks).CompliesWith(x => x.IsEmpty());
```

When the object changes in the background, you can also verify that it complies with an expectation within a given time
period:

```csharp
List<Track> tracks = new();
// Start a background task that adds items to `tracks`

await Expect.That(tracks).CompliesWith(x => x.HasCount().GreaterThanOrEqualTo(4)).Within(2.Seconds());
// using aweXpect.Chronology
```
