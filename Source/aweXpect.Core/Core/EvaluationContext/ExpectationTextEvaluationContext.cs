using System.Diagnostics.CodeAnalysis;

namespace aweXpect.Core.EvaluationContext;

/// <summary>
///     An <see cref="IEvaluationContext" /> in which the nodes only build the expectation result, without evaluating
///     their constraints or accessing members.
/// </summary>
/// <remarks>
///     Used when the value for the expectations is not available (e.g. a <see langword="null" /> subject or a throwing
///     member), so that constraints which cannot handle a <see langword="default" /> value are never invoked, or when
///     it must not be evaluated anymore (e.g. the branches of an <c>Or</c> after one of them was met).
/// </remarks>
internal sealed class ExpectationTextEvaluationContext : IEvaluationContext
{
	private readonly IEvaluationContext? _inner;

	private ExpectationTextEvaluationContext(IEvaluationContext? inner)
	{
		_inner = inner;
	}

	/// <summary>
	///     The evaluation whose expectation text is built, or <see langword="null" /> when there is none.
	/// </summary>
	public EvaluationContext? Evaluation => _inner as EvaluationContext;

	/// <inheritdoc />
	public void Store<T>(string key, T value)
		=> _inner?.Store(key, value);

	/// <inheritdoc />
	public bool TryReceive<T>(string key, [NotNullWhen(true)] out T? value)
	{
		if (_inner is not null)
		{
			return _inner.TryReceive(key, out value);
		}

		value = default;
		return false;
	}

	/// <inheritdoc />
	public EvaluationCancellation Cancellation => _inner?.Cancellation ?? EvaluationCancellation.None;

	/// <summary>
	///     Returns an <see cref="ExpectationTextEvaluationContext" /> wrapping the <paramref name="context" />.
	/// </summary>
	public static IEvaluationContext For(IEvaluationContext? context)
		=> context as ExpectationTextEvaluationContext ?? new ExpectationTextEvaluationContext(context);
}
