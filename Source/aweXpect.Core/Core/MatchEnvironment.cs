using System.Threading;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     The evaluation for which a match type creates its matcher, see
///     <see cref="StringMatchType.ForEvaluation(MatchEnvironment, StringMatchSettings)" /> and
///     <see cref="ObjectMatchType{TSubject}.ForEvaluation(MatchEnvironment)" />.
/// </summary>
/// <remarks>
///     The <see langword="default" /> value describes a comparison outside an evaluation.
/// </remarks>
public readonly struct MatchEnvironment
{
	private readonly IEvaluationContext? _context;

	/// <summary>
	///     Describes the evaluation in the <paramref name="context" /> that the <paramref name="cancellationToken" />
	///     cancels.
	/// </summary>
	/// <exception cref="System.ArgumentNullException">The <paramref name="context" /> is <see langword="null" />.</exception>
	public MatchEnvironment(IEvaluationContext context, CancellationToken cancellationToken)
	{
		context.ThrowIfNull();
		_context = context;
		CancellationToken = cancellationToken;
	}

	/// <summary>
	///     The environment of a comparison outside an evaluation.
	/// </summary>
	internal static MatchEnvironment WithoutEvaluation => default;

	private static IEvaluationContext ContextWithoutEvaluation { get; } = ExpectationTextEvaluationContext.For(null);

	/// <summary>
	///     The <see cref="Context" />, or <see langword="null" /> outside an evaluation.
	/// </summary>
	internal IEvaluationContext? Evaluation => _context;

	/// <summary>
	///     The context of the evaluation.
	/// </summary>
	/// <remarks>
	///     A comparison outside an evaluation, e.g. in a filter that is not part of an expectation, receives a context
	///     that stores nothing and is never completed.
	/// </remarks>
	public IEvaluationContext Context => _context ?? ContextWithoutEvaluation;

	/// <summary>
	///     The cancellation of the evaluation.
	/// </summary>
	public CancellationToken CancellationToken { get; }
}
