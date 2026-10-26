using System.Collections.Generic;
using System.Threading;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class EqualityOptionsContextHelpers
{
	/// <summary>
	///     Returns the evaluator for all comparisons of the evaluation in the <paramref name="context" />, see
	///     <see cref="IEqualityOptions{TSubject}.ForEvaluation(IEvaluationContext, CancellationToken)" />, from the
	///     current value of the <paramref name="options" />.
	/// </summary>
	/// <remarks>
	///     Options that wrap the holder of the expectation, to let the comparer of a set subject decide, are kept while the
	///     comparison is the default one, so that the comparer can still decide.
	/// </remarks>
	public static IEqualityEvaluator<T> ForEvaluation<T>(this IReadOnlyOptionsHolder<IEqualityOptions<T>> options,
		IEvaluationContext context, CancellationToken cancellationToken)
	{
		IEqualityEvaluator<T> evaluator = options.Value.ForEvaluation(context, cancellationToken);
		return options is IWrapsEqualityEvaluator<T> wrapper ? wrapper.Wrap(evaluator) : evaluator;
	}

	/// <summary>
	///     Returns the evaluator for the texts and contexts of a constraint that was not evaluated.
	/// </summary>
	/// <remarks>
	///     The context of <see langword="default" /> <see cref="MatchEnvironment" /> marks a comparison outside an
	///     evaluation. Options that wrap the holder are not asked to wrap it, as no subject is known whose comparer could
	///     decide, so that they keep the evaluator of their last evaluation.
	/// </remarks>
	public static IEqualityEvaluator<T> WithoutEvaluation<T>(this IReadOnlyOptionsHolder<IEqualityOptions<T>> options)
		=> options.Value.ForEvaluation(default(MatchEnvironment).Context, CancellationToken.None);

	/// <summary>
	///     Rejects the first of the <paramref name="expected" /> items, which the caller received as
	///     <paramref name="parameterName" />, that the match type of string
	///     <paramref name="options" /> cannot use, e.g. an invalid regex pattern.
	/// </summary>
	/// <remarks>
	///     The options only validate an item that an item of the subject is compared with, so an expectation calls
	///     this at the start of an evaluation, which rejects the item whichever items the subject has.
	/// </remarks>
	public static void ValidateExpectedItems<T, TItem>(this IReadOnlyOptionsHolder<IEqualityOptions<T>> options,
		IEnumerable<TItem> expected, string parameterName)
		where TItem : T
	{
		if (options.Value is not StringEqualityOptions stringOptions || stringOptions.ComparesByOrdinalEquality)
		{
			return;
		}

		foreach (TItem item in expected)
		{
			stringOptions.ValidateExpected(item as string, parameterName);
		}
	}
}
