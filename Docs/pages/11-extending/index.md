# Extending aweXpect

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Core?label=aweXpect.Core)](https://www.nuget.org/packages/aweXpect.Core)

This library will never be able to cope with all ideas and use cases. Therefore, it is possible to use the
[`aweXpect.Core`](https://www.nuget.org/packages/aweXpect.Core/) package and write your own extensions.
This package aims to be more stable than the main aweXpect package, to reduce the risk of version conflicts between
different extensions.

Keep your extensions trimmable and AOT compatible like aweXpect itself, as described in
[Native AOT for extensions](./09-native-aot.md).

| Page                                                                               | Topics                                                                   |
|------------------------------------------------------------------------------------|--------------------------------------------------------------------------|
| [Constraints and results](./02-constraints-and-results.md)                         | constraints, result helpers, `null` subjects, negation, argument checks  |
| [Options and match types](./03-options-and-match-types.md)                         | options of your own results, time tolerances, custom comparisons         |
| [Collections and nested expectations](./04-collections-and-nested-expectations.md) | collection subjects, expectations on members and on collection items     |
| [Asynchronous expectations](./05-asynchronous-expectations.md)                     | asynchronous constraints, cancellation, repeated checks                  |
| [Message conventions](./06-message-conventions.md)                                 | how the failure messages of an extension should read                     |
| [Customization values](./07-customization-values.md)                               | customization values of your own, with lifetimes                         |
| [Initialization](./08-initialization.md)                                           | value formatters and test framework adapters                             |
| [Native AOT for extensions](./09-native-aot.md)                                    | the metadata your expectations need under Native AOT                     |
| [Testing and packaging](./10-testing-and-packaging.md)                             | testing an extension and referencing aweXpect.Core                       |

## Your first expectation

The samples in this section use the following namespaces:

```csharp
using aweXpect.Core;
using aweXpect.Results;
```

The samples on this and the following pages verify tracks of the following type, such as "Love Me Do" (2:22) or
"Hey Jude" (7:11):

```csharp
public record Track(string Title, TimeSpan Duration);
```

### Expectations

You can extend the expectations for any types, by adding extension methods on `IThat<TType>`.

If you want to verify that a `Track` is radio friendly, i.e. that it lasts at most three minutes, you specify the
following method signature:

```csharp no-compile
/// <summary>
///     Verifies that the <paramref name="subject"/> is radio friendly, i.e. that it lasts at most three minutes.
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

public static AndOrResult<Track, IThat<Track?>> IsRadioFriendly(this IThat<Track?> subject)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsRadioFriendlyConstraint(it, grammars)),
        subject);
```

The factory receives the name of the subject (`it`) and the `grammars` of the sentence, which the constraint uses to
write its part of the failure message. [Constraints and results](./02-constraints-and-results.md) shows how to write
the `IsRadioFriendlyConstraint`, and [message conventions](./06-message-conventions.md) how its texts should read.
