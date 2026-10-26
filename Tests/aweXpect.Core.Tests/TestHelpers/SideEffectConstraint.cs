using System.Collections.Generic;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     What the <see cref="SideEffectConstraint{T}" />s of a test did.
/// </summary>
internal sealed class SideEffectLog
{
	public List<string> Effects { get; } = [];
	public int Evaluations { get; set; }
	public List<CancellationToken> Tokens { get; } = [];

	/// <summary>
	///     An exception that the callbacks throw, or <see langword="null" />.
	/// </summary>
	public Exception? ThrowOnCompleted { get; set; }
}

/// <summary>
///     A constraint with a side effect that depends on how the whole expectation ended, like the snapshot constraint of
///     aweXpect.Snapshots: it registers its callback in <c>IsMetBy</c>, tracks its negation by overriding
///     <see cref="Negate" /> and decides about the side effect in the callback.
/// </summary>
internal sealed class SideEffectConstraint<T>(
	string it,
	ExpectationGrammars grammars,
	SideEffectLog log,
	string name,
	Func<T, Outcome> outcome,
	Func<CancellationToken, Task>? whileEvaluating)
	: ConstraintResult.WithValue<T>(it, grammars),
		IAsyncContextConstraint<T>
{
	private bool _isNegated;

	public async ValueTask<ConstraintResult> IsMetBy(T actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Actual = actual;
		Outcome = outcome(actual);
		log.Evaluations++;
		context.OnCompleted(OnCompletedAsync);
		if (whileEvaluating is not null)
		{
			await whileEvaluating(cancellationToken);
		}

		return this;
	}

	protected internal override void NegateInPlace()
	{
		_isNegated = !_isNegated;
		base.NegateInPlace();
	}

	private ValueTask OnCompletedAsync(CompletedEvaluation evaluation, CancellationToken cancellationToken)
	{
		log.Tokens.Add(cancellationToken);
		if (log.ThrowOnCompleted is not null)
		{
			throw log.ThrowOnCompleted;
		}

		log.Effects.Add(
			$"{name} {Decide(evaluation.Outcome)} (whole: {evaluation.Outcome}, own: {Outcome}, negated: {_isNegated})");
		return default;
	}

	private string Decide(Outcome whole)
	{
		if (whole == Outcome.Undecided || _isNegated)
		{
			return "does nothing";
		}

		if (whole == Outcome.Failure && Outcome == Outcome.Failure)
		{
			return "writes the pending snapshot";
		}

		return whole == Outcome.Success && Outcome == Outcome.Success
			? "deletes a leftover pending snapshot"
			: "does nothing";
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append("has side effect ").Append(name);

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(It).Append(" did not");

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append("has no side effect ").Append(name);

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(It).Append(" did");
}

internal static class SideEffectConstraintExtensions
{
	public static AndOrWhoseResult<T, IThat<T>> HasSideEffect<T>(this IThat<T> subject, SideEffectLog log,
		string name, Outcome outcome = Outcome.Success, Func<CancellationToken, Task>? whileEvaluating = null)
		=> subject.HasSideEffect(log, name, _ => outcome, whileEvaluating);

	public static AndOrWhoseResult<T, IThat<T>> HasSideEffect<T>(this IThat<T> subject, SideEffectLog log,
		string name, Func<T, Outcome> outcome, Func<CancellationToken, Task>? whileEvaluating = null)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SideEffectConstraint<T>(it, grammars, log, name, outcome, whileEvaluating)),
			subject);
}
