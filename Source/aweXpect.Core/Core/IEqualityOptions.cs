using System.Threading;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core;

/// <summary>
///     Options that compare values of <typeparamref name="TElement" />, e.g. the
///     <see cref="aweXpect.Options.StringEqualityOptions" /> or an <see cref="ObjectMatchType{TElement}" />.
/// </summary>
/// <remarks>
///     The options are immutable values, so a constraint reads them from the holder of its expectation when it evaluates
///     and compares with the evaluator they return for that evaluation.
/// </remarks>
public interface IEqualityOptions<in TElement>
{
	/// <summary>
	///     Returns the comparison for all comparisons of the evaluation in the <paramref name="context" />, which may keep
	///     state across them.
	/// </summary>
	/// <remarks>
	///     A comparison that evaluates expectations of its own, i.e. the <c>It.Is…</c> in the expected object of an
	///     equivalency comparison, evaluates them as part of that evaluation: the <paramref name="cancellationToken" />
	///     cancels them, and its timeout and time provider apply to them.
	/// </remarks>
	IEqualityEvaluator<TElement> ForEvaluation(IEvaluationContext context, CancellationToken cancellationToken);

	/// <summary>
	///     Rejects the <paramref name="expected" /> value, which the caller received as <paramref name="parameterName" />,
	///     when the options cannot compare with it, e.g. an invalid regex pattern.
	/// </summary>
	/// <remarks>
	///     An expectation calls this for every expected value before it looks at the subject, so that an unusable value is
	///     rejected whichever subject it is verified for, also when no value is compared with it.
	/// </remarks>
	/// <exception cref="System.ArgumentException">
	///     The options cannot compare with the <paramref name="expected" /> value.
	/// </exception>
	void ValidateExpected(TElement expected, string parameterName);
}
