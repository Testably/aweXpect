using System.Threading;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class EqualityOptionsContextHelpers
{
	/// <summary>
	///     Adds the contexts of the match type of the <paramref name="options" />, e.g. the equivalency options.
	/// </summary>
	public static void AddOptionsContexts<T>(this ResultContextCollector contexts, IOptionsEquality<T> options)
	{
		if (options is IOptionsProvider<IOptionsEquality<T>> provider)
		{
			contexts.AddOptionsContexts(provider.Options);
		}
		else if (options is ObjectEqualityOptions<T> objectEqualityOptions)
		{
			objectEqualityOptions.AppendContexts(contexts);
		}
	}

	/// <summary>
	///     Returns the <paramref name="options" /> to use for all comparisons of the evaluation in the
	///     <paramref name="context" />, see
	///     <see cref="ObjectEqualityOptions{TSubject}.ForEvaluation(IEvaluationContext, CancellationToken)" />.
	/// </summary>
	/// <remarks>
	///     Options that wrap other options, to let the comparer of a set subject decide, are kept while the comparison is
	///     the default one, so that the comparer can still decide, and are replaced by the wrapped options for the
	///     evaluation otherwise, as the comparer never decides then.
	/// </remarks>
	public static IOptionsEquality<T> ForEvaluation<T>(this IOptionsEquality<T> options, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (options is ObjectEqualityOptions<T> objectEqualityOptions)
		{
			return objectEqualityOptions.ForEvaluation(context, cancellationToken);
		}

		if (options is IOptionsProvider<IOptionsEquality<T>> { Options: ObjectEqualityOptions<T> wrappedOptions, } &&
		    !DefaultEquality.IsUsedBy(wrappedOptions))
		{
			ObjectEqualityOptions<T> evaluationOptions = wrappedOptions.ForEvaluation(context, cancellationToken);
			return ReferenceEquals(evaluationOptions, wrappedOptions) ? options : evaluationOptions;
		}

		return options;
	}
}
