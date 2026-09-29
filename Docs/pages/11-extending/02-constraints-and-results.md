# Constraints and results

The basis for expectations are constraints. You can add different constraints to the `ExpectationBuilder` that is
available for the `IThat<T>`. They differ in the input and output parameters for the `IsMetBy` method:

- `IValueConstraint<T>`   
  It receives the actual value `T` and returns a `ConstraintResult`.
- `IAsyncConstraint<T>`  
  It receives the actual value `T` and a `CancellationToken` and returns the `ConstraintResult` asynchronously.  
  *Use it when you need asynchronous functionality or access to the timeout `CancellationToken`.*
- `IContextConstraint<T>` / `IAsyncContextConstraint<T>`  
  Similar to the `IValueConstraint<T>` and `IAsyncConstraint<T>` respectively but receives an additional
  `IEvaluationContext` parameter that allows storing and receiving data between expectations.  
  *This mechanism is used for example to avoid enumerating an `IEnumerable` multiple times across multiple constraints.*

```csharp
/// <summary>
///     This example does NOT support the negated case!
/// </summary>
private sealed class IsAbsolutePathConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult(grammars),
        IValueConstraint<string>
{
    private string? _actual;
    public ConstraintResult IsMetBy(string actual)
    {
        _actual = actual;
        Outcome = Path.IsPathRooted(actual) ? Outcome.Success : Outcome.Failure;
        return this;
    }

    public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is an absolute path");

    public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(it).Append(" was ");
        Formatter.Format(stringBuilder, _actual);
    }

    public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
    {
        if (_actual is TValue typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return typeof(TValue).IsAssignableFrom(typeof(string));
    }

    public override ConstraintResult Negate() => this;
}
```

:::note[Older target frameworks]
`NotNullWhenAttribute` is missing in `netstandard2.0` and `net48`. Declare it as an `internal` type in your own package,
e.g. with the [Nullable](https://www.nuget.org/packages/Nullable) package.
:::

All constraints should also provide the expectations and results for the negated case (so that they are compatible with
`DoesNotComplyWith`).

In order to streamline common cases, the recommended practice is to use the same class also for the `ConstraintResult`;
in most cases with one of the following helper classes:

- `ConstraintResult.WithValue<T>`
  You have to set the `Actual` property in the `IsMetBy` method and overwrite `AppendNormalExpectation` and
  `AppendNegatedExpectation` as well as either the corresponding `AppendNormalResult` and `AppendNegatedResult` or the
  common method for both cases `AppendResult` (when the result text is identical in both cases)
- `ConstraintResult.WithNotNullValue<T>`
  Similar to `ConstraintResult.WithValue<T>`, but will automatically include a check that Actual is not `null` with the
  generic result text.
- `ConstraintResult.WithEqualToValue<T>`
  Ensures consistent `null`-handling when comparing two values for equality. Similar to `ConstraintResult.WithValue<T>`,
  but you have to also provide a flag indicating if the expected value is `null` or not.

All three take the name of the subject (`it`) and the `grammars` in their constructor and expose the name as the
inherited `It` property, which the default result texts use.

Which of the three to pick is decided by how your expectation treats a `null` subject, and that follows the rule that
all built-in expectations follow (see [concepts](./02-concepts.md#null-subjects)):

> A `null` subject fails an expectation **and its negation**, unless the expectation is *about* `null`: equality and
> identity comparisons, where `null` is a legitimate value on either side, or an explicit `null` or tri-state check.

A `null` subject does not mean "the expectation is false", it means there is no value to inspect and the question
cannot be answered. Negating an unanswerable question does not make it true, which is why the rule covers the negated
case as well.

- Your expectation **inspects the subject**: its length, its type, its items, whether it is empty. There is nothing to
  inspect when the subject is `null`, so it has to fail, in the negated case as well: `IsNotEmpty()` fails for a `null`
  subject just like `IsEmpty()` does, and so does `DoesNotComplyWith(x => x.IsEmpty())`. Use
  `ConstraintResult.WithNotNullValue<T>`.
- Your expectation **compares the subject for equality or identity** against a value the caller supplied. Then `null`
  is an ordinary value on both sides: `IsEqualTo(null)` succeeds for a `null` subject, `IsNotEqualTo(null)` fails and
  `IsNotEqualTo("foo")` succeeds. Use `ConstraintResult.WithEqualToValue<T>` and pass whether the expected value is
  `null`; that flag is what makes the subject fail on the side where `null` is not a legitimate answer.

Do not read the second case as "any value the caller supplied": `HasValue(2)` takes one and still fails for `null`,
because it inspects the subject rather than comparing it. Only equality and identity give `null` a meaning on both
sides; an ordering or a range does not, which is why `IsGreaterThan` and `IsNotBetween` use
`ConstraintResult.WithNotNullValue<T>`.

Use `ConstraintResult.WithValue<T>` only when the subject cannot be `null` at all (a non-nullable `bool`, `int` or
`DateTime`), when your expectation is one of the `null` checks that a `null` subject is meant to satisfy, such as
`IsNull()`, or when it decides every `null` case itself, such as `IsOneOf(...)`, whose expected values may or may not
include `null`, which the single flag of `WithEqualToValue<T>` cannot express. It applies no `null` policy of its own,
so deciding the outcome with `Actual is null ? Outcome.Failure : ...` inside `IsMetBy` is **not** enough: that failure
is inverted into a success when the expectation is negated. Only `WithNotNullValue<T>` decides before the inversion is
applied.

With these the above example could be written (with support for the negated case):

```csharp
private sealed class IsAbsolutePathConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult.WithNotNullValue<string>(it, grammars),
        IValueConstraint<string>
{
    public ConstraintResult IsMetBy(string actual)
    {
        Actual = actual;
        Outcome = Path.IsPathRooted(actual) ? Outcome.Success : Outcome.Failure;
        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is an absolute path");

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(It).Append(" was ");
        Formatter.Format(stringBuilder, Actual);
    }

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is not an absolute path");

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(It).Append(" was ");
        Formatter.Format(stringBuilder, Actual);
    }
}
```

Note that the `it` parameter is passed to the base class and the inherited `It` property is used in the body: capturing
the parameter *and* passing it to the base stores it twice, which the compiler warns about (CS9107).

This then also allows you to write an explicit negated expectation with the same constraint using the `.Invert()`
method:

```csharp
/// <summary>
///     Verifies that the <paramref name="subject"/> is not an absolute path.
/// </summary>
public static AndOrResult<string, IThat<string>> IsNotAbsolutePath(
    this IThat<string> subject)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsAbsolutePathConstraint(it, grammars).Invert()),
        subject);
```

## Code of the caller

When a constraint calls code of the caller while it is evaluated, e.g. a predicate, a member selector or a comparer,
call it through `UserCode.Invoke`. An exception it throws then fails the expectation and its negation alike, with the
exception as inner exception, instead of aborting the evaluation:

```csharp no-compile
Outcome = UserCode.Invoke(predicate, actual, "the predicate") ? Outcome.Success : Outcome.Failure;
```

The optional last argument names the code in the failure message, e.g. "the predicate did throw an
InvalidOperationException"; without it, the subject is named ("it did throw …").

An exception that your constraint throws itself, e.g. to reject an invalid argument, is still thrown as it is.
