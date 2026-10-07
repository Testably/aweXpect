# Test frameworks

aweXpect reports a failed, skipped or inconclusive test by throwing an exception that your test framework understands.
For MSTest, NUnit, TUnit and xUnit (v2 and v3), the `aweXpect` package generates an adapter into the test project that
throws these exceptions. Another test framework needs an adapter of its own, see
[test framework adapter](../11-extending/05-initialization.md#test-framework-adapter).

## Failing a test

`Fail.Test` fails the running test with the given reason, in the same way as a failed expectation. `Fail.When` only
fails it when the condition is `true`, and `Fail.Unless` when it is `false`:

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
