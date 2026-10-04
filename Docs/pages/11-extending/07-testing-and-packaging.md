# Testing and packaging

## Testing an extension

Test your expectations with aweXpect itself. Pass a delegate that awaits the expectation, and verify the failure
message with `Throws().WithMessage(…)`. The exception type depends on the test framework, which `Throws()` leaves open:

```csharp
Track track = new("Hey Jude", new TimeSpan(0, 7, 11));

async Task Act() => await Expect.That(track).IsRadioFriendly();

await Expect.That(Act).Throws()
    .WithMessage("""
                 Expected that track
                 is radio friendly,
                 but it was 7:11 long
                 """);
```

Besides the success and the failure of the expectation, cover what a caller can combine it with:

- Pin the complete failure message, so that a change of the expectation or the result text is noticed.
- Verify the negated case with `DoesNotComplyWith(it => it.IsRadioFriendly())`, both its outcome and its message. It
  shows whether the constraint [supports the negation](./02-constraints-and-results.md#results).
- Verify a `null` subject for the expectation and for its negation, as described in
  [`null` subjects](./02-constraints-and-results.md#null-subjects).
- When the constraint calls [code of the caller](./02-constraints-and-results.md#code-of-the-caller), let that code
  throw and verify that the failure has the exception as its `InnerException`.

## Packaging

- Reference the [`aweXpect.Core`](https://www.nuget.org/packages/aweXpect.Core) package in your extension, not
  `aweXpect`. Core contains the building blocks of an extension, like the `ExpectationBuilder`, the constraints and
  results, the formatter and the customizations, and changes less often, so extensions that reference it conflict
  less with each other. Some parts only exist in aweXpect: the expectations themselves, the result types of many
  built-in expectations (e.g. `SingleItemResult`), the extension methods on the equivalency options (e.g.
  `IgnoringMember`) and the internal helpers of the built-in expectations. Copy a small helper into your extension
  instead of referencing aweXpect.
- Target `netstandard2.0`, so that the extension also works on .NET Framework, and add further target frameworks
  only if you need their APIs. The samples on these pages also compile against the `netstandard2.0` build.
- The test project that uses your extension also references the `aweXpect` package. It brings the built-in
  expectations, the [test framework adapters](./05-initialization.md#test-framework-adapter) and the
  [source generator](./06-native-aot.md) for Native AOT.

## Versioning

The `aweXpect` package depends on a minimum version of `aweXpect.Core`, and so does your extension. NuGet then uses
the highest of these minimum versions in the test project. Therefore:

- Reference the lowest version of `aweXpect.Core` that contains the API you need, so that your extension works
  together with as many versions of `aweXpect` as possible.
- A new major version of `aweXpect.Core` can contain breaking changes. Build and test your extension against it, and
  release a new version if it needs changes.
