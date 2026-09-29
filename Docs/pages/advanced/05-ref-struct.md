# Ref struct types

Describes how you can work with `ref` structure types

## Spans

On .NET 8 or later, you can pass a `Span<T>` or a `ReadOnlySpan<T>` directly to `Expect.That`. The items are copied
into a `SpanWrapper<T>`, which implements `ICollection<T>`, so all [collection expectations](../03-collections/index.md)
are available:

```csharp
await Expect.That("foo".AsSpan()).HasCount(3);
await Expect.That("foo".AsSpan()).IsEqualTo(['f', 'o', 'o']);
```

A span can't be kept across an `await`, so create it in the statement of the expectation.

## Other ref struct types

[ref struct types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct) can't be
used in an `async` context.  
In order to allow using aweXpect on properties of other `ref struct` types, or of spans on older target frameworks,
there is a built-in workaround to verify the expectation synchronously, so that the test method itself can remain
synchronous:

```csharp
using aweXpect.Synchronous;

ReadOnlySpan<char> subject = @"foo".AsSpan();

Synchronously.Verify(Expect.That(subject.Length).IsEqualTo(3));
// or alternatively:
Expect.That(subject.Length).IsEqualTo(3).VerifySynchronously();
```

Both methods are in the namespace `aweXpect.Synchronous`.
