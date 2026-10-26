using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.EvaluationContext;

/// <summary>
///     The evaluation context.
/// </summary>
/// <remarks>
///     Use it by implementing <see cref="IContextConstraint{TValue}" />.
/// </remarks>
public interface IEvaluationContext
{
	/// <summary>
	///     The cancellation of the current evaluation.
	/// </summary>
	/// <remarks>
	///     Its <see cref="EvaluationCancellation.Token" /> is the cancellation token that the constraints receive.
	///     A context outside of an evaluation returns <see cref="EvaluationCancellation.None" />.
	/// </remarks>
	EvaluationCancellation Cancellation { get; }

	/// <summary>
	///     Stores a <paramref name="value" /> under the <paramref name="key" /> in the evaluation context.
	/// </summary>
	/// <remarks>
	///     A value stored while an item of a collection or a member (e.g. after <c>Whose</c> or <c>Which</c>) is
	///     evaluated is only received by the expectations on that item or member.
	/// </remarks>
	void Store<T>(string key, T value);

	/// <summary>
	///     Tries to retrieve a previously stored <paramref name="value" /> under the <paramref name="key" /> from the
	///     evaluation context.
	/// </summary>
	bool TryReceive<T>(string key, [NotNullWhen(true)] out T? value);

	/// <summary>
	///     Runs the <paramref name="callback" /> once after the whole expectation is decided and every negation is applied,
	///     before a failure is thrown.
	/// </summary>
	/// <remarks>
	///     Use it for side effects that depend on how the expectation ended, as a constraint cannot know during its
	///     evaluation whether it is negated or whether another branch of an <c>Or</c> decides the expectation. The
	///     <see cref="CompletedEvaluation" /> holds the outcome of the whole expectation, also when it is
	///     <see cref="Constraints.Outcome.Undecided" />, while the constraint knows its own outcome and, by overriding
	///     <see cref="ConstraintResult.NegateInPlace()" />, its own negation.
	///     <para />
	///     Each delegate runs once, however often it is registered (e.g. for each item of a collection or each check of
	///     <c>Within</c>), so register a method of the constraint, which is equal on every registration. The callbacks run
	///     in the order of their first registration, also those registered while an item or a member is evaluated, and an
	///     exception they throw is thrown by the expectation. They receive the cancellation token of the caller, which is
	///     not canceled by the timeout of the evaluation.
	/// </remarks>
	void OnCompleted(Func<CompletedEvaluation, CancellationToken, ValueTask> callback);
}
