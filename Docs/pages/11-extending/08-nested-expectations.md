---
description: Let the caller continue with expectations on a member or a new subject, and evaluate expectations inside expected values.
---

# Nested expectations

An expectation can let the caller continue with expectations on a member of the subject or on a new subject, and an
expected value can contain expectations of its own.

<details>
<summary>Namespaces used on this page</summary>

```csharp
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Equivalency;
using aweXpect.Results;
```

</details>

## Expectations on a member

An expectation that lets the caller continue with expectations on a part of the subject, like
`Throws().WithInner<T>(e => …)`, selects the part with `ForMember` and adds the caller's expectations to it:

```csharp
public static AndOrResult<Track, IThat<Track?>> HasTitle(
    this IThat<Track?> subject,
    Action<IThatSubject<string?>> expectations)
    => new(subject.Get().ExpectationBuilder
            .ForMember(
                MemberAccessor<Track?, string?>.FromFuncWithName(track => track?.Title, "title "),
                (member, stringBuilder) => stringBuilder.Append("has ").Append(member).Append("that "))
            .AddExpectations(expectations, grammars => grammars | ExpectationGrammars.Nested),
        subject);
```

`await That(track).HasTitle(title => title.StartsWith("Let"))` then reads "has title that starts with …".

- The second argument of `ForMember` writes the text in front of the nested expectations.
- The name of the member replaces `it` in their result texts ("title was …"), unless you pass `replaceIt: false`.
- The function passed to `AddExpectations` sets the grammars of the nested expectations.
- `Validate(…)` before `AddExpectations` adds a constraint on the subject itself, e.g. based on
  `ConstraintResult.WithNotNullValue<T>` to rule out a `null` subject, as `WithInner` does.
- `ForAsyncMember` does the same for a member that has to be awaited, returned as a `Task<T>` or a `ValueTask<T>`.

<details>
<summary>Member paths, negation as an alternative and allocations</summary>

- A member named by a path within the subject, like the JSON path `$.artist.origin`, uses
  `MemberAccessor<TSource, TMember>.FromFuncWithPath(…, path, "$")`, so that the contexts of a nested path replace the
  label of the enclosing one instead of repeating it, and follow the label of another member without the root, e.g.
  `elements[1].origin`.
- When the constraint from `Validate(…)` finds the member, e.g. a property that may be missing, `NegatedAsAlternative(…)`
  writes the negation as an alternative, e.g. `does not have property "a" or $.a is not equal to 1`, and the result
  of the member then explains a failed negation.
- The other overloads of `AddExpectations` hand the nested expectations the `ExpectationBuilder` instead of an
  `IThatSubject<T>`, optionally with a state, so that a `static` lambda adds them without allocating a closure.
- An exception that the member selector throws fails the nested expectations with "… did throw …".

</details>

## Continuing with a new subject

A result that continues with a new subject after the expectation, like `HasSingle().Which`, uses `ForWhich` instead.
It selects the new subject from the value of the expectation, and the expectations that the caller adds to the
returned `ThatSubject<T>` apply to it:

```csharp
public class LongestTrackResult(ExpectationBuilder expectationBuilder, IThat<Track[]?> returnValue)
    : AndOrResult<Track[], IThat<Track[]?>>(expectationBuilder, returnValue)
{
    public IThatSubject<Track?> Which
        => new ThatSubject<Track?>(ExpectationBuilder.ForWhich<Track[], Track?>(
            tracks => tracks.OrderByDescending(track => track.Duration).FirstOrDefault(),
            " whose longest track ", "longest track"));
}
```

The second argument of `ForWhich` writes the text in front of the expectations on the new subject, and the third one
replaces `it` in their result texts. Unlike `ForMember`, the expectations are added later by the caller, after `Which`.
`ForAsyncWhich` takes the same arguments for a new subject that has to be awaited, returned as a `Task<T>` or a
`ValueTask<T>`.

Hand out a new subject as `IThatSubject<T>`, both in an `Action<IThatSubject<T>>` parameter and in a property like
`Which`, so that the caller can narrow it with `Is<TType>()` and the other type checks. Unlike `That<T>(T)`,
which returns an `IThatSubject<T>`, `That(bool)` returns a `ThatBoolSubject` that is only an `IThat<bool>`,
because a type check cannot narrow a `bool`, so write expectations on booleans for `IThat<bool>`.

## Members that need the evaluation

A member that is read asynchronously, e.g. from a stream or a service, should stop when the evaluation is canceled or
times out. `MemberAccessor<TSource, TMember>.FromFuncWithName` and `ForAsyncWhich` also take a function that receives
the `IEvaluationContext` and the `CancellationToken` of the evaluation besides the source value. Pass the token on to
the asynchronous calls, and use the context to share a value that is expensive to load with the other expectations of
the same evaluation:

```csharp
public interface ILyricsSource
{
    Task<string?> LoadAsync(Track track, CancellationToken cancellationToken);
}

public static AndOrResult<Track, IThat<Track?>> HasLyrics(
    this IThat<Track?> subject,
    ILyricsSource source,
    Action<IThatSubject<string?>> expectations)
    => new(subject.Get().ExpectationBuilder
            .ForAsyncMember(
                MemberAccessor<Track, Task<string?>>.FromFuncWithName(
                    (track, context, cancellationToken) => LoadLyrics(source, track, context, cancellationToken),
                    "lyrics "),
                (member, stringBuilder) => stringBuilder.Append("has ").Append(member).Append("that "))
            .AddExpectations(expectations, grammars => grammars | ExpectationGrammars.Nested),
        subject);

private static async Task<string?> LoadLyrics(ILyricsSource source, Track track, IEvaluationContext context,
    CancellationToken cancellationToken)
{
    string key = "lyrics:" + track.Title;
    if (context.TryReceive(key, out string? lyrics))
    {
        return lyrics;
    }

    lyrics = await source.LoadAsync(track, cancellationToken);
    context.Store(key, lyrics);
    return lyrics;
}
```

- The accessor is called with the context of the expectation that it continues, so a value stored there is received
  by the other expectations on the same subject, e.g. a second `HasLyrics` after `And`. A value stored while an item
  of a collection or a member is evaluated is only received by the expectations on that item or member.
- `ForMember` passes the context and the token as well, if its accessor takes them.
- A canceled evaluation does not wait for a member that ignores the token, but the member keeps running in the
  background.

## Expectations in expected values

An expected value can contain expectations, like `new { Title = It.Is<string>().That.StartsWith("Let") }`. Pass the
`IEvaluationContext` and the `CancellationToken` of your constraint to `EquivalencyComparison.Compare`, or to
`EquivalencyExpectationBuilder.IsMetBy` when you evaluate such an expectation yourself, so that the timeout and the
cancellation of the evaluation also end these expectations:

```csharp
public static AndOrResult<Track, IThat<Track?>> MatchesTrack(this IThat<Track?> subject, object expected)
    => new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
            => new MatchesTrackConstraint(it, grammars, expected)),
        subject);

private sealed class MatchesTrackConstraint(string it, ExpectationGrammars grammars, object expected)
    : ConstraintResult.WithNotNullValue<Track>(it, grammars),
        IAsyncContextConstraint<Track?>
{
    private readonly StringBuilder _differences = new();

    public async ValueTask<ConstraintResult> IsMetBy(Track? actual, IEvaluationContext context,
        CancellationToken cancellationToken)
    {
        Actual = actual;
        if (actual is not null)
        {
            _differences.Clear();
            bool isEquivalent = await EquivalencyComparison.Compare(actual, expected,
                new EquivalencyOptions(), _differences, context, cancellationToken);
            Outcome = isEquivalent ? Outcome.Success : Outcome.Failure;
        }

        return this;
    }

    protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("matches the expected track");

    protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" did not:").Append(_differences);

    protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append("does not match the expected track");

    protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
        => stringBuilder.Append(It).Append(" did");
}
```

```csharp
Track track = new("Let It Be", new TimeSpan(0, 4, 3));

await That(track).MatchesTrack(new { Title = It.Is<string>().That.StartsWith("Let") });
```

When your constraint evaluates such an expectation itself, e.g. because it compares a format of its own, get its
`EquivalencyExpectationBuilder` with `EquivalencyExpectations.TryGetBuilder` from `aweXpect.Core.Extending` and evaluate
it with the context and the token of your constraint. The expected value is the result of the whole chain, e.g.
`It.Is<string>().That.StartsWith("Let")`, so test for the builder rather than for the type `It.IsEquivalent<T>`:

```csharp no-compile
if (EquivalencyExpectations.TryGetBuilder(expectedValue, out EquivalencyExpectationBuilder? expectation))
{
    ConstraintResult result = await expectation.IsMetBy(actualValue, context, cancellationToken);
    if (result.Outcome != Outcome.Success)
    {
        result.AppendResult(_differences);
    }
}
```

- The collections are released before `IsMetBy` returns, so describe the failure from the result right away: it only
  shows the items that the expectation read.
- An `Outcome.Undecided` result means that the evaluation was canceled. Leave the outcome of your constraint
  undecided as well, see [stopping without an exception](./02-constraints-and-results.md#asynchronous-constraints).
- After a cancellation, `EquivalencyComparison.Compare` throws an `OperationCanceledException` instead, which needs
  no handling.

<details>
<summary>The context of an expectation in an expected value</summary>

`IsMetBy` evaluates the expectation in a context of its own, which the timeout and the cancellation of your evaluation
end. The collections it materializes, the reasons it leaves pending and the values it stores do not reach your
context, so the expectation can be evaluated again for every value it is compared with.

</details>
