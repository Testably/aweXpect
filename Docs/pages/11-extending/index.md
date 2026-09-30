# Extending aweXpect

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Core?label=aweXpect.Core)](https://www.nuget.org/packages/aweXpect.Core)

This library will never be able to cope with all ideas and use cases. Therefore, it is possible to use the
[`aweXpect.Core`](https://www.nuget.org/packages/aweXpect.Core/) package and write your own extensions.
This package aims to be more stable than the main aweXpect package, to reduce the risk of version conflicts between
different extensions.

Keep your extensions trimmable and AOT compatible like aweXpect itself, as described in
[Native AOT for extensions](./06-native-aot.md).

| Page                                                       | Topics                                                                    |
|------------------------------------------------------------|---------------------------------------------------------------------------|
| [Constraints and results](./02-constraints-and-results.md) | constraints, negation, result types, nested and asynchronous expectations |
| [Message conventions](./03-message-conventions.md)         | how the failure messages of an extension should read                      |
| [Customization values](./04-customization-values.md)       | customization values of your own, with lifetimes                          |
| [Initialization](./05-initialization.md)                   | value formatters and test framework adapters                              |
| [Native AOT for extensions](./06-native-aot.md)            | the metadata your expectations need under Native AOT                      |
| [Testing and packaging](./07-testing-and-packaging.md)     | testing an extension and referencing aweXpect.Core                        |

## Your first expectation

The samples in this section use the following namespaces:

```csharp
using System.Diagnostics.CodeAnalysis;
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
clear, you have to cast the `IThat<TType>` interface to `IExpectThat<TType>`, which will then give access to the
`ExpectationBuilder` property.
To improve readability you can copy the following internal extension method into your project:

```csharp
[ExcludeFromCodeCoverage]
internal static IExpectThat<T> Get<T>(this IThat<T> subject)
{
    if (subject is IExpectThat<T> thatIs)
    {
        return thatIs;
    }

    throw new NotSupportedException("IThat<T> must also implement IExpectThat<T>");
}
```

You can then use the `ExpectationBuilder` to add an `IsRadioFriendlyConstraint`:

```csharp
public static AndOrResult<Track, IThat<Track?>> IsRadioFriendly(this IThat<Track?> subject)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsRadioFriendlyConstraint(it, grammars)),
        subject);
```

The factory receives the name of the subject (`it`) and the `grammars` of the sentence, which the constraint uses to
write its part of the failure message. [Constraints and results](./02-constraints-and-results.md) shows how to write
the `IsRadioFriendlyConstraint`, and [message conventions](./03-message-conventions.md) how its texts should read.
