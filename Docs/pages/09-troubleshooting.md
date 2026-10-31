---
description: Short answers to the problems that come up most often with aweXpect, from an expectation that never fails to compiler errors and test framework questions.
---

# Troubleshooting

Short answers to the problems that come up most often, each with a link to the page that explains the details.

## Expectations

### My expectation never fails

An expectation is only evaluated when it is awaited. Without `await` it is never evaluated, so it never fails, whatever the subject is:

```csharp
bool isPlayed = false;

That(isPlayed).IsTrue();         // never evaluated: the test passes
await That(isPlayed).IsTrue();   // evaluated: the test fails
```

The analyzer [aweXpect0001](./08-analyzers.md#awexpect0001) reports the missing `await` as an error. Its code fix adds the `await` and makes the enclosing method, local function or lambda `async` where needed. Where you cannot await, e.g. for a `Span<T>`, verify the expectation [synchronously](./02-how-it-works/00-index.md#when-you-cannot-await).

If the analyzer reports nothing, check your compiler: the analyzers need Roslyn 4.8 or later, which ships with the .NET 8 SDK and Visual Studio 2022 17.8. An older compiler skips them with warning `CS8032`.

### My expectation in a `ForEach` lambda never fails

An expectation that is awaited in an `async void` method or lambda, e.g. in `list.ForEach(async x => await That(x)...)`, throws its failure after the test has completed, so the test does not observe it. The analyzer [aweXpect0005](./08-analyzers.md#awexpect0005) warns about it. Await the expectation in the test method itself, e.g. in a `foreach` loop, or return a `Task` instead.

## Coming from FluentAssertions

### How does the syntax differ from FluentAssertions?

An expectation starts with `That(subject)` instead of `subject.Should()`, the expectations read as `Is…`, `Has…` or `Contains` instead of `Be…`, `Have…` or `Contain`, and the whole expectation is awaited:

```csharp no-compile title="FluentAssertions"
title.Should().StartWith("Let").And.EndWith("Be", "it is the last track");
act.Should().Throw<InvalidOperationException>().WithMessage("Yesterday");
```

```csharp title="aweXpect"
string title = "Let It Be";
Action act = () => throw new InvalidOperationException("Yesterday");

await That(title).StartsWith("Let").And.EndsWith("Be").Because("it is the last track");
await That(act).Throws<InvalidOperationException>().WithMessage("Yesterday");
```

- `.Should().Be(x)` becomes `IsEqualTo(x)`, and `BeTrue()` becomes `IsTrue()`. The [Getting started](./01-getting-started.md#find-the-expectation-you-need) page lists where to find the expectations for each subject.
- The `because` parameter becomes [`.Because(reason)`](./02-how-it-works/05-options.md#because), appended to the expectation it belongs to.
- `.And` combines expectations on the same subject as in FluentAssertions, and `.Or` requires only one of them, see [combining expectations](./02-how-it-works/03-combining.md#on-the-same-subject).
- Instead of an `AssertionScope`, [`Expect.ThatAll`](./02-how-it-works/03-combining.md#on-different-subjects) combines expectations on different subjects and reports all failures together.

### Can I migrate my FluentAssertions tests automatically?

Yes. The [aweXpect.Migration](/aweXpect/migration) package reports every FluentAssertions and xUnit assertion, and its code fix rewrites it to aweXpect where it knows an equivalent. A second step with the code fix of aweXpect0001 adds the missing `await`. The rewrites are listed on [Migrating from FluentAssertions](/aweXpect/migration/fluentassertions).

:::warning
aweXpect is strict by default, so some rewritten expectations fail where FluentAssertions passed, e.g. `WithMessage` compares the whole message unless you add `.AsWildcard()`. The [behavioral differences](/aweXpect/migration/fluentassertions#behavioral-differences) list these cases.
:::

## Installation and setup

### The documented API is missing

This documentation describes aweXpect v4, which is available as a pre-release. Without a pre-release version you get v3, which lacks some of the APIs described here. [Getting started](./01-getting-started.md#install-the-package) shows how to install the pre-release.

When you upgrade from v3, some expectations were renamed without an `[Obsolete]` forwarder, so their old names no longer compile. [What's new in v4](./10-migration/04-from-awexpect-3.md#renamed-expectations) lists the renames.

### Which versions of the extension packages do I need?

From v4 on, `aweXpect`, `aweXpect.Core`, `aweXpect.Json`, `aweXpect.Web`, `aweXpect.Xml`, `aweXpect.Reflection` and the `aweXpect.Snapshots` packages are released together with one version. Use the same version for all of them, see [one version for all packages](./10-migration/04-from-awexpect-3.md#one-version-for-all-packages).

### The name `That` does not exist in the current context

`That` is a member of the static `Expect` class. Add `using static aweXpect.Expect;`, or a `global using static aweXpect.Expect;` anywhere in your test project, or write `Expect.That(...)`, see [Getting started](./01-getting-started.md#install-the-package).

### `Seconds()` does not exist

The samples in this documentation write durations like `5.Seconds()` with the [aweXpect.Chronology](/aweXpect/extensions/aweXpect.Chronology) package. Install it, or write `TimeSpan.FromSeconds(5)` instead.

## Compiler errors and warnings

### CS0121: The call is ambiguous

Some overloads of aweXpect rely on `[OverloadResolutionPriority]`, which only takes effect with C# 13 or later. C# 13 is the default language version only from .NET 9 on, so a test project with an older language version, e.g. on .NET 8 where C# 12 is the default, can fail with CS0121 in these cases:

- An `async` lambda or a lambda that only throws, passed to `That(...)`, fits both the `Task` and the `ValueTask` overload, see [Delegates](./06-behavior/01-delegates.md).
- A collection expectation with a `string` as single item, a `params` list or a collection expression `[…]` can bind to a different overload or be ambiguous, see [Collections](./05-collections/00-index.md).

```csharp
Task PlayAsync() => Task.CompletedTask;

await That(async () => await PlayAsync()).DoesNotThrow();   // CS0121 with C# 12 on .NET 8
await That(() => PlayAsync()).DoesNotThrow();               // binds to the Task overload
```

Set the language version to 13 or later in the test project:

```xml
<PropertyGroup>
  <LangVersion>13</LangVersion>
</PropertyGroup>
```

For a delegate, you can also return the task directly, as in the second line above, or declare a local function and pass it.

In aweXpect.Json v4, `IsValidJson(null)` without a parameter name is ambiguous as well; write `IsValidJson(options: null)`, see [What's new in v4](./10-migration/04-from-awexpect-3.md#renamed-expectations).

### `Skip` is an ambiguous reference

TUnit declares a `Skip` class of its own in the `TUnit.Core` namespace. Where the compiler reports the name as ambiguous, write `aweXpect.Skip`, see [skipping a test](./03-setup/01-test-frameworks.md#skipping-a-test).

### CS8602 after `IsNotNull()`

After an expectation like `IsNotNull()`, aweXpect suppresses the nullability warnings for the subject, but only for a local variable or a parameter that is checked in a preceding statement of the same method, see the [nullability suppressor](./08-analyzers.md#nullability-suppressor). For a property or another expression, use the result of the expectation instead, which is not nullable:

```csharp
Track track = new() { Title = "Let It Be" };

string title = await That(track.Title).IsNotNull();
int length = title.Length;
```

## Test frameworks

### Which test frameworks does aweXpect support?

MSTest 3 and 4, NUnit 3, 4 and 5, TUnit, and xUnit v2 and v3. The `aweXpect` package generates an adapter for them into the test project, so you don't have to configure anything. Another test framework needs an adapter of its own, see [test frameworks](./03-setup/01-test-frameworks.md).

### A failure throws `aweXpect.FailException` instead of the exception of my test framework

A failed expectation throws the assertion exception of the test framework when the test project references the assembly that declares it. Otherwise it throws `aweXpect.FailException`, which fails the test as well, e.g.:

- in a TUnit project that only references `TUnit.Core` and not `TUnit.Assertions`,
- in an xUnit v2 project that only references `xunit.core` and not `xunit.assert`,
- in a test project on .NET 8 or later that sets `<LangVersion>` below 9, where the generated adapter cannot register itself, see [aweXpect2002](./08-analyzers.md#test-framework-adapter).

The thrown exception for each test framework is listed under [failed expectations](./03-setup/01-test-frameworks.md#failed-expectations).

### A skipped test is reported as failed

xUnit v2 cannot skip a running test, so `Skip.Test`, `Skip.When` and `Skip.Unless` fail it instead, and `Fail.Inconclusive` fails it as well, as xUnit v2 has no inconclusive outcome. When no test framework is detected, as described above, aweXpect throws its own `aweXpect.SkipException` or `aweXpect.InconclusiveException`, and the outcome depends on the runner. The outcome for each test framework is listed under [skipping a test](./03-setup/01-test-frameworks.md#skipping-a-test) and [inconclusive](./03-setup/01-test-frameworks.md#inconclusive).
