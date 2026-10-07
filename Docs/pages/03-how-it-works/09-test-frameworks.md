# Test frameworks

aweXpect reports a failed, skipped or inconclusive test by throwing an exception that your test framework understands.
For MSTest, NUnit, TUnit and xUnit (v2 and v3), the `aweXpect` package generates an adapter into the test project that
throws these exceptions. Another test framework needs an adapter of its own, see
[test framework adapter](../11-extending/05-initialization.md#test-framework-adapter).

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

- The `TUnit` and `xunit.v3` packages include `TUnit.Assertions` and `xunit.v3.assert`. The rows "without" apply to a
  test project that only references `TUnit.Core` or `xunit.v3.core`, because it uses aweXpect instead of the
  assertions of the test framework.
- `XunitException` is declared in `xunit.v3.assert`. Without it, the adapter throws an exception of its own that
  implements an interface named `IAssertionException`, by which xUnit v3 recognizes an assertion failure.
- xUnit v2 is detected by `xunit.assert`, which the `xunit` package includes. A test project that only references
  `xunit.core` counts as "none detected".
- "None detected" also applies when the generated adapter is not registered, see
  [aweXpect2002](../07-analyzers.md#test-framework-adapter).

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

### Inconclusive

`Fail.Inconclusive` ends the running test as inconclusive, i.e. as neither passed nor failed, e.g. when it depends on
something that is not available:

```csharp
Fail.Inconclusive("the music service did not respond");
```

It throws the same exception as an inconclusive expectation, see
[the table of exceptions per test framework](./06-time-and-cancellation.md#outcome).

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
