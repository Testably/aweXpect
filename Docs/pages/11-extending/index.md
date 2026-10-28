# Extending aweXpect

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Core?label=aweXpect.Core)](https://www.nuget.org/packages/aweXpect.Core)

This library will never be able to cope with all ideas and use cases. Therefore, it is possible to use the
[`aweXpect.Core`](https://www.nuget.org/packages/aweXpect.Core/) package and write your own extensions.
This package aims to be more stable than the main aweXpect package, to reduce the risk of version conflicts between
different extensions.

<details>
<summary>Namespaces used on this page</summary>

```csharp
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Formatting;
using aweXpect.Results;
using static aweXpect.Formatting.Format;
```

</details>

| Page                                                                  | Topics                                                                                                                                                                                         |
|-----------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [Generated expectations](./01-generated-expectations.md)              | the method, name and texts, `null` subjects, questions that cannot be answered, collections, diagnostics                                                                                       |
| [Constraints and results](./02-constraints-and-results.md)            | constraints, results, negated expectations, `null` subjects, argument validation, continuing with the value, code of the caller, failing both ways, side effects, constraints without closures |
| [Message conventions](./03-message-conventions.md)                    | shape of a failure message, expectation text, result text, vocabulary, grammar, contexts, exceptions                                                                                           |
| [Formatting](./04-formatting.md)                                      | formatting values, value formatters, formatters of an expectation                                                                                                                              |
| [Options](./05-options.md)                                            | an expectation with options, one holder per expectation, setting options in your own code, options of your own, results that are stored, time tolerances                                       |
| [Comparisons](./06-comparisons.md)                                    | equality expectations, custom match types, comparing with the options, property comparisons                                                                                                    |
| [Collections](./07-collections.md)                                    | collection subjects, expectations on collection items, expected collections                                                                                                                    |
| [Nested expectations](./08-nested-expectations.md)                    | expectations on a member, continuing with a new subject, members that need the evaluation, expectations in expected values                                                                     |
| [Repeated checks and synchronous boundaries](./09-repeated-checks.md) | repeated checks, synchronous boundaries                                                                                                                                                        |
| [Customization values](./10-customization-values.md)                  | simple values, list values, customization groups, values in a built-in group, excluded assemblies                                                                                              |
| [Test frameworks and initialization](./11-initialization.md)          | module initializers, test framework adapter, registration from the test project                                                                                                                |
| [Native AOT for extensions](./12-native-aot.md)                       | generic parameters, registering metadata yourself, the source generator, reflection in an extension                                                                                            |

## Your first expectation

The samples on this and the following pages verify tracks of the following type, such as "Love Me Do" (2:22) or
"Hey Jude" (7:11):

```csharp
public record Track(string Title, TimeSpan Duration);
```

### Generated from a condition

If you want to verify that a `Track` is radio friendly, i.e. that it lasts at most three minutes, the simplest way is a
method that checks the condition, marked with `[GenerateExpectation]`:

```csharp no-compile
using aweXpect.Core.Extending;

public static partial class ThatTrack
{
    [GenerateExpectation("Is{Not}RadioFriendly", "is {not} radio friendly")]
    private static bool IsRadioFriendly(Track track)
        => track.Duration <= TimeSpan.FromMinutes(3);
}
```

The source generator in `aweXpect.Core` writes `IsRadioFriendly()` and `IsNotRadioFriendly()` with their failure
messages, see [generated expectations](./01-generated-expectations.md). With `[GenerateCollectionExpectation]`, the
same method also verifies the items of a collection, e.g. `That(tracks).All().AreRadioFriendly()`.

When an expectation needs more than a condition, e.g. options, a different result or a nested expectation, write it
yourself as described in the rest of this section.

### Expectations

You can extend the expectations for any types, by adding extension methods on `IThat<TType>`.

To write `IsRadioFriendly` yourself, you specify the following method signature:

```csharp no-compile
/// <summary>
///     Verifies that the <paramref name="subject"/> is radio friendly,
///     i.e. that it lasts at most three minutes.
/// </summary>
public static AndOrResult<Track, IThat<Track?>> IsRadioFriendly(this IThat<Track?> subject)
{
    // ...
}
```

The result type decides how the expectation can continue: `AndOrResult` allows combining it with further expectations
using `.And` and `.Or`.

### ExpectationBuilder

The next step is to extract the `ExpectationBuilder`. In order to keep the automatic code suggestions for developers
clear, `IThat<TType>` doesn't show it. The `Get()` extension method in the `aweXpect.Core.Extending` namespace gives
access to it. This namespace holds the helpers for extension authors that extend types every user sees, so they only
appear where you import it.

You can then use the `ExpectationBuilder` to add an `IsRadioFriendlyConstraint`:

```csharp
using aweXpect.Core.Extending;

public static partial class ThatTrack
{
    public static AndOrResult<Track, IThat<Track?>> IsRadioFriendly(this IThat<Track?> subject)
        => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
                => new IsRadioFriendlyConstraint(it, grammars)),
            subject);
}
```

The factory receives the name of the subject (`it`) and the `grammars` of the sentence, which the constraint uses to
write its part of the failure message.

### The constraint

The constraint decides whether the subject meets the expectation and writes its part of the failure message. Declare
it as a nested class of `ThatTrack`, next to the expectation, and derive it from the helper class
`ConstraintResult.WithNotNullValue<Track>`, which fails the expectation for a `null` track:

```csharp
private sealed class IsRadioFriendlyConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult.WithNotNullValue<Track>(it, grammars),
        IValueConstraint<Track?>
{
    public ConstraintResult IsMetBy(Track? actual)
    {
        Actual = actual;
        Outcome = actual?.Duration <= TimeSpan.FromMinutes(3) ? Outcome.Success : Outcome.Failure;
        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is radio friendly");

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(It).Append(" was ");
        Formatter.Format(stringBuilder, Actual?.Duration);
        stringBuilder.Append(" long");
    }

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is not radio friendly");

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(It).Append(" was ");
        Formatter.Format(stringBuilder, Actual?.Duration);
        stringBuilder.Append(" long");
    }
}
```

- `IsMetBy` receives the track, keeps it as `Actual` and decides the `Outcome`.
- `AppendNormalExpectation` writes the expectation text and `AppendNormalResult` the result text of the failure
  message. The negated variants write them for the negated expectation, e.g. in
  `DoesNotComplyWith(t => t.IsRadioFriendly())`.

[Constraints and results](./02-constraints-and-results.md) explains the other kinds of constraints and results, and
[message conventions](./03-message-conventions.md) how the texts should read.

### Using it in a test

A test project that references your extension and aweXpect calls the expectation like a built-in one. As `ThatTrack`
is in the namespace `aweXpect` (see [namespace and class names](#namespace-and-class-names)), the test needs no
further `using`:

```csharp
[Test]
public async Task Track_ShouldBeRadioFriendly()
{
    Track track = new("Hey Jude", new TimeSpan(0, 7, 11));

    await Expect.That(track).IsRadioFriendly();
}
```

For "Hey Jude", the test fails with:

```text title="Failure message"
Expected that track
is radio friendly,
but it was 7:11 long
```

## Publishing as a package

A test project that declares expectations for its own types needs none of this setup: it is for an extension that you
publish as a package.

### Referencing aweXpect.Core

Reference `aweXpect.Core` in your extension, not `aweXpect`, and use the lowest version that contains the API you
need, so that the extension works together with as many versions of `aweXpect` as possible.

By default, a package marks the analyzers of its dependencies as private (`exclude="Analyzers"` in its `.nuspec`).
Reference `aweXpect.Core` with `PrivateAssets="contentfiles;build"`, so that its source generators also run in the
projects that use your extension, see [Native AOT for extensions](./12-native-aot.md#the-source-generator):

```xml
<ItemGroup>
  <PackageReference Include="aweXpect.Core" Version="…" PrivateAssets="contentfiles;build" />
</ItemGroup>
```

### Target frameworks

`aweXpect.Core` targets `netstandard2.0`, `net8.0` and `net10.0`, like the extension packages of aweXpect. Include
`netstandard2.0` in the targets of your extension, so that it also works in test projects on .NET Framework.

Keep your extensions trimmable and AOT compatible like aweXpect itself, as described in
[Native AOT for extensions](./12-native-aot.md).

### Namespace and class names

Declare your expectations in the namespace `aweXpect`, which the users import for `Expect.That` anyway, so that they
need no further `using` for your extension. Name the class after the subject type, e.g. `ThatTrack` for the
expectations on a `Track`, and make it `partial`, so that each expectation can have a file of its own, like
`ThatJsonElement.HasProperty.cs` in `aweXpect.Json`:

```csharp no-compile
namespace aweXpect;

public static partial class ThatTrack
{
    // the expectations on a Track
}
```

### Polyfills for older targets

Some attributes that an extension uses are compiler features that are missing in `netstandard2.0` and `net48`. Declare
them as `internal` types in your own package there:

- `CallerArgumentExpressionAttribute` (C# 10) shows the code of the caller in the expectation text, see
  [message conventions](./03-message-conventions.md#code-of-the-caller-in-the-expectation-text).
- `ModuleInitializerAttribute` (C# 9) runs code before the first expectation, see
  [module initializers](./11-initialization.md#module-initializers). The source generators of `aweXpect.Core` also
  need it, see [the source generator](./12-native-aot.md#the-source-generator).

```csharp no-compile
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute
    {
        public string ParameterName { get; } = parameterName;
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class ModuleInitializerAttribute : Attribute;
}
```
