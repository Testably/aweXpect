using System.Threading;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core;

/// <summary>
///     Options that compare values of <typeparamref name="TSubject" />, e.g. the
///     <see cref="aweXpect.Options.StringEqualityOptions" /> or an <see cref="ObjectMatchType{TSubject}" />.
/// </summary>
/// <remarks>
///     The options are immutable values, so a constraint reads them from the holder of its expectation when it evaluates
///     and compares with the evaluator they return for that evaluation.
/// </remarks>
public interface IEqualityOptions<in TSubject>
{
	/// <summary>
	///     Returns the comparison for all comparisons of the evaluation in the <paramref name="context" />, which may keep
	///     state across them.
	/// </summary>
	/// <remarks>
	///     A comparison that evaluates expectations of its own, i.e. the <c>It.Is…</c> in the expected object of an
	///     equivalency comparison, evaluates them as part of that evaluation: the <paramref name="cancellationToken" />
	///     cancels them, and its timeout and time system apply to them.
	/// </remarks>
	IEqualityEvaluator<TSubject> ForEvaluation(IEvaluationContext context, CancellationToken cancellationToken);
}
