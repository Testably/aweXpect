# Your first expectation

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Core?label=aweXpect.Core)](https://www.nuget.org/packages/aweXpect.Core)

This library will never be able to cope with all ideas and use cases. Therefore, it is possible to use the
[`aweXpect.Core`](https://www.nuget.org/packages/aweXpect.Core/) package and write your own extensions.
This package aims to be more stable than the main aweXpect package, to reduce the risk of version conflicts between
different extensions.

The samples on this page use the following namespaces:

```csharp
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core;
using aweXpect.Results;
```

## Expectations

You can extend the expectations for any types, by adding extension methods on `IThat<TType>`.

If you want to verify that a `string` is an absolute path, you specify the following method signature:

```csharp no-compile
/// <summary>
///     Verifies that the <paramref name="subject"/> is an absolute path.
/// </summary>
public static AndOrResult<string, IThat<string>> IsAbsolutePath(this IThat<string> subject)
{
    // ...
}
```

The result type decides how the expectation can continue: `AndOrResult` allows combining it with further expectations
using `.And` and `.Or`.

## ExpectationBuilder

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

You can then use the `ExpectationBuilder` to add an `IsAbsolutePathConstraint`:

```csharp
public static AndOrResult<string, IThat<string>> IsAbsolutePath(this IThat<string> subject)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsAbsolutePathConstraint(it, grammars)),
        subject);
```

The factory receives the name of the subject (`it`) and the `grammars` of the sentence, which the constraint uses to
write its part of the failure message. [Constraints and results](./02-constraints-and-results.md) shows how to write
the `IsAbsolutePathConstraint`, and [message conventions](./03-message-conventions.md) how its texts should read.
