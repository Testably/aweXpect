# Getting started

## Installation

1. Install the [`aweXpect`](https://www.nuget.org/packages/aweXpect) nuget package
   ```ps
   dotnet add package aweXpect --prerelease
   ```
   This documentation describes aweXpect v4, which is available as a pre-release. Without `--prerelease` you get
   v3, which lacks some of the APIs described here.

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

4. Write durations more readably (optional)  
   The samples in this documentation write durations like `5.Seconds()` with the
   [aweXpect.Chronology](/aweXpect/extensions/aweXpect.Chronology) package, marked with the comment
   `// from the aweXpect.Chronology package`; `TimeSpan.FromSeconds(5)` works as well.

## Write your first expectation

Every expectation starts with `Expect.That(subject)`, continues with what you expect and is awaited:

```csharp
[Test]
public async Task IsInLibrary_WhenAlbumIsMissing_ShouldReturnFalse()
{
  bool result = IsInLibrary("Unknown Album");
  
  await Expect.That(result).IsFalse();
}
```

:::warning
An expectation is only evaluated when it is awaited: without `await` it never fails. The analyzer
[aweXpect0001](./08-analyzers.md#awexpect0001) reports a missing `await` as an error.
:::

If it fails, it throws [an exception that fails the test](./08-setup/01-test-frameworks.md#failed-expectations)
with the following message:

```text title="Failure message"
Expected that result
is False,
but it was True
```

## Add a reason

You can add a reason for all expectations that will be included in the exception message:

```csharp
[Test]
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

## Find the expectation you need

The expectations are grouped by the type of the subject:

| What you test                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | Where to look                                       |
|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------|
| [`bool`](./04-values/01-boolean.md), [numbers](./04-values/02-number.md), [`string`](./04-values/03-string.md), [`char`](./04-values/04-char.md), [enums](./04-values/05-enum.md), [`Guid`](./04-values/06-guid.md), [`Version`](./04-values/07-version.md), [`Stream`](./04-values/08-stream.md), [`TimeSpan`](./04-values/09-timespan.md), [`DateTime`](./04-values/10-datetime-offset.md), [`DateOnly`, `TimeOnly`](./04-values/11-date-time-only.md), [any object](./04-values/12-object.md), [equivalency](./04-values/13-equivalency.md) | [Values](./04-values/index.md)                      |
| `IEnumerable<T>`, arrays, [dictionaries](./05-collections/04-dictionaries.md)                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | [Collections](./05-collections/index.md)            |
| [delegates and exceptions](./06-behaviour/01-delegates.md), [`Task`](./06-behaviour/02-tasks.md), [events](./06-behaviour/03-events.md), [callbacks](./06-behaviour/04-callbacks.md)                                                                                                                                                                                                                                                                                                                                                           | [Behaviour](./06-behaviour/index.md)                |
| JSON, XML, HTTP, `Uri`, `IPAddress`, reflection, snapshots                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | [Extension packages](./07-extensions/index.md)      |
| fail, skip or end a test without an expectation: `Fail`, `Skip`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                | [Test frameworks](./08-setup/01-test-frameworks.md) |

## Target frameworks

aweXpect supports test projects on .NET Framework 4.8 and on .NET 6 or later; some features need .NET 8 or later, see
[target frameworks](./08-setup/02-target-frameworks.md).

## Next steps

- [Concepts](./03-how-it-works/index.md) explains how expectations are evaluated, negated and combined,
  how they treat `null`, the options they share and how to configure their defaults.
- [Setup and integration](./08-setup/index.md) covers the test frameworks, the target frameworks and Native AOT.
- [Extending aweXpect](./11-extending/index.md) shows how to add expectations for your own types.
