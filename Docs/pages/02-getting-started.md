# Getting started

## Installation

1. Install the [`aweXpect`](https://www.nuget.org/packages/aweXpect) nuget package
   ```ps
   dotnet add package aweXpect
   ```

2. Add the following `using` statement:
   ```csharp
   using aweXpect;
   ```
   This brings the static `Expect` class and lots of extension methods into scope.

3. Simplify expectations (optional)  
   If you want to simplify the assertions, you can add a `global using static aweXpect.Expect;` statement anywhere in
   your test project.
   This allows writing a more concise syntax:
   ```csharp
   //    ↓ Default behaviour
   await Expect.That(subject).IsTrue();
   await That(subject).IsTrue();
   //    ↑ With global static
   ```

## Write your first expectation

Every expectation starts with `Expect.That(subject)`, continues with what you expect and is awaited:

```csharp
[Fact]
public async Task IsInLibrary_WhenAlbumIsMissing_ShouldReturnFalse()
{
  bool result = IsInLibrary("Unknown Album");
  
  await Expect.That(result).IsFalse();
}
```

If it fails, it will throw a framework-specific exception with the following message:

```text title="Failure message"
Expected that result
is False,
but it was True
```

## Add a reason

You can add a reason for all expectations that will be included in the exception message:

```csharp
[Fact]
public async Task IsInLibrary_WhenAlbumIsMissing_ShouldReturnFalse()
{
  bool result = IsInLibrary("Unknown Album");
  
  await Expect.That(result).IsFalse().Because("the album is not in the library");
}
```

The reason then appears in the failure message:

```text title="Failure message"
Expected that result
is False, because the album is not in the library,
but it was True
```

## Target frameworks

aweXpect supports .NET Framework 4.8 and .NET Standard 2.0 as well as .NET 8 and later. The following features are
only available on .NET 8 or later:

- expectations for `IAsyncEnumerable<T>`;
- `DateOnly` and `TimeOnly`;
- `Span<T>` and `ReadOnlySpan<T>` subjects;
- `IsParsableInto` for strings;
- `HasBufferSize` for buffered streams;
- delegates that return a `ValueTask` or `ValueTask<T>`.
## Next steps

- [Concepts](./03-how-it-works/01-anatomy.md) explains how expectations are combined and negated, and how they treat `null`.
- The pages for [common types](./04-values/01-boolean.md), [collections](./05-collections/index.md),
  [delegates](./06-behaviour/01-delegates.md), [events](./06-behaviour/03-events.md) and [equivalency](./04-values/13-equivalency.md) list the available
  expectations.
