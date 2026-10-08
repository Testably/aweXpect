# Asynchronous expectations

The samples on this page use the following namespaces:

```csharp
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;
using aweXpect.Results;
```

## Asynchronous constraints

An `IAsyncConstraint<T>` receives the `CancellationToken` of the expectation, which is canceled when the timeout
(`WithTimeout`) elapses or the caller cancels (`WithCancellation`). Pass it on to the asynchronous work:

```csharp no-compile
public async ValueTask<ConstraintResult> IsMetBy(Track? actual, CancellationToken cancellationToken)
{
    Actual = actual;
    if (actual is not null)
    {
        Outcome = await IsInCatalogAsync(actual, cancellationToken) ? Outcome.Success : Outcome.Failure;
    }

    return this;
}
```

An `OperationCanceledException` from that token needs no handling: a timeout fails the expectation with "did not
finish within …", and a cancellation by the caller leaves it inconclusive.

<details>
<summary>Stopping without an exception</summary>

- A constraint that stops at the cancellation without throwing leaves its `Outcome` undecided. A timeout then fails the
  expectation in the same way. After a cancellation, the helper classes write the result text with
  `AppendUndecidedResult`, which you can override. By default it writes "it could not be verified, because the
  evaluation was already canceled".
- Leave the `Outcome` undecided only for a cancellation. An expectation whose outcome stays undecided although nothing
  was canceled, e.g. because one branch of `IsMetBy` does not set it or a collection constraint does not call
  `Complete()`, fails with "it could not be verified, because the expectation did not decide its outcome".

</details>

## Repeated checks

An expectation that returns a `RepeatedCheckResult<TType, TThat>` lets the caller wait for a condition with
`.Within(timeout)` and `.CheckEvery(interval)`, like `Satisfies` does. Pass the same `RepeatedCheckOptions` to the
result and to the constraint, and make the check with `CheckRepeatedly`, which honours the options. For a player that
starts playing asynchronously:

```csharp
public class Player
{
    public bool IsPlaying { get; private set; }

    public void Play(string title) => IsPlaying = true;
}
```

```csharp
public static RepeatedCheckResult<Player, IThat<Player?>> IsPlaying(this IThat<Player?> subject)
{
    RepeatedCheckOptions options = new();
    return new RepeatedCheckResult<Player, IThat<Player?>>(subject.Get().ExpectationBuilder
            .AddConstraint((it, grammars) => new IsPlayingConstraint(it, grammars, options)),
        subject,
        options);
}

private sealed class IsPlayingConstraint(
    string it,
    ExpectationGrammars grammars,
    RepeatedCheckOptions options)
    : ConstraintResult.WithNotNullValue<Player>(it, grammars),
        IAsyncContextConstraint<Player?>
{
    public async ValueTask<ConstraintResult> IsMetBy(Player? actual, IEvaluationContext context,
        CancellationToken cancellationToken)
    {
        Actual = actual;
        if (actual is not null)
        {
            Outcome outcome = await options.CheckRepeatedly(_ =>
            {
                bool isPlaying = actual.IsPlaying;
                Outcome = isPlaying ? Outcome.Success : Outcome.Failure;
                return new ValueTask<bool>(isPlaying != IsNegated);
            }, context);
            if (outcome == Outcome.Undecided)
            {
                Outcome = Outcome.Undecided;
            }
        }

        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is playing").Append(options);

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" was not playing");

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("is not playing").Append(options);

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" was playing");
}
```

```csharp
Player player = new();
_ = Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ => player.Play("Let It Be"));

await Expect.That(player).IsPlaying().Within(TimeSpan.FromSeconds(5)).CheckEvery(TimeSpan.FromMilliseconds(100));
```

- `CheckRepeatedly` makes the first check immediately. When `IsRepeated` is `true`, because `Within` set a positive
  or an infinite timeout, it repeats the check in the interval until it succeeds, and makes the last check at the
  timeout.
- The check returns whether the expectation is met, so for a negated variant created with `.Invert()` it returns
  `true` when the player is *not* playing. The helper class inverts the stored `Outcome` itself.
- Appending the options writes " within …" to the expectation text when `Within` was specified.

<details>
<summary>Contexts, exceptions and cancellation</summary>

- The check receives the `IEvaluationContext` to use. The first check gets the context of the evaluation, every
  further check a new one, so that a collection taken with `UseMaterializedEnumerable` is read again instead of
  replaying the items that a previous check read.
- An exception of code of the caller called through `UserCode.Invoke` counts as not met while the check is repeated.
  When the last check still throws, the expectation fails with that exception.
- `CheckRepeatedly` returns the `Outcome`: `Success` when a check succeeded, `Failure` when the last check failed, and
  `Undecided` when the evaluation was canceled before the timeout. The cancellation is only observed while waiting
  for the next check, so the first check is made even when the evaluation is already canceled.
- A cancellation at the timeout, or by an effective timeout of the evaluation (`WithTimeout` or
  `TestCancellation.FromTimeout`) that is not shorter than `Within` when the checks started with the evaluation, lets
  the last check decide. For `Undecided`, the constraint sets its `Outcome` to `Undecided`: the helper class then
  reports that the expectation could not be verified, and a shorter effective timeout is reported as "did not finish
  within …".

</details>
