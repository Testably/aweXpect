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

### Nullability warnings

After an expectation that fails for a `null` subject, the subject can't be `null` any more. Mark such an expectation
with the `[GuaranteesNotNull]` attribute from `aweXpect.Core`, so that the
[nullability suppressor](../07-analyzers.md) of the `aweXpect` package suppresses the nullability warnings for the
subject after your expectation, as it does after `IsNotNull()`:

```csharp no-compile
[GuaranteesNotNull]
public static AndOrResult<string, IThat<string?>> IsAbsolutePath(this IThat<string?> subject)
    => // ...
```

```csharp
string? path = "/music/album.txt";

await Expect.That(path).IsAbsolutePath();
int length = path.Length;   // no CS8602
```

- Only mark an expectation that fails for a `null` subject regardless of its other arguments, e.g. one that uses
  `ConstraintResult.WithNotNullValue<T>`. A wrongly marked expectation hides real nullability warnings.
- The expectation has to be an extension method on `IThat<TSubject>`.
- An expectation of your package before `.And`, as in `Expect.That(path).IsAbsolutePath().And.IsNotNull()`, keeps the
  subject for the suppressor only when it returns an aweXpect result for the same `IThat<TSubject>`, such as
  `AndOrResult<string, IThat<string?>>`. An expectation that returns an `IThat<…>` itself could continue with a
  different subject, so the suppressor ignores the expectations after it.

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

## Collection subjects

A constraint that enumerates a collection subject gets it from the `IEvaluationContext` with
`UseMaterializedEnumerable` instead of enumerating the subject itself. All expectations on the subject, including the
built-in ones, then share one lazily materialized copy, so that a subject which can only be enumerated once, e.g. a
query or an iterator with side effects, is enumerated at most once, also when the expectations are combined with
`.And` or `.Or`. Implement `IContextConstraint<T>` to receive the context:

```csharp
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core.EvaluationContext;

public static AndOrResult<IEnumerable<int>, IThat<IEnumerable<int>?>> HasEvenItems(
    this IThat<IEnumerable<int>?> subject, int expected)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new HasEvenItemsConstraint(it, grammars, expected)),
        subject);

private sealed class HasEvenItemsConstraint(string it, ExpectationGrammars grammars, int expected)
    : ConstraintResult.WithNotNullValue<IEnumerable<int>>(it, grammars),
        IContextConstraint<IEnumerable<int>?>
{
    private int _count;

    public ConstraintResult IsMetBy(IEnumerable<int>? actual, IEvaluationContext context)
    {
        Actual = actual;
        if (actual is not null)
        {
            _count = context.UseMaterializedEnumerable(actual).Count(item => item % 2 == 0);
            Outcome = _count == expected ? Outcome.Success : Outcome.Failure;
        }

        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("has ").Append(expected).Append(" even items");

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" had ").Append(_count).Append(" even items");

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("does not have ").Append(expected).Append(" even items");

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" did");
}
```

`await Expect.That(numbers).Contains(2).And.HasEvenItems(3)` then enumerates `numbers` only once.

- Every call for the same subject in the same evaluation returns the same sequence. It reads the subject only as far
  as it is enumerated, and a further enumeration replays the items read so far before it continues the subject.
- A subject that already is a collection is returned unchanged. For any other subject, an exception while it is
  enumerated fails the expectation like one of the [code of the caller](#code-of-the-caller).
- For an `IAsyncEnumerable<T>`, implement `IAsyncContextConstraint<T>` and call `UseMaterializedAsyncEnumerable` with
  its `CancellationToken`. The subject stays governed by the token of the first call. This method is only available
  on .NET 8 or later.

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

## Time tolerances

A `TimeToleranceResult<TType, TThat>` adds `.Within(…)` and stores the tolerance in the `TimeTolerance` options you
pass to it and to your constraint. For a date without a time of day, such as `DateOnly`, pass a `DayTolerance`
instead, as the built-in expectations do: it rejects a tolerance that is not a whole number of days with an
`ArgumentOutOfRangeException`, instead of silently dropping the part below one day.

```csharp no-compile
public static TimeToleranceResult<DateOnly, IThat<DateOnly>> IsOnSameDayAs(
    this IThat<DateOnly> subject, DateOnly expected)
{
    TimeTolerance tolerance = new DayTolerance();
    return new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsOnSameDayAsConstraint(it, grammars, expected, tolerance)),
        subject,
        tolerance);
}
```

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

## Expectations on collection items

An expectation on the items of a collection, i.e. an extension method on `ThatEnumerable.Elements<TItem>` that
follows `All()`, `AtLeast(2)` and the other quantifiers, derives from `QuantifiedCollectionConstraint<TValue, TItem>`.
`ThatEnumerable.IElements<TItem>` gives access to the quantifier and the subject:

```csharp
using aweXpect.Options;

public static AndOrResult<IEnumerable<int>, IThat<IEnumerable<int>?>> AreEven(
    this ThatEnumerable.Elements<int> elements)
{
    ThatEnumerable.IElements<int> source = elements;
    ExpectationBuilder expectationBuilder = source.Subject.Get().ExpectationBuilder;
    return new(expectationBuilder.AddConstraint((it, grammars)
            => new AreEvenConstraint(expectationBuilder, it, grammars, source.Quantifier)),
        source.Subject);
}

private sealed class AreEvenConstraint(
    ExpectationBuilder expectationBuilder,
    string it,
    ExpectationGrammars grammars,
    EnumerableQuantifier quantifier)
    : QuantifiedCollectionConstraint<IEnumerable<int>?, int>(expectationBuilder, it, grammars, quantifier,
            g => g.IsPlural() ? "are even" : "is even", "were"),
        IValueConstraint<IEnumerable<int>?>
{
    public ConstraintResult IsMetBy(IEnumerable<int>? actual)
    {
        Actual = actual;
        if (actual is not null)
        {
            foreach (int item in actual)
            {
                Record(item, item % 2 == 0);
            }

            Complete();
        }

        return this;
    }
}
```

```csharp
int[] values = [2, 4, 6];

await Expect.That(values).All().AreEven();
```

- `Record` classifies an item as matching or not matching, and `Complete` decides the outcome from the quantifier.
- The expectation text is for a single item and is never negated, as the quantifier carries the negation. The
  `Plural` grammar asks for the plural form, e.g. in "has values of which at least 2 are even".
- The verb completes the result, e.g. "but only 1 of 3 were".

The base class renders the quantifier like the built-in `Satisfy`, also when negated or nested, and adds the matching
or not matching items as context. `DoesNotComplyWith(v => v.AtLeast(2).AreEven())` then reads "is even for fewer than
2 items, but 3 of 3 were", followed by the "Matching items". Unlike the built-in expectations, it does not add the
"Collection" context.

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

### Repeated checks

An expectation that returns a `RepeatedCheckResult<TType, TThat>` lets the caller wait for a condition with
`.Within(timeout)` and `.CheckEvery(interval)`, like `Satisfies` does. Pass the same `RepeatedCheckOptions` to the
result and to the constraint, and make the check with `CheckRepeatedly`, which honours the options:

```csharp
using aweXpect.Options;

public static RepeatedCheckResult<string, IThat<string?>> Exists(this IThat<string?> subject)
{
    RepeatedCheckOptions options = new();
    return new RepeatedCheckResult<string, IThat<string?>>(subject.Get().ExpectationBuilder
            .AddConstraint((expectationBuilder, it, grammars)
                => new ExistsConstraint(expectationBuilder, it, grammars, options)),
        subject,
        options);
}

private sealed class ExistsConstraint(
    ExpectationBuilder expectationBuilder,
    string it,
    ExpectationGrammars grammars,
    RepeatedCheckOptions options)
    : ConstraintResult.WithNotNullValue<string>(it, grammars),
        IAsyncConstraint<string?>
{
    public async Task<ConstraintResult> IsMetBy(string? actual, CancellationToken cancellationToken)
    {
        Actual = actual;
        if (actual is not null)
        {
            await options.CheckRepeatedly(() =>
            {
                bool exists = File.Exists(actual);
                Outcome = exists ? Outcome.Success : Outcome.Failure;
                return Task.FromResult(exists != IsNegated);
            }, expectationBuilder, cancellationToken);
        }

        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("exists").Append(options);

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" did not exist");

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("does not exist").Append(options);

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" did exist");
}
```

```csharp
string path = "/music/album.txt";

await Expect.That(path).Exists().Within(TimeSpan.FromSeconds(5)).CheckEvery(TimeSpan.FromMilliseconds(100));
```

- `CheckRepeatedly` makes the first check immediately. When `IsRepeated` is `true`, because `Within` set a positive
  or an infinite timeout, it repeats the check in the interval until it succeeds, and makes the last check at the
  timeout.
- The check returns whether the expectation is met, so for a negated variant created with `.Invert()` it returns
  `true` when the file does *not* exist. The helper class inverts the stored `Outcome` itself.
- Appending the options writes " within …" to the expectation text when `Within` was specified.
- The cancellation token is only observed while waiting for the next check, so the first check is made even with a
  canceled token, and its result is returned when it succeeds or when `IsRepeated` is `false`. A cancellation at the
  timeout lets the last check decide. Any other cancellation during a wait is thrown and needs no handling, like in
  any asynchronous constraint.
