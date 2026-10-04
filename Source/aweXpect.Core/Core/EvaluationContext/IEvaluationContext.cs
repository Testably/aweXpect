using System.Diagnostics.CodeAnalysis;
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
	///     The cancellation of the current evaluation.
	/// </summary>
	/// <remarks>
	///     Its <see cref="EvaluationCancellation.Token" /> is the cancellation token that the constraints receive.
	///     A context outside of an evaluation returns <see cref="EvaluationCancellation.None" />.
	/// </remarks>
	EvaluationCancellation Cancellation { get; }
}
