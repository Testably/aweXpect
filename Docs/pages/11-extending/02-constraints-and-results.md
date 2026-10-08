# Constraints and results

The samples on this page use the following namespaces:

```csharp
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

`IsMetBy` returns a `ConstraintResult`, which decides the outcome and writes the expectation and the result texts of
the failure message. The `IEvaluationContext` shares data between the constraints of an evaluation, e.g. a
[materialized collection](./04-collections-and-nested-expectations.md#collection-subjects).

<details>
<summary>The evaluation context</summary>

The `IEvaluationContext` allows storing and receiving data between expectations. This mechanism is used for example to
avoid enumerating an `IEnumerable` multiple times across multiple constraints. A value stored while an item of a
collection or a member after `Whose` or `Which` is evaluated is only received by the expectations on that item or
member, so a value cached for one item is not handed to the next one. Its `Cancellation` describes the
cancellation of the evaluation: the `Token` (the same token that an asynchronous constraint receives), the effective
`Timeout`, and the `Reason` of a cancellation: `None`, the `Timeout`, or the `Caller`.

</details>

The factory passed to `AddConstraint` usually captures the arguments of the expectation, which allocates a closure and a
delegate for every expectation. To avoid that, pass the arguments as state to a `static` lambda:

```csharp
public static AndOrResult<Track, IThat<Track?>> IsShorterThan(this IThat<Track?> subject, TimeSpan maximum)
    => new(subject.Get().ExpectationBuilder.AddConstraint(maximum,
            static (max, it, grammars) => new IsShorterThanConstraint(it, grammars, max)),
        subject);
```

For several arguments, pass a tuple as the state.

## Results

All constraints should also provide the expectations and results for the negated case, so that they are compatible
with `DoesNotComplyWith`. The recommended practice is to use the same class for the constraint and its
`ConstraintResult`, derived from one of the following helper classes:

| Helper class                           | `null` subject                                   | Use it when the expectation                         |
|----------------------------------------|--------------------------------------------------|-----------------------------------------------------|
| `ConstraintResult.WithNotNullValue<T>` | fails the expectation and its negation           | inspects the subject                                |
| `ConstraintResult.WithEqualToValue<T>` | is an ordinary value, compared with the expected | compares for equality or identity                   |
| `ConstraintResult.WithValue<T>`        | is not handled                                   | handles `null` itself or has a non-nullable subject |

All three take the name of the subject (`it`) and the `grammars` in their constructor and expose the name as the
inherited `It` property. You set the `Actual` property in the `IsMetBy` method and override `AppendNormalExpectation`
and `AppendNegatedExpectation` as well as `AppendNormalResult` and `AppendNegatedResult`. `WithEqualToValue<T>`
additionally takes a flag indicating if the expected value is `null`.

The constraint for the `IsRadioFriendly` expectation from [Extending aweXpect](./index.md) then reads:

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

Pass `it` to the base class and use the inherited `It` property: capturing the parameter as well stores it twice,
which the compiler warns about (CS9107). The helper classes also let `.And` combine the result texts like the built-in
expectations, e.g. "it was 2 and was not even" instead of "it was 2 and it was not even".

<details>
<summary>Deriving from ConstraintResult directly</summary>

A constraint can also derive from `ConstraintResult` directly. It then writes the texts in `AppendExpectation` and
`AppendResult`, returns the value in `TryGetStoredValue` and implements `Negate()` itself. This example does not
support the negated case:

```csharp
private sealed class IsRadioFriendlyConstraint(string it, ExpectationGrammars grammars)
    : ConstraintResult(grammars),
        IValueConstraint<Track?>
{
    private Track? _actual;
    public ConstraintResult IsMetBy(Track? actual)
    {
        _actual = actual;
        Outcome = actual?.Duration <= TimeSpan.FromMinutes(3) ? Outcome.Success : Outcome.Failure;
        return this;
    }

    public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is radio friendly");

    public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
    {
        stringBuilder.Append(it).Append(" was ");
        Formatter.Format(stringBuilder, _actual?.Duration);
        stringBuilder.Append(" long");
    }

    public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
    {
        if (_actual is TValue typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return typeof(TValue).IsAssignableFrom(typeof(Track));
    }

    public override ConstraintResult Negate()
        => throw Tracing.WriteException(
            new NotSupportedException("Negation of IsRadioFriendly is not supported."));
}
```

`Negate()` is called whenever the expectation is negated, e.g. by `DoesNotComplyWith(x => x.IsRadioFriendly())`, and
the caller relies on the returned result being negated. Returning `this` unchanged would silently check the
non-negated expectation instead, so a constraint that cannot be negated throws, like the built-in `ExecutesIn()`.

To combine the result texts with `.And` like the helper classes, override `LeadingSubject` and `TrailingSubject` with
`GetSubjectOfResult(it)`.

</details>

## `null` subjects

Which of the three helper classes to pick is decided by how your expectation treats a `null` subject, and that follows
the rule that all built-in expectations follow (see [`null` subjects](../03-how-it-works/04-null-subjects.md)):

> A `null` subject fails an expectation **and its negation**, unless the expectation is *about* `null`: equality and
> identity comparisons, where `null` is a legitimate value on either side, or an explicit `null` or tri-state check.

- Your expectation **inspects the subject**, e.g. its length, its type or its items: use
  `ConstraintResult.WithNotNullValue<T>`. `IsNotEmpty()` fails for a `null` subject just like `IsEmpty()` does. This
  includes expectations that take a value but do not compare for equality, e.g. `HasValue(2)`, `IsGreaterThan` or
  `IsNotBetween`.
- Your expectation **compares the subject for equality or identity** against a value the caller supplied: use
  `ConstraintResult.WithEqualToValue<T>` and pass whether the expected value is `null`. `IsEqualTo(null)` succeeds for
  a `null` subject, `IsNotEqualTo(null)` fails and `IsNotEqualTo("foo")` succeeds.
- Your subject **cannot be `null`**, or your expectation **decides every `null` case itself**, e.g. `IsNull()` or
  `IsOneOf(...)`, whose expected values may or may not include `null`: use `ConstraintResult.WithValue<T>`.

`WithValue<T>` applies no `null` policy of its own: a failure that you set for a `null` subject in `IsMetBy` is
inverted into a success when the expectation is negated. Set [`Outcome.FailureBothWays`](#failing-both-ways) instead.

### Nullability warnings

After an expectation that fails for a `null` subject, the subject can't be `null` any more. Mark such an expectation
with the `[GuaranteesNotNull]` attribute from `aweXpect.Core`, so that the
[nullability suppressor](../07-analyzers.md#nullability-suppressor) of the `aweXpect` package suppresses the
nullability warnings for the subject after your expectation, as it does after `IsNotNull()`:

```csharp no-compile
[GuaranteesNotNull]
public static AndOrResult<Track, IThat<Track?>> IsRadioFriendly(this IThat<Track?> subject)
    => // ...
```

```csharp
Track[] tracks = [new("Love Me Do", new TimeSpan(0, 2, 22)), new("Hey Jude", new TimeSpan(0, 7, 11))];
Track? track = tracks.FirstOrDefault(t => t.Title == "Love Me Do");

await Expect.That(track).IsRadioFriendly();
string title = track.Title;   // no CS8602
```

- Only mark an expectation that fails for a `null` subject regardless of its other arguments, e.g. one that uses
  `ConstraintResult.WithNotNullValue<T>`. A wrongly marked expectation hides real nullability warnings.
- The expectation has to be an extension method on `IThat<TSubject>`.

<details>
<summary>Expectations combined with And</summary>

An expectation of your package before `.And`, as in `Expect.That(track).IsRadioFriendly().And.IsNotNull()`, keeps
the subject for the suppressor only when it returns an aweXpect result for the same `IThat<TSubject>`, such as
`AndOrResult<Track, IThat<Track?>>`. An expectation that returns an `IThat<…>` itself could continue with a different
subject, so the suppressor ignores the expectations after it.

</details>

## Failing both ways

Some expectations cannot be answered at all, e.g. because the subject is `null`, the values are not comparable or code
of the caller threw. Negating such an expectation does not make it true, so it fails the expectation and its negation
alike. Set `Outcome.FailureBothWays` in `IsMetBy` to say so:

```csharp no-compile
catch (Exception exception)
{
    _exception = exception;
    Outcome = Outcome.FailureBothWays;
}
```

- `Outcome.FailureBothWays` fails the expectation like `Outcome.Failure`, but a negation keeps it.
  `WithNotNullValue<T>` reports it for a `null` subject.
- When you derive from `ConstraintResult` directly, swap only `Success` and `Failure` in your `Negate()` and keep
  `FailureBothWays`.
- Code that reads an outcome treats `Outcome.Failure` and `Outcome.FailureBothWays` alike as a failed expectation. An
  expectation that evaluates nested expectations, e.g. on the items of a collection, checks for
  `Outcome.FailureBothWays` to find out whether an item was answered.

## Negated expectations

A constraint that supports the negated case also allows you to write an explicit negated expectation with the
`.Invert()` method:

```csharp
/// <summary>
///     Verifies that the <paramref name="subject"/> is not radio friendly.
/// </summary>
public static AndOrResult<Track, IThat<Track?>> IsNotRadioFriendly(
    this IThat<Track?> subject)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsRadioFriendlyConstraint(it, grammars).Invert()),
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
InvalidOperationException"; without it, the subject is named ("it did throw …"). For an asynchronous predicate, use
`UserCode.InvokeAsync(() => predicate(actual), "the predicate", cancellationToken)`, which throws an
`OperationCanceledException` unchanged while the `cancellationToken` is canceled.

An exception that your constraint throws itself, e.g. to reject an invalid argument, is still thrown as it is.

## Argument validation

Check the arguments in the extension method itself, so that a wrong argument fails right where the caller passes it.
Use the wording of the built-in expectations: a missing argument reads "The 'title' cannot be null.", and any other
invalid argument is described in a complete sentence, as the [message conventions](./06-message-conventions.md#exceptions)
explain. Throw the exception via `Tracing.WriteException`, which also hands it to the trace writer that users enable
with `Customize.aweXpect.EnableTracing(…)`. Core has no public helper for this, so add a small one to your extension:

```csharp
internal static void ThrowIfNull(object? value, string paramName)
{
    if (value is null)
    {
        throw Tracing.WriteException(new ArgumentNullException(paramName, $"The '{paramName}' cannot be null."));
    }
}
```

```csharp no-compile
public static AndOrResult<Track, IThat<Track?>> HasTitle(this IThat<Track?> subject, string title)
{
    ThrowIfNull(title, nameof(title));
    // ...
}
```

## Continuing with the value

The first type argument of the result, e.g. `Track` in `AndOrResult<Track, IThat<Track?>>`, is the type of the value
that the expectation passes on. Awaiting the expectation returns this value, and an `AndOrWhoseResult<TType, TThat>`
also continues with `Whose` on a member of it:

```csharp
public static AndOrWhoseResult<Track, IThat<Track?>> IsRadioFriendly(this IThat<Track?> subject)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new IsRadioFriendlyConstraint(it, grammars)),
        subject);
```

```csharp
Track track = new("Love Me Do", new TimeSpan(0, 2, 22));

Track verifiedTrack = await Expect.That(track).IsRadioFriendly();
await Expect.That(track).IsRadioFriendly().Whose(t => t.Title, title => title.StartsWith("Love"));
```

Both ask the `TryGetStoredValue<TValue>` method of the `ConstraintResult` for the value, so a constraint that narrows
or converts the subject returns the converted value there. The helper classes return their `Actual` value.

<details>
<summary>How the stored value is used</summary>

`Whose` and `AndWhose` access the member on that value when the expectation is met; when it is not met, a member of a
converted value is not evaluated, only its expectations are shown. A stored `null` value of a matching type still
returns `true`, so that awaiting the expectation returns `null`. When `TryGetStoredValue` returns `false` for the type,
awaiting the successful expectation returns the `default` of the type, e.g. after an `.Or` whose earlier alternative
was met. `TryGetValue<TValue>` builds on it and only returns `true` for a value that is not `null`.

</details>

A result of your own that derives from `AndOrResult<TType, TThat>` reaches the `ExpectationBuilder` through the
protected property of the same name, e.g. to continue with `ExpectationBuilder.And()` or `ExpectationBuilder.Or()`.
For the options a result offers, see [options and match types](./03-options-and-match-types.md).
