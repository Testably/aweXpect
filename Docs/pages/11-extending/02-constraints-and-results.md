# Constraints and results

The samples on this page use the following namespaces:

```csharp
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Formatting;
using aweXpect.Results;
using static aweXpect.Formatting.Format;
```

## Constraints

The basis for expectations are constraints. You can add different constraints to the `ExpectationBuilder` that is
available for the `IThat<T>`. They differ in the input and output parameters for the `IsMetBy` method:

| Constraint                                            | `IsMetBy` receives                         | Use it                                          |
|-------------------------------------------------------|--------------------------------------------|-------------------------------------------------|
| `IValueConstraint<T>`                                 | the actual value                           | for a synchronous check                         |
| `IAsyncConstraint<T>`                                 | the actual value and a `CancellationToken` | for asynchronous work or to observe the timeout |
| `IContextConstraint<T>`, `IAsyncContextConstraint<T>` | additionally an `IEvaluationContext`       | to share data between constraints               |

The `IEvaluationContext` allows storing and receiving data between expectations. This mechanism is used for example to
avoid enumerating an `IEnumerable` multiple times across multiple constraints.

`IsMetBy` returns a `ConstraintResult`, which decides the outcome and writes the expectation and the result texts of
the failure message:

```csharp
/// <summary>
///     This example does NOT support the negated case!
/// </summary>
private sealed class IsAbsolutePathConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult(grammars),
        IValueConstraint<string?>
{
    private string? _actual;
    public ConstraintResult IsMetBy(string? actual)
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

    public override ConstraintResult Negate()
        => throw new NotSupportedException("Negation of IsAbsolutePath is not supported.");
}
```

`Negate()` is called whenever the expectation is negated, e.g. by `DoesNotComplyWith(x => x.IsAbsolutePath())`, and
the caller relies on the returned result being negated. Returning `this` unchanged would silently check the
non-negated expectation instead, so a constraint that cannot be negated throws, like the built-in `ExecutesIn()`.

:::note[Older target frameworks]
`NotNullWhenAttribute` is missing in `netstandard2.0` and `net48`. Declare it as an `internal` type in your own package,
e.g. with the [Nullable](https://www.nuget.org/packages/Nullable) package.
:::

## Results

All constraints should also provide the expectations and results for the negated case, so that they are compatible
with `DoesNotComplyWith`. In order to streamline common cases, the recommended practice is to use the same class also
for the `ConstraintResult`, in most cases with one of the following helper classes:

| Helper class                           | `null` subject                                   | Use it when the expectation                         |
|----------------------------------------|--------------------------------------------------|-----------------------------------------------------|
| `ConstraintResult.WithNotNullValue<T>` | fails the expectation and its negation           | inspects the subject                                |
| `ConstraintResult.WithEqualToValue<T>` | is an ordinary value, compared with the expected | compares for equality or identity                   |
| `ConstraintResult.WithValue<T>`        | is not handled                                   | handles `null` itself or has a non-nullable subject |

With all three, you set the `Actual` property in the `IsMetBy` method and override `AppendNormalExpectation` and
`AppendNegatedExpectation` as well as `AppendNormalResult` and `AppendNegatedResult`. `WithEqualToValue<T>`
additionally takes a flag indicating if the expected value is `null`.

All three take the name of the subject (`it`) and the `grammars` in their constructor and expose the name as the
inherited `It` property, which the default result texts use.

With these the above example could be written (with support for the negated case):

```csharp
private sealed class IsAbsolutePathConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult.WithNotNullValue<string>(it, grammars),
        IValueConstraint<string?>
{
    public ConstraintResult IsMetBy(string? actual)
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

## `null` subjects

Which of the three helper classes to pick is decided by how your expectation treats a `null` subject, and that follows
the rule that all built-in expectations follow (see [`null` subjects](../03-how-it-works/04-null-subjects.md)):

> A `null` subject fails an expectation **and its negation**, unless the expectation is *about* `null`: equality and
> identity comparisons, where `null` is a legitimate value on either side, or an explicit `null` or tri-state check.

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

## Negated expectations

A constraint that supports the negated case also allows you to write an explicit negated expectation with the
`.Invert()` method:

```csharp
/// <summary>
///     Verifies that the <paramref name="subject"/> is not an absolute path.
/// </summary>
public static AndOrResult<string, IThat<string?>> IsNotAbsolutePath(
    this IThat<string?> subject)
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

## Continuing with the value

The first type argument of the result, e.g. `string` in `AndOrResult<string, IThat<string?>>`, is the type of the value
that the expectation passes on. Awaiting the expectation returns this value, and an `AndOrWhoseResult<TType, TThat>`
also continues with `Whose` on a member of it:

```csharp
public static AndOrWhoseResult<string, IThat<string?>> IsAbsolutePath(this IThat<string?> subject)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsAbsolutePathConstraint(it, grammars)),
        subject);
```

```csharp
string path = "/music/album.txt";

string verifiedPath = await Expect.That(path).IsAbsolutePath();
await Expect.That(path).IsAbsolutePath().Whose(p => p.Length, length => length.IsLessThan(260));
```

Both ask the `TryGetValue<TValue>` method of the `ConstraintResult` for the value, so a constraint that narrows or
converts the subject returns the converted value there. The helper classes return their `Actual` value. When
`TryGetValue` returns `false` for the type, awaiting the successful expectation throws a `FailException`.

## Nested expectations

An expectation that lets the caller continue with expectations on a part of the subject, like
`Throws().WithInner<T>(e => …)`, selects the part with `ForMember` and adds the caller's expectations to it:

```csharp
public static AndOrResult<string, IThat<string?>> HasFileName(
    this IThat<string?> subject,
    Action<IThatSubject<string?>> expectations)
    => new(subject.Get().ExpectationBuilder
            .ForMember(
                MemberAccessor<string?, string?>.FromFunc(path => Path.GetFileName(path), "file name "),
                (member, stringBuilder) => stringBuilder.Append("has ").Append(member).Append("that "))
            .AddExpectations(e => expectations(new ThatSubject<string?>(e)),
                grammars => grammars | ExpectationGrammars.Nested),
        subject);
```

`await Expect.That(path).HasFileName(name => name.EndsWith(".txt"))` then reads "has file name that ends with …".

- The second argument of `ForMember` writes the text in front of the nested expectations.
- The name of the member replaces `it` in their result texts ("file name was …"), unless you pass `replaceIt: false`.
- The function passed to `AddExpectations` sets the grammars of the nested expectations.
- `Validate(…)` before `AddExpectations` adds a constraint on the subject itself, e.g. based on
  `ConstraintResult.WithNotNullValue<T>` to rule out a `null` subject, as `WithInner` does.
- An exception that the member selector throws fails the nested expectations with "… did throw …".
- `ForAsyncMember` does the same for a member that has to be awaited.

## Asynchronous constraints

An `IAsyncConstraint<T>` receives the `CancellationToken` of the expectation, which is canceled when the timeout
(`WithTimeout`) elapses or the caller cancels (`WithCancellation`). Pass it on to the asynchronous work:

```csharp no-compile
public async Task<ConstraintResult> IsMetBy(string? actual, CancellationToken cancellationToken)
{
    Actual = actual;
    if (actual is not null)
    {
        Outcome = await ExistsAsync(actual, cancellationToken) ? Outcome.Success : Outcome.Failure;
    }

    return this;
}
```

- An `OperationCanceledException` from that token needs no handling: a timeout fails the expectation with "did not
  finish within …", and a cancellation by the caller leaves it inconclusive.
- A constraint that stops at the cancellation without throwing leaves its `Outcome` undecided. A timeout then fails the
  expectation in the same way. After a cancellation, the helper classes write the result text with
  `AppendUndecidedResult`, which you can override. By default it writes "it could not be verified, because the
  evaluation was already canceled".
