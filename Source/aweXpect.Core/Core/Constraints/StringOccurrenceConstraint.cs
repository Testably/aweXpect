using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Core.Constraints;

/// <summary>
///     Verifies how often the <paramref name="expected" /> <see langword="string" /> occurs in a
///     <see langword="string" /> subject, e.g. <c>contains "a" at least twice</c>.
/// </summary>
/// <remarks>
///     Return it together with a <see cref="Results.StringOccurrenceCountResult{TType,TThat}" /> that shares the
///     <paramref name="quantifier" /> and the <paramref name="options" />, and invert it for the negated expectation,
///     e.g. <c>does not contain "a"</c>.
///     <para />
///     A <see langword="null" /> subject fails the expectation and its negation.
/// </remarks>
/// <param name="it">The name of the subject.</param>
/// <param name="grammars">The grammars of the expectation.</param>
/// <param name="expected">The <see langword="string" /> whose occurrences are counted.</param>
/// <param name="quantifier">How often the <paramref name="expected" /> value has to occur.</param>
/// <param name="options">How the <paramref name="expected" /> value is matched, e.g. as a wildcard pattern.</param>
public sealed class StringOccurrenceConstraint(
	string it,
	ExpectationGrammars grammars,
	string expected,
	Quantifier quantifier,
	StringEqualityOptions options)
	: ConstraintResult(grammars),
		IAsyncContextConstraint<string?>
{
	private string? _actual;
	private int _actualCount;
	private StringEqualityOptions? _evaluationOptions;
	private bool _isFinished;
	private bool _isNegated;

	/// <inheritdoc />
	/// <remarks>
	///     The occurrences are also counted for a <see langword="null" /> subject, as in an empty one, because the
	///     options validate the expected value while they count, so an unusable pattern is rejected whichever
	///     subject it is searched in.
	/// </remarks>
	public async ValueTask<ConstraintResult> IsMetBy(string? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		_actual = actual;
		_evaluationOptions = options.ForEvaluation(context, cancellationToken);
		(_actualCount, _isFinished) =
			await StringOccurrences.Count(_evaluationOptions, actual ?? string.Empty, expected, quantifier);
		if (actual is null)
		{
			Outcome = Outcome.FailureBothWays;
			return this;
		}

		Outcome = quantifier.Check(_actualCount, true, _isNegated) ?? _isNegated ? Outcome.Success : Outcome.Failure;
		return this;
	}

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> StringOccurrences.AppendExpectation(stringBuilder, quantifier, _isNegated,
			Grammars.Verb("contains", "contain"), Grammars.Verb("does not contain", "do not contain"), expected,
			options);

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		if (!string.IsNullOrEmpty(_actual))
		{
			contexts.AddStringContext("Actual", _actual, this);
			contexts.AddStringContext("Expected", expected, this);
		}

		(_evaluationOptions ?? options).AppendContexts(contexts);
	}

	/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (_actual is TValue typedValue)
		{
			value = typedValue;
			return true;
		}

		value = default;
		return typeof(TValue).IsAssignableFrom(typeof(string));
	}

	/// <inheritdoc />
	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_actual is null)
		{
			stringBuilder.ItWasNull(it, Grammars);
			return;
		}

		StringOccurrences.AppendResult(stringBuilder, it, expected, _actual, _actualCount, _isFinished,
			_evaluationOptions ?? options, indentation);
	}

	/// <inheritdoc />
	public override ConstraintResult Negate()
	{
		_isNegated = !_isNegated;
		Outcome = Outcome switch
		{
			Outcome.Failure => Outcome.Success,
			Outcome.Success => Outcome.Failure,
			_ => Outcome,
		};
		return this;
	}
}
