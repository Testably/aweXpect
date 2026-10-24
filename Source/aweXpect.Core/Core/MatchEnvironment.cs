using System.Threading;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     The evaluation for which a match type creates its matcher, see
///     <see cref="StringMatchType.ForEvaluation(MatchEnvironment, StringMatchSettings)" /> and
///     <see cref="ObjectMatchType.ForEvaluation(MatchEnvironment)" />.
/// </summary>
public readonly struct MatchEnvironment
{
	/// <summary>
	///     Describes the evaluation in the <paramref name="context" /> that the <paramref name="cancellationToken" />
	///     cancels.
	/// </summary>
	/// <exception cref="System.ArgumentNullException">The <paramref name="context" /> is <see langword="null" />.</exception>
	public MatchEnvironment(IEvaluationContext context, CancellationToken cancellationToken)
	{
		context.ThrowIfNull();
		Context = context;
		CancellationToken = cancellationToken;
	}

	/// <summary>
	///     The environment of a comparison outside an evaluation.
	/// </summary>
	internal static MatchEnvironment WithoutEvaluation { get; }
		= new(ExpectationTextEvaluationContext.For(null), CancellationToken.None);

	/// <summary>
	///     The <see cref="Context" />, or <see langword="null" /> outside an evaluation.
	/// </summary>
	internal IEvaluationContext? Evaluation
		=> ReferenceEquals(Context, WithoutEvaluation.Context) ? null : Context;

	/// <summary>
	///     The context of the evaluation.
	/// </summary>
	/// <remarks>
	///     A comparison outside an evaluation, e.g. in a filter that is not part of an expectation, receives a context
	///     that stores nothing and is never completed.
	/// </remarks>
	public IEvaluationContext Context { get; }

	/// <summary>
	///     The cancellation of the evaluation.
	/// </summary>
	public CancellationToken CancellationToken { get; }
}
