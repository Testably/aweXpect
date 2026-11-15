# aweXpect.Core

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Core)](https://www.nuget.org/packages/aweXpect.Core)

[aweXpect.Core](https://github.com/Testably/aweXpect/tree/main/Source/aweXpect.Core) is the engine of [aweXpect](https://docs.testably.org/aweXpect) and the API to write your own expectations and extension packages. It brings `Expect.That`, the evaluation, the failure messages, the formatting and the customization, but not the expectations for strings, numbers or collections, which are in [`aweXpect`](https://www.nuget.org/packages/aweXpect).

## Installation

To write tests, reference [`aweXpect`](https://www.nuget.org/packages/aweXpect), which brings this package with it.

An extension package references `aweXpect.Core` instead of `aweXpect`, with `PrivateAssets="contentfiles;build"`, so that its source generators also run in the projects that use the extension:

```xml
<ItemGroup>
  <PackageReference Include="aweXpect.Core" Version="…" PrivateAssets="contentfiles;build" />
</ItemGroup>
```

See [Publishing as a package](https://docs.testably.org/aweXpect/extending#publishing-as-a-package) for the version, the target frameworks and the namespace of an extension.

## At a glance

An expectation that only checks a condition is generated from a `static` method marked with `[GenerateExpectation]`:

```csharp
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatTrack
{
    [GenerateExpectation("Is{Not}RadioFriendly", "is {not} radio friendly",
        Result = nameof(DescribeDuration))]
    private static bool IsRadioFriendly(Track track)
        => track.Duration <= TimeSpan.FromMinutes(3);

    private static string DescribeDuration(Track track)
        => $"was {track.Duration:m\\:ss} long";
}
```

The source generator writes `IsRadioFriendly()` and `IsNotRadioFriendly()` with their failure messages, and `Result` names the method that describes the subject after "but". A test calls them like a built-in expectation:

```csharp
Track track = new("Hey Jude", new TimeSpan(0, 7, 11));

await That(track).IsRadioFriendly();
```

```text
Expected that track
is radio friendly,
but it was 7:11 long
```

When an expectation needs more than a condition, e.g. options, a different result or a nested expectation, add a constraint of your own through the `ExpectationBuilder`, see [Extending aweXpect](https://docs.testably.org/aweXpect/extending#expectations).

## Documentation

The full documentation is available at [docs.testably.org](https://docs.testably.org/aweXpect/extending):

- [Generated expectations](https://docs.testably.org/aweXpect/extending/generated-expectations): the method, name and texts, `null` subjects, questions that cannot be answered, collections
- [Constraints and results](https://docs.testably.org/aweXpect/extending/constraints-and-results): constraints, results, negated expectations, argument validation, continuing with the value
- [Message conventions](https://docs.testably.org/aweXpect/extending/message-conventions): the shape of a failure message, the expectation and result texts, vocabulary and grammar
- [Formatting](https://docs.testably.org/aweXpect/extending/formatting): formatting values and value formatters
- [Options](https://docs.testably.org/aweXpect/extending/options): expectations with options and options of your own
- [Comparisons](https://docs.testably.org/aweXpect/extending/comparisons): equality expectations, custom match types and property comparisons
- [Collections](https://docs.testably.org/aweXpect/extending/collections): collection subjects, expectations on collection items and expected collections
- [Nested expectations](https://docs.testably.org/aweXpect/extending/nested-expectations): expectations on a member and continuing with a new subject
- [Repeated checks and synchronous boundaries](https://docs.testably.org/aweXpect/extending/repeated-checks)
- [Customization values](https://docs.testably.org/aweXpect/extending/customization-values): values and customization groups of your own
- [Test frameworks and initialization](https://docs.testably.org/aweXpect/extending/initialization): module initializers and test framework adapters
- [Native AOT for extensions](https://docs.testably.org/aweXpect/extending/native-aot): keeping an extension trimmable and AOT compatible
