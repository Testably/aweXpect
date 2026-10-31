---
description: Install aweXpect, write your first test with an expectation and find the expectations for your subject.
---

import PackageInstall from '@site/src/components/PackageInstall';

# Getting started

## Requirements

:::info[.NET Framework 4.8 or .NET 6 or later]
aweXpect supports test projects on .NET Framework 4.8 and on .NET 6 or later; some features need .NET 8 or later, see [target frameworks](./03-setup/02-target-frameworks.md).
:::

## Install the package

<PackageInstall package="aweXpect" prerelease />

This documentation describes aweXpect v4, which is available as a pre-release, so the snippets above install the latest pre-release. Without `--prerelease` or a pre-release version you get v3, which lacks some of the APIs described here.

Add the following `using` directives: the first brings the static `Expect` class and lots of extension methods into scope, the second lets you write `That(...)` instead of `Expect.That(...)`:

```csharp
using aweXpect;
using static aweXpect.Expect;
```

:::note
The samples in this documentation write durations like `5.Seconds()` with the [aweXpect.Chronology](/aweXpect/extensions/aweXpect.Chronology) package, marked with the comment `// from the aweXpect.Chronology package`; `TimeSpan.FromSeconds(5)` works as well.
:::

## Write your first test

Every expectation starts with `That(subject)`, continues with what you expect and is awaited:

```csharp title="MusicLibraryTests.cs"
using aweXpect;
using static aweXpect.Expect;

public class MusicLibraryTests
{
    [Test]
    public async Task Contains_WhenAlbumIsMissing_ShouldReturnFalse()
    {
        MusicLibrary sut = new();

        bool result = sut.Contains("Unknown Album");

        await That(result).IsFalse();
    }
}
```

`That` is a member of the static `Expect` class. With `using static aweXpect.Expect;`, or a `global using static aweXpect.Expect;` anywhere in your test project, you can write `That(...)`; without it, write `Expect.That(...)`. All examples in this documentation assume the static using and write `That(...)`.

:::warning
An expectation is only evaluated when it is awaited: without `await` it never fails. The analyzer [aweXpect0001](./08-analyzers.md#awexpect0001) reports a missing `await` as an error.
:::

If it fails, it throws [an exception that fails the test](./03-setup/01-test-frameworks.md#failed-expectations) with the following message:

```text title="Failure message"
Expected that result
is False,
but it was True
```

## Add a reason

You can add a reason for all expectations that will be included in the exception message:

```csharp
await That(result).IsFalse().Because("the album is not in the library");
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
| [`bool`](./04-values/01-boolean.md), [numbers](./04-values/02-number.md), [`string`](./04-values/03-string.md), [`char`](./04-values/04-char.md), [enums](./04-values/05-enum.md), [`Guid`](./04-values/06-guid.md), [`Version`](./04-values/07-version.md), [`Stream`](./04-values/08-stream.md), [`TimeSpan`](./04-values/09-timespan.md), [`DateTime`](./04-values/10-datetime-offset.md), [`DateOnly`, `TimeOnly`](./04-values/11-date-time-only.md), [any object](./04-values/12-object.md), [equivalency](./04-values/13-equivalency.md) | [Values](./04-values/00-index.md)                   |
| `IEnumerable<T>`, arrays, [dictionaries](./05-collections/04-dictionaries.md)                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | [Collections](./05-collections/00-index.md)         |
| [delegates and exceptions](./06-behavior/01-delegates.md), [`Task`](./06-behavior/02-tasks.md), [events](./06-behavior/03-events.md), [callbacks](./06-behavior/04-callbacks.md)                                                                                                                                                                                                                                                                                                                                                               | [Behavior](./06-behavior/00-index.md)               |
| JSON, XML, HTTP, `Uri`, `IPAddress`, reflection, snapshots                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | [Extension packages](./07-extensions/00-index.md)   |
| fail, skip or end a test without an expectation: `Fail`, `Skip`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                | [Test frameworks](./03-setup/01-test-frameworks.md) |

## Next steps

- [How it works](./02-how-it-works/00-index.md) explains how expectations are evaluated, negated and combined, how they treat `null`, the options they share and how to configure their defaults.
- [Setup and integration](./03-setup/00-index.md) covers the test frameworks, the target frameworks and Native AOT.
- [Extending aweXpect](./11-extending/00-index.md) shows how to add expectations for your own types.
