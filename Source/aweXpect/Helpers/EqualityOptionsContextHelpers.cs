using System.Collections.Generic;
using System.Threading;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class EqualityOptionsContextHelpers
{
	/// <summary>
	///     Returns the <paramref name="options" /> to use for all comparisons of the evaluation in the
	///     <paramref name="context" />, see
	///     <see cref="ObjectEqualityOptions{TSubject}.ForEvaluation(IEvaluationContext, CancellationToken)" /> and
	///     <see cref="StringEqualityOptions.ForEvaluation(IEvaluationContext, CancellationToken)" />.
	/// </summary>
	/// <remarks>
	///     Options that wrap other options, to let the comparer of a set subject decide, are kept while the comparison is
	///     the default one, so that the comparer can still decide, and are replaced by the wrapped options for the
	///     evaluation otherwise, as the comparer never decides then.
	/// </remarks>
	public static IOptionsEquality<T> ForEvaluation<T>(this IOptionsEquality<T> options, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		IOptionsEquality<T>? evaluationOptions = ForEvaluationOrNull(options, context, cancellationToken);
		if (evaluationOptions is not null)
		{
			return evaluationOptions;
		}

		if (options is IOptionsProvider<IOptionsEquality<T>> { Options: { } wrappedOptions, } &&
		    !DefaultEquality.IsUsedBy(wrappedOptions))
		{
			IOptionsEquality<T>? wrappedEvaluationOptions =
				ForEvaluationOrNull(wrappedOptions, context, cancellationToken);
			return wrappedEvaluationOptions is null || ReferenceEquals(wrappedEvaluationOptions, wrappedOptions)
				? options
				: wrappedEvaluationOptions;
		}

		return options;
	}

	private static IOptionsEquality<T>? ForEvaluationOrNull<T>(IOptionsEquality<T> options,
		IEvaluationContext context, CancellationToken cancellationToken)
		=> options switch
		{
			ObjectEqualityOptions<T> objectEqualityOptions
				=> objectEqualityOptions.ForEvaluation(context, cancellationToken),
			StringEqualityOptions stringEqualityOptions
				=> (IOptionsEquality<T>)stringEqualityOptions.ForEvaluation(context, cancellationToken),
			_ => null,
		};

	/// <summary>
	///     Rejects the first of the <paramref name="expected" /> items that the match type of string
	///     <paramref name="options" /> cannot use, e.g. an invalid regex pattern.
	/// </summary>
	/// <remarks>
	///     The options only validate an item that an item of the subject is compared with, so an expectation calls
	///     this at the start of an evaluation, which rejects the item whichever items the subject has.
	/// </remarks>
	public static void ValidateExpectedItems<T, TItem>(this IOptionsEquality<T> options, IEnumerable<TItem> expected)
		where TItem : T
	{
		StringEqualityOptions? stringOptions = options switch
		{
			StringEqualityOptions directOptions => directOptions,
			IOptionsProvider<IOptionsEquality<T>> { Options: StringEqualityOptions wrappedOptions, } => wrappedOptions,
			_ => null,
		};
		if (stringOptions is null || stringOptions.ComparesByOrdinalEquality)
		{
			return;
		}

		foreach (TItem item in expected)
		{
			stringOptions.ValidateExpected(item as string);
		}
	}
}
