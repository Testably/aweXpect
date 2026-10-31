---
description: How aweXpect fails, skips and ends tests as inconclusive in MSTest, NUnit, TUnit and xUnit, and what it knows about the running test.
---

# Test frameworks

aweXpect reports a failed, skipped or inconclusive test by throwing an exception that your test framework understands.
It supports MSTest 3 and 4, NUnit 3, 4 and 5, TUnit, and xUnit v2 and v3: for them, the `aweXpect` package generates
an adapter into the test project that throws these exceptions. Another test framework needs an adapter of its own, see
[test framework adapter](../11-extending/11-initialization.md#test-framework-adapter).

## Failed expectations

A failed expectation throws the assertion exception of the test framework, when the test project references the
assembly that declares it:

| Test framework                     | Thrown exception                                 |
|------------------------------------|--------------------------------------------------|
| MSTest                             | `AssertFailedException`                          |
| NUnit                              | `AssertionException`                             |
| TUnit                              | `TUnit.Assertions.Exceptions.AssertionException` |
| TUnit without `TUnit.Assertions`   | `aweXpect.FailException`                         |
| xUnit v3                           | `Xunit.Sdk.XunitException`                       |
| xUnit v3 without `xunit.v3.assert` | an exception marked as assertion failure         |
| xUnit v2                           | `Xunit.Sdk.XunitException`                       |
| none detected                      | `aweXpect.FailException`                         |

Each of them fails the test, and has the exception that caused the failure, if any, as inner exception.

<details>
<summary>When the "without" and "none detected" rows apply</summary>

- The `TUnit` and `xunit.v3` packages include `TUnit.Assertions` and `xunit.v3.assert`. The rows "without" apply to a
  test project that only references `TUnit.Core` or `xunit.v3.core`, because it uses aweXpect instead of the
  assertions of the test framework.
- `XunitException` is declared in `xunit.v3.assert`. Without it, the adapter throws an exception of its own that
  implements an interface named `IAssertionException`, by which xUnit v3 recognizes an assertion failure.
- xUnit v2 is detected by `xunit.assert`, which the `xunit` package includes. A test project that only references
  `xunit.core` counts as "none detected".
- "None detected" also applies when the generated adapter is not registered, see
  [aweXpect2002](../08-analyzers.md#test-framework-adapter).
- The loaded assemblies are only scanned for a test framework adapter below .NET 8, e.g. on .NET Framework, skipping
  the [`ExcludedAssemblyPrefixes`](../02-how-it-works/07-configuration.md#settings). On .NET 8 or later the generated
  adapter registers itself when the test assembly is loaded, so `ExcludedAssemblyPrefixes` has no effect on the test
  framework detection there. This registration needs C# 9 or later, see
  [aweXpect2002](../08-analyzers.md#test-framework-adapter).

</details>

## Failing a test

`Fail.Test` fails the running test with the given reason, with [the same exception](#failed-expectations) as a failed
expectation. `Fail.When` only fails it when the condition is `true`, and `Fail.Unless` when it is `false`:

```csharp
List<Track> playlist = // ...
string? title = // ...

Fail.When(playlist.Count == 0, "the playlist should contain tracks");
Fail.Unless(title is not null, "the first track should have a title");
if (title.Length > 100)
{
  Fail.Test("the title should fit on the cover");
}
```

The reason is the whole message of the thrown exception. `Fail.Test` never returns, and after `Fail.When` or
`Fail.Unless` the compiler knows the result of the condition, so `title` is not `null` in the code that follows.

Each of the three methods also accepts the exception that caused the failure as last parameter, which becomes the
inner exception of the thrown exception:

```csharp
try
{
  File.ReadAllText("playlist.json");
}
catch (IOException exception)
{
  Fail.Test("the playlist should be readable", exception);
}
```

## Inconclusive

`Fail.Inconclusive` ends the running test as inconclusive, i.e. as neither passed nor failed, e.g. when it depends on
something that is not available:

```csharp
Fail.Inconclusive("the music service did not respond");
```

It throws the same exception as an expectation that was
[canceled](../02-how-it-works/06-time-and-cancellation.md#outcome):

| Test framework | Thrown exception                    | Reported as                      |
|----------------|-------------------------------------|----------------------------------|
| MSTest         | `AssertInconclusiveException`       | inconclusive                     |
| NUnit          | `InconclusiveException`             | inconclusive                     |
| TUnit          | `InconclusiveTestException`         | inconclusive                     |
| xUnit v3       | an exception marked as test timeout | timed out                        |
| xUnit v2       | `aweXpect.InconclusiveException`    | failed (no inconclusive outcome) |
| none detected  | `aweXpect.InconclusiveException`    | depends on the runner            |

## Skipping a test

`Skip.Test` skips the running test with the given reason, when this can only be decided while the test runs.
`Skip.When` only skips it when the condition is `true`, and `Skip.Unless` when it is `false`:

```csharp
string? apiKey = Environment.GetEnvironmentVariable("MUSIC_API_KEY");

Skip.When(apiKey is null, "the MUSIC_API_KEY is not set");
Skip.Unless(apiKey.Length == 32, "the MUSIC_API_KEY is not valid");
```

The exception that skips the test depends on the test framework:

| Test framework | Thrown exception                                        | Reported as                               |
|----------------|---------------------------------------------------------|-------------------------------------------|
| MSTest         | `AssertInconclusiveException`                           | inconclusive                              |
| NUnit          | `IgnoreException`                                       | ignored                                   |
| TUnit          | `SkipTestException`                                     | skipped                                   |
| xUnit v3       | `aweXpect.SkipException`, marked as dynamically skipped | skipped                                   |
| xUnit v2       | `aweXpect.SkipException`                                | failed (a running test cannot be skipped) |
| none detected  | `aweXpect.SkipException`                                | depends on the runner                     |

:::note
TUnit declares a `Skip` class of its own in the `TUnit.Core` namespace. Where the compiler reports the name as
ambiguous, write `aweXpect.Skip`.
:::

## The running test

Besides throwing its exceptions, the adapter tells aweXpect and its extensions about the running test, as far as the
test framework exposes it:

- the running test case with its arguments, e.g. to
  [name a snapshot after the test case](/aweXpect/extensions/aweXpect.Snapshots/naming-snapshots#naming-after-the-test-case),
- the cancellation token of the test, e.g. to cancel the expectations with
  [`TestCancellation.FromCancellationToken(…)`](../02-how-it-works/06-time-and-cancellation.md#cancellationtoken),
- and attachments to the result of the test, e.g. the
  [pending file of a snapshot](/aweXpect/extensions/aweXpect.Snapshots/accepting-snapshots#on-a-build-server).

| Test framework       | Test case | Arguments of the test class | Retries                    | Cancellation token | Attachments |
|----------------------|-----------|-----------------------------|----------------------------|--------------------|-------------|
| MSTest 4\*           | opt-in    | no                          | no                         | opt-in             | opt-in      |
| MSTest 3             | no        | no                          | no                         | no                 | no          |
| NUnit 4 or later     | yes       | yes                         | yes, also `[Repeat]` runs  | yes                | yes         |
| NUnit 3              | yes       | no                          | yes, also `[Repeat]` runs  | no                 | yes         |
| TUnit                | yes       | yes                         | yes                        | yes                | yes         |
| xUnit v3             | yes       | no                          | no                         | yes                | yes         |
| xUnit v2             | no        | no                          | no                         | no                 | no          |
| none detected        | no        | no                          | no                         | no                 | no          |

\* MSTest 4 marks the API for the running test as experimental, so the adapter only uses it when the test project
opts in:

```xml
<PropertyGroup>
  <AweXpectExperimentalMSTest>true</AweXpectExperimentalMSTest>
</PropertyGroup>
```

<details>
<summary>Arguments, retries and attachments in detail</summary>

- The arguments of the test class are those of a parameterized fixture of NUnit and of a TUnit test class, including
  the objects that the test framework injects, e.g. a shared fixture. xUnit v3 and MSTest have no parameterized test
  classes.
- A retry counts the runs of the test before, starting with `0`. NUnit also counts the runs of `[Repeat]`, and xUnit v3
  and MSTest have no retries.
- MSTest, NUnit and TUnit read an attached file when the test ends, so it must still exist then. xUnit v3 attaches the
  content of the file immediately.
- With the opt-in, the generated MSTest adapter suppresses the diagnostic `MSTESTEXP` in its own code.

</details>
