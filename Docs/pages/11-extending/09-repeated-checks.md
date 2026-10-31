---
description: Wait for a condition with repeated checks, and call asynchronous code from code that has to answer synchronously.
---

# Repeated checks and synchronous boundaries

The basics of asynchronous constraints, the `CancellationToken` and an undecided outcome, are described in
[constraints and results](./02-constraints-and-results.md#asynchronous-constraints). This page shows how an
expectation waits for a condition, and how code that has to answer synchronously calls asynchronous code.

<details>
<summary>Namespaces used on this page</summary>

```csharp
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;
using aweXpect.Results;
```

</details>

## Repeated checks

An expectation that returns a `RepeatedCheckResult<TType, TThat>` lets the caller wait for a condition with `.Within(timeout)` and `.CheckEvery(interval)`, like `Satisfies` does. Pass the same holder of `RepeatedCheckOptions` to the result and to the constraint, and make the check with `CheckRepeatedly` of its value, which honors the options. For a player that starts playing asynchronously:

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
    OptionsHolder<RepeatedCheckOptions> options = new(RepeatedCheckOptions.Default);
    return new RepeatedCheckResult<Player, IThat<Player?>>(subject.Get().ExpectationBuilder
            .AddConstraint((it, grammars) => new IsPlayingConstraint(it, grammars, options)),
        subject,
        options);
}

private sealed class IsPlayingConstraint(
    string it,
    ExpectationGrammars grammars,
    OptionsHolder<RepeatedCheckOptions> options)
    : ConstraintResult.WithNotNullValue<Player>(it, grammars),
        IAsyncContextConstraint<Player?>
{
    public async ValueTask<ConstraintResult> IsMetBy(Player? actual, IEvaluationContext context,
        CancellationToken cancellationToken)
    {
        Actual = actual;
        if (actual is not null)
        {
            Outcome outcome = await options.Value.CheckRepeatedly(_ =>
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

await That(player).IsPlaying()
    .Within(TimeSpan.FromSeconds(5)).CheckEvery(TimeSpan.FromMilliseconds(100));
```

- The check returns whether the expectation is met, so for a negated variant created with `.Invert()` it returns
  `true` when the player is *not* playing. The helper class inverts the stored `Outcome` itself.
- When `CheckRepeatedly` returns `Undecided`, because the evaluation was canceled, set the `Outcome` of the constraint
  to `Undecided` as well, as the sample does.
- Appending the options writes " within …" to the expectation text when `Within` was specified.

<details>
<summary>Timing, contexts, exceptions and cancellation</summary>

- `CheckRepeatedly` makes the first check immediately. When `IsRepeated` is `true`, because `Within` set a positive
  or an infinite timeout, it repeats the check in the interval until it succeeds, and makes the last check at the
  timeout.
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

## Synchronous boundaries

Some code has to answer synchronously although it calls asynchronous code, e.g. an `IEnumerable<T>` that filters its
items with an asynchronous predicate. Wait there with `SynchronousBoundary.Wait` from `aweXpect.Core.Extending`
instead of `.GetAwaiter().GetResult()`:

```csharp no-compile
public IEnumerator<Track> GetEnumerator()
    => _tracks.Where(track => SynchronousBoundary.Wait(() => IsInCatalogAsync(track))).GetEnumerator();
```

The expectations of aweXpect run on the synchronization context of the caller, e.g. on the UI thread of a WPF or
WinForms test. Blocking that thread would deadlock as soon as the asynchronous code continues on it, so `Wait` starts
the operation without the synchronization context and the task scheduler of the caller.
